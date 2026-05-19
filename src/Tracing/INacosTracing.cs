namespace Lycoris.Nacos.Extensions.Tracing
{
    /// <summary>
    /// Nacos HTTP 请求链路追踪抽象，用于集成 OpenTelemetry、SkyWalking 等追踪系统
    /// </summary>
    public interface INacosTracing
    {
        /// <summary>
        /// 开始一个追踪 Span
        /// </summary>
        /// <param name="serviceName">Nacos 服务名</param>
        /// <param name="httpMethod">HTTP 方法</param>
        /// <param name="url">请求路径</param>
        /// <returns>Span 上下文，返回 null 表示不追踪</returns>
        INacosTracingSpan? StartSpan(string serviceName, string httpMethod, string? url);
    }

    /// <summary>
    /// 追踪 Span 上下文，在请求结束后自动释放
    /// </summary>
    public interface INacosTracingSpan : IDisposable
    {
        /// <summary>
        /// 设置 Span 标签
        /// </summary>
        void SetTag(string key, string? value);

        /// <summary>
        /// 记录异常到 Span
        /// </summary>
        void SetError(Exception exception);

        /// <summary>
        /// 将追踪头传播到出站 HTTP 请求
        /// </summary>
        /// <param name="addHeader">添加请求头的回调 (key, value)</param>
        void PropagateHeaders(Action<string, string> addHeader);
    }

    /// <summary>
    /// 空追踪实现（默认，不进行任何追踪）
    /// </summary>
    internal sealed class NoopNacosTracing : INacosTracing
    {
        public INacosTracingSpan? StartSpan(string serviceName, string httpMethod, string? url) => null;
    }
}
