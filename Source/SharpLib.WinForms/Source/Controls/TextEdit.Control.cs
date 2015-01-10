using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Наследние TextBox с Watermark
    /// </summary>
    public class TextEdit : TextBox
    {
        #region Константы

        private const string DEFAULT_WATERMARK_COLOR = "DarkGray";

        private const int WM_PAINT = 0xF;

        #endregion

        #region Поля

        private string _watermark;

        private Color _watermarkColor;

        #endregion

        #region Свойства

        /// <summary>
        /// Подсказка
        /// </summary>
        [Category(ControlsConst.CATEGORY_NAME), Description("Подсказка")]
        [DefaultValue("")]
        public string Watermark
        {
            get { return _watermark; }
            set
            {
                _watermark = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Цвет подсказки
        /// </summary>
        [Category(ControlsConst.CATEGORY_NAME), Description("Цвет подсказки")]
        [DefaultValue(typeof(Color), DEFAULT_WATERMARK_COLOR)]
        public Color WatermarkColor
        {
            get { return _watermarkColor; }
            set
            {
                _watermarkColor = value;
                Invalidate();
            }
        }

        #endregion

        #region Конструктор

        public TextEdit()
        {
            _watermark = string.Empty;
            _watermarkColor = Color.FromName(DEFAULT_WATERMARK_COLOR);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка оконных сообщений
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                WatermarkPaint();
            }
        }

        /// <summary>
        /// Отображение подсказки
        /// </summary>
        private void WatermarkPaint()
        {
            using (Graphics graphics = Graphics.FromHwnd(Handle))
            {
                if (Text.Length == 0 && !string.IsNullOrEmpty(_watermark))
                {
                    var format = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;

                    if (RightToLeft == RightToLeft.Yes)
                    {
                        format |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
                    }

                    TextRenderer.DrawText(graphics, _watermark, Font, base.ClientRectangle, _watermarkColor, format);
                }
            }
        }

        #endregion
    }
}