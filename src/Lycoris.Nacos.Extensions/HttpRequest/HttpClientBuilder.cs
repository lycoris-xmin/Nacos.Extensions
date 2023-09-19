using Lycoris.Nacos.Extensions.HttpRequest.Options;
using System.Net;

namespace Lycoris.Nacos.Extensions.HttpRequest
{
    internal class HttpClientBuilder
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        private int DefaultTimeout;

        public HttpClientBuilder()
        {
            DefaultTimeout = 30;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public HttpClient Create(RequestOption option)
        {
            try
            {
                var handler = CreateHttpClientHandler(option);

                if (option.Timeout.HasValue && option.Timeout.Value >= 1)
                    DefaultTimeout = option.Timeout.Value;

                return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(DefaultTimeout) };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static HttpClientHandler CreateHttpClientHandler(RequestOption options)
        {
            var handler = options.HttpHandlerOption.UseCookieContainer
                ? UseCookiesHandler(options, options.HttpMessageHandler)
                : UseNonCookiesHandler(options, options.HttpMessageHandler);

            if (options.DangerousAcceptAnyServerCertificateValidator)
                handler.ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) => true;

            //响应压缩处理
            handler.AutomaticDecompression = options.AutomaticDecompression;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private static HttpClientHandler UseNonCookiesHandler(RequestOption route, HttpClientHandler? handler)
        {
            handler ??= new HttpClientHandler();

            handler.AllowAutoRedirect = route.HttpHandlerOption.AllowAutoRedirect;
            handler.UseCookies = route.HttpHandlerOption.UseCookieContainer;
            handler.UseProxy = route.HttpHandlerOption.UseProxy;
            handler.MaxConnectionsPerServer = route.HttpHandlerOption.MaxConnectionsPerServer;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private static HttpClientHandler UseCookiesHandler(RequestOption route, HttpClientHandler? handler)
        {
            handler ??= new HttpClientHandler();

            handler.AllowAutoRedirect = route.HttpHandlerOption.AllowAutoRedirect;
            handler.UseCookies = route.HttpHandlerOption.UseCookieContainer;
            handler.UseProxy = route.HttpHandlerOption.UseProxy;
            handler.MaxConnectionsPerServer = route.HttpHandlerOption.MaxConnectionsPerServer;
            handler.CookieContainer = route.HttpHandlerOption.Cookies ?? new CookieContainer();

            return handler;
        }
    }
}
