namespace SharpLib.Json
{
    internal interface IXmlDeclaration : IXmlNode
    {
        #region Свойства

        string Version { get; }

        string Encoding { get; set; }

        string Standalone { get; set; }

        #endregion
    }
}