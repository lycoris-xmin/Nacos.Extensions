using Lycoris.Base.Extensions;
using Lycoris.Nacos.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NacosConfiguration<T> : INacosConfiguration where T : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string? DataId { get; init; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string? Group { get; init; }

        /// <summary>
        /// 配置格式 默认格式：<see langword="json"/>
        /// </summary>
        [JsonIgnore]
        public string NacosConfigurationType { get; init; } = "json";

        /// <summary>
        /// 是否添加监听器 默认：<see langword="false"/>
        /// </summary>
        [JsonIgnore]
        public bool NacosListener { get; init; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public virtual void Listener(ILogger? logger, string? configuration) => Received(configuration.ToObject<T>());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public virtual void Received(T? configuration) { }
    }
}
