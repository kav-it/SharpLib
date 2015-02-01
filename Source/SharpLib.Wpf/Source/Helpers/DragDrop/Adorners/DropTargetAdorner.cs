using System;
using System.Windows;
using System.Windows.Documents;

namespace SharpLib.Wpf.Dragging
{
    public abstract class DropTargetAdorner : Adorner
    {
        #region Поля

        private readonly AdornerLayer _adornerLayer;

        #endregion

        #region Свойства

        public DropInfo DropInfo { get; set; }

        #endregion

        #region Конструктор

        protected DropTargetAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            _adornerLayer.Add(this);
            IsHitTestVisible = false;
        }

        #endregion

        #region Методы

        public void Detatch()
        {
            _adornerLayer.Remove(this);
        }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");
            }

            var constructorInfo = type.GetConstructor(new[] { typeof(UIElement) });
            if (constructorInfo != null)
            {
                return (DropTargetAdorner)constructorInfo.Invoke(new object[] { adornedElement });
            }

            return null;
        }

        #endregion
    }
}