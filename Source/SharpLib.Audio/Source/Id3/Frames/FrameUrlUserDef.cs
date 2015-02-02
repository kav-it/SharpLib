using System.IO;

namespace Id3Lib.Frames
{
    [Frame("WXXX")]
    internal class FrameUrlUserDef : FrameBase, IFrameDescription
    {
        #region Поля

        private string _contents;

        private TextCode _textEncoding;

        private string _url;

        #endregion

        #region Свойства

        public TextCode TextCode
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        public string Description
        {
            get { return _contents; }
            set { _contents = value; }
        }

        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        #endregion

        #region Конструктор

        public FrameUrlUserDef(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            if (frame.Length < 1)
            {
                return;
            }

            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;
            _contents = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _url = TextBuilder.ReadTextEnd(frame, index, _textEncoding);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteText(_contents, _textEncoding));
            writer.Write(TextBuilder.WriteTextEnd(_url, _textEncoding));
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return _url;
        }

        #endregion
    }
}