using System;

namespace SharpLib.Audio.SoundFont
{
    internal enum ControllerSourceEnum
    {
        NoController = 0,

        NoteOnVelocity = 2,

        NoteOnKeyNumber = 3,

        PolyPressure = 10,

        ChannelPressure = 13,

        PitchWheel = 14,

        PitchWheelSensitivity = 16
    }

    internal enum SourceTypeEnum
    {
        Linear,

        Concave,

        Convex,

        Switch
    }

    internal class ModulatorType
    {
        #region Поля

        private readonly ControllerSourceEnum controllerSource;

        private readonly bool midiContinuousController;

        private readonly ushort midiContinuousControllerNumber;

        private readonly SourceTypeEnum sourceType;

        private bool direction;

        private bool polarity;

        #endregion

        #region Конструктор

        internal ModulatorType(ushort raw)
        {
            polarity = ((raw & 0x0200) == 0x0200);
            direction = ((raw & 0x0100) == 0x0100);
            midiContinuousController = ((raw & 0x0080) == 0x0080);
            sourceType = (SourceTypeEnum)((raw & (0xFC00)) >> 10);

            controllerSource = (ControllerSourceEnum)(raw & 0x007F);
            midiContinuousControllerNumber = (ushort)(raw & 0x007F);
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            if (midiContinuousController)
            {
                return String.Format("{0} CC{1}", sourceType, midiContinuousControllerNumber);
            }
            return String.Format("{0} {1}", sourceType, controllerSource);
        }

        #endregion
    }
}