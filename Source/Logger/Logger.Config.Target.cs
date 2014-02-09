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
using System.Xml.Serialization;

namespace SharpLib.Log
{

    #region Класс LoggerConfigTarget

    [XmlInclude(typeof(LoggerConfigTargetFile))]
    [XmlInclude(typeof(LoggerConfigTargetNet))]
    [XmlInclude(typeof(LoggerConfigTargetDatabase))]
    [XmlInclude(typeof(LoggerConfigTargetService))]
    public class LoggerConfigTarget
    {
    }

    #endregion Класс LoggerConfigTarget

    #region Класс LoggerConfigTargetFile

    public class LoggerConfigTargetFile : LoggerConfigTarget
    {
        #region Свойства

        public String Location { get; set; }

        public LoggerConfigTargetFileBackup Backup {get;set;}

        public override string ToString()
        {
            return String.Format("{0}", Location);
        }

        public LoggerConfigTargetFile()
        {
            Backup = new LoggerConfigTargetFileBackup();
        }

        #endregion
    }

    #endregion Класс LoggerConfigTargetFile

    #region Класс LoggerConfigTargetNet

    public class LoggerConfigTargetNet : LoggerConfigTarget
    {
        #region Свойства

        public String Uri { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfigTargetNet()
        {
            Uri = "";
        }

        public override string ToString()
        {
            return Uri;
        }

        #endregion
    }

    #endregion Класс LoggerConfigTargetNet

    #region Класс LoggerConfigTargetDatabase

    public class LoggerConfigTargetDatabase : LoggerConfigTarget
    {
        #region Свойства

        public String Location { get; set; }

        public TargetDatabaseTyp Database { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfigTargetDatabase()
        {
            Database = TargetDatabaseTyp.Unknow;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Location, Database);
        }

        #endregion
    }

    #endregion Класс LoggerConfigTargetDatabase

    #region Класс LoggerConfigTargetNet

    public class LoggerConfigTargetService : LoggerConfigTarget
    {
        #region Свойства

        public String Address { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfigTargetService()
        {
            Address = "";
        }

        public override string ToString()
        {
            return Address;
        }

        #endregion
    }

    #endregion Класс LoggerConfigTargetService


    #region Класс LoggerConfigTargetFileBackup
    public class LoggerConfigTargetFileBackup
    {
        public String MaxSize { get; set; }

        public String MaxCount { get; set; }
    }
    #endregion Класс LoggerConfigTargetFileBackup
}