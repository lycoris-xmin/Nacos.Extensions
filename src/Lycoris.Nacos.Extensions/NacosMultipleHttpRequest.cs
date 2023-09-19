namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosMultipleHttpRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<NacosHttpRequest>? Option { get; set; }
    }
}
