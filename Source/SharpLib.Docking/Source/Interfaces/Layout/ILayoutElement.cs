using System.ComponentModel;

namespace SharpLib.Docking
{
    public interface ILayoutElement : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Свойства

        ILayoutContainer Parent { get; }

        ILayoutRoot Root { get; }

        #endregion
    }
}