// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.16 clrChange (Color Change Effect)
    /// This element specifies a Color Change Effect.
    /// Instances of clrFrom are replaced with instances of clrTo.
    /// </summary>
    internal class DmlColorChangeEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.ColorChange; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlColorChangeEffect value = (DmlColorChangeEffect)obj;

            return (value.ConsiderAlphaValues == ConsiderAlphaValues) &&
                   object.Equals(value.SourceColor, SourceColor) &&
                   object.Equals(value.DestinationColor, DestinationColor);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (SourceColor != null)
                hash ^= SourceColor.GetHashCode();
            if (DestinationColor != null)
                hash ^= DestinationColor.GetHashCode();
            hash ^= ConsiderAlphaValues.GetHashCode();
            return hash;
        }

        internal DmlColor SourceColor
        {
            get { return mSourceColor; }
            set { mSourceColor = value; }
        }

        internal DmlColor DestinationColor
        {
            get { return mDestinationColor; }
            set { mDestinationColor = value; }
        }

        /// <summary>
        /// Specifies whether alpha values are considered for the effect. 
        /// Effect alpha values are considered if useA is true, else they are ignored.
        /// </summary>
        internal bool ConsiderAlphaValues
        {
            get { return mConsiderAlphaValues; }
            set { mConsiderAlphaValues = value; }
        }

        private DmlColor mSourceColor;
        private DmlColor mDestinationColor;
        private bool mConsiderAlphaValues;
    }
}
