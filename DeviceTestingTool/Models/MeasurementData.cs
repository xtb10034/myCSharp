using System;

namespace DeviceTestingTool.Models
{
    /// <summary>
    /// 测量数据模型，包含单次测量的所有信息
    /// </summary>
    public class MeasurementData
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 测量时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 测量值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
    }
}
