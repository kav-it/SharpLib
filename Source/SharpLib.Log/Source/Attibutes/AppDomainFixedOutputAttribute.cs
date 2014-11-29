using System;

namespace NLog.Config
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AppDomainFixedOutputAttribute : Attribute
    {
    }
}