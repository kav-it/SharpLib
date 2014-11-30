using System.Collections.Generic;

namespace SharpLib.Log
{
    internal static class SortHelpers
    {
        #region Делегаты

        internal delegate TKey KeySelector<in TValue, out TKey>(TValue value);

        #endregion

        #region Методы

        public static Dictionary<TKey, List<TValue>> BucketSort<TValue, TKey>(this IEnumerable<TValue> inputs, KeySelector<TValue, TKey> keySelector)
        {
            var buckets = new Dictionary<TKey, List<TValue>>();

            foreach (var input in inputs)
            {
                var keyValue = keySelector(input);
                List<TValue> eventsInBucket;
                if (!buckets.TryGetValue(keyValue, out eventsInBucket))
                {
                    eventsInBucket = new List<TValue>();
                    buckets.Add(keyValue, eventsInBucket);
                }

                eventsInBucket.Add(input);
            }

            return buckets;
        }

        #endregion
    }
}