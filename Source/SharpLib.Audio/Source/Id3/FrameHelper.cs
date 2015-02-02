using System;
using System.IO;

using Id3Lib.Exceptions;
using Id3Lib.Frames;

namespace Id3Lib
{
    internal class FrameHelper
    {
        #region Поля

        private readonly byte _revison;

        private readonly byte _version;

        #endregion

        #region Свойства

        public byte Version
        {
            get { return _version; }
        }

        public byte Revision
        {
            get { return _revison; }
        }

        #endregion

        #region Конструктор

        public FrameHelper(TagHeader header)
        {
            _version = header.Version;
            _revison = header.Revision;
        }

        #endregion

        #region Методы

        public FrameBase Build(string frameId, ushort flags, byte[] buffer)
        {
            var frame = FrameFactory.Build(frameId);
            SetFlags(frame, flags);

            uint index = 0;
            uint size = (uint)buffer.Length;
            Stream stream = new MemoryStream(buffer, false);
            var reader = new BinaryReader(stream);
            if (GetGrouping(flags))
            {
                frame.Group = reader.ReadByte();
                index++;
            }
            if (frame.Compression)
            {
                throw new Exception("Сжатие пока отключено");
                //switch (Version)
                //{
                //    case 3:
                //        {
                //            size = Swap.UInt32(reader.ReadUInt32());
                //            break;
                //        }
                //    case 4:
                //        {
                //            size = Swap.UInt32(Sync.UnsafeBigEndian(reader.ReadUInt32()));
                //            break;
                //        }
                //    default:
                //        {
                //            throw new NotImplementedException("ID3v2 Version " + Version + " is not supported.");
                //        }
                //}
                //index = 0;
                //stream = new InflaterInputStream(stream);
            }
            if (frame.Encryption)
            {
                throw new NotImplementedException("Encryption is not implemented, consequently it is not supported.");
            }
            if (frame.Unsynchronisation)
            {
                var memoryStream = new MemoryStream();
                size = Sync.Unsafe(stream, memoryStream, size);
                index = 0;
                memoryStream.Seek(0, SeekOrigin.Begin);
                stream = memoryStream;
            }
            byte[] frameBuffer = new byte[size - index];
            stream.Read(frameBuffer, 0, (int)(size - index));
            frame.Parse(frameBuffer);
            return frame;
        }

        public byte[] Make(FrameBase frame, out ushort flags)
        {
            flags = GetFlags(frame);
            var buffer = frame.Make();

            var memoryStream = new MemoryStream();
            var writer = new BinaryWriter(memoryStream);

            if (frame.Group.HasValue)
            {
                writer.Write((byte)frame.Group);
            }

            if (frame.Compression)
            {
                throw new Exception("Сжатие пока отключено");

                //switch (Version)
                //{
                //    case 3:
                //        {
                //            writer.Write(Swap.Int32(buffer.Length));
                //            break;
                //        }
                //    case 4:
                //        {
                //            writer.Write(Sync.UnsafeBigEndian(Swap.UInt32((uint)buffer.Length)));
                //            break;
                //        }
                //    default:
                //        {
                //            throw new NotImplementedException("ID3v2 Version " + Version + " is not supported.");
                //        }
                //}
                //var buf = new byte[2048];
                //var deflater = new Deflater(Deflater.BEST_COMPRESSION);
                //deflater.SetInput(buffer, 0, buffer.Length);
                //deflater.Finish();
                //while (!deflater.IsNeedingInput)
                //{
                //    int len = deflater.Deflate(buf, 0, buf.Length);
                //    if (len <= 0)
                //    {
                //        break;
                //    }
                //    memoryStream.Write(buf, 0, len);
                //}

                //if (!deflater.IsNeedingInput)
                //{
                //    throw new InvalidFrameException("Can't decompress frame '" + frame.FrameId + "' missing data");
                //}
            }
            else
            {
                memoryStream.Write(buffer, 0, buffer.Length);
            }

            if (frame.Encryption)
            {
                throw new NotImplementedException("Encryption is not implemented, consequently it is not supported.");
            }

            if (frame.Unsynchronisation)
            {
                MemoryStream synchStream = new MemoryStream();
                Sync.Unsafe(memoryStream, synchStream, (uint)memoryStream.Position);
                memoryStream = synchStream;
            }
            return memoryStream.ToArray();
        }

        private void SetFlags(FrameBase frame, ushort flags)
        {
            frame.TagAlter = GetTagAlter(flags);
            frame.FileAlter = GetFileAlter(flags);
            frame.ReadOnly = GetReadOnly(flags);
            frame.Compression = GetCompression(flags);
            frame.Encryption = GetEncryption(flags);
            frame.Unsynchronisation = GetUnsynchronisation(flags);
            frame.DataLength = GetDataLength(flags);
        }

        private bool GetTagAlter(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x8000) > 0;
                case 4:
                    return (flags & 0x4000) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetFileAlter(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x4000) > 0;
                case 4:
                    return (flags & 0x2000) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetReadOnly(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x2000) > 0;
                case 4:
                    return (flags & 0x1000) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetGrouping(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x0020) > 0;
                case 4:
                    return (flags & 0x0040) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetCompression(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x0080) > 0;
                case 4:
                    return (flags & 0x0008) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetEncryption(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return (flags & 0x0040) > 0;
                case 4:
                    return (flags & 0x0004) > 0;
                    ;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetUnsynchronisation(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return false;
                case 4:
                    return (flags & 0x0002) > 0;
                    ;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private bool GetDataLength(ushort flags)
        {
            switch (_version)
            {
                case 3:
                    return false;
                case 4:
                    return (flags & 0x0001) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private ushort GetFlags(FrameBase frame)
        {
            ushort flags = 0;
            SetTagAlter(frame.TagAlter, ref flags);
            SetFileAlter(frame.FileAlter, ref flags);
            SetReadOnly(frame.ReadOnly, ref flags);
            SetGrouping(frame.Group.HasValue, ref flags);
            SetCompression(frame.Compression, ref flags);
            SetEncryption(frame.Encryption, ref flags);
            SetUnsynchronisation(frame.Unsynchronisation, ref flags);
            SetDataLength(frame.DataLength, ref flags);
            return flags;
        }

        private void SetTagAlter(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    {
                        flags = value ? (ushort)(flags | 0x8000) : (ushort)(flags & unchecked((ushort)~(0x8000)));
                        break;
                    }
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x4000) : (ushort)(flags & unchecked((ushort)~(0x4000)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private void SetFileAlter(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    {
                        flags = value ? (ushort)(flags | 0x4000) : (ushort)(flags & unchecked((ushort)~(0x4000)));
                        break;
                    }
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x2000) : (ushort)(flags & unchecked((ushort)~(0x2000)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private void SetReadOnly(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    {
                        flags = value ? (ushort)(flags | 0x2000) : (ushort)(flags & unchecked((ushort)~(0x2000)));
                        break;
                    }
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x1000) : (ushort)(flags & unchecked((ushort)~(0x1000)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private void SetGrouping(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    {
                        flags = value ? (ushort)(flags | 0x0020) : (ushort)(flags & unchecked((ushort)~(0x0020)));
                        break;
                    }
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x0040) : (ushort)(flags & unchecked((ushort)~(0x0040)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private void SetCompression(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    {
                        flags = value ? (ushort)(flags | 0x0080) : (ushort)(flags & unchecked((ushort)~(0x0080)));
                        break;
                    }
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x0008) : (ushort)(flags & unchecked((ushort)~(0x0008)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private void SetEncryption(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    {
                        flags = value ? (ushort)(flags | 0x0040) : (ushort)(flags & unchecked((ushort)~(0x0040)));
                        break;
                    }
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x0004) : (ushort)(flags & unchecked((ushort)~(0x0004)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        private void SetUnsynchronisation(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    break;
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x0002) : (ushort)(flags & unchecked((ushort)~(0x0002)));
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
                    }
            }
        }

        private void SetDataLength(bool value, ref ushort flags)
        {
            switch (_version)
            {
                case 3:
                    break;
                case 4:
                    {
                        flags = value ? (ushort)(flags | 0x0001) : (ushort)(flags & unchecked((ushort)~(0x0001)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("ID3v2 Version " + _version + " is not supported.");
            }
        }

        #endregion
    }
}