
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AmbientPropertyAttribute : NameBaseAttribute
    {
        #region Конструктор

        public AmbientPropertyAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}
