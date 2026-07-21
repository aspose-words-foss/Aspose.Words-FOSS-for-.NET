// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2023 by Edward Voronov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents bibliography source contributors.
    /// </summary>
    public sealed class ContributorCollection : IEnumerable<Contributor>
    {
        internal ContributorCollection()
        {
        }

        internal void SetContributor(ContributorType type, Contributor contributor)
        {
            if (contributor != null)
                mContributors[type] = contributor;
            else
                mContributors.Remove(type);
        }

        /// <summary>
        /// Gets or sets the artist of a source.
        /// </summary>
        public Contributor Artist
        {
            get { return TryGetContributor(ContributorType.Artist); }
            set { SetContributor(ContributorType.Artist, value); }
        }

        /// <summary>
        /// Gets or sets the author of a source.
        /// </summary>
        public Contributor Author
        {
            get { return TryGetContributor(ContributorType.Author); }
            set { SetContributor(ContributorType.Author, value); }
        }

        /// <summary>
        /// Gets or sets the book author of a source.
        /// </summary>
        public Contributor BookAuthor
        {
            get { return TryGetContributor(ContributorType.BookAuthor); }
            set { SetContributor(ContributorType.BookAuthor, value); }
        }

        /// <summary>
        /// Gets or sets the compiler of a source.
        /// </summary>
        public Contributor Compiler
        {
            get { return TryGetContributor(ContributorType.Compiler); }
            set { SetContributor(ContributorType.Compiler, value); }
        }

        /// <summary>
        /// Gets or sets the composer of a source.
        /// </summary>
        public Contributor Composer
        {
            get { return TryGetContributor(ContributorType.Composer); }
            set { SetContributor(ContributorType.Composer, value); }
        }

        /// <summary>
        /// Gets or sets the conductor of a source.
        /// </summary>
        public Contributor Conductor
        {
            get { return TryGetContributor(ContributorType.Conductor); }
            set { SetContributor(ContributorType.Conductor, value); }
        }

        /// <summary>
        /// Gets or sets the counsel of a source.
        /// </summary>
        public Contributor Counsel
        {
            get { return TryGetContributor(ContributorType.Counsel); }
            set { SetContributor(ContributorType.Counsel, value); }
        }

        /// <summary>
        /// Gets or sets the director of a source.
        /// </summary>
        public Contributor Director
        {
            get { return TryGetContributor(ContributorType.Director); }
            set { SetContributor(ContributorType.Director, value); }
        }

        /// <summary>
        /// Gets or sets the editor of a source.
        /// </summary>
        public Contributor Editor
        {
            get { return TryGetContributor(ContributorType.Editor); }
            set { SetContributor(ContributorType.Editor, value); }
        }

        /// <summary>
        /// Gets or sets the interviewee of a source.
        /// </summary>
        public Contributor Interviewee
        {
            get { return TryGetContributor(ContributorType.Interviewee); }
            set { SetContributor(ContributorType.Interviewee, value); }
        }

        /// <summary>
        /// Gets or sets the interviewer of a source.
        /// </summary>
        public Contributor Interviewer
        {
            get { return TryGetContributor(ContributorType.Interviewer); }
            set { SetContributor(ContributorType.Interviewer, value); }
        }

        /// <summary>
        /// Gets or sets the inventor of a source.
        /// </summary>
        public Contributor Inventor
        {
            get { return TryGetContributor(ContributorType.Inventor); }
            set { SetContributor(ContributorType.Inventor, value); }
        }

        /// <summary>
        /// Gets or sets the performer of a source.
        /// </summary>
        public Contributor Performer
        {
            get { return TryGetContributor(ContributorType.Performer); }
            set { SetContributor(ContributorType.Performer, value); }
        }

        /// <summary>
        /// Gets or sets the producer of a source.
        /// </summary>
        public Contributor Producer
        {
            get { return TryGetContributor(ContributorType.Producer); }
            set { SetContributor(ContributorType.Producer, value); }
        }

        /// <summary>
        /// Gets or sets the translator of a source.
        /// </summary>
        public Contributor Translator
        {
            get { return TryGetContributor(ContributorType.Translator); }
            set { SetContributor(ContributorType.Translator, value); }
        }

        /// <summary>
        /// Gets or sets the writer of a source.
        /// </summary>
        public Contributor Writer
        {
            get { return TryGetContributor(ContributorType.Writer); }
            set { SetContributor(ContributorType.Writer, value); }
        }

        internal Contributor GetContributor(ContributorType type)
        {
            return mContributors[type];
        }

        private Contributor TryGetContributor(ContributorType type)
        {
            Contributor contributor = null;
            mContributors.TryGetValue(type, out contributor);
            return contributor;
        }

        internal ContributorCollection Clone()
        {
            ContributorCollection lhs = new ContributorCollection();

            foreach (ContributorType contributorType in ContributorTypes)
                lhs.SetContributor(contributorType, GetContributor(contributorType).Clone());

            return lhs;
        }

        internal bool IsEmpty
        {
            get { return mContributors.Count == 0; }
        }

        internal IEnumerable<ContributorType> ContributorTypes
        {
            get { return mContributors.Keys; }
        }

        IEnumerator<Contributor> IEnumerable<Contributor>.GetEnumerator()
        {
            return mContributors.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Contributor>)this).GetEnumerator();
        }

        private readonly IDictionary<ContributorType, Contributor> mContributors = new Dictionary<ContributorType, Contributor>();
    }
}
