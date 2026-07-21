// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.1.3 normAutofit (Normal AutoFit)
    /// This element specifies that text within the text body should be normally auto-fit to the bounding box. 
    /// Auto-fitting is when text within a text box is scaled in order to remain inside the text box. 
    /// If this element is omitted, then noAutofit or auto-fit off is implied.
    /// </summary>
    internal class DmlNormalAutoFitMode : DmlAutoFitMode
    {
        /// <summary>
        /// Specifies the percentage of the original font size to which each run in the text body is scaled. 
        /// In order to auto-fit text within a bounding box it is sometimes necessary to decrease the font 
        /// size by a certain percentage. Using this attribute the font within a text box can be scaled based 
        /// on the value provided. A value of 100% scales the text to 100%, while a value of 1% scales the 
        /// text to 1%. If this attribute is omitted, then a value of 100% is implied.
        /// </summary>
        internal double FontScale
        {
            get { return mFontScale; }
            set { mFontScale = value; }
        }

        /// <summary>
        /// Space Reduction)
        /// Specifies the percentage amount by which the line spacing of each paragraph in the text body is reduced. 
        /// The reduction is applied by subtracting it from the original line spacing value. Using this attribute the 
        /// vertical spacing between the lines of text can be scaled by a percent amount. A value of 100% reduces the 
        /// line spacing by 100%, while a value of 1% reduces the line spacing by one percent. If this attribute is 
        /// omitted, then a value of 0% is implied.
        /// </summary>
        internal double LineSpaceReduction
        {
            get { return mLineSpaceReduction; }
            set { mLineSpaceReduction = value; }
        }

        private double mFontScale = 1.0f;
        private double mLineSpaceReduction = 0.0;
    }
}