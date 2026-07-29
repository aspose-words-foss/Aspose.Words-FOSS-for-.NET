// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2011 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies layout information for text in East Asian languages, as well as the text that should be considered part of the same layout unit.
    /// See [MS-DOC] 2.9.68 FarEastLayoutOperand.
    /// </summary>
    /// <remarks>
    /// AM. I adopted names to DOCX/WML specification rather than DOC as they are more readable.
    /// </remarks>
    internal class FarEastLayout : IComplexAttr, ICustomEquality
    {
        private CombineBrackets mCombineBrackets = CombineBrackets.Default;

        /// <summary>
        /// Specifies if the text displays horizontally within vertical text, or vertically within horizontal text.
        /// The text is rendered with a 90-degree rotation to the left from all other contents of the containing line,
        /// while keeping the text on the same line as all other text in the paragraph.
        /// </summary>
        internal bool Vertical { get; set; }

        /// <summary>
        /// Specifies that the text displays on a single line by creating two sub-lines within the regular line, and laying out this text equally between those sub-lines.
        /// </summary>
        internal bool Combine { get; set; }

        /// <summary>
        /// Specifies whether the two sub-lines within one line are enclosed within a pair of brackets when displayed, and the type of brackets that are displayed.
        /// </summary>
        internal CombineBrackets CombineBrackets
        {
            get { return mCombineBrackets; }
            set { mCombineBrackets = value; }
        }

        /// <summary>
        /// Specifies whether other Sprm structures were applied that cause the text to be scaled to fit within the existing line.
        /// A value of 0x1 means that other Sprm structures were applied. A value of 0x0 means that they were not.
        /// </summary>
        internal bool VerticalCompress { get; set; }

        /// <summary>
        /// Specifies whether the corresponding text is in the same layout unit as other text.
        /// If two adjacent text runs have the same lFELayoutID value applied to them, they are laid out together.
        /// </summary>
        internal int FarEastLayoutId { get; set; }

        // According to spec these must be ignored.
        // fKumimoji
        // fRuby
        // fLSFitText
        // fVRuby
        // fWarichuNoOpenBracket
        // fTNYFetchTxm
        // fCellFitText

        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return false; }
        }

        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return (FarEastLayout)MemberwiseClone();
        }

        /// <summary>
        /// Returns <b>true</b> if the current layout and the specified one have the same value.
        /// </summary>
        bool ICustomEquality.HasSameValue(ICustomEquality layout)
        {
            if ((layout == null) || (GetType() != layout.GetType()))
                return false;

            FarEastLayout rhs = (FarEastLayout)layout;

            // FarEastLayoutId is not included because layouts with different IDs should be collapsed if the other
            // properties are equal.
            return
                (Vertical == rhs.Vertical) &&
                (Combine == rhs.Combine) &&
                (!Vertical || (VerticalCompress == rhs.VerticalCompress)) &&
                (!Combine || (CombineBrackets == rhs.CombineBrackets));
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (GetType() != obj.GetType())) return false;

            return Equals((FarEastLayout)obj);
        }

        public bool Equals(FarEastLayout rhs)
        {
            return (Vertical == rhs.Vertical) &&
                   (Combine == rhs.Combine) &&
                   (VerticalCompress == rhs.VerticalCompress) &&
                   (CombineBrackets == rhs.CombineBrackets) &&
                   (FarEastLayoutId == rhs.FarEastLayoutId);
        }

        public override int GetHashCode()
        {
            int hashCode = Vertical.GetHashCode();
            hashCode = (hashCode * 397) ^ Combine.GetHashCode();
            hashCode = (hashCode * 397) ^ CombineBrackets.GetHashCode();
            hashCode = (hashCode * 397) ^ VerticalCompress.GetHashCode();
            hashCode = (hashCode * 397) ^ FarEastLayoutId.GetHashCode();
            return hashCode;
        }
    }
}
