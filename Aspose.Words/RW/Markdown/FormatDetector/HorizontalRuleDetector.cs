// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2019 by Denis Panov

using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown HorizontalRule detector.
    /// </summary>
    internal class HorizontalRuleDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Detects HorizontalRule block in a specified context.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            // There cannot be a HorizontalRule inside a Heading.
            if (context.CurLineDetectedFeatures.Contains(MarkdownFeatureType.Heading))
                return false;

            if (mHorizontalRuleRegex.IsMatch(context.Line))
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
            get { return MarkdownFeatureSignificance.Medium; }
        }

        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected override MarkdownFeatureType Type
        {
            get { return MarkdownFeatureType.HorizontalRule; }
        }

        /// <summary>
        /// The regex for matching HorizontalRule blocks.
        /// </summary>
        private readonly Regex mHorizontalRuleRegex = new Regex(
            "^[ ]{0,3}" +       // Leading spaces at the beginning
                "([-*_])" +     // $1: First marker
                "(?>" +         // Repeated marker group
                   "[ ]{0,2}" + // Zero, one, or two spaces.
                    "\\1" +     // Marker character
                "){2,}" +       // Group repeated at least twice
                "[ ]*" +        // Trailing spaces
            "$"                 // End of line.
            , RegexOptions.Multiline | RegexOptions.Compiled);
    }
}
