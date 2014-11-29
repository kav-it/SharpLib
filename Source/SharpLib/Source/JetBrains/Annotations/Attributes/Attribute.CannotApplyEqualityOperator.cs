using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class |
        AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    internal sealed class CannotApplyEqualityOperatorAttribute : Attribute
    {
    }
}