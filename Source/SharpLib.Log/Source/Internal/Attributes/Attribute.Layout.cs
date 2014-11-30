
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LayoutAttribute : NameBaseAttribute
    {
        #region Конструктор

        public LayoutAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}
