using System.Windows;

using SharpLib.Wpf.Dialogs;

namespace DemoWpf
{
    public partial class TabDialogs
    {
        #region Конструктор

        public TabDialogs()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        private void _showDialogButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_dialogComboBox.SelectedIndex)
            {
                case 0:
                    ShowFolderBrowserDialog();
                    break;
                case 1:
                    ShowOpenFileDialog();
                    break;
                case 2:
                    ShowSaveFileDialog();
                    break;
            }
        }

        private void ShowFolderBrowserDialog()
        {
            Dialog.SelectFolder("Please select a folder.");
        }

        private void ShowOpenFileDialog()
        {
            Dialog.OpenFile("All files (*.*)|*.*");
        }

        private void ShowSaveFileDialog()
        {
            Dialog.SaveFile("Text files (*.txt)|*.txt|All files (*.*)|*.*", "txt");
        }

        #endregion
    }
}