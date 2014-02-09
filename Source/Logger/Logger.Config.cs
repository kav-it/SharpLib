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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpLib.Log
{

    #region Класс LoggerConfig

    [XmlRoot("Config")]
    public class LoggerConfig
    {
        #region Свойства

        public LogLevel Level { get; set; }

        [XmlArray("Modules")]
        [XmlArrayItem("Module")]
        public List<LoggerConfigModule> Modules { get; set; }

        [XmlArray("Targets")]
        [XmlArrayItem("Target")]
        public List<LoggerConfigTarget> Targets { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfig()
        {
            Modules = new List<LoggerConfigModule>();
            Targets = new List<LoggerConfigTarget>();
        }

        #endregion

        #region Методы


        #endregion
    }

    #endregion Класс LoggerConfig
}