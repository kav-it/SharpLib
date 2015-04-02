using System;
using System.ComponentModel;

using SharpLib.Texter.Document;

namespace SharpLib.Texter
{
    internal interface ITextEditorComponent : IServiceProvider
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