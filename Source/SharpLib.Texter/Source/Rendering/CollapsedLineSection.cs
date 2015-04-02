using SharpLib.Texter.Document;

namespace SharpLib.Texter.Rendering
{
    public sealed class CollapsedLineSection
    {
        private DocumentLine start, end;

        private readonly HeightTree heightTree;

#if DEBUG
        internal string ID;

        private static int nextId;
#else
		const string ID = "";
#endif

        internal CollapsedLineSection(HeightTree heightTree, DocumentLine start, DocumentLine end)
        {
            this.heightTree = heightTree;
            this.start = start;
            this.end = end;
#if DEBUG
            unchecked
            {
                ID = " #" + (nextId++);
            }
#endif
        }

        public bool IsCollapsed
        {
            get { return start != null; }
        }

        public DocumentLine Start
        {
            get { return start; }
            internal set { start = value; }
        }

        public DocumentLine End
        {
            get { return end; }
            internal set { end = value; }
        }

        public void Uncollapse()
        {
            if (start == null)
            {
                return;
            }

            heightTree.Uncollapse(this);
#if DEBUG
            heightTree.CheckProperties();
#endif

            start = null;
            end = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        public override string ToString()
        {
            return "[CollapsedSection" + ID + " Start=" + (start != null ? start.LineNumber.ToString() : "null")
                   + " End=" + (end != null ? end.LineNumber.ToString() : "null") + "]";
        }
    }
}