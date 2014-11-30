using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class PathReferenceAttribute : Attribute
    {
        #region ��������

        [NotNull]
        public string BasePath { get; private set; }

        #endregion

        #region �����������

        public PathReferenceAttribute()
        {
        }

        public PathReferenceAttribute([PathReference] string basePath)
        {
            BasePath = basePath;
        }

        #endregion
    }
}