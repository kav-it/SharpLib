using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonArrayAttribute : JsonContainerAttribute
    {
        #region Свойства

        public bool AllowNullItems { get; set; }

        #endregion

        #region Конструктор

        public JsonArrayAttribute()
        {
        }

        public JsonArrayAttribute(bool allowNullItems)
        {
            AllowNullItems = allowNullItems;
        }

        public JsonArrayAttribute(string id)
            : base(id)
        {
        }

        #endregion
    }
}