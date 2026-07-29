// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2012 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Paragraph line spacing attribute. Currently used only internally. Customers still have separate attributes exposed.
    /// </summary>
    internal class LineSpacing : InternableComplexAttr, IComplexAttr
    {
        internal LineSpacing(int value, LineSpacingRule rule)
        {
            mValue = value;
            mRule = rule;
        }

        /// <summary>
        /// Creates model default spacing.
        /// </summary>
        /// <returns></returns>
        public static LineSpacing CreateDefault()
        {
            LineSpacing defaultLineSpacing = (LineSpacing)ParaPr.FetchDefaultAttr(ParaAttr.LineSpacing);
            return defaultLineSpacing.Clone();
        }

        public LineSpacing Clone()
        {
            return (LineSpacing)MemberwiseClone();
        }

        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        public IComplexAttr DeepCloneComplexAttr()
        {
            return (LineSpacing)MemberwiseClone();
        }

        public bool Equals(LineSpacing rhs)
        {
            return (mValue == rhs.mValue) && (mRule == rhs.mRule);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((LineSpacing)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (mValue * 397) ^ (int) mRule;
            }
        }

        internal int Value
        {
            get { return mValue; }
            set
            {
                NotifyChanging();

                mValue = value;
            }
        }

        internal LineSpacingRule Rule
        {
            get { return mRule; }
            set
            {
                NotifyChanging();

                mRule = value;
            }
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("({0}, {1})", mValue, mRule);
        }
#endif

        private int mValue;
        private LineSpacingRule mRule;
    }
}
