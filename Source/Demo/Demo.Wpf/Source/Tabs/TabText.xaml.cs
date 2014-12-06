namespace DemoWpf
{
    public partial class TabText
    {
        #region Конструктор

        public TabText()
        {
            InitializeComponent();
        }

        #endregion

        private void ButtonEx_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PART_passwordEdit.ShowPassword = !PART_passwordEdit.ShowPassword;
        }
    }
}