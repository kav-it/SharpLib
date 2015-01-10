using System;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Диалог перехода по адресу
    /// </summary>
    internal partial class HexBoxGotoDialog : Form
    {
        #region Свойства

        /// <summary>
        /// Введенный адрес
        /// </summary>
        internal int Addr { get; private set; }

        #endregion

        #region Конструктор

        internal HexBoxGotoDialog()
        {
            InitializeComponent();

            Addr = -1;

            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Установка начального фокуса
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            textEdit1.Select();
        }

        /// <summary>
        /// Обработка Esc и Enter
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;

                case Keys.Enter:
                    button1_Click(this, EventArgs.Empty);
                    break;
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Addr = Parse(textEdit1.Text);

            DialogResult = DialogResult.OK;

            Close();
        }

        private int Parse(string text)
        {
            text = text.ToLower();

            if (text.EndsWith("k"))
            {
                text = text.TrimEndEx("k");
                return text.ToIntEx() * 1024;
            }

            if (text.EndsWith("m"))
            {
                text = text.TrimEndEx("m");
                return text.ToIntEx() * 1024 * 1024;
            }

            return text.ToIntFromHexEx();
        }

        #endregion
    }
}