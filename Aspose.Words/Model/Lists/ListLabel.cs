// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2009 by Viktor Sazhaev

using System;
// This is required for AE because it has own Paragraph class in the Aspose.Editor namespace.
using WordsParagraph = Aspose.Words.Paragraph;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Defines properties specific to a list label.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-lists/">Working with Lists</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// <para>Implements <see cref="IRunAttrSource" /> interface treating list level direct formatting
    /// as direct run attributes since they take priority over the other sources.</para>
    /// <para><see cref="ListLabel"/> contains the most common textual representation of the list label
    /// in its <see cref="LabelString"/> property.</para>
    /// <para>There are some classes which need different textual representation of list labels. Use
    /// <see cref="ListLabel"/>'s property <see cref="NumberState"/> which introduced to provide enough information
    /// to build custom labels from the inside of such classes.</para>
    /// <para>Both <see cref="LabelString"/> and <see cref="NumberState"/> are updated
    /// by <see cref="Document.UpdateListLabels"/>.</para>
    /// </dev>
    public class ListLabel : IRunAttrSource
    {
        /// <summary>
        /// Ctor.
        /// You don't normally construct this object. They are constructed by other objects that can have list labels.
        /// </summary>
        internal ListLabel(WordsParagraph para)
        {
            mPara = para;
            mListFormat = para.ListFormat;
        }

        internal void SetLabelStringAndValue(
            string[] labelFragments,
            string[] labelArabicNumbers,
            ListNumberState listNumberState,
            RevisionsView view)
        {
            if (view == RevisionsView.Original)
            {
                mLabelFragmentsOriginal = labelFragments;
                mLabelArabicNumbersOriginal = labelArabicNumbers;
                mNumberStateOriginal = listNumberState;
            }
            else
            {
                mLabelFragmentsFinal = labelFragments;
                mLabelArabicNumbersFinal = labelArabicNumbers;
                mNumberStateFinal = listNumberState;
            }
        }

        /// <summary>
        /// Gets an object containing enough information for building customized list label.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="Document.UpdateListLabels"/> method to update the value of this property.
        /// </remarks>
        internal ListNumberState NumberState
        {
            get { return mNumberStateOriginal; }
        }

        /// <summary>
        /// Gets an object containing enough information for building customized list label.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="Document.UpdateListLabels"/> method to update the value of this property.
        /// </remarks>
        internal ListNumberState NumberStateFinal
        {
            get { return mNumberStateFinal; }
        }

        /// <summary>
        /// Gets the list label font.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(this, mPara.Document);
                return mFont;
            }
        }

        /// <summary>
        /// Checks whether the list label has text representation.
        /// TODO list label depends on list format which in turn depends on revision options.
        /// Most of the other properties of this class consequently depend on attributes of the displayed revision.
        /// However this property explicitly depends on the original revision which, I believe, might be wrong.
        /// At least the name of the property shall clearly say that it is original label being questioned.
        /// I would recommend thoroughly reviewing this class and how layout revision options are used in the document model.
        /// </summary>
        internal bool HasChars
        {
            get { return ArrayUtil.HasData(mLabelFragmentsOriginal); }
        }

        /// <summary>
        /// Checks whether the list label has text representation in final revision.
        /// </summary>
        internal bool HasCharsFinal
        {
            get { return ArrayUtil.HasData(mLabelFragmentsFinal); }
        }

        /// <summary>
        /// Gets a string array that represents list label split to fragments.
        /// </summary>
        /// <remarks>
        /// This property is required while building the document layout since each part of list label
        /// can have different font properties.
        /// Use the <see cref="Document.UpdateListLabels"/> method to update the value of this property.
        /// </remarks>
        internal string[] LabelFragments
        {
            get { return mLabelFragmentsOriginal; }
        }

        internal string[] LabelFragmentsOriginal
        {
            get { return mLabelFragmentsOriginal; }
        }

        internal string[] LabelFragmentsFinal
        {
            get { return mLabelFragmentsFinal; }
        }

        internal string[] LabelArabicNumbers
        {
            get { return mLabelArabicNumbersOriginal; }
        }

        internal string[] LabelArabicNumbersOriginal
        {
            get { return mLabelArabicNumbersOriginal; }
        }

        internal string[] LabelArabicNumbersFinal
        {
            get { return mLabelArabicNumbersFinal; }
        }

        /// <summary>
        /// Gets a string representation of list label.
        /// </summary>
        public string LabelString
        {
            get
            {
                Document doc = mPara.Document as Document;

                if ((doc != null) && (doc.RevisionsView == RevisionsView.Final))
                    return LabelStringFinal;

                return LabelStringOriginal;
            }
        }

        /// <summary>
        /// Gets a numeric value for this label.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="Document.UpdateListLabels"/> method to update the value of this property.
        /// </remarks>
        public int LabelValue
        {
            get { return (mNumberStateOriginal == null) ? 0 : mNumberStateOriginal.GetNumber(); }
        }

        /// <summary>
        /// Gets a numeric value for this label in final revision.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="Document.UpdateListLabels"/> method to update the value of this property.
        /// </remarks>
        internal int LabelValueFinal
        {
            get { return (mNumberStateFinal == null) ? 0 : mNumberStateFinal.GetNumber(); }
        }

        internal string LabelStringOriginal
        {
            get { return ArrayUtil.StringArrayToString(mLabelFragmentsOriginal); }
        }

        internal string LabelStringFinal
        {
            get { return ArrayUtil.StringArrayToString(mLabelFragmentsFinal); }
        }

        /// <summary>
        /// Gets trailing character for this label.
        /// </summary>
        internal ListTrailingCharacter TrailingCharacter
        {
            get
            {
                return (mNumberStateOriginal == null) ? ListTrailingCharacter.Nothing
                    : mNumberStateOriginal.GetListLevel().TrailingCharacter;
            }
        }

        /// <summary>
        /// Gets trailing character for this label in final revision.
        /// </summary>
        internal ListTrailingCharacter TrailingCharacterFinal
        {
            get
            {
                return (mNumberStateFinal == null) ? ListTrailingCharacter.Nothing
                    : mNumberStateFinal.GetListLevel().TrailingCharacter;
            }
        }

        /// <summary>
        /// Gets a <see cref="ListLevel"/> instance used to build the label.
        /// </summary>
        internal ListLevel ListLevel
        {
            get { return mListFormat.ListLevel; }
        }

        /// <summary>
        /// Gets a <see cref="ListLevel"/> instance used to build the label in final revision.
        /// </summary>
        internal ListLevel ListLevelFinal
        {
            get { return mListFormat.ListLevelFinal; }
        }

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return ((IRunAttrSource)this).GetDirectRunAttr(key, RevisionsView.Original);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            ListLevel listLevel = mListFormat.ListLevel;
            return (listLevel != null) ? listLevel.RunPr[key] : null;
        }

        /// <summary>
        /// Performs the bottom-up single attribute resolution.
        /// </summary>
        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            // RK This looks like needs rethought and simplification.

            // WORDSNET-3944 Label font does not match in DOC and PDF.
            // RK I don't fully understand the algorithm to resolve this attribute in MS Word,
            // just trying to do what MS Word seems to do.

            // Here we should also check how style affects formatting if referenced from the level!
            // Maybe its attributes should be considered after direct formatting of the paragraph break font.
            // Moved querying from ListLevel to GetDirectRunAttr.

            // 1. Try to get the attribute from paragraph break direct properties.
            // Seemingly MS Word always takes it from there if it is directly specified.
            // Don't take decoration attributes from paragraph break! Maybe we should filter out
            // in another place but MS Word doesn't take paragraph break in consideration for
            // decoration attributes. (Here is another trick: if underline is defined elsewhere then
            // underline color from paragraph break will affect formatting! Don't remove.)
            // If it's a bulleted list we'll also exclude some other attributes. I found that at least should
            // exclude bold and italic (but not emboss and engrave). See TestDefect861.doc.
            if ((key != FontAttr.Underline) && (key != FontAttr.StrikeThrough) && (key != FontAttr.DoubleStrikeThrough))
            {
                bool excludeParaBreakProps = false;
                if ((key == FontAttr.Bold) || (key == FontAttr.Italic))
                {
                    ListLevel listLevel = mListFormat.ListLevel;
                    excludeParaBreakProps = (listLevel != null) && (listLevel.NumberStyle == NumberStyle.Bullet);

                }

                if (!excludeParaBreakProps)
                {
                    object value = mPara.ParagraphBreakRunPr[key];
                    if (value != null)
                        return ProcessListLabelAttributeBoolExAware(value, key);
                }
            }

            // 2. If the attribute is not obtained above, get it from the paragraph style.
            // Here we have some problem with attribute resolution. WORDSNET-9953.
            // WORDSNET-10833 The problem occurred because paragraph style is list style and does not have Font.
            if (mPara.ParagraphStyle.Font != null)
            {
                // WORDSNET-15348 It seems that the underline attribute is never applied from the paragraph to the list label.
                if (key != FontAttr.Underline)
                    return mPara.ParagraphStyle.Font.FetchAttr(key);
            }

            // 3. If paragraph style does not have Font definition, try get the attribute from base style.
            Style baseStyle = mPara.Document.Styles[mPara.ParagraphStyle.BaseStyleName];
            if (baseStyle != null && baseStyle.Font != null)
                return baseStyle.Font.FetchAttr(key);

            // 4. Finally, get the attribute from Normal style.
            return mPara.Document.Styles[StyleIdentifier.Normal].Font.FetchAttr(key);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            ListLevel listLevel = mListFormat.ListLevel;
            if (listLevel != null)
                listLevel.RunPr.SetAttr(key, value);
            else
                ThrowCannotChangeException();
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            ListLevel listLevel = mListFormat.ListLevel;
            if (listLevel != null)
                listLevel.RunPr.Remove(key);
            else
                ThrowCannotChangeException();
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            ListLevel listLevel = mListFormat.ListLevel;
            if (listLevel != null)
                listLevel.RunPr.Clear();
            else
                ThrowCannotChangeException();
        }

        #endregion

        /// <summary>
        /// Gets the complete collection of attributes how the list label is formatted.
        /// Performs the top-bottom complete attribute expansion.
        /// </summary>
        internal RunPr GetExpandedRunPr(bool expandListLevelFormatting, RunPrExpandFlags flags)
        {
            // Basically we need to do exactly opposite of GetDirectAttr + FetchInheritedAttr,
            // but FetchInheritedAttr is quite messy so we just do the simplest that seems enough for now.

            // This expands defaults, paragraph style and paragraph break run properties all what FetchInheritedAttr does.
            // andrnosk: WORDSNET-10119 We have to use revised collection here.
            RunPr result = mPara.GetExpandedParagraphBreakRunPr(flags);

            // Note: I believe that there is any "strict" logic in inheritance (non-inheritance) of paragraph formatting
            // attributes to list label formatting attributes. Can't find it.

            // I don't do all the poorly understood complex attribute exclusion rules here, but I clearly
            // can see that underline should not be propagated from the paragraph formatting to the list label.
            result.Remove(FontAttr.Underline);

            // DM Neither should Hidden.
            result.Remove(FontAttr.Hidden);

            // WORDSNET-6782 List labels ignore shading.
            result.Remove(FontAttr.Shading);

            // expandListLevelFormatting is false when GetExpandedRunPr() is called for list label separators.
            if (!expandListLevelFormatting)
            {
                // WORDSNET-8782 these attributes should be ignored for list label separators.
                result.Remove(FontAttr.StrikeThrough);
                result.Remove(FontAttr.DoubleStrikeThrough);
            }

            bool isRevised = (flags & RunPrExpandFlags.Revised) != 0;

            ListLevel listLevel = mPara.GetListLevel(isRevised);

            if (listLevel != null)
            {
                // IV For bulleted list Italic and Bold also should not be propagated from the paragraph formatting to the list label.
                if (listLevel.NumberStyle == NumberStyle.Bullet)
                {
                    result.Remove(FontAttr.Italic);
                    result.Remove(FontAttr.Bold);
                }

                if (listLevel.RunPr.Contains(FontAttr.Istd))
                {
                    // WORDSNET-14864 Word takes font size from Normal style if Istd present in level properties.
                    Style styleNormal = mPara.Document.Styles.GetBySti(StyleIdentifier.Normal, false);

                    if (styleNormal != null)
                        styleNormal.RunPr.MirrorTo(result, FontAttr.Size);
                }

                // Apply direct formatting from the list label.
                if (expandListLevelFormatting)
                    listLevel.RunPr.ExpandTo(result);
            }

            return result;
        }

        /// <summary>
        /// Throws an exception if cannot change attributes of direct formatting.
        /// If you query Paragraph.ListLabel.Font and then remove the paragraph from the list this will occur.
        /// We can consider storing stateless complex attrs in a separate collection. But this is meaningless anyway.
        /// </summary>
        private static void ThrowCannotChangeException()
        {
            throw new InvalidOperationException("Cannot change direct attributes on a list label. Paragraph doesn't belong to a list.");
        }

        /// <summary>
        /// Properly resolves BoolEx values if any.
        /// This is repetition from Font class. We can avoid this if perform redesign.
        /// </summary>
        private object ProcessListLabelAttributeBoolExAware(object value, int key)
        {
            // The value is specified directly, but might need to be resolved using parent values.
            if (value is AttrBoolEx)
                return ((AttrBoolEx)value).ResolveFetchAttr(mPara.ParagraphStyle.Font, key);

            return value;
        }

        /// <summary>
        /// Paragraph that the list label belongs to.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly WordsParagraph mPara;

        /// <summary>
        /// List format of the paragraph that the list label belongs to.
        /// Holding a reference since this class is stateless and operates only on paragraph state.
        /// That's not good (ineffective) to query ListLevel everytime it's needed. But list and list level
        /// might change and ListLevel might get invalid. We can create another method that expands all
        /// list label attributes if this is considered more effective and needed for optimization.
        /// </summary>
        private readonly ListFormat mListFormat;

        /// <summary>
        /// Lazily initialized font associated with this list label.
        /// </summary>
        private Font mFont;

        /// <summary>
        /// Cached label split to fragments that is updated by a <see cref="ListLabelUpdater.UpdateListLabels"/> function.
        /// </summary>
        private string[] mLabelFragmentsOriginal;

        private string[] mLabelFragmentsFinal;

        private string[] mLabelArabicNumbersOriginal;

        private string[] mLabelArabicNumbersFinal;

        /// <summary>
        /// Cached list number stated of the paragraph. Only used in some scenarios like when REF-like fields need it
        /// to build up list label string in full or relative context.
        /// </summary>
        private ListNumberState mNumberStateOriginal;

        private ListNumberState mNumberStateFinal;
    }
}
