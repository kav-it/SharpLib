using System;
using System.IO;

namespace Id3Lib.Frames
{
    [Frame("W")]
    internal class FrameUrl : FrameBase
    {
        #region Поля

        private Uri _uri;

        #endregion

        #region Свойства

        public string Url
        {
            get { return _uri.AbsoluteUri; }
        }

        public Uri Uri
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _uri = value;
            }

            get { return _uri; }
        }

        #endregion

        #region Конструктор

        public FrameUrl(string frameId)
            : base(frameId)
        {
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            if (frame.Length < 1)
            {
                return;
            }

            var url = TextBuilder.ReadTextEnd(frame, 0, TextCode.Ascii);
            if (Uri.TryCreate(url, UriKind.Absolute, out _uri) == false)
            {
                _uri = null;
            }
        }

        public override byte[] Make()
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            var url = _uri != null ? _uri.AbsoluteUri : String.Empty;
            writer.Write(TextBuilder.WriteTextEnd(url, TextCode.Ascii));
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return _uri != null ? _uri.AbsoluteUri : String.Empty;
        }

        #endregion
    }
}