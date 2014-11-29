using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace NLog.Config
{
    internal class NLogXmlElement
    {
        #region Свойства

        public string LocalName { get; private set; }

        public Dictionary<string, string> AttributeValues { get; private set; }

        public IList<NLogXmlElement> Children { get; private set; }

        public string Value { get; private set; }

        #endregion

        #region Конструктор

        public NLogXmlElement(string inputUri)
            : this()
        {
            using (var reader = XmlReader.Create(inputUri))
            {
                reader.MoveToContent();
                Parse(reader);
            }
        }

        public NLogXmlElement(XmlReader reader)
            : this()
        {
            Parse(reader);
        }

        private NLogXmlElement()
        {
            AttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Children = new List<NLogXmlElement>();
        }

        #endregion

        #region Методы

        public IEnumerable<NLogXmlElement> Elements(string elementName)
        {
            var result = new List<NLogXmlElement>();

            foreach (var ch in Children)
            {
                if (ch.LocalName.Equals(elementName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(ch);
                }
            }

            return result;
        }

        public string GetRequiredAttribute(string attributeName)
        {
            string value = GetOptionalAttribute(attributeName, null);
            if (value == null)
            {
                throw new NLogConfigurationException("Expected " + attributeName + " on <" + LocalName + " />");
            }

            return value;
        }

        public bool GetOptionalBooleanAttribute(string attributeName, bool defaultValue)
        {
            string value;

            if (!AttributeValues.TryGetValue(attributeName, out value))
            {
                return defaultValue;
            }

            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }

        public string GetOptionalAttribute(string attributeName, string defaultValue)
        {
            string value;

            if (!AttributeValues.TryGetValue(attributeName, out value))
            {
                value = defaultValue;
            }

            return value;
        }

        public void AssertName(params string[] allowedNames)
        {
            foreach (var en in allowedNames)
            {
                if (LocalName.Equals(en, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            throw new InvalidOperationException("Assertion failed. Expected element name '" + string.Join("|", allowedNames) + "', actual: '" + LocalName + "'.");
        }

        private void Parse(XmlReader reader)
        {
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    AttributeValues.Add(reader.LocalName, reader.Value);
                } while (reader.MoveToNextAttribute());

                reader.MoveToElement();
            }

            LocalName = reader.LocalName;

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }

                    if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
                    {
                        Value += reader.Value;
                        continue;
                    }

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        Children.Add(new NLogXmlElement(reader));
                    }
                }
            }
        }

        #endregion
    }
}