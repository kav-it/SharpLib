using System;

namespace SharpLib.Docking.Layout
{
    public class LayoutElementEventArgs : EventArgs
    {
        #region Свойства

        public LayoutElement Element { get; private set; }

        #endregion

        #region Конструктор

        public LayoutElementEventArgs(LayoutElement element)
        {
            Element = element;
        }

        #endregion
    }
}