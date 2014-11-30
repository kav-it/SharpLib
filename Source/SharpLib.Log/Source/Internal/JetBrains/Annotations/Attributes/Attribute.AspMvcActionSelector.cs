using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class AspMvcActionSelectorAttribute : Attribute
    {
    }
}