using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XAttributeWrapper : XObjectWrapper
    {
        #region Свойства

        private XAttribute Attribute
        {
            get { return (XAttribute)WrappedNode; }
        }

        public override string Value
        {
            get { return Attribute.Value; }
            set { Attribute.Value = value; }
        }

        public override string LocalName
        {
            get { return Attribute.Name.LocalName; }
        }

        public override string NamespaceUri
        {
            get { return Attribute.Name.NamespaceName; }
        }

        public override IXmlNode ParentNode
        {
            get
            {
                if (Attribute.Parent == null)
                {
                    return null;
                }

                return XContainerWrapper.WrapNode(Attribute.Parent);
            }
        }

        #endregion

        #region Конструктор

        public XAttributeWrapper(XAttribute attribute)
            : base(attribute)
        {
        }

        #endregion
    }
}