using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Id3Lib.Frames
{
    [Frame("POPM")]
    internal class FramePopularimeter : FrameBase, IFrameDescription
    {
        #region Поля

        private byte[] _counter = { 0 };

        private string _description;

        private byte _rating;

        #endregion

        #region Свойства

        public byte Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ulong Counter
        {
            get { return Memory.ToInt64(_counter); }
            set { _counter = Memory.GetBytes(value); }
        }

        #endregion

        #region Конструктор

        public FramePopularimeter(string frameId)
            : base(frameId)
        {
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            int index = 0;
            _description = TextBuilder.ReadASCII(frame, ref index);
            _rating = frame[index];
            index++;
            _counter = Memory.Extract(frame, index, frame.Length - index);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write(TextBuilder.WriteASCII(_description));
            writer.Write(_rating);
            writer.Write(_counter);
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return null;
        }

        #endregion
    }
}