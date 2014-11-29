using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcActionAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region Конструктор

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