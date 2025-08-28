using System;

namespace DeviceTestingTool.Models
{
    /// <summary>
    /// 压力传感器，继承自设备基类
    /// 生成100±波动范围kPa的随机值
    /// </summary>
    public class PressureSensor : DeviceBase
    {
        private static readonly Random _random = new Random();

        public override string DeviceType => "压力传感器";
        public override double BaseValue => 100.0;
        public override double MinThreshold => 95.0;
        public override double MaxThreshold => 105.0;

        public PressureSensor(string deviceId) : base(deviceId)
        {
            FluctuationRange = 5; // 默认波动范围±5kPa
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

            // 生成100±波动范围kPa的随机值
            var fluctuation = (_random.NextDouble() * 2 - 1) * FluctuationRange;
            var value = BaseValue + fluctuation;

            return new MeasurementData
            {
                DeviceId = DeviceId,
                DeviceType = DeviceType,
                Timestamp = DateTime.Now,
                Value = Math.Round(value, 2),
                Unit = "kPa"
            };
        }
    }
}
