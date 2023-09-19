using Lycoris.Base.Extensions;

namespace Lycoris.Nacos.Extensions.Options
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosBaseOption
    {
        /// <summary>
        /// Nacos服务地址
        /// </summary>
        public List<string>? Server { get; set; }

        /// <summary>
        /// 阿里云 <see langword="AccessKey"/>
        /// </summary>
        public string? AccessKey { get; set; }

        /// <summary>
        /// 阿里云 <see langword="SecretKey"/>
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Nacos用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Nacos密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 连接超时时间 默认 <see langword="15000"/>
        /// </summary>
        public int DefaultTimeOut { get; set; } = 15000;

        /// <summary>
        /// 监听时间 默认 <see langword="1000"/>
        /// </summary>
        public int ListenInterval { get; set; } = 1000;

        /// <summary>
        /// 配置信息使用Rpc请求 默认 <see langword="true"/> ， 建议不要改动，有些版本设置为<see langword="false"/>会有一定问题
        /// </summary>
        public bool ConfigUseRpc { get; set; } = true;

        /// <summary>
        /// 服务信息用Rpc请求 默认 <see langword="true"/> ， 建议不要改动，有些版本设置为<see langword="false"/>会有一定问题
        /// </summary>
        public bool NamingUseRpc { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public string NamingLoadCacheAtStart { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal bool ModelIsValid
        {
            get
            {
                if (!this.Server.HasValue())
                    throw new ArgumentNullException(nameof(this.Server));

                return true;
            }
        }
    }
}
