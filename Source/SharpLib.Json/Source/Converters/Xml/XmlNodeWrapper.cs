using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SharpLib.Json
{
    internal class XmlNodeWrapper : IXmlNode
    {
        #region Поля

        private readonly XmlNode _node;

        private IList<IXmlNode> _childNodes;

        #endregion

        #region Свойства

        public object WrappedNode
        {
            get { return _node; }
        }

        public XmlNodeType NodeType
        {
            get { return _node.NodeType; }
        }

        public virtual string LocalName
        {
            get { return _node.LocalName; }
        }

        public IList<IXmlNode> ChildNodes
        {
            get { return _childNodes ?? (_childNodes = _node.ChildNodes.Cast<XmlNode>().Select(WrapNode).ToList()); }
        }

        public IList<IXmlNode> Attributes
        {
            get
            {
                if (_node.Attributes == null)
                {
                    return null;
                }

                return _node.Attributes.Cast<XmlAttribute>().Select(WrapNode).ToList();
            }
        }

        public IXmlNode ParentNode
        {
            get
            {
                XmlNode node = (_node is XmlAttribute)
                    ? ((XmlAttribute)_node).OwnerElement
                    : _node.ParentNode;

                if (node == null)
                {
                    return null;
                }

                return WrapNode(node);
            }
        }

        public string Value
        {
            get { return _node.Value; }
            set { _node.Value = value; }
        }

        public string NamespaceUri
        {
            get { return _node.NamespaceURI; }
        }

        #endregion

        #region Конструктор

        public XmlNodeWrapper(XmlNode node)
        {
            _node = node;
        }

        #endregion

        #region Методы

        internal static IXmlNode WrapNode(XmlNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    return new XmlElementWrapper((XmlElement)node);
                case XmlNodeType.XmlDeclaration:
                    return new XmlDeclarationWrapper((XmlDeclaration)node);
                case XmlNodeType.DocumentType:
                    return new XmlDocumentTypeWrapper((XmlDocumentType)node);
                default:
                    return new XmlNodeWrapper(node);
            }
        }

        public IXmlNode AppendChild(IXmlNode newChild)
        {
            XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
            _node.AppendChild(xmlNodeWrapper._node);
            _childNodes = null;

            return newChild;
        }

        #endregion
    }
}