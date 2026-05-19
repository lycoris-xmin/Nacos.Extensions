using System.Net;

namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosHttpResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="HttpResponseMessage"></param>
        public NacosHttpResponse(HttpResponseMessage HttpResponseMessage)
        {
            Success = true;
            this.HttpResponseMessage = HttpResponseMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Exception"></param>
        /// <param name="HttpResponseMessage"></param>
        public NacosHttpResponse(Exception Exception, HttpResponseMessage? HttpResponseMessage = null)
        {
            Success = false;
            this.HttpResponseMessage = HttpResponseMessage;
            this.Exception = Exception;
        }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// 请求失败异常信息
        /// </summary>
        public Exception? Exception { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public HttpResponseMessage? HttpResponseMessage { get; set; }
    }
}
