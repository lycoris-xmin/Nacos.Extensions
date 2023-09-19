using Lycoris.Base.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class NacosOptions<T> : INacosOptions<T> where T : NacosConfiguration<T>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly INacosConfigurationService _configurationService;

        /// <summary>
        /// 
        /// </summary>
        public T? Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public NacosOptions(IServiceProvider provider)
        {
            this.Value = provider.GetRequiredService<T>();
            _configurationService = provider.GetService<INacosConfigurationService>()!;
        }

        /// <summary>
        /// 推送当前值至Nacos远端配置中心
        /// </summary>
        /// <returns></returns>
        public async Task PushAsync()
        {
            if (this.Value!.DataId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.Value.DataId));
            else if (this.Value!.Group.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.Value.Group));

            await _configurationService.PublishConfigurationAsync(this.Value!.DataId!, this.Value!.Group!, this.Value, this.Value.NacosConfigurationType);
        }

        /// <summary>
        /// 重新拉取Nacos远端配置中心配置
        /// </summary>
        /// <returns></returns>
        public async Task PullAsync()
        {
            if (this.Value!.DataId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.Value.DataId));
            else if (this.Value!.Group.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.Value.Group));

            this.Value = await _configurationService.GetConfigurationAsync<T>(this.Value!.DataId!, this.Value!.Group!);
        }

        /// <summary>
        /// 移除Nacos远端配置中心配置
        /// </summary>
        /// <returns></returns>
        public async Task RemoveAsync()
        {
            if (this.Value!.DataId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.Value.DataId));
            else if (this.Value!.Group.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.Value.Group));

            await _configurationService.RemoveConfigurationAsync(this.Value!.DataId!, this.Value!.Group!);
        }
    }
}
