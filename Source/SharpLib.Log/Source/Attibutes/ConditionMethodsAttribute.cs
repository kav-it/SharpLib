using System;

namespace NLog.Conditions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ConditionMethodsAttribute : Attribute
    {
    }
}