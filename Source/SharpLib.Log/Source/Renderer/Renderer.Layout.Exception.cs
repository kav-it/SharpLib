using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("exception")]
    [ThreadAgnostic]
    public class ExceptionLayoutRenderer : LayoutRenderer
    {
        #region Делегаты

        private delegate void ExceptionDataTarget(StringBuilder sb, Exception ex);

        #endregion

        #region Поля

        private ExceptionDataTarget[] _exceptionDataTargets;

        private string _format;

        private ExceptionDataTarget[] _innerExceptionDataTargets;

        private string _innerFormat;

        #endregion

        #region Свойства

        [DefaultParameter]
        public string Format
        {
            get { return _format; }

            set
            {
                _format = value;
                _exceptionDataTargets = CompileFormat(value);
            }
        }

        public string InnerFormat
        {
            get { return _innerFormat; }

            set
            {
                _innerFormat = value;
                _innerExceptionDataTargets = CompileFormat(value);
            }
        }

        [DefaultValue(" ")]
        public string Separator { get; set; }

        [DefaultValue(0)]
        public int MaxInnerExceptionLevel { get; set; }

        public string InnerExceptionSeparator { get; set; }

        #endregion

        #region Конструктор

        public ExceptionLayoutRenderer()
        {
            _innerFormat = string.Empty;
            Format = "message";
            Separator = " ";
            InnerExceptionSeparator = EnvironmentHelper.NewLine;
            MaxInnerExceptionLevel = 0;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Exception != null)
            {
                var sb2 = new StringBuilder(128);
                string separator = string.Empty;

                foreach (ExceptionDataTarget targetRenderFunc in _exceptionDataTargets)
                {
                    sb2.Append(separator);
                    targetRenderFunc(sb2, logEvent.Exception);
                    separator = Separator;
                }

                Exception currentException = logEvent.Exception.InnerException;
                int currentLevel = 0;
                while (currentException != null && currentLevel < MaxInnerExceptionLevel)
                {
                    sb2.Append(InnerExceptionSeparator);

                    separator = string.Empty;
                    foreach (ExceptionDataTarget targetRenderFunc in _innerExceptionDataTargets ?? _exceptionDataTargets)
                    {
                        sb2.Append(separator);
                        targetRenderFunc(sb2, currentException);
                        separator = Separator;
                    }

                    currentException = currentException.InnerException;
                    currentLevel++;
                }

                builder.Append(sb2);
            }
        }

        protected virtual void AppendMessage(StringBuilder sb, Exception ex)
        {
            try
            {
                sb.Append(ex.Message);
            }
            catch (Exception exception)
            {
                var message = string.Format("Exception in {0}.AppendMessage(): {1}.", typeof(ExceptionLayoutRenderer).FullName, exception.GetType().FullName);
                sb.Append("log message:" + message);
            }
        }

        protected virtual void AppendMethod(StringBuilder sb, Exception ex)
        {
            if (ex.TargetSite != null)
            {
                sb.Append(ex.TargetSite);
            }
        }

        protected virtual void AppendStackTrace(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.StackTrace);
        }

        protected virtual void AppendToString(StringBuilder sb, Exception ex)
        {
            sb.Append(ex);
        }

        protected virtual void AppendType(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.GetType().FullName);
        }

        protected virtual void AppendShortType(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.GetType().Name);
        }

        protected virtual void AppendData(StringBuilder sb, Exception ex)
        {
            string separator = string.Empty;
            foreach (var key in ex.Data.Keys)
            {
                sb.Append(separator);
                sb.AppendFormat("{0}: {1}", key, ex.Data[key]);
                separator = ";";
            }
        }

        private ExceptionDataTarget[] CompileFormat(string formatSpecifier)
        {
            string[] parts = formatSpecifier.Replace(" ", string.Empty).Split(',');
            var dataTargets = new List<ExceptionDataTarget>();

            foreach (string s in parts)
            {
                switch (s.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "MESSAGE":
                        dataTargets.Add(AppendMessage);
                        break;

                    case "TYPE":
                        dataTargets.Add(AppendType);
                        break;

                    case "SHORTTYPE":
                        dataTargets.Add(AppendShortType);
                        break;

                    case "TOSTRING":
                        dataTargets.Add(AppendToString);
                        break;

                    case "METHOD":
                        dataTargets.Add(AppendMethod);
                        break;

                    case "STACKTRACE":
                        dataTargets.Add(AppendStackTrace);
                        break;

                    case "DATA":
                        dataTargets.Add(AppendData);
                        break;
                }
            }

            return dataTargets.ToArray();
        }

        #endregion
    }
}