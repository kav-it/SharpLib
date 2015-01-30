using System;
using System.Diagnostics;
using System.IO;

using SharpLib.Audio.Wave;

namespace SharpLib.Audio
{
    /// <summary>
    /// Музыкальный файл с возможностью управления
    /// </summary>
    public class AudioFile: IDisposable
    {
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
        /// Плеер
        /// </summary>
        private IWavePlayer _waveOutDevice;

        /// <summary>
        /// Состояние процесса воспроизведения
        /// </summary>
        public PlaybackState State
        {
            get { return _waveOutDevice.PlaybackState; }
        }

        #endregion

        #region Свойства

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
        }

        /// <summary>
        /// Общее время воспроизведения
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return _reader.TotalTime; }
        }

        /// <summary>
        /// Громкость 
        /// </summary>
        public float Volume
        {
            get { return _reader.Volume; }
        }

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

            _isDisposed = true;

            _reader.Volume
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
                case PlaybackState.Stopped: _waveOutDevice.Play(); break;
                case PlaybackState.Paused: _waveOutDevice.Play(); break;
                    
            }
            if (_waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                return;
            }


            _waveOutDevice.Play();
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

            // _waveOutDevice.Pause();
            _waveOutDevice.Stop();
            _reader.Position = 0;
        }

        #endregion
    }
}