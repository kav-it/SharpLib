using System;
using System.Diagnostics;
using System.Windows.Threading;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    internal class AutoHideWindowManager
    {
        #region Поля

        private readonly DockingManager _manager;

        private DispatcherTimer _closeTimer;

        private WeakReference _currentAutohiddenAnchor;

        #endregion

        #region Конструктор

        internal AutoHideWindowManager(DockingManager manager)
        {
            _manager = manager;
            SetupCloseTimer();
        }

        #endregion

        #region Методы

        public void ShowAutoHideWindow(LayoutAnchorControl anchor)
        {
            if (_currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>() != anchor)
            {
                StopCloseTimer();
                _currentAutohiddenAnchor = new WeakReference(anchor);
                _manager.AutoHideWindow.Show(anchor);
                StartCloseTimer();
            }
        }

        public void HideAutoWindow(LayoutAnchorControl anchor = null)
        {
            if (anchor == null || anchor == _currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>())
            {
                StopCloseTimer();
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void SetupCloseTimer()
        {
            _closeTimer = new DispatcherTimer(DispatcherPriority.Background);
            _closeTimer.Interval = TimeSpan.FromMilliseconds(1500);
            _closeTimer.Tick += (s, e) =>
            {
                if (_manager.AutoHideWindow.IsWin32MouseOver ||
                    ((LayoutAnchorable)_manager.AutoHideWindow.Model).IsActive ||
                    _manager.AutoHideWindow.IsResizing)
                {
                    return;
                }

                StopCloseTimer();
            };
        }

        private void StartCloseTimer()
        {
            _closeTimer.Start();
        }

        private void StopCloseTimer()
        {
            _closeTimer.Stop();
            _manager.AutoHideWindow.Hide();
            _currentAutohiddenAnchor = null;
        }

        #endregion
    }
}