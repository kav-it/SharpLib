using System.Collections.Generic;
using System.Xml;

namespace SharpLib.Json
{
    internal interface IXmlNode
    {
        #region Ñגמיסעגא

        XmlNodeType NodeType { get; }

        string LocalName { get; }

        IList<IXmlNode> ChildNodes { get; }

        IList<IXmlNode> Attributes { get; }

        IXmlNode ParentNode { get; }

        string Value { get; set; }

        string NamespaceUri { get; }

        object WrappedNode { get; }

        #endregion

        #region Ìועמהû

        IXmlNode AppendChild(IXmlNode newChild);

        #endregion
    }
}