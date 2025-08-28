using System;
using System.Collections.Generic;

namespace DeviceTestingTool.Models
{
    /// <summary>
    /// 测试结果模型，包含一次完整测试的统计信息
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// 测试开始时间
        /// </summary>
        public DateTime TestStartTime { get; set; }

        /// <summary>
        /// 测试结束时间
        /// </summary>
        public DateTime TestEndTime { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 采样次数
        /// </summary>
        public int SampleCount { get; set; }

        /// <summary>
        /// 平均值
        /// </summary>
        public double AverageValue { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// 最小值阈值
        /// </summary>
        public double MinThreshold { get; set; }

        /// <summary>
        /// 最大值阈值
        /// </summary>
        public double MaxThreshold { get; set; }

        /// <summary>
        /// 测试是否通过
        /// </summary>
        public bool IsPassed { get; set; }

        /// <summary>
        /// 所有测量数据
        /// </summary>
        public List<MeasurementData> MeasurementDataList { get; set; } = new List<MeasurementData>();
    }
}
