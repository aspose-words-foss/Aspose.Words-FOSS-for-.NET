// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2005 by Roman Korchagin
using System;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="TextColumn"/> objects that represent all the columns of text in a section of a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-sections/">Working with Sections</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use <see cref="SetCount"/> to set the number of text columns.</p>
    /// <p>To make all columns equal width and spaced evenly, set <see cref="EvenlySpaced"/> to <c>true</c>
    /// and specify the amount of space between the columns in <see cref="Spacing"/>. MS Word will
    /// automatically calculate column widths.</p>
    /// <p>If you have <see cref="EvenlySpaced"/> set to <c>false</c>, you need to specify width and spacing for each
    /// column individually. Use the indexer to access individual <see cref="TextColumn"/> objects.</p>
    /// <p>When using custom column widths, make sure the sum of all column widths and spacings between them
    /// equals page width minus left and right page margins.</p>
    /// <seealso cref="PageSetup"/>
    /// <seealso cref="Section"/>
    /// </remarks>
    /// <dev>
    /// This is a pure presentation class. Does not store any model data, just provides friendly
    /// access to section attributes that are related to managing text columns.
    /// </dev>
    public class TextColumnCollection
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="pageSetup">Essentially we just need an attribute source,
        /// but we use page width and margins in some calculations here so it is just convenient
        /// to know the page setup object and we can obtain attribute source from it too.</param>
        internal TextColumnCollection(PageSetup pageSetup)
        {
            mPageSetup = pageSetup;
        }

        /// <summary>
        /// Arranges text into the specified number of text columns.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="EvenlySpaced"/> is <c>false</c> and you increase the number of columns,
        /// new <see cref="TextColumn"/> objects are created with zero width and spacing.
        /// You need to set width and spacing for the new columns.</p>
        /// </remarks>
        /// <param name="newCount">The number of columns the text is to be arranged into.</param>
        public void SetCount(int newCount)
        {
            if ((newCount < MinColumns) || (newCount > MaxColumns))
                throw new ArgumentOutOfRangeException("newCount");

            SetAttr(SectAttr.ColumnsCount, newCount);
        }

        /// <summary>
        /// True if text columns are of equal width and evenly spaced.
        /// </summary>
        public bool EvenlySpaced
        {
            get { return (bool)FetchAttr(SectAttr.ColumnsEvenlySpaced); }
            set { SetAttr(SectAttr.ColumnsEvenlySpaced, value); }
        }

        /// <summary>
        /// When columns are evenly spaced, gets or sets the amount of space between each column in points.
        /// </summary>
        /// <remarks>
        /// Has effect only when <see cref="EvenlySpaced"/> is set to <c>true</c>.
        /// </remarks>
        public double Spacing
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.ColumnsSpacing)); }
            set { SetAttr(SectAttr.ColumnsSpacing, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// When columns are evenly spaced, gets the width of the columns.
        /// </summary>
        /// <remarks>
        /// <p>Has effect only when <see cref="EvenlySpaced"/> is set to <c>true</c>.</p>
        /// </remarks>
        public double Width
        {
            get
            {
                double totalColumnsWidth = mPageSetup.ContentWidthInTextDirection - Spacing * (Count - 1);
                return totalColumnsWidth / Count;
            }
        }

        /// <summary>
        /// When <c>true</c>, adds a vertical line between columns.
        /// </summary>
        public bool LineBetween
        {
            get { return (bool)FetchAttr(SectAttr.ColumnsLineBetween); }
            set { mPageSetup.Parent.SetSectionAttr(SectAttr.ColumnsLineBetween, value); }
        }

        /// <summary>
        /// Gets the number of columns in the section of a document.
        /// </summary>
        public int Count
        {
            get { return ColumnsCount; }
        }

        /// <summary>
        /// Returns a text column at the specified index.
        /// </summary>
        public TextColumn this[int index]
        {
            get { return Columns[index]; }
        }

        /// <summary>
        /// Gets the attribute from the collection or from default attributes, throws if cannot get it.
        /// </summary>
        private object FetchAttr(int key)
        {
            return mPageSetup.Parent.FetchSectionAttr(key);
        }

        /// <summary>
        /// Gets or sets "SectAttr.ColumnsCount".
        /// </summary>
        private int ColumnsCount
        {
            get { return (int)FetchAttr(SectAttr.ColumnsCount); }
            set { mPageSetup.Parent.SetSectionAttr(SectAttr.ColumnsCount, value); }
        }

        /// <summary>
        /// Gets or sets the model data that represents custom column widths.
        /// Automatically creates the columns in the model if they are not yet there.
        /// If the custom columns were automatically created, their width and spacing will be all zeroes.
        /// </summary>
        /// <remarks>This method creates column attribute automatically.</remarks>
        private TextColumnCollectionInternal Columns
        {
            get
            {
                //<<GetOrCreateComplexAttr>> pattern
                TextColumnCollectionInternal columns = (TextColumnCollectionInternal)mPageSetup.Parent.GetDirectSectionAttr(SectAttr.Columns);
                if (null == columns)
                {
                    columns = new TextColumnCollectionInternal();
                    columns.SetCount(ColumnsCount);
                    mPageSetup.Parent.SetSectionAttr(SectAttr.Columns, columns);
                }
                return columns;
            }
        }

        internal const int MinColumns = 1;
        internal const int MaxColumns = 45;      // As per Word 2003.

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly PageSetup mPageSetup;

        /// <summary>
        /// This method is compiled in Aspose.Words.
        /// Sets value of an attribute.
        /// </summary>
        private void SetAttr(int key, object value)
        {
            if (key == SectAttr.ColumnsCount)
            {
                int newCount = (int)value;

                // Keep the column counter and the actual number of columns in sync.
                ColumnsCount = newCount;
                Columns.SetCount(newCount);
            }
            else
            {
                mPageSetup.Parent.SetSectionAttr(key, value);
            }
        }
    }
}
