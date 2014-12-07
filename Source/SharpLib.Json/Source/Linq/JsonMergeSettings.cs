using System;

namespace SharpLib.Json.Linq
{
    public class JsonMergeSettings
    {
        #region Поля

        private MergeArrayHandling _mergeArrayHandling;

        #endregion

        #region Свойства

        public MergeArrayHandling MergeArrayHandling
        {
            get { return _mergeArrayHandling; }
            set
            {
                if (value < MergeArrayHandling.Concat || value > MergeArrayHandling.Merge)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _mergeArrayHandling = value;
            }
        }

        #endregion
    }
}