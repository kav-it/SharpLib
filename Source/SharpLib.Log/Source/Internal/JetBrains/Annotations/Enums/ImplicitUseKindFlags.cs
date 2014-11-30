using System;

namespace JetBrains.Annotations
{
    [Flags]
    internal enum ImplicitUseKindFlags
    {
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

        Access = 1,

        Assign = 2,

        InstantiatedWithFixedConstructorSignature = 4,

        InstantiatedNoFixedConstructorSignature = 8,
    }
}