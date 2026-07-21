// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin
using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Drawing;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="Border"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Different document elements have different borders.
    /// For example, <see cref="ParagraphFormat"/> has <see cref="Bottom"/>, <see cref="Left"/>, <see cref="Right"/> and <see cref="Top"/> borders.
    /// You can specify different formatting for each border independently or
    /// enumerate through all borders and apply same formatting.
    /// </remarks>
    /// <dev>
    /// This is a presentation object only, does not store any model data.
    /// </dev>
    public sealed class BorderCollection : IEnumerable<Border>
    {
        internal BorderCollection(IBorderAttrSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Compares collections of borders.
        /// </summary>
        public bool Equals(BorderCollection brColl)
        {
            if (ReferenceEquals(this, brColl))
            {
                return true;
            }

            return (Left.Equals(brColl.Left)) &&
                   (Right.Equals(brColl.Right)) &&
                   (Top.Equals(brColl.Top)) &&
                   (Bottom.Equals(brColl.Bottom)) &&
                   (Horizontal.Equals(brColl.Horizontal)) &&
                   (Vertical.Equals(brColl.Vertical)) &&
                   (Color.Equals(brColl.Color)) &&
                   (ColorInternal.Equals(brColl.ColorInternal)) &&
                   (DistanceFromText.Equals(brColl.DistanceFromText)) &&
                   (LineWidth.Equals(brColl.LineWidth)) &&
                   (LineStyle == brColl.LineStyle) &&
                   (Count == brColl.Count) &&
                   (Shadow == brColl.Shadow) &&
                   (IsVisible == brColl.IsVisible);
        }

        /// <overloads>Retrieves a <see cref="Border"/> object.</overloads>
        /// <summary>
        /// Retrieves a <see cref="Border"/> object by border type.
        /// </summary>
        /// <remarks>
        /// <p>Note that not all borders are present for different document elements.
        /// This method throws an exception if you request a border not applicable to the current object.</p>
        /// </remarks>
        /// <param name="borderType">A <see cref="BorderType"/> value
        /// that specifies the type of the border to retrieve.</param>
        public Border this[BorderType borderType]
        {
            get
            {
                int borderKey;
                if (!mParent.PossibleBorderKeys.TryGetValue(borderType, out borderKey))
                    throw new InvalidOperationException("The requested border is not available for this object.");

                //<<GetOrCreateComplexAttr>> pattern
                Border border = (Border)mParent.GetDirectBorderAttr(borderKey);
                if (border == null)
                {
                    //If the border is not specified directly on this object, create an inherited border.
                    border = new Border(mParent, borderKey);
                    border = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(border, mParent);
                    mParent.SetBorderAttr(borderKey, border);
                }
                return border;
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Retrieves a <see cref="Border"/> object by border type.
        /// </summary>
        public Border GetByBorderType(BorderType borderType)
        {
            return this[borderType];
        }
#endif

        /// <summary>
        /// Retrieves a <see cref="Border"/> object by index.
        /// </summary>
        /// <param name="index">Zero-based index of the border to retrieve.</param>
        public Border this[int index]
        {
            get
            {
                //Convert the index into border type and retrieve the border by border type.
                BorderType borderType = mParent.PossibleBorderKeys.Keys[index];
                return this[borderType];
            }
        }

        /// <summary>
        /// Gets the left border.
        /// </summary>
        public Border Left
        {
            get { return this[BorderType.Left]; }
        }

        /// <summary>
        /// Gets the right border.
        /// </summary>
        public Border Right
        {
            get { return this[BorderType.Right]; }
        }

        /// <summary>
        /// Gets the top border.
        /// </summary>
        public Border Top
        {
            get { return this[BorderType.Top]; }
        }

        /// <summary>
        /// Gets the bottom border.
        /// </summary>
        public Border Bottom
        {
            get { return this[BorderType.Bottom]; }
        }

        /// <summary>
        /// Gets the horizontal border that is used between cells or conforming paragraphs.
        /// </summary>
        public Border Horizontal
        {
            get { return this[BorderType.Horizontal]; }
        }

        /// <summary>
        /// Gets the vertical border that is used between cells.
        /// </summary>
        public Border Vertical
        {
            get { return this[BorderType.Vertical]; }
        }

        /// <summary>
        /// Gets the number of borders in the collection.
        /// </summary>
        public int Count
        {
            get { return mParent.PossibleBorderKeys.Count; }
        }

        /// <summary>
        /// Gets or sets the border width in points.
        /// </summary>
        /// <remarks>
        /// <p>Returns the width of the first border in the collection.</p>
        /// <p>Sets the width of all borders in the collection excluding diagonal borders.</p>
        /// </remarks>
        public double LineWidth
        {
            get { return this[0].LineWidth; }
            set
            {
                foreach (BorderType borderType in mParent.PossibleBorderKeys.Keys)
                {
                    if (IsCommonBorder(borderType))
                        this[borderType].LineWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        /// <remarks>
        /// <p>Returns the style of the first border in the collection.</p>
        /// <p>Sets the style of all borders in the collection excluding diagonal borders.</p>
        /// </remarks>
        public LineStyle LineStyle
        {
            get { return this[0].LineStyle; }
            set
            {
                foreach (BorderType borderType in mParent.PossibleBorderKeys.Keys)
                {
                    if (IsCommonBorder(borderType))
                        this[borderType].LineStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        /// <remarks>
        /// <p>Returns the color of the first border in the collection.</p>
        /// <p>Sets the color of all borders in the collection excluding diagonal borders.</p>
        /// </remarks>
        public System.Drawing.Color Color
        {
            get { return ColorInternal.ToNativeColor(); }
            set { ColorInternal = DrColor.FromNativeColor(value); }
        }

        internal DrColor ColorInternal
        {
            get { return this[0].ColorInternal; }
            set
            {
                foreach (BorderType borderType in mParent.PossibleBorderKeys.Keys)
                {
                    if (IsCommonBorder(borderType))
                        this[borderType].ColorInternal = value;
                }
            }
        }

        internal bool Frame
        {
            get { return this[0].Frame; }
            set
            {
                foreach (BorderType borderType in mParent.PossibleBorderKeys.Keys)
                {
                    if (IsCommonBorder(borderType))
                        this[borderType].Frame = value;
                }
            }
        }

        /// <summary>
        /// Returns True if specified border is available in the collection.
        /// </summary>
        internal bool IsPossible(BorderType borderType)
        {
            return mParent.PossibleBorderKeys.ContainsKey(borderType);
        }

        /// <summary>
        /// Gets or sets distance of the border from text in points.
        /// </summary>
        /// <remarks>
        /// <p>Gets the distance from text for the first border.</p>
        /// <p>Sets the distance from text for all borders in the collection excluding diagonal borders.</p>
        /// <p>Has no effect and will be automatically reset to zero for borders of table cells.</p>
        /// </remarks>
        public double DistanceFromText
        {
            get { return this[0].DistanceFromText; }
            set
            {
                foreach (BorderType borderType in mParent.PossibleBorderKeys.Keys)
                {
                    if (IsCommonBorder(borderType))
                        this[borderType].DistanceFromText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the border has a shadow.
        /// </summary>
        /// <remarks>
        /// <p>Gets the value from the first border in the collection.</p>
        /// <p>Sets the value for all borders in the collection excluding diagonal borders.</p>
        /// </remarks>
        public bool Shadow
        {
            get { return this[0].Shadow; }
            set
            {
                foreach (BorderType borderType in mParent.PossibleBorderKeys.Keys)
                {
                    if (IsCommonBorder(borderType))
                        this[borderType].Shadow = value;
                }
            }
        }

        /// <summary>
        /// Removes all borders of an object.
        /// </summary>
        public void ClearFormatting()
        {
            foreach (Border border in this)
                border.ClearFormatting();
        }

        /// <summary>
        /// Returns an enumerator object that can be used to iterate over all borders in the collection.
        /// </summary>
        public IEnumerator<Border> GetEnumerator()
        {
            return new BordersEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator object that can be used to iterate over all borders in the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns true if at least one of the borders is visible.
        /// </summary>
        internal bool IsVisible
        {
            get
            {
                foreach (Border border in this)
                {
                    if (border.IsVisible)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns true if this is not a diagonal border.
        /// </summary>
        private static bool IsCommonBorder(BorderType borderType)
        {
            return ((borderType != BorderType.DiagonalDown) && (borderType != BorderType.DiagonalUp));
        }

        /// <summary>
        /// Enumerates over the borders of an object. We need to implement this enumerator because:
        /// 1. The borders are not stored in a single collection that I can simply enumerate.
        /// 2. Some borders that might not yet exist and need to be created on demand.
        /// </summary>
        private sealed class BordersEnumerator : IEnumerator<Border>
        {
            internal BordersEnumerator(BorderCollection borders)
            {
                mBorders = borders;
                mBorderIndex = -1;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public bool MoveNext()
            {
                if (mBorderIndex >= (mBorders.Count - 1))
                    return false;

                mBorderIndex++;
                return true;
            }

            public void Reset()
            {
                mBorderIndex = -1;
            }

            [JavaConvertCheckedExceptions]
            public Border Current
            {
                get { return mBorders[mBorderIndex]; }
            }
            
            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly BorderCollection mBorders;
            private int mBorderIndex;
        }

#if CPLUSPLUS // Method is implemented in C++ to set Shared mode for weak mParent.
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        internal void KeepParentAsSharedPtr()
        {
        }
#endif

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IBorderAttrSource mParent;
    }
}
