using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpLib
{
    public static class ExtensionAssembly
    {
        #region Методы

        /// <summary>
        /// Признак сборки в отладочной конфигурации
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsDebugEx(this Assembly assembly)
        {
            var result = assembly
                .GetCustomAttributes(false)
                .OfType<DebuggableAttribute>()
                .Select(debuggableAttribute => debuggableAttribute.IsJITTrackingEnabled)
                .FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Дата/время создания сборки
        /// </summary>
        public static DateTime GetTimeEx(this Assembly assembly)
        {
            var fileInfo = new FileInfo(assembly.Location);
            var result = fileInfo.LastWriteTime;

            return result;
        }

        /// <summary>
        /// Версия сборки
        /// </summary>
        public static Version GetVersionEx(this Assembly assembly)
        {
            var attr = assembly.GetAssemblyAttributeEx<AssemblyFileVersionAttribute>();

            return new Version(attr.Version);
        }

        /// <summary>
        /// Название сборки
        /// </summary>
        public static string GetTitleEx(this Assembly assembly)
        {
            var attr = assembly.GetAssemblyAttributeEx<AssemblyTitleAttribute>();

            return attr.Title;
        }

        /// <summary>
        /// Чтение атрибута из сборки
        /// </summary>
        public static T GetAssemblyAttributeEx<T>(this Assembly assembly) where T : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
            {
                return null;
            }
            return attributes.OfType<T>().SingleOrDefault();
        }

        /// <summary>
        /// Чтение Embedded-ресурсов как строку
        /// </summary>
        /// <param name="assembly">Сборка</param>
        /// <param name="uriEmbeddedResource">Путь к ресурсам</param>
        /// <returns></returns>
        /// <example>
        /// SharpLib.Log.Source.Assets.Config.xml, где
        /// SharpLib.Log - имя сборки
        /// 
        /// Source
        ///   + Assets
        ///     + Config.xml      // Расположение файла в директории проекта
        /// </example>
        public static string GetEmbeddedText(this Assembly assembly, string uriEmbeddedResource)
        {
            var result = string.Empty;

            using (var stream = assembly.GetManifestResourceStream(uriEmbeddedResource))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            return result;
        }

        #endregion
    }
}