using System;
using System.Collections.Generic;
using System.Reflection;

using NLog.Common;
using NLog.Conditions;
using NLog.Filters;
using NLog.Internal;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;
using NLog.Time;

namespace NLog.Config
{
    public class ConfigurationItemFactory
    {
        #region Поля

        private readonly IList<object> allFactories;

        private readonly Factory<LayoutRenderer, AmbientPropertyAttribute> ambientProperties;

        private readonly MethodFactory<ConditionMethodsAttribute, ConditionMethodAttribute> conditionMethods;

        private readonly Factory<Filter, FilterAttribute> filters;

        private readonly Factory<LayoutRenderer, LayoutRendererAttribute> layoutRenderers;

        private readonly Factory<Layout, LayoutAttribute> layouts;

        private readonly Factory<Target, TargetAttribute> targets;

        private readonly Factory<TimeSource, TimeSourceAttribute> timeSources;

        #endregion

        #region Свойства

        public static ConfigurationItemFactory Default { get; set; }

        public ConfigurationItemCreator CreateInstance { get; set; }

        public INamedItemFactory<Target, Type> Targets
        {
            get { return targets; }
        }

        public INamedItemFactory<Filter, Type> Filters
        {
            get { return filters; }
        }

        public INamedItemFactory<LayoutRenderer, Type> LayoutRenderers
        {
            get { return layoutRenderers; }
        }

        public INamedItemFactory<Layout, Type> Layouts
        {
            get { return layouts; }
        }

        public INamedItemFactory<LayoutRenderer, Type> AmbientProperties
        {
            get { return ambientProperties; }
        }

        public INamedItemFactory<TimeSource, Type> TimeSources
        {
            get { return timeSources; }
        }

        public INamedItemFactory<MethodInfo, MethodInfo> ConditionMethods
        {
            get { return conditionMethods; }
        }

        #endregion

        #region Конструктор

        static ConfigurationItemFactory()
        {
            Default = BuildDefaultFactory();
        }

        public ConfigurationItemFactory(params Assembly[] assemblies)
        {
            CreateInstance = FactoryHelper.CreateInstance;
            targets = new Factory<Target, TargetAttribute>(this);
            filters = new Factory<Filter, FilterAttribute>(this);
            layoutRenderers = new Factory<LayoutRenderer, LayoutRendererAttribute>(this);
            layouts = new Factory<Layout, LayoutAttribute>(this);
            conditionMethods = new MethodFactory<ConditionMethodsAttribute, ConditionMethodAttribute>();
            ambientProperties = new Factory<LayoutRenderer, AmbientPropertyAttribute>(this);
            timeSources = new Factory<TimeSource, TimeSourceAttribute>(this);
            allFactories = new List<object>
            {
                targets,
                filters,
                layoutRenderers,
                layouts,
                conditionMethods,
                ambientProperties,
                timeSources,
            };

            foreach (var asm in assemblies)
            {
                RegisterItemsFromAssembly(asm);
            }
        }

        #endregion

        #region Методы

        public void RegisterItemsFromAssembly(Assembly assembly)
        {
            RegisterItemsFromAssembly(assembly, string.Empty);
        }

        public void RegisterItemsFromAssembly(Assembly assembly, string itemNamePrefix)
        {
            InternalLogger.Debug("ScanAssembly('{0}')", assembly.FullName);
            var typesToScan = assembly.SafeGetTypes();
            foreach (IFactory f in allFactories)
            {
                f.ScanTypes(typesToScan, itemNamePrefix);
            }
        }

        public void Clear()
        {
            foreach (IFactory f in allFactories)
            {
                f.Clear();
            }
        }

        public void RegisterType(Type type, string itemNamePrefix)
        {
            foreach (IFactory f in allFactories)
            {
                f.RegisterType(type, itemNamePrefix);
            }
        }

        private static ConfigurationItemFactory BuildDefaultFactory()
        {
            var factory = new ConfigurationItemFactory(typeof(Logger).Assembly);
            factory.RegisterExtendedItems();

            return factory;
        }

        private void RegisterExtendedItems()
        {
            string suffix = typeof(Logger).AssemblyQualifiedName;
            string myAssemblyName = "NLog,";
            string extendedAssemblyName = "NLog.Extended,";
            int p = suffix.IndexOf(myAssemblyName, StringComparison.OrdinalIgnoreCase);
            if (p >= 0)
            {
                suffix = ", " + extendedAssemblyName + suffix.Substring(p + myAssemblyName.Length);

                string targetsNamespace = typeof(DebugTarget).Namespace;
                targets.RegisterNamedType("AspNetTrace", targetsNamespace + ".AspNetTraceTarget" + suffix);
                targets.RegisterNamedType("MSMQ", targetsNamespace + ".MessageQueueTarget" + suffix);
                targets.RegisterNamedType("AspNetBufferingWrapper", targetsNamespace + ".Wrappers.AspNetBufferingTargetWrapper" + suffix);

                string layoutRenderersNamespace = typeof(MessageLayoutRenderer).Namespace;
                layoutRenderers.RegisterNamedType("appsetting", layoutRenderersNamespace + ".AppSettingLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-application", layoutRenderersNamespace + ".AspNetApplicationValueLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-request", layoutRenderersNamespace + ".AspNetRequestValueLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-sessionid", layoutRenderersNamespace + ".AspNetSessionIDLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-session", layoutRenderersNamespace + ".AspNetSessionValueLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-user-authtype", layoutRenderersNamespace + ".AspNetUserAuthTypeLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-user-identity", layoutRenderersNamespace + ".AspNetUserIdentityLayoutRenderer" + suffix);
            }
        }

        #endregion
    }
}