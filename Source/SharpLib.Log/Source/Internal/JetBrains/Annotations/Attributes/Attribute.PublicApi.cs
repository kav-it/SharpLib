using System;

namespace JetBrains.Annotations
{
    [MeansImplicitUse]
    internal sealed class PublicAPIAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string Comment { get; private set; }

        #endregion

        #region Конструктор

        public PublicAPIAttribute()
        {
        }

        public PublicAPIAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        #endregion
    }
}