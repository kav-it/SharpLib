namespace SharpLib.Log
{
    public abstract class TargetWithLayoutHeaderAndFooter : TargetWithLayout
    {
        #region Свойства

        [RequiredParameter]
        public override Layout Layout
        {
            get { return LayoutHeaderFooter.Layout; }
            set
            {
                if (value is LayoutWithHeaderAndFooter)
                {
                    base.Layout = value;
                }
                else if (LayoutHeaderFooter == null)
                {
                    LayoutHeaderFooter = new LayoutWithHeaderAndFooter
                    {
                        Layout = value
                    };
                }
                else
                {
                    LayoutHeaderFooter.Layout = value;
                }
            }
        }

        public Layout Footer
        {
            get { return LayoutHeaderFooter.Footer; }
            set { LayoutHeaderFooter.Footer = value; }
        }

        public Layout Header
        {
            get { return LayoutHeaderFooter.Header; }
            set { LayoutHeaderFooter.Header = value; }
        }

        private LayoutWithHeaderAndFooter LayoutHeaderFooter
        {
            get { return (LayoutWithHeaderAndFooter)base.Layout; }
            set { base.Layout = value; }
        }

        #endregion
    }
}