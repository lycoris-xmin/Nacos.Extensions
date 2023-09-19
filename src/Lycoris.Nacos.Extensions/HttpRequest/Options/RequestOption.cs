using System.Net;

namespace Lycoris.Nacos.Extensions.HttpRequest.Options
{
    internal class RequestOption
    {
        public RequestOption()
        {
            HttpHandlerOption = new HttpHandlerOption(true, false, true, true, 200);
            DangerousAcceptAnyServerCertificateValidator = true;
            AutomaticDecompression = DecompressionMethods.Brotli;
        }

        /// <summary>
        /// 超时时间(单位:秒)
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// http 处理程序的配置参数
        /// </summary>
        public HttpHandlerOption HttpHandlerOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpClientHandler? HttpMessageHandler { get; set; }

        /// <summary>
        /// 响应压缩(默认：Brotli)
        /// </summary>
        public DecompressionMethods AutomaticDecompression { get; set; }

        /// <summary>
        /// 接受任何服务器证书验证器
        /// </summary>
        public bool DangerousAcceptAnyServerCertificateValidator { get; set; }
    }
}
