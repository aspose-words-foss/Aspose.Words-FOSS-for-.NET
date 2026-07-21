// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2005 by Roman Korchagin

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a single text column. <see cref="TextColumn"/> is a member of the <see cref="TextColumnCollection"/> collection.
    /// The <see cref="TextColumn"/> collection includes all the columns in a section of a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-sections/">Working with Sections</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="TextColumn"/> objects are only used to specify columns with custom width and spacing. If you want
    /// the columns in the document to be of equal width, set TextColumns.<see cref="TextColumnCollection.EvenlySpaced"/> to <c>true</c>.</p>
    /// <p>When a new <see cref="TextColumn"/> is created it has its width and spacing set to zero.</p>
    /// <seealso cref="TextColumnCollection"/>
    /// <seealso cref="PageSetup"/>
    /// <seealso cref="Section"/>
    /// </remarks>
    /// <dev>
    /// This is both a presentation and a model class. Represents width and spacing of a single column
    /// in a section. Only used for custom width columns (not for evenly spaced columns).
    /// </dev>
    public class TextColumn
    {
        /// <summary>
        /// Creates a column with zero width and space after.
        /// </summary>
        internal TextColumn()
        {
        }

        /// <summary>
        /// Gets or sets the width of the text column in points.
        /// </summary>
        public double Width
        {
            get { return ConvertUtilCore.TwipToPoint(mWidth); }
            set
            {
                if (0 > value)
                    throw new ArgumentOutOfRangeException("value");

                int twipValue = ConvertUtilCore.PointToTwip(value);
                mWidth = twipValue;
            }
        }

        /// <summary>
        /// Gets or sets the space between this column and the next column in points. Not required for the last column.
        /// </summary>
        public double SpaceAfter
        {
            get { return ConvertUtilCore.TwipToPoint(mSpaceAfter); }
            set 
            {
                if (0 > value)
                    throw new ArgumentOutOfRangeException("value");

                int twipValue = ConvertUtilCore.PointToTwip(value);
                mSpaceAfter = twipValue;
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        internal TextColumn Clone()
        {
            TextColumn clone = (TextColumn)MemberwiseClone();
            return clone;
        }

        /// <summary>
        /// Width of the column in twips. This is used by codecs.
        /// </summary>
        internal int RawWidth
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Space after the column in twips. This is used by codecs.
        /// </summary>
        internal int RawSpaceAfter
        {
            get { return mSpaceAfter; }
            set { mSpaceAfter = value; }
        }

        /// <summary>
        /// Returns <b>true</b> if the column has non-empty width or space after.
        /// </summary>
        internal bool HasData
        {
            get { return (mWidth > 0) || (mSpaceAfter > 0); }
        }

        private int mWidth;
        private int mSpaceAfter;
    }
}
