

using Microsoft.Extensions.Logging;

namespace Lycoris.Nacos.Extensions.Options
{
    /// <summary>
    /// 
    /// </summary>
    public interface INacosConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        string? DataId { get; }

        /// <summary>
        /// 
        /// </summary>
        string? Group { get; }

        /// <summary>
        /// 
        /// </summary>
        string NacosConfigurationType { get; }

        /// <summary>
        /// 
        /// </summary>
        bool NacosListener { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        void Listener(ILogger? logger, string? configuration);
    }
}
