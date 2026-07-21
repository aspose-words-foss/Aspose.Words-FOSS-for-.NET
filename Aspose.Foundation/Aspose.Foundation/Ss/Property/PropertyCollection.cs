// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2005 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Ss.Property
{
    /// <summary>
    /// Typed collection of properties.
    /// </summary>
    public class PropertyCollection
    {
        public Property this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public void Add(Property item)
        {
            mItems.Add(item);
        }

        /// <summary>
        /// Linear search for a property by id.
        /// </summary>
        public Property GetById(int id)
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                if (this[i].Id == id)
                    return this[i];
            }
            return null;
        }

        /// <summary>
        /// Linear search for a property by name.
        /// </summary>
        public Property GetByName(string name)
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                if (this[i].Name == name)
                    return this[i];
            }
            return null;
        }

        /// <summary>
        /// Returns true if at least one property has a name.
        /// </summary>
        public bool HasNames
        {
            get
            {
                for (int i = 0; i < mItems.Count; i++)
                {
                    if (this[i].HasName)
                        return true;
                }
                return false;
            }
        }

        public int CountOfPropertiesWithNames
        {
            get
            {
                int result = 0;
                for (int i = 0; i < mItems.Count; i++)
                {
                    if (this[i].HasName)
                        result++;
                }
                return result;
            }
        }

        private readonly List<Property> mItems = new List<Property>();
    }
}
