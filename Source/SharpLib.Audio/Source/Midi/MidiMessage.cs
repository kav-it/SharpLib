namespace NAudio.Midi
{
    internal class MidiMessage
    {
        #region ����

        private readonly int rawData;

        #endregion

        #region ��������

        public int RawData
        {
            get { return rawData; }
        }

        #endregion

        #region �����������

        public MidiMessage(int status, int data1, int data2)
        {
            rawData = status + (data1 << 8) + (data2 << 16);
        }

        public MidiMessage(int rawData)
        {
            this.rawData = rawData;
        }

        #endregion

        #region ������

        public static MidiMessage StartNote(int note, int volume, int channel)
        {
            return new MidiMessage((int)MidiCommandCode.NoteOn + channel - 1, note, volume);
        }

        public static MidiMessage StopNote(int note, int volume, int channel)
        {
            return new MidiMessage((int)MidiCommandCode.NoteOff + channel - 1, note, volume);
        }

        public static MidiMessage ChangePatch(int patch, int channel)
        {
            return new MidiMessage((int)MidiCommandCode.PatchChange + channel - 1, patch, 0);
        }

        public static MidiMessage ChangeControl(int controller, int value, int channel)
        {
            return new MidiMessage((int)MidiCommandCode.ControlChange + channel - 1, controller, value);
        }

        #endregion
    }
}