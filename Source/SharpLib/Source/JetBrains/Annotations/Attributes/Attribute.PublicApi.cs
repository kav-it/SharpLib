using System;

namespace JetBrains.Annotations
{
    [MeansImplicitUse]
    internal sealed class PublicAPIAttribute : Attribute
    {
        #region ��������

        [NotNull]
        public string Comment { get; private set; }

        #endregion

        #region �����������

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