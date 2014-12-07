namespace SharpLib.Json
{
    internal interface IXmlElement : IXmlNode
    {
        #region Ñגמיסעגא

        bool IsEmpty { get; }

        #endregion

        #region Ìועמהû

        void SetAttributeNode(IXmlNode attribute);

        string GetPrefixOfNamespace(string namespaceUri);

        #endregion
    }
}