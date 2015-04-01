using System;
using System.IO;

namespace SharpLib.Notepad.Utils
{
    public static class CharRope
    {
        #region Методы

        public static Rope<char> Create(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            return new Rope<char>(InitFromString(text));
        }

        public static string ToString(this Rope<char> rope, int startIndex, int length)
        {
            if (rope == null)
            {
                throw new ArgumentNullException("rope");
            }
#if DEBUG
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value must be >= 0");
            }
#endif
            if (length == 0)
            {
                return string.Empty;
            }
            var buffer = new char[length];
            rope.CopyTo(startIndex, buffer, 0, length);
            return new string(buffer);
        }

        public static void WriteTo(this Rope<char> rope, TextWriter output, int startIndex, int length)
        {
            if (rope == null)
            {
                throw new ArgumentNullException("rope");
            }
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            rope.VerifyRange(startIndex, length);
            rope.root.WriteTo(startIndex, output, length);
        }

        public static void AddText(this Rope<char> rope, string text)
        {
            InsertText(rope, rope.Length, text);
        }

        public static void InsertText(this Rope<char> rope, int index, string text)
        {
            if (rope == null)
            {
                throw new ArgumentNullException("rope");
            }
            rope.InsertRange(index, text.ToCharArray(), 0, text.Length);
        }

        internal static RopeNode<char> InitFromString(string text)
        {
            if (text.Length == 0)
            {
                return RopeNode<char>.emptyRopeNode;
            }
            var node = RopeNode<char>.CreateNodes(text.Length);
            FillNode(node, text, 0);
            return node;
        }

        private static void FillNode(RopeNode<char> node, string text, int start)
        {
            if (node.contents != null)
            {
                text.CopyTo(start, node.contents, 0, node.length);
            }
            else
            {
                FillNode(node.left, text, start);
                FillNode(node.right, text, start + node.left.length);
            }
        }

        internal static void WriteTo(this RopeNode<char> node, int index, TextWriter output, int count)
        {
            if (node.height == 0)
            {
                if (node.contents == null)
                {
                    node.GetContentNode().WriteTo(index, output, count);
                }
                else
                {
                    output.Write(node.contents, index, count);
                }
            }
            else
            {
                if (index + count <= node.left.length)
                {
                    node.left.WriteTo(index, output, count);
                }
                else if (index >= node.left.length)
                {
                    node.right.WriteTo(index - node.left.length, output, count);
                }
                else
                {
                    int amountInLeft = node.left.length - index;
                    node.left.WriteTo(index, output, amountInLeft);
                    node.right.WriteTo(0, output, count - amountInLeft);
                }
            }
        }

        public static int IndexOfAny(this Rope<char> rope, char[] anyOf, int startIndex, int length)
        {
            if (rope == null)
            {
                throw new ArgumentNullException("rope");
            }
            if (anyOf == null)
            {
                throw new ArgumentNullException("anyOf");
            }
            rope.VerifyRange(startIndex, length);

            while (length > 0)
            {
                var entry = rope.FindNodeUsingCache(startIndex).PeekOrDefault();
                var contents = entry.node.contents;
                int startWithinNode = startIndex - entry.nodeStartIndex;
                int nodeLength = Math.Min(entry.node.length, startWithinNode + length);
                for (int i = startIndex - entry.nodeStartIndex; i < nodeLength; i++)
                {
                    char element = contents[i];
                    foreach (char needle in anyOf)
                    {
                        if (element == needle)
                        {
                            return entry.nodeStartIndex + i;
                        }
                    }
                }
                length -= nodeLength - startWithinNode;
                startIndex = entry.nodeStartIndex + nodeLength;
            }
            return -1;
        }

        public static int IndexOf(this Rope<char> rope, string searchText, int startIndex, int length, StringComparison comparisonType)
        {
            if (rope == null)
            {
                throw new ArgumentNullException("rope");
            }
            if (searchText == null)
            {
                throw new ArgumentNullException("searchText");
            }
            rope.VerifyRange(startIndex, length);
            int pos = rope.ToString(startIndex, length).IndexOf(searchText, comparisonType);
            if (pos < 0)
            {
                return -1;
            }
            return pos + startIndex;
        }

        public static int LastIndexOf(this Rope<char> rope, string searchText, int startIndex, int length, StringComparison comparisonType)
        {
            if (rope == null)
            {
                throw new ArgumentNullException("rope");
            }
            if (searchText == null)
            {
                throw new ArgumentNullException("searchText");
            }
            rope.VerifyRange(startIndex, length);
            int pos = rope.ToString(startIndex, length).LastIndexOf(searchText, comparisonType);
            if (pos < 0)
            {
                return -1;
            }
            return pos + startIndex;
        }

        #endregion
    }
}