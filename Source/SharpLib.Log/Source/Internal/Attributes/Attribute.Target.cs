using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TargetAttribute : NameBaseAttribute
    {
        #region Свойства

        public bool IsWrapper { get; set; }

        public bool IsCompound { get; set; }

        #endregion

        #region Конструктор

        public TargetAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}