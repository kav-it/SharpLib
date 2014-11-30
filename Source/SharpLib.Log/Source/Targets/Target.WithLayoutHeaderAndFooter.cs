namespace SharpLib.Log
{
    public abstract class TargetWithLayoutHeaderAndFooter : TargetWithLayout
    {
        #region Свойства

        [RequiredParameter]
        public override Layout Layout
        {
            get { return LHF.Layout; }

            set
            {
                if (value is LayoutWithHeaderAndFooter)
                {
                    base.Layout = value;
                }
                else if (LHF == null)
                {
                    LHF = new LayoutWithHeaderAndFooter
                    {
                        Layout = value
                    };
                }
                else
                {
                    LHF.Layout = value;
                }
            }
        }

        public Layout Footer
        {
            get { return LHF.Footer; }
            set { LHF.Footer = value; }
        }

        public Layout Header
        {
            get { return LHF.Header; }
            set { LHF.Header = value; }
        }

        private LayoutWithHeaderAndFooter LHF
        {
            get { return (LayoutWithHeaderAndFooter)base.Layout; }
            set { base.Layout = value; }
        }

        #endregion
    }
}