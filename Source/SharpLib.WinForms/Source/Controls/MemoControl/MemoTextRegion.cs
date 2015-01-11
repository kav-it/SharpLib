using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    internal class MemoTextRegion
    {
        #region Поля

        private Font _font;

        private Point _sizeInPixels;

        private StringFormat _stringFormat;

        #endregion

        #region Свойства

        private StringFormat StringFormat
        {
            get
            {
                if (_stringFormat != null)
                {
                    return _stringFormat;
                }

                _stringFormat = new StringFormat(StringFormat.GenericTypographic);
                _stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

                return _stringFormat;
            }
        }

        /// <summary>
        /// Текущий шрифт
        /// </summary>
        internal Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                UpdateCharSize();
                UpdateSizeInChars();
            }
        }

        /// <summary>
        /// Информация о регионе (размеры, положение)
        /// </summary>
        internal RectangleF Rect { get; private set; }

        /// <summary>
        /// Размер символа
        /// </summary>
        internal Point CharSize { get; private set; }

        /// <summary>
        /// Размер области (в пикселях)
        /// </summary>
        internal Point SizeInPixels
        {
            get { return _sizeInPixels; }
            set
            {
                _sizeInPixels = value;
                UpdateSizeInChars();
            }
        }

        /// <summary>
        /// Размер области (в символах)
        /// </summary>
        internal Point SizeInChars { get; private set; }

        #endregion

        #region Методы

        /// <summary>
        /// Изменение размеров региона
        /// </summary>
        internal void Resize(int widht, int height)
        {
            SizeInPixels = new Point(widht, height);
        }

        /// <summary>
        /// Обновление размера символа
        /// </summary>
        private void UpdateCharSize()
        {
            using (var control = new Label())
            {
                control.Font = Font;
                SizeF charSize;
                using (var g = control.CreateGraphics())
                {
                    charSize = g.MeasureString("A", Font, 100, StringFormat);
                }

                CharSize = new Point((int)charSize.Width, (int)charSize.Height);
            }
        }

        /// <summary>
        /// Перерасчет информации о количество символов
        /// </summary>
        private void UpdateSizeInChars()
        {
            // Расчет количества символов
            var countX = SizeInPixels.X / CharSize.X;
            var countY = SizeInPixels.Y / CharSize.Y + 1;
            SizeInChars = new Point(countX, countY);
        }

        /// <summary>
        /// Вывод текста в указанной позиции
        /// </summary>
        internal void DrawText(Graphics g, int charX, int charY, string text, Color color)
        {
            var brush = new SolidBrush(color);
            var pointX = charX * CharSize.X;
            var pointY = charY * CharSize.Y;

            g.DrawString(text, Font, brush, pointX, pointY, _stringFormat);
        }

        /// <summary>
        /// Нарисовать таблицу
        /// </summary>
        internal void DrawGrid(Graphics g)
        {
            var pen = new Pen(Color.Black);
            var countX = SizeInChars.X;
            var countY = SizeInChars.Y;

            for (int x = 0; x < countX; x++)
            {
                g.DrawLine(pen, x * CharSize.X, 0, x * CharSize.X, countY * CharSize.Y);
            }

            for (int y = 0; y < countY; y++)
            {
                g.DrawLine(pen, 0, y * CharSize.Y, countX * CharSize.X, y * CharSize.Y);
            }

            var charSize = g.MeasureString("A", Font, 100, _stringFormat);
        }

        #endregion
    }
}