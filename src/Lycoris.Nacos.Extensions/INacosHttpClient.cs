namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public interface INacosHttpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceId"></param>
        void SetTraceId(string traceId);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        NacosHttpResponse HttpGet(string groupName, string serviceName);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        NacosHttpResponse HttpGet(string groupName, string serviceName, string? querying = null);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        NacosHttpResponse HttpGet(string groupName, string serviceName, string url, string? querying = null);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpGetAsync(string groupName, string serviceName);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpGetAsync(string groupName, string serviceName, string? querying = null);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpGetAsync(string groupName, string serviceName, string url, string? querying = null);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        NacosHttpResponse HttpPost(string groupName, string serviceName, string? body = null);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        NacosHttpResponse HttpPost(string groupName, string serviceName, string? url = null, string? body = null);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpPostAsync(string groupName, string serviceName, string? body = null);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpPostAsync(string groupName, string serviceName, string? url = null, string? body = null);

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        NacosHttpResponse HttpPut(string groupName, string serviceName, string? body = null);

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        NacosHttpResponse HttpPut(string groupName, string serviceName, string? url = null, string? body = null);

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpPutAsync(string groupName, string serviceName, string? body = null);

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpPutAsync(string groupName, string serviceName, string? url = null, string? body = null);

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        NacosHttpResponse HttpDelete(string groupName, string serviceName, string? body = null);

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        NacosHttpResponse HttpDelete(string groupName, string serviceName, string? url = null, string? body = null);

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpDeleteAsync(string groupName, string serviceName, string? body = null);

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpDeleteAsync(string groupName, string serviceName, string? url = null, string? body = null);

        /// <summary>
        /// Http请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        NacosHttpResponse HttpRequest(string groupName, string serviceName, Action<NacosHttpRequest> configure);

        /// <summary>
        /// Http请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<NacosHttpResponse> HttpRequestAsync(string groupName, string serviceName, Action<NacosHttpRequest> configure);

        /// <summary>
        /// 批量请求
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        Task<NacosHttpResponse[]?> MultipleHttpRequestAsync(params NacosMultipleHttpRequest[] requests);
    }
}