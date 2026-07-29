// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Aspose.Collections;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Formatting.Intern;
using Aspose.Words.Forms2;
using Aspose.Words.Math;
using Aspose.Words.Tables;
using FieldInfo = System.Reflection.FieldInfo;

namespace Aspose.Words
{
    /// <summary>
    /// Collection of attribute values and their keys.
    /// This is the backbone of the mechanism for objects to store and inherit attributes.
    /// </summary>
    internal abstract class AttrCollection : SortedShortListIntegerFallback
    {
        public override object this[int key]
        {
            get
            {
                return (InternState == InternState.Interned) ? mPoolItem.Pr[key] : base[key];
            }
            set
            {
                BeforeChange();
                base[key] = value;
            }
        }

        public override int Count
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                return (InternState == InternState.Interned) ? mPoolItem.Pr.Count : base.Count;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public override int GetKey(int index)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.GetKey(index) : base.GetKey(index);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public override object GetByIndex(int index)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.GetByIndex(index) : base.GetByIndex(index);
        }

        public override void Remove(int key)
        {
            BeforeChange();
            base.Remove(key);
        }

        public override void RemoveAt(int key)
        {
            BeforeChange();
            base.RemoveAt(key);
        }

        /// <summary>
        /// Gets the attribute specified directly in the collection, return null if not found.
        /// </summary>
        public object GetDirectAttr(int key)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.GetDirectAttr(key) : base[key];
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public override bool ContainsKey(int key)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.ContainsKey(key) : base.ContainsKey(key);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public override bool Contains(int key)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.Contains(key) : base.Contains(key);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public override int IndexOfKey(int key)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.IndexOfKey(key) : base.IndexOfKey(key);
        }

        public override void Clear()
        {
            BeforeChange();
            base.Clear();
        }

        public override void Add(int key, Object value)
        {
            BeforeChange();
            base.Add(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public override bool ContainsAnyKey(params int[] keys)
        {
            return (InternState == InternState.Interned) ? mPoolItem.Pr.ContainsAnyKey(keys) : base.ContainsAnyKey(keys);
        }

        internal InternState InternState
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                if (mPoolItem == null)
                    return InternState.None;

                if (ReferenceEquals(mPoolItem.Pr, this))
                    return InternState.Pooled;

                return InternState.Interned;
            }
        }

        internal void BeforeChange()
        {
            if (mPoolItem != null)
                mPoolItem.InternManager.Remove(this);
        }

        /// <summary>
        /// Gets the attribute from the collection or from default attributes, throws if cannot get it.
        /// </summary>
        internal object FetchAttr(int key)
        {
            object value = GetDirectAttr(key);
            return (value != null) ? value : FetchInheritedAttr(key);
        }

        /// <summary>
        /// This implementation gets the attribute from the global defaults, throws if not found.
        /// </summary>
        public object FetchInheritedAttr(int key)
        {
            AttrCollection defaults = GetDefaults();

            int index = defaults.IndexOfKey(key);
            if (index < 0)
                throw new InvalidOperationException(string.Format("Requested default value for an unknown attribute {0}.", key));

            return defaults.GetByIndex(index);
        }

        /// <summary>
        /// Stores an attribute in the collection.
        /// </summary>
        public void SetAttr(int key, object value)
        {
            // Adding null attributes to collections is not normally allowed.
            Debug.Assert(value != null);

            BeforeChange();

            base[key] = value;
        }

        /// <summary>
        /// Stores an attribute in the collection if the value is not null.
        /// </summary>
        public void SetAttrIfNotNull(int key, object value)
        {
            if (value != null)
                SetAttr(key, value);
        }

        /// <summary>
        /// Stores an attribute in the collection if the value is not default value.
        /// </summary>
        public void SetAttrIfNotDefault(int key, object value)
        {
            object defaultValue = FetchInheritedAttr(key);
            if ((defaultValue == null) || !value.Equals(defaultValue))
                SetAttr(key, value);
        }

        /// <summary>
        /// Only add attribute to this collection if it is non-default (isNonDefault is true).
        /// This method helps maintain collections where all attributes are not equal to defaults.
        /// </summary>
        /// <remarks>
        /// Used in <see cref="MathObject"/> hierarchy. In all descendant objects the attributes are added only
        /// if they are non-default.
        /// </remarks>
        protected void SetAttr(int key, object value, bool isNonDefault)
        {
            if (isNonDefault)
                SetAttr(key, value);
            else
                Remove(key);
        }

        /// <summary>
        /// Moves the attribute to the specified collection.
        /// </summary>
        internal void MoveTo(AttrCollection dst, int key)
        {
            object value = GetDirectAttr(key);
            if (value != null)
            {
                Remove(key);
                dst.SetAttr(key, value);
            }
        }

        /// <summary>
        /// Mirrors the specified attribute to the specified collection.
        /// </summary>
        /// <remarks>
        /// Note that attribute absence is also copied i.e attribute missing in source collection is removed from destination.
        /// </remarks>
        internal void MirrorTo(AttrCollection dst, int key)
        {
            object value = GetDirectAttr(key);
            if (value != null)
            {
                dst.SetAttr(key, value);
            }
            else
            {
                dst.Remove(key);
            }
        }

        /// <summary>
        /// Mirrors the specified attributes to the specified collection.
        /// </summary>
        /// <remarks>
        /// Note that attribute absence is also copied i.e attribute missing in source collection is removed from destination.
        /// </remarks>
        internal void MirrorTo(AttrCollection dst, params int[] keys)
        {
            foreach (int key in keys)
                MirrorTo(dst, key);
        }

        /// <summary>
        /// Copies the attribute collection to destination collection.
        /// </summary>
        /// <remarks>
        /// Similar to ExpandTo method but differs in complex attributes handling: does not clone and does not merge complex attribute values.
        /// </remarks>
        internal void CopyTo(AttrCollection dst)
        {
            for (int i = 0; i < Count; i++)
            {
                int key = GetKey(i);
                dst.SetAttr(key, this[key]);
            }
        }

        /// <summary>
        /// Copies the attribute value of given key to destination collection.
        /// </summary>
        /// <remarks>
        /// Similar to ExpandTo method but differs in complex attributes handling:
        /// does not clone and does not merge complex attribute values.
        /// </remarks>
        internal void CopyTo(AttrCollection dst, int key)
        {
            object val = this[key];

            if(val != null)
                dst.SetAttr(key, val);
        }

        /// <summary>
        /// Adds attribute to this collection if there is no such attribute in it.
        /// </summary>
        internal void SetAttrNoOverride(int key, object value)
        {
            if (!ContainsKey(key))
                SetAttr(key, value);
        }

        /// <summary>
        /// Concrete classes to implement and return a collection of default attributes.
        /// </summary>
        protected abstract AttrCollection GetDefaults();

        /// <summary>
        /// Creates a deep copy of the collection.
        ///
        /// Attributes that implement the IAttr interface are deep copied,
        /// but value types are value copied.
        /// </summary>
        internal virtual AttrCollection CloneCore()
        {
            // Cloned collection should be uninterned.
            AttrCollection lhs = (InternState == InternState.Interned)
                ? (AttrCollection)mPoolItem.Pr.CreateEmptyCopy()
                : (AttrCollection)base.CreateEmptyCopy();

            lhs.mPoolItem = null;

            for (int i = 0; i < Count; i++)
            {
                int key = GetKey(i);
                object srcValue = GetByIndex(i);
                object newValue;

                if (srcValue is IComplexAttr)
                {
                    IComplexAttr srcComplex = (IComplexAttr)srcValue;

                    // The value is a complex attribute that can be inherited.
                    // We do not clone inherited attributes because they can (and should be) safely discarded.
                    if (srcComplex.IsInheritedComplexAttr)
                        continue;

                    if (srcValue is ComplexFontName)
                    {
                        // WORDSJAVA-1127 Aspose.Words for Java consumes too much memory during PDF conversion.
                        // ComplexFontName is immutable object so we don't need to clone it.
                        newValue = srcValue;
                    }
                    else
                    {
                        newValue = srcComplex.DeepCloneComplexAttr();
                    }
                }
                else
                {
                    // All other attributes - we treat as immutable and don't create a deep clone.
                    newValue = srcValue;
                }

                lhs[key] = newValue;
            }

            return lhs;
        }

        /// <summary>
        /// Expands formatting specified in this collection into the destination collection.
        /// Basically attributes that exist in this collection either override or somehow modify
        /// the attributes in the destination collection.
        ///
        /// DstAttrs is the BeforeChanges attributes.
        /// This is the PositiveDifference attributes.
        /// This method performs AfterChanges = BeforeChanges + PositiveDifference.
        /// DstAttrs becomes AfterChanges.
        /// </summary>
        internal void ExpandTo(AttrCollection dstAttrs)
        {
            ExpandTo(dstAttrs, (IExpandedAttrSaver)null); // Casting for C++ to make it select correct overload. See WORDSCPP-532.
        }

        /// <summary>
        /// Expands formatting with an option to include default collection to dstCollection.
        /// </summary>
        internal void ExpandTo(AttrCollection dstAttrs, bool isAllowDefault)
        {
            if (isAllowDefault)
            {
                AttrCollection defaults = GetDefaults();
                Debug.Assert(!ReferenceEquals(this, defaults));
                defaults.ExpandTo(dstAttrs);
            }
            ExpandTo(dstAttrs);
        }

        internal void ExpandTo(AttrCollection dstAttrs, IExpandedAttrSaver saver)
        {
            if (dstAttrs == null)
                throw new ArgumentNullException("dstAttrs");

            for (int i = 0; i < Count; i++)
            {
                int key = GetKey(i);
                object srcValue = GetByIndex(i);
                ExpandTo(dstAttrs, key, srcValue, saver);
            }
        }

        private static void ExpandTo(AttrCollection dstAttrs, int key, object srcValue, IExpandedAttrSaver saver)
        {
            object newValue;

            if (srcValue is IComplexAttr)
            {
                IComplexAttr srcComplex = (IComplexAttr)srcValue;

                // If the attribute is inherited, in this context it basically means "missing".
                if (srcComplex.IsInheritedComplexAttr)
                    return;

                if (srcComplex is IExpandableAttr)
                {
                    // The attribute has to be merged with the parent value.
                    IExpandableAttr parentExpandable = (IExpandableAttr)dstAttrs[key];
                    newValue = ((IExpandableAttr)srcComplex).Expand(parentExpandable);
                }
                else if (srcComplex is ComplexFontName)
                {
                    // WORDSJAVA-1127 Aspose.Words for Java consumes too much memory during PDF conversion.
                    // ComplexFontName is immutable object so we don't need to clone it.
                    newValue = srcComplex;
                }
                else
                {
                    // The attribute just has to be cloned.
                    newValue = srcComplex.DeepCloneComplexAttr();
                }
            }
            else if (srcValue is AttrBoolEx)
            {
                AttrBoolEx srcBoolExValue = (AttrBoolEx)srcValue;
                newValue = srcBoolExValue.ResolveFetchAttr(dstAttrs, key);
            }
            else
            {
                // All other attributes - we treat as immutable and don't create a deep clone.
                newValue = srcValue;
            }

            if (saver != null)
                saver.Save(dstAttrs, key, newValue);
            else
                dstAttrs[key] = newValue;
        }

        /// <summary>
        /// Expands formatting for the specified attribute keys.
        /// </summary>
        /// <param name="dstAttrs"></param>
        /// <param name="keys"></param>
        internal void ExpandToInclusive(AttrCollection dstAttrs, params int[] keys)
        {
            if (dstAttrs == null)
                throw new ArgumentNullException("dstAttrs");

            if (keys == null)
                throw new ArgumentNullException("keys");

            foreach (int key in keys)
            {
                object srcValue = this[key];
                if (srcValue != null)
                    ExpandTo(dstAttrs, key, srcValue, null);
            }
        }

        /// <summary>
        /// Performs inverse of expanding attributes.
        /// Updates this collection by comparing attributes in this collection with the specified base.
        ///
        /// BaseAttrs is the BeforeChanges attributes.
        /// This is the AfterChanges attributes.
        /// This method performs PositiveDifference = AfterChanges - BeforeChanges.
        /// This becomes PositiveDifference.
        /// </summary>
        internal void Collapse(AttrCollection baseAttrs)
        {
            Collapse(baseAttrs, -1);
        }

        /// <summary>
        /// Performs inverse of expanding attributes.
        /// See comments above.
        /// This method is used in RTF reader where seems to be a need to preserve the istd attribute
        /// during collapse. I can't remember exactly why, will figure out later.
        /// </summary>
        /// <param name="baseAttrs">BeforeChanges collection.</param>
        /// <param name="keysToIgnore">Ids of the attributes to ignore during collapsing.</param>
        internal void Collapse(AttrCollection baseAttrs, params int[] keysToIgnore)
        {
            if (baseAttrs == null)
                throw new ArgumentNullException("baseAttrs");

            for (int baseIndex = 0; baseIndex < baseAttrs.Count; baseIndex++)
            {
                int key = baseAttrs.GetKey(baseIndex);

                if (ArrayUtil.Contains(keysToIgnore, key) || IsIgnoredOnCollapse(key))
                    continue;

                int childIndex = IndexOfKey(key);
                if (childIndex >= 0)
                {
                    // The value exists both in the base collection and in the child collection.
                    // We must compare the values to decide what to do.
                    object baseValue = baseAttrs.GetByIndex(baseIndex);
                    object childValue = GetByIndex(childIndex);
                    if (AreSameValues(childValue, baseValue))
                    {
                        // The value is same in the base collection and in the child collection.
                        // This is a result of expanding, we reverse it by removing the child attribute.
                        RemoveAt(childIndex);
                    }
                    else
                    {
                        // The value is different in the base collection and in the child collection.
                        // It means a value is directly specified in the child collection.
                        // For an expandable attribute (such as tab stops), need to collapse it.
                        // For other attributes simply do nothing and the child value will be kept.
                        CollapseExpandableAttrIfNeeded(childValue, baseValue);
                    }
                }
                else
                {
                    // The value exists in the base collection, but not in the child collection.
                    // We must add a default value to the child collection.

                    // This gets a default value.
                    object childValue = FetchInheritedAttr(key);

                    // If we have a complex attribute, we must clone.
                    if (childValue is IComplexAttr)
                    {
                        // WORDSJAVA-1127 Aspose.Words for Java consumes too much memory during PDF conversion.
                        // ComplexFontName is immutable object so we don't need to clone it.
                        if (!(childValue is ComplexFontName))
                        {
                            childValue = ((IComplexAttr)childValue).DeepCloneComplexAttr();
                        }

                        // If it is a tab stop, it needs to be collapsed relative to the base tab stops.
                        object baseValue = baseAttrs.GetByIndex(baseIndex);
                        CollapseExpandableAttrIfNeeded(childValue, baseValue);
                    }

                    this[key] = childValue;
                }
            }
        }

        /// <summary>
        /// Removes values from specified collection which AW ignores while collapsing.
        /// </summary>
        internal void RemoveIgnorableOnCollapseValues()
        {
            foreach (int key in GetKeys())
            {
                if (IsIgnoredOnCollapse(key))
                    Remove(key);
            }
        }

        /// <summary>
        /// Check given attribute to be excluded from collapse process.
        /// </summary>
        private static bool IsIgnoredOnCollapse(int key)
        {
            switch (key)
            {
                // Revision keys
                case RevisionAttr.FormatRevision:
                case RevisionAttr.InsertRevision:
                case RevisionAttr.DeleteRevision:
                case RevisionAttr.MoveFromRevision:
                case RevisionAttr.MoveToRevision:
                case RevisionAttr.NumberRevision:

                // Rsids keys
                case FontAttr.RsidRPr:
                case FontAttr.RsidR:
                case ParaAttr.RsidP:

                // Internal keys
                case TableAttr.Sys_CalculatedTableGrid:
                case TableAttr.Sys_TableGrid:
                case TableAttr.Sys_TableGridForNewAlgorithm:
                case FontAttr.Sys_Symbol:

                case ParaAttr.Sys_Alignment97:
                case ParaAttr.Sys_FirstLineIndent97:
                case ParaAttr.Sys_LeftIndent97:
                case ParaAttr.Sys_RightIndent97:
                // WORDSNET-18265 Added attribute.
                case WordAttrCollection.SysDirectAttrsBackup:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Compares this collection with another.
        /// </summary>
        internal bool Equals(AttrCollection coll, int[] keysToIgnore)
        {
            return Equals(this, coll, keysToIgnore);
        }

        /// <summary>
        /// Checks whether this collection contains any non-inherited attribute except for the given subset.
        /// </summary>
        internal bool ContainsNonInheritedAttrsNotInList(int[] keysToIgnore)
        {
            for (int pos = 0; pos < Count; ++pos)
            {
                int key = GetKey(pos);
                if (ArrayUtil.BinarySearch(keysToIgnore, 0, keysToIgnore.Length, key) < 0)
                {
                    object attr = GetByIndex(pos);
                    if (!IsInheritedComplexAttribute(attr))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Compares the two collections.
        /// </summary>
        internal static bool Equals(AttrCollection coll1, AttrCollection coll2, int[] keysToIgnore)
        {
            // Since this is sorted collection we can skip ignored keys and compare very next keys and values.
            // In the loop:
            //   Find first non-ignored attribute in each of two collections.
            //   If both are found then compare values.
            //   If both are not found that's okay - no difference found.
            //   If one of collections has an attribute but other has not, check if it is a complex attribute that
            //   has inherited value, otherwise consider collections different.

            int pos1 = 0;
            int pos2 = 0;
            int key1 = 0;
            int key2 = 0;
            bool result = true;

            do
            {
                bool found1 = FindNonIgnoredKey(coll1, ref pos1, ref key1, keysToIgnore);
                bool found2 = FindNonIgnoredKey(coll2, ref pos2, ref key2, keysToIgnore);

                if (!found1 && !found2)
                    break;

                if (found1 && found2 && (key1 != key2))
                {
                    // Process key that is present in one of the collections but none in the other.
                    if (coll2.ContainsKey(key1))
                        found1 = false;
                    else
                        found2 = false;
                }

                if (found1 && found2)
                    result = coll1.GetByIndex(pos1++).Equals(coll2.GetByIndex(pos2++));
                else if (found1)
                    result = IsInheritedComplexAttribute(coll1.GetByIndex(pos1++));
                else
                    result = IsInheritedComplexAttribute(coll2.GetByIndex(pos2++));
            }
            while (result);

            return result;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified value is a complex attribute that has inherited value.
        /// </summary>
        private static bool IsInheritedComplexAttribute(object value1)
        {
            return (value1 is IComplexAttr) && ((IComplexAttr)value1).IsInheritedComplexAttr;
        }

        /// <summary>
        /// Removes attributes which are equal to global defaults.
        /// </summary>
        internal void RemoveGlobalDefaults()
        {
            AttrCollection globalDefaults = GetDefaults();
            RemoveEquals(globalDefaults);
        }

        /// <summary>
        /// Removes attributes, which are the same as in the specified other attribute collection.
        /// </summary>
        internal void RemoveEquals(AttrCollection otherAttrs)
        {
            for (int i = 0; i < otherAttrs.Count; i++)
            {
                int key = otherAttrs.GetKey(i);

                int index = IndexOfKey(key);
                if (index >= 0)
                {
                    // Values exist both in this and in the other collection.
                    // If they are equal, we remove it from this collection.
                    object otherValue = otherAttrs.GetByIndex(i);
                    object value = GetByIndex(index);
                    // Some defaults can be null, for example ShapeAttr.ImageBytes.
                    // Maybe there will be special check for that, but for now just skip nulls.
                    if (otherValue != null && otherValue.Equals(value))
                        RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Returns a copy of the array of collection keys.
        /// </summary>
        internal int[] GetKeys()
        {
            int[] keys = new int[Count];
            for (int i = 0; i < Count; i++)
                keys[i] = GetKey(i);

            return keys;
        }

        /// <summary>
        /// Finds first non-ignored key starting from pos in collection coll.
        /// If anything is found then returns true and sets pos to that key.
        /// </summary>
        private static bool FindNonIgnoredKey(AttrCollection coll, ref int pos, ref int key, int[] keysToIgnore)
        {
            int collCount = coll.Count;
            int ignoreCount = (keysToIgnore != null) ? keysToIgnore.Length : 0;
            while (pos < collCount)
            {
                key = coll.GetKey(pos);
                if ((keysToIgnore == null) || (ArrayUtil.BinarySearch(keysToIgnore, 0, ignoreCount, key) < 0))
                    return true;

                ++pos;
            }
            return false;
        }

        /// <summary>
        /// Returns <b>true</b> if the specified attribute values are the same.
        /// </summary>
        private static bool AreSameValues(object childValue, object baseValue)
        {
            return (childValue is ICustomEquality)
                ? ((ICustomEquality)childValue).HasSameValue((ICustomEquality)baseValue)
                : baseValue.Equals(childValue);
        }

        private static void CollapseExpandableAttrIfNeeded(object childValue, object baseValue)
        {
            if (childValue is IExpandableAttr)
            {
                // Reverse merge the tab stop collection.
                ((IExpandableAttr)childValue).Collapse((IExpandableAttr)baseValue);
            }
        }

        /// <summary>
        /// Calculates hash code for collection.
        /// </summary>
        /// <remarks>
        /// AM. I think that attribute collections within one document which have equal set of keys are equal with high probability.
        /// That's why I don't take attribute values into calculation.
        /// </remarks>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < Count; i++)
            {
                int key = GetKey(i);

                hashCode += key;
                hashCode += (hashCode << 10);
                hashCode ^= (hashCode >> 6);
            }

            return hashCode;
        }

        /// <summary>
        /// Calculates hash code for this collection. Allows to specify keys to ignore during calculation.
        /// </summary>
        internal int GetHashCode(int[] keysToIgnore)
        {
            int hashCode = 0;

            for (int i = 0; i < Count; i++)
            {
                int key = GetKey(i);
                if (ArrayUtil.BinarySearch(keysToIgnore, 0, keysToIgnore.Length, key) < 0)
                {
                    hashCode = (hashCode * 397) ^ key.GetHashCode();

                    object attr = GetByIndex(i);
                    if ((attr != null) && !IsInheritedComplexAttribute(attr))
                        hashCode = (hashCode * 397) ^ attr.GetHashCode();
                }
            }

            return hashCode;
        }

        public bool Equals(AttrCollection rhs)
        {
            // Generated by ReSharper.
            if (ReferenceEquals(null, rhs))
                return false;
            if (ReferenceEquals(this, rhs))
                return true;

            return Equals(rhs, null);
        }

        /// <summary>
        /// Determines whether the specified object is equal in value to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Generated by ReSharper.
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as AttrCollection);
        }

        /// <summary>
        /// Checks if the option that is specified by its key is not defined or has default value.
        /// </summary>
        internal bool IsDefaultValue(int key)
        {
            object value = GetDirectAttr(key);
            if (value == null)
                return true;

            object defaultValue = FetchInheritedAttr(key);
            if (defaultValue == null)
                return false;

            return value.Equals(defaultValue);
        }

        /// <summary>
        /// Reference to pooled collection which holds actual attribute values.
        /// </summary>
        internal InternPoolItem PoolItem
        {
            get { return mPoolItem; }
            set
            {
                Debug.Assert(value == null || mPoolItem == null);
                mPoolItem = value;
            }
        }

        private InternPoolItem mPoolItem;

#if DEBUG && !CPLUSPLUS && !JAVA
        public override string ToString()
        {
            return ToString();
        }

        public string ToString(params int[] filter)
        {
            string result = ToString("; ", filter);
            return string.IsNullOrEmpty(result)
                ? result
                : string.Format("[{0}]", result);
        }

        public void dd(params int[] filter)
        {
            Debug.WriteLine(ToString(Environment.NewLine, filter));
        }

        private string ToString(string delimeter, params int[] filter)
        {
            StringBuilder builder = new StringBuilder();

            Type collectionType = GetType();

            bool hasInclude = System.Linq.Enumerable.Any(filter, p => p > 0);
            bool hasExclude = System.Linq.Enumerable.Any(filter, p => p < 0);

            for (int i = 0; i < Count; i++)
            {
                int key = GetKey(i);

                if (hasInclude && Array.IndexOf(filter, key) == -1)
                    continue;

                if (hasExclude && Array.IndexOf(filter, -key) != -1)
                    continue;

                string keyName = ExtractKeyName(collectionType, key);
                object value = GetByIndex(i);

                if (builder.Length != 0)
                    builder.Append(delimeter);
                builder.Append(string.Format("{0}: {1}", keyName, GetStringValue(key, value)));
            }

            return builder.ToString();
        }

        private static string ExtractKeyName(Type collectionType, int key)
        {
            string keyName;

            IDictionary<int, string> keyNameDictionary;
            if (CollectionTypesDictionary.TryGetValue(collectionType, out keyNameDictionary) &&
                keyNameDictionary.TryGetValue(key, out keyName))
                return keyName;

            if (CollectionTypesDictionary[gUnknownCollectionType].TryGetValue(key, out keyName))
                return keyName;

            return "<Unknown key>";
        }

        private static string GetStringValue(int key, object value)
        {
            switch (key)
            {
                case FontAttr.RsidR:
                case FontAttr.RsidRPr:
                    return Common.FormatterPal.IntToStrX8((int)value);
                default:
                    return value.ToString();
            }
        }

        private static IDictionary<Type, IDictionary<int, string>> gCollectionTypesDictionary;

        private static IDictionary<Type, IDictionary<int, string>> CollectionTypesDictionary
        {
            get
            {
                return gCollectionTypesDictionary ?? (gCollectionTypesDictionary = BuildCollectionTypesDictionary());
            }
        }

        private static IDictionary<Type, IDictionary<int, string>> BuildCollectionTypesDictionary()
        {
            IDictionary<Type, Type[]> collections = new Dictionary<Type, Type[]>
            {
                {typeof(SectPr), new[] {typeof(SectAttr)}},
                {typeof(ParaPr), new[] {typeof(ParaAttr)}},
                {typeof(RunPr), new[] {typeof(FontAttr)}},
                {typeof(TablePr), new[] {typeof(TableAttr)}},
                {typeof(CellPr), new[] {typeof(CellAttr)}},
                {typeof(FormFieldPr), new[] {typeof(FormFieldAttr)}},
                {typeof(ShapePr), new[] {typeof(ShapeAttr)}},
                {typeof(Forms2Pr), new[] {typeof(Forms2Attr)}},
                {typeof(MorphDataControlPr), new[] {typeof(Forms2Attr)}},
                {typeof(FormControlPr), new[] {typeof(Forms2Attr)}},
                {typeof(ImageControlPr), new[] {typeof(Forms2Attr)}},
                {typeof(LabelControlPr), new[] {typeof(Forms2Attr)}},
                {typeof(ScrollBarControlPr), new[] {typeof(Forms2Attr)}},
                {gUnknownCollectionType, new[] {typeof(RevisionAttr)}}
            };

            IDictionary<Type, IDictionary<int, string>> result = new Dictionary<Type, IDictionary<int, string>>();

            foreach (KeyValuePair<Type, Type[]> pair in collections)
            {
                IDictionary<int, string> keyNameDictionary = new Dictionary<int, string>();

                foreach (Type keysType in pair.Value)
                {
                    foreach (FieldInfo field in keysType.GetFields(BindingFlags.Static | BindingFlags.NonPublic))
                    {
                        if (field.FieldType != typeof(int))
                            continue;

                        int key = (int)field.GetValue(null);
                        keyNameDictionary[key] = field.Name;
                    }
                }

                result[pair.Key] = keyNameDictionary;
            }

            return result;
        }

        private static readonly Type gUnknownCollectionType = typeof(object);
#endif
    }
}
