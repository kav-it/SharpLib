﻿using System;

namespace SharpLib.Json
{
    public abstract class DateTimeConverterBase : JsonConverter
    {
        #region Методы

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                return true;
            }
            if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}