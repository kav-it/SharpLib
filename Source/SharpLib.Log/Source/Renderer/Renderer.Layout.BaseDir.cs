using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("basedir")]
    [AppDomainFixedOutput]
    public class BaseDirLayoutRenderer : LayoutRenderer
    {
        #region Поля

        private readonly string _baseDir;

        #endregion

        #region Свойства

        public string File { get; set; }

        public string Dir { get; set; }

        #endregion

        #region Конструктор

        public BaseDirLayoutRenderer()
            : this(AppDomainWrapper.CurrentDomain)
        {
        }

        public BaseDirLayoutRenderer(IAppDomain appDomain)
        {
            _baseDir = appDomain.BaseDirectory;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string outputPath = _baseDir;

            if (Dir != null)
            {
                outputPath = Path.Combine(outputPath, Dir);
            }

            if (File != null)
            {
                outputPath = Path.Combine(outputPath, File);
            }

            builder.Append(outputPath);
        }

        #endregion
    }
}