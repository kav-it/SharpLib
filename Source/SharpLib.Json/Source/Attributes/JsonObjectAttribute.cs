using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonObjectAttribute : JsonContainerAttribute
    {
        #region Поля

        internal Required? _itemRequired;

        private MemberSerialization _memberSerialization = MemberSerialization.OptOut;

        #endregion

        #region Свойства

        public MemberSerialization MemberSerialization
        {
            get { return _memberSerialization; }
            set { _memberSerialization = value; }
        }

        public Required ItemRequired
        {
            get { return _itemRequired ?? default(Required); }
            set { _itemRequired = value; }
        }

        #endregion

        #region Конструктор

        public JsonObjectAttribute()
        {
        }

        public JsonObjectAttribute(MemberSerialization memberSerialization)
        {
            MemberSerialization = memberSerialization;
        }

        public JsonObjectAttribute(string id)
            : base(id)
        {
        }

        #endregion
    }
}