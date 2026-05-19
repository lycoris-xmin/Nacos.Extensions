using Lycoris.Nacos.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nacos.V2;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosConfigurationListener : IListener
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string? DataId;

        /// <summary>
        /// 
        /// </summary>
        public readonly string? Group;

        /// <summary>
        /// 
        /// </summary>
        private readonly INacosConfiguration? Configuration;

        /// <summary>
        /// 
        /// </summary>
        private readonly Type ConfigurationType;

        private readonly ILoggerFactory? _factory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="type"></param>
        public NacosConfigurationListener(IServiceProvider provider, Type type)
        {
            this.ConfigurationType = type;
            this.Configuration = provider.GetService(type) as INacosConfiguration;

            this.DataId = this.Configuration!.DataId;
            this.Group = this.Configuration!.Group;

            this._factory = provider.GetService<ILoggerFactory>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configInfo"></param>
        public void ReceiveConfigInfo(string configInfo) => this.Configuration!.Listener(_factory?.CreateLogger(this.Configuration.GetType()), configInfo);
    }
}
