namespace SharpLib.Notepad.Document
{
    public static class SegmentExtensions
    {
        #region Ìועמהû

        public static bool Contains(this ISegment segment, int offset, int length)
        {
            return segment.Offset <= offset && offset + length <= segment.EndOffset;
        }

        public static bool Contains(this ISegment thisSegment, ISegment segment)
        {
            return segment != null && thisSegment.Offset <= segment.Offset && segment.EndOffset <= thisSegment.EndOffset;
        }

        #endregion
    }
}