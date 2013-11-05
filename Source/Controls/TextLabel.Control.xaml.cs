//*****************************************************************************
//
// Имя файла    : 'TextLabel.Control.cs'
// Заголовок    : Компонент "TextLabel" с дополнительным функциями
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 29/09/2012
//
//*****************************************************************************
			
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SharpLib
{
    #region Класс TextLabel
    public partial class TextLabel : Label
    {
        #region Поля
        private TextBlock _textBlock;
        #endregion Поля

        #region Свойства
        [Browsable(true), Category("Common"), Description("Текст элемента")]
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion Свойства

        #region Свойства зависимости
        public static readonly DependencyProperty TextProperty;
        #endregion Свойства зависимости

        #region Конструктор
        static TextLabel()
        {
            TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(TextLabel), new PropertyMetadata("TextLabel"));
        }
        public TextLabel()
        {
            InitializeComponent();

        }
        #endregion Конструктор

        #region Методы
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBlock = (TextBlock)this.Template.FindName("PART_textBlock", this);
        }
        public void SetColor(int start, int count, Brush brush)
        {
            TextPointer startPointer = GetPointerFromCharOffset(start);
            TextPointer endPointer = GetPointerFromCharOffset(start + count);
            if ((startPointer != null) && (endPointer != null))
            {
                TextRange range = new TextRange(startPointer, endPointer);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }
        }
        public TextPointer GetPointerFromCharOffset(Int32 charOffset)
        {
            TextPointer nextPointer = _textBlock.ContentStart;
            if (charOffset == 0)
                return nextPointer;
            Int32 counter = 0;
            for (int i = 0; (nextPointer != null) && (counter < charOffset); i++)
            {
                if (nextPointer.CompareTo(_textBlock.ContentEnd) == 0)
                    return nextPointer;
                if (nextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    nextPointer = _textBlock.ContentStart.GetPositionAtOffset(i);
                    counter++;
                }
                else
                    nextPointer = _textBlock.ContentStart.GetPositionAtOffset(i);
            }
            return nextPointer;
        }
        #endregion Методы
    }
    #endregion Класс TextLabel
}
