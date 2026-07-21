// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Victor Chebotok

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A hashtable that maps CSS property names to corresponding declarations. Each declaration can be marked with flags,
    /// which are useful, for example to indicate the source of the declaration.
    /// </summary>
    /// <remarks>
    /// CSS property should be lower-case.
    /// After all modifications are done, the hashtable can be marked as readonly in order to make it suitable for usage in
    /// immutable classes.
    /// </remarks>
    internal class CssDeclarationHashtable : IEnumerable<CssDeclaration>
    {
        internal CssDeclarationHashtable()
            : this(new StringToObjDictionary<CssDeclaration>(), null)
        {
            // Empty constructor.
        }

        private CssDeclarationHashtable(StringToObjDictionary<CssDeclaration> declarationStorage,
            StringToIntDictionary flagsStorage)
        {
            Debug.Assert(declarationStorage != null);
            mDeclarationStorage = declarationStorage;
            mFlags = flagsStorage;
        }

        public IEnumerator<CssDeclaration> GetEnumerator()
        {
            return mDeclarationStorage.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through instances of <see cref="CssDeclaration"/> stored in this collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a copy of this hashtable. The copy is guaranteed to be writable.
        /// </summary>
        /// <returns>A copy of this hashtable.</returns>
        internal CssDeclarationHashtable GetWritableCopy()
        {
            // SPEED. Don't create a flags storage if the collection doesn't use them.
            StringToIntDictionary flagsCopy = ((mFlags != null) && (mFlags.Count > 0))
                ? new StringToIntDictionary(mFlags)
                : null;
            return new CssDeclarationHashtable(new StringToObjDictionary<CssDeclaration>(mDeclarationStorage), flagsCopy);
        }

        /// <summary>
        /// Adds a declaration to the hasthable.
        /// </summary>
        /// <param name="declaration">A declaration to be added.</param>
        internal void Add(CssDeclaration declaration)
        {
            Debug.Assert(declaration != null);
            EnsureHashtableIsWritable();
            AddUnsafe(declaration);
        }

        /// <summary>
        /// Adds zero or more declarations to the hasthable.
        /// </summary>
        /// <param name="declarations">Declarations to be added. Instances of <see cref="CssDeclaration"/>.</param>
        internal void Add(IEnumerable<CssDeclaration> declarations)
        {
            Debug.Assert(declarations != null);
            EnsureHashtableIsWritable();
            foreach (CssDeclaration declaration in declarations)
            {
                Debug.Assert(declaration != null);
                AddUnsafe(declaration);
            }
        }

        /// <summary>
        /// Adds zero or more declarations to the hasthable.
        /// </summary>
        /// <param name="declarations">Declarations to be added. Instances of <see cref="CssDeclaration"/>.</param>
        internal void Add(CssDeclaration[] declarations)
        {
            Debug.Assert(declarations != null);
            EnsureHashtableIsWritable();
            foreach (CssDeclaration declaration in declarations)
            {
                Debug.Assert(declaration != null);
                AddUnsafe(declaration);
            }
        }

        /// <summary>
        /// Removes a declaration for the specified CSS property from the hashtable.
        /// </summary>
        /// <param name="property">A lower-case CSS property name.</param>
        internal void Remove(string property)
        {
            Debug.Assert(StringUtil.HasChars(property));
            Debug.Assert(property == property.ToLowerInvariant());

            EnsureHashtableIsWritable();
            mDeclarationStorage.Remove(property);
            if (mFlags != null)
            {
                mFlags.Remove(property);
            }
        }

        /// <summary>
        /// Gets the value of a flag for a declaration.
        /// </summary>
        internal bool GetFlag(string property, CssDeclarationFlags flag)
        {
            return (GetFlags(property) & flag) != 0;
        }

        /// <summary>
        /// Modifies the value of a flag for a declaration. Preserves values of other flags.
        /// </summary>
        internal void SetFlag(string property, CssDeclarationFlags flag, bool value)
        {
            CssDeclarationFlags oldFlags = GetFlags(property);
            CssDeclarationFlags newFlags = (value)
                ? oldFlags | flag
                : oldFlags & (~flag);
            SetFlags(property, newFlags);
        }

        /// <summary>
        /// Gets all flags that are set for a declaration.
        /// </summary>
        internal CssDeclarationFlags GetFlags(string property)
        {
            Debug.Assert(StringUtil.HasChars(property));
            Debug.Assert(property == property.ToLowerInvariant());

            // We don't write empty flags each time we add a property, because there are situations where we don't need flags
            // at all - we just want a simple collection of properties. Flags are only filled when set explicitly
            // so it is a normal case if flags for a property don't exist.
            
            if (mFlags == null)
            {
                return CssDeclarationFlags.None;
            }

            return mFlags.ContainsKey(property) ? (CssDeclarationFlags) mFlags[property] : CssDeclarationFlags.None;
        }

        /// <summary>
        /// Sets flag values for a declaration. Replaces old flags with the specified ones.
        /// </summary>
        internal void SetFlags(string property, CssDeclarationFlags flags)
        {
            Debug.Assert(StringUtil.HasChars(property));
            Debug.Assert(property == property.ToLowerInvariant());

            EnsureHashtableIsWritable();

            // SPEED. Don't create the flags storage unless any flags are set.
            if ((flags == CssDeclarationFlags.None) && (mFlags == null))
            {
                return;
            }
            
            // Ensure that declarations and flags are in sync.
            Debug.Assert(mDeclarationStorage.ContainsKey(property));

            if (mFlags == null)
            {
                // SPEED. Usually flags are set after all declarations are written. Create a flag storage large enough
                // to store flags for every declaration.
                mFlags = new StringToIntDictionary(mDeclarationStorage.Count);
            }

            mFlags[property] = (int) flags;
        }

        /// <summary>
        /// Makes the hashtable readonly. No further addition or removal of declarations will be allowed for this hashtable.
        /// </summary>
        internal void MakeReadOnly()
        {
            mIsReadonly = true;
        }

        /// <summary>
        /// Gets the number of declarations in the hashtable.
        /// </summary>
        internal int Count
        {
            get { return mDeclarationStorage.Count; }
        }

        /// <summary>
        /// Indicates whether the hashtable is readonly. If a hashtable is readonly, no declarations can be added to or removed
        /// from it.
        /// </summary>
        internal bool IsReadOnly
        {
            get { return mIsReadonly; }
        }

        /// <summary>
        /// Gets a declaration by CSS property name.
        /// </summary>
        /// <param name="property">A lower-case CSS property name.</param>
        /// <returns>A declaration or <c>null</c> if there is no declaration for the specified property name.</returns>
        internal CssDeclaration this[string property]
        {
            get
            {
                Debug.Assert(StringUtil.HasChars(property));
                Debug.Assert(property == property.ToLowerInvariant());

                return mDeclarationStorage[property];
            }
            set
            {
                Debug.Assert(value.Property == property);

                EnsureHashtableIsWritable();
                mDeclarationStorage[value.Property] = value;
            }
        }

        private void EnsureHashtableIsWritable()
        {
            if (mIsReadonly)
            {
                throw new InvalidOperationException();
            }
        }

        private void AddUnsafe(CssDeclaration declaration)
        {
            mDeclarationStorage.Add(declaration.Property, declaration);
        }

        // Two different synchronous dictionaries of simple values (one for integer flags and another for references
        // to immutable declarations) seem easier to clone than one dictionary of complex objects (mutable classes).
        // Another advantage is that flags can be filled only when needed and they won't occupy memory if not used.
        private readonly StringToObjDictionary<CssDeclaration> mDeclarationStorage;
        /// <summary>
        /// Flags of declarations stored in this collection. <c>null</c> unless any flags are set.
        /// </summary>
        /// <remarks>
        /// The flag is CssDeclarationFlags Enum type. Stored as int value for autoportability to Java.
        /// </remarks>
        private StringToIntDictionary mFlags;

        private bool mIsReadonly;
    }
}
