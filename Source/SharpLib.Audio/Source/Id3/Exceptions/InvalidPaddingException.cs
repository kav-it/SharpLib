using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Id3Lib.Exceptions
{
    [Serializable]
    internal class InvalidPaddingException : InvalidStructureException
    {
        #region Поля

        private readonly uint _measured;

        private readonly uint _specified;

        #endregion

        #region Свойства

        public uint Measured
        {
            get { return _measured; }
        }

        public uint Specified
        {
            get { return _specified; }
        }

        public override string Message
        {
            get
            {
                if (_measured > _specified)
                {
                    return string.Format(CultureInfo.InvariantCulture, "Padding is corrupt; {0} zero bytes found, but only {1} bytes should be left between last id3v2 frame and the end of the tag",
                        _measured, _specified);
                }
                return string.Format(CultureInfo.InvariantCulture, "Padding is corrupt; {1} bytes should be left between last id3v2 frame and the end of the tag, but only {0} zero bytes found.",
                    _measured, _specified);
            }
        }

        #endregion

        #region Конструктор

        protected InvalidPaddingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _measured = info.GetUInt32("measured");
            _specified = info.GetUInt32("specified");
        }

        public InvalidPaddingException()
        {
        }

        public InvalidPaddingException(string message)
            : base(message)
        {
        }

        public InvalidPaddingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public InvalidPaddingException(uint measured, uint specified)
            : base("Padding is corrupt, must be zeroes to end of id3 tag.")
        {
            Debug.Assert(measured != specified);
            _measured = measured;
            _specified = specified;
        }

        #endregion

        #region Методы

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("measured", _measured);
            info.AddValue("specified", _specified);
            base.GetObjectData(info, context);
        }

        #endregion
    }
}