using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XTextWrapper : XObjectWrapper
    {
        #region Свойства

        private XText Text
        {
            get { return (XText)WrappedNode; }
        }

        public override string Value
        {
            get { return Text.Value; }
            set { Text.Value = value; }
        }

        public override IXmlNode ParentNode
        {
            get
            {
                if (Text.Parent == null)
                {
                    return null;
                }

                return XContainerWrapper.WrapNode(Text.Parent);
            }
        }

        #endregion

        #region Конструктор

        public XTextWrapper(XText text)
            : base(text)
        {
        }

        #endregion
    }
}