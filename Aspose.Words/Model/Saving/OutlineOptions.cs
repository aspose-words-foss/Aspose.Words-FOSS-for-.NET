// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2013 by Konstantin Kornilov

using System;
using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Allows to specify outline options.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    public class OutlineOptions
    {
        /// <summary>
        /// <para>Gets or sets a value determining whether or not to create missing outline levels when the document is
        /// exported.</para>
        /// <para>Default value for this property is <c>false</c>.</para>
        /// </summary>
        public bool CreateMissingOutlineLevels
        {
            get { return mCreateMissingOutlineLevels; }
            set { mCreateMissingOutlineLevels = value; }
        }


        /// <summary>
        /// Specifies how many levels of headings (paragraphs formatted with the Heading styles) to include in the
        /// document outline.
        /// </summary>
        /// <remarks>
        /// <para>Specify 0 for no headings in the outline; specify 1 for one level of headings in the outline and so on.</para>
        /// <para>Default is 0. Valid range is 0 to 9.</para>
        /// </remarks>
        public int HeadingsOutlineLevels
        {
            get { return mHeadingsOutlineLevels; }
            set
            {
                if ((value < 0) || (value > 9))
                    throw new ArgumentOutOfRangeException("value");

                mHeadingsOutlineLevels = value;
            }
        }

        /// <summary>
        /// Specifies how many levels in the document outline to show expanded when the file is viewed.
        /// </summary>
        /// <remarks>
        /// <para>Note that this options will not work when saving to XPS.</para>
        /// <para>Specify 0 and the document outline will be collapsed; specify 1 and the first level items
        /// in the outline will be expanded and so on.</para>
        /// <para>Default is 0. Valid range is 0 to 9.</para>
        /// </remarks>
        public int ExpandedOutlineLevels
        {
            get { return mExpandedOutlineLevels; }
            set
            {
                if ((value < 0) || (value > 9))
                    throw new ArgumentOutOfRangeException("value");

                mExpandedOutlineLevels = value;
            }
        }

        /// <summary>
        /// Specifies the default level in the document outline at which to display Word bookmarks.
        /// </summary>
        /// <remarks>
        /// <para>Individual bookmarks level could be specified using <see cref="BookmarksOutlineLevels"/> property.</para>
        /// <para>Specify 0 and Word bookmarks will not be displayed in the document outline.
        /// Specify 1 and Word bookmarks will be displayed in the document outline at level 1; 2 for level 2 and so on.</para>
        /// <para>Default is 0. Valid range is 0 to 9.</para>
        /// </remarks>
        public int DefaultBookmarksOutlineLevel
        {
            get { return mDefaultBookmarksOutlineLevel; }
            set
            {
                if ((value < 0) || (value > 9))
                    throw new ArgumentOutOfRangeException("value");

                mDefaultBookmarksOutlineLevel = value;
            }
        }

        /// <summary>
        /// Allows to specify individual bookmarks outline level.
        /// </summary>
        /// <remarks>
        /// <para>If bookmark level is not specified in this collection then <see cref="DefaultBookmarksOutlineLevel"/> value is used.</para>
        /// </remarks>
        public BookmarksOutlineLevelCollection BookmarksOutlineLevels
        {
            get { return mBookmarksOutlineLevels; }
        }

        /// <summary>
        /// Specifies whether or not to create outlines for headings (paragraphs formatted with the Heading styles) inside tables.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <c>false</c>.</para>
        /// </remarks>
        public bool CreateOutlinesForHeadingsInTables
        {
            get { return mCreateHeadingOutlinesFromTables; }
            set { mCreateHeadingOutlinesFromTables = value; }
        }

        private int mHeadingsOutlineLevels;
        private int mExpandedOutlineLevels;
        private int mDefaultBookmarksOutlineLevel;
        private readonly BookmarksOutlineLevelCollection mBookmarksOutlineLevels = new BookmarksOutlineLevelCollection();
        private bool mCreateMissingOutlineLevels;
        private bool mCreateHeadingOutlinesFromTables;
    }
}
