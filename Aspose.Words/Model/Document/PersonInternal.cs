// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2016 by Alexander Zhiltsov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Stores contact information for an author of a comment or revision in a document.
    /// </summary>
    /// <remarks>
    /// Using identity information that is stored in the class, MS Word gets and displays additional information about
    /// author of a comment/revision.
    /// </remarks>
    internal class PersonInternal
    {
        /// <summary>
        /// Makes a deep copy of the person.
        /// </summary>
        internal PersonInternal Clone()
        {
            return (PersonInternal)MemberwiseClone();
        }

        /// <summary>
        /// Gets/sets author name to which this person is associated.
        /// </summary>
        /// <remarks>
        /// By this property a person is mapped to a comment or revision. It equals to <see cref="Comment.Author"/> or
        /// <see cref="RevisionBase.Author"/>.
        /// </remarks>
        internal string Author
        {
            get { return mAuthor; }
            set { mAuthor = value; }
        }

        /// <summary>
        /// Gets/sets an identity provider that produces value for the subsequent <see cref="UserId"/> property.
        /// </summary>
        internal ContactIdentityProvider IdentityProvider
        {
            get { return mIdentityProvider; }
            set { mIdentityProvider = value; }
        }

        /// <summary>
        /// Gets/sets a unique user ID for a person.
        /// </summary>
        internal string UserId
        {
            get { return mUserId; }
            set { mUserId = value; }
        }

        private string mAuthor;
        private ContactIdentityProvider mIdentityProvider;
        private string mUserId;
    }
}
