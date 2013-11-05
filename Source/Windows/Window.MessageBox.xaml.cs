// ****************************************************************************
//
// Имя файла    : 'Window.MessageBox.cs'
// Заголовок    : Окно "MessageBox" (custom)
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 21/07/2012
//
// ****************************************************************************

using System;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib
{
    public partial class WindowMessageBox : Window
    {
        #region Свойства
        public MessageBoxResult Result;
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
        #endregion Свойства

        #region Свойства зависимости
        public static DependencyProperty MessageProperty;
        public static DependencyProperty MessageImageProperty;
        public static DependencyProperty YesVisibilityProperty;
        public static DependencyProperty NoVisibilityProperty;
        public static DependencyProperty OkVisibilityProperty;
        public static DependencyProperty CancelVisibilityProperty;
        #endregion Свойства зависимости

        #region Конструктор
        static WindowMessageBox()
        {
            MessageProperty          = DependencyProperty.Register("Message",          typeof(String),      typeof(WindowMessageBox));
            MessageImageProperty     = DependencyProperty.Register("MessageImage",     typeof(ImageSource), typeof(WindowMessageBox));
            YesVisibilityProperty    = DependencyProperty.Register("YesVisibility",    typeof(Visibility),  typeof(WindowMessageBox));
            NoVisibilityProperty     = DependencyProperty.Register("NoVisibility",     typeof(Visibility),  typeof(WindowMessageBox));
            OkVisibilityProperty     = DependencyProperty.Register("OkVisibility",     typeof(Visibility),  typeof(WindowMessageBox));
            CancelVisibilityProperty = DependencyProperty.Register("CancelVisibility", typeof(Visibility),  typeof(WindowMessageBox));
        }
        public WindowMessageBox()
        {
            InitializeComponent();

            // Настройка внешнего вида окна
            #warning Доработать
            // this.Owner                 = Program.CurrentWindow;
            this.ShowInTaskbar         = false;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode            = ResizeMode.NoResize;
            this.DataContext           = this;
        }
        #endregion Конструктор

        #region Методы

        #region Обработчики событий
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
            if (sender == PART_buttonOk)
            {
                Result = MessageBoxResult.OK; 
            }
            else if (sender == PART_buttonYes)
            {
                Result = MessageBoxResult.Yes; 
            }
            else if (sender == PART_buttonNo)
            {
                Result = MessageBoxResult.No;
            }
            else if (sender == PART_buttonCancel)
            {
                Result = MessageBoxResult.Cancel;
            }
            else
            {
                return;
            }

            this.Close();
        }
        #endregion Обработчики событий

        #region Внутренние методы установки свойств
        private void SetCaption(String caption)
        {
            this.Title = caption;
            this.Width = 400;
        }
        private void SetImageSource(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    MessageImage = GuiImages.IconError.Source;
                    break;

                case MessageBoxImage.Warning:
                    MessageImage = GuiImages.IconWarning.Source;
                    break;

                case MessageBoxImage.Question:
                    MessageImage = GuiImages.IconQuestion.Source;
                    break;

                case MessageBoxImage.Information:
                    MessageImage = GuiImages.IconInformation.Source;
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
                case MessageBoxResult.OK:     PART_buttonOk.Focus();     break;
                case MessageBoxResult.Yes:    PART_buttonYes.Focus();    break;
                case MessageBoxResult.No:     PART_buttonNo.Focus();     break;
                case MessageBoxResult.Cancel: PART_buttonCancel.Focus(); break;
            }
        }
        #endregion Внутренние методы установки свойств

        #region Методы отображения содержимого
        private MessageBoxResult ShowCustom(String caption, String message, MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultResult)
        {
            Message = message;
            SetCaption(caption);
            SetButtonVisibility(buttons);
            SetImageSource(image);
            SetButtonDefault(defaultResult);

            this.ShowDialog();

            return Result;
        }
        public static MessageBoxResult Show(String caption, String message, MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultResult)
        {
            MessageBoxResult result = MessageBoxResult.None;

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(
                    (Action)(() => 
                    {
                        WindowMessageBox window = new WindowMessageBox();

                        result = window.ShowCustom(caption, message, buttons, image, defaultResult);
                    })
                );
            }

            return result;
        }
        public static MessageBoxResult ShowWarning(String message)
        {
            return Show("Внимание", message, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }
        public static MessageBoxResult ShowError(String message)
        {
            return Show("Ошибка", message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
        public static Boolean ShowQuestion(String message)
        {
            MessageBoxResult result = Show("Вопрос", message, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            return (result == MessageBoxResult.Yes);
        }
        public static MessageBoxResult ShowInformation(String message)
        {
            return Show("Информация", message, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }
        public static MessageBoxResult ShowBlank(String caption, String message)
        {
            return Show(caption, message, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }
        #endregion Методы отображения Методы отображения

        #endregion Методы
    }
}
