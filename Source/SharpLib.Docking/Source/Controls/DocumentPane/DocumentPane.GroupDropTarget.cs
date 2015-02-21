using System.Linq;
using System.Windows;
using System.Windows.Media;

using SharpLib.Docking;

namespace SharpLib.Docking.Controls
{
    internal class DocumentPaneGroupDropTarget : DropTarget<LayoutDocumentPaneGroupControl>
    {
        #region Поля

        private readonly LayoutDocumentPaneGroupControl _targetPane;

        #endregion

        #region Конструктор

        internal DocumentPaneGroupDropTarget(LayoutDocumentPaneGroupControl paneControl, Rect detectionRect, DropTargetType type)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
        }

        #endregion

        #region Методы

        protected override void Drop(LayoutDocumentFloatingWindow floatingWindow)
        {
            var targetModel = _targetPane.Model as ILayoutPane;

            switch (Type)
            {
                case DropTargetType.DocumentPaneGroupDockInside:

                    #region DropTargetType.DocumentPaneGroupDockInside

                    {
                        var paneGroupModel = targetModel as LayoutDocumentPaneGroup;
                        var paneModel = paneGroupModel.Children[0] as LayoutDocumentPane;
                        var sourceModel = floatingWindow.RootDocument;

                        paneModel.Children.Insert(0, sourceModel);
                    }
                    break;

                    #endregion
            }
            base.Drop(floatingWindow);
        }

        protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
        {
            var targetModel = _targetPane.Model as ILayoutPane;

            switch (Type)
            {
                case DropTargetType.DocumentPaneGroupDockInside:

                    #region DropTargetType.DocumentPaneGroupDockInside

                    {
                        var paneGroupModel = targetModel as LayoutDocumentPaneGroup;
                        var paneModel = paneGroupModel.Children[0] as LayoutDocumentPane;
                        var layoutAnchorablePaneGroup = floatingWindow.RootPanel;

                        int i = 0;
                        foreach (var anchorableToImport in layoutAnchorablePaneGroup.Descendents().OfType<LayoutAnchorable>().ToArray())
                        {
                            paneModel.Children.Insert(i, anchorableToImport);
                            i++;
                        }
                    }
                    break;

                    #endregion
            }

            base.Drop(floatingWindow);
        }

        public override System.Windows.Media.Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
        {
            switch (Type)
            {
                case DropTargetType.DocumentPaneGroupDockInside:

                    #region DropTargetType.DocumentPaneGroupDockInside

                    {
                        var targetScreenRect = TargetElement.GetScreenArea();
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                        return new RectangleGeometry(targetScreenRect);
                    }

                    #endregion
            }

            return null;
        }

        #endregion
    }
}