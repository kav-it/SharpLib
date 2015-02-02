using System;
using System.IO;
using System.Text;

namespace Id3Lib.Frames
{
    [Frame("USLT"), Frame("COMM")]
    internal class FrameFullText : FrameBase, IFrameDescription
    {
        #region Поля

        private string _contents;

        private string _language = "eng";

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

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        #endregion

        #region Конструктор

        public FrameFullText(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;

            if (frame.Length - index < 3)
            {
                return;
            }

            _language = Encoding.UTF8.GetString(frame, index, 3);
            index += 3;

            if (frame.Length - index < 1)
            {
                return;
            }

            _contents = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _text = TextBuilder.ReadTextEnd(frame, index, _textEncoding);
        }

        public override byte[] Make()
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);

            var language = TextBuilder.WriteASCII(_language);
            if (language.Length != 3)
            {
                writer.Write(new[] { (byte)'e', (byte)'n', (byte)'g' });
            }
            else
            {
                writer.Write(language, 0, 3);
            }
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