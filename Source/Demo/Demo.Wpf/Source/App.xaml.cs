
using SharpLib;
using SharpLib.Log;

namespace DemoWpf
{
    public partial class App
    {
        public App()
        {
            var a = LogManager.Instance.GetLogger("sdfsdf");
            a.Info("sdfsdf");
            SharpLibApp.Instance.Init();
        }
    }
}