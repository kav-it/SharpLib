using System;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Диалог перехода по адресу
    /// </summary>
    internal partial class HexBoxFindDialog : Form
    {
        #region Свойства

        private static string _oldData;

        /// <summary>
        /// Блок данных поиска
        /// </summary>
        internal string Data { get; private set; }

        #endregion

        #region Конструктор

        internal HexBoxFindDialog()
        {
            InitializeComponent();

            Data = string.Empty;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;

            if (_oldData.IsValid())
            {
                textEdit1.Text = _oldData;
            }
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

        /// <summary>
        /// Обработка нажатия "ОК"
        /// </summary>
        private void button1_Click(object sender, System.EventArgs e)
        {
            _oldData = textEdit1.Text;
            Data = Parse(textEdit1.Text);

            DialogResult = DialogResult.OK;

            Close();
        }

        /// <summary>
        /// Разбор введенного адреса
        /// </summary>
        private string Parse(string text)
        {
            text = text.ToLower();

            var result = string.Empty;
            int flag = 0;
            var str = string.Empty;

            for (int i = 0; i < text.Length; i++)
            {
                // Чтение символа
                char ch = text[i];

                // Анализ символа
                if (flag == 1)
                {
                    // Первый символ после "#"
                    if (ch == '#')
                    {
                        result = result + ch;
                        flag = 0;
                    }
                    else
                    {
                        str = "" + ch;
                        flag = 2;
                    }
                }
                else if (flag == 2)
                {
                    // Второй символ после "#"
                    str = str + ch;
                    result = result + str.ToAsciiByteEx().ToCharEx();
                    flag = 0;
                    str = string.Empty;
                }
                else
                {
                    if (ch == '#') flag = 1;
                    else result = result + ch;
                }
            } // end for (перебор символов)

            return result;
        }

        #endregion
    }
}