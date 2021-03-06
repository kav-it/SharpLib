﻿using System;

namespace SharpLib.Docking
{
    public class ChildrenTreeChangedEventArgs : EventArgs
    {
        #region Свойства

        public ChildrenTreeChange Change { get; private set; }

        #endregion

        #region Конструктор

        public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
        {
            Change = change;
        }

        #endregion
    }
}