using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter
{
    internal class TextEditorAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region Свойства

        private TextEditor TextEditor
        {
            get { return (TextEditor)base.Owner; }
        }

        string IValueProvider.Value
        {
            get { return TextEditor.Text; }
        }

        bool IValueProvider.IsReadOnly
        {
            get { return TextEditor.IsReadOnly; }
        }

        #endregion

        #region Конструктор

        public TextEditorAutomationPeer(TextEditor owner)
            : base(owner)
        {
            Debug.WriteLine("TextEditorAutomationPeer was created");
        }

        #endregion

        #region Методы

        void IValueProvider.SetValue(string value)
        {
            TextEditor.Text = value;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }

            if (patternInterface == PatternInterface.Scroll)
            {
                var scrollViewer = TextEditor.ScrollViewer;
                if (scrollViewer != null)
                {
                    return UIElementAutomationPeer.CreatePeerForElement(scrollViewer);
                }
            }

            return base.GetPattern(patternInterface);
        }

        internal void RaiseIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            RaisePropertyChangedEvent(ValuePatternIdentifiers.IsReadOnlyProperty, Boxes.Box(oldValue), Boxes.Box(newValue));
        }

        #endregion
    }
}