using System;
using System.Collections.Generic;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExSorter : IComparer<TreeListExNode>
    {
        #region ����

        private static readonly Lazy<TreeListExSorter> _default = new Lazy<TreeListExSorter>();

        #endregion

        #region ��������

        public static TreeListExSorter Default
        {
            get { return _default.Value; }
        }

        #endregion

        #region ������

        /// <summary>
        /// ����� ��������� (����� ������������� � �����������)
        /// </summary>
        protected virtual int OnCompare(TreeListExNode x, TreeListExNode y)
        {
            return string.CompareOrdinal(x.Text.ToString(), y.Text.ToString());
        }

        /// <summary>
        /// ��������� (���������� ����������)
        /// </summary>
        public int Compare(TreeListExNode x, TreeListExNode y)
        {
            return OnCompare(x, y);
        }

        #endregion
    }
}