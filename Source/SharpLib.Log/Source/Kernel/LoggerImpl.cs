using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SharpLib.Log
{
    internal static class LoggerImpl
    {
        #region Константы

        private const int STACK_TRACE_SKIP_METHODS = 0;

        #endregion

        #region Поля

        private static readonly Assembly _mscorlibAssembly = typeof(string).Assembly;

        private static readonly Assembly _logAssembly = typeof(LoggerImpl).Assembly;

        private static readonly Assembly _systemAssembly = typeof(Debug).Assembly;

        #endregion

        #region Методы

        internal static void Write(Type loggerType, TargetWithFilterChain targets, LogEventInfo logEvent)
        {
            if (targets == null)
            {
                return;
            }

            var stu = targets.GetStackTraceUsage();

            if (stu != StackTraceUsage.None && !logEvent.HasStackTrace)
            {
                var stackTrace = new StackTrace(STACK_TRACE_SKIP_METHODS, stu == StackTraceUsage.WithSource);

                int firstUserFrame = FindCallingMethodOnStackTrace(stackTrace, loggerType);

                logEvent.SetStackTrace(stackTrace, firstUserFrame);
            }

            for (var t = targets; t != null; t = t.NextInChain)
            {
                var result = WriteToTargetWithFilterChain(t, logEvent);

                if (result == false)
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
                    var frame = stackTrace.GetFrame(i);
                    var mb = frame.GetMethod();

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
                    var frame = stackTrace.GetFrame(i);
                    var mb = frame.GetMethod();
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
            if (assembly == _logAssembly)
            {
                return true;
            }

            if (assembly == _mscorlibAssembly)
            {
                return true;
            }

            if (assembly == _systemAssembly)
            {
                return true;
            }

            return false;
        }

        private static bool WriteToTargetWithFilterChain(TargetWithFilterChain targetListHead, LogEventInfo logEvent)
        {
            Target target = targetListHead.Target;
            FilterResult result = GetFilterResult(targetListHead.FilterChain, logEvent);

            if ((result == FilterResult.Ignore) || (result == FilterResult.IgnoreFinal))
            {
                if (result == FilterResult.IgnoreFinal)
                {
                    return false;
                }

                return true;
            }

            target.WriteAsyncLogEvent(logEvent.WithContinuation());
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

                return FilterResult.Ignore;
            }
        }

        #endregion
    }
}