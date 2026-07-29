// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown FencedCode block detector.
    /// </summary>
    /// <remarks>
    /// The FencedCode block starts and ends with ``` or ~~~.
    /// </remarks>
    internal class FencedCodeDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Detects a FencedCode block in a specified context.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            if (mIsFencedBlockStarted)
            {
                // Search for the FencedCode end.
                if (mFencedCodeEndRegex.IsMatch(context.Line) || context.EndOfFile)
                {
                    Count++;
                    mIsFencedBlockStarted = false;
                    return true;
                }
            }
            else
            {
                mIsFencedBlockStarted = mFencedCodeStartRegex.IsMatch(context.Line);

                // Check if FencedCode block is closed by EOF.
                if (mIsFencedBlockStarted && context.EndOfFile)
                {
                    Count++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets markdown feature significance of the detector.
        /// </summary>
        protected override MarkdownFeatureSignificance Significance
        {
            // WORDSNET-26236 Increased from Medium to Big.
            get { return MarkdownFeatureSignificance.Big; }
        }

        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected override MarkdownFeatureType Type
        {
            get { return MarkdownFeatureType.FencedCode; }
        }

        /// <summary>
        /// Indicates whether FencedCode block has been started.
        /// </summary>
        private bool mIsFencedBlockStarted;

        /// <summary>
        /// The regex for matching FencedCode block start.
        /// </summary>
        private readonly Regex mFencedCodeStartRegex = new Regex(@"(^\s{0,3}(>\s{0,4})?(([`]{3,}(?!`))|[~]{3,}))",
            RegexOptions.Compiled);

        /// <summary>
        /// The regex for matching FencedCode block end.
        /// </summary>
        private readonly Regex mFencedCodeEndRegex = new Regex(@"(^\s{0,3}(>\s{0,4})?([`]{3,}|[~]{3,})$)",
            RegexOptions.Compiled);
    }
}
