using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace SharpLib.Log
{
    public class XmlLoggingConfiguration : LoggingConfiguration
    {
        #region Константы

        private const string ATTR_NAME_TYPE = @"type";

        private const string ATTR_NAME_NAME = @"name";

        private const string ATTR_NAME_VALUE = @"value";

        private const string LOG_ATTR_AUTO_RELOAD = @"autoReload";

        private const string LOG_ATTR_GLOBAL_THRESHOLD = @"globalThreshold";

        private const string LOG_CHILD_RULES = @"rules";

        private const string LOG_CHILD_TARGETS = @"targets";

        private const string LOG_CHILD_TIME = @"time";

        private const string LOG_CHILD_VARIABLE = @"variable";

        private const string TARGETS_ATTR_ASYNC = @"async";

        private const string TARGETS_CHILD_TARGET = @"target";

        #endregion

        #region Поля

        private readonly ConfigurationItemFactory _configurationItemFactory;

        private readonly Dictionary<string, bool> _visitedFile;

        private string _originalFileName;

        #endregion

        #region Свойства

        public Dictionary<string, string> Variables { get; private set; }

        public bool AutoReload { get; set; }

        public override IEnumerable<string> FileNamesToWatch
        {
            get
            {
                if (AutoReload)
                {
                    return _visitedFile.Keys;
                }

                return new string[0];
            }
        }

        #endregion

        #region Конструктор

        public XmlLoggingConfiguration(string fileName)
        {
            Variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _visitedFile = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            _configurationItemFactory = ConfigurationItemFactory.Default;

            LoadFromFile(fileName);
        }

        #endregion

        #region Методы

        private void LoadFromFile(string fileName)
        {
            var doc = XDocument.Load(fileName);

            string key = Path.GetFullPath(fileName);
            _visitedFile[key] = true;

            _originalFileName = fileName;
            ParseRootElement(doc.Root);
        }

        /// <summary>
        /// Разбор корневого элемента
        /// </summary>
        private void ParseRootElement(XElement xElement)
        {
            var autoReload = xElement.GetAttributeBoolEx(LOG_ATTR_AUTO_RELOAD, false);
            var globalThreshold = xElement.GetAttributeStringEx(LOG_ATTR_GLOBAL_THRESHOLD, LogManager.Instance.GlobalThreshold.Name);

            AutoReload = autoReload;
            LogManager.Instance.GlobalThreshold = LogLevel.FromString(globalThreshold);

            var xChilds = xElement.Elements();

            foreach (var xChild in xChilds)
            {
                var value = xChild.Name.LocalName.ToLower(CultureInfo.InvariantCulture);

                switch (value)
                {
                    case LOG_CHILD_TARGETS:
                        ParseTargetsElement(xChild);
                        break;

                    case LOG_CHILD_VARIABLE:
                        ParseVariableElement(xChild);
                        break;

                    case LOG_CHILD_RULES:
                        ParseRulesElement(xChild, Rules);
                        break;

                    case LOG_CHILD_TIME:
                        ParseTimeElement(xChild);
                        break;
                }
            }
        }

        /// <summary>
        /// Разбор элемента 'targets'
        /// </summary>
        private void ParseTargetsElement(XElement xRoot)
        {
            var asyncWrap = xRoot.GetAttributeBoolEx(TARGETS_ATTR_ASYNC, false);
            var xChilds = xRoot.Elements();

            foreach (var xChild in xChilds)
            {
                var name = xChild.Name.LocalName.ToLower(CultureInfo.InvariantCulture);
                var typeValue = xChild.GetAttributeStringEx(ATTR_NAME_TYPE, null);
                var type = StripOptionalNamespacePrefix(typeValue);

                switch (name)
                {
                    case TARGETS_CHILD_TARGET:
                        if (type == null)
                        {
                            throw new Exception(string.Format("Пропущен аттрибут 'type' для <{0}/>", name));
                        }

                        var newTarget = _configurationItemFactory.Targets.CreateInstance(type);

                        ParseTargetElement(newTarget, xChild);

                        if (asyncWrap)
                        {
                            newTarget = WrapWithAsyncTargetWrapper(newTarget);
                        }

                        AddTarget(newTarget);
                        break;
                }
            }
        }

        /// <summary>
        /// Разбор элемента 'target'
        /// </summary>
        private void ParseTargetElement(Target target, XElement xRoot)
        {
            var compound = target as CompoundTargetBase;
            var wrapper = target as WrapperTargetBase;
            var xChilds = xRoot.Elements();

            ConfigureObjectFromAttributes(target, xRoot, true);

            foreach (var xChild in xChilds)
            {
                string name = xChild.Name.LocalName;

                if (compound != null)
                {
                    if (IsTargetRefElement(name))
                    {
                        string targetName = xChild.GetAttributeEx(ATTR_NAME_NAME);
                        Target newTarget = FindTargetByName(targetName);
                        if (newTarget == null)
                        {
                            throw new Exception(string.Format("Target '{0} не найден", targetName));
                        }

                        compound.Targets.Add(newTarget);
                        continue;
                    }

                    if (IsTargetElement(name))
                    {
                        string type = StripOptionalNamespacePrefix(xChild.GetAttributeEx(ATTR_NAME_TYPE));

                        var newTarget = _configurationItemFactory.Targets.CreateInstance(type);
                        if (newTarget != null)
                        {
                            ParseTargetElement(newTarget, xChild);
                            if (newTarget.Name != null)
                            {
                                AddTarget(newTarget);
                            }

                            compound.Targets.Add(newTarget);
                        }

                        continue;
                    }
                }

                if (wrapper != null)
                {
                    if (IsTargetRefElement(name))
                    {
                        string targetName = xChild.GetAttributeEx(ATTR_NAME_NAME);
                        Target newTarget = FindTargetByName(targetName);
                        if (newTarget == null)
                        {
                            throw new Exception(string.Format("Target '{0} не найден", targetName));
                        }

                        wrapper.WrappedTarget = newTarget;
                        continue;
                    }

                    if (IsTargetElement(name))
                    {
                        string type = StripOptionalNamespacePrefix(xChild.GetAttributeEx(ATTR_NAME_TYPE));

                        Target newTarget = _configurationItemFactory.Targets.CreateInstance(type);
                        if (newTarget != null)
                        {
                            ParseTargetElement(newTarget, xChild);
                            if (newTarget.Name != null)
                            {
                                AddTarget(newTarget);
                            }

                            if (wrapper.WrappedTarget != null)
                            {
                                throw new Exception("Wrapped target уже определен");
                            }

                            wrapper.WrappedTarget = newTarget;
                        }

                        continue;
                    }
                }

                SetPropertyFromElement(target, xChild);
            }
        }


        public override LoggingConfiguration Reload()
        {
            return new XmlLoggingConfiguration(_originalFileName);
        }

        private static bool IsTargetElement(string name)
        {
            return name.Equals("target", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("wrapper", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("wrapper-target", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("compound-target", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTargetRefElement(string name)
        {
            return name.Equals("target-ref", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("wrapper-target-ref", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("compound-target-ref", StringComparison.OrdinalIgnoreCase);
        }

        private static string CleanWhitespace(string s)
        {
            s = s.Replace(" ", string.Empty);
            return s;
        }

        private string StripOptionalNamespacePrefix(string attributeValue)
        {
            if (attributeValue == null)
            {
                return null;
            }

            int p = attributeValue.IndexOf(':');
            return p < 0 ? attributeValue : attributeValue.Substring(p + 1);
        }

        private static Target WrapWithAsyncTargetWrapper(Target target)
        {
            var asyncTargetWrapper = new AsyncTargetWrapper();
            asyncTargetWrapper.WrappedTarget = target;
            asyncTargetWrapper.Name = target.Name;
            target.Name = target.Name + "_wrapped";
            target = asyncTargetWrapper;
            return target;
        }

        private void ParseRulesElement(XElement rulesElement, IList<LoggingRule> rulesCollection)
        {
            var xChilds = rulesElement.Elements("logger");
            foreach (var loggerElement in xChilds)
            {
                ParseLoggerElement(loggerElement, rulesCollection);
            }
        }

        private void ParseLoggerElement(XElement loggerElement, IList<LoggingRule> rulesCollection)
        {
            var namePattern = loggerElement.GetAttributeStringEx("name", "*");
            var enabled = loggerElement.GetAttributeBoolEx("enabled", true);
            if (!enabled)
            {
                return;
            }

            var rule = new LoggingRule();
            var writeTo = loggerElement.GetAttributeStringEx("writeTo", null);

            rule.LoggerNamePattern = namePattern;
            if (writeTo != null)
            {
                foreach (string t in writeTo.Split(','))
                {
                    string targetName = t.Trim();
                    Target target = FindTargetByName(targetName);

                    if (target != null)
                    {
                        rule.Targets.Add(target);
                    }
                    else
                    {
                        throw new Exception("Target " + targetName + " not found.");
                    }
                }
            }

            rule.Final = loggerElement.GetAttributeBoolEx("final", false);

            var attrLevel = loggerElement.Attribute("level");
            var attrLevels = loggerElement.Attribute("levels");
            var attrMinLevel = loggerElement.GetAttributeEx("minLevel");
            var attrMaxLevel = loggerElement.GetAttributeEx("maxLevel");

            if (attrLevel != null)
            {
                var level = LogLevel.FromString(attrLevel.Value);
                rule.EnableLoggingForLevel(level);
            }
            else if (attrLevels != null)
            {
                var levelString = CleanWhitespace(attrLevels.Value);

                string[] tokens = levelString.Split(',');
                foreach (string s in tokens)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        LogLevel level = LogLevel.FromString(s);
                        rule.EnableLoggingForLevel(level);
                    }
                }
            }
            else
            {
                int minLevel = 0;
                int maxLevel = LogLevel.MaxLevel.Ordinal;

                if (attrMinLevel != null)
                {
                    minLevel = LogLevel.FromString(attrMinLevel).Ordinal;
                }

                if (attrMaxLevel != null)
                {
                    maxLevel = LogLevel.FromString(attrMaxLevel).Ordinal;
                }

                for (int i = minLevel; i <= maxLevel; ++i)
                {
                    rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                }
            }

            var xChilds = loggerElement.Elements();

            foreach (var xChild in xChilds)
            {
                var name = xChild.Name.LocalName.ToLower(CultureInfo.InvariantCulture);

                switch (name)
                {
                    case "filters":
                        ParseFilters(rule, xChild);
                        break;

                    case "logger":
                        ParseLoggerElement(xChild, rule.ChildRules);
                        break;
                }
            }

            rulesCollection.Add(rule);
        }

        private void ParseFilters(LoggingRule rule, XElement xRoot)
        {
            var xChilds = xRoot.Elements();

            foreach (var xChild in xChilds)
            {
                string name = xChild.Name.LocalName;

                var filter = _configurationItemFactory.Filters.CreateInstance(name);
                ConfigureObjectFromAttributes(filter, xChild, false);
                rule.Filters.Add(filter);
            }
        }

        private void ParseVariableElement(XElement xRoot)
        {
            string name = xRoot.GetAttributeEx(ATTR_NAME_NAME);
            string value = ExpandVariables(xRoot.GetAttributeEx(ATTR_NAME_VALUE));

            Variables[name] = value;
        }

        private void ParseTimeElement(XElement xRoot)
        {
            string type = xRoot.GetAttributeEx(ATTR_NAME_TYPE);

            TimeSource newTimeSource = _configurationItemFactory.TimeSources.CreateInstance(type);

            ConfigureObjectFromAttributes(newTimeSource, xRoot, true);

            TimeSource.Current = newTimeSource;
        }

        private void SetPropertyFromElement(object o, XElement element)
        {
            if (AddArrayItemFromElement(o, element))
            {
                return;
            }

            if (SetLayoutFromElement(o, element))
            {
                return;
            }

            PropertyHelper.SetPropertyFromString(o, element.Name.LocalName, ExpandVariables(element.Value), _configurationItemFactory);
        }

        private bool AddArrayItemFromElement(object o, XElement element)
        {
            string name = element.Name.LocalName;

            PropertyInfo propInfo;
            if (!PropertyHelper.TryGetPropertyInfo(o, name, out propInfo))
            {
                return false;
            }

            Type elementType = PropertyHelper.GetArrayItemType(propInfo);
            if (elementType != null)
            {
                IList propertyValue = (IList)propInfo.GetValue(o, null);
                object arrayItem = FactoryHelper.CreateInstance(elementType);
                ConfigureObjectFromAttributes(arrayItem, element, true);
                ConfigureObjectFromElement(arrayItem, element);
                propertyValue.Add(arrayItem);
                return true;
            }

            return false;
        }

        private void ConfigureObjectFromAttributes(object targetObject, XElement xElement, bool ignoreType)
        {
            var attributes = xElement.Attributes();

            foreach (var kvp in attributes)
            {
                string childName = kvp.Name.LocalName;
                string childValue = kvp.Value;

                if (ignoreType && childName.Equals(ATTR_NAME_TYPE, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                PropertyHelper.SetPropertyFromString(targetObject, childName, ExpandVariables(childValue), _configurationItemFactory);
            }
        }

        private bool SetLayoutFromElement(object o, XElement layoutElement)
        {
            PropertyInfo targetPropertyInfo;
            string name = layoutElement.Name.LocalName;

            if (PropertyHelper.TryGetPropertyInfo(o, name, out targetPropertyInfo))
            {
                if (typeof(Layout).IsAssignableFrom(targetPropertyInfo.PropertyType))
                {
                    string layoutTypeName = StripOptionalNamespacePrefix(layoutElement.GetAttributeStringEx(ATTR_NAME_TYPE, null));

                    if (layoutTypeName != null)
                    {
                        Layout layout = _configurationItemFactory.Layouts.CreateInstance(ExpandVariables(layoutTypeName));
                        ConfigureObjectFromAttributes(layout, layoutElement, true);
                        ConfigureObjectFromElement(layout, layoutElement);
                        targetPropertyInfo.SetValue(o, layout, null);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ConfigureObjectFromElement(object targetObject, XElement element)
        {
            var xChilds = element.Elements();

            foreach (var child in xChilds)
            {
                SetPropertyFromElement(targetObject, child);
            }
        }

        /// <summary>
        /// Замена переменных значениями
        /// </summary>
        private string ExpandVariables(string input)
        {
            return Variables.Aggregate(input, (current, entry) => current.Replace("${" + entry.Key + "}", entry.Value));
        }

        internal static string GetDefaultConfigAsText()
        {
            string result = GetResourceTextFile(@"SharpLib.Log.Source.Assets.Config.xml");

            return result;
        }

        private static string GetResourceTextFile(string filename)
        {
            string result;

            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(filename))
            {
                if (stream == null)
                {
                    throw new Exception("Не найден ресурс " + filename);
                }

                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        #endregion
    }
}