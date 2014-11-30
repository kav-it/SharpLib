using System;

namespace SharpLib.Log
{
    [TimeSource("FastUTC")]
    public class FastUtcTimeSource : CachedTimeSource
    {
        #region Свойства

        protected override DateTime FreshTime
        {
            get { return DateTime.UtcNow; }
        }

        #endregion
    }
}