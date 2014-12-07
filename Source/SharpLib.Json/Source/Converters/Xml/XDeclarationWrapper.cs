using System.Xml;
using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XDeclarationWrapper : XObjectWrapper, IXmlDeclaration
    {
        #region ��������

        internal XDeclaration Declaration { get; private set; }

        public override XmlNodeType NodeType
        {
            get { return XmlNodeType.XmlDeclaration; }
        }

        public string Version
        {
            get { return Declaration.Version; }
        }

        public string Encoding
        {
            get { return Declaration.Encoding; }
            set { Declaration.Encoding = value; }
        }

        public string Standalone
        {
            get { return Declaration.Standalone; }
            set { Declaration.Standalone = value; }
        }

        #endregion

        #region �����������

        public XDeclarationWrapper(XDeclaration declaration)
            : base(null)
        {
            Declaration = declaration;
        }

        #endregion
    }
}