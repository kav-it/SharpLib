using System;

namespace SharpLib.Json
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnErrorAttribute : Attribute
    {
    }
}