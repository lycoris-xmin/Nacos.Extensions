using Lycoris.Base.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosHttpClientLogger : INacosHttpClientLogger
    {
        private readonly ILogger _logger;
        private readonly NacosHttpClientConfig config;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="options"></param>
        public NacosHttpClientLogger(ILoggerFactory factory, IOptions<NacosHttpClientConfig> options)
        {
            _logger = factory.CreateLogger<NacosHttpClient>();
            config = options.Value;
        }

        /// <summary>
        /// 请求日志记录
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestMessage"></param>
        /// <param name="content"></param>
        public void RequestLog(string? traceId, string requestId, HttpRequestMessage requestMessage, string? content = null)
        {
            if (!config.EnableLogger)
                return;

            try
            {
                _logger?.LogInformation($"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - request {requestMessage.Method} {requestMessage.RequestUri!} {requestMessage.Version}");

                RequestHeaderFilter(traceId, requestId, requestMessage);

                if (!string.IsNullOrEmpty(content))
                {
                    _logger?.LogInformation($"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - request body: {content}");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - request logging failed");
            }
        }

        /// <summary>
        /// 请求头部过滤
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestMessage"></param>
        private void RequestHeaderFilter(string? traceId, string requestId, HttpRequestMessage requestMessage)
        {
            if (!config.AllowAllHeaderFilter && (config.HeaderFilter == null || !config.HeaderFilter.Any()))
                return;

            var headers = new Dictionary<string, string>();
            if (requestMessage.Headers != null && requestMessage.Headers.Any())
            {
                foreach (var item in requestMessage.Headers)
                {
                    var val = item.Value != null && item.Value.Any() ? JsonConvert.SerializeObject(item.Value.ToList()) : "";

                    if (config.AllowAllHeaderFilter)
                        headers.TryAdd(item.Key, val);
                    else if (config.HeaderFilter != null && config.HeaderFilter.Any(x => x.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase)))
                        headers.TryAdd(item.Key, val);
                }
            }

            if (requestMessage.Content != null && requestMessage.Content.Headers != null && requestMessage.Content.Headers.Any())
            {
                foreach (var item in requestMessage.Content.Headers)
                {
                    var val = item.Value != null && item.Value.Any() ? JsonConvert.SerializeObject(item.Value.ToList()) : "";

                    if (config.AllowAllHeaderFilter)
                        headers.TryAdd(item.Key, val);
                    else if (config.HeaderFilter != null && config.HeaderFilter.Any(x => x.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase)))
                        headers.TryAdd(item.Key, val);
                }
            }

            //

            if (headers.Count > 0)
                RequestHeaderLog(traceId, requestId, headers);
        }

        /// <summary>
        /// 请求头日志记录
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="headers"></param>
        private void RequestHeaderLog(string? traceId, string requestId, Dictionary<string, string> headers)
        {
            var sb = new StringBuilder();

            foreach (var (key, value) in headers)
            {
                sb.AppendFormat("{0}:{1};", key, value);
            }

            _logger?.LogInformation($"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - response headers:[{sb.ToString().TrimEnd(';')}]");
        }

        /// <summary>
        /// 响应日志记录
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="response"></param>
        public void ResponseLog(string? traceId, string requestId, NacosHttpResponse response)
        {
            if (!config.EnableLogger)
                return;

            try
            {
                ResponseHeaderFilter(traceId, requestId, response);

                if (response.Success)
                {
                    _logger?.LogInformation($"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - response body: {response.Content} - statuscode: {response.HttpStatusCode}");
                }
                else
                {
                    _logger?.LogError($"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - response exceotion:{response.Exception!.Message}\r\n{response.Exception!.StackTrace}");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - response logging failed");
            }
        }

        /// <summary>
        /// 响应头过滤
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="response"></param>
        private void ResponseHeaderFilter(string? traceId, string requestId, NacosHttpResponse response)
        {
            if (response.HttpResponseMessage == null)
                return;

            if (!config.AllowAllHeaderFilter && (config.HeaderFilter == null || !config.HeaderFilter.Any()))
                return;

            var headers = new Dictionary<string, string>();
            if (response.HttpResponseMessage.Headers != null && response.HttpResponseMessage.Headers.Any())
            {
                foreach (var item in response.HttpResponseMessage.Headers)
                {
                    var val = item.Value != null && item.Value.Any() ? JsonConvert.SerializeObject(item.Value.ToList()) : "";

                    if (config.AllowAllHeaderFilter)
                        headers.TryAdd(item.Key, val);
                    else if (config.HeaderFilter != null && config.HeaderFilter.Any(x => x.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase)))
                        headers.TryAdd(item.Key, val);
                }
            }

            if (headers.Count > 0)
                ResponseHeaderLog(traceId, requestId, headers);
        }

        /// <summary>
        /// 响应头日志记录
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="requestId"></param>
        /// <param name="headers"></param>
        private void ResponseHeaderLog(string? traceId, string requestId, Dictionary<string, string> headers)
        {
            var sb = new StringBuilder();

            foreach (var (key, value) in headers)
            {
                sb.AppendFormat("{0}:{1};", key, value);
            }

            _logger?.LogInformation($"{GetTraceId(traceId)}NacosHttpClient[{requestId}] - response headers:[{sb.ToString().TrimEnd(';')}]");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceId"></param>
        /// <returns></returns>
        private static string GetTraceId(string? traceId) => traceId.IsNullOrEmpty() ? "" : $"{traceId} - ";
    }
}
