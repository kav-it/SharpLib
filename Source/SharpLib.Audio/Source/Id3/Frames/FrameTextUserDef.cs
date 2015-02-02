using System.IO;

namespace Id3Lib.Frames
{
    [Frame("TXXX")]
    internal class FrameTextUserDef : FrameBase, IFrameDescription
    {
        #region Поля

        private string _contents;

        private string _text;

        private TextCode _textEncoding;

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

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        #region Конструктор

        public FrameTextUserDef(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;

            if (frame.Length - index < 3)
            {
                return;
            }

            _contents = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _text = TextBuilder.ReadTextEnd(frame, index, _textEncoding);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteText(_contents, _textEncoding));
            writer.Write(TextBuilder.WriteTextEnd(_text, _textEncoding));
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return _text;
        }

        #endregion
    }
}