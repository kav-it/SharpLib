using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;

using SharpLib;
using SharpLib.Audio;
using SharpLib.Audio.Wave;

namespace DemoWpf
{
    public partial class TabAudio
    {
        #region Поля

        private readonly AudioFile _file;

        private bool _isDragging;

        private bool _isPlaying;

        #endregion

        #region Конструктор

        public TabAudio()
        {
            InitializeComponent();

            var asm = Assembly.GetExecutingAssembly();
            asm.CopyEmbeddedResourceToFileEx("Source/Assets/Music.mp3", false);
            asm.CopyEmbeddedResourceToFileEx("Source/Assets/Music.wma", false);

            // _file = new AudioFile(asm.GetDirectoryEx() + "\\Music.mp3");
            _file = new AudioFile(asm.GetDirectoryEx() + "\\Music.wma");
            _file.PlayProgress += _file_PlayProgress;

            PART_sliderPlay.IsSnapToTickEnabled = true;
            PART_sliderPlay.Minimum = 0;
            PART_sliderPlay.Maximum = _file.TotalTime.TotalSeconds;

            UpdateSlider(_file.CurrentTime);

            Application.Current.Exit += ApplicationExit;
        }

        #endregion

        #region Методы

        private void _file_PlayProgress(object sender, AudioFileProgressArgs args)
        {
            UpdateSlider(args.Current);
        }

        private void UpdateSlider(TimeSpan current)
        {
            var total = new TimeSpan(0, 0, 0, (int)PART_sliderPlay.Maximum);
            var text = string.Format("{0}/{1}", current.ToStringMinEx(), total.ToStringMinEx());
            PART_labelTime.Content = text;
            PART_sliderPlay.Value = current.TotalSeconds;
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
            Play();
            
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

        private void PART_sliderPlay_OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;

            if (_file.State == PlaybackState.Playing)
            {
                _isPlaying = true;
                _file.Pause();
            }
        }

        private void PART_sliderPlay_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;

            if (_isPlaying)
            {
                Play();

                _isPlaying = false;
            }
        }

        private void PART_sliderPlay_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging)
            {
                var current = new TimeSpan(0, 0, 0, (int)e.NewValue);
                UpdateSlider(current);
            }
        }

        private void Play()
        {
            var current = new TimeSpan(0, 0, 0, (int)PART_sliderPlay.Value);
            _file.Play(current);
        }
    }
}