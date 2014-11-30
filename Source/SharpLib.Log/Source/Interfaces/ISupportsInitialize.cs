
namespace SharpLib.Log
{
    internal interface ISupportsInitialize
    {
        #region Методы

        void Initialize(LoggingConfiguration configuration);

        void Close();

        #endregion
    }
}
