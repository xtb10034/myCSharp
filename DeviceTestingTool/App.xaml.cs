using System;
using System.Windows;

namespace DeviceTestingTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 注册全局异常处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        /// <summary>
        /// 处理UI线程未捕获的异常
        /// </summary>
        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// 处理非UI线程未捕获的异常
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// 处理异常，显示错误信息并记录日志
        /// </summary>
        private void HandleException(Exception ex)
        {
            try
            {
                var loggingService = new Services.LoggingService();
                loggingService.LogError("发生未处理的异常", ex);
                
                MessageBox.Show(
                    $"发生错误: {ex.Message}\n请查看log.txt获取详细信息", 
                    "应用程序错误", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            catch
            {
                // 如果日志记录也失败，直接显示错误
                MessageBox.Show(
                    $"发生严重错误: {ex.Message}", 
                    "应用程序错误", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
    }
}
