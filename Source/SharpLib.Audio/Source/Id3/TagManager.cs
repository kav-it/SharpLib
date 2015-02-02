using System;
using System.IO;
using System.Text;

using Id3Lib.Exceptions;
using Id3Lib.Frames;

namespace Id3Lib
{
    internal static class TagManager
    {
        #region Ìועמהû

        public static TagModel Deserialize(Stream stream)
        {
            var frameModel = new TagModel();
            frameModel.Header.Deserialize(stream);
            if (frameModel.Header.Version != 3 & frameModel.Header.Version != 4)
            {
                throw new NotImplementedException("ID3v2 Version " + frameModel.Header.Version + " is not supported.");
            }

            uint id3TagSize = frameModel.Header.TagSize;

            if (frameModel.Header.Unsync)
            {
                var memory = new MemoryStream();
                id3TagSize -= Sync.Unsafe(stream, memory, id3TagSize);
                stream = memory;
                if (id3TagSize <= 0)
                {
                    throw new InvalidTagException("Data is missing after the header.");
                }
            }
            uint rawSize;

            if (frameModel.Header.ExtendedHeader)
            {
                frameModel.ExtendedHeader.Deserialize(stream);
                rawSize = id3TagSize - frameModel.ExtendedHeader.Size;
                if (id3TagSize <= 0)
                {
                    throw new InvalidTagException("Data is missing after the extended header.");
                }
            }
            else
            {
                rawSize = id3TagSize;
            }

            if (rawSize <= 0)
            {
                throw new InvalidTagException("No frames are present in the Tag, there must be at least one present.");
            }

            uint index = 0;
            var frameHelper = new FrameHelper(frameModel.Header);

            while (index < rawSize)
            {
                byte[] frameId = new byte[4];
                stream.Read(frameId, 0, 1);
                if (frameId[0] == 0)
                {
                    frameModel.Header.PaddingSize = rawSize - index;

                    stream.Seek(frameModel.Header.PaddingSize - 1, SeekOrigin.Current);

                    break;
                }
                if (index + 10 > rawSize)
                {
                    throw new InvalidTagException("Tag is corrupt, must be formed of complete frames.");
                }

                stream.Read(frameId, 1, 3);
                index += 4;

                var reader = new BinaryReader(stream);
                uint frameSize = Swap.UInt32(reader.ReadUInt32());
                index += 4;

                if (frameModel.Header.Version == 4)
                {
                    frameSize = Sync.Unsafe(frameSize);
                }

                if (frameSize > rawSize - index)
                {
                    throw new InvalidFrameException("A frame is corrupt, it can't be larger than the available space remaining.");
                }

                ushort flags = Swap.UInt16(reader.ReadUInt16());
                index += 2;
                byte[] frameData = new byte[frameSize];
                reader.Read(frameData, 0, (int)frameSize);
                index += frameSize;
                frameModel.Add(frameHelper.Build(Encoding.UTF8.GetString(frameId, 0, 4), flags, frameData));
            }
            return frameModel;
        }

        public static void Serialize(TagModel frameModel, Stream stream)
        {
            if (frameModel.Count <= 0)
            {
                throw new InvalidTagException("Can't serialize a ID3v2 tag without any frames, there must be at least one present.");
            }

            var memory = new MemoryStream();
            var writer = new BinaryWriter(memory);

            var frameHelper = new FrameHelper(frameModel.Header);

            foreach (FrameBase frame in frameModel)
            {
                byte[] frameId = new byte[4];
                Encoding.UTF8.GetBytes(frame.FrameId, 0, 4, frameId, 0);
                writer.Write(frameId);
                ushort flags;
                byte[] buffer = frameHelper.Make(frame, out flags);
                uint frameSize = (uint)buffer.Length;

                if (frameModel.Header.Version == 4)
                {
                    frameSize = Sync.Safe(frameSize);
                }

                writer.Write(Swap.UInt32(frameSize));
                writer.Write(Swap.UInt16(flags));
                writer.Write(buffer);
            }

            uint id3TagSize = (uint)memory.Position;

            stream.Seek(10, SeekOrigin.Begin);

            if (frameModel.Header.Unsync)
            {
                id3TagSize += Sync.Safe(memory, stream, id3TagSize);
            }
            else
            {
                memory.WriteTo(stream);
            }

            frameModel.Header.TagSize = id3TagSize;

            if (frameModel.Header.Padding)
            {
                for (int i = 0; i < frameModel.Header.PaddingSize; i++)
                {
                    stream.WriteByte(0);
                }
            }

            if (frameModel.Header.Footer)
            {
                frameModel.Header.SerializeFooter(stream);
            }

            long position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            frameModel.Header.Serialize(stream);

            stream.Position = position;
        }

        #endregion
    }
}