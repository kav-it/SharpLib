using System.IO;

namespace SharpLib.Audio.Wave
{
    internal class CueWaveFileWriter : WaveFileWriter
    {
        #region Поля

        private CueList cues;

        #endregion

        #region Конструктор

        public CueWaveFileWriter(string fileName, WaveFormat waveFormat)
            : base(fileName, waveFormat)
        {
        }

        #endregion

        #region Методы

        public void AddCue(int position, string label)
        {
            if (cues == null)
            {
                cues = new CueList();
            }
            cues.Add(new Cue(position, label));
        }

        private void WriteCues(BinaryWriter w)
        {
            if (cues != null)
            {
                byte[] cueChunks = cues.GetRIFFChunks();
                int cueChunksSize = cueChunks.Length;
                w.Seek(0, SeekOrigin.End);
                w.Write(cues.GetRIFFChunks(), 0, cueChunksSize);
                w.Seek(4, SeekOrigin.Begin);
                w.Write((int)(w.BaseStream.Length - 8));
            }
        }

        protected override void UpdateHeader(BinaryWriter writer)
        {
            base.UpdateHeader(writer);
            WriteCues(writer);
        }

        #endregion
    }
}