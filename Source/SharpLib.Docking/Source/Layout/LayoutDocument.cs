using System;
using System.Linq;

namespace SharpLib.Docking
{
    [Serializable]
    public class LayoutDocument : LayoutContent
    {
        #region Поля

        private string _description;

        #endregion

        #region Свойства

        public bool IsVisible
        {
            get { return true; }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        #endregion

        #region Методы

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WriteAttributeString("Description", Description);
            }

            WriteXmlInternalDesc(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Description"))
            {
                Description = reader.Value;
            }

            ReadXmlInternalDesc(reader);

            base.ReadXml(reader);
        }

        public override void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("Document()");
        }

        protected override void InternalDock()
        {
            var root = Root as LayoutRoot;
            LayoutDocumentPane documentPane = null;
            if (root != null && (root.LastFocusedDocument != null && !Equals(root.LastFocusedDocument, this)))
            {
                documentPane = root.LastFocusedDocument.Parent as LayoutDocumentPane;
            }

            if (documentPane == null)
            {
                documentPane = root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }

            bool added = false;
            if (root != null && root.Manager.LayoutUpdateStrategy != null)
            {
                added = root.Manager.LayoutUpdateStrategy.BeforeInsertDocument(root, this, documentPane);
            }

            if (!added)
            {
                if (documentPane == null)
                {
                    throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
                }

                documentPane.Children.Add(this);
                added = true;
            }

            if (root != null && root.Manager.LayoutUpdateStrategy != null)
            {
                root.Manager.LayoutUpdateStrategy.AfterInsertDocument(root, this);
            }

            base.InternalDock();
        }

        #endregion
    }
}