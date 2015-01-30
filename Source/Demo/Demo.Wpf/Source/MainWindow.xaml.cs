
using System.Windows;

using SharpLib;

namespace DemoWpf
{
    public partial class MainWindow
    {
        #region Конструктор

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Title = SharpLibApp.Instance.Caption;
        }
    }
}