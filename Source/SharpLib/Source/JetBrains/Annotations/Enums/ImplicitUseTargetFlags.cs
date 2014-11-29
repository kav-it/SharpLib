using System;

namespace JetBrains.Annotations
{
    [Flags]
    internal enum ImplicitUseTargetFlags
    {
        Default = Itself,

        Itself = 1,

        Members = 2,

        WithMembers = Itself | Members
    }
}