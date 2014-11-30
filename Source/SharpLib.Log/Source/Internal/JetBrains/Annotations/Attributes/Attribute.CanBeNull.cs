using System;

#pragma warning disable 1591

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter |
        AttributeTargets.Property | AttributeTargets.Delegate |
        AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    internal sealed class CanBeNullAttribute : Attribute
    {
    }
}