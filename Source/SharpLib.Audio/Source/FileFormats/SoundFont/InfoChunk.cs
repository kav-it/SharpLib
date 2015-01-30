using System;
using System.IO;

namespace NAudio.SoundFont
{
    internal class InfoChunk
    {
        #region Поля

        private readonly SFVersion verSoundFont;

        #endregion

        #region Свойства

        public SFVersion SoundFontVersion
        {
            get { return verSoundFont; }
        }

        public string WaveTableSoundEngine { get; set; }

        public string BankName { get; set; }

        public string DataROM { get; set; }

        public string CreationDate { get; set; }

        public string Author { get; set; }

        public string TargetProduct { get; set; }

        public string Copyright { get; set; }

        public string Comments { get; set; }

        public string Tools { get; set; }

        public SFVersion ROMVersion { get; set; }

        #endregion

        #region Конструктор

        internal InfoChunk(RiffChunk chunk)
        {
            bool ifilPresent = false;
            bool isngPresent = false;
            bool INAMPresent = false;
            if (chunk.ReadChunkID() != "INFO")
            {
                throw new InvalidDataException("Not an INFO chunk");
            }

            RiffChunk c;
            while ((c = chunk.GetNextSubChunk()) != null)
            {
                switch (c.ChunkID)
                {
                    case "ifil":
                        ifilPresent = true;
                        verSoundFont = c.GetDataAsStructure(new SFVersionBuilder());
                        break;
                    case "isng":
                        isngPresent = true;
                        WaveTableSoundEngine = c.GetDataAsString();
                        break;
                    case "INAM":
                        INAMPresent = true;
                        BankName = c.GetDataAsString();
                        break;
                    case "irom":
                        DataROM = c.GetDataAsString();
                        break;
                    case "iver":
                        ROMVersion = c.GetDataAsStructure(new SFVersionBuilder());
                        break;
                    case "ICRD":
                        CreationDate = c.GetDataAsString();
                        break;
                    case "IENG":
                        Author = c.GetDataAsString();
                        break;
                    case "IPRD":
                        TargetProduct = c.GetDataAsString();
                        break;
                    case "ICOP":
                        Copyright = c.GetDataAsString();
                        break;
                    case "ICMT":
                        Comments = c.GetDataAsString();
                        break;
                    case "ISFT":
                        Tools = c.GetDataAsString();
                        break;
                    default:
                        throw new InvalidDataException(String.Format("Unknown chunk type {0}", c.ChunkID));
                }
            }
            if (!ifilPresent)
            {
                throw new InvalidDataException("Missing SoundFont version information");
            }
            if (!isngPresent)
            {
                throw new InvalidDataException("Missing wavetable sound engine information");
            }
            if (!INAMPresent)
            {
                throw new InvalidDataException("Missing SoundFont name information");
            }
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return
                String.Format(
                    "Bank Name: {0}\r\nAuthor: {1}\r\nCopyright: {2}\r\nCreation Date: {3}\r\nTools: {4}\r\nComments: {5}\r\nSound Engine: {6}\r\nSoundFont Version: {7}\r\nTarget Product: {8}\r\nData ROM: {9}\r\nROM Version: {10}",
                    BankName,
                    Author,
                    Copyright,
                    CreationDate,
                    Tools,
                    "TODO-fix comments",
                    WaveTableSoundEngine,
                    SoundFontVersion,
                    TargetProduct,
                    DataROM,
                    ROMVersion);
        }

        #endregion
    }
} 