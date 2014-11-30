
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FilterAttribute : NameBaseAttribute
    {
        #region Конструктор

        public FilterAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}
