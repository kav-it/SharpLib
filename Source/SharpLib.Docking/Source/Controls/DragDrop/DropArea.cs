using System.Windows;

namespace SharpLib.Docking.Controls
{
    public class DropArea<T> : IDropArea where T : FrameworkElement
    {
        #region Поля

        private readonly Rect _detectionRect;

        private readonly T _element;

        private readonly DropAreaType _type;

        #endregion

        #region Свойства

        public Rect DetectionRect
        {
            get { return _detectionRect; }
        }

        public DropAreaType Type
        {
            get { return _type; }
        }

        public T AreaElement
        {
            get { return _element; }
        }

        #endregion

        #region Конструктор

        internal DropArea(T areaElement, DropAreaType type)
        {
            _element = areaElement;
            _detectionRect = areaElement.GetScreenArea();
            _type = type;
        }

        #endregion
    }
}