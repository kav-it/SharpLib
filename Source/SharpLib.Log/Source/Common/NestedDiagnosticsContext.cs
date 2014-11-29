using System;
using System.Collections.Generic;

using NLog.Internal;

namespace NLog
{
    public static class NestedDiagnosticsContext
    {
        #region ����

        private static readonly object dataSlot = ThreadLocalStorageHelper.AllocateDataSlot();

        #endregion

        #region ��������

        public static string TopMessage
        {
            get
            {
                Stack<string> stack = ThreadStack;
                if (stack.Count > 0)
                {
                    return stack.Peek();
                }
                return string.Empty;
            }
        }

        private static Stack<string> ThreadStack
        {
            get { return ThreadLocalStorageHelper.GetDataForSlot<Stack<string>>(dataSlot); }
        }

        #endregion

        #region ������

        public static IDisposable Push(string text)
        {
            Stack<string> stack = ThreadStack;
            int previousCount = stack.Count;
            stack.Push(text);
            return new StackPopper(stack, previousCount);
        }

        public static string Pop()
        {
            Stack<string> stack = ThreadStack;
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            return string.Empty;
        }

        public static void Clear()
        {
            ThreadStack.Clear();
        }

        public static string[] GetAllMessages()
        {
            return ThreadStack.ToArray();
        }

        #endregion

        #region ��������� �����: StackPopper

        private class StackPopper : IDisposable
        {
            #region ����

            private readonly int previousCount;

            private readonly Stack<string> stack;

            #endregion

            #region �����������

            public StackPopper(Stack<string> stack, int previousCount)
            {
                this.stack = stack;
                this.previousCount = previousCount;
            }

            #endregion

            #region ������

            void IDisposable.Dispose()
            {
                while (stack.Count > previousCount)
                {
                    stack.Pop();
                }
            }

            #endregion
        }

        #endregion
    }
}