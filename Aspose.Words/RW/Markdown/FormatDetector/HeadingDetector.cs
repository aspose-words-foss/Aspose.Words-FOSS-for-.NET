// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown Heading detector.
    /// </summary>
    internal class HeadingDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Detects Heading block in a specified context.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            // Check for AtxHeading.
            if (mAtxHeadingRegex.IsMatch(context.Line))
            {
                Count++;
                return true;
            }

            // Check for SetextHeading.
            // If previous line contains features that breaks paragraph, then current line cannot be a SetextHeading.
            if (!context.IsPrevParaBroken && mSetextHeadingRegex.IsMatch(context.Line))
            {
                Count++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets markdown feature significance of the detector.
        /// </summary>
        protected override MarkdownFeatureSignificance Significance
        {
            get { return MarkdownFeatureSignificance.Low; }
        }

        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected override MarkdownFeatureType Type
        {
            get { return MarkdownFeatureType.Heading; }
        }

        /// <summary>
        /// The regex for matching SetextHeading blocks.
        /// </summary>
        private readonly Regex mSetextHeadingRegex = new Regex(@"^[ >]*(=+|-+)\s*$", RegexOptions.Compiled);
        
        /// <summary>
        /// The regex for matching AtxHeading blocks.
        /// </summary>
        private readonly Regex mAtxHeadingRegex = new Regex(@"^[ >]*\#{1,6}\s.*$", RegexOptions.Compiled);
    }
}
