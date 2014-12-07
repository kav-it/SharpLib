namespace SharpLib.Json
{
    internal interface IXmlDocumentType : IXmlNode
    {
        #region ��������

        string Name { get; }

        string System { get; }

        string Public { get; }

        string InternalSubset { get; }

        #endregion
    }
}