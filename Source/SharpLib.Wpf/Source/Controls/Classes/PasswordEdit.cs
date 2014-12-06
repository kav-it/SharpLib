using System;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpLib.Wpf.Controls
{
    /// <summary>
    /// Класс редактора ввода пароля
    /// </summary>
    public class PasswordEdit : TextBox
    {
        #region Константы

        private const char CHAR_BULLET = '\u2022';

        #endregion

        #region Поля

        public static readonly DependencyProperty PasswordCharProperty;

        public static readonly DependencyProperty PasswordProperty;

        public static readonly DependencyProperty SecurePasswordProperty;

        public static readonly DependencyProperty ShowPasswordProperty;

        public static readonly DependencyProperty WatermarkTextProperty;

        #endregion

        #region Свойства

        /// <summary>
        /// Пароль (защищенный)
        /// </summary>
        [Browsable(false)]
        public SecureString SecurePassword
        {
            get { return (SecureString)GetValue(SecurePasswordProperty); }
            set { SetValue(SecurePasswordProperty, value); }
        }

        /// <summary>
        /// Пароль (открытый текст)
        /// </summary>
        [Browsable(true)]
        [Category("SharpLib")]
        [Description("Показывать скрытый пароль")]
        public bool ShowPassword
        {
            get { return (bool)GetValue(ShowPasswordProperty); }
            set { SetValue(ShowPasswordProperty, value); }
        }

        /// <summary>
        /// Подсказка когда нет введенного текста
        /// </summary>
        [Browsable(true)]
        [Category("SharpLib")]
        [Description("Подсказка когда нет введенного текста")]
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        /// <summary>
        /// Символ замены введеннных символов
        /// </summary>
        [Browsable(true)]
        [Category("SharpLib")]
        [Description("Символ замены введеннных символов")]
        public char PasswordChar
        {
            get { return (char)GetValue(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }

        /// <summary>
        /// Перекрыто свойство родителя Text для удобства
        /// </summary>
        [Browsable(true)]
        [Category("SharpLib")]
        [Description("Пароль (открытый текст)")]
        public new string Text
        {
            get { return ((string)GetValue(PasswordProperty)); }
            set { SetValue(PasswordProperty, value); }
        }

        #endregion

        #region Конструктор

        static PasswordEdit()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordEdit), new FrameworkPropertyMetadata(typeof(PasswordEdit)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(PasswordEdit), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordEdit), new UIPropertyMetadata(string.Empty, UpdateTextByPassword));
            SecurePasswordProperty = DependencyProperty.Register("SecurePassword", typeof(SecureString), typeof(PasswordEdit), new UIPropertyMetadata(new SecureString()));
            WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(PasswordEdit), new PropertyMetadata(null));
            PasswordCharProperty = DependencyProperty.Register("PasswordChar", typeof(char), typeof(PasswordEdit), new PropertyMetadata(CHAR_BULLET));
            ShowPasswordProperty = DependencyProperty.Register("ShowPassword", typeof(bool), typeof(PasswordEdit), new PropertyMetadata(false, UpdateTextByPassword));
        }

        public PasswordEdit()
        {
            PreviewTextInput += OnPreviewTextInput;
            PreviewKeyDown += OnPreviewKeyDown;
            CommandManager.AddPreviewExecutedHandler(this, PreviewExecutedHandler);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка события "Смена пароля"
        /// </summary>
        private static void UpdateTextByPassword(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var obj = d as PasswordEdit;

            if (obj != null)
            {
                obj.UpdateText(obj.Text);
            }
        }

        /// <summary>
        /// Обработка события Copy/Paste
        /// </summary>
        private static void PreviewExecutedHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Command == ApplicationCommands.Copy ||
                executedRoutedEventArgs.Command == ApplicationCommands.Cut ||
                executedRoutedEventArgs.Command == ApplicationCommands.Paste)
            {
                executedRoutedEventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Обработка события "Смена текста"
        /// </summary>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            AddToSecureString(textCompositionEventArgs.Text);
            textCompositionEventArgs.Handled = true;
        }

        /// <summary>
        /// Обработка события "Нажатие клавиши"
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var pressedKey = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            switch (pressedKey)
            {
                case Key.Space:
                    AddToSecureString(" ");
                    keyEventArgs.Handled = true;
                    break;
                case Key.Back:
                case Key.Delete:
                    if (SelectionLength > 0)
                    {
                        RemoveFromSecureString(SelectionStart, SelectionLength);
                    }
                    else if (pressedKey == Key.Delete && CaretIndex < Text.Length)
                    {
                        RemoveFromSecureString(CaretIndex, 1);
                    }
                    else if (pressedKey == Key.Back && CaretIndex > 0)
                    {
                        int caretIndex = CaretIndex;
                        if (CaretIndex > 0 && CaretIndex < Text.Length)
                        {
                            caretIndex = caretIndex - 1;
                        }
                        RemoveFromSecureString(CaretIndex - 1, 1);
                        CaretIndex = caretIndex;
                    }

                    keyEventArgs.Handled = true;
                    break;
            }
        }

        private void UpdateText(string value)
        {
            var securePassword = new SecureString();
            foreach (char c in value)
            {
                securePassword.AppendChar(c);
            }

            SecurePassword = securePassword;

            base.Text = ShowPassword ? value : new string(PasswordChar, value.Length);    
        }

        private void AddToSecureString(string value)
        {
            if (SelectionLength > 0)
            {
                RemoveFromSecureString(SelectionStart, SelectionLength);
            }

            int caretIndex = CaretIndex;
            var password = Text;

            foreach (char c in value)
            {
                SecurePassword.InsertAt(caretIndex, c);
                password = password.Insert(caretIndex, c.ToString(CultureInfo.InvariantCulture));
                caretIndex++;
            }

            Text = password;
            CaretIndex = caretIndex;
        }

        /// <summary>
        /// Удаление текста из пароля
        /// </summary>
        private void RemoveFromSecureString(int startIndex, int trimLength)
        {
            int caretIndex = CaretIndex;
            var password = Text;

            for (int i = 0; i < trimLength; ++i)
            {
                SecurePassword.RemoveAt(startIndex);
                password = password.Remove(startIndex, 1);
            }

            Text = password;

            CaretIndex = caretIndex;
        }

        #endregion
    }
}