using System;

using NLog.Config;

namespace NLog.Time
{
    /// <summary>
    /// Marks class as a time source and assigns a name to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TimeSourceAttribute : NameBaseAttribute
    {
        #region Конструктор

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSourceAttribute" /> class.
        /// </summary>
        /// <param name="name">Name of the time source.</param>
        public TimeSourceAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}