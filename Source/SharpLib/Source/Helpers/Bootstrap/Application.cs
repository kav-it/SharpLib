using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public static ModuleVersion Version { get; private set; }

        /// <summary>
        /// Текстовое название приложения
        /// </summary>
        public static String Title
        {
            get { return Version.Title; }
        }

        /// <summary>
        /// Данные о расположении приложения (директория, имя, расширение)
        /// </summary>
        public static FileLocation Location { get; private set; }

        public static void Init()
        {
            Version = ModuleVersion.UpdateFromAssemblyInstance(Assembly.GetEntryAssembly());
            StartTime = DateTime.Now;
            Thread.CurrentThread.Name = "Application";
        }
    }
}
