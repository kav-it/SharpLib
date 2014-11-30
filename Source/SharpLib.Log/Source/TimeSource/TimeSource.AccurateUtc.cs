using System;

namespace SharpLib.Log
{
    [TimeSource("AccurateUTC")]
    public class AccurateUtcTimeSource : TimeSource
    {
        #region Свойства

        public override DateTime Time
        {
            get { return DateTime.UtcNow; }
        }

        #endregion
    }
}