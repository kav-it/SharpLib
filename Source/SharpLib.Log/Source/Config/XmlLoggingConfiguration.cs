using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

using NLog.Common;
using NLog.Filters;
using NLog.Internal;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Time;

namespace NLog.Config
{
    public class XmlLoggingConfiguration : LoggingConfiguration
    {
        private readonly ConfigurationItemFactory configurationItemFactory = ConfigurationItemFactory.Default;

        private readonly Dictionary<string, bool> visitedFile = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private string originalFileName;

        public XmlLoggingConfiguration(string fileName)
        {
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                Initialize(reader, fileName, false);
            }
        }

        public XmlLoggingConfiguration(string fileName, bool ignoreErrors)
        {
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                Initialize(reader, fileName, ignoreErrors);
            }
        }

        public XmlLoggingConfiguration(XmlReader reader, string fileName)
        {
            Initialize(reader, fileName, false);
        }

        public XmlLoggingConfiguration(XmlReader reader, string fileName, bool ignoreErrors)
        {
            Initialize(reader, fileName, ignoreErrors);
        }

#if !SILVERLIGHT

        internal XmlLoggingConfiguration(XmlElement element, string fileName)
        {
            using (var stringReader = new StringReader(element.OuterXml))
            {
                XmlReader reader = XmlReader.Create(stringReader);

                Initialize(reader, fileName, false);
            }
        }

        internal XmlLoggingConfiguration(XmlElement element, string fileName, bool ignoreErrors)
        {
            using (var stringReader = new StringReader(element.OuterXml))
            {
                XmlReader reader = XmlReader.Create(stringReader);

                Initialize(reader, fileName, ignoreErrors);
            }
        }
#endif

        public static LoggingConfiguration AppConfig
        {
            get { return null; }
        }

        public Dictionary<string, string> Variables
        {
            get { return variables; }
        }

        public bool AutoReload { get; set; }

        public override IEnumerable<string> FileNamesToWatch
        {
            get
            {
                if (AutoReload)
                {
                    return visitedFile.Keys;
                }

                return new string[0];
            }
        }

        public override LoggingConfiguration Reload()
        {
            return new XmlLoggingConfiguration(originalFileName);
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

        private static string StripOptionalNamespacePrefix(string attributeValue)
        {
            if (attributeValue == null)
            {
                return null;
            }

            int p = attributeValue.IndexOf(':');
            if (p < 0)
            {
                return attributeValue;
            }

            return attributeValue.Substring(p + 1);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Target is disposed elsewhere.")]
        private static Target WrapWithAsyncTargetWrapper(Target target)
        {
            var asyncTargetWrapper = new AsyncTargetWrapper();
            asyncTargetWrapper.WrappedTarget = target;
            asyncTargetWrapper.Name = target.Name;
            target.Name = target.Name + "_wrapped";
            InternalLogger.Debug("Wrapping target '{0}' with AsyncTargetWrapper and renaming to '{1}", asyncTargetWrapper.Name, target.Name);
            target = asyncTargetWrapper;
            return target;
        }

        private void Initialize(XmlReader reader, string fileName, bool ignoreErrors)
        {
            try
            {
                reader.MoveToContent();
                var content = new NLogXmlElement(reader);
                if (fileName != null)
                {
#if SILVERLIGHT
                    string key = fileName;
#else
                    string key = Path.GetFullPath(fileName);
#endif
                    visitedFile[key] = true;

                    originalFileName = fileName;
                    ParseTopLevel(content, Path.GetDirectoryName(fileName));

                    InternalLogger.Info("Configured from an XML element in {0}...", fileName);
                }
                else
                {
                    ParseTopLevel(content, null);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                NLogConfigurationException ConfigException = new NLogConfigurationException("Exception occurred when loading configuration from " + fileName, exception);

                if (!ignoreErrors)
                {
                    if (LogManager.ThrowExceptions)
                    {
                        throw ConfigException;
                    }
                    InternalLogger.Error("Error in Parsing Configuration File. Exception : {0}", ConfigException);
                }
                else
                {
                    InternalLogger.Error("Error in Parsing Configuration File. Exception : {0}", ConfigException);
                }
            }
        }

        private void ConfigureFromFile(string fileName)
        {
#if SILVERLIGHT
    
            string key = fileName;
#else
            string key = Path.GetFullPath(fileName);
#endif
            if (visitedFile.ContainsKey(key))
            {
                return;
            }

            visitedFile[key] = true;

            ParseTopLevel(new NLogXmlElement(fileName), Path.GetDirectoryName(fileName));
        }

        private void ParseTopLevel(NLogXmlElement content, string baseDirectory)
        {
            content.AssertName("nlog", "configuration");

            switch (content.LocalName.ToUpper(CultureInfo.InvariantCulture))
            {
                case "CONFIGURATION":
                    ParseConfigurationElement(content, baseDirectory);
                    break;

                case "NLOG":
                    ParseNLogElement(content, baseDirectory);
                    break;
            }
        }

        private void ParseConfigurationElement(NLogXmlElement configurationElement, string baseDirectory)
        {
            InternalLogger.Trace("ParseConfigurationElement");
            configurationElement.AssertName("configuration");

            foreach (var el in configurationElement.Elements("nlog"))
            {
                ParseNLogElement(el, baseDirectory);
            }
        }

        private void ParseNLogElement(NLogXmlElement nlogElement, string baseDirectory)
        {
            InternalLogger.Trace("ParseNLogElement");
            nlogElement.AssertName("nlog");

            if (nlogElement.GetOptionalBooleanAttribute("useInvariantCulture", false))
            {
                DefaultCultureInfo = CultureInfo.InvariantCulture;
            }
            AutoReload = nlogElement.GetOptionalBooleanAttribute("autoReload", false);
            LogManager.ThrowExceptions = nlogElement.GetOptionalBooleanAttribute("throwExceptions", LogManager.ThrowExceptions);
            InternalLogger.LogToConsole = nlogElement.GetOptionalBooleanAttribute("internalLogToConsole", InternalLogger.LogToConsole);
            InternalLogger.LogToConsoleError = nlogElement.GetOptionalBooleanAttribute("internalLogToConsoleError", InternalLogger.LogToConsoleError);
            InternalLogger.LogFile = nlogElement.GetOptionalAttribute("internalLogFile", InternalLogger.LogFile);
            InternalLogger.LogLevel = LogLevel.FromString(nlogElement.GetOptionalAttribute("internalLogLevel", InternalLogger.LogLevel.Name));
            LogManager.GlobalThreshold = LogLevel.FromString(nlogElement.GetOptionalAttribute("globalThreshold", LogManager.GlobalThreshold.Name));

            foreach (var el in nlogElement.Children)
            {
                switch (el.LocalName.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "EXTENSIONS":
                        ParseExtensionsElement(el, baseDirectory);
                        break;

                    case "INCLUDE":
                        ParseIncludeElement(el, baseDirectory);
                        break;

                    case "APPENDERS":
                    case "TARGETS":
                        ParseTargetsElement(el);
                        break;

                    case "VARIABLE":
                        ParseVariableElement(el);
                        break;

                    case "RULES":
                        ParseRulesElement(el, LoggingRules);
                        break;

                    case "TIME":
                        ParseTimeElement(el);
                        break;

                    default:
                        InternalLogger.Warn("Skipping unknown node: {0}", el.LocalName);
                        break;
                }
            }
        }

        private void ParseRulesElement(NLogXmlElement rulesElement, IList<LoggingRule> rulesCollection)
        {
            InternalLogger.Trace("ParseRulesElement");
            rulesElement.AssertName("rules");

            foreach (var loggerElement in rulesElement.Elements("logger"))
            {
                ParseLoggerElement(loggerElement, rulesCollection);
            }
        }

        private void ParseLoggerElement(NLogXmlElement loggerElement, IList<LoggingRule> rulesCollection)
        {
            loggerElement.AssertName("logger");

            var namePattern = loggerElement.GetOptionalAttribute("name", "*");
            var enabled = loggerElement.GetOptionalBooleanAttribute("enabled", true);
            if (!enabled)
            {
                InternalLogger.Debug("The logger named '{0}' are disabled");
                return;
            }

            var rule = new LoggingRule();
            string appendTo = loggerElement.GetOptionalAttribute("appendTo", null);
            if (appendTo == null)
            {
                appendTo = loggerElement.GetOptionalAttribute("writeTo", null);
            }

            rule.LoggerNamePattern = namePattern;
            if (appendTo != null)
            {
                foreach (string t in appendTo.Split(','))
                {
                    string targetName = t.Trim();
                    Target target = FindTargetByName(targetName);

                    if (target != null)
                    {
                        rule.Targets.Add(target);
                    }
                    else
                    {
                        throw new NLogConfigurationException("Target " + targetName + " not found.");
                    }
                }
            }

            rule.Final = loggerElement.GetOptionalBooleanAttribute("final", false);

            string levelString;

            if (loggerElement.AttributeValues.TryGetValue("level", out levelString))
            {
                LogLevel level = LogLevel.FromString(levelString);
                rule.EnableLoggingForLevel(level);
            }
            else if (loggerElement.AttributeValues.TryGetValue("levels", out levelString))
            {
                levelString = CleanWhitespace(levelString);

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
                string minLevelString;
                string maxLevelString;

                if (loggerElement.AttributeValues.TryGetValue("minLevel", out minLevelString))
                {
                    minLevel = LogLevel.FromString(minLevelString).Ordinal;
                }

                if (loggerElement.AttributeValues.TryGetValue("maxLevel", out maxLevelString))
                {
                    maxLevel = LogLevel.FromString(maxLevelString).Ordinal;
                }

                for (int i = minLevel; i <= maxLevel; ++i)
                {
                    rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                }
            }

            foreach (var child in loggerElement.Children)
            {
                switch (child.LocalName.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "FILTERS":
                        ParseFilters(rule, child);
                        break;

                    case "LOGGER":
                        ParseLoggerElement(child, rule.ChildRules);
                        break;
                }
            }

            rulesCollection.Add(rule);
        }

        private void ParseFilters(LoggingRule rule, NLogXmlElement filtersElement)
        {
            filtersElement.AssertName("filters");

            foreach (var filterElement in filtersElement.Children)
            {
                string name = filterElement.LocalName;

                Filter filter = configurationItemFactory.Filters.CreateInstance(name);
                ConfigureObjectFromAttributes(filter, filterElement, false);
                rule.Filters.Add(filter);
            }
        }

        private void ParseVariableElement(NLogXmlElement variableElement)
        {
            variableElement.AssertName("variable");

            string name = variableElement.GetRequiredAttribute("name");
            string value = ExpandVariables(variableElement.GetRequiredAttribute("value"));

            variables[name] = value;
        }

        private void ParseTargetsElement(NLogXmlElement targetsElement)
        {
            targetsElement.AssertName("targets", "appenders");

            bool asyncWrap = targetsElement.GetOptionalBooleanAttribute("async", false);
            NLogXmlElement defaultWrapperElement = null;
            var typeNameToDefaultTargetParameters = new Dictionary<string, NLogXmlElement>();

            foreach (var targetElement in targetsElement.Children)
            {
                string name = targetElement.LocalName;
                string type = StripOptionalNamespacePrefix(targetElement.GetOptionalAttribute("type", null));

                switch (name.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "DEFAULT-WRAPPER":
                        defaultWrapperElement = targetElement;
                        break;

                    case "DEFAULT-TARGET-PARAMETERS":
                        if (type == null)
                        {
                            throw new NLogConfigurationException("Missing 'type' attribute on <" + name + "/>.");
                        }

                        typeNameToDefaultTargetParameters[type] = targetElement;
                        break;

                    case "TARGET":
                    case "APPENDER":
                    case "WRAPPER":
                    case "WRAPPER-TARGET":
                    case "COMPOUND-TARGET":
                        if (type == null)
                        {
                            throw new NLogConfigurationException("Missing 'type' attribute on <" + name + "/>.");
                        }

                        Target newTarget = configurationItemFactory.Targets.CreateInstance(type);

                        NLogXmlElement defaults;
                        if (typeNameToDefaultTargetParameters.TryGetValue(type, out defaults))
                        {
                            ParseTargetElement(newTarget, defaults);
                        }

                        ParseTargetElement(newTarget, targetElement);

                        if (asyncWrap)
                        {
                            newTarget = WrapWithAsyncTargetWrapper(newTarget);
                        }

                        if (defaultWrapperElement != null)
                        {
                            newTarget = WrapWithDefaultWrapper(newTarget, defaultWrapperElement);
                        }

                        InternalLogger.Info("Adding target {0}", newTarget);
                        AddTarget(newTarget.Name, newTarget);
                        break;
                }
            }
        }

        private void ParseTargetElement(Target target, NLogXmlElement targetElement)
        {
            var compound = target as CompoundTargetBase;
            var wrapper = target as WrapperTargetBase;

            ConfigureObjectFromAttributes(target, targetElement, true);

            foreach (var childElement in targetElement.Children)
            {
                string name = childElement.LocalName;

                if (compound != null)
                {
                    if (IsTargetRefElement(name))
                    {
                        string targetName = childElement.GetRequiredAttribute("name");
                        Target newTarget = FindTargetByName(targetName);
                        if (newTarget == null)
                        {
                            throw new NLogConfigurationException("Referenced target '" + targetName + "' not found.");
                        }

                        compound.Targets.Add(newTarget);
                        continue;
                    }

                    if (IsTargetElement(name))
                    {
                        string type = StripOptionalNamespacePrefix(childElement.GetRequiredAttribute("type"));

                        Target newTarget = configurationItemFactory.Targets.CreateInstance(type);
                        if (newTarget != null)
                        {
                            ParseTargetElement(newTarget, childElement);
                            if (newTarget.Name != null)
                            {
                                AddTarget(newTarget.Name, newTarget);
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
                        string targetName = childElement.GetRequiredAttribute("name");
                        Target newTarget = FindTargetByName(targetName);
                        if (newTarget == null)
                        {
                            throw new NLogConfigurationException("Referenced target '" + targetName + "' not found.");
                        }

                        wrapper.WrappedTarget = newTarget;
                        continue;
                    }

                    if (IsTargetElement(name))
                    {
                        string type = StripOptionalNamespacePrefix(childElement.GetRequiredAttribute("type"));

                        Target newTarget = configurationItemFactory.Targets.CreateInstance(type);
                        if (newTarget != null)
                        {
                            ParseTargetElement(newTarget, childElement);
                            if (newTarget.Name != null)
                            {
                                AddTarget(newTarget.Name, newTarget);
                            }

                            if (wrapper.WrappedTarget != null)
                            {
                                throw new NLogConfigurationException("Wrapped target already defined.");
                            }

                            wrapper.WrappedTarget = newTarget;
                        }

                        continue;
                    }
                }

                SetPropertyFromElement(target, childElement);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom",
            Justification = "Need to load external assembly.")]
        private void ParseExtensionsElement(NLogXmlElement extensionsElement, string baseDirectory)
        {
            extensionsElement.AssertName("extensions");

            foreach (var addElement in extensionsElement.Elements("add"))
            {
                string prefix = addElement.GetOptionalAttribute("prefix", null);

                if (prefix != null)
                {
                    prefix = prefix + ".";
                }

                string type = StripOptionalNamespacePrefix(addElement.GetOptionalAttribute("type", null));
                if (type != null)
                {
                    configurationItemFactory.RegisterType(Type.GetType(type, true), prefix);
                }

                string assemblyFile = addElement.GetOptionalAttribute("assemblyFile", null);
                if (assemblyFile != null)
                {
                    try
                    {
#if SILVERLIGHT
                                var si = Application.GetResourceStream(new Uri(assemblyFile, UriKind.Relative));
                                var assemblyPart = new AssemblyPart();
                                Assembly asm = assemblyPart.Load(si.Stream);
#else

                        string fullFileName = Path.Combine(baseDirectory, assemblyFile);
                        InternalLogger.Info("Loading assembly file: {0}", fullFileName);

                        Assembly asm = Assembly.LoadFrom(fullFileName);
#endif
                        configurationItemFactory.RegisterItemsFromAssembly(asm, prefix);
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        InternalLogger.Error("Error loading extensions: {0}", exception);
                        if (LogManager.ThrowExceptions)
                        {
                            throw new NLogConfigurationException("Error loading extensions: " + assemblyFile, exception);
                        }
                    }

                    continue;
                }

                string assemblyName = addElement.GetOptionalAttribute("assembly", null);
                if (assemblyName != null)
                {
                    try
                    {
                        InternalLogger.Info("Loading assembly name: {0}", assemblyName);
#if SILVERLIGHT
                        var si = Application.GetResourceStream(new Uri(assemblyName + ".dll", UriKind.Relative));
                        var assemblyPart = new AssemblyPart();
                        Assembly asm = assemblyPart.Load(si.Stream);
#else
                        Assembly asm = Assembly.Load(assemblyName);
#endif

                        configurationItemFactory.RegisterItemsFromAssembly(asm, prefix);
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        InternalLogger.Error("Error loading extensions: {0}", exception);
                        if (LogManager.ThrowExceptions)
                        {
                            throw new NLogConfigurationException("Error loading extensions: " + assemblyName, exception);
                        }
                    }
                }
            }
        }

        private void ParseIncludeElement(NLogXmlElement includeElement, string baseDirectory)
        {
            includeElement.AssertName("include");

            string newFileName = includeElement.GetRequiredAttribute("file");

            try
            {
                newFileName = ExpandVariables(newFileName);
                newFileName = SimpleLayout.Evaluate(newFileName);
                if (baseDirectory != null)
                {
                    newFileName = Path.Combine(baseDirectory, newFileName);
                }

#if SILVERLIGHT
                newFileName = newFileName.Replace("\\", "/");
                if (Application.GetResourceStream(new Uri(newFileName, UriKind.Relative)) != null)
#else
                if (File.Exists(newFileName))
#endif
                {
                    InternalLogger.Debug("Including file '{0}'", newFileName);
                    ConfigureFromFile(newFileName);
                }
                else
                {
                    throw new FileNotFoundException("Included file not found: " + newFileName);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error when including '{0}' {1}", newFileName, exception);

                if (includeElement.GetOptionalBooleanAttribute("ignoreErrors", false))
                {
                    return;
                }

                throw new NLogConfigurationException("Error when including: " + newFileName, exception);
            }
        }

        private void ParseTimeElement(NLogXmlElement timeElement)
        {
            timeElement.AssertName("time");

            string type = timeElement.GetRequiredAttribute("type");

            TimeSource newTimeSource = configurationItemFactory.TimeSources.CreateInstance(type);

            ConfigureObjectFromAttributes(newTimeSource, timeElement, true);

            InternalLogger.Info("Selecting time source {0}", newTimeSource);
            TimeSource.Current = newTimeSource;
        }

        private void SetPropertyFromElement(object o, NLogXmlElement element)
        {
            if (AddArrayItemFromElement(o, element))
            {
                return;
            }

            if (SetLayoutFromElement(o, element))
            {
                return;
            }

            PropertyHelper.SetPropertyFromString(o, element.LocalName, ExpandVariables(element.Value), configurationItemFactory);
        }

        private bool AddArrayItemFromElement(object o, NLogXmlElement element)
        {
            string name = element.LocalName;

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

        private void ConfigureObjectFromAttributes(object targetObject, NLogXmlElement element, bool ignoreType)
        {
            foreach (var kvp in element.AttributeValues)
            {
                string childName = kvp.Key;
                string childValue = kvp.Value;

                if (ignoreType && childName.Equals("type", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                PropertyHelper.SetPropertyFromString(targetObject, childName, ExpandVariables(childValue), configurationItemFactory);
            }
        }

        private bool SetLayoutFromElement(object o, NLogXmlElement layoutElement)
        {
            PropertyInfo targetPropertyInfo;
            string name = layoutElement.LocalName;

            if (PropertyHelper.TryGetPropertyInfo(o, name, out targetPropertyInfo))
            {
                if (typeof(Layout).IsAssignableFrom(targetPropertyInfo.PropertyType))
                {
                    string layoutTypeName = StripOptionalNamespacePrefix(layoutElement.GetOptionalAttribute("type", null));

                    if (layoutTypeName != null)
                    {
                        Layout layout = configurationItemFactory.Layouts.CreateInstance(ExpandVariables(layoutTypeName));
                        ConfigureObjectFromAttributes(layout, layoutElement, true);
                        ConfigureObjectFromElement(layout, layoutElement);
                        targetPropertyInfo.SetValue(o, layout, null);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ConfigureObjectFromElement(object targetObject, NLogXmlElement element)
        {
            foreach (var child in element.Children)
            {
                SetPropertyFromElement(targetObject, child);
            }
        }

        private Target WrapWithDefaultWrapper(Target t, NLogXmlElement defaultParameters)
        {
            string wrapperType = StripOptionalNamespacePrefix(defaultParameters.GetRequiredAttribute("type"));

            Target wrapperTargetInstance = configurationItemFactory.Targets.CreateInstance(wrapperType);
            WrapperTargetBase wtb = wrapperTargetInstance as WrapperTargetBase;
            if (wtb == null)
            {
                throw new NLogConfigurationException("Target type specified on <default-wrapper /> is not a wrapper.");
            }

            ParseTargetElement(wrapperTargetInstance, defaultParameters);
            while (wtb.WrappedTarget != null)
            {
                wtb = wtb.WrappedTarget as WrapperTargetBase;
                if (wtb == null)
                {
                    throw new NLogConfigurationException("Child target type specified on <default-wrapper /> is not a wrapper.");
                }
            }

            wtb.WrappedTarget = t;
            wrapperTargetInstance.Name = t.Name;
            t.Name = t.Name + "_wrapped";

            InternalLogger.Debug("Wrapping target '{0}' with '{1}' and renaming to '{2}", wrapperTargetInstance.Name, wrapperTargetInstance.GetType().Name, t.Name);
            return wrapperTargetInstance;
        }

        private string ExpandVariables(string input)
        {
            string output = input;

            foreach (var kvp in variables)
            {
                output = output.Replace("${" + kvp.Key + "}", kvp.Value);
            }

            return output;
        }
    }
}