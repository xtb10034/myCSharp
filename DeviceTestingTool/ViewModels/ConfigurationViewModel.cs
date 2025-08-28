using DeviceTestingTool.Models;

namespace DeviceTestingTool.ViewModels
{
    /// <summary>
    /// 配置对话框的ViewModel
    /// 处理配置相关的业务逻辑
    /// </summary>
    public class ConfigurationViewModel : BaseViewModel
    {
        private int _fluctuationRange;
        private int _samplingInterval;
        private int _sampleCount;

        public DeviceBase Device { get; }

        public int FluctuationRange
        {
            get => _fluctuationRange;
            set => SetProperty(ref _fluctuationRange, value);
        }

        public int SamplingInterval
        {
            get => _samplingInterval;
            set => SetProperty(ref _samplingInterval, value);
        }

        public int SampleCount
        {
            get => _sampleCount;
            set => SetProperty(ref _sampleCount, value);
        }

        public ConfigurationViewModel(DeviceBase device, int sampleCount)
        {
            Device = device;
            FluctuationRange = device.FluctuationRange;
            SamplingInterval = device.SamplingInterval;
            SampleCount = sampleCount;
        }

        /// <summary>
        /// 保存配置到设备
        /// </summary>
        public void SaveConfiguration()
        {
            Device.FluctuationRange = FluctuationRange;
            Device.SamplingInterval = SamplingInterval;
        }
    }
}
