using System;
using System.IO;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("specialfolder")]
    [AppDomainFixedOutput]
    public class SpecialFolderLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultParameter]
        public Environment.SpecialFolder Folder { get; set; }

        public string File { get; set; }

        public string Dir { get; set; }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string outputPath = Environment.GetFolderPath(Folder);

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