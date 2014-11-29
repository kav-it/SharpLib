using System;

namespace NLog.Config
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ThreadAgnosticAttribute : Attribute
    {
    }
}