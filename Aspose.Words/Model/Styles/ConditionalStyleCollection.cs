// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/04/2019 by Alexander Zhiltsov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a collection of <see cref="ConditionalStyle"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// It is not possible to add or remove items from this collection. It contains permanent set of items: one item for
    /// each value of the <see cref="ConditionalStyleType"/> enumeration type.
    /// </remarks>
    /// <dev>
    /// With using public properties of the class, it is possible to get only conditional styles that correspond to items
    /// of the <see cref="ConditionalStyleType"/> enumeration type. But a collection may contain also styles related
    /// to <see cref="TableStyleOverrideType.None"/> and <see cref="TableStyleOverrideType.WholeTable"/> values, use the
    /// this[TableStyleOverrideType] or <see cref="DefinedStyles"/> properties to get them.
    /// </dev>
    public sealed class ConditionalStyleCollection : IEnumerable<ConditionalStyle>
    {
        internal ConditionalStyleCollection(TableStyle parentStyle)
        {
            mParentStyle = parentStyle;
        }

        /// <summary>
        /// Clears all conditional styles of the table style.
        /// </summary>
        public void ClearFormatting()
        {
            // Let's clear only formatting included in public API, i.e. not clear formatting of whole table and for
            // TableStyleOverrideType.None value that means whole table too.

            ConditionalStyle noneStyle = mItems[(int)TableStyleOverrideType.None];
            ConditionalStyle tableStyle = mItems[(int)TableStyleOverrideType.WholeTable];

            mItems.Clear();

            if (noneStyle != null)
                mItems.Add((int)TableStyleOverrideType.None, noneStyle);
            if (tableStyle != null)
                mItems.Add((int)TableStyleOverrideType.WholeTable, tableStyle);
        }

        /// <summary>
        /// Returns an enumerator object that can be used to iterate over all conditional styles in the collection.
        /// </summary>
        /// <dev>
        /// Conditional styles related to <see cref="TableStyleOverrideType.None"/> and
        /// <see cref="TableStyleOverrideType.WholeTable"/> values are not included into the enumeration.
        /// </dev>
        public IEnumerator<ConditionalStyle> GetEnumerator()
        {
            return new ConditionalStylesEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator object that can be used to iterate over all conditional styles in the collection.
        /// </summary>
        /// <dev>
        /// Conditional styles related to <see cref="TableStyleOverrideType.None"/> and
        /// <see cref="TableStyleOverrideType.WholeTable"/> values are not included into the enumeration.
        /// </dev>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a conditional style into the collection.
        /// </summary>
        internal void Add(ConditionalStyle conditionalStyle)
        {
            mItems.Add((int)conditionalStyle.OverrideType, conditionalStyle);
            conditionalStyle.SetParentStyle(mParentStyle);
        }

        /// <summary>
        /// Moves the collection to the default state with no conditional styles defined.
        /// </summary>
        internal void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Returns a flag indicating whether the collection contains a defined conditional style of that type.
        /// </summary>
        internal bool ContainsTableStyleOverride(TableStyleOverrideType type)
        {
            return mItems.ContainsKey((int)type);
        }

        /// <summary>
        /// Returns a flag indicating whether the collection contains a defined conditional style of that type.
        /// </summary>
        internal bool ContainsConditionalStyle(ConditionalStyleType type)
        {
            return ContainsTableStyleOverride(ConditionalStyleTypeToOverrideType(type));
        }

        /// <summary>
        /// Converts value of <see cref="ConditionalStyleType"/> type to <see cref="TableStyleOverrideType"/>.
        /// </summary>
        private static TableStyleOverrideType ConditionalStyleTypeToOverrideType(ConditionalStyleType value)
        {
            switch (value)
            {
                case ConditionalStyleType.FirstRow:
                    return TableStyleOverrideType.FirstRow;
                case ConditionalStyleType.FirstColumn:
                    return TableStyleOverrideType.FirstColumn;
                case ConditionalStyleType.LastRow:
                    return TableStyleOverrideType.LastRow;
                case ConditionalStyleType.LastColumn:
                    return TableStyleOverrideType.LastColumn;
                case ConditionalStyleType.OddRowBanding:
                    return TableStyleOverrideType.OddRowBanding;
                case ConditionalStyleType.OddColumnBanding:
                    return TableStyleOverrideType.OddColumnBanding;
                case ConditionalStyleType.EvenRowBanding:
                    return TableStyleOverrideType.EvenRowBanding;
                case ConditionalStyleType.EvenColumnBanding:
                    return TableStyleOverrideType.EvenColumnBanding;
                case ConditionalStyleType.TopLeftCell:
                    return TableStyleOverrideType.TopLeftCell;
                case ConditionalStyleType.TopRightCell:
                    return TableStyleOverrideType.TopRightCell;
                case ConditionalStyleType.BottomLeftCell:
                    return TableStyleOverrideType.BottomLeftCell;
                case ConditionalStyleType.BottomRightCell:
                    return TableStyleOverrideType.BottomRightCell;
                default:
                {
                    Debug.Assert(false);
                    return TableStyleOverrideType.None;
                }
            }
        }

        /// <overloads>
        /// Retrieves a <see cref="ConditionalStyle"/> object.
        /// </overloads>
        /// <summary>
        /// Retrieves a <see cref="ConditionalStyle"/> object by conditional style type.
        /// </summary>
        public ConditionalStyle this[ConditionalStyleType conditionalStyleType]
        {
            get
            {
                TableStyleOverrideType type = ConditionalStyleTypeToOverrideType(conditionalStyleType);
                return this[type];
            }
        }

        /// <summary>
        /// Retrieves a <see cref="ConditionalStyle"/> object by table style override type.
        /// </summary>
        internal ConditionalStyle this[TableStyleOverrideType overrideType]
        {
            get
            {
                ConditionalStyle result;
                if (!mItems.ContainsKey((int)overrideType))
                {
                    result = new ConditionalStyle(overrideType, mParentStyle);
                    mItems.Add((int)overrideType, result);
                }
                else
                {
                    result = mItems[(int)overrideType];
                }

                return result;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="ConditionalStyle"/> object by index.
        /// </summary>
        /// <param name="index">Zero-based index of the conditional style to retrieve.</param>
        public ConditionalStyle this[int index]
        {
            get
            {
                ConditionalStyleType[] types = (ConditionalStyleType[])Enum.GetValues(typeof(ConditionalStyleType));
                return this[types[index]];
            }
        }

        /// <summary>
        /// Gets the number of conditional styles in the collection.
        /// </summary>
        public int Count
        {
            get { return gCount; }
        }

        /// <summary>
        /// Gets all defined conditional styles of the collection including internal ones (None and WholeTable).
        /// </summary>
        internal ICollection<ConditionalStyle> DefinedStyles
        {
            get
            {
                return new List<ConditionalStyle>(mItems.Values);
            }
        }

        /// <summary>
        /// Gets the first row style.
        /// </summary>
        public ConditionalStyle FirstRow
        {
            get { return this[ConditionalStyleType.FirstRow]; }
        }

        /// <summary>
        /// Gets the first column style.
        /// </summary>
        public ConditionalStyle FirstColumn
        {
            get { return this[ConditionalStyleType.FirstColumn]; }
        }

        /// <summary>
        /// Gets the last row style.
        /// </summary>
        public ConditionalStyle LastRow
        {
            get { return this[ConditionalStyleType.LastRow]; }
        }

        /// <summary>
        /// Gets the last column style.
        /// </summary>
        public ConditionalStyle LastColumn
        {
            get { return this[ConditionalStyleType.LastColumn]; }
        }

        /// <summary>
        /// Gets the odd row banding style.
        /// </summary>
        public ConditionalStyle OddRowBanding
        {
            get { return this[ConditionalStyleType.OddRowBanding]; }
        }

        /// <summary>
        /// Gets the odd column banding style.
        /// </summary>
        public ConditionalStyle OddColumnBanding
        {
            get { return this[ConditionalStyleType.OddColumnBanding]; }
        }

        /// <summary>
        /// Gets the even row banding style.
        /// </summary>
        public ConditionalStyle EvenRowBanding
        {
            get { return this[ConditionalStyleType.EvenRowBanding]; }
        }

        /// <summary>
        /// Gets the even column banding style.
        /// </summary>
        public ConditionalStyle EvenColumnBanding
        {
            get { return this[ConditionalStyleType.EvenColumnBanding]; }
        }

        /// <summary>
        /// Gets the top left cell style.
        /// </summary>
        public ConditionalStyle TopLeftCell
        {
            get { return this[ConditionalStyleType.TopLeftCell]; }
        }

        /// <summary>
        /// Gets the top right cell style.
        /// </summary>
        public ConditionalStyle TopRightCell
        {
            get { return this[ConditionalStyleType.TopRightCell]; }
        }

        /// <summary>
        /// Gets the bottom left cell style.
        /// </summary>
        public ConditionalStyle BottomLeftCell
        {
            get { return this[ConditionalStyleType.BottomLeftCell]; }
        }

        /// <summary>
        /// Gets the bottom right cell style.
        /// </summary>
        public ConditionalStyle BottomRightCell
        {
            get { return this[ConditionalStyleType.BottomRightCell]; }
        }


        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly TableStyle mParentStyle;
        // Dictionary<Enum,ConditionalStyle> is changed to IntToObjDictionary<ConditionalStyle> for Java
        // to get consistent sorting (Enum is autoported as static int constant to Java).
        private readonly IntToObjDictionary<ConditionalStyle> mItems = new IntToObjDictionary<ConditionalStyle>();

        private static readonly int gCount =
            EnumUtilPal.GetEffectiveArrayLength(ConditionalStyleType.OddRowBanding.GetType(), 12);

        /// <summary>
        /// Enumerates over the conditional styles of a collection. We need to implement this enumerator because it
        /// should include all possible public conditional styles (without None and WholeTable) even non-created yet.
        /// </summary>
        private sealed class ConditionalStylesEnumerator : IEnumerator<ConditionalStyle>
        {
            internal ConditionalStylesEnumerator(ConditionalStyleCollection collection)
            {
                mCollection = collection;
                Reset();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public bool MoveNext()
            {
                if (mIndex >= (mCollection.Count - 1))
                    return false;

                mIndex++;
                return true;
            }

            public void Reset()
            {
                mIndex = -1;
            }

            [JavaConvertCheckedExceptions]
            public ConditionalStyle Current
            {
                get { return mCollection[mIndex]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly ConditionalStyleCollection mCollection;
            private int mIndex;
        }
    }
}
