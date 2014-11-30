
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ConditionMethodAttribute : NameBaseAttribute
    {
        #region Конструктор

        public ConditionMethodAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}
