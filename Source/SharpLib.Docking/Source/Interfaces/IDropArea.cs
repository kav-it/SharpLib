using System.Windows;

namespace SharpLib.Docking.Controls
{
    public interface IDropArea
    {
        #region Свойства

        Rect DetectionRect { get; }

        DropAreaType Type { get; }

        #endregion
    }
}