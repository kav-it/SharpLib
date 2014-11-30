using System.ComponentModel;

namespace SharpLib.Log
{
    public abstract class TargetWithLayout : Target
    {
        #region Свойства

        [RequiredParameter]
        [DefaultValue("${longdate}|${level:uppercase=true}|${logger}|${message}")]
        public virtual Layout Layout { get; set; }

        #endregion

        #region Конструктор

        protected TargetWithLayout()
        {
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";
        }

        #endregion
    }
}