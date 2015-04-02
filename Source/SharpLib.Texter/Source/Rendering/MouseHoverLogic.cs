using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SharpLib.Texter.Rendering
{
    public class MouseHoverLogic : IDisposable
    {
        #region Поля

        private readonly UIElement target;

        private bool disposed;

        private MouseEventArgs mouseHoverLastEventArgs;

        private Point mouseHoverStartPoint;

        private DispatcherTimer mouseHoverTimer;

        private bool mouseHovering;

        #endregion

        #region События

        public event EventHandler<MouseEventArgs> MouseHover;

        public event EventHandler<MouseEventArgs> MouseHoverStopped;

        #endregion

        #region Конструктор

        public MouseHoverLogic(UIElement target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            this.target = target;
            this.target.MouseLeave += MouseHoverLogicMouseLeave;
            this.target.MouseMove += MouseHoverLogicMouseMove;
            this.target.MouseEnter += MouseHoverLogicMouseEnter;
        }

        #endregion

        #region Методы

        private void MouseHoverLogicMouseMove(object sender, MouseEventArgs e)
        {
            var mouseMovement = mouseHoverStartPoint - e.GetPosition(target);
            if (Math.Abs(mouseMovement.X) > SystemParameters.MouseHoverWidth
                || Math.Abs(mouseMovement.Y) > SystemParameters.MouseHoverHeight)
            {
                StartHovering(e);
            }
        }

        private void MouseHoverLogicMouseEnter(object sender, MouseEventArgs e)
        {
            StartHovering(e);
        }

        private void StartHovering(MouseEventArgs e)
        {
            StopHovering();
            mouseHoverStartPoint = e.GetPosition(target);
            mouseHoverLastEventArgs = e;
            mouseHoverTimer = new DispatcherTimer(SystemParameters.MouseHoverTime, DispatcherPriority.Background, OnMouseHoverTimerElapsed, target.Dispatcher);
            mouseHoverTimer.Start();
        }

        private void MouseHoverLogicMouseLeave(object sender, MouseEventArgs e)
        {
            StopHovering();
        }

        private void StopHovering()
        {
            if (mouseHoverTimer != null)
            {
                mouseHoverTimer.Stop();
                mouseHoverTimer = null;
            }
            if (mouseHovering)
            {
                mouseHovering = false;
                OnMouseHoverStopped(mouseHoverLastEventArgs);
            }
        }

        private void OnMouseHoverTimerElapsed(object sender, EventArgs e)
        {
            mouseHoverTimer.Stop();
            mouseHoverTimer = null;

            mouseHovering = true;
            OnMouseHover(mouseHoverLastEventArgs);
        }

        protected virtual void OnMouseHover(MouseEventArgs e)
        {
            if (MouseHover != null)
            {
                MouseHover(this, e);
            }
        }

        protected virtual void OnMouseHoverStopped(MouseEventArgs e)
        {
            if (MouseHoverStopped != null)
            {
                MouseHoverStopped(this, e);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                target.MouseLeave -= MouseHoverLogicMouseLeave;
                target.MouseMove -= MouseHoverLogicMouseMove;
                target.MouseEnter -= MouseHoverLogicMouseEnter;
            }
            disposed = true;
        }

        #endregion
    }
}