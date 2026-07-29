// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/04/2016 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Height attribute. Currently used only internally. Customers still have separate attributes exposed.
    /// </summary>
    internal class Height : InternableComplexAttr, IComplexAttr
    {
        internal Height(HeightRule rule, int value)
        {
            Rule = rule;
            Value = value;
        }

        /// <summary>
        /// Specifies the rule for determining the height of an object.
        /// </summary>
        internal HeightRule Rule
        {
            get { return mRule; }
            set
            {
                NotifyChanging();

                mRule = value;

                if (value == HeightRule.Auto)
                    mValue = 0;
            }
        }

        /// <summary>
        /// Specifies the value for determining the height of an object.
        /// </summary>
        internal int Value
        {
            get
            {
                return (mRule == HeightRule.Auto) ? 0 : mValue;
            }
            set
            {
                NotifyChanging();

                if (value == 0)
                {
                    // Reset rule to Auto (Exactly can not be 0 and AtLeast with 0 is equal to Auto).
                    mRule = HeightRule.Auto;
                }
                else
                {
                    if (mRule == HeightRule.Auto)
                    {
                        // Not 0 value switches rule to AtLeast.
                        mRule = HeightRule.AtLeast;
                    }
                }

                mValue = value;
            }
        }

        public Height Clone()
        {
            return (Height)MemberwiseClone();
        }

        bool IComplexAttr.IsInheritedComplexAttr 
        {
            get { return false; }
        }

        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return (Height)MemberwiseClone();
        }

        public bool Equals(Height rhs)
        {
            return (mValue == rhs.mValue) && (mRule == rhs.mRule);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((Height)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (mValue.GetHashCode() * 397) ^ mRule.GetHashCode();
            }
        }

#if DEBUG
        public override string ToString()
        {
            switch (mRule)
            {
                case HeightRule.Auto:
                    return "(Auto)";

                case HeightRule.AtLeast:
                    return string.Format("(AtLeast, {0})", mValue);

                case HeightRule.Exactly:
                    return string.Format("(Exactly, {0})", mValue);

                default:
                    return "(?)";
            }
        }
#endif

        private HeightRule mRule;
        private int mValue;
    }
}
