using System.Threading;

using SharpLib.Log;

namespace DemoWpf
{
    public partial class TabMemo
    {
        #region Поля

        private readonly ILogger _logger;

        private int _id;

        private Timer _timer;

        #endregion

        #region Конструктор

        public TabMemo()
        {
            InitializeComponent();

            var target = new MemoryEventTarget("memory", TargetOnEventReceived);
            LogManager.Instance.Configuration.AddTarget(target, LogLevel.Debug);
            _logger = LogManager.Instance.GetLogger();

            _timer = new Timer(OnTimer, null, 0, 500);
        }

        #endregion

        #region Методы

        private void TargetOnEventReceived(MemoryEventTarget sender, MemoryEventTargetArgs args)
        {
            var evt = args.Value.FormattedMessage;

            PART_memo.AddLine(evt);
        }

        private void OnTimer(object state)
        {
            _logger.Info("Сообщение: {0}", ++_id);
        }

        #endregion
    }
}