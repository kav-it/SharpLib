namespace SharpLib.Json
{
    internal interface IXmlElement : IXmlNode
    {
        #region ��������

        bool IsEmpty { get; }

        #endregion

        #region ������

        void SetAttributeNode(IXmlNode attribute);

        string GetPrefixOfNamespace(string namespaceUri);

        #endregion
    }
}