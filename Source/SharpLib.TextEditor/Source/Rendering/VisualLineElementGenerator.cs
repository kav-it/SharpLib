using System;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public abstract class VisualLineElementGenerator
    {
        #region Поля

        internal int cachedInterest;

        #endregion

        #region Свойства

        protected ITextRunConstructionContext CurrentContext { get; private set; }

        #endregion

        #region Методы

        public virtual void StartGeneration(ITextRunConstructionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            CurrentContext = context;
        }

        public virtual void FinishGeneration()
        {
            CurrentContext = null;
        }

        public abstract int GetFirstInterestedOffset(int startOffset);

        public abstract VisualLineElement ConstructElement(int offset);

        #endregion
    }
}