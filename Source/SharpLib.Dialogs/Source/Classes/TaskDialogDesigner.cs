using System;
using System.ComponentModel.Design;

namespace Ookii.Dialogs.Wpf
{
    internal class TaskDialogDesigner : ComponentDesigner
    {
        #region Ñגמיסעגא

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

        #region Ìועמהû

        private void Preview(object sender, EventArgs e)
        {
            ((TaskDialog)Component).ShowDialog();
        }

        #endregion
    }
}