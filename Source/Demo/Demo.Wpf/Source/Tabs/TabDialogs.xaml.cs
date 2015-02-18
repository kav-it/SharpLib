using System.Windows;

using SharpLib.Wpf;
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

        private void ShowDialogButtonClick(object sender, RoutedEventArgs e)
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

        private void ShowPromtButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = WindowPromt.ShowText("Title", "value", "watermark", true);

            MessageBoxEx.ShowBlank("Caption", result);
        }

        #endregion
    }
}