using System.IO;

namespace Id3Lib.Frames
{
    [Frame("PCNT")]
    internal class FramePlayCounter : FrameBase
    {
        #region Поля

        private byte[] _counter = { 0 };

        #endregion

        #region Свойства

        public ulong Counter
        {
            get { return Memory.ToInt64(_counter); }
            set { _counter = Memory.GetBytes(value); }
        }

        #endregion

        #region Конструктор

        public FramePlayCounter(string frameId)
            : base(frameId)
        {
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            int index = 0;
            _counter = Memory.Extract(frame, index, frame.Length - index);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
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