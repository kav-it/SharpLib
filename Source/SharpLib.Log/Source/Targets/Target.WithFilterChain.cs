using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    internal class TargetWithFilterChain
    {
        #region Поля

        private StackTraceUsage stackTraceUsage = StackTraceUsage.None;

        #endregion

        #region Свойства

        public Target Target { get; private set; }

        public IList<Filter> FilterChain { get; private set; }

        public TargetWithFilterChain NextInChain { get; set; }

        #endregion

        #region Конструктор

        public TargetWithFilterChain(Target target, IList<Filter> filterChain)
        {
            Target = target;
            FilterChain = filterChain;
            stackTraceUsage = StackTraceUsage.None;
        }

        #endregion

        #region Методы

        public StackTraceUsage GetStackTraceUsage()
        {
            return stackTraceUsage;
        }

        internal void PrecalculateStackTraceUsage()
        {
            stackTraceUsage = StackTraceUsage.None;

            foreach (var item in ObjectGraphScanner.FindReachableObjects<IUsesStackTrace>(this))
            {
                var stu = item.StackTraceUsage;

                if (stu > stackTraceUsage)
                {
                    stackTraceUsage = stu;

                    if (stackTraceUsage >= StackTraceUsage.Max)
                    {
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
