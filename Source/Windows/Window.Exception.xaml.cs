// ****************************************************************************
//
// Имя файла    : 'Window.Exception.cs'
// Заголовок    : Окно "Обработка исключений"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 25/08/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Win32;

namespace SharpLib
{

    #region Класс WindowException

    public partial class WindowException : Window
    {
        #region Константы

        private const String DEFAULT_BODY = "";

        private const String DEFAULT_FILENAME = "report";

        private const String DEFAULT_MAIL_TO = "report@xxx.ru";

        private const String DEFAULT_PASSWORD = "KAVIT_FOREVER";

        private const String DEFAULT_SUBJECT = "report";

        private const String TEXT_INFO = "К сожалению в приложении произошла ошибка. " +
                                         "Вы можете\r\n\r\n" +
                                         "> Продолжить выполнение программы\r\n" +
                                         "> Завершить выполнение программы\r\n" +
                                         "> Сохранить файл отчета на диск\r\n" +
                                         "> Отправить отчет\r\n\r\n" +
                                         "В поле 'Примечание' укажите краткое описание того, что произошло";

        #endregion

        #region Поля

        private Exception _ex;

        private ImageSource _imageSource;

        private Boolean _isTerminate;

        private Thread _thread;

        #endregion

        #region Свойства

        public Boolean IsAddScreenshot
        {
            get
            {
                Boolean result = (PART_checkBoxAddScreenshot.IsChecked == true);

                return result;
            }
        }

        #endregion

        #region Конструктор

        public WindowException(Exception ex) : this(ex, Thread.CurrentThread, false)
        {
        }

        public WindowException(Exception ex, Thread thread, Boolean isTerminate)
        {
            InitializeComponent();

            _ex = ex;
            _thread = thread;
            _imageSource = null;
            Owner = Program.CurrentWindow;
            ShowInTaskbar = false;
            _isTerminate = isTerminate;

            PART_textBlockInfo.Text = TEXT_INFO;
            PART_image.Source = GuiImages.IconError.Source;

            PART_buttonFinish.Click += ButtonClick;
            PART_buttonContinue.Click += ButtonClick;
            PART_buttonSave.Click += ButtonClick;
            PART_buttonSend.Click += ButtonClick;

            if (_isTerminate)
                PART_buttonContinue.IsEnabled = false;
        }

        #endregion

        #region Внутренние методы

        private List<ExceptionImage> GetImages(Boolean includeImages)
        {
            if (includeImages && IsAddScreenshot)
            {
                if (_imageSource != null)
                {
                    MemoryStream stream = Gui.ImageSourceToStream(_imageSource, ImageTyp.Png);
                    Byte[] buffer = stream.ToArray();

                    ExceptionImage image = new ExceptionImage();
                    image.Name = "Secreenshot.png";
                    image.Value = buffer;

                    List<ExceptionImage> list = new List<ExceptionImage>();

                    list.Add(image);

                    return list;
                }
            }

            return null;
        }

        private String GetReport(Boolean includeImages)
        {
            // Составление списка добавленых изображений
            List<ExceptionImage> images = GetImages(includeImages);

            // Формирование отчета об ошибке
            String userInfo = PART_textBoxUserInfo.Text;
            ExceptionReport report = new ExceptionReport(_ex, _thread, userInfo, images);
            // Сериализация объекта (для xml-представления)
            String text = Xmler.ObjectToString(report);

            // 20120920 - Убрано шифрование по соображения открытости для 
            // пользователя передаваемой информации
            // Кодирование данных
            // text = Aes.Encrypt(text, PASSWORD);
            // Добавление заголовка
            // text = "Интересно, что Вы планировали здесь увидеть?" + text;

            return text;
        }

        private void SendMail(String text)
        {
            try
            {
                Mailer mailer = new Mailer();
                String subject = String.Format("{0} [{1}]", DEFAULT_SUBJECT, ProgramBase.Location.ExeName);
                String body = DEFAULT_BODY;

                mailer.AddRecipientTo(DEFAULT_MAIL_TO);
                mailer.AddAttachmentAsFile(text, DEFAULT_FILENAME + ".xml");
                Boolean result = mailer.SendMail(subject, body);

                // Важно: После выполнения передачи почты почтовой программой, текущее окно
                // теряет активность, и как следствие модальный диалог не может определить 
                // текущего родительского окна для отображения
                Activate();

                if (result == false)
                    WindowMessageBox.ShowError("Ошибка отправки отчета\r\n\r\n" + mailer.GetLastError());
                else
                    WindowMessageBox.ShowInformation("Отчет отправлен");
            }
            catch (Exception)
            {
                WindowMessageBox.ShowError("Ошибка отправки отчета");
            }
        }

        private void TabControlSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (PART_tabControl.SelectedItem == PART_tabItemMessage)
            {
                // Заполнение текста сообщения
                String text = GetReport(false);

                PART_textBoxMessage.Text = text;
            }
        }

        #endregion Внутренние методы

        #region Обработчики закладки "General"

        private void ButtonClick(Object sender, RoutedEventArgs e)
        {
            if (sender == PART_buttonFinish)
            {
                // Завершение приложения
                Application.Current.Shutdown();
            }
            else if (sender == PART_buttonContinue)
            {
                // Продолжение выполнения приложения
                Close();
            }
            else if (sender == PART_buttonSave)
            {
                // Сохранение отчета на диск
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = DEFAULT_FILENAME;
                dialog.Filter = "Файлы отчета об ошибках (*.xml)|*.xml";

                if (dialog.ShowDialog(this) == true)
                {
                    String filename = dialog.FileName;
                    String text = GetReport(true);
                    File.WriteAllText(filename, text);
                }
            }
            else if (sender == PART_buttonSend)
            {
                // Отправка отчета
                String text = GetReport(true);

                SendMail(text);
            }
        }

        #endregion Обработчики закладки "General"

        #region Обработчики нажатий закладки "Screenshot"

        private void RegionButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            _imageSource = Screenshot.CaptureUserArea();

            Show();

            PART_buttonSaveScreenshot.IsEnabled = true;
            PART_imageScreenshot.Source = _imageSource;
        }

        private void ScreenButton_Click(object sender, RoutedEventArgs e)
        {
            // Sleep для ожидания закрытия текущего окна, которое не должны быть включено
            // в результирующий Screenshot
            Hide();
            Thread.Sleep(500);

            _imageSource = Screenshot.CaptureScreen();

            Show();

            PART_buttonSaveScreenshot.IsEnabled = true;
            PART_imageScreenshot.Source = _imageSource;
        }

        private void SaveScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (_imageSource != null)
                Dialogs.SaveImage(_imageSource, "Screenshot");
        }

        #endregion Обработчики нажатий закладки "Screenshot"
    }

    #endregion Класс WindowException

    #region Класс ExceptionReport

    public class ExceptionReport
    {
        #region Константы

        private const String DEFAULT_VERSION = "2.0";

        #endregion

        #region Свойства

        public String Version { get; set; }

        public String AppName { get; set; }

        public String AppVersion { get; set; }

        public DateTime AppStamp { get; set; }

        public String Win { get; set; }

        public String Net { get; set; }

        public String Processor { get; set; }

        public String MemorySize { get; set; }

        public String Monitor { get; set; }

        public String Thread { get; set; }

        public DateTime ExStamp { get; set; }

        public String ExMessage { get; set; }

        public String StackInfo { get; set; }

        public String UserInfo { get; set; }

        public List<ExceptionImage> Images { get; set; }

        #endregion

        #region Конструктор

        public ExceptionReport()
        {
        }

        public ExceptionReport(Exception ex, Thread thread, String userInfo, List<ExceptionImage> images)
        {
            String monitorInfo = String.Format("{0}x{1} ({2}%)",
                                               Hardware.MonitorInfo.Width,
                                               Hardware.MonitorInfo.Height,
                                               Hardware.MonitorInfo.Scale * 100);

            Version = DEFAULT_VERSION;
            AppName = ProgramBase.Location.ExePath;
            AppVersion = String.Format("{0}, {1}", ProgramBase.Version, ProgramBase.Version.DateTimeText);
            AppStamp = ProgramBase.StartTime;
            Win = Hardware.OsInfo.VersionString;
            Net = Hardware.DotNetInfo.VersionString;
            Processor = Hardware.ProcInfo.Title;
            MemorySize = Hardware.MemoryInfo.TotalPhySize.ToString();
            Monitor = monitorInfo;
            Thread = String.Format("{0} ({1})", thread.Name, thread.ManagedThreadId);
            ExStamp = DateTime.Now;
            ExMessage = Program.GetExceptionMessage(ex);
            StackInfo = Program.GetStackTrace(ex);
            UserInfo = userInfo;
            Images = images;
        }

        #endregion
    }

    #endregion Класс ExceptionReport

    #region Класс ExceptionImage

    public class ExceptionImage
    {
        #region Свойства

        public String Name { get; set; }

        public Byte[] Value { get; set; }

        #endregion
    }

    #endregion Класс ExceptionImage
}