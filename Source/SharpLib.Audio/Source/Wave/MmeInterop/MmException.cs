using System;

namespace NAudio
{
    internal class MmException : Exception
    {
        #region ����

        private readonly MmResult result;

        private string function;

        #endregion

        #region ��������

        public MmResult Result
        {
            get { return result; }
        }

        #endregion

        #region �����������

        public MmException(MmResult result, string function)
            : base(MmException.ErrorMessage(result, function))
        {
            this.result = result;
            this.function = function;
        }

        #endregion

        #region ������

        private static string ErrorMessage(MmResult result, string function)
        {
            return String.Format("{0} calling {1}", result, function);
        }

        public static void Try(MmResult result, string function)
        {
            if (result != MmResult.NoError)
            {
                throw new MmException(result, function);
            }
        }

        #endregion
    }
}