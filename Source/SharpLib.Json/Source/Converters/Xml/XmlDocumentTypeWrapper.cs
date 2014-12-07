using System.Xml;

namespace SharpLib.Json
{
    internal class XmlDocumentTypeWrapper : XmlNodeWrapper, IXmlDocumentType
    {
        #region ����

        private readonly XmlDocumentType _documentType;

        #endregion

        #region ��������

        public string Name
        {
            get { return _documentType.Name; }
        }

        public string System
        {
            get { return _documentType.SystemId; }
        }

        public string Public
        {
            get { return _documentType.PublicId; }
        }

        public string InternalSubset
        {
            get { return _documentType.InternalSubset; }
        }

        public override string LocalName
        {
            get { return "DOCTYPE"; }
        }

        #endregion

        #region �����������

        public XmlDocumentTypeWrapper(XmlDocumentType documentType)
            : base(documentType)
        {
            _documentType = documentType;
        }

        #endregion
    }
}