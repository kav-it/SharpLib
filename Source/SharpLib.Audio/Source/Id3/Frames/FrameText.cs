using System.IO;

namespace Id3Lib.Frames
{
    [Frame("T")]
    internal class FrameText : FrameBase
    {
        #region ����

        private string _text;

        private TextCode _textEncoding;

        #endregion

        #region ��������

        public TextCode TextCode
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        #region �����������

        public FrameText(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }

        #endregion

        #region ������

        public override void Parse(byte[] frame)
        {
            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;
            _text = TextBuilder.ReadTextEnd(frame, index, _textEncoding);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
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