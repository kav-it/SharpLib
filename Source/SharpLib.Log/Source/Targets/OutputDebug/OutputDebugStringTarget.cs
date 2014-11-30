namespace SharpLib.Log
{
    [Target("OutputDebugString")]
    public sealed class OutputDebugStringTarget : TargetWithLayout
    {
        #region Ìועמהû

        protected override void Write(LogEventInfo logEvent)
        {
            NativeMethods.OutputDebugString(Layout.Render(logEvent));
        }

        #endregion
    }
}