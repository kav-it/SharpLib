using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SharpLib
{
    /// <summary>
    /// Метод расширения класса XElement
    /// </summary>
    public static class ExtensionXElement
    {
        #region Методы

        public static void AddAttributeEx(this XElement elem, string name, string value)
        {
            var attr = new XAttribute(name, value);
            elem.Add(attr);
        }

        public static XElement AddElementEx(this XElement elem, string name, string value)
        {
            var newElem = new XElement(name, value);
            elem.Add(newElem);

            return newElem;
        }

        public static XElement AddElementEx(this XElement elem, string name)
        {
            return elem.AddElementEx(name, "");
        }

        public static string GetAttributeEx(this XElement elem, string name, StringComparison comparsion = StringComparison.OrdinalIgnoreCase)
        {
            var xAttr = elem.Attributes().FirstOrDefault(x => x.Name.LocalName.Equals(name, comparsion));

            if (xAttr != null)
            {
                return xAttr.Value;
            }

            return null;
        }

        public static XElement GetElementEx(this XElement elem, string name)
        {
            XElement newElem = elem.Element(name);

            return newElem;
        }

        public static String GetNameEx(this XElement elem)
        {
            String name = elem.Name.LocalName;

            return name;
        }

        public static List<XElement> GetElementsEx(this XElement elem, string name = null)
        {
            IEnumerable<XElement> elements = name != null ? elem.Elements(name) : elem.Elements();

            List<XElement> list = elements.ToList();

            return list;
        }

        public static void ClearEx(this XElement elem)
        {
            elem.RemoveNodes();
        }

        /// <summary>
        /// Чтение атрибута типа "bool"
        /// </summary>
        public static bool GetAttributeBoolEx(this XElement elem, string name, bool defaultValue)
        {
            var value = elem.GetAttributeEx(name);

            if (value.IsValid())
            {
                defaultValue = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }

        /// <summary>
        /// Чтение атрибута типа "string"
        /// </summary>
        public static string GetAttributeStringEx(this XElement elem, string name, string defaultValue)
        {
            var value = elem.GetAttributeEx(name);

            if (value.IsValid())
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Чтение атрибута типа "int"
        /// </summary>
        public static int GetAttributeIntEx(this XElement elem, string name, int defaultValue)
        {
            var value = elem.GetAttributeEx(name);

            if (value.IsValid())
            {
                defaultValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }

        #endregion
    }
}