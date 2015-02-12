using System.Xml;
using System.Xml.Linq;

namespace SharpLib
{
    /// <summary>
    /// Метод расширения класса XDocument
    /// </summary>
    public static class ExtensionXDocument
    {
        #region Методы

        /// <summary>
        /// Инициализация документа параметрами по умолчанию
        /// </summary>
        public static XDocument InitEx(this XDocument self)
        {
            self.Declaration = new XDeclaration("1.0", "UTF-8", "true");

            return self;
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        public static XElement AddElementEx(this XDocument self, string name, string value)
        {
            XElement newElem = new XElement(name, value);
            self.Add(newElem);

            return newElem;
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        public static XElement AddElementEx(this XDocument self, string name)
        {
            return self.AddElementEx(name, string.Empty);
        }

        /// <summary>
        /// Сохранение файла (без BOM)
        /// </summary>
        public static void SaveEx(this XDocument self, string path, int ident = 2)
        {
            using (var writer = new XmlTextWriter(path, ExtensionEncoding.Utf8))
            {
                writer.Indentation = ident;
                writer.Formatting = ident == 0 ? Formatting.None : Formatting.Indented;

                self.Save(writer);
            }
        }

        #endregion
    }
}