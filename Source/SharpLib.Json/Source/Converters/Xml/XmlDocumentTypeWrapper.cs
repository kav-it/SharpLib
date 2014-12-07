using System.Xml;

namespace SharpLib.Json
{
    internal class XmlDocumentTypeWrapper : XmlNodeWrapper, IXmlDocumentType
    {
        #region Поля

        private readonly XmlDocumentType _documentType;

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

        public XmlDocumentTypeWrapper(XmlDocumentType documentType)
            : base(documentType)
        {
            _documentType = documentType;
        }

        #endregion
    }
}