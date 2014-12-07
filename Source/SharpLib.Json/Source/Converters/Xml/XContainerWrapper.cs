using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XContainerWrapper : XObjectWrapper
    {
        #region Поля

        private IList<IXmlNode> _childNodes;

        #endregion

        #region Свойства

        private XContainer Container
        {
            get { return (XContainer)WrappedNode; }
        }

        public override IList<IXmlNode> ChildNodes
        {
            get { return _childNodes ?? (_childNodes = Container.Nodes().Select(WrapNode).ToList()); }
        }

        public override IXmlNode ParentNode
        {
            get
            {
                if (Container.Parent == null)
                {
                    return null;
                }

                return WrapNode(Container.Parent);
            }
        }

        #endregion

        #region Конструктор

        public XContainerWrapper(XContainer container)
            : base(container)
        {
        }

        #endregion

        #region Методы

        internal static IXmlNode WrapNode(XObject node)
        {
            if (node is XDocument)
            {
                return new XDocumentWrapper((XDocument)node);
            }
            if (node is XElement)
            {
                return new XElementWrapper((XElement)node);
            }
            if (node is XContainer)
            {
                return new XContainerWrapper((XContainer)node);
            }
            if (node is XProcessingInstruction)
            {
                return new XProcessingInstructionWrapper((XProcessingInstruction)node);
            }
            if (node is XText)
            {
                return new XTextWrapper((XText)node);
            }
            if (node is XComment)
            {
                return new XCommentWrapper((XComment)node);
            }
            if (node is XAttribute)
            {
                return new XAttributeWrapper((XAttribute)node);
            }
            if (node is XDocumentType)
            {
                return new XDocumentTypeWrapper((XDocumentType)node);
            }
            return new XObjectWrapper(node);
        }

        public override IXmlNode AppendChild(IXmlNode newChild)
        {
            Container.Add(newChild.WrappedNode);
            _childNodes = null;

            return newChild;
        }

        #endregion
    }
}