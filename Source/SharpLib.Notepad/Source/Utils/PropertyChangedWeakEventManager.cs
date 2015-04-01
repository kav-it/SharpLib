using System.ComponentModel;

namespace SharpLib.Notepad.Utils
{
    public sealed class PropertyChangedWeakEventManager : WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>
    {
        #region Методы

        protected override void StartListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged += DeliverEvent;
        }

        protected override void StopListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged -= DeliverEvent;
        }

        #endregion
    }
}