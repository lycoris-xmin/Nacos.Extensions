using Lycoris.Nacos.Extensions.Options;
using Nacos.V2.Common;

namespace Lycoris.Nacos.Extensions.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosRegistrationCenterBuilder : NacosBaseOption
    {
        /// <summary>
        /// 服务群组
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// 集群名称 默认：<see cref="Constants.DEFAULT_CLUSTER_NAME"/>
        /// </summary>
        public string ClusterName { get; set; } = Constants.DEFAULT_CLUSTER_NAME;

        /// <summary>
        /// 实例IP地址，不设置的话由 <see langword="Nacos"/> 自动获取
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 实例开放端口
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PreferredNetworks { get; set; }

        /// <summary>
        /// 实例权重 默认：<see langword="100.00"/>
        /// </summary>
        public double Weight { get; set; } = 100;

        /// <summary>
        /// 注册服务 默认：<see langword="true"/>，如果只想订阅请设置：<see langword="false"/>
        /// </summary>
        public bool RegisterEnabled { get; set; } = true;

        /// <summary>
        ///  启用实例  默认：<see langword="true"/>
        /// </summary>
        public bool InstanceEnabled { get; set; } = true;

        /// <summary>
        ///  临时实例  默认：<see langword="true"/>
        /// </summary>
        public bool Ephemeral { get; set; } = true;

        /// <summary>
        /// 是否为Https服务 默认：<see langword="false"/>
        /// </summary>
        public bool Secure { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddMeta(string key, string value)
        {
            this.Metadata ??= new Dictionary<string, string>();
            this.Metadata.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, string>? Metadata { get; set; }
    }
}
