using System;
using System.Globalization;
using System.Windows.Data;

namespace DeviceTestingTool.Converters
{
    /// <summary>
    /// 将布尔值转换为测试结果文本（通过/失败）
    /// </summary>
    public class BoolToResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPassed)
            {
                return isPassed ? "通过 (Pass)" : "失败 (Fail)";
            }
            return "未测试";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
