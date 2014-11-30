using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SharpLib
{
    public static class ExtensionXElement
    {
        #region Ìועמהû

        public static void AddAttributeEx(this XElement elem, string name, string value)
        {
            XAttribute attr = new XAttribute(name, value);
            elem.Add(attr);
        }

        public static XElement AddElementEx(this XElement elem, string name, string value)
        {
            XElement newElem = new XElement(name, value);
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

        public static bool GetAttributeBoolEx(this XElement elem, string name, bool defaultValue)
        {
            var value = elem.GetAttributeEx(name);

            if (value.IsValid())
            {
                defaultValue = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }

        public static string GetAttributeStringEx(this XElement elem, string name, string defaultValue)
        {
            var value = elem.GetAttributeEx(name);

            if (value.IsValid())
            {
                return value;
            }

            return defaultValue;
        }

        #endregion
    }
}