using System;
using System.Collections.Generic;

namespace NAudio.Midi
{
    internal class MidiEventComparer : IComparer<MidiEvent>
    {
        #region Ìועמהû

        public int Compare(MidiEvent x, MidiEvent y)
        {
            long xTime = x.AbsoluteTime;
            long yTime = y.AbsoluteTime;

            if (xTime == yTime)
            {
                MetaEvent xMeta = x as MetaEvent;
                MetaEvent yMeta = y as MetaEvent;

                if (xMeta != null)
                {
                    if (xMeta.MetaEventType == MetaEventType.EndTrack)
                    {
                        xTime = Int64.MaxValue;
                    }
                    else
                    {
                        xTime = Int64.MinValue;
                    }
                }
                if (yMeta != null)
                {
                    if (yMeta.MetaEventType == MetaEventType.EndTrack)
                    {
                        yTime = Int64.MaxValue;
                    }
                    else
                    {
                        yTime = Int64.MinValue;
                    }
                }
            }
            return xTime.CompareTo(yTime);
        }

        #endregion
    }
}