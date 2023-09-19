using Lycoris.Base.Extensions;
using Lycoris.Base.Logging;
using Lycoris.Nacos.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosConfigurationHostedService : IHostedService
    {
        private readonly IServiceProvider provider;
        private readonly INacosConfigurationService configurationService;
        private readonly List<Func<IServiceProvider, INacosConfiguration>> configurations;
        private readonly IEnumerable<NacosConfigurationListener> listeners;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="configurations"></param>
        public NacosConfigurationHostedService(IServiceProvider provider, List<Func<IServiceProvider, INacosConfiguration>> configurations)
        {
            this.provider = provider;
            this.configurations = configurations;
            this.configurationService = provider.GetService<INacosConfigurationService>()!;
            this.listeners = provider.GetServices<NacosConfigurationListener>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = provider.GetService<ILycorisLoggerFactory>();

            // 初始化处理
            if (this.configurations.HasValue())
            {
                foreach (var item in this.configurations!)
                {
                    var configuration = item(provider);

                    if (configuration!.DataId.IsNullOrEmpty() || configuration!.Group.IsNullOrEmpty())
                        continue;

                    var data = await configurationService.GetConfigurationAsync(configuration!.DataId!, configuration!.Group!).ConfigureAwait(false);
                    configuration.Listener(factory?.CreateLogger(configuration.GetType()), data);
                }
            }

            // 监听处理
            if (this.listeners.HasValue())
            {
                foreach (var item in this.listeners)
                {
                    if (item!.DataId.IsNullOrEmpty() || item!.Group.IsNullOrEmpty())
                        continue;

                    await configurationService.AddConfigListenerAsync(item.DataId!, item.Group!, item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.listeners.HasValue())
            {
                try
                {
                    foreach (var item in this.listeners)
                    {
                        if (item!.DataId.IsNullOrEmpty() || item!.Group.IsNullOrEmpty())
                            continue;

                        await configurationService.RemoveConfigListenerAsync(item.DataId!, item.Group!, item);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
