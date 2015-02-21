﻿namespace SharpLib.Docking
{
    public interface ILayoutUpdateStrategy
    {
        #region Методы

        bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer);

        void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown);

        bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer);

        void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown);

        #endregion
    }
}