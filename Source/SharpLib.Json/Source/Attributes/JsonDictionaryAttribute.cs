using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonDictionaryAttribute : JsonContainerAttribute
    {
        #region Конструктор

        public JsonDictionaryAttribute()
        {
        }

        public JsonDictionaryAttribute(string id)
            : base(id)
        {
        }

        #endregion
    }
}