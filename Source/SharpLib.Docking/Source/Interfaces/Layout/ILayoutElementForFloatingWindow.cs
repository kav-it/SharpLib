namespace SharpLib.Docking
{
    internal interface ILayoutElementForFloatingWindow
    {
        #region ��������

        double FloatingWidth { get; set; }

        double FloatingHeight { get; set; }

        double FloatingLeft { get; set; }

        double FloatingTop { get; set; }

        bool IsMaximized { get; set; }

        #endregion
    }
}