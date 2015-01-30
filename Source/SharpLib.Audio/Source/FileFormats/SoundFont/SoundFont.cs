using System;
using System.IO;

namespace NAudio.SoundFont
{
    internal class SoundFont
    {
        private readonly InfoChunk info;

        private readonly PresetsChunk presetsChunk;

        private readonly SampleDataChunk sampleData;

#if !NETFX_CORE

        public SoundFont(string fileName) :
            this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
        }
#endif

        public SoundFont(Stream sfFile)
        {
            using (sfFile)
            {
                RiffChunk riff = RiffChunk.GetTopLevelChunk(new BinaryReader(sfFile));
                if (riff.ChunkID == "RIFF")
                {
                    string formHeader = riff.ReadChunkID();
                    if (formHeader != "sfbk")
                    {
                        throw new InvalidDataException(String.Format("Not a SoundFont ({0})", formHeader));
                    }
                    RiffChunk list = riff.GetNextSubChunk();
                    if (list.ChunkID == "LIST")
                    {
                        info = new InfoChunk(list);

                        RiffChunk r = riff.GetNextSubChunk();
                        sampleData = new SampleDataChunk(r);

                        r = riff.GetNextSubChunk();
                        presetsChunk = new PresetsChunk(r);
                    }
                    else
                    {
                        throw new InvalidDataException(String.Format("Not info list found ({0})", list.ChunkID));
                    }
                }
                else
                {
                    throw new InvalidDataException("Not a RIFF file");
                }
            }
        }

        public InfoChunk FileInfo
        {
            get { return info; }
        }

        public Preset[] Presets
        {
            get { return presetsChunk.Presets; }
        }

        public Instrument[] Instruments
        {
            get { return presetsChunk.Instruments; }
        }

        public SampleHeader[] SampleHeaders
        {
            get { return presetsChunk.SampleHeaders; }
        }

        public byte[] SampleData
        {
            get { return sampleData.SampleData; }
        }

        public override string ToString()
        {
            return String.Format("Info Chunk:\r\n{0}\r\nPresets Chunk:\r\n{1}",
                info, presetsChunk);
        }
    }
}