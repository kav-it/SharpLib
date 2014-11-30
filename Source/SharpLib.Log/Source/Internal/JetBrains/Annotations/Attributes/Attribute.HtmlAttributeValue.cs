using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property, Inherited = true)]
    internal sealed class HtmlAttributeValueAttribute : Attribute
    {
        #region ��������

        [NotNull]
        public string Name { get; private set; }

        #endregion

        #region �����������

        public HtmlAttributeValueAttribute([NotNull] string name)
        {
            Name = name;
        }

        #endregion
    }
}