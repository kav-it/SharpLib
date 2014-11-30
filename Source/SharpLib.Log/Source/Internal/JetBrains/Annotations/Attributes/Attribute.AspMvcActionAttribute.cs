using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcActionAttribute : Attribute
    {
        #region ��������

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region �����������

        public AspMvcActionAttribute()
        {
        }

        public AspMvcActionAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        #endregion
    }
}