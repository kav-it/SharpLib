namespace SharpLib.Log
{
    [LayoutRenderer("rot13")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public sealed class Rot13LayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Ñגמיסעגא

        public Layout Text
        {
            get { return Inner; }
            set { Inner = value; }
        }

        #endregion

        #region Ìועמהû

        public static string DecodeRot13(string encodedValue)
        {
            if (encodedValue == null)
            {
                return null;
            }

            char[] chars = encodedValue.ToCharArray();
            for (int i = 0; i < chars.Length; ++i)
            {
                chars[i] = DecodeRot13Char(chars[i]);
            }

            return new string(chars);
        }

        protected override string Transform(string text)
        {
            return DecodeRot13(text);
        }

        private static char DecodeRot13Char(char c)
        {
            if (c >= 'A' && c <= 'M')
            {
                return (char)('N' + (c - 'A'));
            }

            if (c >= 'a' && c <= 'm')
            {
                return (char)('n' + (c - 'a'));
            }

            if (c >= 'N' && c <= 'Z')
            {
                return (char)('A' + (c - 'N'));
            }

            if (c >= 'n' && c <= 'z')
            {
                return (char)('a' + (c - 'n'));
            }

            return c;
        }

        #endregion
    }
}