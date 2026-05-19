using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace Lycoris.Nacos.Extensions.Resilience
{
    /// <summary>
    /// 基于 Polly 的弹性策略管道
    /// </summary>
    public sealed class NacosResiliencePipeline
    {
        private readonly ResiliencePipeline<NacosHttpResponse>? _pipeline;
        private readonly NacosResilienceOptions _options;
        private readonly object _circuitStateLock = new();
        private CircuitState _circuitState = CircuitState.Closed;

        /// <summary>
        /// 当前断路器状态
        /// </summary>
        public CircuitState CircuitState
        {
            get { lock (_circuitStateLock) return _circuitState; }
            private set { lock (_circuitStateLock) _circuitState = value; }
        }

        /// <summary>
        /// 断路器是否打开（熔断中）
        /// </summary>
        public bool IsCircuitOpen => CircuitState == CircuitState.Open;

        private NacosResiliencePipeline(ResiliencePipeline<NacosHttpResponse>? pipeline, NacosResilienceOptions options)
        {
            _pipeline = pipeline;
            _options = options;
        }

        /// <summary>
        /// 创建弹性策略管道
        /// </summary>
        public static NacosResiliencePipeline Create(NacosResilienceOptions options)
        {
            if (!options.EnableRetry && !options.EnableCircuitBreaker && !options.EnableFallback)
                return new NacosResiliencePipeline(null, options);

            var builder = new ResiliencePipelineBuilder<NacosHttpResponse>();

            // 断路器（最内层，优先判断）
            if (options.EnableCircuitBreaker)
            {
                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<NacosHttpResponse>
                {
                    ShouldHandle = new PredicateBuilder<NacosHttpResponse>()
                        .Handle<Exception>()
                        .HandleResult(r => !r.Success),
                    FailureRatio = options.CircuitBreakerFailureRatio,
                    MinimumThroughput = options.CircuitBreakerMinimumThroughput,
                    BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakerBreakDurationSeconds),
                    SamplingDuration = TimeSpan.FromSeconds(options.CircuitBreakerSamplingDurationSeconds),
                    OnOpened = _ => { /* 断路器打开 */ return default; },
                    OnClosed = _ => { /* 断路器关闭 */ return default; },
                    OnHalfOpened = _ => { /* 断路器半开 */ return default; }
                });
            }

            // 重试（中间层）
            if (options.EnableRetry)
            {
                builder.AddRetry(new RetryStrategyOptions<NacosHttpResponse>
                {
                    ShouldHandle = new PredicateBuilder<NacosHttpResponse>()
                        .Handle<Exception>()
                        .HandleResult(r => !r.Success),
                    MaxRetryAttempts = options.RetryCount,
                    BackoffType = options.RetryBackoffType switch
                    {
                        RetryBackoffType.Linear => DelayBackoffType.Linear,
                        RetryBackoffType.Exponential => DelayBackoffType.Exponential,
                        _ => DelayBackoffType.Constant
                    },
                    Delay = TimeSpan.FromSeconds(options.RetryBaseDelaySeconds),
                    OnRetry = args =>
                    {
                        return default;
                    }
                });
            }

            // 降级（最外层，兜底）
            if (options.EnableFallback && options.FallbackAsync != null)
            {
                var fallbackAction = options.FallbackAsync;
                builder.AddFallback(new FallbackStrategyOptions<NacosHttpResponse>
                {
                    ShouldHandle = new PredicateBuilder<NacosHttpResponse>()
                        .Handle<Exception>()
                        .HandleResult(r => !r.Success),
                    FallbackAction = async _ =>
                    {
                        var response = await fallbackAction();
                        return Outcome.FromResult(response);
                    }
                });
            }

            return new NacosResiliencePipeline(builder.Build(), options);
        }

        /// <summary>
        /// 通过弹性管道执行请求
        /// </summary>
        public async Task<NacosHttpResponse> ExecuteAsync(
            Func<Task<NacosHttpResponse>> action,
            CancellationToken cancellationToken = default)
        {
            if (_pipeline == null)
                return await action();

            return await _pipeline.ExecuteAsync(async _ => await action(), cancellationToken);
        }
    }

    /// <summary>
    /// 断路器状态
    /// </summary>
    public enum CircuitState
    {
        /// <summary>关闭（正常）</summary>
        Closed = 0,
        /// <summary>打开（熔断）</summary>
        Open = 1,
        /// <summary>半开（探测恢复）</summary>
        HalfOpen = 2
    }
}
