// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents a list of persons who are bibliography source contributors.
    /// </summary>
    public sealed class PersonCollection : Contributor, IEnumerable<Person>
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="PersonCollection"/> class.
        /// </summary>
        public PersonCollection()
        {

        }

        /// <summary>
        /// Initialize a new instance of the <see cref="PersonCollection"/> class.
        /// </summary>
        public PersonCollection(IEnumerable<Person> persons)
        {
            foreach (Person person in persons)
                Add(person);
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="PersonCollection"/> class.
        /// </summary>
        public PersonCollection(params Person[] persons)
        {
            foreach (Person person in persons)
                Add(person);
        }

        /// <summary>
        /// Adds a <see cref="Person"/> to the collection.
        /// </summary>
        /// <param name="person">The person to add to the collection.</param>
        public void Add(Person person)
        {
            mPersons.Add(person);
        }

        /// <summary>
        /// Removes the person from the collection.
        /// </summary>
        /// <param name="person">The person to remove from the collection.</param>
        public bool Remove(Person person)
        {
            return mPersons.Remove(person);
        }

        /// <summary>
        /// Removes the person at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the person to remove.</param>
        public void RemoveAt(int index)
        {
            mPersons.RemoveAt(index);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            mPersons.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific person.
        /// </summary>
        /// <param name="person">The person to locate in the collection.</param>
        public bool Contains(Person person)
        {
            return mPersons.Contains(person);
        }

        /// <summary>
        /// Gets the number of persons contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mPersons.Count; }
        }

        /// <summary>
        /// Gets or sets a person at the specified index.
        /// </summary>
        /// <param name="index">An index into the collection.</param>
        public Person this[int index]
        {
            get { return mPersons[index]; }
            set { mPersons[index] = value; }
        }

        internal override Contributor Clone()
        {
            PersonCollection lhs = new PersonCollection();

            foreach (Person person in mPersons)
                lhs.Add(person.Clone());

            return lhs;
        }

        IEnumerator<Person> IEnumerable<Person>.GetEnumerator()
        {
            return mPersons.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Person>)this).GetEnumerator();
        }

        private readonly IList<Person> mPersons = new List<Person>();
    }
}
