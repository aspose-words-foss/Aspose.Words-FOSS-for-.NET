// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents individual (a person) bibliography source contributor.
    /// </summary>
    public sealed class Person
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="last">The last name.</param>
        /// <param name="first">The last name.</param>
        /// <param name="middle">The last name.</param>
        public Person(string last, string first, string middle)
        {
            Last = last;
            First = first;
            Middle = middle;
        }

        /// <summary>
        /// Gets or sets the last name of a person.
        /// </summary>
        public string Last { get; set; }

        /// <summary>
        /// Gets or sets the first name of a person.
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// Gets or sets the middle name of a person.
        /// </summary>
        public string Middle { get; set; }

        internal Person Clone()
        {
            return new Person(Last, First, Middle);
        }

        // moved from PersonEqualityComparer for Java porting
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            Person person = obj as Person;
            return (person != null) && Equals(person);
        }

        private bool Equals(Person other)
        {
            return (Last == other.Last) &&
                   (First == other.First) &&
                   (Middle == other.Middle);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Last != null ? Last.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (First != null ? First.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Middle != null ? Middle.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
