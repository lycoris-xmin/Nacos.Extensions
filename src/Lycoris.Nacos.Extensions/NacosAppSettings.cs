using Lycoris.Base.Extensions;
using Microsoft.Extensions.Configuration;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class NacosAppSettings
    {
        /// <summary>
        /// Nacos远端配置
        /// </summary>
        public static IConfiguration? Configuration { get; internal set; }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfig(this string key, string defaultValue = "")
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            var value = ConfigurationBinder.GetValue(Configuration, key, defaultValue);
            if (value.IsNullOrEmpty())
                return defaultValue.Trim();
            else
                return value.Trim();
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string TryGetConfig(this string key, string defaultValue = "")
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            try
            {
                return GetConfig(key, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetConfig<T>(this string key)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            return ConfigurationBinder.GetValue<T>(Configuration, key);
        }


        /// <summary>
        /// 获取属性配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? TryGetConfig<T>(this string key)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            try
            {
                return ConfigurationBinder.GetValue<T>(Configuration, key);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection GetSection(string key)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            return Configuration!.GetSection(key);
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection? TryGetSection(string key)
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            try
            {
                return Configuration!.GetSection(key);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetSection<T>(string key) where T : class, new()
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            return ConfigurationBinder.Get<T>(Configuration!.GetSection(key));
        }

        /// <summary>
        /// 获取属性节点配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? TryGetSection<T>(string key) where T : class, new()
        {
            if (Configuration == null)
                throw new ArgumentNullException(nameof(Configuration));

            try
            {
                return ConfigurationBinder.Get<T>(Configuration!.GetSection(key));
            }
            catch
            {
                return default;
            }
        }
    }
}
