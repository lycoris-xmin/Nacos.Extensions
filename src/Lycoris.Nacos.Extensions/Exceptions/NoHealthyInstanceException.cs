namespace Lycoris.Nacos.Extensions.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NoHealthyInstanceException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceName"></param>
        public NoHealthyInstanceException(string groupName, string serviceName) : base($"unable to find healthy service instance from nacos by {groupName}.{serviceName}")
        {

        }
    }
}
