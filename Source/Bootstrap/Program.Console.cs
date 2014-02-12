//*****************************************************************************
//
// Имя файла    : 'WinApi.cs'
// Заголовок    : Реализация работы с WinApi-функциями
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
//*****************************************************************************

using System;
using System.Threading;

namespace SharpLib
{

    #region Класс ProgramConsole

    public class ProgramConsole : ProgramBase
    {
        #region Начальная инициализация

        public static void Init(AppConfigBase config)
        {
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

            Init(config, "App.Console");

            Consoler.Logger = Logger;
        }

        #endregion Начальная инициализация

        #region Обработка исключений домена

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Thread thread = Thread.CurrentThread;
            Boolean isTerminate = e.IsTerminating;

            String message = String.Format("Thread: {0}\r\nException: {1}", thread.Name, ex.ToString());
            Consoler.Print("===== Произошло необработанное исключение ============");
            Consoler.Print(message);
            Consoler.Print("======================================================");
            Consoler.WaitPressKey();

            if (isTerminate)
                Environment.Exit(0);
        }

        #endregion Обработка исключений домена
    }

    #endregion Класс ProgramConsole
}