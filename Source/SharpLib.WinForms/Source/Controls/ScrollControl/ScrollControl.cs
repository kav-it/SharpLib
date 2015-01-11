using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    public abstract class ScrollControl : Control
    {
        #region Свойства

        protected ScrollBarInfo ScrollBarX { get; private set; }

        protected ScrollBarInfo ScrollBarY { get; private set; }

        #endregion

        #region Конструктор

        protected ScrollControl()
        {
            ScrollBarX = new ScrollBarInfo(this, false, 0);
            ScrollBarY = new ScrollBarInfo(this, true, 0);

            Controls.Add(ScrollBarX.ScrollControl);
            Controls.Add(ScrollBarY.ScrollControl);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Установка минимального размера элемента
        /// </summary>
        protected override void InitLayout()
        {
            base.InitLayout();

            // 50 взяты с "потолка"
            MinimumSize = new Size(50, 50);
        }

        /// <summary>
        /// Перерасчет видимости Scrollbar с учетом изменения размера элемента
        /// </summary>
        protected void UpdateScrollBarVisibly()
        {
            // Изменилось содержимое элемента. Определение желаемого размера
            var size = OnMeasureSize();

            // Перерасчет видимости и размеров ScrollBar
            size = CalculateClientSize(size);

            /*
            var xVisible = ScrollBarX.Visible;
            var yVisible = ScrollBarY.Visible;

            UpdateClientSize(ref size);

            if (xVisible != ScrollBarX.Visible || yVisible != ScrollBarY.Visible)
            {
                OnArrangeSize(size);
            }
            */
        }

        /// <summary>
        /// Перерасчет размеров клиентской области
        /// </summary>
        private Size CalculateClientSize(Size desireSize)
        {
            int width = Width;
            int height = Height;
            var xVisible = false;
            var yVisible = false;

            if (desireSize.Height >= height)
            {
                // Ожидаемая высота больше возможной: Необходимо отображение с вертикальным скролбаром
                width = width - ScrollBarY.ScrollControl.Width;
                yVisible = true;
            }

            if (desireSize.Width >= width)
            {
                // Ожидаемая ширина больше возможной: Необходимо отображение с горизонтальный скролбаром
                // Значит высота будет уменьшена на высоту скролбара
                height = height - ScrollBarX.ScrollControl.Height;
                xVisible = true;
            }

            var realSize = new Size(width, height);

            ScrollBarX.UpdateVisibleAndPosition(xVisible, desireSize);
            ScrollBarY.UpdateVisibleAndPosition(yVisible, desireSize);

            return realSize;
        }

        /// <summary>
        /// Изменение размеров элемента
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var size = OnMeasureSize();

            size = CalculateClientSize(size);

            OnArrangeSize(size);
        }

        /// <summary>
        /// Измерение размеров контента (реализация в наследниках)
        /// </summary>
        protected abstract Size OnMeasureSize();

        /// <summary>
        /// Установка размеров клиента (реализация в наследниках)
        /// </summary>
        protected abstract void OnArrangeSize(Size size);

        /// <summary>
        /// Маштабирование через Scroll
        /// </summary>
        protected virtual void OnZoomScroll(int direction)
        {
        }

        /// <summary>
        /// Реализация Focusable
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused)
            {
                Focus();
            }

            base.OnMouseDown(e);
        }

        #endregion

        #region Вложенный класс: ScrollBarInfo

        public class ScrollBarInfo
        {
            #region Поля

            /// <summary>
            /// Тип элемента (вертикальный/горизонтальный)
            /// </summary>
            private readonly bool _isVertical;

            /// <summary>
            /// Элемент-владелец
            /// </summary>
            private readonly ScrollControl _owner;

            #endregion

            #region Свойства

            /// <summary>
            /// Компонент скролбар
            /// </summary>
            internal ScrollBar ScrollControl { get; private set; }

            /// <summary>
            /// Видимость скролбара
            /// </summary>
            internal bool Visible { get; private set; }

            /// <summary>
            /// Текущая позиция скролбара
            /// </summary>
            internal int Position { get; private set; }

            /// <summary>
            /// Ширина Scrollbar
            /// </summary>
            internal int Width
            {
                get { return ScrollControl.Width; }
            }

            /// <summary>
            /// Высота Scrollbar
            /// </summary>
            internal int Height
            {
                get { return ScrollControl.Height; }
            }

            /// <summary>
            /// Маштаб (используется при скроллинге на фиксированые единицы, например строка текстового редактора)
            /// </summary>
            internal int Scale { get; set; }

            #endregion

            #region Конструктор

            public ScrollBarInfo(ScrollControl owner, bool isVertical, int position)
            {
                _owner = owner;
                _owner.MouseWheel += Control_MouseWheel;

                _isVertical = isVertical;
                Scale = 1;
                ScrollControl = isVertical ? (ScrollBar)new VScrollBar() : new HScrollBar();
                ScrollControl.Dock = isVertical ? DockStyle.Right : DockStyle.Bottom;
                ScrollControl.Visible = false;
                ScrollControl.Scroll += Control_Scroll;
                ScrollControl.ValueChanged += Control_ValueChanged;
                ScrollControl.LargeChange = 1;
                Visible = false;
                Position = position;
            }

            #endregion

            #region Методы

            /// <summary>
            /// Определение максимального значения
            /// </summary>
            private int CalculateMax(Size desireSize)
            {
                int size;
                int desire;
                if (_isVertical)
                {
                    desire = desireSize.Height / Scale;
                    size = _owner.Height / Scale;
                }
                else
                {
                    desire = (desireSize.Width / Scale);
                    size = (_owner.Width / Scale);
                }

                return desire > size ? desire - size : 0;
            }

            /// <summary>
            /// Смена видимость скролбара
            /// </summary>
            internal void UpdateVisibleAndPosition(bool newValue, Size desireSize)
            {
                // Изменилась видимость скролбара
                if (Visible != newValue)
                {
                    if (newValue)
                    {
                        // Расчет максимального значения
                        int max = CalculateMax(desireSize);

                        ScrollControl.Minimum = 0;
                        ScrollControl.Maximum = max;
                        Position = 0;
                        ScrollControl.Value = 0;
                    }
                    else
                    {
                        ScrollControl.Minimum = 0;
                        ScrollControl.Maximum = 0;
                        Position = 0;
                        ScrollControl.Value = 0;
                    }

                    Visible = newValue;
                    ScrollControl.Visible = newValue;
                }
                else
                {
                    // Видимость скролбара не изменилась, но изменился размер.
                    // Возможно понадобится пересчет позиции
                    var newMax = CalculateMax(desireSize);

                    if (ScrollControl.Maximum != newMax)
                    {
                        // Пересчет текущей позиции
                        var newPosition = Position * newMax / ScrollControl.Maximum;
                        // Изменился максимальный размер
                        ScrollControl.Maximum = newMax;
                        ScrollControl.Value = newPosition;
                    }
                }
            }

            /// <summary>
            /// Скролл средней кнопкой мыши
            /// </summary>
            private void Control_MouseWheel(object sender, MouseEventArgs e)
            {
                var dir = e.Delta > 0 ? 1 : -1;

                if (ModifierKeys == Keys.Control)
                {
                    _owner.OnZoomScroll(dir);
                    return;
                }

                if (_isVertical)
                {
                    if (dir == -1)
                    {
                        ScrollInc();
                    }
                    else
                    {
                        ScrollDec();
                    }
                }
            }

            /// <summary>
            /// Изменилось значение позиции
            /// </summary>
            private void Control_ValueChanged(object sender, EventArgs e)
            {
                Position = ScrollControl.Value;

                _owner.Invalidate();
            }

            private void Control_Scroll(object sender, ScrollEventArgs e)
            {
            }

            /// <summary>
            /// Увеличение значения (горизонтальный - вправо, вертикальный - вниз)
            /// </summary>
            private void ScrollInc()
            {
                if (ScrollControl.Value + 1 <= ScrollControl.Maximum)
                {
                    ScrollControl.Value = ScrollControl.Value + 1;
                }
            }

            /// <summary>
            /// Уменьшение значения (горизонтальный - влево, вертикальный - вверх)
            /// </summary>
            private void ScrollDec()
            {
                if (ScrollControl.Value - 1 >= ScrollControl.Minimum)
                {
                    ScrollControl.Value = ScrollControl.Value - 1;
                }
            }

            #endregion
        }

        #endregion
    }
}