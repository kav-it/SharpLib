using System.Reflection;
using System.Windows;

using SharpLib;
using SharpLib.Wpf;

namespace DemoWpf
{
    public partial class TabAudio
    {
        #region Конструктор

        public TabAudio()
        {
            InitializeComponent();

            // Копирование файла в исполняемую директорию
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFileEx("Source/Assets/Music.mp3", false);
        }

        #endregion

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Методы


        #endregion
    }
}