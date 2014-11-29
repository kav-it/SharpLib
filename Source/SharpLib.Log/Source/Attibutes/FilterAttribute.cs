using System;

using NLog.Config;

namespace NLog.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FilterAttribute : NameBaseAttribute
    {
        #region �����������

        public FilterAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}