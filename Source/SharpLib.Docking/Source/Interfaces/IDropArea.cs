using System.Windows;

namespace SharpLib.Docking.Controls
{
    public interface IDropArea
    {
        #region ��������

        Rect DetectionRect { get; }

        DropAreaType Type { get; }

        #endregion
    }
}