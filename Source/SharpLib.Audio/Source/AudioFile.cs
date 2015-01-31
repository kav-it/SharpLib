using System;
using System.Diagnostics;
using System.IO;

using SharpLib.Audio.Wave;

namespace SharpLib.Audio
{
    /// <summary>
    /// Музыкальный файл с возможностью управления
    /// </summary>
    public class AudioFile : IDisposable
    {
        #region Константы

        /// <summary>
        /// Максимальное значение громкости
        /// </summary>
        private const float MAX_VOLULME = 1.0f;

        /// <summary>
        /// Период уведомления о процессе воспроизведения (мс)
        /// </summary>
        private const int PLAY_PROGRESS_EVENT_INTERVAL = 500;

        #endregion

        #region Поля

        /// <summary>
        /// Был вызван явный Dispose
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Провайдер поток данных
        /// </summary>
        private AudioFileReader _reader;

        /// <summary>
        /// Таймер уведомления о процессе воспроизведения
        /// </summary>
        private ITimer _timer;

        /// <summary>
        /// Плеер
        /// </summary>
        private IWavePlayer _waveOutDevice;

        #endregion

        #region Свойства

        /// <summary>
        /// Состояние процесса воспроизведения
        /// </summary>
        public PlaybackState State
        {
            get { return _waveOutDevice.PlaybackState; }
        }

        /// <summary>
        /// Расположение файла
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Текущее время воспроизведения
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return _reader.CurrentTime; }
            set { _reader.CurrentTime = value; }
        }

        /// <summary>
        /// Общее время воспроизведения
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return _reader.TotalTime; }
        }

        /// <summary>
        /// Громкость (от 0 до 100%)
        /// </summary>
        /// <remarks>
        /// Сейчас реализовано линейная зависимость, но она неравномерна.
        /// </remarks>
        public int Volume
        {
            get { return (int)(_reader.Volume * 100 / MAX_VOLULME); }
            set { _reader.Volume = (value * MAX_VOLULME) / 100; }
        }

        #endregion

        #region События

        /// <summary>
        /// Событие "Пауза воспроизведения"
        /// </summary>
        public event EventHandler PlayPaused;

        /// <summary>
        /// Событие "Продолжение воспроизведения после паузы"
        /// </summary>
        public event EventHandler PlayResumed;

        /// <summary>
        /// Событие "Запущено воспроизведение"
        /// </summary>
        public event EventHandler PlayStarted;

        /// <summary>
        /// Событие "Завершено воспроизведение"
        /// </summary>
        public event EventHandler PlayStopped;

        /// <summary>
        /// Событие "Текущее состояние процесса воспроизведения"
        /// </summary>
        public event EventHandler<AudioFileProgressArgs> PlayProgress;

        #endregion

        #region Конструктор

        public AudioFile(string location)
        {
            _isDisposed = false;
            Location = location;

            if (File.Exists(location) == false)
            {
                return;
            }

            _timer = Timers.Create(PLAY_PROGRESS_EVENT_INTERVAL, OnTimerTick);
            _reader = new AudioFileReader(location);
            _waveOutDevice = new WaveOutEvent();
            _waveOutDevice.Init(_reader);
        }

        ~AudioFile()
        {
            Debug.Assert(false, "AudioFile Dispose was not called");
            // ReSharper disable HeuristicUnreachableCode
            DisposeAll();
            // ReSharper restore HeuristicUnreachableCode
        }

        #endregion

        #region Методы

        /// <summary>
        /// Тик таймера (прогресса)
        /// </summary>
        private void OnTimerTick(object sender, EventArgs args)
        {
            RaiseEventProgress();
        }

        /// <summary>
        /// Освобождение элемента
        /// </summary>
        private void DisposeAll()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_waveOutDevice != null)
            {
                _waveOutDevice.Stop();
            }

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            if (_waveOutDevice != null)
            {
                _waveOutDevice.Dispose();
                _waveOutDevice = null;
            }

            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Освобождение элемента
        /// </summary>
        public void Dispose()
        {
            DisposeAll();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Воспроизведение файла с указанной позиции
        /// </summary>
        public void Play(TimeSpan position)
        {
            if (position < TotalTime)
            {
                CurrentTime = position;
            }

            Play();
        }

        /// <summary>
        /// Воспроизведение файла
        /// </summary>
        public void Play()
        {
            if (_waveOutDevice == null)
            {
                return;
            }

            switch (_waveOutDevice.PlaybackState)
            {
                case PlaybackState.Stopped:
                    {
                        _waveOutDevice.Play();
                        RaiseEventPlay();
                    }
                    break;
                case PlaybackState.Paused:
                    {
                        _waveOutDevice.Play();
                        RaiseEventResume();
                    }
                    break;

                default:
                    return;
            }

            _waveOutDevice.Play();
            _timer.Start();

            RaiseEventProgress();
        }

        /// <summary>
        /// Пауза
        /// </summary>
        public void Pause()
        {
            if (_waveOutDevice == null)
            {
                return;
            }

            _waveOutDevice.Pause();
            _timer.Stop();
            RaiseEventPause();
        }

        /// <summary>
        /// Остановка
        /// </summary>
        public void Stop()
        {
            if (_waveOutDevice == null)
            {
                return;
            }

            _waveOutDevice.Stop();
            _timer.Stop();
            _reader.Position = 0;

            RaiseEventStop();
            RaiseEventProgress();
        }

        /// <summary>
        /// Генерация события "Play"
        /// </summary>
        private void RaiseEventPlay()
        {
            if (PlayStarted != null)
            {
                PlayStarted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Генерация события "Stop"
        /// </summary>
        private void RaiseEventStop()
        {
            if (PlayStopped != null)
            {
                PlayStopped(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Генерация события "Pause"
        /// </summary>
        private void RaiseEventPause()
        {
            if (PlayPaused != null)
            {
                PlayPaused(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Генерация события "Resume"
        /// </summary>
        private void RaiseEventResume()
        {
            if (PlayResumed != null)
            {
                PlayResumed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Генерация события "Progress"
        /// </summary>
        private void RaiseEventProgress()
        {
            if (PlayProgress != null)
            {
                PlayProgress(this, new AudioFileProgressArgs(CurrentTime, TotalTime));
            }
        }

        #endregion
    }
}