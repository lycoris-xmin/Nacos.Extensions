using Lycoris.Nacos.Extensions.HttpRequest.Options;
using System.Net;

namespace Lycoris.Nacos.Extensions.HttpRequest
{
    internal class HttpClientBuilder
    {
        private static readonly HttpClientHandler DefaultHandler = new()
        {
            AllowAutoRedirect = true,
            UseProxy = true,
            MaxConnectionsPerServer = 200,
            AutomaticDecompression = DecompressionMethods.Brotli,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        public HttpClient Create(RequestOption option)
        {
            var handler = CreateHttpClientHandler(option);
            var timeout = (option.Timeout.HasValue && option.Timeout.Value >= 1) ? option.Timeout.Value : 30;
            return new HttpClient(handler, disposeHandler: false) { Timeout = TimeSpan.FromSeconds(timeout) };
        }

        private static HttpMessageHandler CreateHttpClientHandler(RequestOption options)
        {
            // Use the shared static handler when no cookies are needed and defaults match
            if (!options.HttpHandlerOption.UseCookieContainer
                && options.AutomaticDecompression == DecompressionMethods.Brotli
                && options.DangerousAcceptAnyServerCertificateValidator)
            {
                return DefaultHandler;
            }

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = options.HttpHandlerOption.AllowAutoRedirect,
                UseCookies = options.HttpHandlerOption.UseCookieContainer,
                UseProxy = options.HttpHandlerOption.UseProxy,
                MaxConnectionsPerServer = options.HttpHandlerOption.MaxConnectionsPerServer,
                AutomaticDecompression = options.AutomaticDecompression
            };

            if (options.DangerousAcceptAnyServerCertificateValidator)
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            if (options.HttpHandlerOption.UseCookieContainer)
                handler.CookieContainer = options.HttpHandlerOption.Cookies ?? new CookieContainer();

            return handler;
        }
    }
}
