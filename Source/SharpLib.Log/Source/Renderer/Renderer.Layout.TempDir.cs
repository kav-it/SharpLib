using System;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("tempdir")]
    [AppDomainFixedOutput]
    public class TempDirLayoutRenderer : LayoutRenderer
    {
        #region Поля

        private static readonly string _tempDir = Path.GetTempPath();

        #endregion

        #region Свойства

        public string File { get; set; }

        public string Dir { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string baseDir = _tempDir;

            if (File != null)
            {
                builder.Append(Path.Combine(baseDir, File));
            }
            else if (Dir != null)
            {
                builder.Append(Path.Combine(baseDir, Dir));
            }
            else
            {
                builder.Append(baseDir);
            }
        }

        #endregion
    }
}