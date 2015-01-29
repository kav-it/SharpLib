using System.Windows;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    internal class TreeListExLinesRenderer : FrameworkElement
    {
        #region Поля

        private static readonly Pen _pen;

        #endregion

        #region Свойства

        private TreeListExNodeView ListNodeView
        {
            get { return TemplatedParent as TreeListExNodeView; }
        }

        #endregion

        #region Конструктор

        static TreeListExLinesRenderer()
        {
            _pen = new Pen(Brushes.LightGray, 1);
            _pen.Freeze();
        }

        #endregion

        #region Методы

        protected override void OnRender(DrawingContext dc)
        {
            var indent = ListNodeView.CalculateIndent();
            var p = new Point(indent + 4.5, 0);

            if (!ListNodeView.ListNode.IsRoot || ListNodeView.ParentTreeList.ShowRootExpander)
                dc.DrawLine(_pen, new Point(p.X, ActualHeight / 2), new Point(p.X + 10, ActualHeight / 2));

            if (ListNodeView.ListNode.IsRoot) return;

            dc.DrawLine(_pen, p, ListNodeView.ListNode.IsLast ? new Point(p.X, ActualHeight / 2) : new Point(p.X, ActualHeight));

            var current = ListNodeView.ListNode;
            while (true)
            {
                p.X -= 19;
                current = current.Parent;
                if (p.X < 0) break;
                if (!current.IsLast)
                    dc.DrawLine(_pen, p, new Point(p.X, ActualHeight));
            }
        }

        #endregion
    }
}