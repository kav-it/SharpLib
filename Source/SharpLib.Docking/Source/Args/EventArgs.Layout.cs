using System;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking
{
    internal class LayoutEventArgs : EventArgs
    {
        #region Свойства

        public LayoutRoot LayoutRoot { get; private set; }

        #endregion

        #region Конструктор

        public LayoutEventArgs(LayoutRoot layoutRoot)
        {
            LayoutRoot = layoutRoot;
        }

        #endregion
    }
}