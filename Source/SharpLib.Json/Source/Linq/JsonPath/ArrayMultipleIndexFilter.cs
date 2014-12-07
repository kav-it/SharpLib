using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Json.Linq
{
    internal class ArrayMultipleIndexFilter : PathFilter
    {
        #region ��������

        public List<int> Indexes { get; set; }

        #endregion

        #region ������

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            return current
                .SelectMany(t => Indexes, (t, i) => GetTokenIndex(t, errorWhenNoMatch, i))
                .Where(v => v != null);
        }

        #endregion
    }
}