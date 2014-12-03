
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpLib.Log
{
    public class ConfigurationItemFactory
    {
        #region Поля

        private readonly IList<object> _allFactories;

        private readonly Factory<LayoutRenderer, AmbientPropertyAttribute> _ambientProperties;

        private readonly MethodFactory<ConditionMethodsAttribute, ConditionMethodAttribute> _conditionMethods;

        private readonly Factory<Filter, FilterAttribute> _filters;

        private readonly Factory<LayoutRenderer, LayoutRendererAttribute> _layoutRenderers;

        private readonly Factory<Layout, LayoutAttribute> _layouts;

        private readonly Factory<Target, TargetAttribute> _targets;

        private readonly Factory<TimeSource, TimeSourceAttribute> _timeSources;

        #endregion

        #region Свойства

        public static ConfigurationItemFactory Default { get; set; }

        internal ConfigurationItemCreator CreateInstance { get; set; }

        public INamedItemFactory<Target, Type> Targets
        {
            get { return _targets; }
        }

        public INamedItemFactory<Filter, Type> Filters
        {
            get { return _filters; }
        }

        public INamedItemFactory<LayoutRenderer, Type> LayoutRenderers
        {
            get { return _layoutRenderers; }
        }

        public INamedItemFactory<Layout, Type> Layouts
        {
            get { return _layouts; }
        }

        public INamedItemFactory<LayoutRenderer, Type> AmbientProperties
        {
            get { return _ambientProperties; }
        }

        public INamedItemFactory<TimeSource, Type> TimeSources
        {
            get { return _timeSources; }
        }

        public INamedItemFactory<MethodInfo, MethodInfo> ConditionMethods
        {
            get { return _conditionMethods; }
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
            _targets = new Factory<Target, TargetAttribute>(this);
            _filters = new Factory<Filter, FilterAttribute>(this);
            _layoutRenderers = new Factory<LayoutRenderer, LayoutRendererAttribute>(this);
            _layouts = new Factory<Layout, LayoutAttribute>(this);
            _conditionMethods = new MethodFactory<ConditionMethodsAttribute, ConditionMethodAttribute>();
            _ambientProperties = new Factory<LayoutRenderer, AmbientPropertyAttribute>(this);
            _timeSources = new Factory<TimeSource, TimeSourceAttribute>(this);
            _allFactories = new List<object>
            {
                _targets,
                _filters,
                _layoutRenderers,
                _layouts,
                _conditionMethods,
                _ambientProperties,
                _timeSources,
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
            var typesToScan = assembly.SafeGetTypes();
            foreach (IFactory f in _allFactories)
            {
                f.ScanTypes(typesToScan, itemNamePrefix);
            }
        }

        public void Clear()
        {
            foreach (IFactory f in _allFactories)
            {
                f.Clear();
            }
        }

        public void RegisterType(Type type, string itemNamePrefix)
        {
            foreach (IFactory f in _allFactories)
            {
                f.RegisterType(type, itemNamePrefix);
            }
        }

        private static ConfigurationItemFactory BuildDefaultFactory()
        {
            var factory = new ConfigurationItemFactory(typeof(Logger).Assembly);

            return factory;
        }

        #endregion
    }
}
