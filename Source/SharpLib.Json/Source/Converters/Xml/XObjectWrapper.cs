using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XObjectWrapper : IXmlNode
    {
        #region ����

        private readonly XObject _xmlObject;

        #endregion

        #region ��������

        public object WrappedNode
        {
            get { return _xmlObject; }
        }

        public virtual XmlNodeType NodeType
        {
            get { return _xmlObject.NodeType; }
        }

        public virtual string LocalName
        {
            get { return null; }
        }

        public virtual IList<IXmlNode> ChildNodes
        {
            get { return new List<IXmlNode>(); }
        }

        public virtual IList<IXmlNode> Attributes
        {
            get { return null; }
        }

        public virtual IXmlNode ParentNode
        {
            get { return null; }
        }

        public virtual string Value
        {
            get { return null; }
            set { throw new InvalidOperationException(); }
        }

        public virtual string NamespaceUri
        {
            get { return null; }
        }

        #endregion

        #region �����������

        public XObjectWrapper(XObject xmlObject)
        {
            _xmlObject = xmlObject;
        }

        #endregion

        #region ������

        public virtual IXmlNode AppendChild(IXmlNode newChild)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}