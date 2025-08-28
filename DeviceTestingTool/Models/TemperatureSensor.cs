using System;

namespace DeviceTestingTool.Models
{
    /// <summary>
    /// 温度传感器，继承自设备基类
    /// 生成25±波动范围℃的随机值
    /// </summary>
    public class TemperatureSensor : DeviceBase
    {
        private static readonly Random _random = new Random();

        public override string DeviceType => "温度传感器";
        public override double BaseValue => 25.0;
        public override double MinThreshold => 24.0;
        public override double MaxThreshold => 26.0;

        public TemperatureSensor(string deviceId) : base(deviceId)
        {
            FluctuationRange = 3; // 默认波动范围±3℃
        }

        public override bool Connect()
        {
            // 模拟连接过程，有10%的概率连接失败
            if (_random.Next(100) < 10)
                return false;

            IsConnected = true;
            return true;
        }

        public override void Disconnect()
        {
            IsConnected = false;
        }

        public override MeasurementData CollectData()
        {
            if (!IsConnected)
                throw new InvalidOperationException("设备未连接，无法采集数据");

            // 模拟数据采集失败的情况，5%的概率
            if (_random.Next(100) < 5)
                throw new Exception("数据采集失败，通讯中断");

            // 生成25±波动范围℃的随机值
            var fluctuation = (_random.NextDouble() * 2 - 1) * FluctuationRange;
            var value = BaseValue + fluctuation;

            return new MeasurementData
            {
                DeviceId = DeviceId,
                DeviceType = DeviceType,
                Timestamp = DateTime.Now,
                Value = Math.Round(value, 2),
                Unit = "℃"
            };
        }
    }
}
