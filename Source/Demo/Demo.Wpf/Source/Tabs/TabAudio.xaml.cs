using System.Reflection;
using System.Windows;

using SharpLib;
using SharpLib.Audio;

namespace DemoWpf
{
    public partial class TabAudio
    {
        #region Поля

        private readonly AudioFile _file;

        #endregion

        #region Конструктор

        public TabAudio()
        {
            InitializeComponent();

            // Копирование файла в исполняемую директорию
            var asm = Assembly.GetExecutingAssembly();
            asm.CopyEmbeddedResourceToFileEx("Source/Assets/Music.mp3", false);

            _file = new AudioFile(asm.GetDirectoryEx() + "\\Music.mp3");
        }

        #endregion

        #region Методы

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            _file.Play();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            _file.Stop();
        }

        #endregion
    }
}