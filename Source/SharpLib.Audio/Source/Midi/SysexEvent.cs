using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NAudio.Midi
{
    internal class SysexEvent : MidiEvent
    {
        #region Поля

        private byte[] data;

        #endregion

        #region Методы

        public static SysexEvent ReadSysexEvent(BinaryReader br)
        {
            SysexEvent se = new SysexEvent();

            List<byte> sysexData = new List<byte>();
            bool loop = true;
            while (loop)
            {
                byte b = br.ReadByte();
                if (b == 0xF7)
                {
                    loop = false;
                }
                else
                {
                    sysexData.Add(b);
                }
            }

            se.data = sysexData.ToArray();

            return se;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:X2} ", b);
            }
            return String.Format("{0} Sysex: {1} bytes\r\n{2}", AbsoluteTime, data.Length, sb);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);

            writer.Write(data, 0, data.Length);
            writer.Write((byte)0xF7);
        }

        #endregion
    }
}