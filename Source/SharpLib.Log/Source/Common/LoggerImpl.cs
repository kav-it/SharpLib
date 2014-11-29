using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

using NLog.Common;
using NLog.Config;
using NLog.Filters;
using NLog.Internal;
using NLog.Targets;

namespace NLog
{
    internal static class LoggerImpl
    {
        #region Константы

        private const int StackTraceSkipMethods = 0;

        #endregion

        #region Поля

        private static readonly Assembly mscorlibAssembly = typeof(string).Assembly;

        private static readonly Assembly nlogAssembly = typeof(LoggerImpl).Assembly;

        private static readonly Assembly systemAssembly = typeof(Debug).Assembly;

        #endregion

        #region Методы

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Using 'NLog' in message.")]
        internal static void Write(Type loggerType, TargetWithFilterChain targets, LogEventInfo logEvent, LogFactory factory)
        {
            if (targets == null)
            {
                return;
            }

            StackTraceUsage stu = targets.GetStackTraceUsage();

            if (stu != StackTraceUsage.None && !logEvent.HasStackTrace)
            {
                StackTrace stackTrace;
#if !SILVERLIGHT
                stackTrace = new StackTrace(StackTraceSkipMethods, stu == StackTraceUsage.WithSource);
#else
                stackTrace = new StackTrace();
#endif

                int firstUserFrame = FindCallingMethodOnStackTrace(stackTrace, loggerType);

                logEvent.SetStackTrace(stackTrace, firstUserFrame);
            }

            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            AsyncContinuation exceptionHandler = ex =>
            {
                if (ex != null)
                {
                    if (factory.ThrowExceptions && Thread.CurrentThread.ManagedThreadId == originalThreadId)
                    {
                        throw new NLogRuntimeException("Exception occurred in NLog", ex);
                    }
                }
            };

            for (var t = targets; t != null; t = t.NextInChain)
            {
                if (!WriteToTargetWithFilterChain(t, logEvent, exceptionHandler))
                {
                    break;
                }
            }
        }

        private static int FindCallingMethodOnStackTrace(StackTrace stackTrace, Type loggerType)
        {
            int? firstUserFrame = null;

            if (loggerType != null)
            {
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    StackFrame frame = stackTrace.GetFrame(i);
                    MethodBase mb = frame.GetMethod();

                    if (mb.DeclaringType == loggerType)
                    {
                        firstUserFrame = i + 1;
                    }
                    else if (firstUserFrame != null)
                    {
                        break;
                    }
                }
            }

            if (firstUserFrame == stackTrace.FrameCount)
            {
                firstUserFrame = null;
            }

            if (firstUserFrame == null)
            {
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    StackFrame frame = stackTrace.GetFrame(i);
                    MethodBase mb = frame.GetMethod();
                    Assembly methodAssembly = null;

                    if (mb.DeclaringType != null)
                    {
                        methodAssembly = mb.DeclaringType.Assembly;
                    }

                    if (SkipAssembly(methodAssembly))
                    {
                        firstUserFrame = i + 1;
                    }
                    else
                    {
                        if (firstUserFrame != 0)
                        {
                            break;
                        }
                    }
                }
            }

            return firstUserFrame ?? 0;
        }

        private static bool SkipAssembly(Assembly assembly)
        {
            if (assembly == nlogAssembly)
            {
                return true;
            }

            if (assembly == mscorlibAssembly)
            {
                return true;
            }

            if (assembly == systemAssembly)
            {
                return true;
            }

            return false;
        }

        private static bool WriteToTargetWithFilterChain(TargetWithFilterChain targetListHead, LogEventInfo logEvent, AsyncContinuation onException)
        {
            Target target = targetListHead.Target;
            FilterResult result = GetFilterResult(targetListHead.FilterChain, logEvent);

            if ((result == FilterResult.Ignore) || (result == FilterResult.IgnoreFinal))
            {
                if (InternalLogger.IsDebugEnabled)
                {
                    InternalLogger.Debug("{0}.{1} Rejecting message because of a filter.", logEvent.LoggerName, logEvent.Level);
                }

                if (result == FilterResult.IgnoreFinal)
                {
                    return false;
                }

                return true;
            }

            target.WriteAsyncLogEvent(logEvent.WithContinuation(onException));
            if (result == FilterResult.LogFinal)
            {
                return false;
            }

            return true;
        }

        private static FilterResult GetFilterResult(IEnumerable<Filter> filterChain, LogEventInfo logEvent)
        {
            FilterResult result = FilterResult.Neutral;

            try
            {
                foreach (Filter f in filterChain)
                {
                    result = f.GetFilterResult(logEvent);
                    if (result != FilterResult.Neutral)
                    {
                        break;
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Warn("Exception during filter evaluation: {0}", exception);
                return FilterResult.Ignore;
            }
        }

        #endregion
    }
}