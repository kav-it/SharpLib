﻿using System.IO;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Wave
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal class WaveFormatExtraData : WaveFormat
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private readonly byte[] extraData = new byte[100];

        public byte[] ExtraData
        {
            get { return extraData; }
        }

        internal WaveFormatExtraData()
        {
        }

        public WaveFormatExtraData(BinaryReader reader)
            : base(reader)
        {
            ReadExtraData(reader);
        }

        internal void ReadExtraData(BinaryReader reader)
        {
            if (extraSize > 0)
            {
                reader.Read(extraData, 0, extraSize);
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            if (extraSize > 0)
            {
                writer.Write(extraData, 0, extraSize);
            }
        }
    }
}