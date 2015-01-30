namespace NAudio.SoundFont
{
    internal class SampleHeader
    {
        #region ����

        public uint End;

        public uint EndLoop;

        public byte OriginalPitch;

        public sbyte PitchCorrection;

        public SFSampleLink SFSampleLink;

        public ushort SampleLink;

        public string SampleName;

        public uint SampleRate;

        public uint Start;

        public uint StartLoop;

        #endregion

        #region ������

        public override string ToString()
        {
            return SampleName;
        }

        #endregion
    }
}