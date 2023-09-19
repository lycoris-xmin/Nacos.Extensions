using Lycoris.Base.Extensions;
using Lycoris.Nacos.Extensions.HttpRequest.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosHttpRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string? TraceId { get; set; } = null;

        /// <summary>
        /// 请求路径，不包含 'BaseUrl
        /// 例子：
        /// 实际请求路径：http://ip:port/admin/login 
        /// 填写路径：/admin/login 
        /// http://ip:port 由扩展服务根据'groupName'与'serviceName'从nacos服务中获取
        /// </summary>
        public string? Url { get; set; } = null;

        /// <summary>
        /// 请求方法，默认为 'Get'
        /// </summary>
        public HttpMethod? HttpMethod { get; set; } = null;

        /// <summary>
        /// 超时时间(单位:秒，默认3秒)
        /// </summary>
        public int Timeout { get; set; } = 3;

        /// <summary>
        /// 请求客服务
        /// </summary>
        public string? UserAgent { get; set; } = null;

        /// <summary>
        /// ContentType
        /// Post、Put、Delete 默认为 'application/json'
        /// </summary>
        public string? ContentType { get; set; } = null;

        /// <summary>
        /// 响应字符集编码，默认为UTF-8
        /// </summary>
        public Encoding ResponseEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 响应压缩(默认：Brotli)
        /// </summary>
        public DecompressionMethods AutomaticDecompression { get; set; } = DecompressionMethods.Brotli;

        /// <summary>
        /// 接受任何服务器证书验证器
        /// </summary>
        public bool DangerousAcceptAnyServerCertificateValidator { get; set; } = true;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="BaseUrl"></param>
        public NacosHttpRequest(string BaseUrl)
        {
            this.BaseUrl = BaseUrl;
        }

        internal string BaseUrl { get; }

        /// <summary>
        /// 
        /// </summary>
        internal string? Querying { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string? ContentBody { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal List<(string key, string value)>? ContentForm { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        internal Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        internal CookieContainer? CookieContainer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal StringContent? Body { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        internal MultipartFormDataContent? Form { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="querying"></param>
        public void AddQuerying(string querying)
            => this.Querying = querying;

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeader(string key, string value)
            => Headers.Add(key, value);

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="values"></param>
        public void AddHeaders(params (string key, string value)[] values)
        {
            foreach (var (key, value) in values)
            {
                if (this.Headers.ContainsKey(key))
                    this.Headers[key] = value;
                else
                    this.Headers.Add(key, value);
            }
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="headers"></param>
        public void AddHeaders([NotNull] Dictionary<string, string> headers)
        {
            foreach (var item in headers)
            {
                if (this.Headers.ContainsKey(item.Key))
                    this.Headers[item.Key] = item.Value;
                else
                    this.Headers.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        public void AddCookie(string cookieName, string cookieValue)
        {
            CookieContainer ??= new CookieContainer();
            CookieContainer.Add(new Cookie(cookieName, cookieValue));
        }

        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        public void AddCookie([NotNull] string path, string cookieName, string cookieValue)
        {
            CookieContainer ??= new CookieContainer();
            CookieContainer.Add(new Uri($"{BaseUrl.TrimEnd('/')}/{path.TrimStart('/')}"), new Cookie(cookieName, cookieValue));
        }

        /// <summary>
        /// 添加多个Cookies
        /// </summary>
        /// <param name="cookies"></param>
        public void AddCookies([NotNull] Dictionary<string, string> cookies)
        {
            CookieContainer ??= new CookieContainer();
            foreach (var item in cookies)
            {
                CookieContainer.Add(new Cookie(item.Key, item.Value));
            }
        }

        /// <summary>
        /// 添加多个Cookies
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cookies"></param>
        public void AddCookies([NotNull] string path, [NotNull] Dictionary<string, string> cookies)
        {
            CookieContainer ??= new CookieContainer();
            var uri = new Uri($"{BaseUrl.TrimEnd('/')}/{path.TrimStart('/')}");
            foreach (var item in cookies)
            {
                CookieContainer.Add(uri, new Cookie(item.Key, item.Value));
            }
        }

        /// <summary>
        /// 添加请求体
        /// </summary>
        /// <param name="body"></param>
        /// <param name="encoding"></param>
        public void AddJsonBody<T>(T body, Encoding? encoding = null) where T : class
        {
            this.ContentBody = body.ToJson();
            Body = new StringContent(this.ContentBody, encoding);
        }

        /// <summary>
        /// 添加请求体
        /// </summary>
        /// <param name="body"></param>
        /// <param name="encoding"></param>
        public void AddJsonBody(string body, Encoding? encoding = null)
        {
            Body = new StringContent(body, encoding);
            this.ContentBody = body;
        }

        /// <summary>
        /// 添加表单
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        public void AddFormData(string key, string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            Form ??= new MultipartFormDataContent();
            Form.Add(new StringContent(value, encoding), key);

            ContentForm ??= new List<(string key, string value)>();
            ContentForm.Add((key, value));
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public void AddFormFile(string key, string filePath, string? fileName = null)
        {
            var _fileName = Path.GetFileName(filePath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"cannot find the file by path:{filePath}", _fileName);

            var bytes = File.ReadAllBytes(filePath);

            Form ??= new MultipartFormDataContent();

            if (fileName.IsNullOrEmpty())
                Form.Add(new ByteArrayContent(bytes), key);
            else
                Form.Add(new ByteArrayContent(bytes), key, fileName!);

            ContentForm ??= new List<(string key, string value)>();
            ContentForm.Add((key, filePath));
        }

        /// <summary>
        /// 构建请求
        /// </summary>
        /// <returns></returns>
        internal HttpRequestMessage BuildHttpRequestMessage()
        {
            if (this.Body != null && this.Form != null)
                throw new Exception("");

            this.TraceId ??= Guid.NewGuid().ToString("N");

            var url = new StringBuilder(this.BaseUrl);
            if (!string.IsNullOrEmpty(this.Url))
                url.AppendFormat("/{0}", this.Url.TrimStart('/'));
            if (!string.IsNullOrEmpty(this.Querying))
                url.AppendFormat("?{0}", this.Querying.TrimStart('?'));

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url.ToString()),
                Method = this.HttpMethod ?? HttpMethod.Get
            };

            if (this.Body != null)
            {
                request.Content = this.Body;
                if (!string.IsNullOrEmpty(this.ContentType))
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
                else
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            else if (this.Form != null)
            {
                request.Content = this.Form;
                foreach (var (key, value) in this.ContentForm!)
                {
                    this.ContentBody += $"{key}={value};";
                }
            }

            if (this.Headers.Count > 0)
            {
                if (request.Content != null)
                {
                    foreach (var item in this.Headers)
                        request.Content.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
                else
                {
                    foreach (var item in this.Headers)
                        request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal RequestOption? BuildHttpClientHandler()
        {
            var requestOption = new RequestOption()
            {
                Timeout = this.Timeout,
                HttpHandlerOption = new HttpHandlerOption(true, this.CookieContainer != null, true, true, 200)
                {
                    Cookies = this.CookieContainer ?? new CookieContainer()
                },
                AutomaticDecompression = this.AutomaticDecompression,
                DangerousAcceptAnyServerCertificateValidator = this.DangerousAcceptAnyServerCertificateValidator
            };

            return requestOption;
        }
    }
}
