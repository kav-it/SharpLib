using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using NAudio.Utils;

namespace NAudio.Wave
{
    internal class Cue
    {
        #region Свойства

        public int Position { get; private set; }

        public string Label { get; private set; }

        #endregion

        #region Конструктор

        public Cue(int position, string label)
        {
            Position = position;
            if (label == null)
            {
                label = "";
            }
            Label = Regex.Replace(label, @"[^\u0000-\u00FF]", "");
        }

        #endregion
    }

    internal class CueList
    {
        #region Поля

        private readonly List<Cue> cues = new List<Cue>();

        #endregion

        #region Свойства

        public int[] CuePositions
        {
            get
            {
                int[] positions = new int[cues.Count];
                for (int i = 0; i < cues.Count; i++)
                {
                    positions[i] = cues[i].Position;
                }
                return positions;
            }
        }

        public string[] CueLabels
        {
            get
            {
                string[] labels = new string[cues.Count];
                for (int i = 0; i < cues.Count; i++)
                {
                    labels[i] = cues[i].Label;
                }
                return labels;
            }
        }

        public int Count
        {
            get { return cues.Count; }
        }

        public Cue this[int index]
        {
            get { return cues[index]; }
        }

        #endregion

        #region Конструктор

        public CueList()
        {
        }

        internal CueList(byte[] cueChunkData, byte[] listChunkData)
        {
            int cueCount = BitConverter.ToInt32(cueChunkData, 0);
            Dictionary<int, int> cueIndex = new Dictionary<int, int>();
            int[] positions = new int[cueCount];
            int cue = 0;

            for (int p = 4; cueChunkData.Length - p >= 24; p += 24, cue++)
            {
                cueIndex[BitConverter.ToInt32(cueChunkData, p)] = cue;
                positions[cue] = BitConverter.ToInt32(cueChunkData, p + 20);
            }

            string[] labels = new string[cueCount];
            int labelLength = 0;
            int cueID = 0;

            Int32 labelChunkID = ChunkIdentifier.ChunkIdentifierToInt32("labl");
            for (int p = 4; listChunkData.Length - p >= 16; p += labelLength + labelLength % 2 + 12)
            {
                if (BitConverter.ToInt32(listChunkData, p) == labelChunkID)
                {
                    labelLength = BitConverter.ToInt32(listChunkData, p + 4) - 4;
                    cueID = BitConverter.ToInt32(listChunkData, p + 8);
                    cue = cueIndex[cueID];
                    labels[cue] = Encoding.Default.GetString(listChunkData, p + 12, labelLength - 1);
                }
            }

            for (int i = 0; i < cueCount; i++)
            {
                cues.Add(new Cue(positions[i], labels[i]));
            }
        }

        #endregion

        #region Методы

        public void Add(Cue cue)
        {
            cues.Add(cue);
        }

        internal byte[] GetRIFFChunks()
        {
            if (Count == 0)
            {
                return null;
            }
            int cueChunkLength = 12 + 24 * Count;
            int listChunkLength = 12;
            int labelChunkLength = 0;
            for (int i = 0; i < Count; i++)
            {
                labelChunkLength = this[i].Label.Length + 1;
                listChunkLength += labelChunkLength + labelChunkLength % 2 + 12;
            }

            byte[] chunks = new byte[cueChunkLength + listChunkLength];
            Int32 cueChunkID = ChunkIdentifier.ChunkIdentifierToInt32("cue ");
            Int32 dataChunkID = ChunkIdentifier.ChunkIdentifierToInt32("data");
            Int32 listChunkID = ChunkIdentifier.ChunkIdentifierToInt32("LIST");
            Int32 adtlTypeID = ChunkIdentifier.ChunkIdentifierToInt32("adtl");
            Int32 labelChunkID = ChunkIdentifier.ChunkIdentifierToInt32("labl");

            using (MemoryStream stream = new MemoryStream(chunks))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(cueChunkID);
                    writer.Write(cueChunkLength - 8);
                    writer.Write(Count);
                    for (int cue = 0; cue < Count; cue++)
                    {
                        int position = this[cue].Position;

                        writer.Write(cue);
                        writer.Write(position);
                        writer.Write(dataChunkID);
                        writer.Seek(8, SeekOrigin.Current);
                        writer.Write(position);
                    }
                    writer.Write(listChunkID);
                    writer.Write(listChunkLength - 8);
                    writer.Write(adtlTypeID);
                    for (int cue = 0; cue < Count; cue++)
                    {
                        writer.Write(labelChunkID);
                        writer.Write(this[cue].Label.Length + 1 + 4);
                        writer.Write(cue);
                        writer.Write(Encoding.Default.GetBytes(this[cue].Label.ToCharArray()));
                        if (this[cue].Label.Length % 2 == 0)
                        {
                            writer.Seek(2, SeekOrigin.Current);
                        }
                        else
                        {
                            writer.Seek(1, SeekOrigin.Current);
                        }
                    }
                }
            }
            return chunks;
        }

        internal static CueList FromChunks(WaveFileReader reader)
        {
            CueList cueList = null;
            byte[] cueChunkData = null;
            byte[] listChunkData = null;

            foreach (RiffChunk chunk in reader.ExtraChunks)
            {
                if (chunk.IdentifierAsString.ToLower() == "cue ")
                {
                    cueChunkData = reader.GetChunkData(chunk);
                }
                else if (chunk.IdentifierAsString.ToLower() == "list")
                {
                    listChunkData = reader.GetChunkData(chunk);
                }
            }
            if (cueChunkData != null && listChunkData != null)
            {
                cueList = new CueList(cueChunkData, listChunkData);
            }
            return cueList;
        }

        #endregion
    }
}