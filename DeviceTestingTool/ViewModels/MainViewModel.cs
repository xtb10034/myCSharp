using DeviceTestingTool.Models;
using DeviceTestingTool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DeviceTestingTool.ViewModels
{
    /// <summary>
    /// 主窗口的ViewModel
    /// 实现设备测试的主要业务逻辑
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly DeviceFactory _deviceFactory;
        private readonly DataAnalysisService _analysisService;
        private readonly DataStorageService _storageService;
        private readonly LoggingService _loggingService;

        private string _selectedDeviceType;
        private string _deviceId;
        private DeviceBase _currentDevice;
        private TestResult _currentTestResult;
        private bool _isTesting;
        private int _sampleCount = 10;
        private string _statusMessage;
        private ObservableCollection<MeasurementData> _historicalData;
        private PointCollection _chartData;

        public string[] DeviceTypes => _deviceFactory.GetSupportedDeviceTypes();

        public string SelectedDeviceType
        {
            get => _selectedDeviceType;
            set => SetProperty(ref _selectedDeviceType, value);
        }

        public string DeviceId
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        public DeviceBase CurrentDevice
        {
            get => _currentDevice;
            private set => SetProperty(ref _currentDevice, value);
        }

        public TestResult CurrentTestResult
        {
            get => _currentTestResult;
            private set => SetProperty(ref _currentTestResult, value);
        }

        public bool IsTesting
        {
            get => _isTesting;
            private set => SetProperty(ref _isTesting, value);
        }

        public int SampleCount
        {
            get => _sampleCount;
            set
            {
                // 确保采样次数在5-20之间
                if (value >= 5 && value <= 20)
                    SetProperty(ref _sampleCount, value);
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ObservableCollection<MeasurementData> HistoricalData
        {
            get => _historicalData;
            private set => SetProperty(ref _historicalData, value);
        }

        public PointCollection ChartData
        {
            get => _chartData;
            //private set => SetProperty(ref _chartData, value);
            set
            {
                _chartData = value;
                // 关键：通知UI属性已变化
                OnPropertyChanged(nameof(ChartData));
            }
        }

        public PointCollection tmp
        {
            get => _chartData;
            private set => SetProperty(ref _chartData, value);
        }

        public RelayCommand ConnectDeviceCommand { get; }
        public RelayCommand DisconnectDeviceCommand { get; }
        public RelayCommand StartTestingCommand { get; }
        public RelayCommand StopTestingCommand { get; }
        public RelayCommand SaveResultsCommand { get; }
        public RelayCommand LoadHistoryCommand { get; }
        public RelayCommand ConfigureDeviceCommand { get; }

        public MainViewModel()
        {
            _deviceFactory = new DeviceFactory();
            _analysisService = new DataAnalysisService();
            _loggingService = new LoggingService();
            _storageService = new DataStorageService(_loggingService);

            HistoricalData = new ObservableCollection<MeasurementData>();
            ChartData = new PointCollection();

            // 初始化命令
            ConnectDeviceCommand = new RelayCommand(ConnectDevice, CanConnectDevice);
            DisconnectDeviceCommand = new RelayCommand(DisconnectDevice, CanDisconnectDevice);
            StartTestingCommand = new RelayCommand(StartTesting, CanStartTesting);
            StopTestingCommand = new RelayCommand(StopTesting, CanStopTesting);
            SaveResultsCommand = new RelayCommand(SaveResults, CanSaveResults);
            LoadHistoryCommand = new RelayCommand(LoadHistory);
            ConfigureDeviceCommand = new RelayCommand(ConfigureDevice, CanConfigureDevice);

            // 默认选择第一个设备类型
            if (DeviceTypes.Any())
                SelectedDeviceType = DeviceTypes[0];

            // 生成默认设备ID
            DeviceId = $"DEV-{DateTime.Now:yyyyMMddHHmmss}";

            StatusMessage = "就绪";
        }

        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 连接设备
        /// </summary>
        private void ConnectDevice()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedDeviceType) || string.IsNullOrEmpty(DeviceId))
                {
                    ShowErrorMessage("请选择设备类型并输入设备ID");
                    return;
                }

                StatusMessage = "正在连接设备...";
                
                // 创建设备实例
                CurrentDevice = _deviceFactory.CreateDevice(SelectedDeviceType, DeviceId);
                
                // 尝试连接
                var isConnected = CurrentDevice.Connect();
                
                if (isConnected)
                {
                    StatusMessage = $"设备 {DeviceId} 已连接";
                    _loggingService.LogInformation($"设备 {DeviceId} 连接成功");
                }
                else
                {
                    CurrentDevice = null;
                    StatusMessage = "设备连接失败";
                    _loggingService.LogError($"设备 {DeviceId} 连接失败");
                    ShowErrorMessage("设备连接失败，请重试");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "连接设备时发生错误";
                _loggingService.LogError("连接设备时发生错误", ex);
                ShowErrorMessage($"连接设备失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 判断是否可以连接设备
        /// </summary>
        private bool CanConnectDevice()
        {
            return CurrentDevice == null && !IsTesting;
        }

        /// <summary>
        /// 断开设备连接
        /// </summary>
        private void DisconnectDevice()
        {
            try
            {
                if (CurrentDevice != null)
                {
                    CurrentDevice.Disconnect();
                    _loggingService.LogInformation($"设备 {CurrentDevice.DeviceId} 已断开连接");
                    StatusMessage = $"设备 {CurrentDevice.DeviceId} 已断开连接";
                    CurrentDevice = null;
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("断开设备连接时发生错误", ex);
                ShowErrorMessage($"断开连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 判断是否可以断开设备连接
        /// </summary>
        private bool CanDisconnectDevice()
        {
            return CurrentDevice != null && !IsTesting;
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        private async void StartTesting()
        {
            if (CurrentDevice == null || !CurrentDevice.IsConnected)
            {
                ShowErrorMessage("请先连接设备");
                return;
            }

            try
            {
                IsTesting = true;
                StatusMessage = "正在进行数据采集...";
                _loggingService.LogInformation($"开始对设备 {CurrentDevice.DeviceId} 进行数据采集，采样次数: {SampleCount}");

                // 清空之前的数据
                HistoricalData.Clear();
                ChartData.Clear();
                CurrentTestResult = null;

                var dataList = new List<MeasurementData>();
                var startTime = DateTime.Now;
                _cancellationTokenSource = new CancellationTokenSource();

                // 使用Task.Run执行异步数据采集
                await Task.Run(async () =>
                {
                    for (int i = 0; i < SampleCount; i++)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            break;

                        try
                        {
                            // 采集数据
                            var data = CurrentDevice.CollectData();
                            dataList.Add(data);

                            // 在UI线程更新数据
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                const double xScale = 50;
                                HistoricalData.Add(data);
                                UpdateChartData(data, i);
                                //tmp = ChartData;
                                StatusMessage = $"正在采集数据... ({i + 1}/{SampleCount})";
                            });
                        }
                        catch (Exception ex)
                        {
                            _loggingService.LogError("数据采集失败", ex);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ShowErrorMessage($"数据采集失败: {ex.Message}");
                            });
                            _cancellationTokenSource.Cancel();
                            break;
                        }

                        // 等待采样间隔，最后一次不需要等待
                        if (i < SampleCount - 1)
                            await Task.Delay(CurrentDevice.SamplingInterval, _cancellationTokenSource.Token);
                    }
                }, _cancellationTokenSource.Token);

                // 如果不是被取消的，进行数据分析
                if (!_cancellationTokenSource.Token.IsCancellationRequested && dataList.Any())
                {
                    CurrentTestResult = _analysisService.AnalyzeData(dataList, CurrentDevice, startTime);
                    StatusMessage = CurrentTestResult.IsPassed ? "测试完成，结果: 通过" : "测试完成，结果: 失败";
                    _loggingService.LogInformation($"设备 {CurrentDevice.DeviceId} 测试完成，结果: {StatusMessage}");
                }
                else if (dataList.Any())
                {
                    StatusMessage = "测试已取消";
                    _loggingService.LogInformation($"设备 {CurrentDevice.DeviceId} 测试已取消");
                }
                else
                {
                    StatusMessage = "未采集到任何数据";
                    _loggingService.LogInformation($"设备 {CurrentDevice.DeviceId} 未采集到任何数据");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "测试过程中发生错误";
                _loggingService.LogError("测试过程中发生错误", ex);
                ShowErrorMessage($"测试失败: {ex.Message}");
            }
            finally
            {
                IsTesting = false;
                _cancellationTokenSource?.Dispose();
            }
         
        }

        /// <summary>
        /// 更新图表数据
        /// </summary>
        private void UpdateChartData(MeasurementData data, int index)
        {
            // 简单缩放，使图表在界面上显示更美观
            const double xScale = 50;
            const double yOffset = 50;
            
            // 对于温度和压力使用不同的缩放比例
            double yScale = data.DeviceType == "温度传感器" ? 10 : 1;
            
            ChartData.Add(new Point(
                index * xScale + 20, 
                //yOffset + (data.Value - (data.DeviceType == "温度传感器" ? 25 : 100)) * yScale
                400
            ));
        }

        /// <summary>
        /// 判断是否可以开始测试
        /// </summary>
        private bool CanStartTesting()
        {
            return CurrentDevice != null && CurrentDevice.IsConnected && !IsTesting;
        }

        /// <summary>
        /// 停止测试
        /// </summary>
        private void StopTesting()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                StatusMessage = "正在停止测试...";
                _loggingService.LogInformation($"设备 {CurrentDevice?.DeviceId} 测试已被手动停止");
            }
        }

        /// <summary>
        /// 判断是否可以停止测试
        /// </summary>
        private bool CanStopTesting()
        {
            return IsTesting;
        }

        /// <summary>
        /// 保存测试结果
        /// </summary>
        private void SaveResults()
        {
            if (CurrentTestResult == null)
            {
                ShowErrorMessage("没有可保存的测试结果");
                return;
            }

            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON文件 (*.json)|*.json",
                    FileName = $"TestResult_{CurrentTestResult.DeviceId}_{DateTime.Now:yyyyMMddHHmmss}.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    _storageService.SaveTestResult(CurrentTestResult, dialog.FileName);
                    StatusMessage = $"测试结果已保存到 {dialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("保存测试结果失败", ex);
                ShowErrorMessage($"保存失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 判断是否可以保存测试结果
        /// </summary>
        private bool CanSaveResults()
        {
            return CurrentTestResult != null && !IsTesting;
        }

        /// <summary>
        /// 加载历史记录
        /// </summary>
        private void LoadHistory()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON文件 (*.json)|*.json",
                    Multiselect = true
                };

                if (dialog.ShowDialog() == true)
                {
                    StatusMessage = "正在加载历史数据...";
                    var data = _storageService.LoadHistoricalData(dialog.FileNames);
                    
                    HistoricalData.Clear();
                    foreach (var item in data)
                    {
                        HistoricalData.Add(item);
                    }
                    
                    // 更新图表
                    UpdateChartFromHistoricalData();
                    
                    StatusMessage = $"已加载 {data.Count} 条历史数据";
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError("加载历史数据失败", ex);
                ShowErrorMessage($"加载失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从历史数据更新图表
        /// </summary>
        private void UpdateChartFromHistoricalData()
        {
            ChartData.Clear();
            
            if (!HistoricalData.Any())
                return;

            // 根据设备类型确定缩放比例
            var firstDeviceType = HistoricalData[0].DeviceType;
            double yScale = firstDeviceType == "温度传感器" ? 10 : 1;
            double baseValue = firstDeviceType == "温度传感器" ? 25 : 100;
            
            const double xScale = 50;
            const double yOffset = 50;

            for (int i = 0; i < HistoricalData.Count; i++)
            {
                var data = HistoricalData[i];
                ChartData.Add(new Point(
                    i * xScale + 20,
                    yOffset + (data.Value - baseValue) * yScale
                ));
            }
        }

        /// <summary>
        /// 配置设备
        /// </summary>
        private void ConfigureDevice()
        {
            if (CurrentDevice == null)
            {
                ShowErrorMessage("请先连接设备");
                return;
            }

            var configViewModel = new ConfigurationViewModel(CurrentDevice, SampleCount);
            var configView = new Views.ConfigurationDialog
            {
                DataContext = configViewModel,
                Owner = Application.Current.MainWindow
            };

            if (configView.ShowDialog() == true)
            {
                configViewModel.SaveConfiguration();
                SampleCount = configViewModel.SampleCount;
                StatusMessage = "设备配置已更新";
                _loggingService.LogInformation($"设备 {CurrentDevice.DeviceId} 配置已更新");
            }
        }

        /// <summary>
        /// 判断是否可以配置设备
        /// </summary>
        private bool CanConfigureDevice()
        {
            return CurrentDevice != null && !IsTesting;
        }

        /// <summary>
        /// 显示错误消息对话框
        /// </summary>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
