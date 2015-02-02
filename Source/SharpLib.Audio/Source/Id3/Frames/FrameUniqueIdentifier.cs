using System;
using System.IO;

namespace Id3Lib.Frames
{
    [Frame("UFID")]
    internal class FrameUniqueIdentifier : FrameBase, IFrameDescription
    {
        #region Поля

        private string _description;

        private byte[] _identifer;

        #endregion

        #region Свойства

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public byte[] Identifier
        {
            get { return _identifer; }
            set
            {
                if (value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("value", "The identifier can't be more than 64 bytes");
                }
                _identifer = value;
            }
        }

        #endregion

        #region Конструктор

        public FrameUniqueIdentifier(string frameId)
            : base(frameId)
        {
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            int index = 0;
            _description = TextBuilder.ReadASCII(frame, ref index);
            _identifer = Memory.Extract(frame, index, frame.Length - index);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write(TextBuilder.WriteASCII(_description));
            writer.Write(_identifer);
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return _description;
        }

        #endregion
    }
}