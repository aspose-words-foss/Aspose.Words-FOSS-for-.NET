// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// This element holds the properties associated with defining a bevel.
    /// </summary>
    internal class DmlBevel
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlBevel value = (DmlBevel)obj;

            return (value.BevelPresetType == BevelPresetType) &&
                   MathUtil.AreEqual(value.Height, Height) &&
                   MathUtil.AreEqual(value.Width, Width);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Height.GetHashCode();
            hash ^= Width.GetHashCode();
            hash ^= BevelPresetType.GetHashCode();
            return hash;
        }

        internal DmlBevel Clone()
        {
            return (DmlBevel)MemberwiseClone();
        }

        /// <summary>
        /// Specifies the height of the bevel, or how far above the shape it is applied.
        /// </summary>
        internal double Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        /// <summary>
        /// Specifies the width of the bevel, or how far into the shape it is applied.
        /// </summary>
        internal double Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Specifies the preset bevel type which defines the look of the bevel.
        /// Default <see cref="DmlBevelPresetType.Circle"/>.
        /// </summary>
        internal DmlBevelPresetType BevelPresetType
        {
            get { return mBevelPresetType; }
            set { mBevelPresetType = value; }
        }


        private double mHeight;
        private double mWidth;
        private DmlBevelPresetType mBevelPresetType = DmlBevelPresetType.Circle;
    }
}
