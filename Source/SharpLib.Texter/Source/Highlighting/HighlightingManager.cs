using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpLib.Texter.Highlighting
{
    public class HighlightingManager : IHighlightingDefinitionReferenceResolver
    {
        #region Поля

        private static readonly Lazy<DefaultHighlightingManager> _instance = new Lazy<DefaultHighlightingManager>();

        private readonly List<IHighlightingDefinition> _allHighlightings;

        private readonly Dictionary<string, IHighlightingDefinition> _highlightingsByExtension;

        private readonly Dictionary<string, IHighlightingDefinition> _highlightingsByName;

        private readonly object _lockObj;

        #endregion

        #region Свойства

        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
        {
            get
            {
                lock (_lockObj)
                {
                    return Array.AsReadOnly(_allHighlightings.ToArray());
                }
            }
        }

        public static HighlightingManager Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region Конструктор

        public HighlightingManager()
        {
            _allHighlightings = new List<IHighlightingDefinition>();
            _highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>(StringComparer.OrdinalIgnoreCase);
            _highlightingsByName = new Dictionary<string, IHighlightingDefinition>();
            _lockObj = new object();
        }

        #endregion

        #region Методы

        public IHighlightingDefinition GetDefinition(HighlightTyp typ)
        {
            var name = typ.ToStringEx();

            return GetDefinitionByName(name);
        }

        public IHighlightingDefinition GetDefinitionByName(string name)
        {
            lock (_lockObj)
            {
                var rh = _highlightingsByName.GetValueEx(name);

                return rh;
            }
        }

        public IHighlightingDefinition GetDefinitionByExtension(string extension)
        {
            lock (_lockObj)
            {
                var rh = _highlightingsByName.GetValueEx(extension);

                return rh;
            }
        }

        public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
        {
            if (highlighting == null)
            {
                throw new ArgumentNullException("highlighting");
            }

            lock (_lockObj)
            {
                _allHighlightings.Add(highlighting);
                if (name != null)
                {
                    _highlightingsByName[name] = highlighting;
                }
                if (extensions != null)
                {
                    foreach (string ext in extensions)
                    {
                        _highlightingsByExtension[ext] = highlighting;
                    }
                }
            }
        }

        #endregion
    }
}