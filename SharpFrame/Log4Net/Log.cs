using System;
using log4net;
using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = @"Log4Net\\log4net.config", Watch = true)]

namespace SharpFrame.log4Net
{
    /// <summary>
    /// 运行日志
    /// </summary>
    public class Log
    {
        public enum State
        {
            Error,
            Normal,
            Run
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            ILog log = LogManager.GetLogger("Info");
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }

        /// <summary>
        /// 错误日志带异常
        /// </summary>
        /// <param name="message">错误日志</param>
        public static void Error(string message, Exception ex)
        {
            ILog log = LogManager.GetLogger("Error");
            if (log.IsErrorEnabled)
            {
                log.Error(message, ex);
            }
        }

        /// <summary>
        /// 错误日志不带异常
        /// </summary>
        /// <param name="message">错误日志</param>
        public static void Error(string message)
        {
            ILog log = LogManager.GetLogger("Error");
            if (log.IsErrorEnabled)
            {
                log.Error(message);
            }
        }
    }
}
