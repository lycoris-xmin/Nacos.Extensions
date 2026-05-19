namespace Lycoris.Nacos.Extensions.Resilience
{
    /// <summary>
    /// 重试退避策略
    /// </summary>
    public enum RetryBackoffType
    {
        /// <summary>固定间隔</summary>
        Fixed = 0,
        /// <summary>线性增长</summary>
        Linear = 1,
        /// <summary>指数退避（默认）</summary>
        Exponential = 2
    }

    /// <summary>
    /// Nacos HTTP 请求弹性策略配置
    /// </summary>
    public class NacosResilienceOptions
    {
        /// <summary>
        /// 启用重试策略
        /// </summary>
        public bool EnableRetry { get; set; } = false;

        /// <summary>
        /// 最大重试次数，默认 3
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 重试基础延迟秒数，默认 2
        /// </summary>
        public int RetryBaseDelaySeconds { get; set; } = 2;

        /// <summary>
        /// 重试退避策略，默认指数退避
        /// </summary>
        public RetryBackoffType RetryBackoffType { get; set; } = RetryBackoffType.Exponential;

        /// <summary>
        /// 启用断路器策略
        /// </summary>
        public bool EnableCircuitBreaker { get; set; } = false;

        /// <summary>
        /// 断路器失败阈值比例（0.0 ~ 1.0），默认 0.5
        /// </summary>
        public double CircuitBreakerFailureRatio { get; set; } = 0.5;

        /// <summary>
        /// 断路器最小吞吐量，默认 5
        /// </summary>
        public int CircuitBreakerMinimumThroughput { get; set; } = 5;

        /// <summary>
        /// 断路器熔断持续时间（秒），默认 30
        /// </summary>
        public int CircuitBreakerBreakDurationSeconds { get; set; } = 30;

        /// <summary>
        /// 断路器采样时间窗口（秒），默认 60
        /// </summary>
        public int CircuitBreakerSamplingDurationSeconds { get; set; } = 60;

        /// <summary>
        /// 启用降级策略
        /// </summary>
        public bool EnableFallback { get; set; } = false;

        /// <summary>
        /// 降级响应，当所有重试失败或断路器打开时返回此值
        /// </summary>
        public Func<Task<NacosHttpResponse>>? FallbackAsync { get; set; }
    }
}
