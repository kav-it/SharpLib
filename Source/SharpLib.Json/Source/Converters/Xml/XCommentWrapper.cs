using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XCommentWrapper : XObjectWrapper
    {
        #region Свойства

        private XComment Text
        {
            get { return (XComment)WrappedNode; }
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

        public XCommentWrapper(XComment text)
            : base(text)
        {
        }

        #endregion
    }
}