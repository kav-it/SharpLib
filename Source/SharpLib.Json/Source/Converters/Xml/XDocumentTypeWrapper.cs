using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XDocumentTypeWrapper : XObjectWrapper, IXmlDocumentType
    {
        #region Поля

        private readonly XDocumentType _documentType;

        #endregion

        #region Свойства

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

        #region Конструктор

        public XDocumentTypeWrapper(XDocumentType documentType)
            : base(documentType)
        {
            _documentType = documentType;
        }

        #endregion
    }
}