﻿using System;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace SharpLib.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorGroup : LayoutGroup<LayoutAnchorable>, ILayoutPreviousContainer, ILayoutPaneSerializable
    {
        #region Поля

        private string _id;

        [field: NonSerialized]
        private ILayoutContainer _previousContainer;

        #endregion

        #region Свойства

        [XmlIgnore]
        ILayoutContainer ILayoutPreviousContainer.PreviousContainer
        {
            get { return _previousContainer; }
            set
            {
                if (_previousContainer != value)
                {
                    _previousContainer = value;
                    RaisePropertyChanged("PreviousContainer");
                    var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                    if (paneSerializable != null &&
                        paneSerializable.Id == null)
                    {
                        paneSerializable.Id = Guid.NewGuid().ToString();
                    }
                }
            }
        }

        string ILayoutPaneSerializable.Id
        {
            get { return _id; }
            set { _id = value; }
        }

        string ILayoutPreviousContainer.PreviousContainerId { get; set; }

        #endregion

        #region Методы

        protected override bool GetVisibility()
        {
            return Children.Count > 0;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (_id != null)
            {
                writer.WriteAttributeString("Id", _id);
            }
            if (_previousContainer != null)
            {
                var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                if (paneSerializable != null)
                {
                    writer.WriteAttributeString("PreviousContainerId", paneSerializable.Id);
                }
            }

            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Id"))
            {
                _id = reader.Value;
            }
            if (reader.MoveToAttribute("PreviousContainerId"))
            {
                ((ILayoutPreviousContainer)this).PreviousContainerId = reader.Value;
            }

            base.ReadXml(reader);
        }

        #endregion
    }
}