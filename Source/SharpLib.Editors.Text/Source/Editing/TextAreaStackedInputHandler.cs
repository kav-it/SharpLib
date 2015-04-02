using System;
using System.Windows.Input;

namespace SharpLib.Notepad.Editing
{
    public abstract class TextAreaStackedInputHandler : ITextAreaInputHandler
    {
        #region Поля

        private readonly TextArea textArea;

        #endregion

        #region Свойства

        public TextArea TextArea
        {
            get { return textArea; }
        }

        #endregion

        #region Конструктор

        protected TextAreaStackedInputHandler(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            this.textArea = textArea;
        }

        #endregion

        #region Методы

        public virtual void Attach()
        {
        }

        public virtual void Detach()
        {
        }

        public virtual void OnPreviewKeyDown(KeyEventArgs e)
        {
        }

        public virtual void OnPreviewKeyUp(KeyEventArgs e)
        {
        }

        #endregion
    }
}