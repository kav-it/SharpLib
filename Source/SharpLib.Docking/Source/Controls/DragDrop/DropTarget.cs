using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    internal abstract class DropTarget<T> : DropTargetBase, IDropTarget where T : FrameworkElement
    {
        #region Поля

        private readonly Rect[] _detectionRect;

        private readonly T _targetElement;

        private readonly DropTargetType _type;

        #endregion

        #region Свойства

        public Rect[] DetectionRects
        {
            get { return _detectionRect; }
        }

        public T TargetElement
        {
            get { return _targetElement; }
        }

        public DropTargetType Type
        {
            get { return _type; }
        }

        #endregion

        #region Конструктор

        protected DropTarget(T targetElement, Rect detectionRect, DropTargetType type)
        {
            _targetElement = targetElement;
            _detectionRect = new[] { detectionRect };
            _type = type;
        }

        protected DropTarget(T targetElement, IEnumerable<Rect> detectionRects, DropTargetType type)
        {
            _targetElement = targetElement;
            _detectionRect = detectionRects.ToArray();
            _type = type;
        }

        #endregion

        #region Методы

        protected virtual void Drop(LayoutAnchorableFloatingWindow floatingWindow)
        {
        }

        protected virtual void Drop(LayoutDocumentFloatingWindow floatingWindow)
        {
        }

        public void Drop(LayoutFloatingWindow floatingWindow)
        {
            var root = floatingWindow.Root;
            var currentActiveContent = floatingWindow.Root.ActiveContent;
            var fwAsAnchorable = floatingWindow as LayoutAnchorableFloatingWindow;

            if (fwAsAnchorable != null)
            {
                Drop(fwAsAnchorable);
            }
            else
            {
                var fwAsDocument = floatingWindow as LayoutDocumentFloatingWindow;
                Drop(fwAsDocument);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                currentActiveContent.IsSelected = false;
                currentActiveContent.IsActive = false;
                currentActiveContent.IsActive = true;
            }), DispatcherPriority.Background);
        }

        public virtual bool HitTest(Point dragPoint)
        {
            return _detectionRect.Any(dr => dr.Contains(dragPoint));
        }

        public abstract Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

        public void DragEnter()
        {
            SetIsDraggingOver(TargetElement, true);
        }

        public void DragLeave()
        {
            SetIsDraggingOver(TargetElement, false);
        }

        #endregion
    }
}