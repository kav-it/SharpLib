using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Реализация Paint элемента
    /// </summary>
    public partial class HexBox
    {
        /// <summary>
        /// Размер адреса (8 байт)
        /// </summary>
        private const int ADDR_SIZE = 8;

        /// <summary>
        /// Размер отступа между колонкой адреса и hex-блоком (в пикселях)
        /// </summary>
        private const int SIZE_BETWEEN_ADDR_AND_HEX = 20;

        #region Поля

        /// <summary>
        /// Регион вывода всего текста (не включаются Scrollbar-ы)
        /// </summary>
        private Rectangle _recContent;

        /// <summary>
        /// Регион вывода Hex-данных
        /// </summary>
        private Rectangle _recHex;

        /// <summary>
        /// Регион вывода колонки адреса
        /// </summary>
        private Rectangle _recColumnAddr;

        /// <summary>
        /// Регион заголовка в котором отображаются смещения (0 1 2 3 и т.д.)
        /// </summary>
        private Rectangle _recHeader;

        /// <summary>
        /// Регион вывода Ascii
        /// </summary>
        private Rectangle _recAscii;

        #endregion

        #region Методы

        /// <summary>
        /// Реализация Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_dataSource == null)
            {
                // Нет источника данных
                return;
            }

            // Рисуется в прямоугольнике, исключающем border и scrollbar
            using (var r = new Region(ClientRectangle))
            {
                r.Exclude(_recContent);
                e.Graphics.ExcludeClip(r);
            }

            // Расчет диапазона видимости
            UpdateVisibilityBytes();

            if (_columnAddrVisible)
            {
                // Рисование блока адреса
                PaintAddr(e.Graphics, _startByte, _endByte);
            }

            if (!_columnAsciiVisible)
            {
                // Отображение только Hex
                PaintHex(e.Graphics, _startByte, _endByte);
            }
            else
            {
                // Отображение Hex и Ascii
                PaintHexAndAsciiView(e.Graphics, _startByte, _endByte);
                if (_shadowSelectionVisible)
                {
                    PaintCurrentBytesSign(e.Graphics);
                }
            }

            if (_headerOffsetVisible)
            {
                // Рисование блока Header (отображение смещений)
                PaintHeaderRow(e.Graphics);
            }

            if (_groupSeparatorVisible)
            {
                // Отображения разделителей
                PaintGroupSeparator(e.Graphics);
            }
        }

        /// <summary>
        /// Рисование блока адреса
        /// </summary>
        private void PaintAddr(Graphics g, long startByte, long endByte)
        {
            // Начальные проверки
            endByte = Math.Min(_dataSource.Length - 1, endByte);

            var lineInfoColor = (InfoForeColor != Color.Empty) ? InfoForeColor : ForeColor;
            var brush = new SolidBrush(lineInfoColor);

            // Расчет региона отображения (в строках и колонках)
            var region = GetGridBytePoint(endByte - startByte);
            int maxLine = region.Y + 1;

            for (int i = 0; i < maxLine; i++)
            {
                long firstLineByte = (startByte + (_iHexMaxHBytes) * i) + _lineInfoOffset;

                var bytePointF = GetBytePointF(new Point(0, 0 + i));
                string info = firstLineByte.ToString(_hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);

                int padding = ADDR_SIZE - info.Length;
                var lineText = padding > -1 
                    ? new string('0', 8 - info.Length) + info 
                    : new string('~', 8);

                g.DrawString(lineText, Font, brush, new PointF(_recColumnAddr.X, bytePointF.Y), _stringFormat);
            }
        }

        /// <summary>
        /// Рисование блока Hex
        /// </summary>
        private void PaintHex(Graphics g, long startByte, long endByte)
        {
            var brush = new SolidBrush(GetDefaultForeColor());
            var selBrush = new SolidBrush(_selectionForeColor);
            var selBrushBack = new SolidBrush(_selectionBackColor);

            int counter = -1;
            long internEndByte = Math.Min(_dataSource.Length - 1, endByte + _iHexMaxHBytes);

            bool isKeyInterpreterActive = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(KeyInterpreter);

            for (long i = startByte; i < internEndByte + 1; i++)
            {
                counter++;
                Point gridPoint = GetGridBytePoint(counter);
                byte b = _dataSource.ReadByte(i);

                bool isSelectedByte = i >= _bytePos && i <= (_bytePos + _selectionLength - 1) && _selectionLength != 0;

                if (isSelectedByte && isKeyInterpreterActive)
                {
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint);
                }
                else
                {
                    PaintHexString(g, b, brush, gridPoint);
                }
            }
        }

        private void PaintHeaderRow(Graphics g)
        {
            Brush brush = new SolidBrush(InfoForeColor);
            for (int col = 0; col < _iHexMaxHBytes; col++)
            {
                PaintHeader(g, (byte)col, brush, col);
            }
        }

       
        /// <summary>
        /// Рисование строки Hex
        /// </summary>
        private void PaintHexString(Graphics g, byte b, Brush brush, Point gridPoint)
        {
            var bytePointF = GetBytePointF(gridPoint);
            var text = HexBoxUtils.ConvertByteToHex(b);

            g.DrawString(text.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
            bytePointF.X += _charSize.Width;
            g.DrawString(text.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
        }

        private void PaintHeader(Graphics g, byte b, Brush brush, int col)
        {
            PointF headerPointF = GetHeaderPointF(col);
            var text = HexBoxUtils.ConvertByteToHex(b);

            g.DrawString(text.Substring(0, 1), Font, brush, headerPointF, _stringFormat);
            headerPointF.X += _charSize.Width;
            g.DrawString(text.Substring(1, 1), Font, brush, headerPointF, _stringFormat);
        }

        private void PaintHexStringSelected(Graphics g, byte b, Brush brush, Brush brushBack, Point gridPoint)
        {
            string sB = b.ToString(_hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (sB.Length == 1)
            {
                sB = "0" + sB;
            }

            PointF bytePointF = GetBytePointF(gridPoint);

            bool isLastLineChar = (gridPoint.X + 1 == _iHexMaxHBytes);
            float bcWidth = (isLastLineChar) ? _charSize.Width * 2 : _charSize.Width * 3;

            g.FillRectangle(brushBack, bytePointF.X, bytePointF.Y, bcWidth, _charSize.Height);
            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
            bytePointF.X += _charSize.Width;
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
        }

        private void PaintHexAndAsciiView(Graphics g, long startByte, long endByte)
        {
            Brush brush = new SolidBrush(GetDefaultForeColor());
            Brush selBrush = new SolidBrush(_selectionForeColor);
            Brush selBrushBack = new SolidBrush(_selectionBackColor);

            int counter = -1;
            long internEndByte = Math.Min(_dataSource.Length - 1, endByte + _iHexMaxHBytes);

            bool isKeyInterpreterActive = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(KeyInterpreter);
            bool isStringKeyInterpreterActive = _keyInterpreter != null && _keyInterpreter.GetType() == typeof(StringKeyInterpreter);

            for (long i = startByte; i < internEndByte + 1; i++)
            {
                counter++;
                Point gridPoint = GetGridBytePoint(counter);
                PointF byteStringPointF = GetAsciiPointF(gridPoint);
                byte b = _dataSource.ReadByte(i);

                bool isSelectedByte = i >= _bytePos && i <= (_bytePos + _selectionLength - 1) && _selectionLength != 0;

                if (isSelectedByte && isKeyInterpreterActive)
                {
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint);
                }
                else
                {
                    PaintHexString(g, b, brush, gridPoint);
                }

                string s = new String(ByteCharConverter.ToChar(b), 1);

                if (isSelectedByte && isStringKeyInterpreterActive)
                {
                    g.FillRectangle(selBrushBack, byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height);
                    g.DrawString(s, Font, selBrush, byteStringPointF, _stringFormat);
                }
                else
                {
                    g.DrawString(s, Font, brush, byteStringPointF, _stringFormat);
                }
            }
        }

        private void PaintCurrentBytesSign(Graphics g)
        {
            if (_keyInterpreter != null && _bytePos != -1 && Enabled)
            {
                if (_keyInterpreter.GetType() == typeof(KeyInterpreter))
                {
                    if (_selectionLength == 0)
                    {
                        Point gp = GetGridBytePoint(_bytePos - _startByte);
                        PointF pf = GetAsciiPointF(gp);
                        Size s = new Size((int)_charSize.Width, (int)_charSize.Height);
                        Rectangle r = new Rectangle((int)pf.X, (int)pf.Y, s.Width, s.Height);
                        if (r.IntersectsWith(_recAscii))
                        {
                            r.Intersect(_recAscii);
                            PaintCurrentByteSign(g, r);
                        }
                    }
                    else
                    {
                        int lineWidth = (int)(_recAscii.Width - _charSize.Width);

                        Point startSelGridPoint = GetGridBytePoint(_bytePos - _startByte);
                        PointF startSelPointF = GetAsciiPointF(startSelGridPoint);

                        Point endSelGridPoint = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1);
                        PointF endSelPointF = GetAsciiPointF(endSelGridPoint);

                        int multiLine = endSelGridPoint.Y - startSelGridPoint.Y;
                        if (multiLine == 0)
                        {
                            Rectangle singleLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(endSelPointF.X - startSelPointF.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (singleLine.IntersectsWith(_recAscii))
                            {
                                singleLine.Intersect(_recAscii);
                                PaintCurrentByteSign(g, singleLine);
                            }
                        }
                        else
                        {
                            Rectangle firstLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(_recAscii.X + lineWidth - startSelPointF.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (firstLine.IntersectsWith(_recAscii))
                            {
                                firstLine.Intersect(_recAscii);
                                PaintCurrentByteSign(g, firstLine);
                            }

                            if (multiLine > 1)
                            {
                                Rectangle betweenLines = new Rectangle(
                                    _recAscii.X,
                                    (int)(startSelPointF.Y + _charSize.Height),
                                    _recAscii.Width,
                                    (int)(_charSize.Height * (multiLine - 1)));
                                if (betweenLines.IntersectsWith(_recAscii))
                                {
                                    betweenLines.Intersect(_recAscii);
                                    PaintCurrentByteSign(g, betweenLines);
                                }
                            }

                            Rectangle lastLine = new Rectangle(
                                _recAscii.X,
                                (int)endSelPointF.Y,
                                (int)(endSelPointF.X - _recAscii.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (lastLine.IntersectsWith(_recAscii))
                            {
                                lastLine.Intersect(_recAscii);
                                PaintCurrentByteSign(g, lastLine);
                            }
                        }
                    }
                }
                else
                {
                    if (_selectionLength == 0)
                    {
                        Point gp = GetGridBytePoint(_bytePos - _startByte);
                        PointF pf = GetBytePointF(gp);
                        Size s = new Size((int)_charSize.Width * 2, (int)_charSize.Height);
                        Rectangle r = new Rectangle((int)pf.X, (int)pf.Y, s.Width, s.Height);
                        PaintCurrentByteSign(g, r);
                    }
                    else
                    {
                        int lineWidth = (int)(_recHex.Width - _charSize.Width * 5);

                        Point startSelGridPoint = GetGridBytePoint(_bytePos - _startByte);
                        PointF startSelPointF = GetBytePointF(startSelGridPoint);

                        Point endSelGridPoint = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1);
                        PointF endSelPointF = GetBytePointF(endSelGridPoint);

                        int multiLine = endSelGridPoint.Y - startSelGridPoint.Y;
                        if (multiLine == 0)
                        {
                            Rectangle singleLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(endSelPointF.X - startSelPointF.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (singleLine.IntersectsWith(_recHex))
                            {
                                singleLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, singleLine);
                            }
                        }
                        else
                        {
                            Rectangle firstLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(_recHex.X + lineWidth - startSelPointF.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (firstLine.IntersectsWith(_recHex))
                            {
                                firstLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, firstLine);
                            }

                            if (multiLine > 1)
                            {
                                Rectangle betweenLines = new Rectangle(
                                    _recHex.X,
                                    (int)(startSelPointF.Y + _charSize.Height),
                                    (int)(lineWidth + _charSize.Width * 2),
                                    (int)(_charSize.Height * (multiLine - 1)));
                                if (betweenLines.IntersectsWith(_recHex))
                                {
                                    betweenLines.Intersect(_recHex);
                                    PaintCurrentByteSign(g, betweenLines);
                                }
                            }

                            Rectangle lastLine = new Rectangle(
                                _recHex.X,
                                (int)endSelPointF.Y,
                                (int)(endSelPointF.X - _recHex.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (lastLine.IntersectsWith(_recHex))
                            {
                                lastLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, lastLine);
                            }
                        }
                    }
                }
            }
        }

        private void PaintCurrentByteSign(Graphics g, Rectangle rec)
        {
            // stack overflowexception on big files - workaround
            if (rec.Top < 0 || rec.Left < 0 || rec.Width <= 0 || rec.Height <= 0)
            {
                return;
            }

            Bitmap myBitmap = new Bitmap(rec.Width, rec.Height);
            Graphics bitmapGraphics = Graphics.FromImage(myBitmap);

            SolidBrush greenBrush = new SolidBrush(_shadowSelectionColor);

            bitmapGraphics.FillRectangle(greenBrush, 0,
                0, rec.Width, rec.Height);

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;

            g.DrawImage(myBitmap, rec.Left, rec.Top);
        }

        /// <summary>
        /// Расование разделителей групп
        /// </summary>
        private void PaintGroupSeparator(Graphics g)
        {
            for (int col = GroupSize; col < _iHexMaxHBytes; col += GroupSize)
            {
                var pen = new Pen(new SolidBrush(InfoForeColor), 1);
                PointF headerPointF = GetHeaderPointF(col);
                headerPointF.X -= _charSize.Width / 2;
                g.DrawLine(pen, headerPointF, new PointF(headerPointF.X, headerPointF.Y + _recHeader.Height + _recHex.Height));

                // if (ColumnAsciiVisible)
                if (false)
                {
                    PointF byteStringPointF = GetAsciiPointF(new Point(col, 0));
                    headerPointF.X -= 2;
                    g.DrawLine(pen, new PointF(byteStringPointF.X, byteStringPointF.Y), new PointF(byteStringPointF.X, byteStringPointF.Y + _recHex.Height));
                }
            }
        }

        #endregion
    }
}