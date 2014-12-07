using SharpLib;

namespace DemoWpf
{
    public partial class App
    {
        #region Конструктор

        public App()
        {
            SharpLibApp.Instance.Init();
        }

        #endregion
    }
}