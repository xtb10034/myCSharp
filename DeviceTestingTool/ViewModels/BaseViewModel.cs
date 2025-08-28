using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DeviceTestingTool.ViewModels
{
    /// <summary>
    /// ViewModel基类，实现INotifyPropertyChanged接口
    /// 提供属性更改通知功能
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性更改事件
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.WriteLine($"属性变化通知：{propertyName}"); // 调试输出
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 设置属性值并触发更改事件
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">字段引用</param>
        /// <param name="value">新值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>是否设置成功</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            //if (EqualityComparer<T>.Default.Equals(field, value))
            //    return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
