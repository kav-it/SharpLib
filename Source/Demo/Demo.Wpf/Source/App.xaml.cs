
using SharpLib;
using SharpLib.Log;

namespace DemoWpf
{
    public partial class App
    {
        public App()
        {
            var logger = LogManager.Instance.GetCurrentClassLogger();
            logger.Debug("sdfsdf");

            SharpLibApp.Instance.Init();
        }
    }
}