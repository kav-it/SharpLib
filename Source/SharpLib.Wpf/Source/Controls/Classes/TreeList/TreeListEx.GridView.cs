using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExGridView : GridView
    {
        #region Свойства

        public static ResourceKey ItemContainerStyleKey { get; private set; }

        protected override object ItemContainerDefaultStyleKey
        {
            get { return ItemContainerStyleKey; }
        }

        #endregion

        #region Конструктор

        static TreeListExGridView()
        {
            ItemContainerStyleKey = new ComponentResourceKey(typeof(TreeListEx), "GridViewItemContainerStyleKey");
        }

        #endregion
    }
}