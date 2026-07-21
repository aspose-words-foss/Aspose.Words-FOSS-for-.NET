// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System;
using System.Text.RegularExpressions;
using Aspose.Collections;
using Aspose.Collections.Generic;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown Link detector. 
    /// </summary>
    internal class LinkDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Detects a Link block in a specified context.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            int linksCount = 0;
            foreach (Match match in mLinkRegex.Matches(context.Line))
            {
                if (match.Value.EndsWith(")", StringComparison.Ordinal))
                {
                    // This is Inline Link, so just count it.
                    linksCount++;
                }
                else
                {
                    string linkDestination = GetLinkDestination(match.Value);
                    
                    int count = mLinkLabels[linkDestination];
                    
                    if (match.Value.EndsWith(">", StringComparison.Ordinal))
                    {
                        // If we encounter a link definition, then add it and check accumulated link labels.
                        mLinkDefs.Add(linkDestination);

                        if (!StringToIntDictionary.IsNullSubstitute(count))
                        {
                            // Now we have a corresponding link definition.
                            // So, if there are link labels with such destination, then count them.
                            linksCount += count;
                            mLinkLabels[linkDestination] = 0;
                        }
                    }
                    else
                    {
                        // If we encounter a link label, then count it if we have a corresponding link definition.
                        // Otherwise accumulate it for further proceeding.
                        if (mLinkDefs.Contains(linkDestination))
                            linksCount++;
                        else
                            mLinkLabels[linkDestination] = StringToIntDictionary.IsNullSubstitute(count) ? 1 : (count + 1);
                    }
                }
            }

            Count += linksCount;

            return (linksCount > 0);
        }

        /// <summary>
        /// Gets a link destination substring from a specified link.
        /// </summary>
        private static string GetLinkDestination(string link)
        {
            // Link destination should be inside a last [].
            int startIdx = link.LastIndexOf('[');
            int endIdx = link.LastIndexOf(']');

            Debug.Assert((startIdx != -1) && (endIdx != -1));

            // Strip '[' and ']' characters.
            string linkDestination = link.Substring(startIdx + 1, endIdx - startIdx - 1);

            // Normalize by trimming whitespace characters and converting it to a lower case.
            return linkDestination.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Gets markdown feature significance of the detector.
        /// </summary>
        protected override MarkdownFeatureSignificance Significance
        {
            get { return MarkdownFeatureSignificance.Big; }
        }

        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected override MarkdownFeatureType Type
        {
            get { return  MarkdownFeatureType.Link; }
        }

        /// <summary>
        /// The regex for all types of links (inline, full reference, shortcut).
        /// </summary>
        /// <remarks>
        /// It matches the following:
        /// []() - Inline links
        /// [][] - Full links | [] - Shortcut links 
        /// []:&lt;&gt; - Link destination.
        /// </remarks>
        private readonly Regex mLinkRegex = new Regex(
            @"\[[^\[\]]+\]\([^\(\)]+\) |  
              (\[[^\[\]]+\]){1,2}(?!:) |
              (\[[^\[\]]+\]):\<.*\>", 
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// The dictionary with link labels and their counts.
        /// </summary>
        private readonly StringToIntDictionary mLinkLabels = new StringToIntDictionary();

        /// <summary>
        /// The hashset with link definitions.
        /// </summary>
        private readonly HashSetGeneric<string> mLinkDefs = new HashSetGeneric<string>();

    }
}
