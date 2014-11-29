using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class PathReferenceAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string BasePath { get; private set; }

        #endregion

        #region Конструктор

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