using System;
using System.ComponentModel;

namespace SharpLib.Json.Linq
{
    public class JPropertyDescriptor : PropertyDescriptor
    {
        #region Свойства

        public override Type ComponentType
        {
            get { return typeof(JObject); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(object); }
        }

        protected override int NameHashCode
        {
            get
            {
                int nameHashCode = base.NameHashCode;
                return nameHashCode;
            }
        }

        #endregion

        #region Конструктор

        public JPropertyDescriptor(string name)
            : base(name, null)
        {
        }

        #endregion

        #region Методы

        private static JObject CastInstance(object instance)
        {
            return (JObject)instance;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            JToken token = CastInstance(component)[Name];

            return token;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            JToken token = (value is JToken) ? (JToken)value : new JValue(value);

            CastInstance(component)[Name] = token;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        #endregion
    }
}
