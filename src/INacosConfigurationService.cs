using Nacos.V2;
using System.Diagnostics.CodeAnalysis;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public interface INacosConfigurationService
    {
        /// <summary>
        /// 发布远端配置信息
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> PublishConfigurationAsync(string dataId, string group, string value, string type = "json");

        /// <summary>
        /// 发布远端配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> PublishConfigurationAsync<T>(string dataId, string group, T value, string type = "json") where T : class;

        /// <summary>
        /// 获取远端配置信息
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<string?> GetConfigurationAsync(string dataId, string group, long timeout = 5000L);

        /// <summary>
        /// 获取远端配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<T?> GetConfigurationAsync<T>(string dataId, string group, long timeout = 5000L);

        /// <summary>
        /// 移除远端配置信息
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<bool> RemoveConfigurationAsync(string dataId, string group);

        /// <summary>
        /// 添加配置监听
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="listener">需要继承 <see cref="IListener"/> 接口并实现相关功能</param>
        /// <returns></returns>
        Task AddConfigListenerAsync(string dataId, string group, [NotNull] IListener listener);

        /// <summary>
        /// 移除配置监听
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="listener">需要继承 <see cref="IListener"/> 接口并实现相关功能</param>
        /// <returns></returns>
        Task RemoveConfigListenerAsync(string dataId, string group, [NotNull] IListener listener);
    }
}
