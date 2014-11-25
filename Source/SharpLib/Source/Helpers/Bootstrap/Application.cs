using System;
using System.Reflection;

namespace SharpLib
{
    public class SharpLibApp
    {
        #region Поля

        private static readonly Lazy<SharpLibApp> _instance = new Lazy<SharpLibApp>(() => new SharpLibApp());

        #endregion

        #region Свойства

        /// <summary>
        /// Синглтон приложения
        /// </summary>
        public static SharpLibApp Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Время запуска приложения
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время работы приложения
        /// </summary>
        public TimeSpan WorkTime
        {
            get
            {
                TimeSpan span = DateTime.Now - StartTime;

                return span;
            }
        }

        /// <summary>
        /// Версия приложения
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// Текстовое название приложения
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Данные о расположении приложения (директория, имя, расширение)
        /// </summary>
        public FileLocation Location { get; private set; }

        /// <summary>
        /// Признак отладочной конфигурации
        /// </summary>
        public bool IsDebug { get; private set; }

        /// <summary>
        /// Полное название с версией и признаком отладки
        /// </summary>
        /// <example>
        /// Монитор ServiceD (2.0.1.12) [debug]
        /// </example>
        public string Caption { get; private set; }

        #endregion

        #region Методы

        /// <summary>
        /// Инициализация (должна вызываться в самом начале при старте приложения)
        /// </summary>
        public void Init()
        {
            var asm = Assembly.GetEntryAssembly();

            Version = asm.GetVersionEx();
            Title = asm.GetTitleEx();
            IsDebug = asm.IsDebugEx();
            StartTime = DateTime.Now;
            Location = new FileLocation(asm.Location);

            var debug = IsDebug ? "[debug]" : string.Empty;
            Caption = string.Format("{0} ({1}) {2}", Title, Version, debug);
        }

        #endregion
    }
}