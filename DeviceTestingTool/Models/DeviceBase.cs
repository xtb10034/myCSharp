using System;
using System.ComponentModel;

namespace DeviceTestingTool.Models
{
    /// <summary>
    /// 设备基类，包含所有设备的公共属性和方法
    /// </summary>
    public abstract class DeviceBase : INotifyPropertyChanged
    {
        private string _deviceId;
        private bool _isConnected;
        private int _samplingInterval = 1000; // 默认采样间隔1秒

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set
            {
                _deviceId = value;
                OnPropertyChanged(nameof(DeviceId));
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public abstract string DeviceType { get; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        /// <summary>
        /// 采样间隔(毫秒)
        /// </summary>
        public int SamplingInterval
        {
            get => _samplingInterval;
            set
            {
                _samplingInterval = value;
                OnPropertyChanged(nameof(SamplingInterval));
            }
        }

        /// <summary>
        /// 模拟值波动范围
        /// </summary>
        public int FluctuationRange { get; set; }

        /// <summary>
        /// 基础值（温度为25℃，压力为100kPa）
        /// </summary>
        public abstract double BaseValue { get; }

        /// <summary>
        /// 最小值阈值
        /// </summary>
        public abstract double MinThreshold { get; }

        /// <summary>
        /// 最大值阈值
        /// </summary>
        public abstract double MaxThreshold { get; }

        protected DeviceBase(string deviceId)
        {
            DeviceId = deviceId;
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns>是否连接成功</returns>
        public abstract bool Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 采集数据
        /// </summary>
        /// <returns>采集到的数据</returns>
        public abstract MeasurementData CollectData();

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
