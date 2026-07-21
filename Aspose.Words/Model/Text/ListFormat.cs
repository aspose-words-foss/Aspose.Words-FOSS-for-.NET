// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2004 by Roman Korchagin

using System;
using Aspose.JavaAttributes;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Allows to control what list formatting is applied to a paragraph.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-lists/">Working with Lists</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A paragraph in a Microsoft Word document can be bulleted or numbered.
    /// When a paragraph is bulleted or numbered, it is said that list formatting
    /// is applied to the paragraph.</p>
    ///
    /// <p>You do not create objects of the <see cref="ListFormat"/> class directly.
    /// You access <see cref="ListFormat"/> as a property of another object that can
    /// have list formatting associated with it. At the moment the objects that can
    /// have list formatting are: <see cref="Aspose.Words.Paragraph"/>,
    /// <see cref="Aspose.Words.Style"/> and <see cref="Aspose.Words.DocumentBuilder"/>.</p>
    ///
    /// <p><see cref="ListFormat"/> of a <see cref="Aspose.Words.Paragraph"/> specifies
    /// what list formatting and list level is applied to that particular paragraph.</p>
    ///
    /// <p><see cref="ListFormat"/> of a <see cref="Aspose.Words.Style"/> (applicable
    /// to paragraph styles only) allows to specify what list formatting and list level
    /// is applied to all paragraphs of that particular style.</p>
    ///
    /// <p><see cref="ListFormat"/> of a <see cref="Aspose.Words.DocumentBuilder"/>
    /// provides access to the list formatting at the current cursor position
    /// inside the <see cref="Aspose.Words.DocumentBuilder"/>.</p>
    ///
    /// <p>The list formatting itself is stored inside a <see cref="Lists.List"/>
    /// object that is stored separately from the paragraphs. The list objects
    /// are stored inside a <see cref="ListCollection"/> collection. There is a single
    /// <see cref="ListCollection"/> collection per <see cref="Aspose.Words.Document"/>.</p>
    ///
    /// <p>The paragraphs do not physically belong to a list. The paragraphs just
    /// reference a particular list object via the <see cref="List"/> property
    /// and a particular level in the list via the <see cref="ListLevelNumber"/> property.
    /// By setting these two properties you control what bullets and numbering is
    /// applied to a paragraph.</p>
    /// </remarks>
    public class ListFormat
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="parentParaAttrs">The object that provides list paragraph formatting attributes.</param>
        /// <param name="parentRunAttrs">The object that provides list run formatting attributes.</param>
        /// <param name="lists">Lists of the document.</param>
        internal ListFormat(IParaAttrSource parentParaAttrs, IRunAttrSource parentRunAttrs, ListCollection lists)
        {
            mParentParaAttrs = parentParaAttrs;
            mParentRunAttrs = parentRunAttrs;
            mLists = lists;
        }

        /// <summary>
        /// Starts a new default bulleted list and applies it to the paragraph.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut method that creates a new list using the default bulleted
        /// template, applies it to the paragraph and selects the 1st list level.</p>
        ///
        /// <seealso cref="List"/>
        /// <seealso cref="RemoveNumbers"/>
        /// <seealso cref="ListLevelNumber"/>
        /// </remarks>
        public void ApplyBulletDefault()
        {
            if(mLists.Count > MaxListCount)
                WarningUtil.WarnDataLoss(mLists.Document.WarningCallback, WarningSource.Validator, WarningStrings.ListCountExceedLimit);

            // Set current paragraph ilvl and ilfo.
            ListId = mLists.Add(ListTemplate.BulletDefault).ListId;
            ListLevelNumber = 0;
            Invalidate();
        }

        /// <summary>
        /// Starts a new default numbered list and applies it to the paragraph.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut method that creates a new list using the default numbered
        /// template, applies it to the paragraph and selects the 1st list level.</p>
        ///
        /// <seealso cref="List"/>
        /// <seealso cref="RemoveNumbers"/>
        /// <seealso cref="ListLevelNumber"/>
        /// </remarks>
        public void ApplyNumberDefault()
        {
            if (mLists.Count > MaxListCount)
                WarningUtil.WarnDataLoss(mLists.Document.WarningCallback, WarningSource.Validator, WarningStrings.ListCountExceedLimit);

            ListId = mLists.Add(ListTemplate.NumberDefault).ListId;
            ListLevelNumber = 0;
            Invalidate();
        }

        /// <summary>
        /// Removes numbers or bullets from the current paragraph and sets list level to zero.
        /// </summary>
        /// <remarks>
        /// <p>Calling this method is equivalent to setting the <see cref="ListFormat.List"/> property to <c>null</c>.</p>
        /// </remarks>
        public void RemoveNumbers()
        {
            List = null;
            Invalidate();
        }

        /// <summary>
        /// Increases the list level of the current paragraph by one level.
        /// </summary>
        /// <remarks>
        /// <p>This method changes the list level and applies formatting properties of the new level.</p>
        ///
        /// <p>In Word documents, lists may consist of up to nine levels. List formatting
        /// for each level specifies what bullet or number is used, left indent, space between
        /// the bullet and text etc.</p>
        /// </remarks>
        [JavaThrows(true)]
        public void ListIndent()
        {
            if (ListLevelNumberOriginal < ListLevel.MaxLevels - 1)
            {
                ListLevelNumber = ListLevelNumberOriginal + 1;
                Invalidate();
            }
        }

        /// <summary>
        /// Decreases the list level of the current paragraph by one level.
        /// </summary>
        /// <remarks>
        /// <p>This method changes the list level and applies formatting properties of the new level.</p>
        ///
        /// <p>In Word documents, lists may consist of up to nine levels. List formatting
        /// for each level specifies what bullet or number is used, left indent, space between
        /// the bullet and text etc.</p>
        /// </remarks>
        [JavaThrows(true)]
        public void ListOutdent()
        {
            if (ListLevelNumberOriginal > 0)
            {
                ListLevelNumber = ListLevelNumberOriginal - 1;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the list level number (0 to 8) for the paragraph.
        /// </summary>
        /// <remarks>
        /// <p>In Word documents, lists may consist of 1 or 9 levels, numbered 0 to 8.</p>
        ///
        /// <p>Has effect only when the <see cref="List"/> property is set to reference a valid list.</p>
        ///
        /// <seealso cref="List"/>
        /// </remarks>
        public int ListLevelNumber
        {
            get
            {
                // WORDSNET-21552 Return "final" value when selected revised version of a document.
                Document doc = mLists.Document as Document;
                if ((doc != null) && (doc.RevisionsView == RevisionsView.Final))
                    return ListLevelNumberFinal;

                return ListLevelNumberOriginal;
            }
            set
            {
                mParentParaAttrs.SetParaAttr(ParaAttr.ListLevel, value);
                Invalidate();
            }
        }

        internal int ListLevelNumberOriginal
        {
            get
            {
                return (int)mParentParaAttrs.FetchParaAttr(ParaAttr.ListLevel);
            }
        }

        internal int ListLevelNumberFinal
        {
            get
            {
                Paragraph paragraph = mParentParaAttrs as Paragraph;
                if (paragraph != null)
                    return (int)paragraph.FetchParaAttr(ParaAttr.ListLevel, RevisionsView.Final);

                return ListLevelNumberOriginal;
            }
        }

        /// <summary>
        /// True when the paragraph has bulleted or numbered formatting applied to it.
        /// </summary>
        public bool IsListItem
        {
            get { return (ListId != 0); }
        }

        /// <summary>
        /// Gets or sets the list this paragraph is a member of.
        /// </summary>
        /// <remarks>
        /// <p>The list that is being assigned to this property must belong to the current document.</p>
        ///
        /// <p>The list that is being assigned to this property must not be a list style definition.</p>
        ///
        /// <p>Setting this property to <c>null</c> removes bullets and numbering from the paragraph
        /// and sets the list level number to zero. Setting this property to <c>null</c> is equivalent
        /// to calling <see cref="RemoveNumbers"/>.</p>
        ///
        /// <seealso cref="ListLevelNumber"/>
        /// <seealso cref="RemoveNumbers"/>
        /// </remarks>
        public List List
        {
            get
            {
                int listId = ListId;
                return (listId != 0) ? mLists.FetchListByListId(listId) : null;
            }
            set
            {
                if (value == null)
                {
                    ListId = 0;
                    ListLevelNumber = 0;
                }
                else
                {
                    if (value.Document != mLists.Document)
                        throw new ArgumentException("The list belongs to a different document.");

                    if (value.IsListStyleDefinition)
                        throw new ArgumentException("The list is a definition of a list style.");

                    ListId = value.ListId;
                }

                Invalidate();
            }
        }

        internal List ListFinal
        {
            get
            {
                int listId = ListIdFinal;
                return (listId != 0) ? mLists.FetchListByListId(listId) : null;
            }
        }

        /// <summary>
        /// Returns the list level formatting plus any formatting overrides applied to the current paragraph.
        /// </summary>
        [JavaConvertCheckedExceptions]
        public ListLevel ListLevel
        {
            get
            {
                Debug.Assert(mLists != null);

                Document doc = mLists.Document as Document;

                if ((doc != null) && (doc.RevisionsView == RevisionsView.Final))
                    return ListLevelFinal;

                return ListLevelOriginal;
            }
        }

        /// <summary>
        /// Returns original list level formatting plus any formatting overrides applied to the current paragraph.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal ListLevel ListLevelOriginal
        {
            get
            {
                if (mCachedListLevel == null)
                {
                    List list = List;
                    ListLevel listLevel = (list != null) ? list.GetListLevelOverrideAware(ListLevelNumberOriginal) : null;

                    // AM. Here we have to create inherited copy of ListLevel.
                    // This copy gets all ListLevel properties from parent ListLevel and 
                    // gets formatting missed in parent ListLevel from parent IRunAttrSource.
                    mCachedListLevel = listLevel != null ? new ListLevel(listLevel, mParentRunAttrs) : null;
                }

                return mCachedListLevel;
            }
        }

        /// <summary>
        /// Returns final list level formatting plus any formatting overrides applied to the current paragraph.
        /// </summary>
        internal ListLevel ListLevelFinal
        {
            get
            {
                if (mCachedListLevelFinal == null)
                {
                    List list = ListFinal;
                    ListLevel listLevel = (list != null) ? list.GetListLevelOverrideAware(ListLevelNumberFinal) : null;

                    // AM. Here we have to create inherited copy of ListLevel.
                    // This copy gets all ListLevel properties from parent ListLevel and 
                    // gets formatting missed in parent ListLevel from parent IRunAttrSource.
                    mCachedListLevelFinal = listLevel != null ? new ListLevel(listLevel, mParentRunAttrs) : null;
                }

                return mCachedListLevelFinal;
            }
        }

        internal int ListId
        {
            get { return (int)mParentParaAttrs.FetchParaAttr(ParaAttr.ListId); }
            set
            {
                object leftIndentAttrValue = mParentParaAttrs.GetDirectParaAttr(ParaAttr.LeftIndent);
                int leftIndent = (leftIndentAttrValue != null) ? (int)leftIndentAttrValue : 0;

                // Word updates direct left indent when list is applied. We do the same, but if the current paragraph
                // already has applied list, at first let's subtract indent that is related to the old list.
                int oldListLeftIndent = 0;
                if ((leftIndent != 0) && (value != 0) && (ListLevel != null))
                {
                    oldListLeftIndent = ListLevel.ParaPr.LeftIndent + ListLevel.ParaPr.FirstLineIndent;
                    // Do not subtract if paragraph indent is not related to the old list indent.
                    if (leftIndent < oldListLeftIndent)
                        oldListLeftIndent = 0;
                }

                mParentParaAttrs.SetParaAttr(ParaAttr.ListId, value);
                Invalidate();

                // If explicitly no list is set or there is no direct left indent or list level is not specified, exit.
                if ((value == 0) || (leftIndentAttrValue == null) || (ListLevel == null))
                    return;

                // Word updates direct left indent when list is applied.
                if (leftIndent == 0)
                {
                    // It seems that Word always remove zero left indent even if it overrides style. 
                    mParentParaAttrs.RemoveParaAttr(ParaAttr.LeftIndent);
                }
                else
                {
                    // Otherwise it updates direct left indent by (LeftIndent + FirstLineIndent) from being applied list.
                    int newLeftIndent = leftIndent - oldListLeftIndent + 
                        ListLevel.ParaPr.LeftIndent + ListLevel.ParaPr.FirstLineIndent;
                    mParentParaAttrs.SetParaAttr(ParaAttr.LeftIndent, newLeftIndent);
                }
            }
        }

        internal int ListIdFinal
        {
            get
            {
                Paragraph paragraph = mParentParaAttrs as Paragraph;
                if (paragraph != null)
                    return (int)paragraph.FetchParaAttr(ParaAttr.ListId, RevisionsView.Final);

                return ListId;
            }
        }

        /// <summary>
        /// Invalidates cached list level.
        /// </summary>
        private void Invalidate()
        {
            mCachedListLevel = null;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IParaAttrSource mParentParaAttrs;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IRunAttrSource mParentRunAttrs;
        private readonly ListCollection mLists;
        private ListLevel mCachedListLevel;
        private ListLevel mCachedListLevelFinal;

        /// <summary>
        /// Maximum allowed list count.
        /// </summary>
        /// <remarks>
        /// For DOC format there are following ranges defined:
        /// 0x0000, 0xf801 - no list
        /// 0x0001 - 0x07FE - normal lists
        /// 0xF802 - 0xFFFF - lists whose left indentation should be ignored.
        /// For DOCX format there is no limitation specified by specs but it seems that Word has 
        /// limited slots for list definitions and stops adding new lists after list count reaches 2046.
        /// </remarks>
        private const int MaxListCount = 0x7FE;
    }
}
