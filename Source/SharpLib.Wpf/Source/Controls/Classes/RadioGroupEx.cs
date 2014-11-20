using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class RadioGroupEx : ItemsControl
    {
        #region Поля

        public static readonly DependencyProperty HeaderProperty;

        public static readonly DependencyProperty SelectedIndexProperty;

        private static int _groupId;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        [Category("SharpLib")]
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        #endregion

        #region Конструктор

        static RadioGroupEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioGroupEx), new FrameworkPropertyMetadata(typeof(RadioGroupEx)));

            HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(RadioGroupEx), new PropertyMetadata(null));
            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(RadioGroupEx), new PropertyMetadata(-1, SelectionIndexPropertyChangedCallback));

            _groupId = 1;
        }

        public RadioGroupEx()
        {
            _groupId++;
        }

        #endregion

        #region Методы

        private static void SelectionIndexPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as RadioGroupEx;

            if (control == null)
            {
                return;
            }

            var index = (int)args.NewValue;

            control.SetSelectedIndex(index);
        }

        private void SetSelectedIndex(int index)
        {
            var items = Items.Cast<RadioButtonEx>().ToList();

            if (index == -1)
            {
                items.ForEach(x => x.IsChecked = false);

                return;
            }

            if (index < items.Count)
            {
                items[index].IsChecked = true;
            }
        }

        protected override void AddChild(object value)
        {
            var radioButton = value as RadioButtonEx;

            if (radioButton != null)
            {
                radioButton.GroupName = string.Format("Group {0}", _groupId);
            }

            base.AddChild(value);
        }

        public override void EndInit()
        {
            base.EndInit();

            SetSelectedIndex(SelectedIndex);
        }

        #endregion
    }
}