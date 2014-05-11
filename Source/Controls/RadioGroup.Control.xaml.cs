using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SharpLib.Controls
{
    [ContentProperty("Children")]
    public partial class RadioGroup
    {
        #region Fields

        public static readonly DependencyProperty HeaderProperty;

        #endregion

        #region Constructors

        static RadioGroup()
        {
            HeaderProperty = DependencyProperty.Register("Header", typeof (string), typeof (RadioGroup), new PropertyMetadata("Header"));
        }

        public RadioGroup()
        {
            InitializeComponent();

            DataContext = this;
        }

        #endregion

        #region Properties

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public List<RadioBox> Items
        {
            get { return Children.OfType<RadioBox>().ToList(); }
        }

        public RadioBox SelectedItem
        {
            get { return Items.FirstOrDefault(x => x.IsChecked == true); }
            set
            {
                if (value != null)
                {
                    value.IsChecked = true;
                }
            }
        }

        public int SelectedIndex
        {
            get { return SelectedItem != null ? SelectedItem.Index : -1; }
            set
            {
                var item = Items.FirstOrDefault(x => x.Index == value);

                SelectedItem = item;
            }
        }

        public new UIElementCollection Children
        {
            get { return PART_stackPanel.Children; }
        }

        #endregion
    }

    public class RadioBox : RadioButton
    {
        #region Properties

        public int Index { get; set; }

        public string Group
        {
            get { return GroupName; }

            set { GroupName = value; }
        }

        #endregion
    }
}