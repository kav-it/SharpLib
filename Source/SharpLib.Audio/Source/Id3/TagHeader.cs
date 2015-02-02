using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Id3Lib.Exceptions;

namespace Id3Lib
{
    internal class TagHeader
    {
        #region Поля

        private static readonly byte[] _3di = { 0x33, 0x44, 0x49 };

        private static readonly byte[] _id3 = { 0x49, 0x44, 0x33 };

        private byte _id3Flags;

        private uint _id3RawSize;

        private byte _id3Revision;

        private byte _id3Version = 4;

        private uint _paddingSize;

        #endregion

        #region Свойства

        public static uint HeaderSize
        {
            get { return 10; }
        }

        public byte Version
        {
            get { return _id3Version; }
            set { _id3Version = value; }
        }

        public byte Revision
        {
            get { return _id3Revision; }
            set { _id3Revision = value; }
        }

        public uint TagSize
        {
            get { return _id3RawSize; }
            set { _id3RawSize = value; }
        }

        public uint TagSizeWithHeaderFooter
        {
            get
            {
                return _id3RawSize
                       + HeaderSize
                       + (Footer ? HeaderSize : 0);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unsync")]
        public bool Unsync
        {
            get { return (_id3Flags & 0x80) > 0; }
            set
            {
                if (value)
                {
                    _id3Flags |= 0x80;
                }
                else
                {
                    unchecked
                    {
                        _id3Flags &= (byte)~(0x80);
                    }
                }
            }
        }

        public bool ExtendedHeader
        {
            get { return (_id3Flags & 0x40) > 0; }
            set
            {
                if (value)
                {
                    _id3Flags |= 0x40;
                }
                else
                {
                    unchecked
                    {
                        _id3Flags &= (byte)~(0x40);
                    }
                }
            }
        }

        public bool Experimental
        {
            get { return (_id3Flags & 0x20) > 0; }
            set
            {
                if (value)
                {
                    _id3Flags |= 0x20;
                }
                else
                {
                    unchecked
                    {
                        _id3Flags &= (byte)~(0x20);
                    }
                }
            }
        }

        public bool Footer
        {
            get { return (_id3Flags & 0x10) > 0; }
            set
            {
                if (value)
                {
                    _id3Flags |= 0x10;
                    _paddingSize = 0;
                }
                else
                {
                    unchecked
                    {
                        _id3Flags &= (byte)~(0x10);
                    }
                }
            }
        }

        public bool Padding
        {
            get { return _paddingSize > 0; }
        }

        public uint PaddingSize
        {
            set
            {
                Debug.Assert(value >= 0);

                if (value > 0)
                {
                    Footer = false;
                }
                _paddingSize = value;
            }
            get { return _paddingSize; }
        }

        #endregion

        #region Методы

        public void Serialize(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(_id3);
            writer.Write(_id3Version);
            writer.Write(_id3Revision);
            writer.Write(_id3Flags);
            writer.Write(Swap.UInt32(Sync.Safe(_id3RawSize + _paddingSize)));
        }

        public void SerializeFooter(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(_3di);
            writer.Write(_id3Version);
            writer.Write(_id3Revision);
            writer.Write(_id3Flags);
            writer.Write(Swap.UInt32(Sync.Safe(_id3RawSize)));
        }

        public void Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] idTag = new byte[3];

            reader.Read(idTag, 0, 3);
            if (Memory.Compare(_id3, idTag) == false)
            {
                throw new TagNotFoundException("ID3v2 tag identifier was not found");
            }

            _id3Version = reader.ReadByte();
            if (_id3Version == 0xff)
            {
                throw new InvalidTagException("Corrupt header, invalid ID3v2 version.");
            }

            _id3Revision = reader.ReadByte();
            if (_id3Revision == 0xff)
            {
                throw new InvalidTagException("Corrupt header, invalid ID3v2 revision.");
            }

            _id3Flags = (byte)(0xf0 & reader.ReadByte());

            _id3RawSize = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
            if (_id3RawSize == 0)
            {
                throw new InvalidTagException("Corrupt header, tag size can't be zero.");
            }
        }

        #endregion
    }
}