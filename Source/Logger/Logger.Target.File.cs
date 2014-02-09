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
using System.IO;
using System.Text;

namespace SharpLib.Log
{

    #region Класс TargetFile

    public class TargetFile : Target
    {
        #region Поля

        private Object Locker { get; set; }

        private FileLocation EngineLocation { get; set; }

        private LoggerConfigTargetFile Config { get; set; }

        public String Filename { get; private set; }

        #endregion

        #region Свойства

        #endregion

        #region Конструктор

        internal TargetFile(FileLocation engineLocation, LoggerConfigTargetFile config): base(TargetTyp.File)
        {
            Locker = new object();
            EngineLocation = engineLocation;

            UpdateConfig(config);
        }

        #endregion

        public void UpdateConfig(LoggerConfigTargetFile config)
        {
            Config = config;

            if (config.Location.IsValid())
            {
                Filename = EngineLocation.DirName + "\\" + config.Location;
            }
            else
            {
                Filename = EngineLocation.ExePathWithoutExt + ".log";                
            }
        }

        public override void Write(LogMessage message)
        {
            lock (Locker)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(Filename, true, ExtensionEncoding.Utf8))
                    {
                        String text = message.ToText();
                        sw.WriteLine(text);
                    }
                }
                catch
                {
                    // Неудачная запись в файл
                }
            }
        }
    }

    #endregion Класс TargetFile
}