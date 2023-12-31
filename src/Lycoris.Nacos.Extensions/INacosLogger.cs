﻿namespace Lycoris.Nacos.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal interface INacosLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Warn(string message, Exception ex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Error(string message, Exception ex);
    }
}
