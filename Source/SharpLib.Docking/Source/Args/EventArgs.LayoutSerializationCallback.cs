using System.ComponentModel;

namespace SharpLib.Docking.Layout.Serialization
{
    public class LayoutSerializationCallbackEventArgs : CancelEventArgs
    {
        #region Свойства

        public LayoutContent Model { get; private set; }

        public object Content { get; set; }

        #endregion

        #region Конструктор

        public LayoutSerializationCallbackEventArgs(LayoutContent model, object previousContent)
        {
            Cancel = false;
            Model = model;
            Content = previousContent;
        }

        #endregion
    }
}