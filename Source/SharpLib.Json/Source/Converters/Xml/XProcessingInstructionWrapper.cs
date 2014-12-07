using System.Xml.Linq;

namespace SharpLib.Json
{
    internal class XProcessingInstructionWrapper : XObjectWrapper
    {
        #region ��������

        private XProcessingInstruction ProcessingInstruction
        {
            get { return (XProcessingInstruction)WrappedNode; }
        }

        public override string LocalName
        {
            get { return ProcessingInstruction.Target; }
        }

        public override string Value
        {
            get { return ProcessingInstruction.Data; }
            set { ProcessingInstruction.Data = value; }
        }

        #endregion

        #region �����������

        public XProcessingInstructionWrapper(XProcessingInstruction processingInstruction)
            : base(processingInstruction)
        {
        }

        #endregion
    }
}