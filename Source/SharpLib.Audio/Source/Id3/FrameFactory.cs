using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Id3Lib.Exceptions;
using Id3Lib.Frames;

namespace Id3Lib
{
    internal static class FrameFactory
    {
        #region Поля

        private static readonly Dictionary<string, Type> _frames = new Dictionary<string, Type>();

        #endregion

        #region Конструктор

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static FrameFactory()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                FrameAttribute[] frameAttributes = type.GetCustomAttributes(typeof(FrameAttribute), false) as FrameAttribute[];
                foreach (FrameAttribute frameAttribute in frameAttributes)
                {
                    _frames.Add(frameAttribute.FrameId, type);
                }
            }
        }

        #endregion

        #region Методы

        public static FrameBase Build(string frameId)
        {
            if (frameId == null)
            {
                throw new ArgumentNullException("frameId");
            }

            if (frameId.Length != 4)
            {
                throw new InvalidTagException("Invalid frame type: '" + frameId + "', it must be 4 characters long.");
            }

            Type type = null;
            if (_frames.TryGetValue(frameId, out type))
            {
                return (FrameBase)Activator.CreateInstance(type, frameId);
            }

            if (_frames.TryGetValue(frameId.Substring(0, 1), out type))
            {
                return (FrameBase)Activator.CreateInstance(type, frameId);
            }

            return new FrameUnknown(frameId);
        }

        #endregion
    }
}