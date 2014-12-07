using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SharpLib.Json
{
    public class XmlNodeConverter : JsonConverter
    {
        #region Константы

        private const string C_DATA_NAME = "#cdata-section";

        private const string COMMENT_NAME = "#comment";

        private const string DECLARATION_NAME = "?xml";

        private const string JSON_NAMESPACE_URI = "http://james.newtonking.com/projects/json";

        private const string SIGNIFICANT_WHITESPACE_NAME = "#significant-whitespace";

        private const string TEXT_NAME = "#text";

        private const string WHITESPACE_NAME = "#whitespace";

        #endregion

        #region Свойства

        public string DeserializeRootElementName { get; set; }

        public bool WriteArrayAttribute { get; set; }

        public bool OmitRootObject { get; set; }

        #endregion

        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IXmlNode node = WrapXml(value);

            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            PushParentNamespaces(node, manager);

            if (!OmitRootObject)
            {
                writer.WriteStartObject();
            }

            SerializeNode(writer, node, manager, !OmitRootObject);

            if (!OmitRootObject)
            {
                writer.WriteEndObject();
            }
        }

        private IXmlNode WrapXml(object value)
        {
            if (value is XObject)
            {
                return XContainerWrapper.WrapNode((XObject)value);
            }
            if (value is XmlNode)
            {
                return XmlNodeWrapper.WrapNode((XmlNode)value);
            }

            throw new ArgumentException("Value must be an XML object.", "value");
        }

        private void PushParentNamespaces(IXmlNode node, XmlNamespaceManager manager)
        {
            List<IXmlNode> parentElements = null;

            IXmlNode parent = node;
            while ((parent = parent.ParentNode) != null)
            {
                if (parent.NodeType == XmlNodeType.Element)
                {
                    if (parentElements == null)
                    {
                        parentElements = new List<IXmlNode>();
                    }

                    parentElements.Add(parent);
                }
            }

            if (parentElements != null)
            {
                parentElements.Reverse();

                foreach (IXmlNode parentElement in parentElements)
                {
                    manager.PushScope();
                    foreach (IXmlNode attribute in parentElement.Attributes)
                    {
                        if (attribute.NamespaceUri == "http://www.w3.org/2000/xmlns/" && attribute.LocalName != "xmlns")
                        {
                            manager.AddNamespace(attribute.LocalName, attribute.Value);
                        }
                    }
                }
            }
        }

        private string ResolveFullName(IXmlNode node, XmlNamespaceManager manager)
        {
            string prefix = (node.NamespaceUri == null || (node.LocalName == "xmlns" && node.NamespaceUri == "http://www.w3.org/2000/xmlns/"))
                ? null
                : manager.LookupPrefix(node.NamespaceUri);

            if (!string.IsNullOrEmpty(prefix))
            {
                return prefix + ":" + node.LocalName;
            }
            return node.LocalName;
        }

        private string GetPropertyName(IXmlNode node, XmlNamespaceManager manager)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Attribute:
                    if (node.NamespaceUri == JSON_NAMESPACE_URI)
                    {
                        return "$" + node.LocalName;
                    }
                    return "@" + ResolveFullName(node, manager);
                case XmlNodeType.CDATA:
                    return C_DATA_NAME;
                case XmlNodeType.Comment:
                    return COMMENT_NAME;
                case XmlNodeType.Element:
                    return ResolveFullName(node, manager);
                case XmlNodeType.ProcessingInstruction:
                    return "?" + ResolveFullName(node, manager);
                case XmlNodeType.DocumentType:
                    return "!" + ResolveFullName(node, manager);
                case XmlNodeType.XmlDeclaration:
                    return DECLARATION_NAME;
                case XmlNodeType.SignificantWhitespace:
                    return SIGNIFICANT_WHITESPACE_NAME;
                case XmlNodeType.Text:
                    return TEXT_NAME;
                case XmlNodeType.Whitespace:
                    return WHITESPACE_NAME;
                default:
                    throw new JsonSerializationException("Unexpected XmlNodeType when getting node name: " + node.NodeType);
            }
        }

        private bool IsArray(IXmlNode node)
        {
            IXmlNode jsonArrayAttribute = (node.Attributes != null)
                ? node.Attributes.SingleOrDefault(a => a.LocalName == "Array" && a.NamespaceUri == JSON_NAMESPACE_URI)
                : null;

            return (jsonArrayAttribute != null && XmlConvert.ToBoolean(jsonArrayAttribute.Value));
        }

        private void SerializeGroupedNodes(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
        {
            Dictionary<string, List<IXmlNode>> nodesGroupedByName = new Dictionary<string, List<IXmlNode>>();

            foreach (IXmlNode childNode in node.ChildNodes)
            {
                string nodeName = GetPropertyName(childNode, manager);

                List<IXmlNode> nodes;
                if (!nodesGroupedByName.TryGetValue(nodeName, out nodes))
                {
                    nodes = new List<IXmlNode>();
                    nodesGroupedByName.Add(nodeName, nodes);
                }

                nodes.Add(childNode);
            }

            foreach (KeyValuePair<string, List<IXmlNode>> nodeNameGroup in nodesGroupedByName)
            {
                List<IXmlNode> groupedNodes = nodeNameGroup.Value;

                bool writeArray = groupedNodes.Count != 1 || IsArray(groupedNodes[0]);

                if (!writeArray)
                {
                    SerializeNode(writer, groupedNodes[0], manager, writePropertyName);
                }
                else
                {
                    string elementNames = nodeNameGroup.Key;

                    if (writePropertyName)
                    {
                        writer.WritePropertyName(elementNames);
                    }

                    writer.WriteStartArray();

                    foreach (IXmlNode t in groupedNodes)
                    {
                        SerializeNode(writer, t, manager, false);
                    }

                    writer.WriteEndArray();
                }
            }
        }

        private void SerializeNode(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                    SerializeGroupedNodes(writer, node, manager, writePropertyName);
                    break;
                case XmlNodeType.Element:
                    if (IsArray(node) && node.ChildNodes.All(n => n.LocalName == node.LocalName) && node.ChildNodes.Count > 0)
                    {
                        SerializeGroupedNodes(writer, node, manager, false);
                    }
                    else
                    {
                        manager.PushScope();

                        foreach (IXmlNode attribute in node.Attributes)
                        {
                            if (attribute.NamespaceUri == "http://www.w3.org/2000/xmlns/")
                            {
                                string namespacePrefix = (attribute.LocalName != "xmlns")
                                    ? attribute.LocalName
                                    : string.Empty;
                                string namespaceUri = attribute.Value;

                                manager.AddNamespace(namespacePrefix, namespaceUri);
                            }
                        }

                        if (writePropertyName)
                        {
                            writer.WritePropertyName(GetPropertyName(node, manager));
                        }

                        if (!ValueAttributes(node.Attributes).Any() && node.ChildNodes.Count == 1
                            && node.ChildNodes[0].NodeType == XmlNodeType.Text)
                        {
                            writer.WriteValue(node.ChildNodes[0].Value);
                        }
                        else if (node.ChildNodes.Count == 0 && CollectionUtils.IsNullOrEmpty(node.Attributes))
                        {
                            IXmlElement element = (IXmlElement)node;

                            if (element.IsEmpty)
                            {
                                writer.WriteNull();
                            }
                            else
                            {
                                writer.WriteValue(string.Empty);
                            }
                        }
                        else
                        {
                            writer.WriteStartObject();

                            foreach (IXmlNode t in node.Attributes)
                            {
                                SerializeNode(writer, t, manager, true);
                            }

                            SerializeGroupedNodes(writer, node, manager, true);

                            writer.WriteEndObject();
                        }

                        manager.PopScope();
                    }

                    break;
                case XmlNodeType.Comment:
                    if (writePropertyName)
                    {
                        writer.WriteComment(node.Value);
                    }
                    break;
                case XmlNodeType.Attribute:
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if (node.NamespaceUri == "http://www.w3.org/2000/xmlns/" && node.Value == JSON_NAMESPACE_URI)
                    {
                        return;
                    }

                    if (node.NamespaceUri == JSON_NAMESPACE_URI)
                    {
                        if (node.LocalName == "Array")
                        {
                            return;
                        }
                    }

                    if (writePropertyName)
                    {
                        writer.WritePropertyName(GetPropertyName(node, manager));
                    }
                    writer.WriteValue(node.Value);
                    break;
                case XmlNodeType.XmlDeclaration:
                    IXmlDeclaration declaration = (IXmlDeclaration)node;
                    writer.WritePropertyName(GetPropertyName(node, manager));
                    writer.WriteStartObject();

                    if (!string.IsNullOrEmpty(declaration.Version))
                    {
                        writer.WritePropertyName("@version");
                        writer.WriteValue(declaration.Version);
                    }
                    if (!string.IsNullOrEmpty(declaration.Encoding))
                    {
                        writer.WritePropertyName("@encoding");
                        writer.WriteValue(declaration.Encoding);
                    }
                    if (!string.IsNullOrEmpty(declaration.Standalone))
                    {
                        writer.WritePropertyName("@standalone");
                        writer.WriteValue(declaration.Standalone);
                    }

                    writer.WriteEndObject();
                    break;
                case XmlNodeType.DocumentType:
                    IXmlDocumentType documentType = (IXmlDocumentType)node;
                    writer.WritePropertyName(GetPropertyName(node, manager));
                    writer.WriteStartObject();

                    if (!string.IsNullOrEmpty(documentType.Name))
                    {
                        writer.WritePropertyName("@name");
                        writer.WriteValue(documentType.Name);
                    }
                    if (!string.IsNullOrEmpty(documentType.Public))
                    {
                        writer.WritePropertyName("@public");
                        writer.WriteValue(documentType.Public);
                    }
                    if (!string.IsNullOrEmpty(documentType.System))
                    {
                        writer.WritePropertyName("@system");
                        writer.WriteValue(documentType.System);
                    }
                    if (!string.IsNullOrEmpty(documentType.InternalSubset))
                    {
                        writer.WritePropertyName("@internalSubset");
                        writer.WriteValue(documentType.InternalSubset);
                    }

                    writer.WriteEndObject();
                    break;
                default:
                    throw new JsonSerializationException("Unexpected XmlNodeType when serializing nodes: " + node.NodeType);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            IXmlDocument document = null;
            IXmlNode rootNode = null;

            if (typeof(XObject).IsAssignableFrom(objectType))
            {
                if (objectType != typeof(XDocument) && objectType != typeof(XElement))
                {
                    throw new JsonSerializationException("XmlNodeConverter only supports deserializing XDocument or XElement.");
                }

                XDocument d = new XDocument();
                document = new XDocumentWrapper(d);
                rootNode = document;
            }
            if (typeof(XmlNode).IsAssignableFrom(objectType))
            {
                if (objectType != typeof(XmlDocument))
                {
                    throw new JsonSerializationException("XmlNodeConverter only supports deserializing XmlDocuments");
                }

                XmlDocument d = new XmlDocument();

                d.XmlResolver = null;

                document = new XmlDocumentWrapper(d);
                rootNode = document;
            }

            if (document == null || rootNode == null)
            {
                throw new JsonSerializationException("Unexpected type when converting XML: " + objectType);
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException("XmlNodeConverter can only convert JSON that begins with an object.");
            }

            if (!string.IsNullOrEmpty(DeserializeRootElementName))
            {
                ReadElement(reader, document, rootNode, DeserializeRootElementName, manager);
            }
            else
            {
                reader.Read();
                DeserializeNode(reader, document, manager, rootNode);
            }

            if (objectType == typeof(XElement))
            {
                XElement element = (XElement)document.DocumentElement.WrappedNode;
                element.Remove();

                return element;
            }
            return document.WrappedNode;
        }

        private void DeserializeValue(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, string propertyName, IXmlNode currentNode)
        {
            switch (propertyName)
            {
                case TEXT_NAME:
                    currentNode.AppendChild(document.CreateTextNode(reader.Value.ToString()));
                    break;
                case C_DATA_NAME:
                    currentNode.AppendChild(document.CreateCDataSection(reader.Value.ToString()));
                    break;
                case WHITESPACE_NAME:
                    currentNode.AppendChild(document.CreateWhitespace(reader.Value.ToString()));
                    break;
                case SIGNIFICANT_WHITESPACE_NAME:
                    currentNode.AppendChild(document.CreateSignificantWhitespace(reader.Value.ToString()));
                    break;
                default:

                    if (!string.IsNullOrEmpty(propertyName) && propertyName[0] == '?')
                    {
                        CreateInstruction(reader, document, currentNode, propertyName);
                    }
                    else if (string.Equals(propertyName, "!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                    {
                        CreateDocumentType(reader, document, currentNode);
                    }
                    else
                    {
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            ReadArrayElements(reader, document, propertyName, currentNode, manager);
                            return;
                        }

                        ReadElement(reader, document, currentNode, propertyName, manager);
                    }
                    break;
            }
        }

        private void ReadElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName, XmlNamespaceManager manager)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new JsonSerializationException("XmlNodeConverter cannot convert JSON with an empty property name to XML.");
            }

            Dictionary<string, string> attributeNameValues = ReadAttributeElements(reader, manager);

            string elementPrefix = MiscellaneousUtils.GetPrefix(propertyName);

            if (propertyName.StartsWith('@'))
            {
                string attributeName = propertyName.Substring(1);
                string attributeValue = reader.Value.ToString();

                string attributePrefix = MiscellaneousUtils.GetPrefix(attributeName);

                IXmlNode attribute = (!string.IsNullOrEmpty(attributePrefix))
                    ? document.CreateAttribute(attributeName, manager.LookupNamespace(attributePrefix), attributeValue)
                    : document.CreateAttribute(attributeName, attributeValue);

                ((IXmlElement)currentNode).SetAttributeNode(attribute);
            }
            else
            {
                IXmlElement element = CreateElement(propertyName, document, elementPrefix, manager);

                currentNode.AppendChild(element);

                foreach (KeyValuePair<string, string> nameValue in attributeNameValues)
                {
                    string attributePrefix = MiscellaneousUtils.GetPrefix(nameValue.Key);

                    IXmlNode attribute = (!string.IsNullOrEmpty(attributePrefix))
                        ? document.CreateAttribute(nameValue.Key, manager.LookupNamespace(attributePrefix), nameValue.Value)
                        : document.CreateAttribute(nameValue.Key, nameValue.Value);

                    element.SetAttributeNode(attribute);
                }

                if (reader.TokenType == JsonToken.String
                    || reader.TokenType == JsonToken.Integer
                    || reader.TokenType == JsonToken.Float
                    || reader.TokenType == JsonToken.Boolean
                    || reader.TokenType == JsonToken.Date)
                {
                    element.AppendChild(document.CreateTextNode(ConvertTokenToXmlValue(reader)));
                }
                else if (reader.TokenType == JsonToken.Null)
                {
                }
                else
                {
                    if (reader.TokenType != JsonToken.EndObject)
                    {
                        manager.PushScope();
                        DeserializeNode(reader, document, manager, element);
                        manager.PopScope();
                    }

                    manager.RemoveNamespace(string.Empty, manager.DefaultNamespace);
                }
            }
        }

        private string ConvertTokenToXmlValue(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString();
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                return XmlConvert.ToString(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
            }
            if (reader.TokenType == JsonToken.Float)
            {
                if (reader.Value is decimal)
                {
                    return XmlConvert.ToString((decimal)reader.Value);
                }
                if (reader.Value is float)
                {
                    return XmlConvert.ToString((float)reader.Value);
                }

                return XmlConvert.ToString(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
            }
            if (reader.TokenType == JsonToken.Boolean)
            {
                return XmlConvert.ToString(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
            }
            if (reader.TokenType == JsonToken.Date)
            {
                if (reader.Value is DateTimeOffset)
                {
                    return XmlConvert.ToString((DateTimeOffset)reader.Value);
                }

                DateTime d = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
                return XmlConvert.ToString(d, DateTimeUtils.ToSerializationMode(d.Kind));
            }
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            throw JsonSerializationException.Create(reader, "Cannot get an XML string value from token type '{0}'.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
        }

        private void ReadArrayElements(JsonReader reader, IXmlDocument document, string propertyName, IXmlNode currentNode, XmlNamespaceManager manager)
        {
            string elementPrefix = MiscellaneousUtils.GetPrefix(propertyName);

            IXmlElement nestedArrayElement = CreateElement(propertyName, document, elementPrefix, manager);

            currentNode.AppendChild(nestedArrayElement);

            int count = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                DeserializeValue(reader, document, manager, propertyName, nestedArrayElement);
                count++;
            }

            if (WriteArrayAttribute)
            {
                AddJsonArrayAttribute(nestedArrayElement, document);
            }

            if (count == 1 && WriteArrayAttribute)
            {
                IXmlElement arrayElement = nestedArrayElement.ChildNodes.OfType<IXmlElement>().Single(n => n.LocalName == propertyName);
                AddJsonArrayAttribute(arrayElement, document);
            }
        }

        private void AddJsonArrayAttribute(IXmlElement element, IXmlDocument document)
        {
            element.SetAttributeNode(document.CreateAttribute("json:Array", JSON_NAMESPACE_URI, "true"));

            if (element is XElementWrapper)
            {
                if (element.GetPrefixOfNamespace(JSON_NAMESPACE_URI) == null)
                {
                    element.SetAttributeNode(document.CreateAttribute("xmlns:json", "http://www.w3.org/2000/xmlns/", JSON_NAMESPACE_URI));
                }
            }
        }

        private Dictionary<string, string> ReadAttributeElements(JsonReader reader, XmlNamespaceManager manager)
        {
            Dictionary<string, string> attributeNameValues = new Dictionary<string, string>();
            bool finishedAttributes = false;
            bool finishedElement = false;

            if (reader.TokenType != JsonToken.String
                && reader.TokenType != JsonToken.Null
                && reader.TokenType != JsonToken.Boolean
                && reader.TokenType != JsonToken.Integer
                && reader.TokenType != JsonToken.Float
                && reader.TokenType != JsonToken.Date
                && reader.TokenType != JsonToken.StartConstructor)
            {
                while (!finishedAttributes && !finishedElement && reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            string attributeName = reader.Value.ToString();

                            if (!string.IsNullOrEmpty(attributeName))
                            {
                                char firstChar = attributeName[0];
                                string attributeValue;

                                switch (firstChar)
                                {
                                    case '@':
                                        attributeName = attributeName.Substring(1);
                                        reader.Read();
                                        attributeValue = ConvertTokenToXmlValue(reader);
                                        attributeNameValues.Add(attributeName, attributeValue);

                                        string namespacePrefix;
                                        if (IsNamespaceAttribute(attributeName, out namespacePrefix))
                                        {
                                            manager.AddNamespace(namespacePrefix, attributeValue);
                                        }
                                        break;
                                    case '$':
                                        attributeName = attributeName.Substring(1);
                                        reader.Read();
                                        attributeValue = reader.Value.ToString();

                                        string jsonPrefix = manager.LookupPrefix(JSON_NAMESPACE_URI);
                                        if (jsonPrefix == null)
                                        {
                                            int? i = null;
                                            while (manager.LookupNamespace("json" + i) != null)
                                            {
                                                i = i.GetValueOrDefault() + 1;
                                            }
                                            jsonPrefix = "json" + i;

                                            attributeNameValues.Add("xmlns:" + jsonPrefix, JSON_NAMESPACE_URI);
                                            manager.AddNamespace(jsonPrefix, JSON_NAMESPACE_URI);
                                        }

                                        attributeNameValues.Add(jsonPrefix + ":" + attributeName, attributeValue);
                                        break;
                                    default:
                                        finishedAttributes = true;
                                        break;
                                }
                            }
                            else
                            {
                                finishedAttributes = true;
                            }

                            break;
                        case JsonToken.EndObject:
                            finishedElement = true;
                            break;
                        default:
                            throw new JsonSerializationException("Unexpected JsonToken: " + reader.TokenType);
                    }
                }
            }

            return attributeNameValues;
        }

        private void CreateInstruction(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName)
        {
            if (propertyName == DECLARATION_NAME)
            {
                string version = null;
                string encoding = null;
                string standalone = null;
                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    switch (reader.Value.ToString())
                    {
                        case "@version":
                            reader.Read();
                            version = reader.Value.ToString();
                            break;
                        case "@encoding":
                            reader.Read();
                            encoding = reader.Value.ToString();
                            break;
                        case "@standalone":
                            reader.Read();
                            standalone = reader.Value.ToString();
                            break;
                        default:
                            throw new JsonSerializationException("Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
                    }
                }

                IXmlNode declaration = document.CreateXmlDeclaration(version, encoding, standalone);
                currentNode.AppendChild(declaration);
            }
            else
            {
                IXmlNode instruction = document.CreateProcessingInstruction(propertyName.Substring(1), reader.Value.ToString());
                currentNode.AppendChild(instruction);
            }
        }

        private void CreateDocumentType(JsonReader reader, IXmlDocument document, IXmlNode currentNode)
        {
            string name = null;
            string publicId = null;
            string systemId = null;
            string internalSubset = null;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                switch (reader.Value.ToString())
                {
                    case "@name":
                        reader.Read();
                        name = reader.Value.ToString();
                        break;
                    case "@public":
                        reader.Read();
                        publicId = reader.Value.ToString();
                        break;
                    case "@system":
                        reader.Read();
                        systemId = reader.Value.ToString();
                        break;
                    case "@internalSubset":
                        reader.Read();
                        internalSubset = reader.Value.ToString();
                        break;
                    default:
                        throw new JsonSerializationException("Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
                }
            }

            IXmlNode documentType = document.CreateXmlDocumentType(name, publicId, systemId, internalSubset);
            currentNode.AppendChild(documentType);
        }

        private IXmlElement CreateElement(string elementName, IXmlDocument document, string elementPrefix, XmlNamespaceManager manager)
        {
            string ns = string.IsNullOrEmpty(elementPrefix) ? manager.DefaultNamespace : manager.LookupNamespace(elementPrefix);

            IXmlElement element = (!string.IsNullOrEmpty(ns)) ? document.CreateElement(elementName, ns) : document.CreateElement(elementName);

            return element;
        }

        private void DeserializeNode(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, IXmlNode currentNode)
        {
            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        if (currentNode.NodeType == XmlNodeType.Document && document.DocumentElement != null)
                        {
                            throw new JsonSerializationException(
                                "JSON root object has multiple properties. The root object must have a single property in order to create a valid XML document. Consider specifing a DeserializeRootElementName.");
                        }

                        string propertyName = reader.Value.ToString();
                        reader.Read();

                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            int count = 0;
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                DeserializeValue(reader, document, manager, propertyName, currentNode);
                                count++;
                            }

                            if (count == 1 && WriteArrayAttribute)
                            {
                                IXmlElement arrayElement = currentNode.ChildNodes.OfType<IXmlElement>().Single(n => n.LocalName == propertyName);
                                AddJsonArrayAttribute(arrayElement, document);
                            }
                        }
                        else
                        {
                            DeserializeValue(reader, document, manager, propertyName, currentNode);
                        }
                        break;
                    case JsonToken.StartConstructor:
                        string constructorName = reader.Value.ToString();

                        while (reader.Read() && reader.TokenType != JsonToken.EndConstructor)
                        {
                            DeserializeValue(reader, document, manager, constructorName, currentNode);
                        }
                        break;
                    case JsonToken.Comment:
                        currentNode.AppendChild(document.CreateComment((string)reader.Value));
                        break;
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        return;
                    default:
                        throw new JsonSerializationException("Unexpected JsonToken when deserializing node: " + reader.TokenType);
                }
            } while (reader.TokenType == JsonToken.PropertyName || reader.Read());
        }

        private bool IsNamespaceAttribute(string attributeName, out string prefix)
        {
            if (attributeName.StartsWith("xmlns", StringComparison.Ordinal))
            {
                if (attributeName.Length == 5)
                {
                    prefix = string.Empty;
                    return true;
                }
                if (attributeName[5] == ':')
                {
                    prefix = attributeName.Substring(6, attributeName.Length - 6);
                    return true;
                }
            }
            prefix = null;
            return false;
        }

        private IEnumerable<IXmlNode> ValueAttributes(IEnumerable<IXmlNode> c)
        {
            return c.Where(a => a.NamespaceUri != JSON_NAMESPACE_URI);
        }

        public override bool CanConvert(Type valueType)
        {
            if (typeof(XObject).IsAssignableFrom(valueType))
            {
                return true;
            }
            if (typeof(XmlNode).IsAssignableFrom(valueType))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}