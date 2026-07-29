// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2014 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies manual run width.
    /// </summary>
    /// <remarks>
    /// See ECMA 376. 17.3.2.14 fitText (Manual Run Width).
    /// </remarks>
    internal class FitText: IComplexAttr, ICustomEquality
    {
        internal FitText(int value, int id)
        {
            mValue = value;
            mId = id;
        }

        public FitText Clone()
        {
            return (FitText)MemberwiseClone();
        }

        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        public IComplexAttr DeepCloneComplexAttr()
        {
            return (FitText)MemberwiseClone();
        }

        /// <summary>
        /// Returns <b>true</b> if the current instance and the specified one have the same value.
        /// </summary>
        bool ICustomEquality.HasSameValue(ICustomEquality layout)
        {
            if ((layout == null) || (GetType() != layout.GetType()))
                return false;

            FitText rhs = (FitText)layout;

            // Id is not included because instances with different IDs should be collapsed if the width is equal.
            return (Value == rhs.Value);
        }

        public bool Equals(FitText rhs)
        {
            return (mValue == rhs.mValue) && (mId == rhs.mId);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (GetType() != obj.GetType())) return false;

            return Equals((FitText)obj);
        }

        public override int GetHashCode()
        {
            return mValue ^ mId;
        }

        internal int Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        internal int Id
        {
            get { return mId; }
            set { mId = value; }
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("({0}, 0x{1:x})", mValue, mId);
        }
#endif

        private int mValue;
        private int mId;
    }
}
