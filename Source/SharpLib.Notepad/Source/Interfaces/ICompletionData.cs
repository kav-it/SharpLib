using System;
using System.Windows.Media;

using SharpLib.Notepad.Editing;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#else
using SharpLib.Notepad.Document;

#endif

namespace SharpLib.Notepad.CodeCompletion
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