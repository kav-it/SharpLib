using System;

namespace SharpLib.Log
{
    [TimeSource("AccurateLocal")]
    public class AccurateLocalTimeSource : TimeSource
    {
        #region Свойства

        public override DateTime Time
        {
            get { return DateTime.Now; }
        }

        #endregion
    }
}