// ****************************************************************************
//
// Имя файла    : 'Programm.cs'
// Заголовок    : Точка входа программы (базовая прослойка всего SharpLib)
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using SharpLib.Log;

namespace SharpLib
{

    #region Класс Program

    public class Program : ProgramBase
    {
        #region Константы

        private const String APP_LANG_XML = @"Source\lang.xml";

        #endregion

        #region Поля

        private static RoutedUICommand _helpCommand;

        private static XmlDataProvider _xmlLang;

        #endregion

        #region Свойства

        /// <summary>
        /// Активное в текущий момент окно
        /// </summary>
        public static Window CurrentWindow
        {
            get
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.IsActive)
                        return window;
                }
                // Данное приложение не активно в момент вызова функции, находим окно родитель по-другому:
                int winCount = Application.Current.Windows.Count;
                if (winCount > 2)
                    return Application.Current.Windows[winCount - 2];
                if (winCount > 1)
                    return Application.Current.Windows[0];

                return null;
            }
        }

        /// <summary>
        /// Имя файла помощи
        /// </summary>
        public static String HelpFileName { get; set; }

        /// <summary>
        /// Глобальная команда "Help"
        /// </summary>
        public static RoutedUICommand HelpCommand
        {
            get { return _helpCommand; }
            set { _helpCommand = value; }
        }

        /// <summary>
        /// Иконка приложения
        /// </summary>
        public static ImageSource Icon
        {
            get
            {
                ImageSource image = ResourcesWpf.LoadImageSource("app.ico");

                return image;
            }
        }

        /// <summary>
        /// Локализация приложения
        /// </summary>
        public static LocaleLanguage Language
        {
            get { return Localizer.Language; }
            set { Localizer.Language = value; }
        }

        #endregion

        #region Начальная инициализация

        public static void Init(AppConfigBase config)
        {
#if !__DEBUG__
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnAppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnDomainUnhandledException);
#endif

            // Инициализация базового класса
            Init(config, "App.Gui");

            InitCommand();
            InitLang();

            Application.Current.Activated += OnActivated;
            Application.Current.Exit += OnExit;
        }

        private static void OnActivated(object sender, EventArgs e)
        {
            String title = Title + " (" + Version + ")";

#if __DEBUG__
            title += " *** Отладочная версия ***";
#endif

            Application.Current.MainWindow.Title = title;
            Application.Current.Activated -= OnActivated;

            // Добавление команды Help "F1"
            CommandBinding bind = new CommandBinding(Program.HelpCommand, CommandExecutedHelp, CanCommandExecutedHelp);
            Application.Current.MainWindow.CommandBindings.Add(bind);
        }

        private static void OnExit(object sender, ExitEventArgs e)
        {
            Uninit();
        }

        private static void InitCommand()
        {
            InputGestureCollection inputHelp = new InputGestureCollection();
            inputHelp.Add(new KeyGesture(Key.F1, ModifierKeys.None, "F1"));
            _helpCommand = new RoutedUICommand("Помощь", "Help", typeof(Program), inputHelp);
        }

        #endregion Начальная инициализация

        #region Работа с ресурсами

        public static Object GetResourceXaml(String resourceKey)
        {
            Object obj;
            try
            {
                obj = Application.Current.FindResource(resourceKey);
            }
            catch (Exception)
            {
                return null;
            }

            return obj;
        }

        #endregion Работа с ресурсами

        #region Работа с локализацией

        private static void InitLang()
        {
            _xmlLang = new XmlDataProvider();
            _xmlLang.Source = new Uri(@"pack://application:,,,/" + APP_LANG_XML, UriKind.Absolute);

            Localizer.Register(_xmlLang);
        }

        public static void BindingLang(FrameworkElement element, DependencyProperty dp, String xpath)
        {
            Binding binding = new Binding
                {
                    Source = _xmlLang,
                    XPath = xpath
                };
            element.SetBinding(dp, binding);
        }

        #endregion Работа с локализацией

        #region Обработка исключений

        private static void OnAppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            WindowException window = new WindowException(e.Exception);
            window.ShowDialog();

            e.Handled = true;
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Thread thread = Thread.CurrentThread;
            Boolean isTerminate = e.IsTerminating;

            Application.Current.Dispatcher.Invoke(new Action(() =>
                                                  {
                                                      WindowException window = new WindowException(ex, thread, isTerminate);
                                                      window.ShowDialog();
                                                  })
                );

            if (isTerminate)
                Environment.Exit(0);
        }

        public static String GetExceptionMessage(Exception ex)
        {
            String text = "";
            do
            {
                text += ex.Message + "\r\n";

                ex = ex.InnerException;
            } while (ex != null);

            text = text.TrimEnd("\r\n");

            return text;
        }

        public static String GetStackTrace(Exception ex)
        {
            String text = "";
            do
            {
                text += GetStackTraceInternal(ex);

                ex = ex.InnerException;
            } while (ex != null);

            text = text.TrimEnd("\r\n");

            return text;
        }

        private static String GetStackTraceInternal(Exception ex)
        {
            string infoString = String.Empty;

            // Generate your stack trace class and provide it with your exception
            // fNeedFileInfo is set to true so it gets the file name, number, etc. Otherwise it won't
            StackTrace stackTrace = new StackTrace(ex, true);

            // This for loop will navigate through the stack of your exception and display the whole trace
            // of the call to the method that caused the exception. In this case, just one method is displayed
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);

                if (String.IsNullOrEmpty(frame.GetFileName()) == false)
                {
                    infoString += String.Format("File/Line: {0}:{1}, Method: {2} \r\n",
                                                Path.GetFileName(frame.GetFileName()),
                                                frame.GetFileLineNumber(),
                                                frame.GetMethod()
                        );
                }
            }

            return infoString;
        }

        #endregion Обработка исключений

        #region Обработка команд

        /// <summary>
        /// Команда "Help (F1)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CommandExecutedHelp(object sender, ExecutedRoutedEventArgs e)
        {
            Files.RunExe(HelpFileName, "");
        }

        private static void CanCommandExecutedHelp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Files.IsExists(HelpFileName);
        }

        #endregion Обработка команд
    }

    #endregion Класс Program
}