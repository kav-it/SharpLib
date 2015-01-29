using System;
using System.ComponentModel.Design;

namespace Ookii.Dialogs.Wpf
{
    internal class TaskDialogDesigner : ComponentDesigner
    {
        #region ��������

        public override DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection verbs = new DesignerVerbCollection();
                verbs.Add(new DesignerVerb("Preview", Preview));
                return verbs;
            }
        }

        #endregion

        #region ������

        private void Preview(object sender, EventArgs e)
        {
            ((TaskDialog)Component).ShowDialog();
        }

        #endregion
    }
}