// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/01/2015 by Victor Chebotok

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a mutable collection of <see cref="CssDeclaration">CSS declarations</see>.
    /// </summary>
    /// <remarks>
    /// This collection is mutable and declarations stored in it can be modified. If this behavior is undesirable,
    /// the immutable <see cref="CssDeclarationCollection"/> should be used instead. The usual strategy is to use a mutable
    /// collection during resolution of CSS declarations and to convert it into an immutable collection
    /// as soon as all declarations are resolved.
    /// This class does not preserve the order in which declarations are added.
    /// </remarks>
    internal class CssDeclarationCollectionBuilder : IEnumerable<CssDeclaration>
    {
        /// <summary>
        /// Creates a new empty collection.
        /// </summary>
        internal CssDeclarationCollectionBuilder()
        {
            mDeclarations = new CssDeclarationHashtable();
        }

        /// <summary>
        /// Creates a new collection containing the specified declarations.
        /// </summary>
        /// <param name="declarations">
        /// Zero or more instances of <see cref="CssDeclaration"/> that should be added to the collection.
        /// </param>
        internal CssDeclarationCollectionBuilder(CssDeclarationCollection declarations)
        {
            // Reuse the backing collection until we need to modify it. This optimizes memory usage in scenarios where
            // a builder is created but is not modified. For example, we can create a builder to collapse CSS declarations
            // into shorthand verions and then realize that no declarations can be collapsed.
            mDeclarations = declarations.GetHashtable();
        }

        public IEnumerator<CssDeclaration> GetEnumerator()
        {
            return mDeclarations.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object that iterates through <see cref="CssDeclaration"/> instances stored in this collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified declaration to the collection.
        /// </summary>
        /// <param name="declaration">CSS declaration.</param> 
        internal void Add(CssDeclaration declaration)
        {
            EnsureDeclarationsAreWritable();
            mDeclarations.Add(declaration);
        }

        /// <summary>
        /// Adds the specified declarations to the collection.
        /// </summary>
        /// <param name="declarations">CSS declarations (instances of <see cref="CssDeclaration"/>).</param> 
        internal void Add(IEnumerable<CssDeclaration> declarations)
        {
            EnsureDeclarationsAreWritable();
            mDeclarations.Add(declarations);
        }

        /// <summary>
        /// Adds the specified declaration to the collection if the declaration is not <c>null</c>.
        /// </summary>
        /// <param name="declaration">CSS declaration.</param> 
        internal void AddIfNotNull(CssDeclaration declaration)
        {
            if (declaration != null)
            {
                EnsureDeclarationsAreWritable();
                mDeclarations.Add(declaration);
            }
        }

        /// <summary>
        /// Adds the specified declaration to the collection if the declaration is not <c>null</c> and does not exist in
        /// the collection.
        /// </summary>
        /// <param name="declaration">CSS declaration.</param> 
        internal void AddIfNotNullAndNotExist(CssDeclaration declaration)
        {
            if ((declaration != null) && (mDeclarations[declaration.Property] == null))
            {
                EnsureDeclarationsAreWritable();
                mDeclarations.Add(declaration);
            }
        }

        /// <summary>
        /// Adds the specified declaration to the collection replacing a declaration with same property name if any exists.
        /// </summary>
        /// <param name="declaration">CSS declaration.</param> 
        internal void AddOrReplace(CssDeclaration declaration)
        {
            Debug.Assert(declaration != null);
            EnsureDeclarationsAreWritable();
            mDeclarations[declaration.Property] = declaration;

            // The purpose of this method is to write a brand new declaration into the collection.
            // As the declaration is new, we reset its flags.
            mDeclarations.SetFlags(declaration.Property, CssDeclarationFlags.None);
        }

        /// <summary>
        /// Adds the specified declarations to the collection replacing existing declarations with same property names.
        /// </summary>
        /// <param name="declarations">CSS declaration collection (instances of <see cref="CssDeclaration"/>).</param>
        internal void AddOrReplace(IEnumerable<CssDeclaration> declarations)
        {
            EnsureDeclarationsAreWritable();
            foreach (CssDeclaration declaration in declarations)
            {
                Debug.Assert(declaration != null);
                mDeclarations[declaration.Property] = declaration;

                // The purpose of this method is to write a bunch of brand new declarations into the collection.
                // As the declarations are new, we reset their flags.
                mDeclarations.SetFlags(declaration.Property, CssDeclarationFlags.None);
            }
        }

        /// <summary>
        /// Adds the specified declarations to the collection replacing existing declarations with same property names if new
        /// declarations are more or equally important as old ones. Old declarations that are more important are not replaced.
        /// </summary>
        /// <param name="declarations">CSS declaration collection (instances of <see cref="CssDeclaration"/>).</param>
        internal void AddOrReplaceIfMoreOrEquallyImportant(IEnumerable<CssDeclaration> declarations)
        {
            EnsureDeclarationsAreWritable();
            foreach (CssDeclaration declaration in declarations)
            {
                Debug.Assert(declaration != null);

                bool newDeclarationIsMoreOrEquallyImportant = declaration.Important;
                if (!newDeclarationIsMoreOrEquallyImportant)
                {
                    CssDeclaration existingDeclaration = mDeclarations[declaration.Property];
                    newDeclarationIsMoreOrEquallyImportant = (existingDeclaration == null) || (!existingDeclaration.Important);
                }

                if (newDeclarationIsMoreOrEquallyImportant)
                {
                    mDeclarations[declaration.Property] = declaration;
                }

                // The purpose of this method is to write a bunch of brand new declarations into the collection.
                // As the declarations are new, we reset their flags.
                mDeclarations.SetFlags(declaration.Property, CssDeclarationFlags.None);
            }
        }

        /// <summary>
        /// Replaces an existing declaration with a new one. Preserves the declaration flags.
        /// </summary>
        internal void Replace(CssDeclaration declaration)
        {
            Debug.Assert(declaration != null);
            EnsureDeclarationsAreWritable();
            mDeclarations[declaration.Property] = declaration;
        }

        /// <summary>
        /// Removes the declaration for the property with the specified name from the collection.
        /// </summary>
        internal void Remove(string property)
        {
            Debug.Assert(StringUtil.HasChars(property));
            Debug.Assert(property == property.ToLowerInvariant());

            EnsureDeclarationsAreWritable();
            mDeclarations.Remove(property);
        }

        /// <summary>
        /// Indicates whether a property comes from our User Agent stylesheet.
        /// </summary>
        internal bool IsUserAgent(string property)
        {
            return mDeclarations.GetFlag(property, CssDeclarationFlags.UserAgent);
        }

        /// <summary>
        /// Marks a property as coming from our User Agent stylesheet.
        /// </summary>
        internal void MarkUserAgent(string property)
        {
            EnsureDeclarationsAreWritable();
            mDeclarations.SetFlag(property, CssDeclarationFlags.UserAgent, true);
        }

        /// <summary>
        /// Gets all flags that are set for a property.
        /// </summary>
        internal CssDeclarationFlags GetFlags(string property)
        {
            return mDeclarations.GetFlags(property);
        }

        /// <summary>
        /// Sets flags for a property. Doesn't preserve flags that are currently set for the property.
        /// </summary>
        internal void SetFlags(string property, CssDeclarationFlags flags)
        {
            EnsureDeclarationsAreWritable();
            mDeclarations.SetFlags(property, flags);
        }

        /// <summary>
        /// Copy flags for a property from another collection. The property must exist in this collection (in the target).
        /// </summary>
        internal void CopyFlags(string property, CssDeclarationCollection source)
        {
            Debug.Assert(source != null);
            EnsureDeclarationsAreWritable();
            mDeclarations.SetFlags(property, source.GetFlags(property));
        }

        /// <summary>
        /// Creates an immutable copy of this collection.
        /// </summary>
        internal CssDeclarationCollection GetDeclarations()
        {
            if (Count != 0)
            {
                // The backing hashtable can be in two modes: shared (when the builder itself and one or more
                // immutable collections work with the hashtable) and unshared (when only the builder itself works
                // with the hashtable). A builder can only modify an unshared hashtable and a hashtable is marked readonly
                // before sharing. If any further modifications are required to an already shared hashtable, it must be cloned. 
                // Initially, a hashtable is unshared, and it becomes shared after a call to GetDeclarations. 
                // Subsequent calls to GetDeclarations use the same shared hashtable. However, any call to a modifying method
                // of CssDeclarationCollectionBuilder (for example, CssDeclarationCollectionBuilder.Add) replaces the shared
                // hashtable inside CssDeclarationCollectionBuilder with its unshared copy that can be modified.
                // This behaviour optimizes performance (~1-2% by tests) and memory usage.
                mDeclarations.MakeReadOnly();
                return new CssDeclarationCollection(mDeclarations);
            }

            return CssDeclarationCollection.Empty;
        }

        /// <summary>
        /// Ensures the backing collection can be modified (is not readonly). If the collection is readonly, it is replaced
        /// with a writable copy.
        /// </summary>
        private void EnsureDeclarationsAreWritable()
        {
            if (mDeclarations.IsReadOnly)
            {
                mDeclarations = mDeclarations.GetWritableCopy();
            }
        }

        /// <summary>
        /// Returns the number of declarations in the collection.
        /// </summary>
        internal int Count
        {
            get { return mDeclarations.Count; }
        }

        /// <summary>
        /// Gets a CSS declaration for the specified property.
        /// </summary>
        /// <param name="property">CSS property name.</param>
        /// <returns>CSS declaration, if found; otherwise, null.</returns>
        internal CssDeclaration this[string property]
        {
            get { return mDeclarations[property]; }
        }

#if DEBUG
        public override string ToString()
        {
            // We can't use the normal way of getting the collection, because it has a side effect: if the underlying collection
            // is modified, a new readonly copy of it will be created. We don't want this method to have any side effects
            // so we manually create our own copy of the underlying collection.
            CssDeclarationCollection clone = new CssDeclarationCollection(mDeclarations.GetWritableCopy());
            return clone.ToString();
        }
#endif

        /// <summary>
        /// The backing collection that stores all declarations. The collection can be either read-only or writable.
        /// </summary>
        private CssDeclarationHashtable mDeclarations;
    }
}
