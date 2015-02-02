using System;
using System.IO;

namespace Id3Lib.Frames
{
    [Frame("GEOB")]
    internal class FrameBinary : FrameBase, IFrameDescription
    {
        #region Поля

        private string _description;

        private string _fileName;

        private string _mime;

        private byte[] _objectData;

        private TextCode _textEncoding;

        #endregion

        #region Свойства

        public TextCode TextEncoding
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        public string Mime
        {
            get { return _mime; }
            set { _mime = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public byte[] ObjectData
        {
            get { return _objectData; }
            set { _objectData = value; }
        }

        #endregion

        #region Конструктор

        public FrameBinary(string frameId)
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
            _mime = TextBuilder.ReadASCII(frame, ref index);
            _fileName = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _description = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _objectData = Memory.Extract(frame, index, frame.Length - index);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteASCII(_mime));
            writer.Write(TextBuilder.WriteText(_fileName, _textEncoding));
            writer.Write(TextBuilder.WriteText(_description, _textEncoding));
            writer.Write(_objectData);
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return _description;
        }

        #endregion
    }
}