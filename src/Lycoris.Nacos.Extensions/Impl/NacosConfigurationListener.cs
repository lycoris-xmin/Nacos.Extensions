using Lycoris.Base.Logging;
using Lycoris.Nacos.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
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

        /// <summary>
        /// 
        /// </summary>
        private readonly ILycorisLoggerFactory? _loggerFactory;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILycorisLogger? _logger;

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

            this._loggerFactory = provider.GetService<ILycorisLoggerFactory>();
            this._logger = this._loggerFactory?.CreateLogger<NacosConfigurationListener>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configInfo"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ReceiveConfigInfo(string configInfo)
        {
            _logger?.Info($"nacso configuration({this.Group}.{this.DataId}) received - {configInfo}");

            if (this.Configuration != null)
            {
                try
                {
                    this.Configuration!.Listener(this._loggerFactory?.CreateLogger(this.ConfigurationType), configInfo);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"nacos configuration listener handle exception:{ex.GetType().FullName}", ex);
                }
            }
        }
    }
}
