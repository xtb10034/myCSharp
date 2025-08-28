using System;
using System.Globalization;
using System.Windows.Data;

namespace DeviceTestingTool.Converters
{
    /// <summary>
    /// 将设备类型转换为对应的单位
    /// </summary>
    public class DeviceTypeToUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string deviceType)
            {
                return deviceType == "温度传感器" ? "℃" : "kPa";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
