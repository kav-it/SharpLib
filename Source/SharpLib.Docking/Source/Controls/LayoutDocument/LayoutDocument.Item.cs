using System.Windows;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class LayoutDocumentItem : LayoutItem
    {
        #region Поля

        public static readonly DependencyProperty DescriptionProperty;

        private LayoutDocument _document;

        #endregion

        #region Свойства

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        #endregion

        #region Конструктор

        static LayoutDocumentItem()
        {
            DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(LayoutDocumentItem), new FrameworkPropertyMetadata(null, OnDescriptionChanged));
        }

        internal LayoutDocumentItem()
        {
        }

        #endregion

        #region Методы

        internal override void Attach(LayoutContent model)
        {
            _document = model as LayoutDocument;
            base.Attach(model);
        }

        protected override void Close()
        {
            var dockingManager = _document.Root.Manager;
            dockingManager._ExecuteCloseCommand(_document);
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentItem)d).OnDescriptionChanged(e);
        }

        protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
        {
            _document.Description = (string)e.NewValue;
        }

        internal override void Detach()
        {
            _document = null;
            base.Detach();
        }

        #endregion
    }
}