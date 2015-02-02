using System;

namespace SharpLib.Audio.FileFormats.Map
{
    internal class CakewalkDrumMapping
    {
        #region Ñגמיסעגא

        public string NoteName { get; set; }

        public int InNote { get; set; }

        public int OutNote { get; set; }

        public int OutPort { get; set; }

        public int Channel { get; set; }

        public int VelocityAdjust { get; set; }

        public float VelocityScale { get; set; }

        #endregion

        #region Ìועמהû

        public override string ToString()
        {
            return String.Format("{0} In:{1} Out:{2} Ch:{3} Port:{4} Vel+:{5} Vel:{6}%",
                NoteName, InNote, OutNote, Channel, OutPort, VelocityAdjust, VelocityScale * 100);
        }

        #endregion
    }
}