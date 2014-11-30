using System.ComponentModel;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("filesystem-normalize")]
    [AmbientProperty("FSNormalize")]
    [ThreadAgnostic]
    public sealed class FileSystemNormalizeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        [DefaultValue(true)]
        public bool FsNormalize { get; set; }

        #endregion

        #region Конструктор

        public FileSystemNormalizeLayoutRendererWrapper()
        {
            FsNormalize = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            if (FsNormalize)
            {
                var builder = new StringBuilder(text);
                for (int i = 0; i < builder.Length; i++)
                {
                    char c = builder[i];
                    if (!IsSafeCharacter(c))
                    {
                        builder[i] = '_';
                    }
                }

                return builder.ToString();
            }

            return text;
        }

        private static bool IsSafeCharacter(char c)
        {
            if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' || c == ' ')
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}