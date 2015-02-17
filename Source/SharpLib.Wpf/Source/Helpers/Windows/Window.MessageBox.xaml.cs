using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib.Wpf
{
    public partial class MessageBoxEx
    {
        /// <summary>
        /// Ширина диалога по умолчанию
        /// </summary>
        private const int WIDTH_DEFAULT = 500;

        #region Поля

        /// <summary>
        /// Изображение
        /// </summary>
        public static DependencyProperty MessageImageProperty;

        /// <summary>
        /// Сообщение
        /// </summary>
        public static DependencyProperty MessageProperty;

        /// <summary>
        /// Результат диалога
        /// </summary>
        public MessageBoxExResult Result;

        /// <summary>
        /// Модели кнопков
        /// </summary>
        private readonly List<MessageBoxExModel> _models;

        #endregion

        #region Свойства

        /// <summary>
        /// Сообщение диалога
        /// </summary>
        public string Message
        {
            get { return (String)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Изображение сообещния
        /// </summary>
        public ImageSource MessageImage
        {
            get { return (ImageSource)GetValue(MessageImageProperty); }
            set { SetValue(MessageImageProperty, value); }
        }

        #endregion

        #region Конструктор

        static MessageBoxEx()
        {
            MessageProperty = DependencyProperty.Register("Message", typeof(String), typeof(MessageBoxEx));
            MessageImageProperty = DependencyProperty.Register("MessageImage", typeof(ImageSource), typeof(MessageBoxEx));
        }

        public MessageBoxEx()
        {
            InitializeComponent();

            // Установка родительского окна
            Owner = Gui.GetActiveWindow();

            // Настройка внешнего вида окна
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = this;

            // Модели кнопок
            _models = new List<MessageBoxExModel>();

            _models.Add(new MessageBoxExModel(PART_buttonOk, MessageBoxExResult.Ok, MessageBoxExButtons.Ok));
            _models.Add(new MessageBoxExModel(PART_buttonCancel, MessageBoxExResult.Cancel, MessageBoxExButtons.Cancel));
            _models.Add(new MessageBoxExModel(PART_buttonYes, MessageBoxExResult.Yes, MessageBoxExButtons.Yes));
            _models.Add(new MessageBoxExModel(PART_buttonNo, MessageBoxExResult.No, MessageBoxExButtons.No));
            _models.Add(new MessageBoxExModel(PART_buttonYesToAll, MessageBoxExResult.YesToAll, MessageBoxExButtons.YesToAll));
            _models.Add(new MessageBoxExModel(PART_buttonNoToAll, MessageBoxExResult.NoToAll, MessageBoxExButtons.NoToAll));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка нажатия "Esc"
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxExResult.Unknown;
                e.Handled = true;
                Close();
            }
        }

        /// <summary>
        /// Обработка нажатия кнопкок
        /// </summary>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                Result = ((MessageBoxExModel)button.Tag).Result;

                Close();
            }
        }

        /// <summary>
        /// Установка заголовка окна
        /// </summary>
        private void SetCaption(string caption)
        {
            Title = caption;
            Width = WIDTH_DEFAULT;
        }

        /// <summary>
        /// Установка изображения диалога
        /// </summary>
        private void SetImageSource(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    MessageImage = GuiImages.IconError;
                    break;

                case MessageBoxImage.Warning:
                    MessageImage = GuiImages.IconWarning;
                    break;

                case MessageBoxImage.Question:
                    MessageImage = GuiImages.IconQuestion;
                    break;

                case MessageBoxImage.Information:
                    MessageImage = GuiImages.IconInfo;
                    break;

                default:
                    MessageImage = null;
                    break;
            }
        }

        /// <summary>
        /// Установка видимости групп кнопок
        /// </summary>
        private void SetButtonVisibility(MessageBoxExButtons types)
        {
            _models.ForEach(x => x.Button.Visibility = Visibility.Collapsed);

            var buttons = _models.Where(x => (int)(x.Typ & types) != 0).Select(x => x.Button);

            foreach (var button in buttons)
            {
                button.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Установка фокуса на кнопке по умолчанию
        /// </summary>
        private void SetButtonDefault(MessageBoxExResult defaultResult)
        {
            var button = _models.Where(x => x.Result == defaultResult).Select(x => x.Button).FirstOrDefault();

            if (button != null)
            {
                button.Focus();
            }
        }

        /// <summary>
        /// Отображение диалога
        /// </summary>
        private MessageBoxExResult ShowCustom(string caption, string message, MessageBoxExButtons buttons, MessageBoxImage image, MessageBoxExResult defaultResult)
        {
            Message = message;
            SetCaption(caption);
            SetButtonVisibility(buttons);
            SetImageSource(image);
            SetButtonDefault(defaultResult);

            ShowDialog();

            return Result;
        }

        /// <summary>
        /// Отображение диалога
        /// </summary>
        public static MessageBoxExResult Show(string caption, string message, MessageBoxExButtons buttons, MessageBoxImage image, MessageBoxExResult defaultResult)
        {
            var result = MessageBoxExResult.Unknown;

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var window = new MessageBoxEx();

                    result = window.ShowCustom(caption, message, buttons, image, defaultResult);
                }));
            }
            else
            {
                var window = new MessageBoxEx();

                result = window.ShowCustom(caption, message, buttons, image, defaultResult);
            }

            return result;
        }

        /// <summary>
        /// Диалог "Внимание"
        /// </summary>
        public static MessageBoxExResult ShowWarning(string message, params object[] args)
        {
            message = string.Format(message, args);

            return Show("Внимание", message, MessageBoxExButtons.Ok, MessageBoxImage.Warning, MessageBoxExResult.Ok);
        }

        /// <summary>
        /// Диалог "Ошибка"
        /// </summary>
        public static MessageBoxExResult ShowError(string message, params object[] args)
        {
            message = string.Format(message, args);

            return Show("Ошибка", message, MessageBoxExButtons.Ok, MessageBoxImage.Error, MessageBoxExResult.Ok);
        }

        /// <summary>
        /// Диалог "Вопрос"
        /// </summary>
        public static bool ShowQuestion(string message, params object[] args)
        {
            message = string.Format(message, args);

            var result = Show("Вопрос", message, MessageBoxExButtons.YesNo, MessageBoxImage.Question, MessageBoxExResult.Yes);

            return (result == MessageBoxExResult.Yes);
        }

        /// <summary>
        /// Диалог "Информация"
        /// </summary>
        public static MessageBoxExResult ShowInformation(string message, params object[] args)
        {
            message = string.Format(message, args);

            return Show("Информация", message, MessageBoxExButtons.Ok, MessageBoxImage.Information, MessageBoxExResult.Ok);
        }

        /// <summary>
        /// Диалог "Без изображения"
        /// </summary>
        public static MessageBoxExResult ShowBlank(string caption, string message, params object[] args)
        {
            message = string.Format(message, args);

            return Show(caption, message, MessageBoxExButtons.Ok, MessageBoxImage.None, MessageBoxExResult.Ok);
        }

        #endregion
    }

    /// <summary>
    /// Контейнер для внутреннего использования
    /// </summary>
    internal class MessageBoxExModel
    {
        #region Свойства

        /// <summary>
        /// Кнопка
        /// </summary>
        internal Button Button { get; private set; }

        /// <summary>
        /// Результат, который возвращает кнопка
        /// </summary>
        internal MessageBoxExResult Result { get; private set; }

        /// <summary>
        /// Тип кнопки
        /// </summary>
        internal MessageBoxExButtons Typ { get; private set; }

        #endregion

        #region Конструктор

        public MessageBoxExModel(Button button, MessageBoxExResult result, MessageBoxExButtons typ)
        {
            Button = button;
            Result = result;
            Typ = typ;

            button.Tag = this;
            button.DataContext = this;
        }

        #endregion
    }
}