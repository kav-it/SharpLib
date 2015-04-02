using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Highlighting.Xshd
{
    [Serializable]
    internal sealed class XmlHighlightingDefinition : IHighlightingDefinition
    {
        #region Поля

        private readonly Dictionary<string, HighlightingColor> _colorDict;

        [OptionalField]
        private readonly Dictionary<string, string> _propDict;

        private readonly Dictionary<string, HighlightingRuleSet> _ruleSetDict;

        #endregion

        #region Свойства

        public string Name { get; private set; }

        public HighlightingRuleSet MainRuleSet { get; private set; }

        public IEnumerable<HighlightingColor> NamedHighlightingColors
        {
            get { return _colorDict.Values; }
        }

        public IDictionary<string, string> Properties
        {
            get { return _propDict; }
        }

        #endregion

        #region Конструктор

        public XmlHighlightingDefinition(XshdSyntaxDefinition xshd, IHighlightingDefinitionReferenceResolver resolver)
        {
            _ruleSetDict = new Dictionary<string, HighlightingRuleSet>();
            _propDict = new Dictionary<string, string>();
            _colorDict = new Dictionary<string, HighlightingColor>();
            Name = xshd.Name;

            var rnev = new RegisterNamedElementsVisitor(this);
            xshd.AcceptElements(rnev);

            foreach (XshdElement element in xshd.Elements)
            {
                var xrs = element as XshdRuleSet;
                if (xrs != null && xrs.Name == null)
                {
                    if (MainRuleSet != null)
                    {
                        throw Error(element, "Duplicate main RuleSet. There must be only one nameless RuleSet!");
                    }
                    MainRuleSet = rnev._ruleSets[xrs];
                }
            }
            if (MainRuleSet == null)
            {
                throw new HighlightingDefinitionInvalidException("Could not find main RuleSet.");
            }

            xshd.AcceptElements(new TranslateElementVisitor(this, rnev._ruleSets, resolver));

            foreach (var p in xshd.Elements.OfType<XshdProperty>())
            {
                _propDict.Add(p.Name, p.Value);
            }
        }

        #endregion

        #region Методы

        private static Exception Error(XshdElement element, string message)
        {
            if (element.LineNumber > 0)
            {
                return new HighlightingDefinitionInvalidException(
                    "Error at line " + element.LineNumber + ":\n" + message);
            }
            return new HighlightingDefinitionInvalidException(message);
        }

        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return MainRuleSet;
            }
            HighlightingRuleSet r;
            if (_ruleSetDict.TryGetValue(name, out r))
            {
                return r;
            }
            return null;
        }

        public HighlightingColor GetNamedColor(string name)
        {
            HighlightingColor c;
            if (_colorDict.TryGetValue(name, out c))
            {
                return c;
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Вложенный класс: RegisterNamedElementsVisitor

        private sealed class RegisterNamedElementsVisitor : IXshdVisitor
        {
            #region Поля

            private readonly XmlHighlightingDefinition _def;

            internal readonly Dictionary<XshdRuleSet, HighlightingRuleSet> _ruleSets;

            #endregion

            #region Конструктор

            public RegisterNamedElementsVisitor(XmlHighlightingDefinition def)
            {
                _ruleSets = new Dictionary<XshdRuleSet, HighlightingRuleSet>();
                _def = def;
            }

            #endregion

            #region Методы

            public object VisitRuleSet(XshdRuleSet ruleSet)
            {
                var hrs = new HighlightingRuleSet();
                _ruleSets.Add(ruleSet, hrs);
                if (ruleSet.Name != null)
                {
                    if (ruleSet.Name.Length == 0)
                    {
                        throw Error(ruleSet, "Name must not be the empty string");
                    }
                    if (_def._ruleSetDict.ContainsKey(ruleSet.Name))
                    {
                        throw Error(ruleSet, "Duplicate rule set name '" + ruleSet.Name + "'.");
                    }

                    _def._ruleSetDict.Add(ruleSet.Name, hrs);
                }
                ruleSet.AcceptElements(this);
                return null;
            }

            public object VisitColor(XshdColor color)
            {
                if (color.Name != null)
                {
                    if (color.Name.Length == 0)
                    {
                        throw Error(color, "Name must not be the empty string");
                    }
                    if (_def._colorDict.ContainsKey(color.Name))
                    {
                        throw Error(color, "Duplicate color name '" + color.Name + "'.");
                    }

                    _def._colorDict.Add(color.Name, new HighlightingColor());
                }
                return null;
            }

            public object VisitKeywords(XshdKeywords keywords)
            {
                return keywords.ColorReference.AcceptVisitor(this);
            }

            public object VisitSpan(XshdSpan span)
            {
                span.BeginColorReference.AcceptVisitor(this);
                span.SpanColorReference.AcceptVisitor(this);
                span.EndColorReference.AcceptVisitor(this);
                return span.RuleSetReference.AcceptVisitor(this);
            }

            public object VisitImport(XshdImport import)
            {
                return import.RuleSetReference.AcceptVisitor(this);
            }

            public object VisitRule(XshdRule rule)
            {
                return rule.ColorReference.AcceptVisitor(this);
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: TranslateElementVisitor

        private sealed class TranslateElementVisitor : IXshdVisitor
        {
            #region Поля

            private readonly XmlHighlightingDefinition _def;

            private readonly HashSet<XshdRuleSet> _processedRuleSets;

            private readonly HashSet<XshdRuleSet> _processingStartedRuleSets;

            private readonly IHighlightingDefinitionReferenceResolver _resolver;

            private readonly Dictionary<HighlightingRuleSet, XshdRuleSet> _reverseRuleSetDict;

            private readonly Dictionary<XshdRuleSet, HighlightingRuleSet> _ruleSetDict;

            private bool _ignoreCase;

            #endregion

            #region Конструктор

            public TranslateElementVisitor(XmlHighlightingDefinition def, Dictionary<XshdRuleSet, HighlightingRuleSet> ruleSetDict, IHighlightingDefinitionReferenceResolver resolver)
            {
                _processingStartedRuleSets = new HashSet<XshdRuleSet>();
                _processedRuleSets = new HashSet<XshdRuleSet>();
                Debug.Assert(def != null);
                Debug.Assert(ruleSetDict != null);
                _def = def;
                _ruleSetDict = ruleSetDict;
                _resolver = resolver;
                _reverseRuleSetDict = new Dictionary<HighlightingRuleSet, XshdRuleSet>();
                foreach (var pair in ruleSetDict)
                {
                    _reverseRuleSetDict.Add(pair.Value, pair.Key);
                }
            }

            #endregion

            #region Методы

            public object VisitRuleSet(XshdRuleSet ruleSet)
            {
                var rs = _ruleSetDict[ruleSet];
                if (_processedRuleSets.Contains(ruleSet))
                {
                    return rs;
                }
                if (!_processingStartedRuleSets.Add(ruleSet))
                {
                    throw Error(ruleSet, "RuleSet cannot be processed because it contains cyclic <Import>");
                }

                bool oldIgnoreCase = _ignoreCase;
                if (ruleSet.IgnoreCase != null)
                {
                    _ignoreCase = ruleSet.IgnoreCase.Value;
                }

                rs.Name = ruleSet.Name;

                foreach (XshdElement element in ruleSet.Elements)
                {
                    var o = element.AcceptVisitor(this);
                    var elementRuleSet = o as HighlightingRuleSet;
                    if (elementRuleSet != null)
                    {
                        Merge(rs, elementRuleSet);
                    }
                    else
                    {
                        var span = o as HighlightingSpan;
                        if (span != null)
                        {
                            rs.Spans.Add(span);
                        }
                        else
                        {
                            var elementRule = o as HighlightingRule;
                            if (elementRule != null)
                            {
                                rs.Rules.Add(elementRule);
                            }
                        }
                    }
                }

                _ignoreCase = oldIgnoreCase;
                _processedRuleSets.Add(ruleSet);

                return rs;
            }

            private static void Merge(HighlightingRuleSet target, HighlightingRuleSet source)
            {
                target.Rules.AddRange(source.Rules);
                target.Spans.AddRange(source.Spans);
            }

            public object VisitColor(XshdColor color)
            {
                HighlightingColor c;
                if (color.Name != null)
                {
                    c = _def._colorDict[color.Name];
                }
                else if (color.Foreground == null && color.FontStyle == null && color.FontWeight == null)
                {
                    return null;
                }
                else
                {
                    c = new HighlightingColor();
                }

                c.Name = color.Name;
                c.Foreground = color.Foreground;
                c.Background = color.Background;
                c.Underline = color.Underline;
                c.FontStyle = color.FontStyle;
                c.FontWeight = color.FontWeight;
                return c;
            }

            public object VisitKeywords(XshdKeywords keywords)
            {
                if (keywords.Words.Count == 0)
                {
                    return Error(keywords, "Keyword group must not be empty.");
                }
                if (keywords.Words.Any(string.IsNullOrEmpty))
                {
                    throw Error(keywords, "Cannot use empty string as keyword");
                }
                var keyWordRegex = new StringBuilder();

                if (keywords.Words.All(IsSimpleWord))
                {
                    keyWordRegex.Append(@"\b(?>");

                    int i = 0;
                    foreach (string keyword in keywords.Words.OrderByDescending(w => w.Length))
                    {
                        if (i++ > 0)
                        {
                            keyWordRegex.Append('|');
                        }
                        keyWordRegex.Append(Regex.Escape(keyword));
                    }
                    keyWordRegex.Append(@")\b");
                }
                else
                {
                    keyWordRegex.Append('(');
                    int i = 0;
                    foreach (string keyword in keywords.Words)
                    {
                        if (i++ > 0)
                        {
                            keyWordRegex.Append('|');
                        }
                        if (char.IsLetterOrDigit(keyword[0]))
                        {
                            keyWordRegex.Append(@"\b");
                        }
                        keyWordRegex.Append(Regex.Escape(keyword));
                        if (char.IsLetterOrDigit(keyword[keyword.Length - 1]))
                        {
                            keyWordRegex.Append(@"\b");
                        }
                    }
                    keyWordRegex.Append(')');
                }
                return new HighlightingRule
                {
                    Color = GetColor(keywords, keywords.ColorReference),
                    Regex = CreateRegex(keywords, keyWordRegex.ToString(), XshdRegexType.Default)
                };
            }

            private static bool IsSimpleWord(string word)
            {
                return char.IsLetterOrDigit(word[0]) && char.IsLetterOrDigit(word, word.Length - 1);
            }

            private Regex CreateRegex(XshdElement position, string regex, XshdRegexType regexType)
            {
                if (regex == null)
                {
                    throw Error(position, "Regex missing");
                }
                var options = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
                if (regexType == XshdRegexType.IgnorePatternWhitespace)
                {
                    options |= RegexOptions.IgnorePatternWhitespace;
                }
                if (_ignoreCase)
                {
                    options |= RegexOptions.IgnoreCase;
                }
                try
                {
                    return new Regex(regex, options);
                }
                catch (ArgumentException ex)
                {
                    throw Error(position, ex.Message);
                }
            }

            private HighlightingColor GetColor(XshdElement position, XshdReference<XshdColor> colorReference)
            {
                if (colorReference.InlineElement != null)
                {
                    return (HighlightingColor)colorReference.InlineElement.AcceptVisitor(this);
                }
                if (colorReference.ReferencedElement != null)
                {
                    var definition = GetDefinitionByName(position, colorReference.ReferencedDefinition);
                    var color = definition.GetNamedColor(colorReference.ReferencedElement);
                    if (color == null)
                    {
                        throw Error(position, "Could not find color named '" + colorReference.ReferencedElement + "'.");
                    }
                    return color;
                }
                return null;
            }

            private IHighlightingDefinition GetDefinitionByName(XshdElement position, string definitionName)
            {
                if (definitionName == null)
                {
                    return _def;
                }
                if (_resolver == null)
                {
                    throw Error(position, "Resolving references to other syntax definitions is not possible because the IHighlightingDefinitionReferenceResolver is null.");
                }
                var d = _resolver.GetDefinitionByName(definitionName);
                if (d == null)
                {
                    throw Error(position, "Could not find definition with name '" + definitionName + "'.");
                }
                return d;
            }

            private HighlightingRuleSet GetRuleSet(XshdElement position, XshdReference<XshdRuleSet> ruleSetReference)
            {
                if (ruleSetReference.InlineElement != null)
                {
                    return (HighlightingRuleSet)ruleSetReference.InlineElement.AcceptVisitor(this);
                }
                if (ruleSetReference.ReferencedElement != null)
                {
                    var definition = GetDefinitionByName(position, ruleSetReference.ReferencedDefinition);
                    var ruleSet = definition.GetNamedRuleSet(ruleSetReference.ReferencedElement);
                    if (ruleSet == null)
                    {
                        throw Error(position, "Could not find rule set named '" + ruleSetReference.ReferencedElement + "'.");
                    }
                    return ruleSet;
                }
                return null;
            }

            public object VisitSpan(XshdSpan span)
            {
                string endRegex = span.EndRegex;
                if (string.IsNullOrEmpty(span.BeginRegex) && string.IsNullOrEmpty(span.EndRegex))
                {
                    throw Error(span, "Span has no start/end regex.");
                }
                if (!span.Multiline)
                {
                    if (endRegex == null)
                    {
                        endRegex = "$";
                    }
                    else if (span.EndRegexType == XshdRegexType.IgnorePatternWhitespace)
                    {
                        endRegex = "($|" + endRegex + "\n)";
                    }
                    else
                    {
                        endRegex = "($|" + endRegex + ")";
                    }
                }
                var wholeSpanColor = GetColor(span, span.SpanColorReference);
                return new HighlightingSpan
                {
                    StartExpression = CreateRegex(span, span.BeginRegex, span.BeginRegexType),
                    EndExpression = CreateRegex(span, endRegex, span.EndRegexType),
                    RuleSet = GetRuleSet(span, span.RuleSetReference),
                    StartColor = GetColor(span, span.BeginColorReference),
                    SpanColor = wholeSpanColor,
                    EndColor = GetColor(span, span.EndColorReference),
                    SpanColorIncludesStart = true,
                    SpanColorIncludesEnd = true
                };
            }

            public object VisitImport(XshdImport import)
            {
                var hrs = GetRuleSet(import, import.RuleSetReference);
                XshdRuleSet inputRuleSet;
                if (_reverseRuleSetDict.TryGetValue(hrs, out inputRuleSet))
                {
                    if (VisitRuleSet(inputRuleSet) != hrs)
                    {
                        Debug.Fail("this shouldn't happen");
                    }
                }
                return hrs;
            }

            public object VisitRule(XshdRule rule)
            {
                return new HighlightingRule
                {
                    Color = GetColor(rule, rule.ColorReference),
                    Regex = CreateRegex(rule, rule.Regex, rule.RegexType)
                };
            }

            #endregion
        }

        #endregion
    }
}