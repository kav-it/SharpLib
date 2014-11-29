using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property |
        AttributeTargets.Field, Inherited = true)]
    internal sealed class HtmlElementAttributesAttribute : Attribute
    {
        #region ��������

        [NotNull]
        public string Name { get; private set; }

        #endregion

        #region �����������

        public HtmlElementAttributesAttribute()
        {
        }

        public HtmlElementAttributesAttribute([NotNull] string name)
        {
            Name = name;
        }

        #endregion
    }
}