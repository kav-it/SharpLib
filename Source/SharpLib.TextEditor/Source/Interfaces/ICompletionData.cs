using System;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Editing;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#else
using ICSharpCode.AvalonEdit.Document;

#endif

namespace ICSharpCode.AvalonEdit.CodeCompletion
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