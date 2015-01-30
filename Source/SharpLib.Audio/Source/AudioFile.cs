using System;
using System.IO;

using NAudio.Wave;

namespace SharpLib.Audio
{
    /// <summary>
    /// Музыкальный файл с возможностью управления
    /// </summary>
    public class AudioFile: IDisposable
    {
        #region Поля

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

        #endregion

        #region Конструктор

        public AudioFile(string location)
        {
            Location = location;

            if (File.Exists(location) == false)
            {
                return;
            }

            _reader = new AudioFileReader(location);
            _waveOutDevice = new WaveOut();
            _waveOutDevice.Init(_reader);
        }

        ~AudioFile()
        {
            Dispose();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Освобождение элемента
        /// </summary>
        public void Dispose()
        {
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

            _waveOutDevice.Stop();
        }

        #endregion
    }
}