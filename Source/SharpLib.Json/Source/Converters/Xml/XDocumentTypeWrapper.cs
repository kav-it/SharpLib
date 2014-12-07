using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XDocumentTypeWrapper : XObjectWrapper, IXmlDocumentType
    {
        #region ����

        private readonly XDocumentType _documentType;

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

        public XDocumentTypeWrapper(XDocumentType documentType)
            : base(documentType)
        {
            _documentType = documentType;
        }

        #endregion
    }
}