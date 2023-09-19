using Lycoris.Nacos.Extensions.Exceptions;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public interface INacosServerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="NoHealthyInstanceException"></exception>
        Task<string> GetHealthyInstanceAsync(string group, string service);
    }
}
