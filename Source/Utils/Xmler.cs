// ****************************************************************************
//
// Имя файла    : 'Xml.cs'
// Заголовок    : Модуль работы с XML-файлами
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Resources;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SharpLib
{

    #region Класс Xmler

    /// <summary>
    /// Формат записи XPath    
    /// /  — корневой узел
    /// // — множество узлов удовлетворяющих след условие (детальнее на вики)
    /// *  — любые символы
    /// @  — аттрибут
    /// [] — аналог () в sql, задает условия
    /// </summary>
    public class Xmler
    {
        #region Константы

        private const int IDENT_DEFAULT = 2;

        #endregion

        #region Переменные

        private XmlDocument _doc;

        private String _fileName;

        #endregion Переменные

        #region Свойства

        public static int Ident { get; set; }

        #endregion

        #region Конструктор

        static Xmler()
        {
            Ident = IDENT_DEFAULT;
        }

        public Xmler()
        {
        }

        public Xmler(String filename)
        {
            Load(filename);
        }

        #endregion

        #region Методы

        private static void SplitOnce(String value, String separator, out String part1, out String part2)
        {
            if (value != null)
            {
                int idx = value.IndexOf(separator);
                if (idx >= 0)
                {
                    part1 = value.Substring(0, idx);
                    part2 = value.Substring(idx + separator.Length);
                }
                else
                {
                    part1 = value;
                    part2 = null;
                }
            }
            else
            {
                part1 = "";
                part2 = null;
            }
        }

        private static XmlNode CreateXPath(XmlDocument doc, String xpath)
        {
            XmlNode node = doc;

            foreach (String part in xpath.Substring(1).Split('/'))
            {
                XmlNodeList nodes = node.SelectNodes(part);
                if (nodes.Count > 1) return null;
                else if (nodes.Count == 1)
                {
                    node = nodes[0];
                    continue;
                }

                if (part.StartsWith("@"))
                {
                    var anode = doc.CreateAttribute(part.Substring(1));
                    node.Attributes.Append(anode);
                    node = anode;
                }
                else
                {
                    String elName, attrib = null;
                    if (part.Contains("["))
                    {
                        SplitOnce(part, "[", out elName, out attrib);
                        if (attrib.EndsWith("]") == false) return null;
                        attrib = attrib.Substring(0, attrib.Length - 1);
                    }
                    else elName = part;

                    XmlNode next = doc.CreateElement(elName);
                    node.AppendChild(next);
                    node = next;

                    if (attrib != null)
                    {
                        if (attrib.StartsWith("@") == false) return null;

                        String name;
                        String value;

                        String temp = attrib.Substring(1);
                        SplitOnce(temp, "='", out name, out value);

                        if (String.IsNullOrEmpty(value) || (value.EndsWith("'") == false)) return null;
                        value = value.Substring(0, value.Length - 1);
                        var anode = doc.CreateAttribute(name);
                        anode.Value = value;
                        node.Attributes.Append(anode);
                    }
                }
            }

            return node;
        }

        public Boolean IsPresent(String filename)
        {
            return File.Exists(filename);
        }

        public void Create(String filename, String context)
        {
            try
            {
                _doc = new XmlDocument();
                _doc.LoadXml(context);
                _doc.Save(filename);
                _fileName = filename;
            }
            catch
            {
            }
        }

        public void Load(String filename)
        {
            try
            {
                _doc = new XmlDocument();
                _doc.Load(filename);
                _fileName = filename;
            }
            catch
            {
            }
        }

        #endregion

        #region Чтение данных из XML

        public String Get(String xpath, String defValue, Boolean autoCreate)
        {
            XmlNode node = _doc.SelectSingleNode(xpath);

            if (node != null)
                return node.InnerText;

            if (autoCreate)
                Set(xpath, defValue);

            return defValue;
        }

        public String Get(String xpath, String defValue)
        {
            return Get(xpath, defValue, false);
        }

        public int Get(String xpath, int defValue, Boolean autoCreate)
        {
            String text = defValue.ToString();
            text = Get(xpath, text, autoCreate);

            return int.Parse(text);
        }

        public int Get(String xpath, int defValue)
        {
            return Get(xpath, defValue, false);
        }

        #endregion Чтение данных из XML

        #region Сохранение данных в XML

        public void Set(String xpath, String value)
        {
            if (String.IsNullOrEmpty(xpath) == false)
            {
                XmlNodeList nodes = _doc.SelectNodes(xpath);
                if (nodes.Count == 0) CreateXPath(_doc, xpath).InnerText = value;
                else if (nodes.Count == 1) nodes[0].InnerText = value;
            }

            _doc.Save(_fileName);
        }

        public void Set(String xpath, int value)
        {
            Set(xpath, value.ToString());
        }

        #endregion Сохранение данных в XML

        #region Сериализация данных в XML

        public static void SaveSerialize(String filename, Object obj)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, new UTF8Encoding(false));

            writer.Formatting = ((Xmler.Ident > 0) ? Formatting.Indented : Formatting.None);
            writer.Indentation = Xmler.Ident;

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
            writer.Close();
        }

        public static Object LoadSerialize(String filename, Type typ)
        {
            try
            {
                XmlReader reader = new XmlTextReader(filename);
                XmlSerializer serializer = new XmlSerializer(typ);
                Object obj = serializer.Deserialize(reader);
                reader.Close();
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static String ObjectToString(Object obj)
        {
            try
            {
                using (Utf8StringWriter writer = new Utf8StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(writer, obj);

                    return writer.ToString();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static Object LoadResource(String absolutPath, Type typ, Assembly asm = null)
        {
            try
            {
                StreamResourceInfo streamResource = ResourcesWpf.LoadStreamResource(absolutPath, asm);
                XmlSerializer serializer = new XmlSerializer(typ);
                Object obj = serializer.Deserialize(streamResource.Stream);

                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void SaveResource(String absolutPath, Object obj, Assembly asm = null)
        {
            try
            {
                StreamResourceInfo streamResource = ResourcesWpf.LoadStreamResource(absolutPath, asm);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(streamResource.Stream, obj);
            }
            catch (Exception)
            {
            }
        }

        #endregion Сериализация данных в XML
    }

    #endregion Класс Xmler

    #region Класс Utf8StringWriter

    public sealed class Utf8StringWriter : StringWriter
    {
        #region Свойства

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        #endregion
    }

    #endregion Класс Utf8StringWriter

    #region Класс XmlDocumentEx

    public class XmlDocumentEx
    {
        #region Поля

        private XDocument _doc;

        #endregion

        #region Свойства

        public XElement Root
        {
            get { return _doc.Root; }
        }

        public String Text
        {
            get { return ToText(); }
        }

        #endregion

        #region Конструктор

        public XmlDocumentEx(String root)
        {
            _doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(root));
        }

        public XmlDocumentEx() : this("Root")
        {
        }

        #endregion

        #region Методы

        private String ToText()
        {
            Encoding outputEncoding = new UTF8Encoding(false);

            if (_doc.Declaration != null && _doc.Declaration.Encoding.ToLower() != System.Text.Encoding.UTF8.WebName)
                outputEncoding = System.Text.Encoding.GetEncoding(_doc.Declaration.Encoding);

            using (var stream = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(stream, outputEncoding))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;
                    _doc.Save(writer);
                }

                String result = outputEncoding.GetString(stream.ToArray());

                return result;
            }
        }

        public void Clear()
        {
            if (_doc.Root != null) 
                _doc.Root.RemoveAll();
        }

        public void Save(String filename)
        {
            _doc.Save(filename);
        }

        public void LoadFile(String filename)
        {
            _doc = XDocument.Load(filename);
        }

        public void LoadResource(String path)
        {
            String text = ResourcesWpf.LoadText(path);
            _doc = XDocument.Parse(text);
        }

        public static XmlDocumentEx LoadFileStatic(String filename)
        {
            var doc = new XmlDocumentEx();

            doc.LoadFile(filename);

            return doc;
        }

        #endregion
    }

    #endregion Класс XmlDocumentEx

    #region Класс ExtensionXElement

    public static class ExtensionXElement
    {
        #region Методы

        public static void AddAttributeEx(this XElement elem, String name, String value)
        {
            XAttribute attr = new XAttribute(name, value);
            elem.Add(attr);
        }

        public static XElement AddElementEx(this XElement elem, String name, String value)
        {
            XElement newElem = new XElement(name, value);
            elem.Add(newElem);

            return newElem;
        }

        public static XElement AddElementEx(this XElement elem, String name)
        {
            return elem.AddElementEx(name, "");
        }

        public static String GetAttributeEx(this XElement elem, String name)
        {
            XAttribute attr = elem.Attribute(name);
            if (attr != null)
                return attr.Value;

            return null;
        }

        public static XElement GetElementEx(this XElement elem, String name)
        {
            XElement newElem = elem.Element(name);

            return newElem;
        }

        public static String GetNameEx(this XElement elem)
        {
            String name = elem.Name.LocalName;

            return name;
        }

        public static List<XElement> GetElementsEx(this XElement elem, String name = null)
        {
            IEnumerable<XElement> elements;

            if (name != null)
                elements = elem.Elements(name);
            else
                elements = elem.Elements();

            List<XElement> list = elements.ToList();

            return list;
        }

        public static void ClearEx(this XElement elem)
        {
            elem.RemoveNodes();
        }

        #endregion
    }

    #endregion ExtensionXElement
}