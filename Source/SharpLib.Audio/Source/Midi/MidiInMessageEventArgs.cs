using System;

namespace NAudio.Midi
{
    internal class MidiInMessageEventArgs : EventArgs
    {
        #region ��������

        public int RawMessage { get; private set; }

        public MidiEvent MidiEvent { get; private set; }

        public int Timestamp { get; private set; }

        #endregion

        #region �����������

        public MidiInMessageEventArgs(int message, int timestamp)
        {
            RawMessage = message;
            Timestamp = timestamp;
            try
            {
                MidiEvent = MidiEvent.FromRawMessage(message);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}