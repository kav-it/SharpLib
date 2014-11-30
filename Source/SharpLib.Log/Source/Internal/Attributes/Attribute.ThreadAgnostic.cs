
using System;

namespace SharpLib.Log
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ThreadAgnosticAttribute : Attribute
    {
    }
}
