
namespace SharpLib.Log
{
    internal interface IRenderable
    {
        #region Методы

        string Render(LogEventInfo logEvent);

        #endregion
    }
}
