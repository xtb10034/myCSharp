using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeviceTestingTool.Models;
using Newtonsoft.Json;

namespace DeviceTestingTool.Services
{
    /// <summary>
    /// 数据存储服务，用于保存和加载测试结果
    /// </summary>
    public class DataStorageService
    {
        private readonly LoggingService _loggingService;

        public DataStorageService(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// 保存测试结果到JSON文件
        /// </summary>
        /// <param name="result">测试结果</param>
        /// <param name="filePath">文件路径</param>
        public void SaveTestResult(TestResult result, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                
                using (var writer = new StreamWriter(filePath))
                {
                    writer.Write(json);
                }

                _loggingService.LogInformation($"测试结果已保存到 {filePath}");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("保存测试结果失败", ex);
                throw;
            }
        }

        /// <summary>
        /// 从JSON文件加载测试结果
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>测试结果</returns>
        public TestResult LoadTestResult(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("文件不存在", filePath);

                using (var reader = new StreamReader(filePath))
                {
                    var json = reader.ReadToEnd();
                    var result = JsonConvert.DeserializeObject<TestResult>(json);
                    
                    _loggingService.LogInformation($"已从 {filePath} 加载测试结果");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("加载测试结果失败", ex);
                throw;
            }
        }

        /// <summary>
        /// 从多个文件加载历史记录
        /// </summary>
        /// <param name="filePaths">文件路径列表</param>
        /// <returns>合并的测量数据列表</returns>
        public List<MeasurementData> LoadHistoricalData(IEnumerable<string> filePaths)
        {
            var allData = new List<MeasurementData>();

            foreach (var filePath in filePaths)
            {
                try
                {
                    var result = LoadTestResult(filePath);
                    allData.AddRange(result.MeasurementDataList);
                }
                catch (Exception ex)
                {
                    _loggingService.LogError($"加载文件 {filePath} 失败", ex);
                }
            }

            // 按时间戳排序
            return allData.OrderBy(d => d.Timestamp).ToList();
        }
    }
}
