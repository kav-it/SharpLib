using System;
using System.Collections.Generic;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Highlighting
{
    public interface IHighlighter : IDisposable
    {
        #region Свойства

        IDocument Document { get; }

        HighlightingColor DefaultTextColor { get; }

        #endregion

        #region События

        event HighlightingStateChangedEventHandler HighlightingStateChanged;

        #endregion

        #region Методы

        IEnumerable<HighlightingColor> GetColorStack(int lineNumber);

        HighlightedLine HighlightLine(int lineNumber);

        void UpdateHighlightingState(int lineNumber);

        void BeginHighlighting();

        void EndHighlighting();

        HighlightingColor GetNamedColor(string name);

        #endregion
    }
}