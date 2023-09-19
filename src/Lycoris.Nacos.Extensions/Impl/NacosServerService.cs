using Lycoris.Nacos.Extensions.Exceptions;
using Nacos.V2;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosServerService : INacosServerService
    {
        private readonly INacosNamingService _nacosNamingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nacosNamingService"></param>
        public NacosServerService(INacosNamingService nacosNamingService)
        {
            _nacosNamingService = nacosNamingService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="NoHealthyInstanceException"></exception>
        public async Task<string> GetHealthyInstanceAsync(string group, string service)
        {
            var instance = await _nacosNamingService.SelectOneHealthyInstance(service, group) ?? throw new NoHealthyInstanceException(group, service);

            var host = $"{instance.Ip}:{instance.Port}";

            return (instance.Metadata.TryGetValue("secure", out _) ? $"https://{host}" : $"http://{host}") ?? "";
        }
    }
}
