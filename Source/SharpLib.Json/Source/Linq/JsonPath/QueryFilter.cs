using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Json.Linq
{
    internal class QueryFilter : PathFilter
    {
        #region ��������

        public QueryExpression Expression { get; set; }

        #endregion

        #region ������

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            return from t in current
                   from v in t
                   where Expression.IsMatch(v)
                   select v;
        }

        #endregion
    }
}