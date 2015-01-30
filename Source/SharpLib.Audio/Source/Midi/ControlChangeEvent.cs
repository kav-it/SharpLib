using System;
using System.IO;

namespace NAudio.Midi
{
    internal class ControlChangeEvent : MidiEvent
    {
        #region Поля

        private MidiController controller;

        private byte controllerValue;

        #endregion

        #region Свойства

        public MidiController Controller
        {
            get { return controller; }
            set
            {
                if ((int)value < 0 || (int)value > 127)
                {
                    throw new ArgumentOutOfRangeException("value", "Controller number must be in the range 0-127");
                }
                controller = value;
            }
        }

        public int ControllerValue
        {
            get { return controllerValue; }
            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentOutOfRangeException("value", "Controller Value must be in the range 0-127");
                }
                controllerValue = (byte)value;
            }
        }

        #endregion

        #region Конструктор

        public ControlChangeEvent(BinaryReader br)
        {
            byte c = br.ReadByte();
            controllerValue = br.ReadByte();
            if ((c & 0x80) != 0)
            {
                throw new InvalidDataException("Invalid controller");
            }
            controller = (MidiController)c;
            if ((controllerValue & 0x80) != 0)
            {
                throw new InvalidDataException(String.Format("Invalid controllerValue {0} for controller {1}, Pos 0x{2:X}", controllerValue, controller, br.BaseStream.Position));
            }
        }

        public ControlChangeEvent(long absoluteTime, int channel, MidiController controller, int controllerValue)
            : base(absoluteTime, channel, MidiCommandCode.ControlChange)
        {
            Controller = controller;
            ControllerValue = controllerValue;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} Controller {1} Value {2}",
                base.ToString(),
                controller,
                controllerValue);
        }

        public override int GetAsShortMessage()
        {
            byte c = (byte)controller;
            return base.GetAsShortMessage() + (c << 8) + (controllerValue << 16);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)controller);
            writer.Write(controllerValue);
        }

        #endregion
    }
}