// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2016 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="PersonInternal"/> objects that represent contact information for authors of comments and
    /// revisions in a document.
    /// </summary>
    internal class PersonCollectionInternal : IEnumerable<PersonInternal>
    {
        public IEnumerator<PersonInternal> GetEnumerator()
        {
            return mPersons.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator object that will enumerate persons that are stored in the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Makes a deep copy of the person collection. Suitable when copying a complete document.
        /// </summary>
        internal PersonCollectionInternal Clone()
        {
            PersonCollectionInternal lhs = (PersonCollectionInternal)MemberwiseClone();

            lhs.mPersons = new List<PersonInternal>();
            foreach (PersonInternal person in this)
                lhs.Add(person.Clone());

            return lhs;
        }

        /// <summary>
        /// Adds the specified person into the collection.
        /// </summary>
        internal void Add(PersonInternal person)
        {
            mPersons.Add(person);
        }

        /// <summary>
        /// Creates a person with the specified values of its properties and adds it into the collection.
        /// </summary>
        internal PersonInternal Add(string author, ContactIdentityProvider provider, string userId)
        {
            PersonInternal person = new PersonInternal();
            person.Author = author;
            person.IdentityProvider = provider;
            person.UserId = userId;

            Add(person);

            return person;
        }

        /// <summary>
        /// Gets a person by index.
        /// </summary>
        internal PersonInternal this[int index]
        {
            get { return mPersons[index]; }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return mPersons.Count; }
        }

        private List<PersonInternal> mPersons = new List<PersonInternal>();
    }
}
