using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcControllerAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region Конструктор

        public AspMvcControllerAttribute()
        {
        }

        public AspMvcControllerAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        #endregion
    }
}