using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Окно ввода текстового значения
    /// </summary>
    public partial class WindowPromt
    {
        #region Поля

        /// <summary>
        /// Проверка ввода пустого значения
        /// </summary>
        private readonly bool _validateEmpty;

        #endregion

        #region Свойства

        /// <summary>
        /// Введенное значение
        /// </summary>
        public string Value
        {
            get { return PART_textBox.Text; }
            set
            {
                PART_textBox.Text = value;
                PART_textBox.SelectionStart = 0;
                PART_textBox.SelectionLength = PART_textBox.Text.Length;
            }
        }

        #endregion

        #region Конструктор

        public WindowPromt(string title, string value, string watermark, bool isCombo, bool validateEmpty)
        {
            InitializeComponent();

            Owner = Gui.GetActiveWindow();
            ShowInTaskbar = false;
            Title = title;
            Value = value;
            _validateEmpty = validateEmpty;

            if (watermark.IsValid())
            {
                PART_textBox.WatermarkText = watermark;
            }

            if (isCombo == false)
            {
                PART_comboBox.Visibility = Visibility.Collapsed;
                PART_textBox.Focus();
            }
            else
            {
                PART_textBox.Visibility = Visibility.Collapsed;
                PART_comboBox.Focus();
            }
        }

        #endregion

        #region Методы

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
        }

        /// <summary>
        /// Обновление состояний клавиш (в зависимости от значения поля ввода)
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_validateEmpty)
            {
                PART_okCancel.ButtonOk.IsEnabled = PART_textBox.Text.IsValid();
            }
        }

        /// <summary>
        /// Смена текста ввода
        /// </summary>
        private void PART_textBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        /// <summary>
        /// Отображение диалога
        /// </summary>
        private static string Show(string title, string value, string watermark, bool validateEmpty)
        {
            var window = new WindowPromt(title, value, watermark, false, validateEmpty);

            if (window.ShowDialog() == false)
            {
                return null;
            }

            return window.Value;
        }

        /// <summary>
        /// Отображение диалога
        /// </summary>
        public static string ShowText(string title, string value, string watermark, bool validateEmpty)
        {
            return Show(title, value, watermark, validateEmpty);
        }

        #endregion
    }
}