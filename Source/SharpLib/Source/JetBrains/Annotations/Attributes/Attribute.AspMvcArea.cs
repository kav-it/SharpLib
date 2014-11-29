using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcAreaAttribute : PathReferenceAttribute
    {
        #region Свойства

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region Конструктор

        public AspMvcAreaAttribute()
        {
        }

        public AspMvcAreaAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        #endregion
    }
}