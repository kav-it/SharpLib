using System.Collections.Generic;
using System.Diagnostics;

namespace SharpLib.Texter.Rendering
{
    internal struct HeightTreeLineNode
    {
        #region Поля

        internal List<CollapsedLineSection> collapsedSections;

        internal double height;

        #endregion

        #region Свойства

        internal bool IsDirectlyCollapsed
        {
            get { return collapsedSections != null; }
        }

        internal double TotalHeight
        {
            get { return IsDirectlyCollapsed ? 0 : height; }
        }

        #endregion

        #region Конструктор

        internal HeightTreeLineNode(double height)
        {
            collapsedSections = null;
            this.height = height;
        }

        #endregion

        #region Методы

        internal void AddDirectlyCollapsed(CollapsedLineSection section)
        {
            if (collapsedSections == null)
            {
                collapsedSections = new List<CollapsedLineSection>();
            }
            collapsedSections.Add(section);
        }

        internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
        {
            Debug.Assert(collapsedSections.Contains(section));
            collapsedSections.Remove(section);
            if (collapsedSections.Count == 0)
            {
                collapsedSections = null;
            }
        }

        #endregion
    }
}