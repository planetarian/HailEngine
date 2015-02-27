// Type: GalaSoft.MvvmLight.Helpers.IExecuteWithObject
// Assembly: GalaSoft.MvvmLight.WPF4, Version=0.0.0.0, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// Assembly location: S:\Projects\MetroPM\MoldMaintenance\bin\Release\GalaSoft.MvvmLight.WPF4.dll

namespace Hail.Messenger
{
    /// <summary>
    /// This interface is meant for the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction`1"/> class and can be
    ///             useful if you store multiple WeakAction{T} instances but don't know in advance
    ///             what type T represents.
    /// 
    /// </summary>
    public interface IExecuteWithObject
    {
        /// <summary>
        /// Executes an action.
        /// 
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        ///             to be casted to the appropriate type.</param>
        void ExecuteWithObject(object parameter);
    }
}