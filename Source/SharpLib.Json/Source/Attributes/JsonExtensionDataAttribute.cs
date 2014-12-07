using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonExtensionDataAttribute : Attribute
    {
        #region Свойства

        public bool WriteData { get; set; }

        public bool ReadData { get; set; }

        #endregion

        #region Конструктор

        public JsonExtensionDataAttribute()
        {
            WriteData = true;
            ReadData = true;
        }

        #endregion
    }
}