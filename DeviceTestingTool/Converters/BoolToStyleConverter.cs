using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeviceTestingTool.Converters
{
    /// <summary>
    /// 将布尔值转换为样式（通过为绿色，失败为红色）
    /// </summary>
    public class BoolToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPassed)
            {
                return isPassed 
                    ? Application.Current.FindResource("PassedResult") 
                    : Application.Current.FindResource("FailedResult");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
