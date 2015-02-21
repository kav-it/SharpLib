using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Markup;

namespace SharpLib.Docking.Layout
{
    [ContentProperty("RootDocument")]
    [Serializable]
    public class LayoutDocumentFloatingWindow : LayoutFloatingWindow
    {
        #region Поля

        private LayoutDocument _rootDocument;

        #endregion

        #region Свойства

        public LayoutDocument RootDocument
        {
            get { return _rootDocument; }
            set
            {
                if (!Equals(_rootDocument, value))
                {
                    RaisePropertyChanging("RootDocument");
                    _rootDocument = value;
                    if (_rootDocument != null)
                    {
                        _rootDocument.Parent = this;
                    }
                    RaisePropertyChanged("RootDocument");

                    if (RootDocumentChanged != null)
                    {
                        RootDocumentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public override IEnumerable<ILayoutElement> Children
        {
            get
            {
                if (RootDocument == null)
                {
                    yield break;
                }

                yield return RootDocument;
            }
        }

        public override int ChildrenCount
        {
            get { return RootDocument != null ? 1 : 0; }
        }

        public override bool IsValid
        {
            get { return RootDocument != null; }
        }

        #endregion

        #region События

        public event EventHandler RootDocumentChanged;

        #endregion

        #region Методы

        public override void RemoveChild(ILayoutElement element)
        {
            Debug.Assert(Equals(element, RootDocument) && element != null);
            RootDocument = null;
        }

        public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            Debug.Assert(Equals(oldElement, RootDocument) && oldElement != null);
            RootDocument = newElement as LayoutDocument;
        }

        public override void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("FloatingDocumentWindow()");

            RootDocument.ConsoleDump(tab + 1);
        }

        #endregion
    }
}