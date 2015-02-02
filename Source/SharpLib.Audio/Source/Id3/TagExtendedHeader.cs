using System;
using System.IO;

using Id3Lib.Exceptions;

namespace Id3Lib
{
    internal class TagExtendedHeader
    {
        #region Поля

        private byte[] _extendedHeader;

        private uint _size;

        #endregion

        #region Свойства

        public uint Size
        {
            get { return _size; }
        }

        #endregion

        #region Методы

        public void Deserialize(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var reader = new BinaryReader(stream);
            _size = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
            if (_size < 6)
            {
                throw new InvalidFrameException("Corrupt id3 extended header.");
            }

            _extendedHeader = new Byte[_size];
            stream.Read(_extendedHeader, 0, (int)_size);
        }

        public void Serialize(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(_extendedHeader);
        }

        #endregion
    }
}