using System;

namespace NLog.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredParameterAttribute : Attribute
    {
    }
}