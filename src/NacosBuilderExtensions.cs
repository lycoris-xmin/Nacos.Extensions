using Lycoris.Nacos.Extensions.Builder;
using Lycoris.Nacos.Extensions.Exceptions;
using Lycoris.Nacos.Extensions.Impl;
using Lycoris.Nacos.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class NacosBuilderExtensions
    {
        /// <summary>
        /// 添加 Nacos 远端 Appsettings 扩展
        /// 注册后，使用 <see cref="NacosAppSettings"/> 获取nacos上的 <see langword="json"/> 配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddNacosAppSettings(this WebApplicationBuilder builder, Action<NacosAppSettingBuilder> configure)
        {
            var setting = new NacosAppSettingBuilder();
            configure(setting);

            if (setting.ModelIsValid && setting.Listeners.Any())
            {
                builder.Configuration.AddNacosV2Configuration(opt =>
                {
                    ApplyAuth(setting,
                        (ak, sk) => { opt.AccessKey = ak; opt.SecretKey = sk; },
                        (user, pwd) => { opt.UserName = user; opt.Password = pwd; });

                    opt.ServerAddresses = setting.Server;
                    opt.DefaultTimeOut = setting.DefaultTimeOut;
                    opt.Namespace = setting.Namespace;
                    opt.ListenInterval = setting.ListenInterval;
                    opt.ConfigUseRpc = setting.ConfigUseRpc;
                    opt.NamingUseRpc = setting.NamingUseRpc;
                    opt.NamingLoadCacheAtStart = setting.NamingLoadCacheAtStart;
                    opt.Listeners = setting.Listeners;
                });

                NacosAppSettings.Configuration = builder.Configuration;
            }

            return builder;
        }

        /// <summary>
        /// 添加 nacos 注册发现服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacosRegisterCenter(this IServiceCollection services, Action<NacosRegistrationCenterBuilder> configure)
        {
            var setting = new NacosRegistrationCenterBuilder();
            configure(setting);

            if (setting.ModelIsValid)
            {
                if (!setting.Port.HasValue)
                    throw new ArgumentNullException(nameof(setting.Port));
                else if (setting.Port.Value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(setting.Port), "must be greater than 0");

                services.AddNacosAspNet(opt =>
                {
                    ApplyAuth(setting,
                        (ak, sk) => { opt.AccessKey = ak; opt.SecretKey = sk; },
                        (user, pwd) => { opt.UserName = user; opt.Password = pwd; });

                    if (!string.IsNullOrEmpty(setting.Ip))
                        opt.Ip = setting.Ip;

                    opt.ServerAddresses = setting.Server;
                    opt.DefaultTimeOut = setting.DefaultTimeOut;
                    opt.Namespace = setting.Namespace;
                    opt.ListenInterval = setting.ListenInterval;
                    opt.ServiceName = setting.ServiceName;
                    opt.GroupName = setting.GroupName;
                    opt.ClusterName = setting.ClusterName;
                    opt.Port = setting.Port!.Value;
                    opt.Weight = setting.Weight;
                    opt.RegisterEnabled = setting.RegisterEnabled;
                    opt.InstanceEnabled = setting.InstanceEnabled;
                    opt.Ephemeral = setting.Ephemeral;
                    opt.Secure = setting.Secure;
                    opt.PreferredNetworks = setting.PreferredNetworks;
                    opt.ConfigUseRpc = setting.ConfigUseRpc;
                    opt.NamingUseRpc = setting.NamingUseRpc;
                    opt.NamingLoadCacheAtStart = setting.NamingLoadCacheAtStart;

                    //配置参数
                    if (setting.Metadata?.Count > 0)
                    {
                        foreach (var kv in setting.Metadata)
                            opt.Metadata.Add(kv.Key, kv.Value);
                    }
                });
            }

            return services;
        }

        /// <summary>
        /// 注册 Nacos 基础服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacosNamingConfiguration(this IServiceCollection services, Action<NacosConfigurationBuilder> configure)
        {
            var setting = new NacosConfigurationBuilder(services);
            configure(setting);

            if (setting.ModelIsValid)
            {
                // 配置管理
                services.AddNacosV2Config(opt =>
                {
                    ApplyAuth(setting,
                        (ak, sk) => { opt.AccessKey = ak; opt.SecretKey = sk; },
                        (user, pwd) => { opt.UserName = user; opt.Password = pwd; });

                    opt.ServerAddresses = setting.Server;
                    opt.Namespace = setting.Namespace;
                    opt.ConfigUseRpc = setting.ConfigUseRpc;
                    opt.NamingUseRpc = setting.NamingUseRpc;
                });

                // 服务管理
                services.AddNacosV2Naming(opt =>
                {
                    ApplyAuth(setting,
                        (ak, sk) => { opt.AccessKey = ak; opt.SecretKey = sk; },
                        (user, pwd) => { opt.UserName = user; opt.Password = pwd; });

                    opt.ServerAddresses = setting.Server;
                    opt.Namespace = setting.Namespace;
                    opt.ConfigUseRpc = setting.ConfigUseRpc;
                    opt.NamingUseRpc = setting.NamingUseRpc;
                });

                // 封装服务
                services.AddSingleton<INacosConfigurationService, NacosConfigurationService>();
                services.AddSingleton<INacosServerService, NacosServerService>();

                // 配置监听处理
                if (setting.Configurations.Count > 0)
                {
                    var unqualified = setting.Configurations.Where(x => string.IsNullOrEmpty(x.DataId) || string.IsNullOrEmpty(x.Group) || string.IsNullOrEmpty(x.NacosConfigurationType)).FirstOrDefault();
                    if (unqualified != null)
                    {
                        var message = "";
                        if (string.IsNullOrEmpty(unqualified.DataId))
                            message += "DataId not set";
                        if (string.IsNullOrEmpty(unqualified.Group))
                            message += "Group not set,";
                        if (string.IsNullOrEmpty(unqualified.NacosConfigurationType))
                            message += "NacosConfigurationType not set";

                        throw new UnqualifiedNacosConfigurationException(unqualified.GetType().Name, message.TrimEnd(','));
                    }

                    // 注册Nacos远端配置
                    setting.NacosConfigurationRegister.ForEach(configure => configure(services));

                    // 监听服务处理
                    setting.ConfigurationListeners.ForEach(configure => services.AddTransient(configure));

                    // 启用任务
                    services.AddHostedService(isp => new NacosConfigurationHostedService(isp, setting.GetConfigurations));
                }
            }

            return services;
        }

        /// <summary>
        /// 使用 <see cref="INacosHttpClient"/> 扩展，必须注册 <see cref="AddNacosNamingConfiguration"/> 或自己手动注册 <see langword="nacos-sdk"/> 的 <see langword="ServiceCollectionExtensions.AddNacosV2Naming"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacosHttpClient(this IServiceCollection services)
        {
            services.Configure<NacosHttpClientConfig>(x =>
            {
                x.EnableLogger = false;
                x.AllowAllHeaderFilter = false;
                x.HeaderFilter = null;
                x.CookieFilter = null;
            });

            if (!services.Any(x => x.ServiceType == typeof(INacosHttpClient)))
                services.AddTransient<INacosHttpClient, NacosHttpClient>();

            return services;
        }

        /// <summary>
        /// 使用 <see cref="INacosHttpClient"/> 扩展，必须注册 <see cref="AddNacosNamingConfiguration"/> 或自己手动注册 <see langword="nacos-sdk"/> 的 <see langword="ServiceCollectionExtensions.AddNacosV2Naming"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacosHttpClient(this IServiceCollection services, Action<NacosHttpClientBuilder> configure)
        {
            var builder = new NacosHttpClientBuilder(services);

            configure(builder);

            services.Configure<NacosHttpClientConfig>(x =>
            {
                x.EnableLogger = builder.EnableLogger;
                x.AllowAllHeaderFilter = builder.AllowAllHeaderFilter ?? false;
                x.HeaderFilter = (builder.HeaderFilter?.Count ?? 0) > 0 ? builder.HeaderFilter : null;
                x.CookieFilter = (builder.CookieFilter?.Count ?? 0) > 0 ? builder.CookieFilter : null;
            });

            if (!services.Any(x => x.ServiceType == typeof(INacosHttpClient)))
                services.AddTransient<INacosHttpClient, NacosHttpClient>();

            if (builder.EnableLogger)
                services.TryAddSingleton<INacosHttpClientLogger, NacosHttpClientLogger>();

            return services;
        }

        private static void ApplyAuth(NacosBaseOption setting,
            Action<string?, string?> applyAkSk,
            Action<string?, string?> applyUserPwd)
        {
            if (!string.IsNullOrEmpty(setting.AccessKey) || !string.IsNullOrEmpty(setting.SecretKey))
            {
                if (string.IsNullOrEmpty(setting.AccessKey))
                    throw new ArgumentNullException(nameof(setting.AccessKey));
                if (string.IsNullOrEmpty(setting.SecretKey))
                    throw new ArgumentNullException(nameof(setting.SecretKey));
                applyAkSk(setting.AccessKey, setting.SecretKey);
            }
            else
            {
                if (string.IsNullOrEmpty(setting.UserName))
                    throw new ArgumentNullException(nameof(setting.UserName));
                if (string.IsNullOrEmpty(setting.Password))
                    throw new ArgumentNullException(nameof(setting.Password));
                applyUserPwd(setting.UserName, setting.Password);
            }
        }
    }
}
