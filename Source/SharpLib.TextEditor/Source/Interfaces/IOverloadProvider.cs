using System.ComponentModel;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    public interface IOverloadProvider : INotifyPropertyChanged
    {
        #region Свойства

        int SelectedIndex { get; set; }

        int Count { get; }

        string CurrentIndexText { get; }

        object CurrentHeader { get; }

        object CurrentContent { get; }

        #endregion
    }
}