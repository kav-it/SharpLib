using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Docking.Controls
{
    public class MenuItemEx : MenuItem
    {
        #region Поля

        public static readonly DependencyProperty IconTemplateProperty;

        public static readonly DependencyProperty IconTemplateSelectorProperty;

        private bool _reentrantFlag;

        #endregion

        #region Свойства

        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(IconTemplateSelectorProperty); }
            set { SetValue(IconTemplateSelectorProperty, value); }
        }

        #endregion

        #region Конструктор

        static MenuItemEx()
        {
            IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(MenuItemEx), new FrameworkPropertyMetadata(null, OnIconTemplateChanged));
            IconTemplateSelectorProperty = DependencyProperty.Register("IconTemplateSelector", typeof(DataTemplateSelector), typeof(MenuItemEx),
                new FrameworkPropertyMetadata(null, OnIconTemplateSelectorChanged));

            IconProperty.OverrideMetadata(typeof(MenuItemEx), new FrameworkPropertyMetadata(OnIconPropertyChanged));
        }

        #endregion

        #region Методы

        private static void OnIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItemEx)d).OnIconTemplateChanged(e);
        }

        protected virtual void OnIconTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateIcon();
        }

        private static void OnIconTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItemEx)d).OnIconTemplateSelectorChanged(e);
        }

        protected virtual void OnIconTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateIcon();
        }

        private static void OnIconPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((MenuItemEx)sender).UpdateIcon();
            }
        }

        private void UpdateIcon()
        {
            if (_reentrantFlag)
            {
                return;
            }
            _reentrantFlag = true;
            if (IconTemplateSelector != null)
            {
                var dataTemplateToUse = IconTemplateSelector.SelectTemplate(Icon, this);
                if (dataTemplateToUse != null)
                {
                    Icon = dataTemplateToUse.LoadContent();
                }
            }
            else if (IconTemplate != null)
            {
                Icon = IconTemplate.LoadContent();
            }
            _reentrantFlag = false;
        }

        #endregion
    }
}