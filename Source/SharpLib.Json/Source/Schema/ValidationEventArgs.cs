using System;

namespace SharpLib.Json.Schema
{
    public class ValidationEventArgs : EventArgs
    {
        #region Поля

        private readonly JsonSchemaException _ex;

        #endregion

        #region Свойства

        public JsonSchemaException Exception
        {
            get { return _ex; }
        }

        public string Path
        {
            get { return _ex.Path; }
        }

        public string Message
        {
            get { return _ex.Message; }
        }

        #endregion

        #region Конструктор

        internal ValidationEventArgs(JsonSchemaException ex)
        {
            ValidationUtils.ArgumentNotNull(ex, "ex");
            _ex = ex;
        }

        #endregion
    }
}