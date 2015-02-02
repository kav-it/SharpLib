using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Mixer
{
    internal class MixerLine
    {
        #region Поля

        private readonly IntPtr mixerHandle;

        private readonly MixerFlags mixerHandleType;

        private MixerInterop.MIXERLINE mixerLine;

        #endregion

        #region Свойства

        public String Name
        {
            get { return mixerLine.szName; }
        }

        public String ShortName
        {
            get { return mixerLine.szShortName; }
        }

        public int LineId
        {
            get { return mixerLine.dwLineID; }
        }

        public MixerLineComponentType ComponentType
        {
            get { return mixerLine.dwComponentType; }
        }

        public String TypeDescription
        {
            get
            {
                switch (mixerLine.dwComponentType)
                {
                    case MixerLineComponentType.DestinationUndefined:
                        return "Undefined Destination";
                    case MixerLineComponentType.DestinationDigital:
                        return "Digital Destination";
                    case MixerLineComponentType.DestinationLine:
                        return "Line Level Destination";
                    case MixerLineComponentType.DestinationMonitor:
                        return "Monitor Destination";
                    case MixerLineComponentType.DestinationSpeakers:
                        return "Speakers Destination";
                    case MixerLineComponentType.DestinationHeadphones:
                        return "Headphones Destination";
                    case MixerLineComponentType.DestinationTelephone:
                        return "Telephone Destination";
                    case MixerLineComponentType.DestinationWaveIn:
                        return "Wave Input Destination";
                    case MixerLineComponentType.DestinationVoiceIn:
                        return "Voice Recognition Destination";

                    case MixerLineComponentType.SourceUndefined:
                        return "Undefined Source";
                    case MixerLineComponentType.SourceDigital:
                        return "Digital Source";
                    case MixerLineComponentType.SourceLine:
                        return "Line Level Source";
                    case MixerLineComponentType.SourceMicrophone:
                        return "Microphone Source";
                    case MixerLineComponentType.SourceSynthesizer:
                        return "Synthesizer Source";
                    case MixerLineComponentType.SourceCompactDisc:
                        return "Compact Disk Source";
                    case MixerLineComponentType.SourceTelephone:
                        return "Telephone Source";
                    case MixerLineComponentType.SourcePcSpeaker:
                        return "PC Speaker Source";
                    case MixerLineComponentType.SourceWaveOut:
                        return "Wave Out Source";
                    case MixerLineComponentType.SourceAuxiliary:
                        return "Auxiliary Source";
                    case MixerLineComponentType.SourceAnalog:
                        return "Analog Source";
                    default:
                        return "Invalid Component Type";
                }
            }
        }

        public int Channels
        {
            get { return mixerLine.cChannels; }
        }

        public int SourceCount
        {
            get { return mixerLine.cConnections; }
        }

        public int ControlsCount
        {
            get { return mixerLine.cControls; }
        }

        public bool IsActive
        {
            get { return (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_ACTIVE) != 0; }
        }

        public bool IsDisconnected
        {
            get { return (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_DISCONNECTED) != 0; }
        }

        public bool IsSource
        {
            get { return (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_SOURCE) != 0; }
        }

        public IEnumerable<MixerControl> Controls
        {
            get { return MixerControl.GetMixerControls(mixerHandle, this, mixerHandleType); }
        }

        public IEnumerable<MixerLine> Sources
        {
            get
            {
                for (int source = 0; source < SourceCount; source++)
                {
                    yield return GetSource(source);
                }
            }
        }

        public string TargetName
        {
            get { return mixerLine.szPname; }
        }

        #endregion

        #region Конструктор

        public MixerLine(IntPtr mixerHandle, int destinationIndex, MixerFlags mixerHandleType)
        {
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            mixerLine = new MixerInterop.MIXERLINE();
            mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
            mixerLine.dwDestination = destinationIndex;
            MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.GetLineInfoOfDestination), "mixerGetLineInfo");
        }

        public MixerLine(IntPtr mixerHandle, int destinationIndex, int sourceIndex, MixerFlags mixerHandleType)
        {
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            mixerLine = new MixerInterop.MIXERLINE();
            mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
            mixerLine.dwDestination = destinationIndex;
            mixerLine.dwSource = sourceIndex;
            MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.GetLineInfoOfSource), "mixerGetLineInfo");
        }

        #endregion

        #region Методы

        public static int GetMixerIdForWaveIn(int waveInDevice)
        {
            int mixerId = -1;
            MmException.Try(MixerInterop.mixerGetID((IntPtr)waveInDevice, out mixerId, MixerFlags.WaveIn), "mixerGetID");
            return mixerId;
        }

        public MixerLine GetSource(int sourceIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= SourceCount)
            {
                throw new ArgumentOutOfRangeException("sourceIndex");
            }
            return new MixerLine(mixerHandle, mixerLine.dwDestination, sourceIndex, mixerHandleType);
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ({2} controls, ID={3})",
                Name, TypeDescription, ControlsCount, mixerLine.dwLineID);
        }

        #endregion
    }
}