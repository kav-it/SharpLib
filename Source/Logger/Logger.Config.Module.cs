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

    #region Класс LoggerConfigModule

    public class LoggerConfigModule
    {
        #region Свойства

        public String Name { get; set; }

        public LogLevel Level { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfigModule()
        {
            Name = "";
            Level = LogLevel.Off;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Level);
        }

        #endregion
    }

    #endregion Класс LoggerConfigModule
}