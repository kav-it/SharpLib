using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    internal sealed class PureAttribute : Attribute
    {
    }
}