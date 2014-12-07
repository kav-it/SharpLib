using System.Xml;

namespace SharpLib.Json
{
    internal class XmlElementWrapper : XmlNodeWrapper, IXmlElement
    {
        #region ����

        private readonly XmlElement _element;

        #endregion

        #region ��������

        public bool IsEmpty
        {
            get { return _element.IsEmpty; }
        }

        #endregion

        #region �����������

        public XmlElementWrapper(XmlElement element)
            : base(element)
        {
            _element = element;
        }

        #endregion

        #region ������

        public void SetAttributeNode(IXmlNode attribute)
        {
            XmlNodeWrapper xmlAttributeWrapper = (XmlNodeWrapper)attribute;

            _element.SetAttributeNode((XmlAttribute)xmlAttributeWrapper.WrappedNode);
        }

        public string GetPrefixOfNamespace(string namespaceUri)
        {
            return _element.GetPrefixOfNamespace(namespaceUri);
        }

        #endregion
    }
}