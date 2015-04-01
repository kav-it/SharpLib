using System;
using System.ComponentModel;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit
{
    public interface ITextEditorComponent : IServiceProvider
    {
        #region Свойства

        TextDocument Document { get; }

        TextEditorOptions Options { get; }

        #endregion

        #region События

        event EventHandler DocumentChanged;

        event PropertyChangedEventHandler OptionChanged;

        #endregion
    }
}