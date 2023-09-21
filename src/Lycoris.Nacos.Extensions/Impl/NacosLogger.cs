
using Microsoft.Extensions.Logging;

namespace Lycoris.Nacos.Extensions.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NacosLogger : INacosLogger
    {
        private readonly ILogger? _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        public NacosLogger(ILogger? logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) => _logger?.LogInformation("{message}", message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) => _logger?.LogWarning("{message}", message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Warn(string message, Exception ex) => _logger?.LogWarning(ex, "{message}", message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message) => _logger?.LogWarning("{message}", message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception ex) => _logger?.LogWarning(ex, "{message}", message);
    }
}
