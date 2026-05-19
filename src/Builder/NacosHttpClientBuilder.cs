using Lycoris.Nacos.Extensions.Resilience;
using Lycoris.Nacos.Extensions.Tracing;
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

        /// <summary>
        /// 启用 OpenTelemetry 兼容的链路追踪（基于 System.Diagnostics.Activity）
        /// </summary>
        public void UseOpenTelemetryTracing()
        {
            services.AddSingleton<INacosTracing, NacosActivityTracing>();
        }

        /// <summary>
        /// 启用 SkyWalking 链路追踪（自动传播 sw8 请求头）
        /// </summary>
        public void UseSkyWalkingTracing()
        {
            services.AddSingleton<INacosTracing, NacosSkyWalkingTracing>();
        }

        /// <summary>
        /// 使用自定义链路追踪实现
        /// </summary>
        /// <typeparam name="T">实现了 INacosTracing 的类型</typeparam>
        public void UseTracing<T>() where T : class, INacosTracing
        {
            services.AddSingleton<INacosTracing, T>();
        }

        /// <summary>
        /// 配置 Polly 弹性策略（重试、断路器、降级）
        /// </summary>
        /// <param name="configure">弹性策略配置</param>
        public void UseResilience(Action<NacosResilienceOptions> configure)
        {
            var options = new NacosResilienceOptions();
            configure(options);

            var pipeline = NacosResiliencePipeline.Create(options);
            services.AddSingleton(pipeline);
        }
    }
}
