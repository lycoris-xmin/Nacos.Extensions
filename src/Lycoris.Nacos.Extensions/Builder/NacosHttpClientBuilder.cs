using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Nacos.Extensions.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosHttpClientBuilder
    {
        private readonly IServiceCollection services;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public NacosHttpClientBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// 启用扩展日志记录
        /// </summary>
        public bool EnableLogger { get; set; } = false;

        /// <summary>
        /// 记录全部头部信息
        /// </summary>
        public bool? AllowAllHeaderFilter { get; set; } = null;

        /// <summary>
        /// 头部过滤器
        /// </summary>
        public List<string> HeaderFilter { get; set; } = new List<string>();

        /// <summary>
        /// Cookie过滤器
        /// </summary>
        public List<string> CookieFilter { get; set; } = new List<string>();

        /// <summary>
        /// 添加自定日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddNacosHttpClientLogger<T>() where T : class, INacosHttpClientLogger
        {
            this.EnableLogger = true;
            services.AddSingleton<INacosHttpClientLogger, T>();
        }
    }
}
