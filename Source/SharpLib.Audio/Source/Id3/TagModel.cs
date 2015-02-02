using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Id3Lib.Frames;

namespace Id3Lib
{
    internal class TagModel : Collection<FrameBase>
    {
        #region Поля

        private readonly TagExtendedHeader _tagExtendedHeader = new TagExtendedHeader();

        private readonly TagHeader _tagHeader = new TagHeader();

        #endregion

        #region Свойства

        public bool IsValid
        {
            get { return Count > 0; }
        }

        public TagHeader Header
        {
            get { return _tagHeader; }
        }

        public TagExtendedHeader ExtendedHeader
        {
            get { return _tagExtendedHeader; }
        }

        #endregion

        #region Методы

        protected override void InsertItem(int index, FrameBase item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.FrameId == null || item.FrameId.Length != 4)
            {
                throw new InvalidOperationException("The frame id is invalid");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, FrameBase item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.FrameId == null || item.FrameId.Length != 4)
            {
                throw new InvalidOperationException("The frame id is invalid");
            }
            base.SetItem(index, item);
        }

        public void AddRange(IEnumerable<FrameBase> frames)
        {
            if (frames == null)
            {
                throw new ArgumentNullException("frames");
            }

            foreach (var frame in frames)
            {
                Add(frame);
            }
        }

        public void UpdateSize()
        {
            if (!IsValid)
            {
                Header.TagSize = 0;
            }

            using (Stream stream = new MemoryStream())
            {
                TagManager.Serialize(this, stream);
            }
        }

        #endregion
    }
}