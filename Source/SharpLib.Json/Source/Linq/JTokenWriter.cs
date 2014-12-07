﻿using System;
using System.Globalization;
using System.Numerics;

namespace SharpLib.Json.Linq
{
    public class JTokenWriter : JsonWriter
    {
        #region Поля

        private JContainer _parent;

        private JContainer _token;

        private JValue _value;

        #endregion

        #region Свойства

        public JToken Token
        {
            get
            {
                if (_token != null)
                {
                    return _token;
                }

                return _value;
            }
        }

        #endregion

        #region Конструктор

        public JTokenWriter(JContainer container)
        {
            ValidationUtils.ArgumentNotNull(container, "container");

            _token = container;
            _parent = container;
        }

        public JTokenWriter()
        {
        }

        #endregion

        #region Методы

        public override void Flush()
        {
        }

        public override void WriteStartObject()
        {
            base.WriteStartObject();

            AddParent(new JObject());
        }

        private void AddParent(JContainer container)
        {
            if (_parent == null)
            {
                _token = container;
            }
            else
            {
                _parent.AddAndSkipParentCheck(container);
            }

            _parent = container;
        }

        private void RemoveParent()
        {
            _parent = _parent.Parent;

            if (_parent != null && _parent.Type == JTokenType.Property)
            {
                _parent = _parent.Parent;
            }
        }

        public override void WriteStartArray()
        {
            base.WriteStartArray();

            AddParent(new JArray());
        }

        public override void WriteStartConstructor(string name)
        {
            base.WriteStartConstructor(name);

            AddParent(new JConstructor(name));
        }

        protected override void WriteEnd(JsonToken token)
        {
            RemoveParent();
        }

        public override void WritePropertyName(string name)
        {
            AddParent(new JProperty(name));

            base.WritePropertyName(name);
        }

        private void AddValue(object value, JsonToken token)
        {
            AddValue(new JValue(value), token);
        }

        internal void AddValue(JValue value, JsonToken token)
        {
            if (_parent != null)
            {
                _parent.Add(value);

                if (_parent.Type == JTokenType.Property)
                {
                    _parent = _parent.Parent;
                }
            }
            else
            {
                _value = value ?? JValue.CreateNull();
            }
        }

        public override void WriteValue(object value)
        {
            if (value is BigInteger)
            {
                InternalWriteValue(JsonToken.Integer);
                AddValue(value, JsonToken.Integer);
            }
            else
            {
                base.WriteValue(value);
            }
        }

        public override void WriteNull()
        {
            base.WriteNull();
            AddValue(null, JsonToken.Null);
        }

        public override void WriteUndefined()
        {
            base.WriteUndefined();
            AddValue(null, JsonToken.Undefined);
        }

        public override void WriteRaw(string json)
        {
            base.WriteRaw(json);
            AddValue(new JRaw(json), JsonToken.Raw);
        }

        public override void WriteComment(string text)
        {
            base.WriteComment(text);
            AddValue(JValue.CreateComment(text), JsonToken.Comment);
        }

        public override void WriteValue(string value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.String);
        }

        public override void WriteValue(int value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(long value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(float value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Float);
        }

        public override void WriteValue(double value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Float);
        }

        public override void WriteValue(bool value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Boolean);
        }

        public override void WriteValue(short value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(char value)
        {
            base.WriteValue(value);
            string s = value.ToString(CultureInfo.InvariantCulture);
            AddValue(s, JsonToken.String);
        }

        public override void WriteValue(byte value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Integer);
        }

        public override void WriteValue(decimal value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Float);
        }

        public override void WriteValue(DateTime value)
        {
            base.WriteValue(value);
            value = DateTimeUtils.EnsureDateTime(value, DateTimeZoneHandling);
            AddValue(value, JsonToken.Date);
        }

        public override void WriteValue(DateTimeOffset value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Date);
        }

        public override void WriteValue(byte[] value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.Bytes);
        }

        public override void WriteValue(TimeSpan value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.String);
        }

        public override void WriteValue(Guid value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.String);
        }

        public override void WriteValue(Uri value)
        {
            base.WriteValue(value);
            AddValue(value, JsonToken.String);
        }

        #endregion
    }
}