using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharpLib.Json
{
    internal abstract class JsonSerializerInternalBase
    {
        #region ����

        internal readonly JsonSerializer Serializer;

        internal readonly ITraceWriter TraceWriter;

        private readonly bool _serializing;

        private ErrorContext _currentErrorContext;

        private BidirectionalDictionary<string, object> _mappings;

        #endregion

        #region ��������

        internal BidirectionalDictionary<string, object> DefaultReferenceMappings
        {
            get
            {
                return _mappings ?? (_mappings = new BidirectionalDictionary<string, object>(
                    EqualityComparer<string>.Default,
                    new ReferenceEqualsEqualityComparer(),
                    "A different value already has the Id '{0}'.",
                    "A different Id has already been assigned for value '{0}'."));
            }
        }

        #endregion

        #region �����������

        protected JsonSerializerInternalBase(JsonSerializer serializer)
        {
            ValidationUtils.ArgumentNotNull(serializer, "serializer");

            Serializer = serializer;
            TraceWriter = serializer.TraceWriter;

            _serializing = (GetType() == typeof(JsonSerializerInternalWriter));
        }

        #endregion

        #region ������

        private ErrorContext GetErrorContext(object currentObject, object member, string path, Exception error)
        {
            if (_currentErrorContext == null)
            {
                _currentErrorContext = new ErrorContext(currentObject, member, path, error);
            }

            if (_currentErrorContext.Error != error)
            {
                throw new InvalidOperationException("Current error context error is different to requested error.");
            }

            return _currentErrorContext;
        }

        protected void ClearErrorContext()
        {
            if (_currentErrorContext == null)
            {
                throw new InvalidOperationException("Could not clear error context. Error context is already null.");
            }

            _currentErrorContext = null;
        }

        protected bool IsErrorHandled(object currentObject, JsonContract contract, object keyValue, IJsonLineInfo lineInfo, string path, Exception ex)
        {
            ErrorContext errorContext = GetErrorContext(currentObject, keyValue, path, ex);

            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Error && !errorContext.Traced)
            {
                errorContext.Traced = true;

                string message = (_serializing) ? "Error serializing" : "Error deserializing";
                if (contract != null)
                {
                    message += " " + contract.UnderlyingType;
                }
                message += ". " + ex.Message;

                if (!(ex is JsonException))
                {
                    message = JsonPosition.FormatMessage(lineInfo, path, message);
                }

                TraceWriter.Trace(TraceLevel.Error, message, ex);
            }

            if (contract != null && currentObject != null)
            {
                contract.InvokeOnError(currentObject, Serializer.Context, errorContext);
            }

            if (!errorContext.Handled)
            {
                Serializer.OnError(new ErrorEventArgs(currentObject, errorContext));
            }

            return errorContext.Handled;
        }

        #endregion

        #region ��������� �����: ReferenceEqualsEqualityComparer

        private class ReferenceEqualsEqualityComparer : IEqualityComparer<object>
        {
            #region ������

            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }

            #endregion
        }

        #endregion
    }
}