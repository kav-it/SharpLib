using System;

namespace SharpLib.Json
{
    public class JsonContainerContract : JsonContract
    {
        #region Поля

        private JsonContract _finalItemContract;

        private JsonContract _itemContract;

        #endregion

        #region Свойства

        internal JsonContract ItemContract
        {
            get { return _itemContract; }
            set
            {
                _itemContract = value;
                if (_itemContract != null)
                {
                    _finalItemContract = (_itemContract.UnderlyingType.IsSealed()) ? _itemContract : null;
                }
                else
                {
                    _finalItemContract = null;
                }
            }
        }

        internal JsonContract FinalItemContract
        {
            get { return _finalItemContract; }
        }

        public JsonConverter ItemConverter { get; set; }

        public bool? ItemIsReference { get; set; }

        public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

        public TypeNameHandling? ItemTypeNameHandling { get; set; }

        #endregion

        #region Конструктор

        internal JsonContainerContract(Type underlyingType)
            : base(underlyingType)
        {
            JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(underlyingType);

            if (jsonContainerAttribute != null)
            {
                if (jsonContainerAttribute.ItemConverterType != null)
                {
                    ItemConverter = JsonTypeReflector.CreateJsonConverterInstance(jsonContainerAttribute.ItemConverterType,
                        jsonContainerAttribute.ItemConverterParameters);
                }

                ItemIsReference = jsonContainerAttribute._itemIsReference;
                ItemReferenceLoopHandling = jsonContainerAttribute._itemReferenceLoopHandling;
                ItemTypeNameHandling = jsonContainerAttribute._itemTypeNameHandling;
            }
        }

        #endregion
    }
}