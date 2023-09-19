namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosHttpClientConfig
    {
        /// <summary>
        /// 启用扩展内部日志记录
        /// </summary>
        public bool EnableLogger { get; set; }

        /// <summary>
        /// 记录全部头部信息
        /// </summary>
        public bool AllowAllHeaderFilter { get; set; } = false;

        /// <summary>
        /// 请求头过滤集合
        /// </summary>
        public List<string>? HeaderFilter { get; set; }

        /// <summary>
        /// Cookie过滤集合
        /// </summary>
        public List<string>? CookieFilter { get; set; }
    }
}
