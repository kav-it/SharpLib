using System;

namespace Id3Lib.Frames
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class FrameAttribute : Attribute
    {
        #region Поля

        private readonly string _frameId;

        #endregion

        #region Свойства

        public string FrameId
        {
            get { return _frameId; }
        }

        #endregion

        #region Конструктор

        public FrameAttribute(string frameId)
        {
            if (frameId == null)
            {
                throw new ArgumentNullException("frameId");
            }

            _frameId = frameId;
        }

        #endregion
    }
}