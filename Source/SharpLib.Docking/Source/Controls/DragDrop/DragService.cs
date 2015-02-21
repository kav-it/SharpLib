using System.Collections.Generic;
using System.Linq;
using System.Windows;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    internal class DragService
    {
        #region Поля

        private readonly List<IDropArea> _currentWindowAreas;

        private readonly LayoutFloatingWindowControl _floatingWindow;

        private readonly DockingManager _manager;

        private readonly List<IOverlayWindowHost> _overlayWindowHosts;

        private IDropTarget _currentDropTarget;

        private IOverlayWindowHost _currentHost;

        private IOverlayWindow _currentWindow;

        #endregion

        #region Конструктор

        public DragService(LayoutFloatingWindowControl floatingWindow)
        {
            _currentWindowAreas = new List<IDropArea>();
            _overlayWindowHosts = new List<IOverlayWindowHost>();
            _floatingWindow = floatingWindow;
            _manager = floatingWindow.Model.Root.Manager;

            GetOverlayWindowHosts();
        }

        #endregion

        #region Методы

        private void GetOverlayWindowHosts()
        {
            _overlayWindowHosts.AddRange(_manager.GetFloatingWindowsByZOrder().OfType<LayoutAnchorableFloatingWindowControl>().Where(fw => fw != _floatingWindow && fw.IsVisible));
            _overlayWindowHosts.Add(_manager);
        }

        public void UpdateMouseLocation(Point dragPosition)
        {
            var floatingWindowModel = _floatingWindow.Model as LayoutFloatingWindow;

            var newHost = _overlayWindowHosts.FirstOrDefault(oh => oh.HitTest(dragPosition));

            if (_currentHost != null || _currentHost != newHost)
            {
                if ((_currentHost != null && !_currentHost.HitTest(dragPosition)) ||
                    _currentHost != newHost)
                {
                    if (_currentDropTarget != null)
                    {
                        _currentWindow.DragLeave(_currentDropTarget);
                    }
                    _currentDropTarget = null;

                    _currentWindowAreas.ForEach(a =>
                        _currentWindow.DragLeave(a));
                    _currentWindowAreas.Clear();

                    if (_currentWindow != null)
                    {
                        _currentWindow.DragLeave(_floatingWindow);
                    }
                    if (_currentHost != null)
                    {
                        _currentHost.HideOverlayWindow();
                    }
                    _currentHost = null;
                }

                if (_currentHost != newHost)
                {
                    _currentHost = newHost;
                    _currentWindow = _currentHost.ShowOverlayWindow(_floatingWindow);
                    _currentWindow.DragEnter(_floatingWindow);
                }
            }

            if (_currentHost == null)
            {
                return;
            }

            if (_currentDropTarget != null &&
                !_currentDropTarget.HitTest(dragPosition))
            {
                _currentWindow.DragLeave(_currentDropTarget);
                _currentDropTarget = null;
            }

            var areasToRemove = new List<IDropArea>();
            _currentWindowAreas.ForEach(a =>
            {
                if (!a.DetectionRect.Contains(dragPosition))
                {
                    _currentWindow.DragLeave(a);
                    areasToRemove.Add(a);
                }
            });

            areasToRemove.ForEach(a =>
                _currentWindowAreas.Remove(a));

            var areasToAdd =
                _currentHost.GetDropAreas(_floatingWindow).Where(cw => !_currentWindowAreas.Contains(cw) && cw.DetectionRect.Contains(dragPosition)).ToList();

            _currentWindowAreas.AddRange(areasToAdd);

            areasToAdd.ForEach(a =>
                _currentWindow.DragEnter(a));

            if (_currentDropTarget == null)
            {
                _currentWindowAreas.ForEach(wa =>
                {
                    if (_currentDropTarget != null)
                    {
                        return;
                    }

                    _currentDropTarget = _currentWindow.GetTargets().FirstOrDefault(dt => dt.HitTest(dragPosition));
                    if (_currentDropTarget != null)
                    {
                        _currentWindow.DragEnter(_currentDropTarget);
                    }
                });
            }
        }

        public void Drop(Point dropLocation, out bool dropHandled)
        {
            dropHandled = false;

            UpdateMouseLocation(dropLocation);

            var floatingWindowModel = _floatingWindow.Model as LayoutFloatingWindow;
            var root = floatingWindowModel.Root;

            if (_currentHost != null)
            {
                _currentHost.HideOverlayWindow();
            }

            if (_currentDropTarget != null)
            {
                _currentWindow.DragDrop(_currentDropTarget);
                root.CollectGarbage();
                dropHandled = true;
            }

            _currentWindowAreas.ForEach(a => _currentWindow.DragLeave(a));

            if (_currentDropTarget != null)
            {
                _currentWindow.DragLeave(_currentDropTarget);
            }
            if (_currentWindow != null)
            {
                _currentWindow.DragLeave(_floatingWindow);
            }
            _currentWindow = null;

            _currentHost = null;
        }

        internal void Abort()
        {
            var floatingWindowModel = _floatingWindow.Model as LayoutFloatingWindow;

            _currentWindowAreas.ForEach(a => _currentWindow.DragLeave(a));

            if (_currentDropTarget != null)
            {
                _currentWindow.DragLeave(_currentDropTarget);
            }
            if (_currentWindow != null)
            {
                _currentWindow.DragLeave(_floatingWindow);
            }
            _currentWindow = null;
            if (_currentHost != null)
            {
                _currentHost.HideOverlayWindow();
            }
            _currentHost = null;
        }

        #endregion
    }
}