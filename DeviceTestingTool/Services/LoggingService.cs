using System;
using System.IO;
using System.Text;

namespace DeviceTestingTool.Services
{
    /// <summary>
    /// 日志服务，用于记录系统运行日志
    /// </summary>
    public class LoggingService
    {
        private readonly string _logFilePath;
        private static readonly object _lockObj = new object();

        public LoggingService()
        {
            // 日志文件保存在应用程序目录下的log.txt
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public void LogInformation(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="ex">异常对象</param>
        public void LogError(string message, Exception ex = null)
        {
            var errorMessage = ex != null ? $"{message}: {ex.Message}\n{ex.StackTrace}" : message;
            WriteLog("ERROR", errorMessage);
        }

        /// <summary>
        /// 写入日志到文件
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        private void WriteLog(string level, string message)
        {
            try
            {
                lock (_lockObj)
                {
                    var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
                    File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
                }
            }
            catch
            {
                // 日志记录失败时不抛出异常，避免影响主程序
            }
        }
    }
}
