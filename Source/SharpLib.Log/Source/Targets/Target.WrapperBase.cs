using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLib.Log
{
    public abstract class WrapperTargetBase : Target
    {
        #region Свойства

        [RequiredParameter]
        public Target WrappedTarget { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return base.ToString() + "(" + WrappedTarget + ")";
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            WrappedTarget.Flush(asyncContinuation);
        }

        protected override sealed void Write(LogEventInfo logEvent)
        {
            throw new NotSupportedException("This target must not be invoked in a synchronous way.");
        }

        #endregion
    }
}