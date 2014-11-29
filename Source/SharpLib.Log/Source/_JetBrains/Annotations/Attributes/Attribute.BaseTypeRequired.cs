using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [BaseTypeRequired(typeof(Attribute))]
    internal sealed class BaseTypeRequiredAttribute : Attribute
    {
        #region ��������

        [NotNull]
        public Type BaseType { get; private set; }

        #endregion

        #region �����������

        public BaseTypeRequiredAttribute([NotNull] Type baseType)
        {
            BaseType = baseType;
        }

        #endregion
    }
}