using System.Windows;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    public class OverloadInsightWindow : InsightWindow
    {
        #region Поля

        private readonly OverloadViewer overloadViewer = new OverloadViewer();

        #endregion

        #region Свойства

        public IOverloadProvider Provider
        {
            get { return overloadViewer.Provider; }
            set { overloadViewer.Provider = value; }
        }

        #endregion

        #region Конструктор

        public OverloadInsightWindow(TextArea textArea)
            : base(textArea)
        {
            overloadViewer.Margin = new Thickness(2, 0, 0, 0);
            Content = overloadViewer;
        }

        #endregion

        #region Методы

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && Provider != null && Provider.Count > 1)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        e.Handled = true;
                        overloadViewer.ChangeIndex(-1);
                        break;
                    case Key.Down:
                        e.Handled = true;
                        overloadViewer.ChangeIndex(+1);
                        break;
                }
                if (e.Handled)
                {
                    UpdateLayout();
                    UpdatePosition();
                }
            }
        }

        #endregion
    }
}