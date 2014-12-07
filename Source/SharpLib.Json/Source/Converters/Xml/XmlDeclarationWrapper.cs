using System.Xml;

namespace SharpLib.Json
{
    internal class XmlDeclarationWrapper : XmlNodeWrapper, IXmlDeclaration
    {
        #region Поля

        private readonly XmlDeclaration _declaration;

        #endregion

        #region Свойства

        public string Version
        {
            get { return _declaration.Version; }
        }

        public string Encoding
        {
            get { return _declaration.Encoding; }
            set { _declaration.Encoding = value; }
        }

        public string Standalone
        {
            get { return _declaration.Standalone; }
            set { _declaration.Standalone = value; }
        }

        #endregion

        #region Конструктор

        public XmlDeclarationWrapper(XmlDeclaration declaration)
            : base(declaration)
        {
            _declaration = declaration;
        }

        #endregion
    }
}