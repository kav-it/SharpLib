using System;

namespace NAudio.Utils
{
    internal class Decibels
    {
        #region Константы

        private const double DB_2_LOG = 0.11512925464970228420089957273422;

        private const double LOG_2_DB = 8.6858896380650365530225783783321;

        #endregion

        #region Методы

        public static double LinearToDecibels(double lin)
        {
            return Math.Log(lin) * LOG_2_DB;
        }

        public static double DecibelsToLinear(double dB)
        {
            return Math.Exp(dB * DB_2_LOG);
        }

        #endregion
    }
}