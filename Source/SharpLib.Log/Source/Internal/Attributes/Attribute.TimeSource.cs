using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TimeSourceAttribute : NameBaseAttribute
    {
        #region Конструктор

        public TimeSourceAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}