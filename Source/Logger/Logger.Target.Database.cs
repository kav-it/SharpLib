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

    #region Перечисление TargetDatabaseTyp

    public enum TargetDatabaseTyp
    {
        Unknow = 0,

        Sqlite = 1,

        MsCompact = 2
    }

    #endregion Перечисление TargetDatabaseTyp

    #region Класс TargetDatabase

    public class TargetDatabase : Target
    {
        #region Свойства

        private FileLocation EngineLocation { get; set; }

        private LoggerConfigTargetDatabase Config { get; set; }

        private String Filename { get; set; }

        #endregion

        #region Конструктор

        public TargetDatabase(FileLocation engineLocation, LoggerConfigTargetDatabase config) : base(TargetTyp.Database)
        {
            EngineLocation = engineLocation;

            UpdateConfig(config);
        }

        #endregion

        #region Методы

        public void UpdateConfig(LoggerConfigTargetDatabase config)
        {
            Config = config;

            if (config.Location.IsValid())
                Filename = EngineLocation.DirName + "\\" + config.Location;
            else
            {
                Filename = EngineLocation.ExePathWithoutExt + ".log";

                switch (Config.Database)
                {
                    case TargetDatabaseTyp.MsCompact:
                        Filename += ".sdf";
                        break;
                    case TargetDatabaseTyp.Sqlite:
                        Filename += ".sqlite";
                        break;
                    default:
                        Filename += ".database";
                        break;
                }
            }
        }

        public override void Write(LogMessage message)
        {
            
        }

        #endregion
    }

    #endregion Класс TargetDatabase
}