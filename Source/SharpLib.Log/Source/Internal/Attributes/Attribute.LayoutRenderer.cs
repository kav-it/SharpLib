
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LayoutRendererAttribute : NameBaseAttribute
    {
        #region Конструктор

        public LayoutRendererAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}
