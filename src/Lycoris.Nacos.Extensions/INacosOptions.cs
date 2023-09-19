namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INacosOptions<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        T? Value { get; }

        /// <summary>
        /// 推送当前值至Nacos远端配置中心
        /// </summary>
        /// <returns></returns>
        Task PushAsync();

        /// <summary>
        /// 重新拉取Nacos远端配置中心配置
        /// </summary>
        /// <returns></returns>
        Task PullAsync();

        /// <summary>
        /// 移除Nacos远端配置中心配置
        /// </summary>
        /// <returns></returns>
        Task RemoveAsync();
    }
}
