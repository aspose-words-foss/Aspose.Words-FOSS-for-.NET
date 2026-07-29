// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.47 pattFill (Pattern Fill)
    /// This element specifies a pattern fill. A repeated pattern is used to fill the object.
    /// </summary>
    internal class DmlPatternFill : DmlFill
    {
        /// <summary>
        /// Set style color in color placeholders used  in the fill.
        /// </summary>
        public override void ApplyStyleColor(DmlColor styleColor)
        {
            ForegroundColor.ApplyStyleColor(styleColor);
            BackgroundColor.ApplyStyleColor(styleColor);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlPatternFill value = (DmlPatternFill)obj;

            return (value.FillPresetPattern == FillPresetPattern) &&
                   object.Equals(value.BackgroundColor, BackgroundColor) &&
                   object.Equals(value.ForegroundColor, ForegroundColor);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= FillPresetPattern.GetHashCode();
            hash ^= BackgroundColor.GetHashCode();
            hash ^= ForegroundColor.GetHashCode();
            return hash;
        }

        internal override DmlFill Clone()
        {
            DmlPatternFill result = new DmlPatternFill();
            result.FillPresetPattern = FillPresetPattern;
            result.BackgroundColor = BackgroundColor.Clone();
            result.ForegroundColor = ForegroundColor.Clone();
            return result;
        }

        /// <summary>
        /// Gets or sets the type of fill.
        /// </summary>
        public override FillTypeCore FillType
        {
            get { return FillTypeCore.Pattern; }
        }

        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.PatternFill; }
        }

        internal PatternType FillPresetPattern
        {
            get { return mFillPresetPattern; }
            set { mFillPresetPattern = value; }
        }

        internal DmlColor BackgroundColor
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mBackgroundColor == null)
                    mBackgroundColor = new DmlHexRgbColor();
                return mBackgroundColor;
            }
            set { mBackgroundColor = value; }
        }

        internal DmlColor ForegroundColor
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mForegroundColor == null)
                    mForegroundColor = new DmlHexRgbColor();
                return mForegroundColor;
            }
            set { mForegroundColor = value; }
        }

        /// <summary>
        /// Gets or sets foreground color of the fill.
        /// </summary>
        internal override DmlColor DmlColorInternal
        {
            get { return ForegroundColor; }
            set { ForegroundColor = value; }
        }

        /// <summary>
        /// Gets or sets background color of the fill.
        /// </summary>
        internal override DmlColor DmlColor2Internal
        {
            get { return BackgroundColor; }
            set { BackgroundColor = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlColor mBackgroundColor;
        private PatternType mFillPresetPattern;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlColor mForegroundColor;
    }
}
