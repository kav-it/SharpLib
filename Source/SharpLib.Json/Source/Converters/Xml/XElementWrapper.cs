using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XElementWrapper : XContainerWrapper, IXmlElement
    {
        #region Свойства

        private XElement Element
        {
            get { return (XElement)WrappedNode; }
        }

        public override IList<IXmlNode> Attributes
        {
            get { return Element.Attributes().Select(a => new XAttributeWrapper(a)).Cast<IXmlNode>().ToList(); }
        }

        public override string Value
        {
            get { return Element.Value; }
            set { Element.Value = value; }
        }

        public override string LocalName
        {
            get { return Element.Name.LocalName; }
        }

        public override string NamespaceUri
        {
            get { return Element.Name.NamespaceName; }
        }

        public bool IsEmpty
        {
            get { return Element.IsEmpty; }
        }

        #endregion

        #region Конструктор

        public XElementWrapper(XElement element)
            : base(element)
        {
        }

        #endregion

        #region Методы

        public void SetAttributeNode(IXmlNode attribute)
        {
            XObjectWrapper wrapper = (XObjectWrapper)attribute;
            Element.Add(wrapper.WrappedNode);
        }

        public string GetPrefixOfNamespace(string namespaceUri)
        {
            return Element.GetPrefixOfNamespace(namespaceUri);
        }

        #endregion
    }
}