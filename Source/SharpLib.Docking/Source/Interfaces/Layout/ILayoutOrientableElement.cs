using System.Windows.Controls;

namespace SharpLib.Docking.Layout
{
    public interface ILayoutOrientableGroup : ILayoutGroup
    {
        #region Свойства

        Orientation Orientation { get; set; }

        #endregion
    }
}