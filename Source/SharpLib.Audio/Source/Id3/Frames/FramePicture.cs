using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Id3Lib.Frames
{
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "The image type is a byte")]
    internal enum PictureTypeCode : byte
    {
        Other = 0x00,

        Icon = 0x01,

        OtherIcon = 0x02,

        CoverFront = 0x03,

        CoverBack = 0x04,

        Leaflet = 0x05,

        Media = 0x06,

        LeadArtist = 0x07,

        Artist = 0x08,

        Conductor = 0x09,

        Orchestra = 0x0A,

        Composer = 0x0B,

        Lyricist = 0x0C,

        Location = 0x0D,

        Recording = 0x0E,

        Performance = 0x0F,

        Movie = 0x10,

        Fish = 0x11,

        Illustration = 0x12,

        BandLogo = 0x13,

        StudioLogo = 0x14,
    };

    [Frame("APIC")]
    internal class FramePicture : FrameBase, IFrameDescription
    {
        #region Поля

        private string _description;

        private string _mime;

        private byte[] _pictureData;

        private PictureTypeCode _pictureType;

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

        public PictureTypeCode PictureType
        {
            get { return _pictureType; }
            set { _pictureType = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public byte[] PictureData
        {
            get { return _pictureData; }
            set { _pictureData = value; }
        }

        public Image Picture
        {
            get { return Image.FromStream(new MemoryStream(_pictureData, false)); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                var memoryStream = new MemoryStream();
                value.Save(memoryStream, value.RawFormat);
                _pictureData = memoryStream.ToArray();
                _mime = GetMimeType(value);
            }
        }

        #endregion

        #region Конструктор

        public FramePicture(string frameId)
            : base(frameId)
        {
            _textEncoding = TextCode.Ascii;
        }

        #endregion

        #region Методы

        public static string GetMimeType(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            foreach (var codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == image.RawFormat.Guid)
                {
                    return codec.MimeType;
                }
            }

            return "image/unknown";
        }

        public override void Parse(byte[] frame)
        {
            int index = 0;
            _textEncoding = (TextCode)frame[index];
            index++;
            _mime = TextBuilder.ReadASCII(frame, ref index);
            _pictureType = (PictureTypeCode)frame[index];
            index++;
            _description = TextBuilder.ReadText(frame, ref index, _textEncoding);
            _pictureData = Memory.Extract(frame, index, frame.Length - index);
        }

        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write((byte)_textEncoding);
            writer.Write(TextBuilder.WriteASCII(_mime));
            writer.Write((byte)_pictureType);
            writer.Write(TextBuilder.WriteText(_description, _textEncoding));
            writer.Write(_pictureData);
            return buffer.ToArray();
        }

        public override string ToString()
        {
            return _description;
        }

        #endregion
    }
}