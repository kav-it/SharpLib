using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Notepad.CodeCompletion
{
    public class OverloadViewer : Control
    {
        #region Поля

        public static readonly DependencyProperty ProviderProperty =
            DependencyProperty.Register("Provider", typeof(IOverloadProvider), typeof(OverloadViewer));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OverloadViewer));

        #endregion

        #region Свойства

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public IOverloadProvider Provider
        {
            get { return (IOverloadProvider)GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        #endregion

        #region Конструктор

        static OverloadViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverloadViewer),
                new FrameworkPropertyMetadata(typeof(OverloadViewer)));
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var upButton = (Button)Template.FindName("PART_UP", this);
            upButton.Click += (sender, e) =>
            {
                e.Handled = true;
                ChangeIndex(-1);
            };

            var downButton = (Button)Template.FindName("PART_DOWN", this);
            downButton.Click += (sender, e) =>
            {
                e.Handled = true;
                ChangeIndex(+1);
            };
        }

        public void ChangeIndex(int relativeIndexChange)
        {
            var p = Provider;
            if (p != null)
            {
                int newIndex = p.SelectedIndex + relativeIndexChange;
                if (newIndex < 0)
                {
                    newIndex = p.Count - 1;
                }
                if (newIndex >= p.Count)
                {
                    newIndex = 0;
                }
                p.SelectedIndex = newIndex;
            }
        }

        #endregion
    }
}