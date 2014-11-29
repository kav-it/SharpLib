using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    internal sealed class InvokerParameterNameAttribute : Attribute
    {
    }
}