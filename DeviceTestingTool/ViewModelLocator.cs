using DeviceTestingTool.ViewModels;

namespace DeviceTestingTool
{
    /// <summary>
    /// ViewModel定位器，用于在XAML中绑定ViewModel
    /// </summary>
    public class ViewModelLocator
    {
        private static MainViewModel _mainViewModel;

        /// <summary>
        /// 主窗口ViewModel
        /// </summary>
        public MainViewModel Main => _mainViewModel ?? (_mainViewModel = new MainViewModel());
    }
}
