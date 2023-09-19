namespace Lycoris.Nacos.Extensions.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class MultipleHttpRequestException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public MultipleHttpRequestException() : base("has a request configuration with empty groupname,please check")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public MultipleHttpRequestException(string groupName) : base($"{groupName} request configuration with empty servicename,please check")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public MultipleHttpRequestException(string groupName, string serviceName) : base($"{groupName}.{serviceName} request's option configuration not configured,please check")
        {

        }
    }
}
