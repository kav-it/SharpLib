using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SharpLib
{
    public class Xmler
    {
        #region Константы

        private const int IDENT_DEFAULT = 2;

        #endregion

        #region Поля

        private XmlDocument _doc;

        #endregion

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

        public Xmler(string filename)
        {
            Load(filename);
        }

        #endregion

        #region Методы

        public void Create(string filename, string context)
        {
            try
            {
                _doc = new XmlDocument();
                _doc.LoadXml(context);
                _doc.Save(filename);
            }
            catch
            {
            }
        }

        public void Load(string filename)
        {
            try
            {
                _doc = new XmlDocument();
                _doc.Load(filename);
            }
            catch
            {
            }
        }

        public static void SaveSerialize(string filename, Object obj)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, new UTF8Encoding(false));

            writer.Formatting = ((Ident > 0) ? Formatting.Indented : Formatting.None);
            writer.Indentation = Ident;

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
            writer.Close();
        }

        public static Object LoadSerialize(string filename, Type typ)
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

        public static string ObjectToString(object obj)
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

        public static object StringToObject(string value, Type typ)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typ);
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(value));
                Object obj = serializer.Deserialize(memStream);

                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Вложенный класс: Utf8StringWriter

        private sealed class Utf8StringWriter : StringWriter
        {
            #region Свойства

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            #endregion
        }

        #endregion
    }
}