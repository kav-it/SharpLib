using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SharpLib.Wpf
{
    public partial class MessageBoxEx
    {
        #region Поля

        public static DependencyProperty CancelVisibilityProperty;

        public static DependencyProperty MessageImageProperty;

        public static DependencyProperty MessageProperty;

        public static DependencyProperty NoVisibilityProperty;

        public static DependencyProperty OkVisibilityProperty;

        public static DependencyProperty YesVisibilityProperty;

        public MessageBoxResult Result;

        #endregion

        #region Свойства

        public String Message
        {
            get { return (String)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public ImageSource MessageImage
        {
            get { return (ImageSource)GetValue(MessageImageProperty); }
            set { SetValue(MessageImageProperty, value); }
        }

        public Visibility YesVisibility
        {
            get { return (Visibility)GetValue(YesVisibilityProperty); }
            set { SetValue(YesVisibilityProperty, value); }
        }

        public Visibility NoVisibility
        {
            get { return (Visibility)GetValue(NoVisibilityProperty); }
            set { SetValue(NoVisibilityProperty, value); }
        }

        public Visibility OkVisibility
        {
            get { return (Visibility)GetValue(OkVisibilityProperty); }
            set { SetValue(OkVisibilityProperty, value); }
        }

        public Visibility CancelVisibility
        {
            get { return (Visibility)GetValue(CancelVisibilityProperty); }
            set { SetValue(CancelVisibilityProperty, value); }
        }

        #endregion

        #region Конструктор

        static MessageBoxEx()
        {
            MessageProperty = DependencyProperty.Register("Message", typeof(String), typeof(MessageBoxEx));
            MessageImageProperty = DependencyProperty.Register("MessageImage", typeof(ImageSource), typeof(MessageBoxEx));
            YesVisibilityProperty = DependencyProperty.Register("YesVisibility", typeof(Visibility), typeof(MessageBoxEx));
            NoVisibilityProperty = DependencyProperty.Register("NoVisibility", typeof(Visibility), typeof(MessageBoxEx));
            OkVisibilityProperty = DependencyProperty.Register("OkVisibility", typeof(Visibility), typeof(MessageBoxEx));
            CancelVisibilityProperty = DependencyProperty.Register("CancelVisibility", typeof(Visibility), typeof(MessageBoxEx));
        }

        public MessageBoxEx()
        {
            InitializeComponent();

            // Установка родительского окна
            var helper = new WindowInteropHelper(this);
            helper.Owner = GetActiveWindow();

            // Настройка внешнего вида окна
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = this;
        }

        #endregion

        #region Методы

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetActiveWindow();

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxResult.None;
                e.Handled = true;
                Close();
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Equals(sender, PART_buttonOk))
            {
                Result = MessageBoxResult.OK;
            }
            else if (Equals(sender, PART_buttonYes))
            {
                Result = MessageBoxResult.Yes;
            }
            else if (Equals(sender, PART_buttonNo))
            {
                Result = MessageBoxResult.No;
            }
            else if (Equals(sender, PART_buttonCancel))
            {
                Result = MessageBoxResult.Cancel;
            }
            else
            {
                return;
            }

            Close();
        }

        private void SetCaption(String caption)
        {
            Title = caption;
            Width = 400;
        }

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

        private void SetButtonVisibility(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.YesNo:
                    OkVisibility = CancelVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.YesNoCancel:
                    OkVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.OK:
                    YesVisibility = NoVisibility = CancelVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.OKCancel:
                    YesVisibility = NoVisibility = Visibility.Collapsed;
                    break;

                default:
                    YesVisibility = NoVisibility = OkVisibility = CancelVisibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SetButtonDefault(MessageBoxResult defaultResult)
        {
            switch (defaultResult)
            {
                case MessageBoxResult.OK:
                    PART_buttonOk.Focus();
                    break;
                case MessageBoxResult.Yes:
                    PART_buttonYes.Focus();
                    break;
                case MessageBoxResult.No:
                    PART_buttonNo.Focus();
                    break;
                case MessageBoxResult.Cancel:
                    PART_buttonCancel.Focus();
                    break;
            }
        }

        private MessageBoxResult ShowCustom(String caption, String message, MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultResult)
        {
            Message = message;
            SetCaption(caption);
            SetButtonVisibility(buttons);
            SetImageSource(image);
            SetButtonDefault(defaultResult);

            ShowDialog();

            return Result;
        }

        public static MessageBoxResult Show(String caption, String message, MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultResult)
        {
            MessageBoxResult result = MessageBoxResult.None;

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBoxEx window = new MessageBoxEx();

                    result = window.ShowCustom(caption, message, buttons, image, defaultResult);
                })
                    );
            }
            else
            {
                MessageBoxEx window = new MessageBoxEx();

                result = window.ShowCustom(caption, message, buttons, image, defaultResult);
            }

            return result;
        }

        public static MessageBoxResult ShowWarning(String message, params object[] args)
        {
            message = string.Format(message, args);

            return Show("Внимание", message, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        public static MessageBoxResult ShowError(String message, params object[] args)
        {
            message = string.Format(message, args);

            return Show("Ошибка", message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        public static bool ShowQuestion(String message, params object[] args)
        {
            message = string.Format(message, args);

            MessageBoxResult result = Show("Вопрос", message, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            return (result == MessageBoxResult.Yes);
        }

        public static MessageBoxResult ShowInformation(String message, params object[] args)
        {
            message = string.Format(message, args);

            return Show("Информация", message, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public static MessageBoxResult ShowBlank(String caption, String message, params object[] args)
        {
            message = string.Format(message, args);

            return Show(caption, message, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        #endregion
    }
}