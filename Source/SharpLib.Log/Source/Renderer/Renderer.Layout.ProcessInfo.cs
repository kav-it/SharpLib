using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("processinfo")]
    public class ProcessInfoLayoutRenderer : LayoutRenderer
    {
        #region Поля

        private Process _process;

        private PropertyInfo _propertyInfo;

        #endregion

        #region Свойства

        [DefaultValue("Id"), DefaultParameter]
        public ProcessInfoProperty Property { get; set; }

        #endregion

        #region Конструктор

        public ProcessInfoLayoutRenderer()
        {
            Property = ProcessInfoProperty.Id;
        }

        #endregion

        #region Методы

        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            _propertyInfo = typeof(Process).GetProperty(Property.ToString());
            if (_propertyInfo == null)
            {
                throw new ArgumentException("Property '" + _propertyInfo + "' not found in System.Diagnostics.Process");
            }

            _process = Process.GetCurrentProcess();
        }

        protected override void CloseLayoutRenderer()
        {
            if (_process != null)
            {
                _process.Close();
                _process = null;
            }

            base.CloseLayoutRenderer();
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (_propertyInfo != null)
            {
                builder.Append(Convert.ToString(_propertyInfo.GetValue(_process, null), CultureInfo.InvariantCulture));
            }
        }

        #endregion
    }
}