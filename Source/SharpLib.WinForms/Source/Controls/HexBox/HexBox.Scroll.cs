using System;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    public partial class HexBox
    {
        #region Поля

        /// <summary>
        /// Вертикальный скролл
        /// </summary>
        private readonly VScrollBar _vScrollBar;

        /// <summary>
        /// Максимальное значение скроллинга (ед.изм - линии)
        /// </summary>
        private long _scrollVmax;

        /// <summary>
        /// Минимальное значение скроллинга (ед.изм - линии)
        /// </summary>
        private long _scrollVmin;

        /// <summary>
        /// Текущая позиция скролла
        /// </summary>
        private long _scrollVpos;

        /// <summary>
        /// Признак видимости ScrollBar
        /// </summary>
        private bool _vScrollBarVisible;

        #endregion

        #region Методы

        private void UpdateScrollSize()
        {
            // calc scroll bar info
            if (VScrollBarVisible && _dataSource != null && _dataSource.Length > 0 && _iHexMaxHBytes != 0)
            {
                long scrollmax = (long)Math.Ceiling((_dataSource.Length + 1) / (double)_iHexMaxHBytes - _iHexMaxVBytes);
                scrollmax = Math.Max(0, scrollmax);

                long scrollpos = _startByte / _iHexMaxHBytes;

                if (scrollmax < _scrollVmax)
                {
                    // Data size has been decreased
                    if (_scrollVpos == _scrollVmax)
                    {
                        // Scroll one line up if we at bottom
                        PerformScrollLineUp();
                    }
                }

                if (scrollmax == _scrollVmax && scrollpos == _scrollVpos)
                {
                    return;
                }

                _scrollVmin = 0;
                _scrollVmax = scrollmax;
                _scrollVpos = Math.Min(scrollpos, scrollmax);
                UpdateVScroll();
            }
            else if (VScrollBarVisible)
            {
                // disable scroll bar
                _scrollVmin = 0;
                _scrollVmax = 0;
                _scrollVpos = 0;
                UpdateVScroll();
            }
        }

        private void UpdateVScroll()
        {
            int max = ToScrollMax(_scrollVmax);

            if (max > 0)
            {
                _vScrollBar.Minimum = 0;
                _vScrollBar.Maximum = max;
                _vScrollBar.Value = ToScrollPos(_scrollVpos);
                _vScrollBar.Visible = true;
            }
            else
            {
                _vScrollBar.Visible = false;
            }
        }

        private int ToScrollPos(long value)
        {
            int max = 65535;

            if (_scrollVmax < max)
            {
                return (int)value;
            }
            double valperc = value / (double)_scrollVmax * 100;
            int res = (int)Math.Floor(max / (double)100 * valperc);
            res = (int)Math.Max(_scrollVmin, res);
            res = (int)Math.Min(_scrollVmax, res);
            return res;
        }

        private long FromScrollPos(int value)
        {
            int max = 65535;
            if (_scrollVmax < max)
            {
                return value;
            }
            double valperc = value / (double)max * 100;
            long res = (int)Math.Floor(_scrollVmax / (double)100 * valperc);
            return res;
        }

        private int ToScrollMax(long value)
        {
            long max = 65535;
            if (value > max)
            {
                return (int)max;
            }
            return (int)value;
        }

        /// <summary>
        /// Скрол к указанной строке
        /// </summary>
        private void PerformScrollToLine(long pos)
        {
            if (pos < _scrollVmin || pos > _scrollVmax || pos == _scrollVpos)
            {
                return;
            }

            _scrollVpos = pos;

            UpdateVScroll();
            UpdateVisibilityBytes();
            UpdateCaret();
            Invalidate();
        }

        /// <summary>
        /// Скрол к указанной строке
        /// </summary>
        private void PerformScrollLines(int lines)
        {
            long pos;
            if (lines > 0)
            {
                pos = Math.Min(_scrollVmax, _scrollVpos + lines);
            }
            else if (lines < 0)
            {
                pos = Math.Max(_scrollVmin, _scrollVpos + lines);
            }
            else
            {
                return;
            }

            PerformScrollToLine(pos);
        }

        /// <summary>
        /// Скролл на одну строку вниз
        /// </summary>
        private void PerformScrollLineDown()
        {
            PerformScrollLines(1);
        }

        /// <summary>
        /// Скролл на одну строку вверх
        /// </summary>
        private void PerformScrollLineUp()
        {
            PerformScrollLines(-1);
        }

        private void PerformScrollPageDown()
        {
            PerformScrollLines(_iHexMaxVBytes);
        }

        private void PerformScrollPageUp()
        {
            PerformScrollLines(-_iHexMaxVBytes);
        }

        private void PerformScrollThumpPosition(long pos)
        {
            // Bug fix: Scroll to end, do not scroll to end
            int difference = (_scrollVmax > 65535) ? 10 : 9;

            if (ToScrollPos(pos) == ToScrollMax(_scrollVmax) - difference)
            {
                pos = _scrollVmax;
            }
            // End Bug fix

            PerformScrollToLine(pos);
        }

        /// <summary>
        /// Scrolls the selection start byte into view
        /// </summary>
        public void ScrollByteIntoView()
        {
            ScrollByteIntoView(_bytePos);
        }

        /// <summary>
        /// Scrolls the specific byte into view
        /// </summary>
        /// <param name="index">the index of the byte</param>
        public void ScrollByteIntoView(long index)
        {
            if (_dataSource == null || _keyProcessor == null)
            {
                return;
            }

            if (index < _startByte)
            {
                long line = (long)Math.Floor(index / (double)_iHexMaxHBytes);
                PerformScrollThumpPosition(line);
            }
            else if (index > _endByte)
            {
                long line = (long)Math.Floor(index / (double)_iHexMaxHBytes);
                line -= _iHexMaxVBytes - 1;
                PerformScrollThumpPosition(line);
            }
        }

        #endregion
    }
}