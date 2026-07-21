// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2005 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Collections.Generic;
using Aspose.Words.Lists;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a single built-in or user-defined style.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-styles-and-themes/">Working with Styles and Themes</a> documentation article.</para>
    /// </summary>
    public class Style : IParaAttrSource, IRunAttrSource
    {
        /// <summary>
        /// Factory method.
        /// </summary>
        internal static Style Create(StyleType styleType)
        {
            switch (styleType)
            {
                case StyleType.Character:
                case StyleType.Paragraph:
                case StyleType.List:
                    return new Style(styleType);
                case StyleType.Table:
                    return new TableStyle();
                default:
                    throw new InvalidOperationException("Unknown style type.");
            }
        }

        /// <summary>
        /// Factory method.
        /// </summary>
        internal static Style Create(StyleType styleType, int istd, StyleIdentifier sti, string name)
        {
            Style style = Create(styleType);
            style.mIstd = istd;
            style.mSti = sti;
            style.mName = name;
            style.mNextIstd = istd;
            return style;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        protected Style(StyleType styleType)
        {
            mType = styleType;
        }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <remarks>
        /// <p>Can not be empty string.</p>
        /// <p>If there already is a style with such name in the collection, then this style will override it. All affected nodes will reference new style.</p>
        /// </remarks>
        public string Name
        {
            get { return mName; }
            set
            {
                if (!StringUtil.HasChars(value))
                    throw new ArgumentException("Style name can not be empty.");

                if (value == mName) // if renaming to same name, skip unnecessary operations.
                    return;

                Debug.Assert(Styles != null);

                // Check if old or new name value belongs to built-in styles.
                // Some istd are reserved for built-in styles, if the new name is reserved for built-in style,
                // or existing style was built-in, we have to tweak related
                // style properties including sti and istd.

                Style oldStyle = mStyles.GetByName(value, false);
                StyleIdentifier sti = StyleIndex.BuiltInStyleNameEnglish(value);
                if (sti != StyleIdentifier.User) // 1) new name = built-in. we have to make a built-in style out of original.
                {
                    StyleIndex.ConvertToBuiltInStyle(this, sti, oldStyle, true);
                }
                else if (BuiltIn) // 2) old name = built-in style, new name = regular.
                {
                    StyleIndex.ConvertToRegularStyle(this, oldStyle);
                }
                else
                {
                    if (oldStyle != null) // if style with such name exists, we override old style with new one.
                    {
                        // Todo DD: Would be nice to refactor/rework this along with the code in StyleCollection.AddCopyCore
                        if ((Type != StyleType.Character) && (mParaPr.ListId > 0))
                        {
                            List list = Document.Lists.FetchListByListId(mParaPr.ListId);
                            foreach (ListLevel level in list.ListLevels)
                                if (level.ParaStyleIstd == Istd)
                                    level.ParaStyleIstd = oldStyle.Istd;
                        }

                        SetIstd(oldStyle.Istd, true); // use istd of old style for new style, update collection maps.
                    }
                }

                SetNameCore(value, true);
            }
        }

        /// <summary>
        /// Get the name of the style, including aliases (like "Heading 1,Heading 1#").
        /// </summary>
        internal string GetNameWithAliases()
        {
            string aliases = this.Styles.GetAliases(this, false);
            if (!StringUtil.HasChars(aliases))
                return this.Name;
            return string.Format("{0},{1}", this.Name, aliases);
        }

        /// <summary>
        /// Gets style aliases.
        /// </summary>
        internal string[] GetAliasesInternal()
        {
            return this.Styles.GetAliases(this, false).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Sets style name and updates mapping in StyleCollection.
        /// Used in renaming through <see cref="Name"/> setter and <see cref="StyleCollection.AddCopy(Style)"/> methods.
        /// </summary>
        internal void SetNameCore(string value, bool updateStyleCollection)
        {
            if (updateStyleCollection)
                mStyles.UpdateNameMap(this, mName, value);

            mName = value;
        }

        /// <summary>
        /// Left for document construction purposes, use with care.
        /// </summary>
        internal void SetNameCore(string value)
        {
            SetNameCore(value, false);
        }

        /// <summary>
        /// Adds an alias to this style. For internal use.
        /// </summary>
        internal void AddAlias(string alias)
        {
            mStyles.AddAlias(this, alias);
        }

        /// <summary>
        /// Gets the locale independent style identifier for a built-in style.
        /// </summary>
        /// <remarks>
        /// <para>For user defined (custom) styles, this property returns <see cref="Words.StyleIdentifier.User"/>.</para>
        /// <seealso cref="Name"/>
        /// </remarks>
        public StyleIdentifier StyleIdentifier
        {
            get { return mSti; }
        }

        /// <summary>
        /// Gets the ID of the style from which this style is cloned.
        /// It is used in WORDSNET-21097, to revert Hyperlink style to the default ID inside TOC.
        /// </summary>
        internal StyleIdentifier ClonedFromStyleIdentifier
        {
            get { return mClonedFromSti; }
        }

        /// <summary>
        /// Gets all aliases of this style. If style has no aliases then empty array of string is returned.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public string[] Aliases
        {
            get
            {
                return GetAliasesInternal();
            }
        }

        /// <summary>
        /// True when the style is one of the built-in Heading styles.
        /// </summary>
        public bool IsHeading
        {
            get
            {
                return ((StyleIdentifier >= StyleIdentifier.Heading1) &&
                    (StyleIdentifier <= StyleIdentifier.Heading9));
            }
        }

        /// <summary>
        /// Gets the style type (paragraph or character).
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public StyleType Type
        {
            get { return mType; }
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        public DocumentBase Document
        {
            get { return (mStyles != null) ? mStyles.Document : null; }
        }

        /// <summary>
        /// Gets/sets the name of the <see cref="Style"/> linked to this one. Returns empty string if no styles are linked.
        /// </summary>
        /// <remarks>
        /// <para>It is only allowed to link the paragraph style to the character style and vice versa.</para>
        /// <para>Setting LinkedStyleName for the current style automatically leads to setting LinkedStyleName for the linked style.</para>
        /// <para>Assigning the empty string is equivalent to unlinking the previously linked style.</para>
        /// </remarks>
        public string LinkedStyleName
        {
            get
            {
                string result = "";
                if (LinkedIstd != StyleIndex.Nil)
                {
                    Style style = Document.Styles.GetByIstd(LinkedIstd, false);
                    Debug.Assert(style != null);
                    if (style != null)
                        result = style.Name;
                }

                return result;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value == string.Empty)
                {
                    UpdateLinkedStyleLinks(null);
                    LinkedIstd = StyleIndex.Nil;
                }
                else
                {
                    Style proposedLinkedStyle = mStyles.FetchByName(value);

                    if (!((mType == StyleType.Paragraph) && (proposedLinkedStyle.Type == StyleType.Character) ||
                        (mType == StyleType.Character) && (proposedLinkedStyle.Type == StyleType.Paragraph)))
                        throw new InvalidOperationException("Only the paragraph and the character styles can be linked.");

                    if ((proposedLinkedStyle.StyleIdentifier == StyleIdentifier.DefaultParagraphFont) ||
                        (StyleIdentifier == StyleIdentifier.DefaultParagraphFont))
                        throw new InvalidOperationException("Default paragraph font cannot be linked style.");

                    UpdateLinkedStyleLinks(proposedLinkedStyle);
                    LinkedIstd = proposedLinkedStyle.Istd;
                }
            }
        }

        /// <summary>
        /// Gets/sets the name of the style this style is based on.
        /// </summary>
        /// <remarks>
        /// This will be an empty string if the style is not based on any other style and it can be set
        /// to an empty string.
        /// </remarks>
        public string BaseStyleName
        {
            get
            {
                Style baseStyle = GetBaseStyle();
                return (baseStyle != null) ? baseStyle.Name : "";
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // andrnosk: WORDSNET-7403 Add StyleType.Table to this condition, because BaseStyleName is allowed for table styles.
                if ((mType != StyleType.Character) && (mType != StyleType.Paragraph) && (mType != StyleType.Table))
                    throw new InvalidOperationException("This property is only allowed for character, paragraph and table styles.");

                if ((mSti == StyleIdentifier.Normal) || (mSti == StyleIdentifier.DefaultParagraphFont))
                    throw new InvalidOperationException("The built-in styles Normal and Default Paragraph Font cannot be based on any style.");

                if (value == "")
                {
                    mBasedOnIstd = StyleIndex.Nil;
                }
                else
                {
                    Style proposedBaseStyle = mStyles.FetchByName(value);
                    VerifySuitableAsBaseStyle(proposedBaseStyle, true);
                    mBasedOnIstd = proposedBaseStyle.Istd;
                }
            }
        }

        /// <summary>
        /// Gets/sets the name of the style to be applied automatically to a new paragraph inserted after a
        /// paragraph formatted with the specified style.
        /// </summary>
        /// <remarks>
        /// This property is not used by Aspose.Words. The next paragraph style will only
        /// be applied automatically when you edit the document in MS Word.
        /// </remarks>
        public string NextParagraphStyleName
        {
            get
            {
                Style nextStyle = GetNextStyle();
                return (nextStyle != null) ? nextStyle.Name : "";
            }
            set
            {
                if (mType != StyleType.Paragraph)
                    throw new InvalidOperationException("This property is allowed only for paragraph styles.");

                ArgumentUtil.CheckHasChars(value, "value");

                Style style = mStyles.FetchByName(value);
                mNextIstd = style.Istd;
            }
        }

        /// <summary>
        /// True if this style is one of the built-in styles in MS Word.
        /// </summary>
        public bool BuiltIn
        {
            get { return (mSti != StyleIdentifier.User); }
        }

        /// <summary>
        /// Gets the character formatting of the style.
        /// </summary>
        /// <remarks>
        /// <para>For list styles this property returns <c>null</c>.</para>
        /// </remarks>
        public Font Font
        {
            get
            {
                if (mType == StyleType.List)
                    return null;

                if (mFontCache == null)
                    mFontCache = new Font(this, mStyles.Document);
                return mFontCache;
            }
        }

        /// <summary>
        /// Gets the paragraph formatting of the style.
        /// </summary>
        /// <remarks>
        /// <para>For character and list styles this property returns <c>null</c>.</para>
        /// </remarks>
        public ParagraphFormat ParagraphFormat
        {
            get
            {
                if ((mType == StyleType.Character) || (mType == StyleType.List))
                    return null;

                if (mParagraphFormatCache == null)
                    mParagraphFormatCache = new ParagraphFormat(this, Styles);
                return mParagraphFormatCache;
            }
        }

        /// <summary>
        /// Gets/sets whether the style hides from the Styles gallery and from the Styles task pane.
        /// </summary>
        public bool SemiHidden
        {
            get { return mSemiHidden; }
            set { mSemiHidden = value; }
        }

        /// <summary>
        /// Gets/sets whether the style used in the current document unhides from the Styles gallery and from the Styles task pane.
        /// True when the used style should be shown in the Styles gallery.
        /// </summary>
        public bool UnhideWhenUsed
        {
            get { return mUnhideWhenUsed; }
            set { mUnhideWhenUsed = value; }
        }

        /// <summary>
        /// Gets/sets the integer value that represents the priority for sorting the styles in the Styles task pane.
        /// </summary>
        public int Priority
        {
            get { return mUIPriority; }
            set { mUIPriority = value; }
        }

        /// <summary>
        /// Gets the frame related formatting of the style.
        /// </summary>
        /// <remarks>
        /// <para>For character and list styles this property returns <c>null</c>.</para>
        /// </remarks>
        /// TODO AM, DD: Exposing nulls in public API is bad, especially when we say in Paragraph class that FrameFormat is always created.
        ///  This is a big inconsistency. Lets keep this internal until we fix this somehow.
        internal FrameFormat FrameFormat
        {
            get
            {
                if ((mType == StyleType.Character) || (mType == StyleType.List))
                    return null;

                if (mFrameFormatCache == null)
                    mFrameFormatCache = new FrameFormat(this);
                return mFrameFormatCache;
            }
        }

        /// <summary>
        /// Gets the list that defines formatting of this list style.
        /// </summary>
        /// <remarks>
        /// <para>This property is only valid for list styles.
        /// For other style types this property returns <c>null</c>.</para>
        /// </remarks>
        public List List
        {
            get
            {
                if (mType != StyleType.List)
                    return null;

                return (Document != null) ? Document.Lists.FetchListByListId(mParaPr.ListId) : null;
            }
        }

        /// <summary>
        /// Provides access to the list formatting properties of a paragraph style.
        /// </summary>
        /// <remarks>
        /// <para>This property is only valid for paragraph styles.
        /// For other style types this property returns <c>null</c>.</para>
        /// </remarks>
        public ListFormat ListFormat
        {
            get
            {
                if (mType != StyleType.Paragraph)
                    return null;

                if (Document == null)
                    return null;

                if (mListFormatCache == null)
                    mListFormatCache = new ListFormat(this, this, Document.Lists);

                return mListFormatCache;
            }
        }

        /// <summary>
        /// Specifies whether this style is shown in the Quick Style gallery inside MS Word UI.
        /// </summary>
        public bool IsQuickStyle
        {
            get { return mIsQuickStyle; }
            set { mIsQuickStyle = value; }
        }

        /// <summary>
        /// Specifies whether this style is automatically redefined based on the appropriate value.
        /// </summary>
        /// <remarks>
        /// <para>If the property value is set to true, MS Word automatically redefines the current style when
        /// the appropriate paragraph formatting has been changed.</para>
        /// <para>AutomaticallyUpdate property is applicable to paragraph styles only.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </remarks>
        public bool AutomaticallyUpdate
        {
            get { return mAutomaticallyUpdate; }
            set { mAutomaticallyUpdate = value; }
        }

        /// <summary>
        /// Specifies whether this style is locked.
        /// </summary>
        public bool Locked
        {
            get { return mLocked; }
            set { mLocked = value; }
        }

        /// <summary>
        /// This is used to identify the style in the model.
        /// Every style has a unique istd and objects refer to styles using istd.
        ///
        /// Istd historically came from the DOC format where it means "index of style definition".
        /// </summary>
        internal int Istd
        {
            get { return mIstd; }
        }

        /// <summary>
        /// Style index of the based on style. Can be StyleIndex.Nil that means the style is not based on another style.
        /// Setting this property corrects any circular references by setting the offending reference to nil style.
        /// </summary>
        internal int BasedOnIstd
        {
            get
            {
                // WORDSNET-23283 DefaultParagraphFont style cannot be based on another style.
                return (Istd == StyleIndex.DefaultParagraphFont)
                    ? StyleIndex.Nil
                    : mBasedOnIstd;
            }
            set
            {
                mBasedOnIstd = value;
                FixUpShallowCircularReferences();
            }
        }

        /// <summary>
        /// Style index of the next paragraph style. By default same as the istd of this style.
        /// Not expected to be StyleIndex.Nil.
        /// Setting this property performs no validation.
        /// </summary>
        internal int NextIstd
        {
            get { return mNextIstd; }
            set { mNextIstd = value; }
        }

        /// <summary>
        /// Style index of the linked style. A paragraph style can link a character style and vice versa.
        /// </summary>
        internal int LinkedIstd
        {
            get { return mLinkedIstd; }
            set
            {
                mLinkedIstd = value;
                FixUpShallowCircularReferences();
            }
        }

        /// <summary>
        /// Rsid of the style.
        /// </summary>
        internal int Rsid
        {
            get { return mRsid; }
            set { mRsid = value; }
        }

        /// <summary>
        /// Hidden from UI.
        /// RK I cannot find how to set this in MS Word.
        /// </summary>
        internal bool Hidden
        {
            get { return mHidden; }
            set { mHidden = value; }
        }

        /// <summary>
        /// HTML Threading compose style.
        /// </summary>
        internal bool PersonalCompose
        {
            get { return mPersonalCompose; }
            set { mPersonalCompose = value; }
        }

        /// <summary>
        /// HTML Threading reply style.
        /// </summary>
        internal bool PersonalReply
        {
            get { return mPersonalReply; }
            set { mPersonalReply = value; }
        }

        /// <summary>
        /// HTML Threading - another user's personal style.
        /// </summary>
        internal bool Personal
        {
            get { return mPersonal; }
            set { mPersonal = value; }
        }

        /// <summary>
        /// RK DOC use only. Not needed in other formats. Not critical in DOC too, but needed
        /// to make low-level binary writer tests pass (preserves a low-level binary flag).
        /// </summary>
        internal bool LidsSet
        {
            get { return mLidsSet; }
            set { mLidsSet = value; }
        }

        /// <summary>
        /// Indicates the style is top level paragraph style.
        /// Used in DocumentValidator when expanding document defaults.
        /// </summary>
        internal bool IsTopLevelParaStyle
        {
            get { return (Type == StyleType.Paragraph) && (BasedOnIstd == StyleIndex.Nil); }
        }

        /// <summary>
        /// Gets the collection of styles this style belongs to.
        /// </summary>
        public StyleCollection Styles
        {
            get { return mStyles; }
        }

        /// <summary>
        /// Removes the specified style from the document.
        /// </summary>
        /// <remarks>
        /// Style removal has following effects on the document model:
        /// <list type="bullet">
        /// <item>All references to the style are removed from corresponding paragraphs, runs and tables.</item>
        /// <item>If base style is removed its formatting is moved to child styles.</item>
        /// <item>If style to be deleted has a linked style, then both of these are deleted.</item>
        /// </list>
        /// </remarks>
        public void Remove()
        {
            Styles.Remove(Name);
        }

        internal ParaPr ParaPr
        {
            get { return mParaPr; }
            set { mParaPr = value; }
        }

        internal RunPr RunPr
        {
            get { return mRunPr; }
            set { mRunPr = value; }
        }

        internal bool HasRevisions
        {
            get { return RunPr.HasFormatRevision || ParaPr.HasFormatRevision; }
        }

        /// <summary>
        /// Returns true if paragraph and run properties have not attribute in the formatting revision.
        /// </summary>
        internal bool HasEmptyFormatRevision
        {
            get { return RunPr.HasEmptyFormatRevision || ParaPr.HasEmptyFormatRevision; }
        }

        /// <summary>
        /// Makes a deep copy of the style.
        /// Note:
        /// 1. The returned object's parent Styles collection is null.
        /// 2. The returned object is valid in the context of the original document only.
        /// To use a style in a different document, you must call ImportStyle.
        ///
        /// Note that list definitions that are referenced by the style are not cloned.
        /// </summary>
        internal virtual Style Clone()
        {
            Style lhs = (Style)MemberwiseClone();

            // RK See comments for RawExtraBytes2 why we clone it by value.

            ParaPr paraPr = mParaPr.Clone();
            lhs.mParaPr = paraPr;

            RunPr runPr = mRunPr.Clone();
            lhs.mRunPr = runPr;

            lhs.mStyles = null;

            lhs.ClearCaches();

            // WORDSNET-21097 Keep track of the source style ID. It is used to revert Hyperlink style to the default ID inside TOC.
            lhs.mClonedFromSti = this.mSti;

            return lhs;
        }

        /// <summary>
        /// Clears caches.
        /// </summary>
        internal void ClearCaches()
        {
            mFontCache = null;
            mParagraphFormatCache = null;
            mFrameFormatCache = null;
            mListFormatCache = null;
        }

        /// <summary>
        /// Compares with the specified style.
        /// Styles Istds are compared for built-in styles only.
        /// Styles defaults are not included in comparison.
        /// Base style, linked style and next paragraph style are recursively compared.
        /// </summary>
        public bool Equals(Style style)
        {
            return Equals(style, new HashSetGeneric<Pair>());
        }

        /// <summary>
        /// Compares with the specified style considering base, linked and next paragraph styles.
        /// Uses HashSet to check already compared styles to avoid dead loops in linked and next styles.
        /// </summary>
        internal bool Equals(Style style, HashSetGeneric<Pair> alreadyCompared)
        {
            if (style == null)
                return false;

            // Same instance.
            if (ReferenceEquals(this, style))
                return true;

            if (alreadyCompared.Contains(new Pair(this, style)))
                return true;

            if (!EqualsCore(style, alreadyCompared))
                return false;

            // Verifying base styles.
            if (!AreEqual(GetBaseStyle(), style.GetBaseStyle(), alreadyCompared))
                return false;

            // Verifying linked styles.
            if (!AreEqual(GetLinkedStyle(), style.GetLinkedStyle(), alreadyCompared))
                return false;

            // Verifying next styles.
            if (!AreEqual(GetNextStyle(), style.GetNextStyle(), alreadyCompared))
                return false;

            return true;
        }

        /// <summary>
        /// Compares two styles considering base, linked and next paragraph styles.
        /// Uses HashSet to check already compared styles to avoid dead loops in linked and next styles.
        /// </summary>
        internal static bool AreEqual(Style styleA, Style styleB, HashSetGeneric<Pair> alreadyCompared)
        {
            if (ArgumentUtil.OneIsNull(styleA, styleB))
                return false;

            if (ArgumentUtil.BothAreNull(styleA, styleB))
                return true;

            return styleA.Equals(styleB, alreadyCompared);
        }

        /// <summary>
        /// Sets the owner collection of styles.
        /// </summary>
        /// <param name="styles"></param>
        internal void SetStyles(StyleCollection styles)
        {
            mStyles = styles;
        }

        /// <summary>
        /// Sets the style identifier. Style identifier does not change under normal circumstances.
        /// You should set identifier only during load or import. Make sure you know what you are doing.
        /// The type of the style should be already set before calling this method.
        /// </summary>
        internal void SetIstd(int istd)
        {
            SetIstd(istd, false);
        }

        /// <summary>
        /// Set Istd and update <see cref="StyleCollection"/> mappings if required.
        /// </summary>
        /// <param name="istd"></param>
        /// <param name="updateStyleCollection">Set to true for styles that are attached to collection, this is needed to update mapping in the collection itself.
        /// </param>
        internal void SetIstd(int istd, bool updateStyleCollection)
        {
            if (updateStyleCollection)
                mStyles.UpdateIstdMap(this, Istd, istd);

            mIstd = istd;
            FixUpShallowCircularReferences();

            // Unfortunately we have a bit of duplication. The style identifier is also stored in the
            // paragraph or character properties appropriately. It could be a good idea to get rid
            // of the separate istd field, but I'm not sure how it will work with table styles.
            switch (mType)
            {
                case StyleType.Paragraph:
                case StyleType.List:
                    mParaPr.Istd = istd;
                    break;
                case StyleType.Character:
                    mRunPr.Istd = istd;
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        internal void SetStyleIdentifier(StyleIdentifier sti)
        {
            SetStyleIdentifier(sti, false);
        }

        /// <param name="sti"></param>
        /// <param name="updateStyleCollection">
        /// Set to true for styles that are attached to collection, this is needed to update mapping in the collection itself.
        /// </param>
        internal void SetStyleIdentifier(StyleIdentifier sti, bool updateStyleCollection)
        {
            if (updateStyleCollection)
                mStyles.UpdateStiMap(this, StyleIdentifier, sti);

            mSti = sti;
        }

        /// <summary>
        /// Gets the based on style. Returns <c>null</c> if it is set to StyleIndex.Nil.
        /// </summary>
        internal Style GetBaseStyle()
        {
            FixUpShallowCircularReferences();

            // WORDSNET-23283 DefaultParagraphFont style cannot be based on another style.
            if (Istd == StyleIndex.DefaultParagraphFont)
                return null;

            if (mBasedOnIstd == StyleIndex.Nil)
                return null;

            // WORDSNET-27108 Check that BaseStyle exists in the style collection.
            if (mBasedOnIstd > StyleCollection.MaxFixedIstd)
                if (mStyles.GetByIstd(mBasedOnIstd, false) == null)
                    return null;

            return mStyles.FetchByIstd(mBasedOnIstd, StyleIndex.Nil);
        }

        /// <summary>
        /// Gets the next paragraph style. Returns <c>null</c> if it is set to StyleIndex.Nil.
        /// </summary>
        internal Style GetNextStyle()
        {
            // RESILIENCY WORDSNET-5736 The problem occurred because mNextIstd refers to non-existing style
            // In this case we tried to get the default style, but since we used StyleIndex.Nil as default we got nothing.
            // Fixed by using the current style istd as default style.
            return (mNextIstd != StyleIndex.Nil) ? mStyles.FetchByIstd(mNextIstd, mIstd) : null;
        }

        /// <summary>
        /// Gets the linked paragraph style. Returns <c>null</c> if it is set to StyleIndex.Nil.
        /// </summary>
        internal Style GetLinkedStyle()
        {
            FixUpShallowCircularReferences();
            return (mLinkedIstd != StyleIndex.Nil) ? mStyles.FetchByIstd(mLinkedIstd, StyleIndex.Nil) : null;
        }

        /// <summary>
        /// Returns true if there is at least one formatting attribute specified.
        /// </summary>
        internal virtual bool HasFormatting()
        {
            if (mParaPr.Count > 0)
                return true;
            if (mRunPr.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Resiliency method, enforces the rule that the style cannot be based on itself and cannot be linked to itself.
        /// Call this whenever you set istd, based on or linked istd using internal methods.
        /// Breaks the loop by setting the offending reference to nil.
        /// </summary>
        private void FixUpShallowCircularReferences()
        {
            if (mBasedOnIstd == mIstd)
                mBasedOnIstd = StyleIndex.Nil;

            if (mLinkedIstd == mIstd)
                mLinkedIstd = StyleIndex.Nil;
        }

        /// <summary>
        /// WORDSNET-16077 Verifies that the based on style exists, otherwise set it to nil.
        /// Call this from <see cref="StyleCollection.FixUpBasedOnStyles"/>.
        /// Do not call this directly when iterating over the styles collection because this method might auto create more styles and invalidate iteration.
        /// </summary>
        internal void FixUpBasedOnMissing()
        {
            bool allowAutoCreate = mBasedOnIstd < StyleCollection.MaxFixedIstd;

            if ((mBasedOnIstd != StyleIndex.Nil) && (mStyles.GetByIstd(mBasedOnIstd, allowAutoCreate) == null))
                mBasedOnIstd = StyleIndex.Nil;
        }

        /// <summary>
        /// Call this from <see cref="StyleCollection.FixUpBasedOnStyles"/>.
        /// Do not call this directly when iterating over the styles collection because this method might auto create more styles and invalidate iteration.
        /// </summary>
        internal void FixUpBasedOnCircularReferences()
        {
            if (!VerifySuitableAsBaseStyle(GetBaseStyle(), false))
                mBasedOnIstd = StyleIndex.Nil;
        }

        /// <summary>
        /// Walks all base styles starting from the proposed base style to verify there will be no circular references to this style.
        /// </summary>
        private bool VerifySuitableAsBaseStyle(Style proposedBaseStyle, bool isThrow)
        {
            while (proposedBaseStyle != null)
            {
                // Verify that we are basing on the style of the same type.
                if (proposedBaseStyle.mType != mType)
                {
                    if (isThrow)
                        throw new InvalidOperationException("Cannot base a style on a style of different type.");

                    return false;
                }

                // Verify that setting the new base style will not create a circular dependency.
                if (proposedBaseStyle.mIstd == mIstd)
                {
                    if (isThrow)
                        throw new InvalidOperationException("This operation will create a circular reference between styles.");

                    return false;
                }

                proposedBaseStyle = proposedBaseStyle.GetBaseStyle();
            }
            return true;
        }

        /// <summary>
        /// Updates the symmetrical links for linked styles.
        /// </summary>
        private void UpdateLinkedStyleLinks(Style linkedStyle)
        {
            if (LinkedIstd != StyleIndex.Nil)
            {
                Style currentLinkedStyle = mStyles.FetchByIstd(LinkedIstd, mIstd);
                currentLinkedStyle.LinkedIstd = StyleIndex.Nil;
            }

            if (linkedStyle == null)
                return;

            if (linkedStyle.LinkedIstd != StyleIndex.Nil)
            {
                Style currentLinkedStyle = mStyles.FetchByIstd(linkedStyle.LinkedIstd, mIstd);
                currentLinkedStyle.LinkedIstd = StyleIndex.Nil;
            }

            linkedStyle.LinkedIstd = mIstd;
        }

        /// <summary>
        /// Recursively expands paragraph properties of this style and all based on styles into direct formatting.
        /// </summary>
        internal void ExpandParaPr(ParaPr dstParaPr, ParaPrExpandFlags flags)
        {
            Style baseStyle = GetBaseStyle();

            if ((baseStyle != null) && IsInheritRunAndParaFormatting)
            {
                baseStyle.ExpandParaPr(dstParaPr, flags & ~ParaPrExpandFlags.NoDirectFormatting);
            }
            else
            {
                // If we are at top of style hierarchy then expand defaults if needed.
                ExpandParaPrDefaults(dstParaPr, flags);
            }

            ParaPr sourceParaPr = mParaPr.GetSourceParaPr(flags);

            if ((flags & ParaPrExpandFlags.NoDirectFormatting) == 0)
            {
                // WORDSNET-18079 It looks like list id and list level need to be expanded "out of band" because in some
                // cases list id can be specified in the based on style and list level in the derived style.
                // If we don't do that, then we will not be able to find the correct list formatting to apply.
                //
                // 1. Get the list id and/or list level attributes that are specified by this style
                object thisStyleListId = sourceParaPr.GetDirectAttr(ParaAttr.ListId);
                object thisStyleListLevel = sourceParaPr.GetDirectAttr(ParaAttr.ListLevel);

                // 2. and expand them into the destination collection.
                dstParaPr.SetAttrIfNotNull(ParaAttr.ListId, thisStyleListId);
                dstParaPr.SetAttrIfNotNull(ParaAttr.ListLevel, thisStyleListLevel);

                // WORDSNET-28427 In this test we have a derived style that does not specify list formatting
                // and had to add this check to make sure we don't expand list formatting again in the derived style.
                if ((thisStyleListId != null) || (thisStyleListLevel != null))
                {
                    // 3. Now we need to expand the list formatting and we need to use the already expanded list id and list level values.
                    Document.Lists.ExpandDirectList(dstParaPr, dstParaPr);
                }

                // Last expand formatting specified in this style.
                sourceParaPr.ExpandTo(dstParaPr);
            }

            sourceParaPr.FrameInheritanceHack(dstParaPr);

            if ((flags & ParaPrExpandFlags.RemoveClearTabStops) != 0)
                dstParaPr.RemoveClearTabStops();
        }

        /// <summary>
        /// Recursively expands paragraph properties of this style and all based on styles into direct formatting.
        /// </summary>
        internal ParaPr GetExpandedParaPr(ParaPrExpandFlags flags)
        {
            ParaPr dstParaPr = new ParaPr();
            ExpandParaPr(dstParaPr, flags);
            return dstParaPr;
        }

        /// <summary>
        /// Recursively expands font properties of this style and all based on styles into direct formatting.
        /// </summary>
        internal void ExpandRunPr(RunPr dstRunPr, RunPrExpandFlags flags)
        {
            Style baseStyle = GetBaseStyle();

            if ((baseStyle != null) && IsInheritRunAndParaFormatting)
            {
                baseStyle.ExpandRunPr(dstRunPr, flags & ~RunPrExpandFlags.NoDirectFormatting);
            }
            else
            {
                // If we are at top of style hierarchy then expand defaults if needed.
                ExpandRunPrDefaults(dstRunPr, flags);
            }

            // WORDSNET-10695 Word ignores any formatting in "Default Paragraph Font" style.
            if (StyleIdentifier == StyleIdentifier.DefaultParagraphFont)
            {
                // AM. Since this style has no base style we can return immediately.
                // To avoid many affected golds lets copy Istd attribute which can be present.
                mRunPr.MirrorTo(dstRunPr, FontAttr.Istd);
            }
            else
            {
                RunPr sourceRunPr = mRunPr.GetSourceRunPr(flags);

                // AM. This code might look strange but the reason is that we should not inherit
                // category hint from styles (see WORDSNET-16263) except the case when global defaults are
                // expanded which is needed for document comparison feature.
                // I don't want to make any side effects by deleting attribute directly from style in this method
                // so one extra clone is needed. This case is rare and I think it's acceptable solution.
                if (sourceRunPr.Contains(FontAttr.CharacterCategoryHint))
                {
                    sourceRunPr = sourceRunPr.Clone();
                    sourceRunPr.Remove(FontAttr.CharacterCategoryHint);
                }

                sourceRunPr.ThemeColorInheritanceHack(dstRunPr);

                // Sometimes we shouldn't expand font size from the 'Normal' style.
                // See InlineHelper.IsIgnoreParaStyleFontSize() for details.
                if ((mSti == StyleIdentifier.Normal) && ((flags & RunPrExpandFlags.IgnoreNormalFontSize) != 0))
                {
                    sourceRunPr = sourceRunPr.Clone();

                    // WORDSNET-16374 Word does not ignore font size specified in the "Normal" style, when actual attributes
                    // contain size equals to 10 pt.
                    if ((sourceRunPr.Size == 24) && (dstRunPr.Size != 20))
                        sourceRunPr.Remove(FontAttr.Size);
                    if ((sourceRunPr.SizeBi == 24) && (dstRunPr.SizeBi != 20))
                        sourceRunPr.Remove(FontAttr.SizeBi);
                }

                // Formatting specified in this style override formatting specified in the base styles.
                if ((flags & RunPrExpandFlags.NoDirectFormatting) == 0)
                    sourceRunPr.ExpandTo(dstRunPr);
            }
        }

        /// <summary>
        /// Recursively expands font properties of this style and all based on styles into direct formatting.
        /// </summary>
        internal RunPr GetExpandedRunPr(RunPrExpandFlags flags)
        {
            RunPr dstRunPr = new RunPr();
            ExpandRunPr(dstRunPr, flags);
            return dstRunPr;
        }

        /// <summary>
        /// Gets a font attribute from this style, one of the parent styles, stylesheet defaults or global defaults.
        /// Note that getting from default values is only performed when isAllowDefault is true.
        ///
        /// If the attribute is not found and isAllowDefault = false, then returns <c>null</c>,
        /// because this is the behavior we expect from character styles: if a font attribute was not found,
        /// might need to look up the attribute in the paragraph style of the run.
        ///
        /// However, if the attribute is not found and isAllowDefault = true, then throws an exception
        /// because it is a programmer's error that a default value is not defined anywhere.
        /// </summary>
        internal object GetFontAttr(int key, bool isAllowDefault)
        {
            // Word ignores all formatting from DefaultParagraphFont.
            if (mSti != StyleIdentifier.DefaultParagraphFont)
            {
                object value = mRunPr.GetDirectAttr(key);
                if (value != null)
                {
                    //The value is specified directly, but might need to be resolved using parent values.
                    if (value is AttrBoolEx)
                    {
                        return ((AttrBoolEx)value).ResolveFetchInheritedRunAttr(this, key);
                    }
                    else
                    {
                        //The value is specified and does not need any special handling.
                        return value;
                    }
                }
            }

            //The value is not specified in this style, get it from parents.
            return GetInheritedFontAttr(key, isAllowDefault);
        }

        internal void Validate(IWarningCallback warningCallback)
        {
            RemoveRunPrIstdThatPointsToParaStyle(warningCallback);
            FixUpNonExistentLinkedStyle(warningCallback);
            RemoveNumberRevision();
            RemoveMagicFontSize();
            CheckReservedStyleIndex(warningCallback);
            ValidateTableNormal();

            // WORDSNET-9944 If OutlineLevel was not read for style IDs from Heading1 through Heading9,
            // then sets it to a corresponding value between 1 and 9, as MS Word behaves.
            FixUpOutlineLevel();
            ValidateOutlineLevel(warningCallback);
        }

        /// <summary>
        /// Copies generic styles attributes.
        /// </summary>
        internal void CopyGenerics(Style srcStyle)
        {
            IsQuickStyle = srcStyle.IsQuickStyle;
            AutomaticallyUpdate = srcStyle.AutomaticallyUpdate;
            Locked = srcStyle.Locked;
            UnhideWhenUsed = srcStyle.UnhideWhenUsed;
            PersonalCompose = srcStyle.PersonalCompose;
            PersonalReply = srcStyle.PersonalReply;
            Personal = srcStyle.Personal;
            LidsSet = srcStyle.LidsSet;
        }

        /// <summary>
        /// Indicates that style inherits run and para attributes from base style.
        /// </summary>
        /// <remarks>
        /// AM. Per WORDSNET-8011 run and para formatting inheritance from base table style was disabled.
        /// I tested and found that run and para formatting are inherited from base table style only if table style has at least one run or para attribute.
        /// It looks like a weird Word issue but lets mimic it.
        /// </remarks>
        private bool IsInheritRunAndParaFormatting
        {
            get
            {
                TableStyle tableStyle = this as TableStyle;

                // This logic is applied for table styles only.
                if (tableStyle == null)
                    return true;

                // If any of paragraph or text formatting specified for whole table.
                if ((tableStyle.ParaPr.Count > 0) || (tableStyle.RunPr.Count > 0))
                    return true;

                foreach (ConditionalStyle conditionalStyle in tableStyle.ConditionalStyles.DefinedStyles)
                {
                    // Or any of paragraph or text formatting specified for conditional formatting.
                    if ((conditionalStyle.ParaPr.Count > 0) || (conditionalStyle.RunPr.Count > 0))
                        return true;

                }

                // Otherwise for unknown reason base table styles formatting is not inherited.
                return false;
            }
        }

        /// <summary>
        /// Expands document-wide and/or global defaults depending on given flags.
        /// </summary>
        private void ExpandRunPrDefaults(RunPr dstRunPr, RunPrExpandFlags flags)
        {
            bool global = (flags & RunPrExpandFlags.GlobalDefaults) != 0;
            bool documentWide = (flags & RunPrExpandFlags.DocumentDefaults) != 0;

            RunPr runPr = documentWide ? Styles.DefaultRunPr : gEmptyRunPr;

            runPr.ExpandTo(dstRunPr, global);
        }

        /// <summary>
        /// Expands document-wide and/or global defaults depending on given flags.
        /// </summary>
        private void ExpandParaPrDefaults(ParaPr dstParaPr, ParaPrExpandFlags flags)
        {
            bool global = (flags & ParaPrExpandFlags.GlobalDefaults) != 0;
            bool documentWide = (flags & ParaPrExpandFlags.DocumentDefaults) != 0;

            ParaPr paraPr = documentWide ? Styles.DefaultParaPr : gEmptyParaPr;

            paraPr.ExpandTo(dstParaPr, global);
        }

        /// <summary>
        /// Preserves only acceptable values for Style.ParaPr.OutlineLevel
        /// </summary>
        private void ValidateOutlineLevel(IWarningCallback warningCallback)
        {
            // WORDSNET-10141 According to MSW behavior, all incorrect values of OutlineLevel just removed from document.
            OutlineLevel outlineLevel = ParaPr.OutlineLevel;
            if ((outlineLevel < OutlineLevel.Level1) || (OutlineLevel.BodyText < outlineLevel))
            {
                ParaPr.Remove(ParaAttr.OutlineLevel);
                WarnUnexpected(warningCallback, WarningStrings.InvalidStyleOutlineLevel, Name);
            }
        }

        /// <summary>
        /// Sometimes paragraph Style.RunPr.Istd points to that Style.Istd. This is no good.
        /// We detect this situation and remove the erroneous istd.
        /// </summary>
        private void RemoveRunPrIstdThatPointsToParaStyle(IWarningCallback warningCallback)
        {
            if ((Type == StyleType.Paragraph) && (RunPr.Istd == Istd))
            {
                WarnUnexpected(warningCallback, WarningStrings.InvalidParagraphStyleIstd, Name);
                RunPr.Remove(FontAttr.Istd);
            }
        }

        private void FixUpNonExistentLinkedStyle(IWarningCallback warningCallback)
        {
            if (LinkedIstd != StyleIndex.Nil)
                if (Styles.GetByIstd(LinkedIstd, false) == null)
                {
                    // WORDSNET-4382 Style.LinkedIstd points to non existent style. This makes MS Word 2007 crash on open.
                    WarnUnexpected(warningCallback, WarningStrings.StyleInvalidReference, Name);
                    LinkedIstd = StyleIndex.Nil;
                }
        }

        /// <summary>
        /// Styles are not expected to have any numbering revisions, but I've seen some.
        /// </summary>
        private void RemoveNumberRevision()
        {
            ParaPr.Remove(RevisionAttr.NumberRevision);
        }

        /// <summary>
        /// In glossary documents I see the Normal style can have 3276 font size value.
        /// Not sure what it is, let's just remove it.
        /// </summary>
        private void RemoveMagicFontSize()
        {
            RemoveMagicFontSize(RunPr, FontAttr.Size);
            RemoveMagicFontSize(RunPr, FontAttr.SizeBi);
        }

        private static void RemoveMagicFontSize(RunPr runPr, int key)
        {
            object sizeObj = runPr.GetDirectAttr(key);
            if ((sizeObj != null) && ((int)sizeObj == 3276))
                runPr.Remove(key);
        }

        /// <summary>
        /// Checks that StyleIndex corresponds to proper StyleIdentifier for Normal and DefaultParagraphFont styles.
        /// </summary>
        private void CheckReservedStyleIndex(IWarningCallback warningCallback)
        {
            // Word fix it when resaves, for the speed reason we don't fix it immediately because whole DOM update is needed.
            // So log warnings only and update styles in DocumentValidator.
            if ((Istd == StyleIndex.Normal) && (StyleIdentifier != StyleIdentifier.Normal))
                Warn(warningCallback, WarningType.UnexpectedContent, WarningSource.Validator,
                    string.Format(WarningStrings.ReservedStyleIndexUsed, "Normal", Name));

            if ((Istd == StyleIndex.DefaultParagraphFont) && (StyleIdentifier != StyleIdentifier.DefaultParagraphFont))
                Warn(warningCallback, WarningType.UnexpectedContent, WarningSource.Validator,
                    string.Format(WarningStrings.ReservedStyleIndexUsed, "DefaultParagraphFont", Name));
        }

        /// <summary>
        /// andrnosk: WORDSNET-8643 According to MS Word behavior style with StyleIdentifier equals TableNormal
        /// cannot contain RunPr and ParaPr, that is why we have to remove RunPr/ParaPr from this style.
        /// </summary>
        /// <remarks>
        /// Upon saving document to WML using MS Word 2013, its writes additional runPr to TableNormal style,
        /// and this runPr contains only wx:font and w:lang properties.
        /// These properties do not have any affect on the document, that is why I have decided do not write them to WML too.
        /// Note: previous versions MSW do not write rPr/pPr for TableNormal style too.
        /// </remarks>
        private void ValidateTableNormal()
        {
            if (StyleIdentifier == StyleIdentifier.TableNormal)
            {
                RunPr.Clear();
                ParaPr.Clear();

                // WORDSNET-23617 Force TableNormal to have defaults.
                TableStyle tableNormal = (TableStyle)this;

                TablePr tablePr = tableNormal.TablePr;

                // WORDSNET-27641 Remove borders and conditional formatting from TableNormal style.
                // AM. Actually Word always makes TableNormal style to have defaults only but this causes
                // many golds affected, lets remove problematic attributes only for a while.
                foreach (int key in TablePr.PossibleBorderKeys.Values)
                    tablePr.Remove(key);

                tableNormal.ConditionalStyles.Clear();

                tablePr.SetAttr(TableAttr.LeftPadding, 108);
                tablePr.SetAttr(TableAttr.RightPadding, 108);
                tablePr.SetAttr(TableAttr.TopPadding, 0);
                tablePr.SetAttr(TableAttr.BottomPadding, 0);
                tablePr.SetAttr(TableAttr.LeftIndent, 0);
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private static void Warn(IWarningCallback warningCallback, WarningType warningType, WarningSource warningSource, string description)
        {
            if (warningCallback != null)
                warningCallback.Warning(new WarningInfo(warningType, warningSource, description));
        }

        private static void WarnUnexpected(IWarningCallback warningCallback, string description, params object[] args)
        {
            WarningUtil.WarnUnexpected(warningCallback, description, args);
        }

        /// <summary>
        /// Gets a font attribute from one of the parent styles, stylesheet defaults or global defaults.
        /// See GetFontAttr for more info as these two functions work together.
        /// </summary>
        internal object GetInheritedFontAttr(int key, bool isAllowDefault)
        {
            Style baseStyle = GetBaseStyle();
            if (baseStyle != null)
            {
                //Parent style is available, get the value from it.
                return baseStyle.GetFontAttr(key, isAllowDefault);
            }
            else
            {
                //No more parent styles, look up in defaults if allowed.
                if (isAllowDefault)
                    return mStyles.DefaultRunPr.FetchAttr(key);
                else
                    return null;
            }
        }

        /// <summary>
        /// Sets OutlineLevel for style IDs from Heading1 through Heading9 to a corresponding value between 1 and 9.
        /// </summary>
        private void FixUpOutlineLevel()
        {
            if (!IsHeading)
                return;

            int currentOutlineLvl = (int)(StyleIdentifier - StyleIdentifier.Heading1);
            if ((ParaPr[ParaAttr.OutlineLevel] == null) || ((int)ParaPr[ParaAttr.OutlineLevel] != currentOutlineLvl))
                ParaPr.OutlineLevel = (OutlineLevel)currentOutlineLvl;
        }

        internal object GetParaAttr(int key, RevisionsView view)
        {
            object value = mParaPr.GetDirectAttr(key, view);
            return (value != null) ? value : GetInheritedParaAttr(key, view);
        }

        /// <summary>
        /// Fetches inherited attribute value.
        /// </summary>
        private object GetInheritedParaAttr(int key, RevisionsView view)
        {
            // Gets a paragraph attribute from one of the parent styles or global defaults.
            // Style can refer to a list. Check this first.
            // But there could be some really rare cases like in 18079 where the list id is specified in a based on style but list level
            // is specified in a derived style. I don't think it is possible to correctly resolve at all using this bottom-up method.
            // However it is handled correctly in the full top-bottom expansion and that's important for rendering.
            object value = GetAttrFromReferredList(key);

            if (value != null)
                return value;

            Style baseStyle = GetBaseStyle();
            return (baseStyle != null) ? baseStyle.GetParaAttr(key, view) : null;
        }

        /// <summary>
        /// Style can refer to a list. Check this first.
        /// </summary>
        internal object GetAttrFromReferredList(int key)
        {
            // WORDSNET-11258 Obviously, the following attributes cannot be defined in ListLevel.
            // So, let's exit if one of them is requested to prevent from infinite loop
            // when this method is involved in recursion.
            if ((key == ParaAttr.ListId) || (key == ParaAttr.ListLevel))
                return null;

            // WORDSNET-14454 Should get attribute from the list only if this style has it in a direct attribute collection.
            // We try to get List and corresponding ListLevel from the chain of the base styles,
            // because ListId and ListLevel can be defined in a different base styles (see more details in GetInheritedParaAttr()).
            // Otherwise, there can be a situation when the requested attribute in direct attributes of some base style
            // is occurred earlier then in list. And therefore, we will skip it in GetInheritedParaAttr() as
            // code for getting direct attributes of base style will not be reached.
            if (mParaPr.Contains(ParaAttr.ListId) || mParaPr.Contains(ParaAttr.ListLevel))
            {
                int listId = (int)((IParaAttrSource)this).FetchParaAttr(ParaAttr.ListId);
                if (listId != 0)
                {
                    List list = Document.Lists.FetchListByListIdResolveStyleReference(listId);

                    int level = (int)((IParaAttrSource)this).FetchParaAttr(ParaAttr.ListLevel);
                    ListLevel listLevel = list.GetListLevelOverrideAware(level);

                    return listLevel.ParaPr.GetDirectAttr(key);
                }
                else if (mParaPr.IsExplicitlyNotListItem)
                {
                    // WORDSNET-5623 Consider explicitly no list attribute.
                    if (ParaPr.IsNoListAttr(key))
                        return 0;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns style's List or <c>null</c>.
        /// </summary>
        internal List GetListInternal()
        {
            if (mParaPr[ParaAttr.ListId] != null)
            {
                int listId = (int)mParaPr[ParaAttr.ListId];

                if ((listId != 0) && (Document != null))
                    return Document.Lists.GetListByListId(listId);
            }

            return null;
        }

        #region IParaAttrSource

        object IParaAttrSource.GetDirectParaAttr(int key)
        {
            return mParaPr.GetDirectAttr(key);
        }

        object IParaAttrSource.GetDirectParaAttr(int key, RevisionsView revisionsView)
        {
            return mParaPr.GetDirectAttr(key, revisionsView);
        }

        object IParaAttrSource.FetchParaAttr(int key)
        {
            object value = mParaPr[key];
            return (value != null) ? value : ((IParaAttrSource)this).FetchInheritedParaAttr(key);
        }

        object IParaAttrSource.FetchInheritedParaAttr(int key)
        {
            object value = GetInheritedParaAttr(key, RevisionsView.Original);
            return (value != null) ? value : Styles.DefaultParaPr.FetchAttr(key);
        }

        void IParaAttrSource.SetParaAttr(int key, object value)
        {
            mParaPr.SetAttr(key, value);
        }

        void IParaAttrSource.RemoveParaAttr(int key)
        {
            mParaPr.Remove(key);
        }

        void IParaAttrSource.ClearParaAttrs()
        {
            mParaPr.Clear();
        }

        #endregion

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return mRunPr.GetDirectAttr(key);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mRunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            return GetInheritedFontAttr(key, true);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            mRunPr.SetAttr(key, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mRunPr.Clear();
        }

        #endregion

        /// <summary>
        /// Core method for comparing with the specified style.
        /// Compares style specific attributes, direct style formatting and list formatting.
        /// Does not compare base, linked and next paragraph styles for the specified style.
        /// </summary>
        private bool EqualsCore(Style style, HashSetGeneric<Pair> alreadyCompared)
        {
            if (style == null)
                return false;

            alreadyCompared.Add(new Pair(this, style));

            // We always treat non-modifiable styles as equal if their style identifiers are equal.
            // For example, see TestStyleImport.TestJira11294_1().
            if (StyleIndex.IsNonModifiable(style) && (this.StyleIdentifier == style.StyleIdentifier))
                return true;

            // Compare built-in reserved styles.
            if (!BuiltinReservedEquals(style))
                return false;

            // Compare style specific properties.
            if (!GenericEquals(style))
                return false;

            // Compare direct style formatting.
            if (!DirectFormattingEquals(style))
                return false;

            // Compare list formatting.
            List listA = GetListInternal();
            List listB = style.GetListInternal();

            if (ArgumentUtil.OneIsNull(listA, listB))
                return false;

            if (!ArgumentUtil.BothAreNull(listA, listB) && !listA.EqualsCore(listB, alreadyCompared))
                return false;

            return true;
        }

        /// <summary>
        /// Checks equality of built-in styles with reserved Istds.
        /// </summary>
        private bool BuiltinReservedEquals(Style style)
        {
            if ((this.BuiltIn && style.BuiltIn) &&
                ((StyleIndex.IsIstdReserved(this.Istd) || StyleIndex.IsIstdReserved(style.Istd)) &&
                 (this.Istd != style.Istd)))
                return false;

            return true;
        }

        /// <summary>
        /// Compares generic styles attributes.
        /// </summary>
        private bool GenericEquals(Style style)
        {
            // Ignore Istd, Rsid, UIPriority, Hidden, SemiHidden, IsHidden, Name and NextParagraphName, Identifier.
            return ((Type == style.Type) &&
                    (IsHeading == style.IsHeading) &&
                    (IsQuickStyle == style.IsQuickStyle) &&
                    (AutomaticallyUpdate == style.AutomaticallyUpdate) &&
                    (Locked == style.Locked) &&
                    (UnhideWhenUsed == style.UnhideWhenUsed) &&
                    (PersonalCompose == style.PersonalCompose) &&
                    (PersonalReply == style.PersonalReply) &&
                    (Personal == style.Personal) &&
                    (LidsSet == style.LidsSet) &&
                    (IsTopLevelParaStyle == style.IsTopLevelParaStyle) &&
                    (HasRevisions == style.HasRevisions));
        }

        /// <summary>
        /// Compares direct styles formatting.
        /// </summary>
        private bool DirectFormattingEquals(Style style)
        {
            switch (Type)
            {
                case StyleType.Character:
                    return this.RunPr.Equals(style.RunPr, ComparisonIgnorableKeys);

                case StyleType.Paragraph:
                    return this.RunPr.Equals(style.RunPr, ComparisonIgnorableKeys) &&
                           this.ParaPr.Equals(style.ParaPr, ComparisonIgnorableKeys);

                case StyleType.Table:
                    TableStyle tableStyleA = (TableStyle)this;
                    TableStyle tableStyleB = (TableStyle)style;
                    ConditionalStyleCollection conditionalStylesA = tableStyleA.ConditionalStyles;
                    ConditionalStyleCollection conditionalStylesB = tableStyleB.ConditionalStyles;

                    ConditionalStyleType[] types = (ConditionalStyleType[])Enum.GetValues(typeof(ConditionalStyleType));
                    foreach (ConditionalStyleType type in types)
                    {
                        if (!conditionalStylesA.ContainsConditionalStyle(type) &&
                            !conditionalStylesB.ContainsConditionalStyle(type))
                            continue;

                        if (!conditionalStylesA[type].Equals(conditionalStylesB[type]))
                            return false;
                    }

                    return tableStyleA.RunPr.Equals(tableStyleB.RunPr, ComparisonIgnorableKeys) &&
                           tableStyleA.ParaPr.Equals(tableStyleB.ParaPr, ComparisonIgnorableKeys) &&
                           tableStyleA.CellPr.Equals(tableStyleB.CellPr, ComparisonIgnorableKeys) &&
                           tableStyleA.RowPr.Equals(tableStyleB.RowPr, ComparisonIgnorableKeys) &&
                           tableStyleA.TablePr.Equals(tableStyleB.TablePr, ComparisonIgnorableKeys);

                case StyleType.List:
                    // Lists are compared in special way separately in EqualsCore().
                    return true;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected style type '{0}'.", Type));
            }
        }

#if DEBUG
        public override string ToString()
        {
            return Name;
        }
#endif

        /// <summary>
        /// Only needed for DOC format TestStyleFiler to pass.
        /// </summary>
        internal bool RawInvalidHeight;
        /// <summary>
        /// Only needed for DOC format TestStyleFiler to pass.
        /// </summary>
        internal bool RawHasUpe;
        /// <summary>
        /// Only needed for DOC format TestStyleFiler to pass.
        /// </summary>
        internal bool RawInternalUse;

        /// <summary>
        /// These keys will be ignored in properties comparison.
        /// </summary>
        internal static readonly int[] ComparisonIgnorableKeys = new int[]
        {
            FontAttr.RsidRPr, FontAttr.RsidR, FontAttr.Istd, ParaAttr.Istd, ParaAttr.ListId, ParaAttr.RsidP,
            // WORDSNET-17538 These unwanted attributes appears in the imported style while copying source formatting to
            // destination. Process of copying formatting is complex and uses expand and collapse operations. As the result,
            // initially equal source and destination styles become different during comparison.
            ParaAttr.Sys_Alignment97, ParaAttr.Sys_LeftIndent97,  ParaAttr.Sys_RightIndent97, ParaAttr.Sys_FirstLineIndent97,
            TableAttr.Istd, TableAttr.RsidTr
        };

        private int mIstd = StyleIndex.Nil;
        /// <summary>
        /// sti (style identifier) of the style.
        /// </summary>
        private StyleIdentifier mSti = StyleIdentifier.Nil;
        private StyleIdentifier mClonedFromSti = StyleIdentifier.Nil;
        private StyleType mType;
        private int mBasedOnIstd = StyleIndex.Nil;
        private int mNextIstd = StyleIndex.Nil;

        private bool mAutomaticallyUpdate;
        private bool mHidden;
        private bool mSemiHidden;
        private bool mLocked;
        private bool mUnhideWhenUsed;
        private bool mIsQuickStyle;
        private int mUIPriority;

        private bool mPersonalCompose;
        private bool mPersonalReply;
        private bool mPersonal;

        private bool mLidsSet;
        private int mLinkedIstd = StyleIndex.Nil;
        private int mRsid;
        private string mName;
        private ParaPr mParaPr = new ParaPr();
        private RunPr mRunPr = new RunPr();

        /// <summary>
        /// Pointer to the parent collection of styles.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private StyleCollection mStyles;
        /// <summary>
        /// Facade object to access run attributes.
        /// </summary>
        private Font mFontCache;

        /// <summary>
        /// Facade object to access paragraph attributes.
        /// </summary>
        private ParagraphFormat mParagraphFormatCache;

        /// <summary>
        /// Facade object to access paragraph frame attributes.
        /// </summary>
        private FrameFormat mFrameFormatCache;

        /// <summary>
        /// Facade object to access list formatting attributes.
        /// </summary>
        private ListFormat mListFormatCache;

        // Dummy empty attributes collection.
        private static readonly RunPr gEmptyRunPr = new RunPr();
        private static readonly ParaPr gEmptyParaPr = new ParaPr();

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxUIPriority = 99;
    }
}
