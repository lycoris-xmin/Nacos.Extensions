using System.Diagnostics;

namespace Lycoris.Nacos.Extensions.Tracing
{
    /// <summary>
    /// 基于 System.Diagnostics.Activity 的追踪实现，自动兼容 OpenTelemetry SDK
    /// </summary>
    public class NacosActivityTracing : INacosTracing
    {
        private static readonly ActivitySource Source = new("Lycoris.Nacos.Extensions", "6.1.1");

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

            return new Span(activity);
        }

        private sealed class Span : INacosTracingSpan
        {
            private readonly Activity _activity;

            public Span(Activity activity) => _activity = activity;

            public void SetTag(string key, string? value) => _activity.SetTag(key, value);

            public void SetError(Exception exception)
            {
                _activity.SetStatus(ActivityStatusCode.Error, exception.Message);
                _activity.SetTag("exception.type", exception.GetType().FullName);
                _activity.SetTag("exception.message", exception.Message);
            }

            public void PropagateHeaders(Action<string, string> addHeader)
            {
                // W3C trace context 由 HttpClient 自动传播，无需手动处理
                // 如需自定义头传播，在此实现
            }

            public void Dispose() => _activity.Dispose();
        }
    }
}
