using System;

namespace SharpLib.Log
{
    public class LoggerConfigTargetFile : LoggerConfigTarget
    {
        #region Свойства

        public string Location { get; set; }

        public string MaxSize { get; set; }

        public string MaxCount { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0}", Location);
        }

        #endregion
    }
}