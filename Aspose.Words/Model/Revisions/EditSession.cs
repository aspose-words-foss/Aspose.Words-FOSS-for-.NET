// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/05/2014 by Alexey Morozov

using System;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Represents an edit session i.e Author and DateTime stored with changes made while <see cref="Document.TrackRevisions"/> is set.
    /// </summary>
    internal class EditSession
    {
        /// <summary>
        /// Creates edit session using given author and time.
        /// </summary>
        internal EditSession(string author, DateTime dateTime)
        {
            mAuthor = author;
            mDateTime = dateTime;
        }

        /// <summary>
        /// Revision tracking author.
        /// </summary>
        internal string Author
        {
            get { return mAuthor; }
            set { mAuthor = value; }
        }

        /// <summary>
        /// Revision tracking date/time.
        /// </summary>
        internal DateTime DateTime
        {
            get { return mDateTime; }
        }

        private string mAuthor;
        private readonly DateTime mDateTime;
    }
}
