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
        public static bool IsDebugEx(this Assembly self)
        {
            var result = self
                .GetCustomAttributes(false)
                .OfType<DebuggableAttribute>()
                .Select(debuggableAttribute => debuggableAttribute.IsJITTrackingEnabled)
                .FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Дата/время создания сборки
        /// </summary>
        public static DateTime GetTimeEx(this Assembly self)
        {
            var fileInfo = new FileInfo(self.Location);
            var result = fileInfo.LastWriteTime;

            return result;
        }

        /// <summary>
        /// Версия сборки
        /// </summary>
        public static Version GetVersionEx(this Assembly self)
        {
            var attr = self.GetAssemblyAttributeEx<AssemblyFileVersionAttribute>();

            return new Version(attr.Version);
        }

        /// <summary>
        /// Название сборки
        /// </summary>
        public static string GetTitleEx(this Assembly self)
        {
            var attr = self.GetAssemblyAttributeEx<AssemblyTitleAttribute>();

            return attr.Title;
        }

        /// <summary>
        /// Чтение атрибута из сборки
        /// </summary>
        public static T GetAssemblyAttributeEx<T>(this Assembly self) where T : Attribute
        {
            var attributes = self.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
            {
                return null;
            }
            return attributes.OfType<T>().SingleOrDefault();
        }

        /// <summary>
        /// Чтение Embedded-ресурсов как строку
        /// </summary>
        /// <param name="self">Сборка</param>
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
        public static string GetResourcesEmbeddedEx(this Assembly self, string uriEmbeddedResource)
        {
            var result = string.Empty;

            var pathInResources = string.Format("{0}.{1}", self.GetName().Name, uriEmbeddedResource);

            using (var stream = self.GetManifestResourceStream(pathInResources))
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