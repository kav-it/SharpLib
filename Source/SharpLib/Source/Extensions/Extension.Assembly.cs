using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpLib
{
    /// <summary>
    /// Класс расширения для "Assembly"
    /// </summary>
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
        /// Директория расположения сборки
        /// </summary>
        public static string GetDirectoryEx(this Assembly self)
        {
            return Path.GetDirectoryName(self.Location);
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
        public static string GetEmbeddedResourceAsStringEx(this Assembly self, string uriEmbeddedResource)
        {
            var result = string.Empty;

            using (var stream = self.GetEmbeddedResourceAsStreamEx(uriEmbeddedResource))
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

        /// <summary>
        /// Чтение потока данных из Embedded ресурсов
        /// </summary>
        public static Stream GetEmbeddedResourceAsStreamEx(this Assembly self, string uriPath)
        {
            var pathInResources = string.Format("{0}/{1}", self.GetName().Name, uriPath);
            var uri = new EmbeddedResourceUri(pathInResources);

            using (var stream = self.GetManifestResourceStream(uri.DotPath))
            {
                return stream;
            }
        }

        /// <summary>
        /// Извлечение Embedded ресурсов в файл
        /// </summary>
        public static void CopyEmbeddedResourceToFileEx(this Assembly self, string uriPath, bool rewrite = true, string filePath = null)
        {
            var pathInResources = string.Format("{0}/{1}", self.GetName().Name, uriPath);
            var uri = new EmbeddedResourceUri(pathInResources);

            using (var stream = self.GetManifestResourceStream(uri.DotPath))
            {
                if (stream != null)
                {
                    // Есть путь не указан, файл будет скопирование в директорию сборки
                    if (filePath.IsNotValid())
                    {
                        filePath = self.GetDirectoryEx();
                        filePath = PathEx.Combine(filePath, uri.Name);
                    }

                    // Сохранение ресурса в файл
                    if (rewrite || !File.Exists(filePath))
                    {
                        stream.WriteToFileEx(filePath);    
                    }
                }
            }
        }

        #endregion
    }
}