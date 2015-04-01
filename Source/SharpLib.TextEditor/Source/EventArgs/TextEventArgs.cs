using System;

namespace ICSharpCode.AvalonEdit.Editing
{
    [Serializable]
    public class TextEventArgs : EventArgs
    {
        #region Поля

        private readonly string text;

        #endregion

        #region Свойства

        public string Text
        {
            get { return text; }
        }

        #endregion

        #region Конструктор

        public TextEventArgs(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            this.text = text;
        }

        #endregion
    }
}