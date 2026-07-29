// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/01/2018 by Victor Chebotok

namespace Aspose.Bidi
{
    /// <summary>
    /// A read-only list of <see cref="BidiLevel"/> elements.
    /// </summary>
    public class BidiLevelList
    {
        /// <summary>
        /// Constructor. Creates an empty list.
        /// </summary>
        public BidiLevelList()
        {
            mLevels = new BidiLevel[0];
        }

        /// <summary>
        /// Constructor. Creates a list with one element.
        /// </summary>
        public BidiLevelList(BidiLevel level)
        {
            mLevels = new BidiLevel[] { level };
        }

        /// <summary>
        /// Constructor. Creates a read-only wrapper for an array.
        /// </summary>
        public BidiLevelList(BidiLevel[] levels)
        {
            mLevels = levels;
        }

        [JavaAttributes.JavaDelete("BidiLevel enum is int constant in Java already.")]
        public BidiLevelList(int[] levels)
        {
            mLevels = new BidiLevel[levels.Length];
            for (int i = 0; i < levels.Length; i++)
                mLevels[i] = (BidiLevel)levels[i];
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (other.GetType() != typeof(BidiLevelList))
            {
                return false;
            }
            return Equals((BidiLevelList)other);
        }

        internal bool Equals(BidiLevelList other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (mLevels.Length != other.mLevels.Length)
            {
                return false;
            }
            for (int i = 0; i < mLevels.Length; i++)
            {
                if (mLevels[i] != other.mLevels[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = -1220689852;
                for (int i = 0; i < mLevels.Length; i++)
                {
                    hashCode = (hashCode * -1521134295) + (int)mLevels[i];
                }
                return hashCode;
            }
        }

        public int Length
        {
            get { return mLevels.Length; }
        }

        public BidiLevel this[int index]
        {
            get { return mLevels[index]; }
        }

        private readonly BidiLevel[] mLevels;
    }
}
