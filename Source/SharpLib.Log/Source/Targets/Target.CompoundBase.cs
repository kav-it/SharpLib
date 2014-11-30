using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    public abstract class CompoundTargetBase : Target
    {
        #region Свойства

        public IList<Target> Targets { get; private set; }

        #endregion

        #region Конструктор

        protected CompoundTargetBase(params Target[] targets)
        {
            Targets = new List<Target>(targets);
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            string separator = string.Empty;
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("(");

            foreach (var t in Targets)
            {
                sb.Append(separator);
                sb.Append(t);
                separator = ", ";
            }

            sb.Append(")");
            return sb.ToString();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            throw new NotSupportedException("This target must not be invoked in a synchronous way.");
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            AsyncHelpers.ForEachItemInParallel(Targets, asyncContinuation, (t, c) => t.Flush(c));
        }

        #endregion
    }
}