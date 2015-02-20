﻿namespace SharpLib.Docking.Layout
{
    public interface ILayoutContentSelector
    {
        #region Свойства

        int SelectedContentIndex { get; set; }

        LayoutContent SelectedContent { get; }

        #endregion

        #region Методы

        int IndexOf(LayoutContent content);

        #endregion
    }
}