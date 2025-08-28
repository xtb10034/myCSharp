using System.Windows;

namespace DeviceTestingTool.Views
{
    /// <summary>
    /// ConfigurationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigurationDialog : Window
    {
        public ConfigurationDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
