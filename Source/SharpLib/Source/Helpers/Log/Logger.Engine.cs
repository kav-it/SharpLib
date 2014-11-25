using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpLib.Log
{
    public class LogEngine
    {
        #region Свойства

        /// <summary>
        /// Статический объект ядра
        /// </summary>
        public static LogEngine Instance { get; set; }

        /// <summary>
        /// Список логгеров
        /// </summary>
        private List<Logger> Loggers { get; set; }

        /// <summary>
        /// Конфигурация модуля
        /// </summary>
        private LoggerConfig Config { get; set; }

        /// <summary>
        /// Объекты записи (файлы, база, сеть)
        /// </summary>
        private List<Target> Targets { get; set; }

        /// <summary>
        /// Расположение исполняемого файла
        /// </summary>
        private FileLocation Location { get; set; }

        private String ConfigFilename
        {
            get { return Location.ExePathWithoutExt + ".log.xml"; }
        }

        #endregion

        #region Конструктор

        static LogEngine()
        {
            Instance = new LogEngine();
            Instance.Init();
        }

        internal LogEngine()
        {
        }

        #endregion

        #region Методы

        internal LogLevel GetLevelByModule(String name)
        {
            var module = Config.Modules.FirstOrDefault(x => x.Name == name);
            if (module == null)
            {
                return LogLevel.Off;
            }

            LogLevel result = (module.Level & Config.Level);

            return result;
        }

        internal void WriteLog(LogMessage message)
        {
            foreach (Target target in Targets)
            {
                target.Write(message);
            }
        }

        internal void Init()
        {
            Location = new FileLocation(Assembly.GetEntryAssembly().Location);
            Loggers = new List<Logger>();

            InitConfig();
            InitTargets();
        }

        private void InitConfig()
        {
            LoadConfig();
        }

        private void InitTargets()
        {
            Targets = new List<Target>();
            foreach (LoggerConfigTarget config in Config.Targets)
            {
                Target target;

                if (config is LoggerConfigTargetFile)
                {
                    target = new TargetFile(Location, config as LoggerConfigTargetFile);
                }
                else
                {
                    throw new NotImplementedException();
                }

                Targets.Add(target);
            } // end foreach (создание объектов из конфигурации)
        }

        private void LoadConfig()
        {
            // Загрузка конфигурации из файла
            Config = (LoggerConfig)Xmler.LoadSerialize(ConfigFilename, typeof(LoggerConfig));

            if (Config == null)
            {
                // Загрузка файла конфигурации из основного приложения
                //Config = (LoggerConfig)Xmler.LoadResource(CONFIG_PROJECT_DEFAULT, typeof(LoggerConfig), Assembly.GetEntryAssembly());

                //// Загрузка конфигурации по умолчанию из ресурсов (в основном не было найдено файла конфигурации)
                //if (Config == null)
                //{
                //    Config = (LoggerConfig)Xmler.LoadResource(CONFIG_LIBRARY_DEFAULT, typeof(LoggerConfig), Assembly.GetExecutingAssembly());
                //}

                //if (Config == null)
                //{
                //    throw new NotImplementedException("Ошибка чтения встроенной конфигурации Logger");
                //}
                // Сохранение (для создания файла конфигурации) 
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            Xmler.SaveSerialize(ConfigFilename, Config);
        }

        private Logger RegisterLogger(String name, int index)
        {
            lock (this)
            {
                var logger = Loggers.SingleOrDefault(x => x.Name == name);

                if (logger == null)
                {
                    logger = new Logger();
                    logger.Init(this, name, index);
                    logger.UpdateConfig();
                }

                return logger;
            }
        }

        public static Logger GetLogger(String name, int index = 0)
        {
            return Instance.RegisterLogger(name, index);
        }

        #endregion
    }
}