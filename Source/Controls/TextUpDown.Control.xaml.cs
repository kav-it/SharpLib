//*****************************************************************************
//
// Имя файла    : 'TextUpDown.Control.cs'
// Заголовок    : Компонент "Изменение числового значения (спин)"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/06/2012
//
//*****************************************************************************

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpLib
{

    #region Перечисление TextUpDownMode

    public enum TextUpDownMode
    {
        IntegerData,

        DoubleData,
    }

    #endregion Перечисление TextUpDownMode

    #region Класс TextUpDown

    public partial class TextUpDown : UserControl
    {
        #region Поля

        public static DependencyProperty FormatStringProperty;

        public static DependencyProperty IncrementProperty;

        public static DependencyProperty MaxValueProperty;

        public static DependencyProperty MinValueProperty;

        public static DependencyProperty ModeProperty;

        public static DependencyProperty RollOverProperty;

        public static DependencyProperty ValueProperty;

        #endregion

        #region Свойства

        [Browsable(true)]
        [Category("Common")]
        [Description("Минимальное значение")]
        public Double MaxValue
        {
            get { return (Double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Максимальное значение")]
        public Double MinValue
        {
            get { return (Double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Текущее значение")]
        public Double Value
        {
            get { return (Double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Величина изменения")]
        public Double Increment
        {
            get { return (Double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Режим Целые/Дробные значения")]
        public TextUpDownMode Mode
        {
            get { return (TextUpDownMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("При достижении максимума переходить к минимуму и наоборот")]
        public Boolean RollOver
        {
            get { return (Boolean)GetValue(RollOverProperty); }
            set { SetValue(RollOverProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Формат отображаемой строки")]
        public String FormatString
        {
            get { return (String)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Количество вводимых символов")]
        public int MaxLength
        {
            get { return PART_textEdit.MaxLength; }
            set { PART_textEdit.MaxLength = value; }
        }

        #endregion

        #region Конструктор

        static TextUpDown()
        {
            ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(TextUpDown), new PropertyMetadata((Double)0, OnValuePropertyChanged, OnValuePropertyCoerce));
            MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(Double), typeof(TextUpDown), new PropertyMetadata((Double)UInt32.MaxValue, OnMaxValuePropertyChanged));
            MinValueProperty = DependencyProperty.Register("MinValue", typeof(Double), typeof(TextUpDown), new PropertyMetadata((Double)0, OnMinValuePropertyChanged));
            IncrementProperty = DependencyProperty.Register("Increment", typeof(Double), typeof(TextUpDown), new PropertyMetadata((Double)1));
            ModeProperty = DependencyProperty.Register("TextUpDownMode", typeof(TextUpDownMode), typeof(TextUpDown), new PropertyMetadata(TextUpDownMode.IntegerData, OnModePropertyChanged));
            RollOverProperty = DependencyProperty.Register("RollOver", typeof(Boolean), typeof(TextUpDown), new PropertyMetadata(false));
            FormatStringProperty = DependencyProperty.Register("FormatString", typeof(String), typeof(TextUpDown), new PropertyMetadata(String.Empty));
        }

        public TextUpDown()
        {
            InitializeComponent();

            PART_textEdit.MaxLength = 10;
        }

        #endregion

        #region Установка свойства Value

        private static Object OnValuePropertyCoerce(DependencyObject sender, Object baseValue)
        {
            TextUpDown control = (TextUpDown)sender;
            Double value = (Double)baseValue;
            Double max = control.MaxValue;
            Double min = control.MinValue;
            Double inc = control.Increment;

            if (value > max)
            {
                if (control.RollOver)
                    value = min;
                else
                    value = max;
            }
            else if (value < min)
            {
                if (control.RollOver)
                    value = max;
                else
                    value = min;
            }

            return value;
        }

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Double value = (Double)e.NewValue;

            ((TextUpDown)sender).SetValue(value);
        }

        private void SetValue(Double value)
        {
            PART_textEdit.Text = value.ToString();
        }

        private static void OnModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextUpDownMode mode = (TextUpDownMode)e.NewValue;

            ((TextUpDown)sender).SetMode(mode);
        }

        private void SetMode(TextUpDownMode mode)
        {
            if (mode == TextUpDownMode.IntegerData)
            {
                MinValue = 0;
                MaxValue = UInt32.MaxValue;
            }
            else if (mode == TextUpDownMode.DoubleData)
            {
                MinValue = Double.MinValue;
                MaxValue = Double.MaxValue;
            }
        }

        private static void OnMinValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Double minValue = (Double)e.NewValue;

            ((TextUpDown)sender).SetMinValue(minValue);
        }

        private void SetMinValue(Double minValue)
        {
            UpdateMaxLenght(minValue, MaxValue);
        }

        private static void OnMaxValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Double maxValue = (Double)e.NewValue;

            ((TextUpDown)sender).SetMaxValue(maxValue);
        }

        private void SetMaxValue(Double maxValue)
        {
            UpdateMaxLenght(MinValue, maxValue);
        }

        private void UpdateMaxLenght(Double minValue, Double maxValue)
        {
            int minLen = minValue.ToString().Length;
            int maxLen = maxValue.ToString().Length;

            MaxLength = Math.Max(minLen, maxLen);
        }

        #endregion Установка свойства Value

        #region Обработка событий

        private void IncValue()
        {
            Value = Value + Increment;
        }

        private void DecValue()
        {
            Value = Value - Increment;
        }

        /// <summary>
        /// Обработка события "Нажатие на клавиатуре"
        /// </summary>
        private void PART_textEdit_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) IncValue();
            else if (e.Key == Key.Down) DecValue();
            else if (e.Key == Key.Enter)
            {
                String text = PART_textEdit.Text;
                Double value = text.ToDoubleEx();
                ClearValue(ValueProperty);
                Value = value;
            }
        }

        /// <summary>
        /// Обработка события "Нажатие кнопки "+"
        /// </summary>
        private void RepeatButton_ClickInc(object sender, RoutedEventArgs e)
        {
            IncValue();
        }

        /// <summary>
        /// Обработка события "Нажатие кнопки "-"
        /// </summary>
        private void RepeatButton_ClickDec(object sender, RoutedEventArgs e)
        {
            DecValue();
        }

        /// <summary>
        /// Обработка события "Скролл мыши"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_textEdit_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int delta = e.Delta;

            if (delta > 0)
                IncValue();
            else
                DecValue();
        }

        #endregion Обработка событий

        #region Установка фокуса

        public new Boolean Focus()
        {
            return PART_textEdit.Focus();
        }

        #endregion Установка фокуса
    }

    #endregion Класс TextUpDown
}