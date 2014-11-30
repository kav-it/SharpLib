using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, Inherited = true)]
    internal sealed class RazorSectionAttribute : Attribute
    {
    }
}