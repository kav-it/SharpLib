using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    public sealed class MemoControl : ScrollControl
    {
        #region Поля

        /// <summary>
        /// Текстовый регион (вспомогательный класс)
        /// </summary>
        private readonly MemoTextRegion _region;

        #endregion

        #region Свойства

        /// <summary>
        /// Показывать таблицу (в отладочных целях)
        /// </summary>
        internal bool ShowGrid { get; set; }

        /// <summary>
        /// Коллекция строк-данных
        /// </summary>
        public MemoLineCollection Lines { get; private set; }

        public new Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                _region.Font = value;
                ScrollBarX.Scale = _region.CharSize.X;
                ScrollBarY.Scale = _region.CharSize.Y;
            }
        }

        #endregion

        #region Конструктор

        public MemoControl()
        {
            _region = new MemoTextRegion();

            BackColor = Color.White;
            Lines = new MemoLineCollection(this);
            Font = new Font("Courier New", 10);

            ScrollBarX.Scale = _region.CharSize.X;
            ScrollBarY.Scale = _region.CharSize.Y;

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Изменены данные
        /// </summary>
        internal void ChangeData()
        {
            UpdateScrollBarVisibly();
            /*
            // Перерасчет размеров (учет Auto видимости ScrollBar)
            UpdateScrollBarVisibly();
            // Обновление информации диапазона scrollbar-ов
            ScrollBarX.Maximum = Lines.MaxLengthX;

            int maxY;
            if (Lines.Count <= _region.SizeInChars.Y)
            {
                // Количество линий меньше, количество отображаемых на экране. В такой ситуации ScrollBar не должен отображаться
                // Но на всякий случай выполняется установка
                maxY = Lines.Count;
            }
            else
            {
                // Максимум скролла = количество непомещаемых строк
                maxY = Lines.Count - _region.SizeInChars.Y;
                // Корректировка, если последняя линии не помещается на экран
                maxY++;
            }

            ScrollBarY.Maximum = maxY;
*/
            // Обновление видимости
            Invalidate();
        }

        /// <summary>
        /// Определение желаемого размера элемента
        /// </summary>
        protected override Size OnMeasureSize()
        {
            // 2 по X это пустые отступы справа для улучшения визуального восприятия
            var maxX = (Lines.MaxLengthX + 2) * _region.CharSize.X;
            var maxY = Lines.Count * _region.CharSize.Y;

            var size = new Size(maxX, maxY);

            return size;
        }

        /// <summary>
        /// Изменение маштабирования
        /// </summary>
        protected override void OnZoomScroll(int direction)
        {
            if ((Font.Size < 5 && direction < 0) || (Font.Size > 60 && direction > 0))
            {
                return;
            }

            Font = direction > 0 
                ? new Font(Font.FontFamily, Font.Size + 1) 
                : new Font(Font.FontFamily, Font.Size - 1);

            UpdateScrollBarVisibly();
        }

        /// <summary>
        /// Установка размера элемента
        /// </summary>
        protected override void OnArrangeSize(Size size)
        {
            ResizeBody(size.Width, size.Height);
        }

        /// <summary>
        /// Событие "Paint"
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            PaintBody(e.Graphics);
        }

        /// <summary>
        /// Изменение размеров элемента
        /// </summary>
        private void ResizeBody(int width, int height)
        {
            _region.Resize(width, height);
        }

        /// <summary>
        /// Основной вывод текста
        /// </summary>
        private void PaintBody(Graphics g)
        {
            var startX = ScrollBarX.Position;
            var startY = ScrollBarY.Position;
            var count = Math.Min(Lines.Count, startY + _region.SizeInChars.Y) - startY;

            for (int index = 0; index < count; index++)
            {
                var text = Lines[index + startY];

                if (text.Length >= startX)
                {
                    text = text.Substring(startX);

                    _region.DrawText(g, 0, index, text, ForeColor);
                }
            }

            if (ShowGrid)
            {
                _region.DrawGrid(g);
            }
        }

        #endregion
    }
}