using System.Collections.Generic;

namespace SharpLib.Json.Linq
{
    public interface IJEnumerable<out T> : IEnumerable<T> where T : JToken
    {
        IJEnumerable<JToken> this[object key] { get; }
    }
}