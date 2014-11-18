using System.Windows;

namespace DemoWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Конструктор

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window1();

            window.Show();
        }

        #endregion
    }
}