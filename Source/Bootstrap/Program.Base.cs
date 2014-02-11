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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;

using SharpLib.Log;

namespace SharpLib
{
    #region Класс ModuleBase

    /// <summary>
    /// Базовый класс модуля
    /// </summary>
    public class ModuleBase
    {
        #region Константы

        private const int DUMP_BYTES_IN_LINE_MAX = 16;

        #endregion

        #region Переменные

        protected LogLevel _modDebug;

        protected int _modIndex;

        protected ModuleError _modLastErr;

        protected String _modName;

        protected Object _modTag;

        protected ModuleTyp _modTyp;

        #endregion Переменные

        #region Свойства

        /// <summary>
        /// Индекс модуля
        /// <para>Задается пользователем при создании. По умолчанию равен 0</para>
        /// </summary>
        public int ModIndex
        {
            get { return _modIndex; }
            set { _modIndex = value; }
        }

        /// <summary>
        /// Имя модуля
        /// <para>
        /// Используется для отладки, записи в лог
        /// </para>
        /// </summary>
        public String ModName
        {
            get { return _modName; }
            set { _modName = value; }
        }

        /// <summary>
        /// Строковое представления последней ошибки модуля
        /// </summary>
        public ModuleError LastError
        {
            get { return _modLastErr; }
            set { _modLastErr = value; }
        }

        /// <summary>
        /// Уровень отладки модуля
        /// </summary>
        public LogLevel ModDebug
        {
            get { return _modDebug; }
            set { _modDebug = value; }
        }

        /// <summary>
        /// Тип модуля
        /// </summary>
        public ModuleTyp ModTyp
        {
            get { return _modTyp; }
        }

        /// <summary>
        /// Произвольное поле, определенное для использования пользователем
        /// </summary>
        public Object ModTag
        {
            get { return _modTag; }
            set { _modTag = value; }
        }

        #endregion

        #region События

        /// <summary>
        /// Событие "Уведомление"
        /// </summary>
        public event ModuleEventHandler ModCallback;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="typ">Тип модуля</param>
        /// <param name="name">Имя модуля</param>
        /// <param name="index">Уникальный идентификатор модуля</param>
        public ModuleBase(ModuleTyp typ, String name, int index)
        {
            _modTyp = typ;
            _modIndex = index;
            _modName = name;
            _modDebug = 0;
            _modLastErr = new ModuleError(ModuleErrorCode.None);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="typ">Тип модуля</param>
        /// <param name="name">Имя модуля</param>
        public ModuleBase(ModuleTyp typ, String name) : this(typ, name, 0)
        {
#if __DEBUG__
            ModDebug = LogLevel.Debug | LogLevel.Info | LogLevel.Warn | LogLevel.Error;
#endif
        }

        #endregion

        #region Генерация событий

        protected virtual void RaiseCallback(object sender, ModuleEventArgs args)
        {
            if (ModCallback != null)
                ModCallback(sender, args);
        }

        protected virtual void RaiseCallbackAsync(object sender, ModuleEventArgs args)
        {
            if (ModCallback != null)
                Application.Current.Dispatcher.Invoke(new Action(() => ModCallback(sender, args)));
        }

        #endregion Генерация событий
    }

    #endregion Класс ModuleBase

    #region Класс ProgramBase

    public class ProgramBase
    {
        #region Свойства

        /// <summary>
        /// Ссылка на Logger
        /// </summary>
        public static Logger Logger { get; private set; }

        /// <summary>
        /// Время запуска приложения
        /// </summary>
        public static DateTime StartTime { get; set; }

        /// <summary>
        /// Версия приложения
        /// </summary>
        public static ModuleVersion Version { get; private set; }

        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        protected static AppConfigBase Config { get; set; }

        /// <summary>
        /// Текстовое название приложения
        /// </summary>
        public static String Title
        {
            get { return Version.Title; }
            set { Version.Title = value; }
        }

        /// <summary>
        /// Данные о расположении приложения (директория, имя, расширение)
        /// </summary>
        public static FileLocation Location { get; private set; }

        public static String ConfigFileName
        {
            get
            {
                return Location.ExePathWithoutExt + ".xml";                 
            }
        }

        #endregion

        #region Начальная инициализация

        protected static void Init(Type typConfig, String threadName)
        {
            Location = new FileLocation(Assembly.GetEntryAssembly().Location);

            // Инициализация логгера (отладка)
            InitLogger();
            // Инициализация внутренних полей
            InitFields(threadName);

            // Инициализация конфигурации
            LoadConfig(typConfig);
        }

        protected static void Uninit()
        {
            SaveConfig();
        }

        private static void InitLogger()
        {
            // Копирование файла конфигурации из ресурсов приложения
            Logger = LogEngine.GetLogger("App");
        }

        private static void InitFields(string threadName)
        {
            Assembly asm = Assembly.GetEntryAssembly();

            Version = new ModuleVersion();
            Version.UpdateFromAssembly(asm);

            StartTime = DateTime.Now;
            Thread.CurrentThread.Name = threadName;
        }

        private static void LoadConfig(Type typConfig)
        {
            var localConfig = (AppConfigBase)Xmler.LoadSerialize(ConfigFileName, typConfig);

            if (localConfig == null)
            {
                localConfig = (AppConfigBase)Reflector.CreateObject(typConfig);
                localConfig.Init();
                // Сохранение новой конфигурации
                SaveConfig(localConfig);
            }

            // Сохранение объекта
            Config = localConfig;
        }

        public static void SaveConfig(AppConfigBase config)
        {
            if (config == null) return;

            Xmler.SaveSerialize(ConfigFileName, config);

            Config = config;
        }

        public static void SaveConfig()
        {
            SaveConfig(Config);
        }

        #endregion Начальная инициализация

    }

    #endregion Класс Program

    #region Интерфейс AppConfigBase

    public class AppConfigBase
    {
        public virtual void Init()
        {
            
        }
    }

    #endregion Интерфейс AppConfigBase
}