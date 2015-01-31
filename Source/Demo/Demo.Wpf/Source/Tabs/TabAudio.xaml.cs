using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using SharpLib;
using SharpLib.Audio;

namespace DemoWpf
{
    public partial class TabAudio
    {
        #region Поля

        private readonly AudioFile _file;

        private DispatcherTimer _timer;

        private ITimer _timer1;

        #endregion

        #region Конструктор

        public TabAudio()
        {
            InitializeComponent();

            var asm = Assembly.GetExecutingAssembly();
            asm.CopyEmbeddedResourceToFileEx("Source/Assets/Music.mp3", false);

            _file = new AudioFile(asm.GetDirectoryEx() + "\\Music.mp3");

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Tick += _timer_Tick;
            // _timer.Start();

            _timer1 = Timers.Create(500, _timer_Tick);
            _timer1.Start();

            Application.Current.Exit += ApplicationExit;
        }

        #endregion

        #region Методы

        private void _timer_Tick(object sender, EventArgs e)
        {
            var text = string.Format("{0}/{1}", _file.CurrentTime.ToStringMinEx(), _file.TotalTime.ToStringMinEx());
            PART_labelTime.Content = text;
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            if (_file != null)
            {
                _file.Dispose();
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            _file.Play();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            _file.Pause();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            _file.Stop();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_file != null)
            {
                var value = (int)PART_sliderVolume.Value;
                _file.Volume = value;
                PART_labelVolume.Content = value;
            }
        }

        #endregion
    }
}