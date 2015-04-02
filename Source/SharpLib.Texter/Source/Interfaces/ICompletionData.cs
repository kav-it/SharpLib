using System;
using System.Windows.Media;

using SharpLib.Texter.Editing;
using SharpLib.Texter.Document;

namespace SharpLib.Texter.CodeCompletion
{
    public interface ICompletionData
    {
        #region Свойства

        ImageSource Image { get; }

        string Text { get; }

        object Content { get; }

        object Description { get; }

        double Priority { get; }

        #endregion

        #region Методы

        void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs);

        #endregion
    }
}