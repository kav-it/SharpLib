using System;
using System.ComponentModel;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad
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