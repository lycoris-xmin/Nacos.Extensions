using Lycoris.Nacos.Extensions.Exceptions;
using Lycoris.Nacos.Extensions.HttpRequest;
using Lycoris.Nacos.Extensions.HttpRequest.Options;
using Lycoris.Nacos.Extensions.Resilience;
using Lycoris.Nacos.Extensions.Tracing;
using Microsoft.Extensions.DependencyInjection;
using Nacos.V2;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosHttpClient : INacosHttpClient
    {
        private const string HTTPS = "https";
        private const string HTTP = "http";
        private const string Secure = "secure";

        private readonly INacosNamingService _nacosNamingService;
        private readonly INacosHttpClientLogger? _logger;
        private readonly INacosTracing? _tracing;
        private readonly NacosResiliencePipeline? _resilience;

        /// <summary>
        /// 请求唯一标识
        /// </summary>
        private string? TraceId { get; set; } = null;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="provider"></param>
        public NacosHttpClient(IServiceProvider provider)
        {
            _nacosNamingService = provider.GetService<INacosNamingService>()!;
            _logger = provider.GetService<INacosHttpClientLogger>();
            _tracing = provider.GetService<INacosTracing>();
            _resilience = provider.GetService<NacosResiliencePipeline>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceId"></param>
        public void SetTraceId(string traceId) => TraceId = traceId;

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpGet(string groupName, string serviceName) => HttpGet(groupName, serviceName, null, null);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpGet(string groupName, string serviceName, string? querying = null) => HttpGet(groupName, serviceName, null, querying);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpGet(string groupName, string serviceName, string? url = null, string? querying = null)
        {
            var host = GetHealthyInstanceAsync(groupName, serviceName, url, querying).GetAwaiter().GetResult();

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Get);

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request);

            var response = HttpRequest(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpGetAsync(string groupName, string serviceName) => await HttpGetAsync(groupName, serviceName, null, null);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpGetAsync(string groupName, string serviceName, string? querying = null) => await HttpGetAsync(groupName, serviceName, null, querying);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpGetAsync(string groupName, string serviceName, string? url = null, string? querying = null)
        {
            var host = await GetHealthyInstanceAsync(groupName, serviceName, url, querying);

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Get);

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request);

            var response = await HttpRequestAsync(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpPost(string groupName, string serviceName, string? body = null) => HttpPost(groupName, serviceName, null, body);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpPost(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = GetHealthyInstanceAsync(groupName, serviceName, url).GetAwaiter().GetResult();

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Post, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request);

            var response = HttpRequest(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpPostAsync(string groupName, string serviceName, string? body = null) => await HttpPostAsync(groupName, serviceName, null, body);

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpPostAsync(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = await GetHealthyInstanceAsync(groupName, serviceName, url);

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Post, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = await HttpRequestAsync(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpPut(string groupName, string serviceName, string? body = null) => HttpPut(groupName, serviceName, null, body);

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpPut(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = GetHealthyInstanceAsync(groupName, serviceName, url).GetAwaiter().GetResult();

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Put, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = HttpRequest(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpPutAsync(string groupName, string serviceName, string? body = null) => await HttpPutAsync(groupName, serviceName, null, body);

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpPutAsync(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = await GetHealthyInstanceAsync(groupName, serviceName, url);

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Put, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = await HttpRequestAsync(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpPatch(string groupName, string serviceName, string? body = null) => HttpPatch(groupName, serviceName, null, body);

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpPatch(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = GetHealthyInstanceAsync(groupName, serviceName, url).GetAwaiter().GetResult();

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Patch, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = HttpRequest(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpPatchAsync(string groupName, string serviceName, string? body = null) => await HttpPatchAsync(groupName, serviceName, null, body);

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpPatchAsync(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = await GetHealthyInstanceAsync(groupName, serviceName, url);

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Patch, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = await HttpRequestAsync(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpDelete(string groupName, string serviceName, string? body = null) => HttpDelete(groupName, serviceName, null, body);

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpDelete(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = GetHealthyInstanceAsync(groupName, serviceName, url).GetAwaiter().GetResult();

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Delete, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = HttpRequest(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpDeleteAsync(string groupName, string serviceName, string? body = null) => await HttpDeleteAsync(groupName, serviceName, null, body);

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpDeleteAsync(string groupName, string serviceName, string? url = null, string? body = null)
        {
            var host = await GetHealthyInstanceAsync(groupName, serviceName, url);

            var request = DefaultMapHttpRequestMessage(host, HttpMethod.Delete, body);

            if (!string.IsNullOrEmpty(body) && request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var requestId = Guid.NewGuid().ToString("N");

            _logger?.RequestLog(TraceId, requestId, request, body);

            var response = await HttpRequestAsync(request);

            _logger?.ResponseLog(TraceId, requestId, response);

            return response;
        }

        /// <summary>
        /// Http请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public NacosHttpResponse HttpRequest(string groupName, string serviceName, Action<NacosHttpRequest> configure)
        {
            var span = _tracing?.StartSpan(serviceName, "CUSTOM", null);
            try
            {
                var host = GetHealthyInstanceAsync(groupName, serviceName).GetAwaiter().GetResult();

                var option = new NacosHttpRequest(host);
                span?.PropagateHeaders((key, value) => option.AddHeader(key, value));
                configure(option);

                span?.SetTag("nacos.group", groupName);
                span?.SetTag("http.url", option.Url ?? "");
                span?.SetTag("http.method", option.HttpMethod?.Method ?? "GET");

                var request = option.BuildHttpRequestMessage();
                var requestOption = option.BuildHttpClientHandler();
                var requestId = Guid.NewGuid().ToString("N");

                _logger?.RequestLog(!string.IsNullOrEmpty(option.TraceId) ? option.TraceId : TraceId, requestId, request, option.ContentBody);

                var response = _resilience != null
                    ? _resilience.ExecuteAsync(async () => await HttpRequestAsync(request, requestOption, option.ResponseEncoding)).GetAwaiter().GetResult()
                    : HttpRequest(request, requestOption, option.ResponseEncoding);

                span?.SetTag("http.status_code", ((int)response.HttpStatusCode).ToString());
                if (!response.Success && response.Exception != null)
                    span?.SetError(response.Exception);

                _logger?.ResponseLog(!string.IsNullOrEmpty(option.TraceId) ? option.TraceId : TraceId, requestId, response);

                return response;
            }
            finally
            {
                span?.Dispose();
            }
        }

        /// <summary>
        /// Http请求
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse> HttpRequestAsync(string groupName, string serviceName, Action<NacosHttpRequest> configure)
        {
            var span = _tracing?.StartSpan(serviceName, "CUSTOM", null);
            try
            {
                var host = await GetHealthyInstanceAsync(groupName, serviceName);

                var option = new NacosHttpRequest(host);
                span?.PropagateHeaders((key, value) => option.AddHeader(key, value));
                configure(option);

                span?.SetTag("nacos.group", groupName);
                span?.SetTag("http.url", option.Url ?? "");
                span?.SetTag("http.method", option.HttpMethod?.Method ?? "GET");

                var request = option.BuildHttpRequestMessage();
                var requestOption = option.BuildHttpClientHandler();
                var requestId = Guid.NewGuid().ToString("N");

                _logger?.RequestLog(!string.IsNullOrEmpty(option.TraceId) ? option.TraceId : TraceId, requestId, request, option.ContentBody);

                var response = _resilience != null
                    ? await _resilience.ExecuteAsync(async () => await HttpRequestAsync(request, requestOption, option.ResponseEncoding))
                    : await HttpRequestAsync(request, requestOption, option.ResponseEncoding);

                span?.SetTag("http.status_code", ((int)response.HttpStatusCode).ToString());
                if (!response.Success && response.Exception != null)
                    span?.SetError(response.Exception);

                _logger?.ResponseLog(!string.IsNullOrEmpty(option.TraceId) ? option.TraceId : TraceId, requestId, response);

                return response;
            }
            finally
            {
                span?.Dispose();
            }
        }

        /// <summary>
        /// 批量请求
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task<NacosHttpResponse[]> MultipleHttpRequestAsync(params NacosMultipleHttpRequest[] requests)
        {
            if (requests == null || requests.Length == 0)
                return Array.Empty<NacosHttpResponse>();

            for (int i = 0; i < requests.Length; i++)
            {
                var req = requests[i];
                if (string.IsNullOrEmpty(req.GroupName))
                    throw new MultipleHttpRequestException();
                if (string.IsNullOrEmpty(req.ServiceName))
                    throw new MultipleHttpRequestException(req.GroupName!);
                if (req.Option == null)
                    throw new MultipleHttpRequestException(req.GroupName!, req.ServiceName!);
            }

            var tasks = new Task<NacosHttpResponse>[requests.Length];
            for (int i = 0; i < requests.Length; i++)
            {
                tasks[i] = HttpRequestAsync(requests[i].GroupName!, requests[i].ServiceName!, requests[i].Option!);
            }

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取对应服务集群的健康实例地址
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="querying"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> GetHealthyInstanceAsync(string groupName, string serviceName, string? url = null, string? querying = null)
        {
            var instance = await _nacosNamingService.SelectOneHealthyInstance(serviceName, groupName).ConfigureAwait(false) ?? throw new NoHealthyInstanceException(groupName, serviceName);

            var host = $"{instance.Ip}:{instance.Port}";

            var instanceUrl = (instance.Metadata.TryGetValue(Secure, out _) ? $"{HTTPS}://{host}" : $"{HTTP}://{host}") ?? "";

            if (string.IsNullOrEmpty(instanceUrl))
                throw new Exception("");

            var tmp = new StringBuilder(instanceUrl.TrimEnd('/'));
            if (!string.IsNullOrEmpty(url))
                tmp.AppendFormat("/{0}", url.TrimStart('/'));
            if (!string.IsNullOrEmpty(querying))
                tmp.AppendFormat("?{0}", querying.TrimStart('?'));

            return tmp.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static HttpRequestMessage DefaultMapHttpRequestMessage(string url, HttpMethod method, string? body = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = method
            };

            if (!string.IsNullOrEmpty(body))
                request.Content = new StringContent(body);

            return request;
        }

        /// <summary>
        /// 发送Http请求
        /// </summary>
        /// <param name="options"></param>
        /// <param name="encoding"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static NacosHttpResponse HttpRequest(HttpRequestMessage request, RequestOption? options = null, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var builder = new HttpClientBuilder();

            try
            {
                options ??= new RequestOption();

                AddDefaultHeader(request);

                var res = builder.Create(options).Send(request);

                var result = new NacosHttpResponse(res);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;

                if (res.Content != null)
                {
                    if (encoding == Encoding.UTF8)
                        result.Content = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    else
                    {
                        var bytes = res.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        result.Content = HttpUtility.UrlDecode(bytes, encoding);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new NacosHttpResponse(ex);
            }
        }

        /// <summary>
        /// 发送Http请求
        /// </summary>
        /// <param name="options"></param>
        /// <param name="encoding"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async Task<NacosHttpResponse> HttpRequestAsync(HttpRequestMessage request, RequestOption? options = null, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var builder = new HttpClientBuilder();

            try
            {
                options ??= new RequestOption();

                AddDefaultHeader(request);

                var res = await builder.Create(options).SendAsync(request);

                var result = new NacosHttpResponse(res);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;

                if (res.Content != null)
                {
                    if (encoding == Encoding.UTF8)
                        result.Content = await res.Content.ReadAsStringAsync();
                    else
                    {
                        var bytes = await res.Content.ReadAsByteArrayAsync();
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        result.Content = HttpUtility.UrlDecode(bytes, encoding);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new NacosHttpResponse(ex);
            }
        }

        /// <summary>
        /// 添加默认请求头
        /// </summary>
        private static void AddDefaultHeader(HttpRequestMessage request)
        {
            if (request.Content == null)
                request.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");
            else
                request.Content.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");

            if (request.Content == null)
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
            else
                request.Content.Headers.TryAddWithoutValidation("Accept", "application/json");
        }
    }
}
