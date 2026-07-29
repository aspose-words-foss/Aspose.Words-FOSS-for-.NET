// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.
// Copyright (C) 2007-2009 Itai Bar-Haim
// 
// Dowloaded from: http://sourceforge.net/projects/nbidi/ 
// version 1.2.1.1

using System.Collections.Generic;
using System.Text;

namespace Aspose.Bidi
{
    /// <summary>
    /// The main class that implements the BIDI algorithm.
    /// </summary>
    /// <remarks>
    /// Note that the BiDi algorithm does not cover reordering a visual string to logical form.
    /// Some times the very same algorithm will do the job, but in more complex situations it'll fail.
    /// The correct way is to always work in logical form in memory, and only run the BiDi algorithm to display the results on the screen.
    /// </remarks>
    /// <example>
    /// <c>string visual = NBidi.NBidi.LogicalToVisual(string logical)</c>
    /// </example>

    public static class NBidi
    {
        /// <summary>
        /// Implementation of the BIDI algorithm, as described in http://www.unicode.org/reports/tr9/tr9-17.html
        /// </summary>
        /// <param name="logicalString">The original logical-ordered string.</param>
        /// <returns>The visual representation of the string.</returns>
        public static string LogicalToVisual(string logicalString)
        {
            IList<BidiParagraph> pars = BidiParagraph.SplitStringToParagraphs(logicalString);
            StringBuilder sb = new StringBuilder();
            foreach (BidiParagraph p in pars)
                sb.Append(p.BidiText);

            return sb.ToString();
        }
    }
}
