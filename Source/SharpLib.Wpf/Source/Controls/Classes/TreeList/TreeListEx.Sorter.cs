using System;
using System.Collections.Generic;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExSorter : IComparer<TreeListExNode>
    {
        #region Поля

        private static readonly Lazy<TreeListExSorter> _default = new Lazy<TreeListExSorter>();

        #endregion

        #region Свойства

        public static TreeListExSorter Default
        {
            get { return _default.Value; }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод сравнения (будет переопределен в наследниказ)
        /// </summary>
        protected virtual int OnCompare(TreeListExNode x, TreeListExNode y)
        {
            return string.CompareOrdinal(x.Text.ToString(), y.Text.ToString());
        }

        /// <summary>
        /// Сравнение (реализация интерфейса)
        /// </summary>
        public int Compare(TreeListExNode x, TreeListExNode y)
        {
            return OnCompare(x, y);
        }

        #endregion
    }
}