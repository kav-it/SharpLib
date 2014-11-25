// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;

namespace SharpLib.Log
{
    public class Logger
    {
        #region Свойства

        /// <summary>
        /// Класс ядра логгирования (внутреннее использование)
        /// </summary>
        private LogEngine Engine { get; set; }

        /// <summary>
        /// Текущий уровень отладки
        /// </summary>
        private LogLevel CurrentLevel { get; set; }

        /// <summary>
        /// Имя модуля (назначается приложением)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Индекс модуля (назначается приложением)
        /// </summary>
        public int Index { get; set; }

        #endregion

        #region Конструктор

        protected internal Logger()
        {
        }

        #endregion

        #region Инициализация

        internal void Init(LogEngine engine, String name, int index)
        {
            Engine = engine;
            Name = name;
            Index = index;
        }

        internal void UpdateConfig()
        {
            CurrentLevel = Engine.GetLevelByModule(Name);
        }

        #endregion Инициализация

        #region Вспомогательные методы

        private void WriteLog(LogLevel level, int index, string message, params object[] args)
        {
            LogLevel mask = CurrentLevel & level;

            if (mask != LogLevel.Off)
            {
                LogMessage logMessage = new LogMessage(level, Name, index, message, args);

                Engine.WriteLog(logMessage);
            }
        }

        #endregion Вспомогательные методы

        #region Вывод в отладку

        public void Debug(string message, params object[] args)
        {
            WriteLog(LogLevel.Debug, Index, message, args);
        }

        public void Info(string message, params object[] args)
        {
            WriteLog(LogLevel.Info, Index, message, args);
        }

        public void Warn(string message, params object[] args)
        {
            WriteLog(LogLevel.Warn, Index, message, args);
        }

        public void Error(string message, params object[] args)
        {
            WriteLog(LogLevel.Error, Index, message, args);
        }

        #endregion Вывод в отладку
    }
}