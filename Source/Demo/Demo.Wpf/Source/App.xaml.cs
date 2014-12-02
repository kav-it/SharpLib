
using SharpLib;
using SharpLib.Log;

namespace DemoWpf
{
    public partial class App
    {
        public App()
        {
            LogManager.Instance.GetLogger()
            SharpLibApp.Instance.Init();
        }
    }
}