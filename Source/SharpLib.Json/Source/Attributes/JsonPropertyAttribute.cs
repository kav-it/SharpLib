using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class JsonPropertyAttribute : Attribute
    {
        #region Поля

        internal DefaultValueHandling? _defaultValueHandling;

        internal bool? _isReference;

        internal bool? _itemIsReference;

        internal ReferenceLoopHandling? _itemReferenceLoopHandling;

        internal TypeNameHandling? _itemTypeNameHandling;

        internal NullValueHandling? _nullValueHandling;

        internal ObjectCreationHandling? _objectCreationHandling;

        internal int? _order;

        internal ReferenceLoopHandling? _referenceLoopHandling;

        internal Required? _required;

        internal TypeNameHandling? _typeNameHandling;

        #endregion

        #region Свойства

        public Type ItemConverterType { get; set; }

        public object[] ItemConverterParameters { get; set; }

        public NullValueHandling NullValueHandling
        {
            get { return _nullValueHandling ?? default(NullValueHandling); }
            set { _nullValueHandling = value; }
        }

        public DefaultValueHandling DefaultValueHandling
        {
            get { return _defaultValueHandling ?? default(DefaultValueHandling); }
            set { _defaultValueHandling = value; }
        }

        public ReferenceLoopHandling ReferenceLoopHandling
        {
            get { return _referenceLoopHandling ?? default(ReferenceLoopHandling); }
            set { _referenceLoopHandling = value; }
        }

        public ObjectCreationHandling ObjectCreationHandling
        {
            get { return _objectCreationHandling ?? default(ObjectCreationHandling); }
            set { _objectCreationHandling = value; }
        }

        public TypeNameHandling TypeNameHandling
        {
            get { return _typeNameHandling ?? default(TypeNameHandling); }
            set { _typeNameHandling = value; }
        }

        public bool IsReference
        {
            get { return _isReference ?? default(bool); }
            set { _isReference = value; }
        }

        public int Order
        {
            get { return _order ?? default(int); }
            set { _order = value; }
        }

        public Required Required
        {
            get { return _required ?? Required.Default; }
            set { _required = value; }
        }

        public string PropertyName { get; set; }

        public ReferenceLoopHandling ItemReferenceLoopHandling
        {
            get { return _itemReferenceLoopHandling ?? default(ReferenceLoopHandling); }
            set { _itemReferenceLoopHandling = value; }
        }

        public TypeNameHandling ItemTypeNameHandling
        {
            get { return _itemTypeNameHandling ?? default(TypeNameHandling); }
            set { _itemTypeNameHandling = value; }
        }

        public bool ItemIsReference
        {
            get { return _itemIsReference ?? default(bool); }
            set { _itemIsReference = value; }
        }

        #endregion

        #region Конструктор

        public JsonPropertyAttribute()
        {
        }

        public JsonPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        #endregion
    }
}