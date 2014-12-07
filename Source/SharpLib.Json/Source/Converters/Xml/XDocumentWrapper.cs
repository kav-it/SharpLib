using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XDocumentWrapper : XContainerWrapper, IXmlDocument
    {
        #region ��������

        private XDocument Document
        {
            get { return (XDocument)WrappedNode; }
        }

        public override IList<IXmlNode> ChildNodes
        {
            get
            {
                IList<IXmlNode> childNodes = base.ChildNodes;

                if (Document.Declaration != null && childNodes[0].NodeType != XmlNodeType.XmlDeclaration)
                {
                    childNodes.Insert(0, new XDeclarationWrapper(Document.Declaration));
                }

                return childNodes;
            }
        }

        public IXmlElement DocumentElement
        {
            get
            {
                if (Document.Root == null)
                {
                    return null;
                }

                return new XElementWrapper(Document.Root);
            }
        }

        #endregion

        #region �����������

        public XDocumentWrapper(XDocument document)
            : base(document)
        {
        }

        #endregion

        #region ������

        public IXmlNode CreateComment(string text)
        {
            return new XObjectWrapper(new XComment(text));
        }

        public IXmlNode CreateTextNode(string text)
        {
            return new XObjectWrapper(new XText(text));
        }

        public IXmlNode CreateCDataSection(string data)
        {
            return new XObjectWrapper(new XCData(data));
        }

        public IXmlNode CreateWhitespace(string text)
        {
            return new XObjectWrapper(new XText(text));
        }

        public IXmlNode CreateSignificantWhitespace(string text)
        {
            return new XObjectWrapper(new XText(text));
        }

        public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone)
        {
            return new XDeclarationWrapper(new XDeclaration(version, encoding, standalone));
        }

        public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset)
        {
            return new XDocumentTypeWrapper(new XDocumentType(name, publicId, systemId, internalSubset));
        }

        public IXmlNode CreateProcessingInstruction(string target, string data)
        {
            return new XProcessingInstructionWrapper(new XProcessingInstruction(target, data));
        }

        public IXmlElement CreateElement(string elementName)
        {
            return new XElementWrapper(new XElement(elementName));
        }

        public IXmlElement CreateElement(string qualifiedName, string namespaceUri)
        {
            string localName = MiscellaneousUtils.GetLocalName(qualifiedName);
            return new XElementWrapper(new XElement(XName.Get(localName, namespaceUri)));
        }

        public IXmlNode CreateAttribute(string name, string value)
        {
            return new XAttributeWrapper(new XAttribute(name, value));
        }

        public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value)
        {
            string localName = MiscellaneousUtils.GetLocalName(qualifiedName);
            return new XAttributeWrapper(new XAttribute(XName.Get(localName, namespaceUri), value));
        }

        public override IXmlNode AppendChild(IXmlNode newChild)
        {
            XDeclarationWrapper declarationWrapper = newChild as XDeclarationWrapper;
            if (declarationWrapper != null)
            {
                Document.Declaration = declarationWrapper.Declaration;
                return declarationWrapper;
            }
            return base.AppendChild(newChild);
        }

        #endregion
    }
}