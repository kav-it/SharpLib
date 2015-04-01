using System;

namespace ICSharpCode.AvalonEdit.Editing
{
    [Serializable]
    public class TextEventArgs : EventArgs
    {
        #region ����

        private readonly string text;

        #endregion

        #region ��������

        public string Text
        {
            get { return text; }
        }

        #endregion

        #region �����������

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