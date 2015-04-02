using System.Collections.Generic;
using System.ComponentModel;

namespace SharpLib.Texter.Highlighting
{
    [TypeConverter(typeof(HighlightingDefinitionTypeConverter))]
    public interface IHighlightingDefinition
    {
        #region Свойства

        string Name { get; }

        HighlightingRuleSet MainRuleSet { get; }

        IEnumerable<HighlightingColor> NamedHighlightingColors { get; }

        IDictionary<string, string> Properties { get; }

        #endregion

        #region Методы

        HighlightingRuleSet GetNamedRuleSet(string name);

        HighlightingColor GetNamedColor(string name);

        #endregion
    }
}