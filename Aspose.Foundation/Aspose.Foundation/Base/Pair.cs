// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/06/2015 by Alexey Morozov

namespace Aspose
{
    /// <summary>
    /// Simple class to hold pair of object.
    /// </summary>
    /// <remarks>
    /// This class is intended to be transparent to comparison and compares contained objects instead of self.
    /// </remarks>
    public sealed class Pair
    {
        public Pair(object first, object second)
        {
            mFirst = first;
            mSecond = second;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Pair rhs = obj as Pair;
            if (rhs == null)
                return false;

            return Equals(rhs);
        }

        public bool Equals(Pair rhs)
        {
            if (rhs == null)
                return false;

            return ReferenceEquals(mFirst, rhs.mFirst) && ReferenceEquals(mSecond, rhs.mSecond);
        }

        public override int GetHashCode()
        {
            return mFirst.GetHashCode() ^ mSecond.GetHashCode();
        }

        public object First
        {
            get { return mFirst; }
        }


        public object Second
        {
            get { return mSecond; }
        }

        private readonly object mFirst;
        private readonly object mSecond;
    }
}
