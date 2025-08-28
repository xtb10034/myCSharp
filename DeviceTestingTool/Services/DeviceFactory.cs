using DeviceTestingTool.Models;

namespace DeviceTestingTool.Services
{
    /// <summary>
    /// 设备工厂类，根据设备类型创建相应的设备实例
    /// 实现工厂模式
    /// </summary>
    public  class DeviceFactory
    {
        /// <summary>
        /// 创建设备实例
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备实例</returns>
        public  DeviceBase CreateDevice(string deviceType, string deviceId)
        {
            switch (deviceType)
            {
                case "温度传感器":
                    return new TemperatureSensor(deviceId);
                case "压力传感器":
                    return new PressureSensor(deviceId);
                default:
                    throw new System.ArgumentException($"不支持的设备类型: {deviceType}");
            }
        }

        /// <summary>
        /// 获取所有支持的设备类型
        /// </summary>
        /// <returns>设备类型列表</returns>
        public string[] GetSupportedDeviceTypes()
        {
            return new[] { "温度传感器", "压力传感器" };
        }
    }
}
