using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Highlighting
{
    public class HighlightingManager : IHighlightingDefinitionReferenceResolver
    {
        #region Поля

        private readonly List<IHighlightingDefinition> allHighlightings = new List<IHighlightingDefinition>();

        private readonly Dictionary<string, IHighlightingDefinition> highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, IHighlightingDefinition> highlightingsByName = new Dictionary<string, IHighlightingDefinition>();

        private readonly object lockObj = new object();

        #endregion

        #region Свойства

        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
        {
            get
            {
                lock (lockObj)
                {
                    return Array.AsReadOnly(allHighlightings.ToArray());
                }
            }
        }

        public static HighlightingManager Instance
        {
            get { return DefaultHighlightingManager.Instance; }
        }

        #endregion

        #region Методы

        public IHighlightingDefinition GetDefinition(string name)
        {
            lock (lockObj)
            {
                IHighlightingDefinition rh;
                if (highlightingsByName.TryGetValue(name, out rh))
                {
                    return rh;
                }
                return null;
            }
        }

        public IHighlightingDefinition GetDefinitionByExtension(string extension)
        {
            lock (lockObj)
            {
                IHighlightingDefinition rh;
                if (highlightingsByExtension.TryGetValue(extension, out rh))
                {
                    return rh;
                }
                return null;
            }
        }

        public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
        {
            if (highlighting == null)
            {
                throw new ArgumentNullException("highlighting");
            }

            lock (lockObj)
            {
                allHighlightings.Add(highlighting);
                if (name != null)
                {
                    highlightingsByName[name] = highlighting;
                }
                if (extensions != null)
                {
                    foreach (string ext in extensions)
                    {
                        highlightingsByExtension[ext] = highlighting;
                    }
                }
            }
        }

        public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
        {
            if (lazyLoadedHighlighting == null)
            {
                throw new ArgumentNullException("lazyLoadedHighlighting");
            }
            RegisterHighlighting(name, extensions, new DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
        }

        #endregion

        #region Вложенный класс: DefaultHighlightingManager

        internal sealed class DefaultHighlightingManager : HighlightingManager
        {
            #region Поля

            public new static readonly DefaultHighlightingManager Instance = new DefaultHighlightingManager();

            #endregion

            #region Конструктор

            public DefaultHighlightingManager()
            {
                Resources.RegisterBuiltInHighlightings(this);
            }

            #endregion

            #region Методы

            internal void RegisterHighlighting(string name, string[] extensions, string resourceName)
            {
                try
                {
#if DEBUG

                    Xshd.XshdSyntaxDefinition xshd;
                    using (var s = Resources.OpenStream(resourceName))
                    {
                        using (var reader = new XmlTextReader(s))
                        {
                            xshd = Xshd.HighlightingLoader.LoadXshd(reader, false);
                        }
                    }
                    Debug.Assert(name == xshd.Name);
                    if (extensions != null)
                    {
                        Debug.Assert(System.Linq.Enumerable.SequenceEqual(extensions, xshd.Extensions));
                    }
                    else
                    {
                        Debug.Assert(xshd.Extensions.Count == 0);
                    }

                    RegisterHighlighting(name, extensions, Xshd.HighlightingLoader.Load(xshd, this));
#else
					RegisterHighlighting(name, extensions, LoadHighlighting(resourceName));
#endif
                }
                catch (HighlightingDefinitionInvalidException ex)
                {
                    throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", ex);
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "LoadHighlighting is used only in release builds")]
            private Func<IHighlightingDefinition> LoadHighlighting(string resourceName)
            {
                Func<IHighlightingDefinition> func = delegate
                {
                    Xshd.XshdSyntaxDefinition xshd;
                    using (var s = Resources.OpenStream(resourceName))
                    {
                        using (var reader = new XmlTextReader(s))
                        {
                            xshd = Xshd.HighlightingLoader.LoadXshd(reader, true);
                        }
                    }
                    return Xshd.HighlightingLoader.Load(xshd, this);
                };
                return func;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: DelayLoadedHighlightingDefinition

        private sealed class DelayLoadedHighlightingDefinition : IHighlightingDefinition
        {
            #region Поля

            private readonly object lockObj = new object();

            private readonly string name;

            private IHighlightingDefinition definition;

            private Func<IHighlightingDefinition> lazyLoadingFunction;

            private Exception storedException;

            #endregion

            #region Свойства

            public string Name
            {
                get
                {
                    if (name != null)
                    {
                        return name;
                    }
                    return GetDefinition().Name;
                }
            }

            public HighlightingRuleSet MainRuleSet
            {
                get { return GetDefinition().MainRuleSet; }
            }

            public IEnumerable<HighlightingColor> NamedHighlightingColors
            {
                get { return GetDefinition().NamedHighlightingColors; }
            }

            public IDictionary<string, string> Properties
            {
                get { return GetDefinition().Properties; }
            }

            #endregion

            #region Конструктор

            public DelayLoadedHighlightingDefinition(string name, Func<IHighlightingDefinition> lazyLoadingFunction)
            {
                this.name = name;
                this.lazyLoadingFunction = lazyLoadingFunction;
            }

            #endregion

            #region Методы

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
                Justification = "The exception will be rethrown")]
            private IHighlightingDefinition GetDefinition()
            {
                Func<IHighlightingDefinition> func;
                lock (lockObj)
                {
                    if (definition != null)
                    {
                        return definition;
                    }
                    func = lazyLoadingFunction;
                }
                Exception exception = null;
                IHighlightingDefinition def = null;
                try
                {
                    using (var busyLock = BusyManager.Enter(this))
                    {
                        if (!busyLock.Success)
                        {
                            throw new InvalidOperationException(
                                "Tried to create delay-loaded highlighting definition recursively. Make sure the are no cyclic references between the highlighting definitions.");
                        }
                        def = func();
                    }
                    if (def == null)
                    {
                        throw new InvalidOperationException("Function for delay-loading highlighting definition returned null");
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                lock (lockObj)
                {
                    lazyLoadingFunction = null;
                    if (definition == null && storedException == null)
                    {
                        definition = def;
                        storedException = exception;
                    }
                    if (storedException != null)
                    {
                        throw new HighlightingDefinitionInvalidException("Error delay-loading highlighting definition", storedException);
                    }
                    return definition;
                }
            }

            public HighlightingRuleSet GetNamedRuleSet(string name)
            {
                return GetDefinition().GetNamedRuleSet(name);
            }

            public HighlightingColor GetNamedColor(string name)
            {
                return GetDefinition().GetNamedColor(name);
            }

            public override string ToString()
            {
                return Name;
            }

            #endregion
        }

        #endregion
    }
}