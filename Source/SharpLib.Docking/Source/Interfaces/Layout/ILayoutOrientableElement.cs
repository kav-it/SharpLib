using System.Windows.Controls;

namespace SharpLib.Docking
{
    public interface ILayoutOrientableGroup : ILayoutGroup
    {
        #region Свойства

        Orientation Orientation { get; set; }

        #endregion
    }
}