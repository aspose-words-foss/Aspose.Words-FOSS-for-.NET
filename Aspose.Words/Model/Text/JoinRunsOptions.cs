// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2026 by Vadim Saltykov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Provides configuration flags for the join runs operation.
    /// </summary>
    public class JoinRunsOptions
    {
        /// <summary>
        /// True indicates that the spacing attributes of all runs will be ignored when joining runs with same formatting.
        /// </summary>
        /// <remarks>
        /// The default value is False.
        /// </remarks>
        public bool IgnoreSpacing { get; set; }

        /// <summary>
        /// True indicates that the redundant attributes of all runs will be ignored when joining runs with same formatting.
        /// </summary>
        /// <remarks>
        /// Redundant attributes are those attributes that do not affect the run with the given text content.
        /// The default value is False.
        /// </remarks>
        public bool IgnoreRedundant { get; set; }

        /// <summary>
        /// True indicates that the insignificant attributes of all runs will be ignored when joining runs with same formatting.
        /// </summary>
        /// <remarks>
        /// Insignificant attributes are those attributes that do not have a noticeable effect on the formatting of a run with the given text content.
        /// The default value is False.
        /// </remarks>
        public bool IgnoreInsignificant { get; set; }
    }
}
