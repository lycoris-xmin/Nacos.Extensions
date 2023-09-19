namespace Lycoris.Nacos.Extensions.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class UnqualifiedNacosConfigurationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationName"></param>
        /// <param name="message"></param>
        public UnqualifiedNacosConfigurationException(string configurationName, string message) : base($"{configurationName} {message}")
        {

        }
    }
}
