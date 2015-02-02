using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

using Id3Lib.Exceptions;
using Id3Lib.Frames;

namespace Id3Lib
{
    internal class ID3v1
    {
        #region Поля

        private static readonly string[] _genres =
        {
            "Blues", "Classic Rock", "Country", "Dance", "Disco", "Funk", "Grunge", "Hip-Hop", "Jazz", "Metal",
            "New Age", "Oldies", "Other", "Pop", "R&B", "Rap", "Reggae", "Rock", "Techno", "Industrial",
            "Alternative", "Ska", "Death Metal", "Pranks", "Soundtrack", "Euro-Techno", "Ambient", "Trip-Hop",
            "Vocal", "Jazz+Funk", "Fusion", "Trance", "Classical", "Instrumental", "Acid", "House",
            "Game", "Sound Clip", "Gospel", "Noise", "Alternative Rock", "Bass", "Soul", "Punk", "Space",
            "Meditative", "Instrumental Pop", "Instrumental Rock", "Ethnic", "Gothic",
            "Darkwave", "Techno-Industrial", "Electronic", "Pop-Folk", "Eurodance", "Dream",
            "Southern Rock", "Comedy", "Cult", "Gangsta", "Top 40", "Christian Rap", "Pop/Funk", "Jungle",
            "Native American", "Cabaret", "New Wave", "Psychadelic", "Rave", "Showtunes", "Trailer", "Lo-Fi",
            "Tribal", "Acid Punk", "Acid Jazz", "Polka", "Retro", "Musical", "Rock & Roll", "Hard Rock", "Folk",
            "Folk/Rock", "National Folk", "Swing", "Fast-Fusion", "Bebob", "Latin", "Revival", "Celtic", "Bluegrass",
            "Avantgarde", "Gothic Rock", "Progressive Rock", "Psychedelic Rock", "Symphonic Rock", "Slow Rock",
            "Big Band", "Chorus", "Easy Listening", "Acoustic", "Humour", "Speech", "Chanson", "Opera", "Chamber Music",
            "Sonata", "Symphony", "Booty Bass", "Primus", "Porn Groove", "Satire", "Slow Jam", "Club",
            "Tango", "Samba", "Folklore", "Ballad", "Power Ballad", "Rhytmic Soul", "Freestyle", "Duet",
            "Punk Rock", "Drum Solo", "Acapella", "Euro-House", "Dance Hall", "Goa", "Drum & Bass", "Club-House",
            "Hardcore", "Terror", "Indie", "BritPop", "Negerpunk", "Polsk Punk", "Beat", "Christian Gangsta Rap",
            "Heavy Metal", "Black Metal", "Crossover", "Contemporary Christian",
            "Christian Rock", "Merengue", "Salsa", "Trash Metal", "Anime", "JPop", "SynthPop"
        };

        private static readonly byte[] _id3 = { 0x54, 0x41, 0x47 };

        private string _album;

        private string _artist;

        private string _comment;

        private byte _genre;

        private string _song;

        private byte _track;

        private string _year;

        #endregion

        #region Свойства

        public static uint TagLength
        {
            get { return 128; }
        }

        public string Song
        {
            get { return _song; }
        }

        public string Artist
        {
            get { return _artist; }
        }

        public string Year
        {
            get { return _year; }
        }

        public string Album
        {
            get { return _album; }
        }

        public string Comment
        {
            get { return _comment; }
        }

        public byte Track
        {
            get { return _track; }
        }

        public byte Genre
        {
            get { return _genre; }
        }

        public TagModel FrameModel
        {
            get { return GetFrameModel(); }
            set { SetFrameModel(value); }
        }

        #endregion

        #region Конструктор

        public ID3v1()
        {
            Clear();
        }

        #endregion

        #region Методы

        private TagModel GetFrameModel()
        {
            var frameModel = new TagModel();
            var frameText = new FrameText("TIT2");
            frameText.TextCode = TextCode.Ascii;
            frameText.Text = _song;
            frameModel.Add(frameText);

            frameText = new FrameText("TPE1");
            frameText.TextCode = TextCode.Ascii;
            frameText.Text = _artist;
            frameModel.Add(frameText);

            frameText = new FrameText("TALB");
            frameText.TextCode = TextCode.Ascii;
            frameText.Text = _album;
            frameModel.Add(frameText);

            frameText = new FrameText("TYER");
            frameText.TextCode = TextCode.Ascii;
            frameText.Text = _year;
            frameModel.Add(frameText);

            frameText = new FrameText("TRCK");
            frameText.TextCode = TextCode.Ascii;
            frameText.Text = _track.ToString(CultureInfo.InvariantCulture);
            frameModel.Add(frameText);

            var frameFullText = new FrameFullText("COMM");
            frameFullText.TextCode = TextCode.Ascii;
            frameFullText.Language = "eng";
            frameFullText.Description = "";
            frameFullText.Text = _comment;
            frameModel.Add(frameFullText);

            if (_genre >= 0 && _genre < _genres.Length)
            {
                frameText = new FrameText("TCON");
                frameText.TextCode = TextCode.Ascii;
                frameText.Text = _genres[_genre];
                frameModel.Add(frameText);
            }

            frameModel.Header.TagSize = 0;
            frameModel.Header.Version = 3;
            frameModel.Header.Revision = 0;
            frameModel.Header.Unsync = false;
            frameModel.Header.Experimental = false;
            frameModel.Header.Footer = false;
            frameModel.Header.ExtendedHeader = false;

            return frameModel;
        }

        private void SetFrameModel(TagModel frameModel)
        {
            Clear();
            foreach (var frame in frameModel)
            {
                switch (frame.FrameId)
                {
                    case "TIT2":
                        _song = GetTagText(frame);
                        break;
                    case "TPE1":
                        _artist = GetTagText(frame);
                        break;
                    case "TALB":
                        _album = GetTagText(frame);
                        break;
                    case "TYER":
                        _year = GetTagText(frame);
                        break;
                    case "TRCK":
                        if (!byte.TryParse(GetTagText(frame), out _track))
                        {
                            _track = 0;
                        }
                        break;
                    case "TCON":
                        _genre = ParseGenre(GetTagText(frame));
                        break;
                    case "COMM":
                        _comment = GetTagText(frame);
                        break;
                }
            }
        }

        private static string GetTagText(FrameBase tag)
        {
            var frameText = tag as FrameText;
            if (frameText != null)
            {
                return frameText.Text;
            }

            var frameFullText = tag as FrameFullText;
            if (frameFullText != null)
            {
                return frameFullText.Text;
            }

            return null;
        }

        private static byte ParseGenre(string sGenre)
        {
            if (String.IsNullOrEmpty(sGenre))
            {
                return 255;
            }

            byte nGenre;
            if (byte.TryParse(sGenre, out nGenre))
            {
                return nGenre;
            }

            if (sGenre[0] == '(' && sGenre[1] != '(')
            {
                int close = sGenre.IndexOf(')');
                if (close != -1)
                {
                    string sGNum = sGenre.Substring(1, close - 1);
                    if (Byte.TryParse(sGNum, out nGenre))
                    {
                        return nGenre;
                    }
                }
            }

            byte index = 0;
            foreach (string name in _genres)
            {
                if (String.Equals(name.Trim(), sGenre, StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
                ++index;
            }

            return 12;
        }

        private void Clear()
        {
            _song = "";
            _artist = "";
            _album = "";
            _year = "";
            _comment = "";
            _track = 0;
            _genre = 255;
        }

        private static string GetString(Encoding encoding, byte[] tag)
        {
            int index = Memory.FindByte(tag, 0x00, 0);
            return index < 0 ? encoding.GetString(tag) : encoding.GetString(tag, 0, index);
        }

        public void Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            var encoding = Encoding.GetEncoding(1252);

            reader.BaseStream.Seek(-128, SeekOrigin.End);

            byte[] idTag = new byte[3];

            reader.Read(idTag, 0, 3);

            if (Memory.Compare(_id3, idTag) != true)
            {
                throw new TagNotFoundException("ID3v1 tag was not found");
            }

            byte[] tag = new byte[30];

            reader.Read(tag, 0, 30);
            _song = GetString(encoding, tag);

            reader.Read(tag, 0, 30);
            _artist = GetString(encoding, tag);

            reader.Read(tag, 0, 30);
            _album = GetString(encoding, tag);

            reader.Read(tag, 0, 4);
            _year = (tag[0] != 0 && tag[1] != 0 && tag[2] != 0 && tag[3] != 0) ? encoding.GetString(tag, 0, 4) : String.Empty;

            reader.Read(tag, 0, 30);
            if (tag[28] == 0)
            {
                _track = tag[29];
                _comment = encoding.GetString(tag, 0, Memory.FindByte(tag, 0x00, 0));
            }
            else
            {
                _track = 0;
                _comment = GetString(encoding, tag);
            }
            _genre = reader.ReadByte();
        }

        public void Serialize(Stream stream)
        {
            var idTag = new byte[3];

            var reader = new BinaryReader(stream);

            reader.BaseStream.Seek(-128, SeekOrigin.End);

            reader.Read(idTag, 0, 3);

            if (Memory.Compare(_id3, idTag))
            {
                stream.Seek(-128, SeekOrigin.End);

                Write(stream);
            }
            else
            {
                var position = stream.Position;
                stream.Seek(0, SeekOrigin.End);

                try
                {
                    Write(stream);
                }
                catch (Exception e)
                {
                    stream.SetLength(position);
                    throw e;
                }
            }
        }

        public void Write(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            var encoding = Encoding.GetEncoding(1252);

            writer.Write(_id3, 0, 3);

            byte[] tag = new byte[30];

            if (_song.Length > 30)
            {
                _song = _song.Substring(0, 30);
            }
            encoding.GetBytes(_song, 0, _song.Length, tag, 0);
            writer.Write(tag, 0, 30);
            Memory.Clear(tag, 0, 30);

            if (_artist.Length > 30)
            {
                _artist = _artist.Substring(0, 30);
            }
            encoding.GetBytes(_artist, 0, _artist.Length, tag, 0);
            writer.Write(tag, 0, 30);
            Memory.Clear(tag, 0, 30);

            if (_album.Length > 30)
            {
                _album = _album.Substring(0, 30);
            }
            encoding.GetBytes(_album, 0, _album.Length, tag, 0);
            writer.Write(tag, 0, 30);
            Memory.Clear(tag, 0, 30);

            if (String.IsNullOrEmpty(_year))
            {
                Memory.Clear(tag, 0, 30);
            }
            else
            {
                UInt16 year;
                if (!UInt16.TryParse(_year, out year))
                {
                    _year = "0";
                }

                if (year > 9999)
                {
                    _year = "0";
                }

                encoding.GetBytes(_year, 0, _year.Length, tag, 0);
            }
            writer.Write(tag, 0, 4);
            Memory.Clear(tag, 0, 30);

            if (_comment.Length > 28)
            {
                _comment = _comment.Substring(0, 28);
            }

            encoding.GetBytes(_comment, 0, _comment.Length, tag, 0);

            writer.Write(tag, 0, 28);
            Memory.Clear(tag, 0, 30);
            writer.Write((byte)0);
            writer.Write(_track);
            writer.Write(_genre);
        }

        #endregion
    }
}