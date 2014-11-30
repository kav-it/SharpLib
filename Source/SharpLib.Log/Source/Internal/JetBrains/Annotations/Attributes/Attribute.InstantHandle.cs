using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
    internal sealed class InstantHandleAttribute : Attribute
    {
    }
}