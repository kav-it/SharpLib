using System.Collections.Generic;

namespace SharpLib.Json.Linq
{
    public class JTokenEqualityComparer : IEqualityComparer<JToken>
    {
        #region Методы

        public bool Equals(JToken x, JToken y)
        {
            return JToken.DeepEquals(x, y);
        }

        public int GetHashCode(JToken obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetDeepHashCode();
        }

        #endregion
    }
}