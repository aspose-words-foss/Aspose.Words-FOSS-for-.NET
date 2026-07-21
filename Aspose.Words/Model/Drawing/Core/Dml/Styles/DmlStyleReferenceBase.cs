// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Styles
{
    /// <summary>
    /// Represents the base class for style references.
    /// </summary>
    internal abstract class DmlStyleReferenceBase
    {
        /// <summary>
        /// Color.
        /// </summary>
        internal DmlColor Color
        {
            get { return mColor; }
            set { mColor = value; }
        }

        /// <summary>
        /// Clones this style reference instance.
        /// </summary>
        internal virtual DmlStyleReferenceBase Clone()
        {
            DmlStyleReferenceBase lhs = (DmlStyleReferenceBase)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            return lhs;
        }

        /// <summary>
        /// Returns true, when this style reference is empty.
        /// </summary>
        internal bool IsEmpty
        {
            get { return (mColor == null); }
        }
        
        private DmlColor mColor;
    }
}
