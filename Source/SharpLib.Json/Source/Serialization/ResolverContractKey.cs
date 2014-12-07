using System;

namespace SharpLib.Json
{
    internal struct ResolverContractKey : IEquatable<ResolverContractKey>
    {
        #region Поля

        private readonly Type _contractType;

        private readonly Type _resolverType;

        #endregion

        #region Конструктор

        public ResolverContractKey(Type resolverType, Type contractType)
        {
            _resolverType = resolverType;
            _contractType = contractType;
        }

        #endregion

        #region Методы

        public override int GetHashCode()
        {
            return _resolverType.GetHashCode() ^ _contractType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ResolverContractKey))
            {
                return false;
            }

            return Equals((ResolverContractKey)obj);
        }

        public bool Equals(ResolverContractKey other)
        {
            return (_resolverType == other._resolverType && _contractType == other._contractType);
        }

        #endregion
    }
}