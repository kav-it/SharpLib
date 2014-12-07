using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public abstract class JsonContainerAttribute : Attribute
    {
        #region Поля

        internal bool? _isReference;

        internal bool? _itemIsReference;

        internal ReferenceLoopHandling? _itemReferenceLoopHandling;

        internal TypeNameHandling? _itemTypeNameHandling;

        #endregion

        #region Свойства

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Type ItemConverterType { get; set; }

        public object[] ItemConverterParameters { get; set; }

        public bool IsReference
        {
            get { return _isReference ?? default(bool); }
            set { _isReference = value; }
        }

        public bool ItemIsReference
        {
            get { return _itemIsReference ?? default(bool); }
            set { _itemIsReference = value; }
        }

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

        #endregion

        #region Конструктор

        protected JsonContainerAttribute()
        {
        }

        protected JsonContainerAttribute(string id)
        {
            Id = id;
        }

        #endregion
    }
}