using System;

namespace SharpLib.Log
{
    public abstract class CachedTimeSource : TimeSource
    {
        #region Поля

        private int lastTicks = -1;

        private DateTime lastTime = DateTime.MinValue;

        #endregion

        #region Свойства

        protected abstract DateTime FreshTime { get; }

        public override DateTime Time
        {
            get
            {
                int tickCount = Environment.TickCount;
                if (tickCount == lastTicks)
                {
                    return lastTime;
                }
                DateTime time = FreshTime;
                lastTicks = tickCount;
                lastTime = time;
                return time;
            }
        }

        #endregion
    }
}