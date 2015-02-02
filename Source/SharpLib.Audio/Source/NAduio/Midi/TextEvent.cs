using System;
using System.IO;
using System.Text;

namespace SharpLib.Audio.Midi
{
    internal class TextEvent : MetaEvent
    {
        #region Поля

        private string text;

        #endregion

        #region Свойства

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                metaDataLength = text.Length;
            }
        }

        #endregion

        #region Конструктор

        public TextEvent(BinaryReader br, int length)
        {
            Encoding byteEncoding = Utils.ByteEncoding.Instance;
            text = byteEncoding.GetString(br.ReadBytes(length));
        }

        public TextEvent(string text, MetaEventType metaEventType, long absoluteTime)
            : base(metaEventType, text.Length, absoluteTime)
        {
            this.text = text;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} {1}", base.ToString(), text);
        }

        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            Encoding byteEncoding = Utils.ByteEncoding.Instance;
            byte[] encoded = byteEncoding.GetBytes(text);
            writer.Write(encoded);
        }

        #endregion
    }
}