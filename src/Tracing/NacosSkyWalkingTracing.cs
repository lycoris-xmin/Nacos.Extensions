using System.Diagnostics;
using System.Text;

namespace Lycoris.Nacos.Extensions.Tracing
{
    /// <summary>
    /// SkyWalking 追踪实现，自动传播 sw8 请求头
    /// <para>
    /// 适用于使用了 SkyWalking .NET Agent 的场景，
    /// 通过 System.Diagnostics.Activity 读取 SkyWalking 注入的 trace 信息并传播到 Nacos HTTP 请求
    /// </para>
    /// </summary>
    public class NacosSkyWalkingTracing : INacosTracing
    {
        private static readonly ActivitySource Source = new("Lycoris.Nacos.Extensions", "6.1.1");

        /// <summary>
        /// 当前 SkyWalking trace 上下文（由 SkyWalking Agent 设置的环境变量或 Activity Baggage）
        /// </summary>
        public static Func<string?>? TraceContextProvider { get; set; }

        /// <inheritdoc />
        public INacosTracingSpan? StartSpan(string serviceName, string httpMethod, string? url)
        {
            var activity = Source.StartActivity($"HTTP {httpMethod} {serviceName}");
            if (activity == null)
                return null;

            activity.SetTag("nacos.service", serviceName);
            activity.SetTag("http.method", httpMethod);
            if (url != null)
                activity.SetTag("http.url", url);

            return new SkyWalkingSpan(activity);
        }

        private sealed class SkyWalkingSpan : INacosTracingSpan
        {
            private readonly Activity _activity;

            public SkyWalkingSpan(Activity activity) => _activity = activity;

            public void SetTag(string key, string? value) => _activity.SetTag(key, value);

            public void SetError(Exception exception)
            {
                _activity.SetStatus(ActivityStatusCode.Error, exception.Message);
                _activity.SetTag("exception.type", exception.GetType().FullName);
                _activity.SetTag("exception.message", exception.Message);
            }

            public void PropagateHeaders(Action<string, string> addHeader)
            {
                // 传播 SkyWalking sw8 头
                var traceContext = TraceContextProvider?.Invoke();
                if (!string.IsNullOrEmpty(traceContext))
                    addHeader("sw8", traceContext!);

                // 传播 W3C traceparent（兼容 OpenTelemetry）
                var traceParent = _activity.Id;
                if (!string.IsNullOrEmpty(traceParent) && traceParent!.StartsWith("00-"))
                    addHeader("traceparent", traceParent);
            }

            public void Dispose() => _activity.Dispose();
        }
    }

    /// <summary>
    /// SkyWalking sw8 头构建工具
    /// </summary>
    public static class SkyWalkingHeaderBuilder
    {
        /// <summary>
        /// 构建 sw8 请求头值
        /// </summary>
        /// <param name="traceId">SkyWalking traceId</param>
        /// <param name="segmentId">Segment ID</param>
        /// <param name="spanId">Span ID</param>
        /// <param name="serviceName">当前服务名</param>
        /// <param name="serviceInstance">当前服务实例</param>
        /// <param name="endpoint">当前端点路径</param>
        /// <param name="peer">目标服务名（Nacos 服务名）</param>
        /// <returns>Base64 编码的 sw8 头值</returns>
        public static string BuildSw8Header(
            string traceId,
            string segmentId,
            int spanId,
            string serviceName,
            string serviceInstance,
            string endpoint,
            string peer)
        {
            var payload = $"1-{traceId}-{segmentId}-{spanId}-{serviceName}-{serviceInstance}-{endpoint}-{peer}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        }
    }
}
