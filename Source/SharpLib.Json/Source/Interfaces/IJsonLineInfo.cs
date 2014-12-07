namespace SharpLib.Json
{
    public interface IJsonLineInfo
    {
        #region Свойства

        int LineNumber { get; }

        int LinePosition { get; }

        #endregion

        #region Методы

        bool HasLineInfo();

        #endregion
    }
}