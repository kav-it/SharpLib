using System.ComponentModel;

namespace SharpLib.Docking.Layout
{
    public interface ILayoutElement : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Свойства

        ILayoutContainer Parent { get; }

        ILayoutRoot Root { get; }

        #endregion
    }
}