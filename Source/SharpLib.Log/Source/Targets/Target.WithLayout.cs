using System.ComponentModel;

namespace SharpLib.Log
{
    public abstract class TargetWithLayout : Target
    {
        #region ��������

        [RequiredParameter]
        [DefaultValue("${longdate}|${level:uppercase=true}|${logger}|${message}")]
        public virtual Layout Layout { get; set; }

        #endregion

        #region �����������

        protected TargetWithLayout()
        {
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";
        }

        #endregion
    }
}