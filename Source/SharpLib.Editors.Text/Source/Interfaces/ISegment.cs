namespace SharpLib.Notepad.Document
{
    public interface ISegment
    {
        #region ��������

        int Offset { get; }

        int Length { get; }

        int EndOffset { get; }

        #endregion
    }
}