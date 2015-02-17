using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpLib.Wpf.Controls
{
    internal class TreeListExEditTextBox : TextBox
    {
        #region ����

        private bool _commiting;

        #endregion

        #region ��������

        public TreeListExViewItem Item { get; set; }

        public TreeListExNode ListNode
        {
            get { return Item.ListNode; }
        }

        #endregion

        #region �����������

        static TreeListExEditTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListExEditTextBox), new FrameworkPropertyMetadata(typeof(TreeListExEditTextBox)));
        }

        public TreeListExEditTextBox()
        {
            Loaded += delegate { Init(); };
        }

        #endregion

        #region ������

        private void Init()
        {
            Text = ListNode.LoadEditText();
            Focus();
            SelectAll();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Commit();
            }
            else if (e.Key == Key.Escape)
            {
                ListNode.IsEditing = false;
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (ListNode.IsEditing)
            {
                Commit();
            }
        }

        private void Commit()
        {
            if (!_commiting)
            {
                _commiting = true;

                ListNode.IsEditing = false;
                if (!ListNode.SaveEditText(Text))
                {
                    Item.Focus();
                }
                ListNode.RaisePropertyChanged("Text");

                _commiting = false;
            }
        }

        #endregion
    }
}