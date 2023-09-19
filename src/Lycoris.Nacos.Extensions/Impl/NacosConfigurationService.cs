using Lycoris.Base.Extensions;
using Nacos.V2;
using System.Diagnostics.CodeAnalysis;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosConfigurationService : INacosConfigurationService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly INacosConfigService _nacosConfigService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nacosConfigService"></param>
        public NacosConfigurationService(INacosConfigService nacosConfigService)
        {
            _nacosConfigService = nacosConfigService;
        }

        /// <summary>
        /// 发布远端配置信息
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> PublishConfigurationAsync(string dataId, string group, string value, string type = "json")
            => await _nacosConfigService.PublishConfig(dataId, group, value, type);

        /// <summary>
        /// 发布远端配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> PublishConfigurationAsync<T>(string dataId, string group, T value, string type = "json") where T : class
            => await _nacosConfigService.PublishConfig(dataId, group, value.ToJson(), type);

        /// <summary>
        /// 获取远端配置信息
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<string?> GetConfigurationAsync(string dataId, string group, long timeout = 5000L)
        {
            var str = await _nacosConfigService.GetConfig(dataId, group, timeout);
            if (str.IsNullOrEmpty())
                return default;

            return str;
        }

        /// <summary>
        /// 获取远端配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<T?> GetConfigurationAsync<T>(string dataId, string group, long timeout = 5000L)
        {
            var str = await _nacosConfigService.GetConfig(dataId, group, timeout);
            if (str.IsNullOrEmpty())
                return default;

            return str.ToObject<T>();
        }

        /// <summary>
        /// 移除远端配置信息
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<bool> RemoveConfigurationAsync(string dataId, string group) => await _nacosConfigService.RemoveConfig(dataId, group);

        /// <summary>
        /// 添加配置监听
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="listener">需要继承 <see cref="IListener"/> 接口并实现相关功能</param>
        /// <returns></returns>
        public async Task AddConfigListenerAsync(string dataId, string group, [NotNull] IListener listener)
            => await _nacosConfigService.AddListener(dataId, group, listener);

        /// <summary>
        /// 移除配置监听
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="group"></param>
        /// <param name="listener">需要继承 <see cref="IListener"/> 接口并实现相关功能</param>
        /// <returns></returns>
        public async Task RemoveConfigListenerAsync(string dataId, string group, [NotNull] IListener listener)
            => await _nacosConfigService.RemoveListener(dataId, group, listener);
    }
}
