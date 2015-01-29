using System.ComponentModel;

namespace Ookii.Dialogs.Wpf
{
    public class TaskDialogRadioButton : TaskDialogItem
    {
        #region Поля

        private bool _checked;

        #endregion

        #region Свойства

        [Category("Appearance"), Description("Indicates whether the radio button is checked."), DefaultValue(false)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                if (value && Owner != null)
                {
                    foreach (TaskDialogRadioButton radioButton in Owner.RadioButtons)
                    {
                        if (radioButton != this)
                        {
                            radioButton.Checked = false;
                        }
                    }
                }
            }
        }

        protected override System.Collections.IEnumerable ItemCollection
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.RadioButtons;
                }
                return null;
            }
        }

        #endregion

        #region Конструктор

        public TaskDialogRadioButton()
        {
        }

        public TaskDialogRadioButton(IContainer container)
            : base(container)
        {
        }

        #endregion
    }
}