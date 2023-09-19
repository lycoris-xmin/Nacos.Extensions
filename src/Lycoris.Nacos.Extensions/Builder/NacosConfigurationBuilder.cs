using Lycoris.Nacos.Extensions.Impl;
using Lycoris.Nacos.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lycoris.Nacos.Extensions.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosConfigurationBuilder : NacosBaseOption
    {
        private readonly IServiceCollection services;

        internal List<INacosConfiguration> Configurations { get; set; }
        internal List<Action<IServiceCollection>> NacosConfigurationRegister { get; set; }
        internal List<Func<IServiceProvider, INacosConfiguration>> GetConfigurations { get; set; }
        internal List<Func<IServiceProvider, NacosConfigurationListener>> ConfigurationListeners { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NacosConfigurationBuilder(IServiceCollection services)
        {
            this.services = services;
            Configurations = new List<INacosConfiguration>();
            NacosConfigurationRegister = new List<Action<IServiceCollection>>();
            GetConfigurations = new List<Func<IServiceProvider, INacosConfiguration>>();
            ConfigurationListeners = new List<Func<IServiceProvider, NacosConfigurationListener>>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddNacosConfiguration<T>() where T : NacosConfiguration<T>, new()
        {
            var tmp = new T();
            Configurations.Add(tmp);
            NacosConfigurationRegister.Add((services) => services.TryAddSingleton<T>());
            GetConfigurations.Add((isp) => isp.GetService<T>()!);

            if (tmp.NacosListener)
                ConfigurationListeners!.Add((isp) => new NacosConfigurationListener(isp, typeof(T)));
        }

        /// <summary>
        /// 
        /// </summary>
        public void UseNacosOptions() => services.AddSingleton(typeof(INacosOptions<>), typeof(NacosOptions<>));
    }
}
