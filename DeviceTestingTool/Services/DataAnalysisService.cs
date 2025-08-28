using System;
using System.Collections.Generic;
using System.Linq;
using DeviceTestingTool.Models;

namespace DeviceTestingTool.Services
{
    /// <summary>
    /// 数据分析服务，用于处理和分析采集到的数据
    /// </summary>
    public class DataAnalysisService
    {
        /// <summary>
        /// 分析测量数据，生成测试结果
        /// </summary>
        /// <param name="dataList">测量数据列表</param>
        /// <param name="device">设备实例</param>
        /// <param name="startTime">测试开始时间</param>
        /// <returns>测试结果</returns>
        public TestResult AnalyzeData(List<MeasurementData> dataList, DeviceBase device, DateTime startTime)
        {
            if (dataList == null || !dataList.Any())
                throw new ArgumentException("没有可分析的数据", nameof(dataList));

            var values = dataList.Select(d => d.Value).ToList();

            return new TestResult
            {
                TestStartTime = startTime,
                TestEndTime = dataList.Last().Timestamp,
                DeviceId = device.DeviceId,
                DeviceType = device.DeviceType,
                SampleCount = dataList.Count,
                AverageValue = Math.Round(values.Average(), 2),
                MaxValue = values.Max(),
                MinValue = values.Min(),
                MinThreshold = device.MinThreshold,
                MaxThreshold = device.MaxThreshold,
                IsPassed = values.All(v => v >= device.MinThreshold && v <= device.MaxThreshold),
                MeasurementDataList = dataList
            };
        }
    }
}
