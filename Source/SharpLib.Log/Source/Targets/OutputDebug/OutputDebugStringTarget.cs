namespace SharpLib.Log
{
    [Target("OutputDebugString")]
    public sealed class OutputDebugStringTarget : TargetWithLayout
    {
        #region ������

        protected override void Write(LogEventInfo logEvent)
        {
            NativeMethods.OutputDebugString(Layout.Render(logEvent));
        }

        #endregion
    }
}