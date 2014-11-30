using System;

namespace SharpLib.Log
{
    [TimeSource("FastLocal")]
    public class FastLocalTimeSource : CachedTimeSource
    {
        #region Свойства

        protected override DateTime FreshTime
        {
            get { return DateTime.Now; }
        }

        #endregion
    }
}