using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal sealed class ContractAnnotationAttribute : Attribute
    {
        #region ��������

        public string Contract { get; private set; }

        public bool ForceFullStates { get; private set; }

        #endregion

        #region �����������

        public ContractAnnotationAttribute([NotNull] string contract)
            : this(contract, false)
        {
        }

        public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
        {
            Contract = contract;
            ForceFullStates = forceFullStates;
        }

        #endregion
    }
}