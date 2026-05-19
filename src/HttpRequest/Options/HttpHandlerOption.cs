using System.Net;

namespace Lycoris.Nacos.Extensions.HttpRequest.Options
{
    /// <summary>
    /// 描述为处理服务请求而创建的 http 处理程序的配置参数
    /// </summary>
    internal class HttpHandlerOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowAutoRedirect">指定是否启用自动重定向</param>
        /// <param name="useCookieContainer">指定处理程序是否使用 cookie 容器</param>
        /// <param name="useTracing">指定是处理程序是否使用开放追踪</param>
        /// <param name="useProxy">指定处理程序是否必须使用代理</param>
        /// <param name="maxConnectionsPerServer">指定与网络端点的最大并发连接数</param>
        public HttpHandlerOption(bool allowAutoRedirect, bool useCookieContainer, bool useTracing, bool useProxy, int maxConnectionsPerServer)
        {
            AllowAutoRedirect = allowAutoRedirect;
            UseCookieContainer = useCookieContainer;
            UseTracing = useTracing;
            UseProxy = useProxy;
            MaxConnectionsPerServer = maxConnectionsPerServer;
            Cookies = new CookieContainer();
        }


        /// <summary>
        /// 指定是否启用自动重定向
        /// </summary>
        /// <value>AllowAutoRedirect</value>
        public bool AllowAutoRedirect { get; private set; }

        /// <summary>
        /// 指定处理程序是否使用 cookie 容器
        /// </summary>
        /// <value>UseCookieContainer</value>
        public bool UseCookieContainer { get; private set; }

        /// <summary>
        /// 指定是处理程序是否使用开放追踪
        /// </summary>
        /// <value>UseTracing</value>
        public bool UseTracing { get; private set; }

        /// <summary>
        /// 指定处理程序是否必须使用代理
        /// </summary>
        /// <value>UseProxy</value>
        public bool UseProxy { get; private set; }

        /// <summary>
        /// 指定与网络端点的最大并发连接数
        /// </summary>
        /// <value>MaxConnectionsPerServer</value>
        public int MaxConnectionsPerServer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CookieContainer Cookies { get; set; }
    }
}
