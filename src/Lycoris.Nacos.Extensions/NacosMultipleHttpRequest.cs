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
        public NacosMultipleHttpRequest() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="ServiceName"></param>
        public NacosMultipleHttpRequest(string GroupName, string ServiceName)
        {
            this.GroupName = GroupName;
            this.ServiceName = ServiceName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="ServiceName"></param>
        /// <param name="Option"></param>
        public NacosMultipleHttpRequest(string GroupName, string ServiceName, Action<NacosHttpRequest> Option)
        {
            this.GroupName = GroupName;
            this.ServiceName = ServiceName;
            this.Option = Option;
        }

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
