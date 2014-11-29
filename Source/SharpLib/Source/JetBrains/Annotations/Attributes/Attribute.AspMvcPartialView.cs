using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcPartialViewAttribute : PathReferenceAttribute
    {
    }
}