using System;
using System.Diagnostics.CodeAnalysis;

using Id3Lib.Exceptions;

namespace Id3Lib.Frames
{
    internal abstract class FrameBase
    {
        #region Поля

        private readonly string _frameId;

        private byte? _group;

        #endregion

        #region Свойства

        public bool TagAlter { get; set; }

        public bool FileAlter { get; set; }

        public bool ReadOnly { get; set; }

        public bool Compression { get; set; }

        public bool Encryption { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Un-synchronisation")]
        public bool Unsynchronisation { get; set; }

        public bool DataLength { get; set; }

        public byte? Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public string FrameId
        {
            get { return _frameId; }
        }

        #endregion

        #region Конструктор

        internal FrameBase(string frameId)
        {
            if (frameId == null)
            {
                throw new ArgumentNullException("frameId");
            }

            if (frameId.Length != 4)
            {
                throw new InvalidTagException("Invalid frame type: '" + frameId + "', it must be 4 characters long.");
            }

            _frameId = frameId;
        }

        #endregion

        #region Методы

        public abstract void Parse(byte[] frame);

        public abstract byte[] Make();

        #endregion
    }
}