// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2012 by Denis Darkin

using System;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Common base for all revisions.
    /// </summary>
    internal class RevisionBase
    {
        internal RevisionBase(string author, DateTime dateTime)
        {
            Author = author;
            DateTime = dateTime;
        }
        /// <summary>
        /// Cannot be null.
        /// </summary>
        internal string Author { get; set; }

        internal DateTime DateTime { get; set; }
    }
}
