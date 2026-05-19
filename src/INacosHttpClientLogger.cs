namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public interface INacosHttpClientLogger
    {
        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestMessage"></param>
        /// <param name="content"></param>
        void RequestLog(string? traceId, string requestId, HttpRequestMessage requestMessage, string? content = null);

        /// <summary>
        /// 响应日志
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="response"></param>
        void ResponseLog(string? traceId, string requestId, NacosHttpResponse response);
    }
}
