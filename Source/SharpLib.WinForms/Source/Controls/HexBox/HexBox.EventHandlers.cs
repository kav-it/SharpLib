using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using SharpLib.Native.Windows;

namespace SharpLib.WinForms.Controls
{
    public partial class HexBox
    {
        #region Методы

        /// <summary>
        /// Preprocesses windows messages.
        /// </summary>
        /// <param name="m">the message to process.</param>
        /// <returns>true, if the message was processed</returns>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true), SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        public override bool PreProcessMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_KEYDOWN:
                    return _keyInterpreter.PreProcessWmKeyDown(ref m);
                case NativeMethods.WM_CHAR:
                    return _keyInterpreter.PreProcessWmChar(ref m);
                case NativeMethods.WM_KEYUP:
                    return _keyInterpreter.PreProcessWmKeyUp(ref m);
                default:
                    return base.PreProcessMessage(ref m);
            }
        }

        private bool BasePreProcessMessage(ref Message m)
        {
            return base.PreProcessMessage(ref m);
        }

        /// <summary>
        /// Paints the background.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    {
                        if (TextBoxRenderer.IsSupported)
                        {
                            VisualStyleElement state = VisualStyleElement.TextBox.TextEdit.Normal;
                            Color backColor = BackColor;

                            if (Enabled)
                            {
                                if (ReadOnly)
                                {
                                    state = VisualStyleElement.TextBox.TextEdit.ReadOnly;
                                }
                                else if (Focused)
                                {
                                    state = VisualStyleElement.TextBox.TextEdit.Focused;
                                }
                            }
                            else
                            {
                                state = VisualStyleElement.TextBox.TextEdit.Disabled;
                                backColor = BackColorDisabled;
                            }

                            VisualStyleRenderer vsr = new VisualStyleRenderer(state);
                            vsr.DrawBackground(e.Graphics, ClientRectangle);

                            Rectangle rectContent = vsr.GetBackgroundContentRectangle(e.Graphics, ClientRectangle);
                            e.Graphics.FillRectangle(new SolidBrush(backColor), rectContent);
                        }
                        else
                        {
                            // draw background
                            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

                            // draw default border
                            ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken);
                        }

                        break;
                    }
                case BorderStyle.FixedSingle:
                    {
                        // draw background
                        e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

                        // draw fixed single border
                        ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
                        break;
                    }
                default:
                    {
                        // draw background
                        e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                        break;
                    }
            }
        }

        private void ScrollBarOnScroll(object sender, ScrollEventArgs e)
        {
            switch (e.Type)
            {
                case ScrollEventType.Last:
                    break;
                case ScrollEventType.EndScroll:
                    break;
                case ScrollEventType.SmallIncrement:
                    PerformScrollLineDown();
                    break;
                case ScrollEventType.SmallDecrement:
                    PerformScrollLineUp();
                    break;
                case ScrollEventType.LargeIncrement:
                    PerformScrollPageDown();
                    break;
                case ScrollEventType.LargeDecrement:
                    PerformScrollPageUp();
                    break;
                case ScrollEventType.ThumbPosition:
                    long lPos = FromScrollPos(e.NewValue);
                    PerformScrollThumpPosition(lPos);
                    break;
                case ScrollEventType.ThumbTrack:
                    // to avoid performance problems use a refresh delay implemented with a timer
                    if (_thumbTrackTimer.Enabled) // stop old timer
                    {
                        _thumbTrackTimer.Enabled = false;
                    }

                    // perform scroll immediately only if last refresh is very old
                    int currentThumbTrack = Environment.TickCount;
                    if (currentThumbTrack - _lastThumbtrack > THUMB_TRACK_DELAY)
                    {
                        ThumbTrackTimerOnTick(null, null);
                        _lastThumbtrack = currentThumbTrack;
                        break;
                    }

                    // start thumbtrack timer 
                    _thumbTrackPosition = FromScrollPos(e.NewValue);
                    _thumbTrackTimer.Enabled = true;
                    break;
                case ScrollEventType.First:
                    break;
            }

            e.NewValue = ToScrollPos(_scrollVpos);
        }

        /// <summary>
        /// Performs the thumbtrack scrolling after an delay.
        /// </summary>
        private void ThumbTrackTimerOnTick(object sender, EventArgs e)
        {
            _thumbTrackTimer.Enabled = false;
            PerformScrollThumpPosition(_thumbTrackPosition);
            _lastThumbtrack = Environment.TickCount;
        }

        /// <summary>
        /// Raises the InsertActiveChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnInsertActiveChanged(EventArgs e)
        {
            if (InsertActiveChanged != null)
            {
                InsertActiveChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the ReadOnlyChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnReadOnlyChanged(EventArgs e)
        {
            if (ReadOnlyChanged != null)
            {
                ReadOnlyChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the ByteProviderChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnByteProviderChanged(EventArgs e)
        {
            if (ByteProviderChanged != null)
            {
                ByteProviderChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the SelectionStartChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnSelectionStartChanged(EventArgs e)
        {
            if (SelectionStartChanged != null)
            {
                SelectionStartChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the SelectionLengthChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnSelectionLengthChanged(EventArgs e)
        {
            if (SelectionLengthChanged != null)
            {
                SelectionLengthChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the LineInfoVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnLineInfoVisibleChanged(EventArgs e)
        {
            if (LineInfoVisibleChanged != null)
            {
                LineInfoVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the OnColumnInfoVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnColumnInfoVisibleChanged(EventArgs e)
        {
            if (ColumnInfoVisibleChanged != null)
            {
                ColumnInfoVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the ColumnSeparatorVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnGroupSeparatorVisibleChanged(EventArgs e)
        {
            if (GroupSeparatorVisibleChanged != null)
            {
                GroupSeparatorVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the StringViewVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnStringViewVisibleChanged(EventArgs e)
        {
            if (StringViewVisibleChanged != null)
            {
                StringViewVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the BorderStyleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            if (BorderStyleChanged != null)
            {
                BorderStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the UseFixedBytesPerLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnUseFixedBytesPerLineChanged(EventArgs e)
        {
            if (UseFixedBytesPerLineChanged != null)
            {
                UseFixedBytesPerLineChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the GroupSizeChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnGroupSizeChanged(EventArgs e)
        {
            if (GroupSizeChanged != null)
            {
                GroupSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the BytesPerLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnBytesPerLineChanged(EventArgs e)
        {
            if (BytesPerLineChanged != null)
            {
                BytesPerLineChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the VScrollBarVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnVScrollBarVisibleChanged(EventArgs e)
        {
            if (VScrollBarVisibleChanged != null)
            {
                VScrollBarVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the HexCasingChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnHexCasingChanged(EventArgs e)
        {
            if (HexCasingChanged != null)
            {
                HexCasingChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the HorizontalByteCountChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnHorizontalByteCountChanged(EventArgs e)
        {
            if (HorizontalByteCountChanged != null)
            {
                HorizontalByteCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the VerticalByteCountChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnVerticalByteCountChanged(EventArgs e)
        {
            if (VerticalByteCountChanged != null)
            {
                VerticalByteCountChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the CurrentLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCurrentLineChanged(EventArgs e)
        {
            if (CurrentLineChanged != null)
            {
                CurrentLineChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the CurrentPositionInLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCurrentPositionInLineChanged(EventArgs e)
        {
            if (CurrentPositionInLineChanged != null)
            {
                CurrentPositionInLineChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the Copied event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCopied(EventArgs e)
        {
            if (Copied != null)
            {
                Copied(this, e);
            }
        }

        /// <summary>
        /// Raises the CopiedHex event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCopiedHex(EventArgs e)
        {
            if (CopiedHex != null)
            {
                CopiedHex(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused)
            {
                Focus();
            }

            if (e.Button == MouseButtons.Left)
            {
                SetCaretPosition(new Point(e.X, e.Y));
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the MouseWhell event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int linesToScroll = -(e.Delta * SystemInformation.MouseWheelScrollLines / 120);
            PerformScrollLines(linesToScroll);

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRectanglePositioning();
        }

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            CreateCaret();
        }

        /// <summary>
        /// Raises the LostFocus event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            DestroyCaret();
        }

        private void CheckCurrentLineChanged()
        {
            long currentLine = (long)Math.Floor(_bytePos / (double)_iHexMaxHBytes) + 1;

            if (_dataSource == null && _currentLine != 0)
            {
                _currentLine = 0;
                OnCurrentLineChanged(EventArgs.Empty);
            }
            else if (currentLine != _currentLine)
            {
                _currentLine = currentLine;
                OnCurrentLineChanged(EventArgs.Empty);
            }
        }

        private void CheckCurrentPositionInLineChanged()
        {
            Point gb = GetGridBytePoint(_bytePos);
            int currentPositionInLine = gb.X + 1;

            if (_dataSource == null && _currentPositionInLine != 0)
            {
                _currentPositionInLine = 0;
                OnCurrentPositionInLineChanged(EventArgs.Empty);
            }
            else if (currentPositionInLine != _currentPositionInLine)
            {
                _currentPositionInLine = currentPositionInLine;
                OnCurrentPositionInLineChanged(EventArgs.Empty);
            }
        }

        private void DataSourceLengthChanged(object sender, EventArgs e)
        {
            UpdateScrollSize();
        }

        #endregion
    }
}