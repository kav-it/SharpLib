using System;
using System.Reflection;

namespace SharpLib
{
    public class SharpLibApp
    {
        /// <summary>
        /// Время запуска приложения
        /// </summary>
        public static DateTime StartTime { get; set; }

        /// <summary>
        /// Время работы приложения
        /// </summary>
        public static TimeSpan WorkTime
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
        public static Version Version { get; private set; }

        /// <summary>
        /// Текстовое название приложения
        /// </summary>
        public static string Title { get; private set; }

        /// <summary>
        /// Данные о расположении приложения (директория, имя, расширение)
        /// </summary>
        public static FileLocation Location { get; private set; }

        /// <summary>
        /// Признак отладочной конфигурации
        /// </summary>
        public static bool IsDebug { get; private set; }

        /// <summary>
        /// Полное название с версией и признаком отладки
        /// </summary>
        /// <example>
        /// Монитор ServiceD (2.0.1.12) [debug]
        /// </example>
        public static string TitleFull { get; private set; }
        
        /// <summary>
        /// Инициализация (должна вызываться в самом начале при старте приложения)
        /// </summary>
        public static void Init()
        {
            var asm = Assembly.GetEntryAssembly();

            Version = asm.GetVersionEx();
            Title = asm.GetTitleEx();
            IsDebug = asm.IsDebugEx();
            StartTime = DateTime.Now;
            Location = new FileLocation(asm.Location);

            var debug = IsDebug ? "[debug]" : string.Empty;
            TitleFull = string.Format("{0} ({1}) {2}", Title, Version, debug);
        }
    }
}
