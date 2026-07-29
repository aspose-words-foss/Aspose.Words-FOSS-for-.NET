// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2005 by Roman Korchagin

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Tables;
using Aspose.Words.Themes;
using Aspose.Words.Validation;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="Style"/> objects that represent both the built-in and user-defined styles in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-styles-and-themes/">Working with Styles and Themes</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// At the moment the styles are identified by istd (style index) that never changes (it only changes
    /// when importing a style from another document). Will it be better if I used a string name as an
    /// identifier? Note however, it should not change once it has been assigned, otherwise will need to
    /// update all references to the style in the model.
    /// </dev>
    public class StyleCollection : IEnumerable<Style>
    {
        internal StyleCollection(DocumentBase doc)
        {
            mDoc = doc;

        }

        /// <summary>
        /// Removes all styles from the Quick Style Gallery panel.
        /// </summary>
        public void ClearQuickStyleGallery()
        {
            foreach (Style style in this)
                if (style.IsQuickStyle)
                    style.IsQuickStyle = false;

            for (int i = 0; i < LatentStyles.Count; i++)
                if (LatentStyles[i].QuickStyle)
                    LatentStyles[i].QuickStyle = false;
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        public DocumentBase Document
        {
            get { return mDoc; }
        }

        /// <summary>
        /// Gets document default text formatting.
        /// </summary>
        /// <remarks>
        /// <p>Note that document-wide defaults were introduced in Microsoft Word 2007 and are fully supported in OOXML formats (<see cref="LoadFormat.Docx" />) only.
        /// Earlier document formats have limited support for this feature and only font names can be stored.</p>
        /// </remarks>
        public Font DefaultFont
        {
            get
            {
                if (mDefaultFont == null)
                    mDefaultFont = new Font(DefaultRunPr, mDoc);

                return mDefaultFont;
            }
        }

        /// <summary>
        /// Gets document default paragraph formatting.
        /// </summary>
        /// <remarks>
        /// <p>Note that document-wide defaults were introduced in Microsoft Word 2007 and are fully supported in OOXML formats (<see cref="LoadFormat.Docx" />) only.
        /// Earlier document formats have no support for document default paragraph formatting.</p>
        /// </remarks>
        /// <dev>
        /// Default paragraph formatting is copied to all top level styles when it is not supported in target document format.
        /// </dev>
        public ParagraphFormat DefaultParagraphFormat
        {
            get
            {
                if (mDefaultParagraphFormat == null)
                    mDefaultParagraphFormat = new ParagraphFormat(DefaultParaPr, this);

                return mDefaultParagraphFormat;
            }
        }

        /// <summary>
        /// Gets the number of styles in the collection.
        /// </summary>
        public int Count
        {
            get { return mStylesByIstd.Count; }
        }

        /// <overloads>Retrieves a style from the collection.</overloads>
        /// <summary>
        /// Gets a style by name or alias.
        /// </summary>
        /// <remarks>
        /// <p>Case sensitive, returns <c>null</c> if the style with the given name is not found.</p>
        /// <p>If this is an English name of a built in style that does not yet exist, automatically creates it.</p>
        /// </remarks>
        public Style this[string name]
        {
            get { return GetByName(name, true); }
        }

        /// <summary>
        /// Gets a built-in style by its locale independent identifier.
        /// </summary>
        /// <remarks>
        /// <p>When accessing a style that does not yet exist, automatically creates it.</p>
        /// </remarks>
        /// <param name="sti">A <see cref="StyleIdentifier"/> value that specifies the built in style to retrieve.</param>
        public Style this[StyleIdentifier sti]
        {
            get { return GetBySti(sti, true); }
        }

        /// <summary>
        /// Gets a style by index.
        /// </summary>
        /// <dev>
        /// Gets by index, not by istd.
        /// </dev>
        public Style this[int index]
        {
            get { return mStylesByIstd.GetByIndex(index); }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Gets a style by name or alias.
        /// </summary>
        public Style GetByName(string name)
        {
            return this[name];
        }

        /// <summary>
        /// Gets a built-in style by its locale independent identifier.
        /// </summary>
        public Style GetByStyleIdentifier(StyleIdentifier sti)
        {
            return this[sti];
        }
#endif

        /// <summary>
        /// Validates this style collection.
        /// </summary>
        internal void Validate()
        {
            FixBalloonStyles();

            // TableNormal MUST be a TableStyle.
            // AM. Seems we need something we do in EnsureMinimum method but many golds have to be re-accepted then.
            // Postpone for a while.
            Style tableNormal = GetByName("Table Normal", false);
            if ((tableNormal != null) && (tableNormal.Type != StyleType.Table))
                RemoveCore(tableNormal);
        }

        /// <summary>
        /// This is the max istd currently in use.
        /// </summary>
        internal int MaxIstd
        {
            get { return (mStylesByIstd.Count > 0) ? mStylesByIstd.GetKey(mStylesByIstd.Count - 1) : -1; }
        }

        /// <summary>
        /// Document default run properties.
        /// </summary>
        internal RunPr DefaultRunPr
        {
            get { return mDefaultRunPr; }
            set { mDefaultRunPr = value; }
        }

        /// <summary>
        /// Document default paragraph properties.
        /// </summary>
        internal ParaPr DefaultParaPr
        {
            get { return mDefaultParaPr; }
        }

        /// <summary>
        /// Checks that document-wide defaults contain other than standard attributes and
        /// should be expanded.
        /// </summary>
        internal bool IsExpandDocumentDefaultsNeeded
        {
            get
            {
                // Expanding is needed for this format in general, now determine if we really
                // need to expand for this document.
                // Expand if we have any default paragraph formatting.
                if (DefaultParaPr.Count > 0)
                    return true;

                // Expand if we have default run formatting other than "standard" attributes.
                for (int i = 0; i < DefaultRunPr.Count; i++)
                {
                    if (!IsIgnorableDefaultRunAttr(DefaultRunPr.GetKey(i)))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns true if the attribute is one of the "standard" ones that we think should be ignored during expanding of document defaults.
        /// </summary>
        internal static bool IsIgnorableDefaultRunAttr(int key)
        {
            return ((key == FontAttr.NameAscii) ||
                    (key == FontAttr.NameFarEast) ||
                    (key == FontAttr.NameOther) ||
                    (key == FontAttr.NameBi) ||
                    (key == FontAttr.LocaleId) ||
                    (key == FontAttr.LocaleIdFarEast) ||
                    (key == FontAttr.LocaleIdBi));
        }

        /// <summary>
        /// Gets a singleton collection of all known MS Word 97-2003 built in styles.
        /// Used as prototypes for creating built in styles on demand.
        /// </summary>
        private static StyleCollection BuiltInStyles2003
        {
            get
            {
                // Implementing Singleton in C# http://msdn.microsoft.com/en-us/library/ms998558.aspx
                if (gAllStylesCache2003 == null)
                {
                    lock (gAllStylesSyncRoot2003)
                    {
                        if (gAllStylesCache2003 == null)
                        {
                            gAllStylesCache2003 = ReadBuiltInStylesResource("Aspose.Words.Resources.AllStyles2003.docx");
                        }
                    }
                }

                return gAllStylesCache2003.Styles;
            }
        }

        /// <summary>
        /// Gets a singleton collection of all known MS Word 2007 built in styles.
        /// Used as prototypes for creating built in styles on demand.
        /// </summary>
        private static StyleCollection BuiltInStyles2007
        {
            get
            {
                // Implementing Singleton in C# http://msdn.microsoft.com/en-us/library/ms998558.aspx
                if (gAllStylesCache2007 == null)
                {
                    lock (gAllStylesSyncRoot2007)
                    {
                        if (gAllStylesCache2007 == null)
                        {
                            gAllStylesCache2007 = ReadBuiltInStylesResource("Aspose.Words.Resources.AllStyles2007.docx");
                        }
                    }
                }

                return gAllStylesCache2007.Styles;
            }
        }

        /// <summary>
        /// Gets a singleton collection of all known MS Word 2013 built in styles.
        /// Used as prototypes for creating built in styles on demand.
        /// </summary>
        private static StyleCollection BuiltInStyles2013
        {
            get
            {
                // Implementing Singleton in C# http://msdn.microsoft.com/en-us/library/ms998558.aspx
                if (gAllStylesCache2013 == null)
                {
                    lock (gAllStylesSyncRoot2013)
                    {
                        if (gAllStylesCache2013 == null)
                        {
                            gAllStylesCache2013 = ReadBuiltInStylesResource("Aspose.Words.Resources.AllStyles2013.docx");
                        }
                    }
                }

                return gAllStylesCache2013.Styles;
            }
        }

        /// <summary>
        /// Get BuildIn StyleCollection depending on MS Word version used to create this document or set upon processing.
        /// </summary>
        internal StyleCollection BuiltInStyles
        {
            get
            {
                Document document = (Document.NodeType == NodeType.Document) ? (Document)Document : ((GlossaryDocument)Document).MainDocument;
                MsWordVersionCore mswVersion = document.CompatibilityOptions.MswVersion;
                if (mswVersion != MsWordVersionCore.Unspecified)
                {
                    switch (mswVersion)
                    {
                        case MsWordVersionCore.Word1997:
                        case MsWordVersionCore.Word2000:
                        case MsWordVersionCore.Word2002:
                        case MsWordVersionCore.Word2003:
                            return BuiltInStyles2003;
                        case MsWordVersionCore.Word2007:
                        case MsWordVersionCore.Word2010:
                            return BuiltInStyles2007;
                        case MsWordVersionCore.Word2013:
                        case MsWordVersionCore.Word2016:
                        case MsWordVersionCore.Word2019:
                            return BuiltInStyles2013;
                        default:
                            break;
                    }
                }

                return GetBuiltInStyles(GetLoadFormat());
            }
        }

        /// <summary>
        /// Get BuildIn StyleCollection depending on load format.
        /// </summary>
        internal static StyleCollection GetBuiltInStyles(LoadFormat loadFormat)
        {
            switch (loadFormat)
            {
                case LoadFormat.Docx:
                case LoadFormat.Docm:
                case LoadFormat.Dotx:
                case LoadFormat.Dotm:
                case LoadFormat.FlatOpc:
                case LoadFormat.FlatOpcMacroEnabled:
                case LoadFormat.FlatOpcTemplate:
                case LoadFormat.FlatOpcTemplateMacroEnabled:
                    return BuiltInStyles2007;
                case LoadFormat.Doc:
                default:
                    return BuiltInStyles2003;
            }
        }

        /// <summary>
        /// Read document with styles from resource.
        /// </summary>
        /// <param name="resourceName">Resource name AllStyles2003 or AllStyles2007 or AllStyles2013.</param>
        /// <returns>Document with styles.</returns>
        [JavaThrows(false)]
        private static Document ReadBuiltInStylesResource(string resourceName)
        {
            try
            {
                // Load the document that contains all built-in styles instantiated.
                // Here is a pitfall: See comment in TestDefect10652.
                using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
                {
                    // alexnosk WORDSNET-19906 Specify LoadFormat.Docx explicitly to skip file format detection to minimize document creation time.
                    // Since we are sure AllStylesXXXX.docx is not encrypted DOCX file, it is safe to specify it explicitly.
                    LoadOptions loadOptions = new LoadOptions();
                    loadOptions.LoadFormat = LoadFormat.Docx;
                    Document doc = new Document(stream, loadOptions, false);
                    // This document contains some locale identifiers assigned to the styles that we don't want, remove them.
                    doc.Styles.RemoveLocaleIdsFromStyles();
                    return doc;
                }
            }
            catch (Exception e)
            {
                // JAVA: Catch and rethrow RuntimeException for java.
                // This needed to not propagate throws exception clause to attribute fetching methods.
                throw new InvalidOperationException("Cannot load built in styles from an embedded resource.", e);
            }
        }

        internal LatentStyles LatentStyles
        {
            get { return mLatentStyles; }
        }

        /// <summary>
        /// Return true if this collection is BuiltIn styles template resource
        /// <see cref="gAllStylesCache2003"/> or <see cref="gAllStylesCache2007"/> or <see cref="gAllStylesCache2013"/>.
        /// </summary>
        private bool IsBuiltinStylesTemplate
        {
            get { return (Document == gAllStylesCache2003) || (Document == gAllStylesCache2007) || (Document == gAllStylesCache2013); }
        }

        /// <summary>
        /// Gets the next free istd. Always returns a value that is greater than fixed-index (reserved) istds.
        /// </summary>
        internal int GetNextFreeIstd()
        {
            // WORDSNET-1389 Too many styles created results in an invalid document that hangs MS Word.
            if (!HasFreeIstd())
            {
                throw new InvalidOperationException("There are too many styles in the document.");
            }

            return CalculateNextIstd();
        }

        /// <summary>
        /// Returns "true", when there is an available istd.
        /// </summary>
        internal bool HasFreeIstd()
        {
            int nextIstd = CalculateNextIstd();
            return nextIstd < StyleIndex.Limit;
        }

        /// <summary>
        /// Calculates the next istd value.
        /// </summary>
        private int CalculateNextIstd()
        {
            // Make sure we return a number that is equal or greater than the max fixed-index (reserved) istd.
            return System.Math.Max(MaxIstd, MaxFixedIstd) + 1;
        }

        /// <summary>
        /// Gets an enumerator object that will enumerate styles in the alphabetical order of their names.
        /// </summary>
        public IEnumerator<Style> GetEnumerator()
        {
            List<Style> result = new List<Style>();

            foreach (KeyValuePair<string, Style> entry in mStylesByName)
            {
                string name = entry.Key;
                Style style = entry.Value;
                if (style.Name == name)
                    result.Add(style);
            }

            result.Sort(new ComparerByName());
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a new user defined style and adds it the collection.
        /// </summary>
        /// <remarks>
        /// <p>You can create character, paragraph or a list style.</p>
        ///
        /// <p>When creating a list style, the style is created with default numbered list formatting (1 \ a \ i).</p>
        ///
        /// <p>Throws an exception if a style with this name already exists.</p>
        /// </remarks>
        /// <param name="type">A <see cref="StyleType"/> value that specifies the type of the style to create.</param>
        /// <param name="name">Case sensitive name of the style to create.</param>
        public Style Add(StyleType type, string name)
        {
            ArgumentUtil.CheckHasChars(name, "name");

            Style style = Style.Create(type, GetNextFreeIstd(), StyleIdentifier.User, name);

            // This is a "constructor of a user defined list style".
            // Not sure if here is the best place for it, but no better ideas at the moment.
            if (type == StyleType.List)
            {
                // Create a list that defines the list style. Make the list point to the style.
                List list = ListFactory.CreateList(Document.Lists, ListTemplate.NumberDefault);
                list.ListDef.ListStyleIstd = style.Istd;
                // Make the list style to point to the list that defines it.
                style.ParaPr.ListId = list.ListId;
            }

            Add(style);
            return style;
        }

        /// <summary>
        /// Removes style from styles collection. Does not remove built-in styles.
        /// </summary>
        internal void Remove(string styleName)
        {
            Style removedStyle = GetByName(styleName, false);

            if (removedStyle == null)
                throw new ArgumentException("Style not found.");

            Style linkedStyle = removedStyle.GetLinkedStyle();
            int linkedStyleLinksCount = 0;

            // We should expand all child of this style, before removing it.
            foreach (Style style in this)
            {
                if (style.BasedOnIstd == removedStyle.Istd)
                {
                    ExpandStyle(style, removedStyle);

                    style.BasedOnIstd = GetDefaultStyleIndex(style.Type);

                    // NextIstd has meaning only for paragraph styles.
                    if ((style.Type == StyleType.Paragraph) && (style.NextIstd == removedStyle.Istd))
                        style.NextIstd = style.Istd;
                }
                else
                {
                    if ((style.Type == StyleType.Paragraph) && (style.NextIstd == removedStyle.Istd))
                        style.NextIstd = StyleIndex.Normal;
                }

                // WORDSNET-28631 Collect a number of links to the linked style.
                if ((linkedStyle != null) && (style.LinkedIstd == linkedStyle.Istd))
                    linkedStyleLinksCount++;
            }

            // When removing style Word doesn't expand formatting to nodes that style being removed is applied and
            // just removes Istd, so node looses any formatting applied by style.
            UpdateDomWithIstd(removedStyle, removedStyle.Istd, -1);

            if ((removedStyle.HasRevisions) && (removedStyle.Document is Document)) // remove style with revisions from RevisionCollection.
                ((Document)removedStyle.Document).Revisions.Remove(removedStyle);

            RemoveCore(removedStyle);

            // WORDSNET-28631 Remove linked style only if there is a single
            // reference to this style is remained in the style collection.
            if ((linkedStyle != null) && (linkedStyleLinksCount == 1))
            {
                // Avoid recursion.
                linkedStyle.LinkedIstd = StyleIndex.Nil;

                Remove(linkedStyle.Name);
            }
        }

        /// <summary>
        /// Removes styles from collection. Does not update any references so document can become invalid.
        /// </summary>
        internal void RemoveCore(Style style)
        {
            if (BuiltInStyles.GetByName(style.Name, false) != null)
                mBuiltInStylesBySti.Remove((int)style.StyleIdentifier);

            mStylesByIstd.Remove(style.Istd);
            RemoveFromByNameList(style);

            mStyleAliasesCache = null;
        }

        /// <summary>
        /// Copies all styles from the specified template collection into this collection.
        /// </summary>
        internal void CopyStylesFromTemplate(StyleCollection template)
        {
            // Copy document defaults.
            mDefaultRunPr = template.DefaultRunPr.Clone();
            mDefaultParaPr = template.DefaultParaPr.Clone();

            CopyAll(template, new ImportContext(template, this));
        }

        /// <summary>
        /// Removes all items, which reference to the style, from the mStylesByName list.
        /// </summary>
        private void RemoveFromByNameList(Style style)
        {
            // mStylesByName may contain aliases of the style
            for (int i = mStylesByName.Count - 1; i >= 0; i--)
                if (mStylesByName.GetByIndex(i) == style)
                    mStylesByName.RemoveAt(i);
        }

        /// <summary>
        /// Returns default StyleIndex for given styleType.
        /// </summary>
        private static int GetDefaultStyleIndex(StyleType styleType)
        {
            switch (styleType)
            {
                case StyleType.Character:
                    return StyleIndex.DefaultParagraphFont;
                case StyleType.Paragraph:
                    return StyleIndex.Normal;
                case StyleType.Table:
                    return StyleIndex.TableNormal;
                default:
                    return StyleIndex.Nil;
            }
        }

        private void ProcessTableStyle(int istd, int newIstd)
        {
            foreach (Row row in Document.GetChildNodes(NodeType.Row, true))
            {
                TablePr tablePr = row.TablePr;
                if (tablePr.Istd == istd)
                {
                    if (newIstd == -1)
                        tablePr.Remove(TableAttr.Istd);
                    else
                        tablePr[TableAttr.Istd] = newIstd;
                }
            }
        }

        private void ProcessListStyle(Style style, int istd, int newIstd)
        {
            foreach (Paragraph para in Document.GetChildNodes(NodeType.Paragraph, true))
            {
                ParaPr paraPr = para.ParaPr;
                if ((paraPr.ListId != 0) && (para.ListFormat.List.Style != null) && (para.ListFormat.List.Style.Istd == istd))
                {
                    if (newIstd == -1)
                    {
                        paraPr.Remove(ParaAttr.ListId);
                        paraPr.Remove(ParaAttr.ListLevel);
                    }
                    else
                    {
                        paraPr[ParaAttr.ListId] = style.List.ListId;
                    }
                }
            }
        }

        private void ProcessParaStyle(int istd, int newIstd)
        {
            foreach (Paragraph para in Document.GetChildNodes(NodeType.Paragraph, true))
            {
                ParaPr paraPr = para.ParaPr;
                if (paraPr.Istd == istd)
                {
                    if (newIstd == -1)
                        paraPr.Remove(ParaAttr.Istd);
                    else
                        paraPr[ParaAttr.Istd] = newIstd;
                }
            }
        }

        private void ProcessCharStyle(int istd, int newIstd)
        {
            foreach (Paragraph para in Document.GetChildNodes(NodeType.Paragraph, true))
            {
                UpdateRunPr(para.ParagraphBreakRunPr, istd, newIstd);

                foreach (Run run in para.Runs)
                    UpdateRunPr(run.RunPr, istd, newIstd);
            }
        }

        private static void UpdateRunPr(RunPr runPr, int istd, int newIstd)
        {
            if (runPr.Istd == istd)
            {
                if (newIstd == -1)
                    runPr.Remove(FontAttr.Istd);
                else
                    runPr[FontAttr.Istd] = newIstd;
            }
        }

        /// <summary>
        /// Expand child style.
        /// </summary>
        private static void ExpandStyle(Style style, Style removedStyle)
        {
            switch (removedStyle.Type)
            {
                case StyleType.Character:
                    style.RunPr = style.GetExpandedRunPr(RunPrExpandFlags.Normal);
                    break;

                case StyleType.Paragraph:
                    style.RunPr = style.GetExpandedRunPr(RunPrExpandFlags.Normal);
                    style.ParaPr = style.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);
                    break;

                case StyleType.Table:
                {
                    TableStyle tableStyle = (TableStyle)style;
                    tableStyle.RowPr = tableStyle.GetExpandedRowPr();
                    tableStyle.CellPr = tableStyle.GetExpandedCellPr();
                    tableStyle.TablePr = tableStyle.GetExpandedTablePr();
                    style.RunPr = style.GetExpandedRunPr(RunPrExpandFlags.Normal);
                    style.ParaPr = style.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);
                    break;
                }
                case StyleType.List:
                    // Do nothing.
                    break;

                default:
                    throw new ArgumentException("Unexpected style type.");
            }
        }

        /// <summary>
        /// Adds the style to the collection of styles.
        /// </summary>
        internal void Add(Style style)
        {
            if (style == null)
                throw new ArgumentNullException("style");
            if (style.Styles != null)
                throw new ArgumentException("Style already belongs to another collection of styles.");

            if (mStylesByName.ContainsKey(style.Name))
                throw new ArgumentException("Cannot add a style because a style with the same name already exists.");

            if (style.BuiltIn && mBuiltInStylesBySti.ContainsKey((int)style.StyleIdentifier))
                throw new ArgumentException("Cannot add a style because a style with this identifier already exists.");

            mStylesByIstd.Add(style.Istd, style);
            mStylesByName.Add(style.Name, style);
            if (style.BuiltIn)
                mBuiltInStylesBySti.Add((int)style.StyleIdentifier, style);

            style.SetStyles(this);

            mStyleAliasesCache = null;
        }

        /// <summary>
        /// Update collection cache with new style name.
        /// </summary>
        internal void UpdateNameMap(Style newStyle, string oldName, string newName)
        {
            mStylesByName.Remove(oldName);

            Style oldStyle = (Style)mStylesByName[newName];
            mStylesByName[newName] = newStyle;
            // WORDSNET-14289 If a replaced style is not an alias, let's also remove all aliases of it.
            if ((oldStyle != null) && (oldStyle != newStyle) && (oldStyle.Name == newName))
                RemoveFromByNameList(oldStyle);

            mStyleAliasesCache = null;
        }

        internal void UpdateStiMap(Style style, StyleIdentifier oldSti, StyleIdentifier newSti)
        {
            Debug.Assert(oldSti != newSti);
            if (style.BuiltIn)
                mBuiltInStylesBySti.Remove((int)oldSti);

            if (newSti != StyleIdentifier.User)
            {
                if (mBuiltInStylesBySti.ContainsKey((int)newSti))
                    mBuiltInStylesBySti[(int)newSti] = style;
                else
                    mBuiltInStylesBySti.Add((int)newSti, style);
            }
        }

        internal void UpdateIstdMap(Style style, int oldIstd, int newIstd)
        {
            mStylesByIstd.Remove(oldIstd);
            if (mStylesByIstd.ContainsKey(newIstd))
                mStylesByIstd[newIstd] = style;
            else
                mStylesByIstd.Add(newIstd, style);

            UpdateModelWithIstd(style, oldIstd, newIstd);
        }

        /// <summary>
        /// Regenerates the <see cref="mStylesByIstd"/> map. It is executed from <see cref="StyleIstdNormalizer"/> after
        /// generation new <see cref="Style.Istd"/>.
        /// </summary>
        internal void UpdateIstdMap()
        {
            SortedIntegerListGeneric<Style> newStylesByIstd = new SortedIntegerListGeneric<Style>(mStylesByIstd.Count);

            for (int i = 0; i < mStylesByIstd.Count; i++)
            {
                Style style = mStylesByIstd.GetByIndex(i);
                newStylesByIstd.Add(style.Istd, style);
            }

            mStylesByIstd = newStylesByIstd;
        }

        private void UpdateModelWithIstd(Style newStyle, int istd, int newIstd)
        {
            UpdateStyles(istd, newIstd);
            UpdateDomWithIstd(newStyle, istd, newIstd);
        }

        /// <summary>
        /// New istd for a style can affect other styles, that rely on its old istd:
        /// - basedOnIstd
        /// - NextIstd
        /// - Linked istd
        /// check these cases and update
        /// </summary>
        private void UpdateStyles(int istd, int newIstd)
        {
            foreach (Style style in this)
            {
                if (style.BasedOnIstd == istd)
                    style.BasedOnIstd = newIstd;

                if (style.NextIstd == istd)
                    style.NextIstd = newIstd;

                if (style.LinkedIstd == istd)
                    style.LinkedIstd = newIstd;
            }
        }

        /// <summary>
        /// Updates/removes Istd for all nodes referred to style being removed/modified.
        /// Updates when newIstd != -1, otherwise removes istd from Pr bag.
        /// </summary>
        private void UpdateDomWithIstd(Style style, int istd, int newIstd)
        {
            switch (style.Type)
            {
                case StyleType.Character:
                    ProcessCharStyle(istd, newIstd);
                    break;
                case StyleType.Paragraph:
                    ProcessParaStyle(istd, newIstd);
                    break;
                case StyleType.List:
                    ProcessListStyle(style, istd, newIstd);
                    break;
                case StyleType.Table:
                    ProcessTableStyle(istd, newIstd);
                    break;
                default:
                    throw new InvalidOperationException("Unknown style type.");
            }
        }

        /// <summary>
        /// Adds a style. For internal use, called from the file readers, solves the problem
        /// with non-unique style names or style identifiers that sometimes occur in documents.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="aliases">Aliases of the style, can be null.</param>
        internal void AddForLoad(Style style, string[] aliases)
        {
            if (!IsValidStyleType(style))
            {
                // AM. Unfortunately TableNormal has been created already as Style object while we need TableStyle.
                // I think best solution here is to instantiate TableNormal style from template.
                // Also I think we can do the same for DefaultParagraphFont and Normal styles.
                GetBySti(style.StyleIdentifier, true);
                return;
            }

            //If a style with this name already exists, add a number suffix to make a unique style name.
            if (mStylesByName.ContainsKey(style.Name))
                style.SetNameCore(GetUniqueStyleName(style.Name));

            //If a style with this sti already exists, have to reset it to user style then.
            if (style.BuiltIn && (mBuiltInStylesBySti.ContainsKey((int)style.StyleIdentifier)))
                style.SetStyleIdentifier(StyleIdentifier.User);

            Add(style);

            //Add any style aliases
            if (aliases != null)
            {
                foreach (string alias in aliases)
                {
                    // WORDSNET-19960 Avoid Normal style name for other than Normal style.
                    if ((alias == NormalStyleName) && (style.StyleIdentifier != StyleIdentifier.Normal))
                        continue;

                    mStylesByName.Add(GetUniqueStyleName(alias), style);
                }

                mStyleAliasesCache = null;
            }
        }

        /// <summary>
        /// Adds an alias to the specified style. For internal use. The style must belong to this collection.
        /// </summary>
        internal void AddAlias(Style style, string alias)
        {
            Debug.Assert(StringUtil.HasChars(alias));
            Debug.Assert(style != null);
            Debug.Assert(style.Styles == this);

            mStylesByName.Add(GetUniqueStyleName(alias), style);
            mStyleAliasesCache = null;
        }

        /// <summary>
        /// Removes all aliases of the specified style. For internal use. The style must belong to this collection.
        /// </summary>
        internal void RemoveAliases(Style style)
        {
            Debug.Assert(style != null);
            Debug.Assert(style.Styles == this);

            // Remove all names that reference this style (including the style itself).
            RemoveFromByNameList(style);
            // Restore only the style itself.
            mStylesByName.Add(style.Name, style);

            mStyleAliasesCache = null;
        }

        /// <summary>
        /// Appends a suffix number to the name of the style to make the name unique.
        /// </summary>
        internal string GetUniqueStyleName(string name)
        {
            string uniqueName = name;
            int i = 0;
            while (mStylesByName.ContainsKey(uniqueName))
            {
                uniqueName = string.Format("{0}_{1}", name, i);
                i++;
            }
            return uniqueName;
        }

        /// <summary>
        /// Copies a style into this collection.
        /// </summary>
        /// <param name="style">Style to be copied.</param>
        /// <returns>Copied style ready for usage.</returns>
        /// <remarks>
        /// <para>Style to be copied can belong to the same document as well as to different document.</para>
        /// <para>Linked style is copied.</para>
        /// <para>This method does doesn't copy base styles.</para>
        /// <para>If collection already contains a style with the same name, then new name is
        /// automatically generated by adding "_number" suffix starting from 0 e.g. "Normal_0", "Heading 1_1" etc.
        /// Use <see cref="Style.Name"/> setter for changing the name of the imported style.</para>
        /// </remarks>
        public Style AddCopy(Style style)
        {
            return AddCopy(style, null);
        }

        /// <summary>
        /// Copies a style into this collection.
        /// </summary>
        /// <remarks>
        /// The <paramref name="context"/> can be <c>null</c>, if we are not in importing process.
        /// Otherwise, if importing in <see cref="ImportFormatMode.KeepDifferentStyles"/> mode,
        /// then direct formatting of the style will not be collapsed over destination attributes,
        /// as it happens in the usual way.
        /// </remarks>
        private Style AddCopy(Style style, ImportContext context)
        {
            if (style == null)
                throw new ArgumentException("Style can not be null.");

            Style copy = AddCopyCore(style, context); // copy style

            if (style.LinkedIstd != StyleIndex.Nil) // copy linked style
            {
                Style linkedStyle = style.Styles.GetByIstd(style.LinkedIstd, false);
                if (linkedStyle != null)
                {
                    Style linkedCopy = AddCopyCore(linkedStyle, context);
                    copy.LinkedIstd = linkedCopy.Istd;
                    linkedCopy.LinkedIstd = copy.Istd;
                }
                else
                {
                    copy.LinkedIstd = StyleIndex.Nil;
                }
            }

            return copy;
        }

        /// <summary>
        /// Checks that style has valid style type.
        /// </summary>
        private static bool IsValidStyleType(Style style)
        {
            switch (style.StyleIdentifier)
            {
                case StyleIdentifier.DefaultParagraphFont:
                    return style.Type == StyleType.Character;
                case StyleIdentifier.Normal:
                    return style.Type == StyleType.Paragraph;
                case StyleIdentifier.TableNormal:
                    return style.Type == StyleType.Table;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Copies a single style into this collection.
        /// </summary>
        private Style AddCopyCore(Style style, ImportContext context)
        {
            Style dstStyle = style.Clone();
            // if added style is not instantiated, retain the original name.
            dstStyle.SetNameCore(mStylesByName.ContainsKey(style.Name) ? GetUniqueStyleName(style.Name) : style.Name);

            StyleIdentifier sti = StyleIndex.BuiltInStyleNameEnglish(dstStyle.Name);
            bool istdChanged = false;
            if (sti != StyleIdentifier.User)
                istdChanged = StyleIndex.ConvertToBuiltInStyle(dstStyle, sti, null, false);
            else
                dstStyle.SetStyleIdentifier(StyleIdentifier.User);

            if (!istdChanged)
                dstStyle.SetIstd(GetNextFreeIstd()); // Update istd if we did not change it during ConvertToBuiltInStyle.

            // For simplicity we will only retain source styles for NextIstd and BasedOnIstd only if they are reserved.
            dstStyle.NextIstd = (StyleIndex.IsIstdReserved(style.NextIstd)) ? style.NextIstd : dstStyle.Istd;
            dstStyle.BasedOnIstd = (StyleIndex.IsIstdReserved(style.BasedOnIstd)) ? style.BasedOnIstd : StyleIndex.Nil;

            Add(dstStyle);

            // We should copy List style in styles of all types except CharacterStyle. DD: I'm not sure this if is 100% correct,
            int listId = (int)((IParaAttrSource)style).FetchParaAttr(ParaAttr.ListId);
            if ((style.Type != StyleType.Character) && (listId > 0))
            {
                List list = style.Document.Lists.FetchListByListIdResolveStyleReference(listId);
                List dstList = Document.Lists.AddCopy(list, false);
                dstStyle.ParaPr.ListId = dstList.ListId;

                foreach (ListLevel level in dstList.ListLevels) // correct references in list levels.
                    if (level.ParaStyleIstd == style.Istd)
                        level.ParaStyleIstd = dstStyle.Istd;
            }

            if ((dstStyle.HasRevisions) && (dstStyle.Document is Document))
                ((Document)dstStyle.Document).Revisions.AddStyleRevisions(dstStyle);

            // WORDSNET-20704 We should calculate styles difference at the very end,
            // after all related staff has been imported already.
            Document srcDoc = style.Document as Document;
            if ((srcDoc != null) && !srcDoc.Styles.IsBuiltinStylesTemplate)
                CopyStyleAttributes(style, dstStyle, context);

            return dstStyle;
        }

        private static void CopyStyleAttributes(Style srcStyle, Style dstStyle, ImportContext context)
        {
            switch (srcStyle.Type)
            {
                case StyleType.Paragraph:
                    CopyRunPr(srcStyle, dstStyle, context);
                    CopyParaPr(srcStyle, dstStyle, context);
                    break;
                case StyleType.Character:
                    CopyRunPr(srcStyle, dstStyle, context);
                    break;
                case StyleType.List:
                    // Do nothing.
                    break;
                case StyleType.Table:
                    CopyTableStyle((TableStyle)srcStyle, (TableStyle)dstStyle, context);
                    break;
                default:
                    throw new ArgumentException("Unknown style type");
            }
        }

        /// <summary>
        /// Expands paragraph formatting of source style considering inherited difference.
        /// </summary>
        private static void CopyParaPr(Style srcStyle, Style dstStyle, ImportContext context)
        {
            const ParaPrExpandFlags expandFlags = ParaPrExpandFlags.DocumentDefaults | ParaPrExpandFlags.GlobalDefaults;

            // IN. Actually, I think we should not expand here a corresponding ListLevel attributes, when style is numbered.
            // We always import a List itself along with such style. So, it can cause a problem if after import
            // we change properties at ListLevel (it will be overridden with old value, which will be set in style after expand).
            // But it looks as a very rare case, so lets postpone it for a while.
            ParaPr expParaPr = srcStyle.GetExpandedParaPr(expandFlags);
            ParaPr baseExpParaPr = dstStyle.GetExpandedParaPr(expandFlags | ParaPrExpandFlags.NoDirectFormatting);

            // WORDSNET-20704 When we expand destination style with flag 'NoDirectFormatting',
            // the corresponding direct list properties are not expanded as well.
            // So, expand them over the destination attributes forcibly.
            // WORDSNET-21615 When style has numbered style reference, the corresponding referenced style might
            // been not set to the style at this point yet. As we need just appropriate list level formatting to be
            // expanded and it should not be changed during import in any case, we can take it from the source style.
            if (srcStyle.ParaPr.ListId != 0)
                srcStyle.Document.Lists.ExpandDirectList(srcStyle.ParaPr, baseExpParaPr);

            // WORDSNET-23910 Don't collapse attributes that were present in direct formatting of source style
            // to not (possible) lose them. This is behavior by design in KeepDifferentStyles mode.
            // WORDSNET-23461 Also don't collapse attributes when 'ForceCopyStyles' option is enabled, by design.
            int[] keysToIgnore = (context != null) &&
                                 ((context.ImportFormatMode == ImportFormatMode.KeepDifferentStyles) ||
                                  context.ImportFormatOptions.ForceCopyStyles)
                ? srcStyle.ParaPr.GetKeys()
                : new int[] { };
            expParaPr.Collapse(baseExpParaPr, keysToIgnore);

            // Restore some attributes that were imported already or are not collapsible.
            // TODO IN We store 'istd' in paragraph and run properties along with the storing
            // it in a separate field of Style. It would be nice to get rid of storing it in
            // attribute collections, but a lot of golds are failed. See also details in Style.SetIstd().
            // WORDSNET-21296 It is possible that the style is not a list in the source document,
            // but one of its basic styles is a list in the destination document. Because of this,
            // the style also becomes a list in the destination document. In this case we should
            // set this style explicitly not a list item.
            if (expParaPr.IsExplicitlyNotListItem)
                dstStyle.ParaPr[ParaAttr.ListId] = 0;
            dstStyle.ParaPr.MirrorTo(expParaPr, new int[] { ParaAttr.Istd, ParaAttr.ListId, ParaAttr.RsidP });
            dstStyle.ParaPr = expParaPr;
        }

        /// <summary>
        /// Expands run formatting of source style considering inherited difference.
        /// </summary>
        private static void CopyRunPr(Style srcStyle, Style dstStyle, ImportContext context)
        {
            Theme srcTheme = srcStyle.Document.GetThemeInternal();
            Theme dstTheme = dstStyle.Document.GetThemeInternal();

            bool isThemeFontsDifferent = (!Theme.ThemeFontsEquals(srcTheme, dstTheme) && !(srcStyle.Document is GlossaryDocument) && !(dstStyle.Document is GlossaryDocument));

            bool isCharStyleLinkedInSrc =
                (srcStyle.Type == StyleType.Character) && (srcStyle.LinkedIstd != (int)StyleIdentifier.Nil);
            RunPrExpandFlags expandFlags = GetRunPrExpandFlags(dstStyle, isCharStyleLinkedInSrc);

            RunPr expRunPr = srcStyle.GetExpandedRunPr(expandFlags);

            if (isThemeFontsDifferent)
                Theme.Apply(srcTheme, expRunPr);

            // Word does not collapse attributes in 'pure' character styles.
            bool isCharacterStyle = (dstStyle.Type == StyleType.Character) &&
                    (dstStyle.LinkedIstd == StyleIndex.Nil) && !isCharStyleLinkedInSrc;
            if (!isCharacterStyle)
            {
                RunPr baseRunPr = dstStyle.GetExpandedRunPr(expandFlags | RunPrExpandFlags.NoDirectFormatting);

                // WORDSNET-17186 We should not apply Themes to a destination attributes
                // even Themes are different to mimic Word behavior.
                // WORDSNET-23910 Don't collapse attributes that were present in direct formatting of source style
                // to not (possible) lose them. This is behavior by design in KeepDifferentStyles mode.
                // WORDSNET-23461 Also don't collapse attributes when 'ForceCopyStyles' option is enabled, by design.
                int[] keysToIgnore = (context != null) &&
                                     ((context.ImportFormatMode == ImportFormatMode.KeepDifferentStyles) ||
                                      context.ImportFormatOptions.ForceCopyStyles)
                    ? srcStyle.RunPr.GetKeys()
                    : new int[] { };
                expRunPr.Collapse(baseRunPr, keysToIgnore);
            }

            // Restore non-collapsible attributes.
            // TODO IN We need to get rid of storing 'istd' in RunPr collection.
            // See more details in CopyParaPr().
            dstStyle.RunPr.MirrorTo(expRunPr, new int[] { FontAttr.Istd, FontAttr.RsidR, FontAttr.RsidRPr });
            dstStyle.RunPr = expRunPr;
        }

        /// <summary>
        /// Retrieves "RunPrExpandFlags" for specified source and destination styles.
        /// </summary>
        private static RunPrExpandFlags GetRunPrExpandFlags(Style dstStyle, bool isStyleLinkedInSource)
        {
            RunPrExpandFlags expandFlags = RunPrExpandFlags.DocumentDefaults | RunPrExpandFlags.GlobalDefaults;
            if ((dstStyle.Type != StyleType.Character) || (dstStyle.LinkedIstd != StyleIndex.Nil))
                return expandFlags;

            // Seems Word does not expand defaults for character styles and styles that have linked style.
            if (!isStyleLinkedInSource)
                expandFlags = RunPrExpandFlags.Normal;

            return expandFlags;
        }

        private static void CopyTableStyle(TableStyle srcTableStyle, TableStyle dstTableStyle, ImportContext context)
        {
            CopyRunPr(srcTableStyle, dstTableStyle, context);
            CopyParaPr(srcTableStyle, dstTableStyle, context);

            dstTableStyle.RowPr = srcTableStyle.GetExpandedRowPr();
            dstTableStyle.CellPr = srcTableStyle.GetExpandedCellPr();
            dstTableStyle.TablePr = srcTableStyle.GetExpandedTablePr();

            TableStyle baseStyle = dstTableStyle.GetBaseStyle() as TableStyle;
            if (baseStyle != null)
            {
                // WORDSNET-23910 Don't collapse attributes that were present in direct formatting of source style
                // to not (possible) lose them. This is behavior by design in KeepDifferentStyles mode.
                if ((context != null) && (context.ImportFormatMode == ImportFormatMode.KeepDifferentStyles))
                {
                    dstTableStyle.RowPr.Collapse(baseStyle.GetExpandedRowPr(), srcTableStyle.RowPr.GetKeys());
                    dstTableStyle.CellPr.Collapse(baseStyle.GetExpandedCellPr(), srcTableStyle.CellPr.GetKeys());
                    dstTableStyle.TablePr.Collapse(baseStyle.GetExpandedTablePr(), srcTableStyle.TablePr.GetKeys());
                }
                else
                {
                    dstTableStyle.RowPr.Collapse(baseStyle.GetExpandedRowPr());
                    dstTableStyle.CellPr.Collapse(baseStyle.GetExpandedCellPr());
                    dstTableStyle.TablePr.Collapse(baseStyle.GetExpandedTablePr());
                }
            }
        }

        /// <summary>
        /// Makes a deep copy of the styles. Suitable when copying a complete document.
        ///
        /// Note that list definitions that are referenced by the styles are not cloned.
        /// List definitions must be cloned separately using Lists.Clone.
        /// </summary>
        internal StyleCollection Clone(DocumentBase dstDoc)
        {
            Debug.Assert(dstDoc != mDoc);

            StyleCollection lhs = (StyleCollection)MemberwiseClone();

            lhs.mDoc = dstDoc;

            // RK It is safe to simply clone these attribute collections. Because we are copying
            // a complete document and styles and lists will have same ids in the new document.
            lhs.mDefaultRunPr = mDefaultRunPr.Clone();
            lhs.mDefaultParaPr = mDefaultParaPr.Clone();

            lhs.mStylesByIstd = new SortedIntegerListGeneric<Style>();
            lhs.mStylesByName = new StringListGeneric<Style>();
            lhs.mBuiltInStylesBySti = new SortedIntegerListGeneric<Style>();

            // Copy styles.
            for (int i = 0; i < mStylesByIstd.Count; i++)
            {
                Style srcStyle = mStylesByIstd.GetByIndex(i);
                lhs.Add(srcStyle.Clone());
            }

            // Copy aliases.
            foreach (KeyValuePair<string, Style> entry in mStylesByName)
            {
                string name = entry.Key;
                Style srcStyle = entry.Value;
                // All styles with their names have already been copied.
                // But if we come across a name that points to a style that does not equal
                // that style's name, it is an alias and we need to copy it.
                if (name != srcStyle.Name)
                {
                    Style dstStyle = lhs.GetByName(srcStyle.Name, false);
                    lhs.mStylesByName.Add(name, dstStyle);
                }
            }

            lhs.mLatentStyles = mLatentStyles.Clone();

            lhs.mStyleAliasesCache = null;

            lhs.mDefaultFont = null;
            lhs.mDefaultParagraphFormat = null;

            return lhs;
        }

        /// <summary>
        /// Returns comma separated list of style aliases optionally including style name.
        /// </summary>
        internal string GetAliases(Style style, bool isIncludeName)
        {
            if (mStyleAliasesCache == null)
                CacheAliases();

            string aliases = mStyleAliasesCache.GetValueOrNull(style);
            if (!StringUtil.HasChars(aliases))
                aliases = string.Empty;

            return isIncludeName
                ? StringUtil.AppendCommaSeparated(style.Name, aliases)
                : aliases;
        }

        /// <summary>
        /// Caches comma separated aliases for each style.
        /// </summary>
        private void CacheAliases()
        {
            mStyleAliasesCache = new Dictionary<Style, string>(mStylesByName.Count);

            for (int i = 0; i < mStylesByName.Count; i++)
            {
                // If found an entry that points to the same style and
                // it is different from the style name, add it as an alias.
                Style style = mStylesByName.GetByIndex(i);
                string name = mStylesByName.GetKey(i);
                if (style.Name != name)
                {
                    string result = mStyleAliasesCache.GetValueOrNull(style);
                    mStyleAliasesCache[style] = StringUtil.AppendCommaSeparated(result, name);
                }
            }
        }

        /// <summary>
        /// Gets style by istd, returns null if not found.
        /// </summary>
        internal Style GetByIstd(int istd, bool isAllowAutoCreate)
        {
            Style style = mStylesByIstd[istd];

            //If cannot find the style, maybe its a built in style, try to instantiate it on demand.
            if ((style == null) && (isAllowAutoCreate))
            {
                Style prototype = BuiltInStyles.GetByIstd(istd, false);
                if (prototype != null)
                    style = ImportStyle(prototype);
            }

            return style;
        }

        /// <summary>
        /// Gets style by name or alias, returns null if not found.
        /// </summary>
        internal Style GetByName(string name, bool isAllowAutoCreate)
        {
            // WORDSNET-1431 Allow empty style name as a document in this defect has one.
            ArgumentUtil.CheckNotNull(name, "name");

            Style style = (Style)mStylesByName[name];

            // If cannot find the style, maybe its a built in style, try to instantiate it on demand.
            if ((style == null) && (isAllowAutoCreate))
            {
                Style prototype = BuiltInStyles.GetByName(name, false);

                if (prototype == null)
                {
                    // In cases when MS Word version of the document is not specifies or higher/lower then requested style, lets
                    // try to get this style by name from another BuiltInStyles collections (2013, 2007 or 2003).
                    prototype = BuiltInStyles2013.GetByName(name, false);
                    if (prototype == null)
                        prototype = BuiltInStyles2007.GetByName(name, false);
                    if (prototype == null)
                        prototype = BuiltInStyles2003.GetByName(name, false);
                }

                // There are 3 character styles linked to built-in paragraph styles in BuiltInStyles2007 collection
                // and 40 linked character styles in BuiltInStyles2013 collection. Lets check and return only built-in styles.
                if ((prototype != null) && prototype.BuiltIn)
                    style = ImportStyle(prototype);
            }

            return style;
        }

        /// <summary>
        /// Gets a built in style by sti, returns null if not found.
        /// </summary>
        [JavaThrows(false)]
        internal Style GetBySti(StyleIdentifier sti, bool isAllowAutoCreate)
        {
            if (sti == StyleIdentifier.User)
                throw new ArgumentException("Cannot return user defined styles by style identifier.");

            Style style = mBuiltInStylesBySti[(int)sti];

            //If the style is not found in the collection, instantiate a built in style on demand.
            if ((style == null) && (isAllowAutoCreate))
            {
                Style prototype = GetBuiltInStylePrototype(sti);

                if (prototype != null)
                    style = ImportStyle(prototype);
            }

            return style;
        }

        /// <summary>
        /// Returns style prototype from a BuiltIn style collection.
        /// </summary>
        /// <remarks>
        /// In cases when MS Word version of the document is not specifies or higher/lower then requested style,
        /// tries to get this style from another BuiltInStyles collections (2013, 2007 or 2003).
        /// </remarks>
        private Style GetBuiltInStylePrototype(StyleIdentifier sti)
        {
            Style prototype = BuiltInStyles.GetBySti(sti, false);

            if (prototype == null)
            {
                prototype = BuiltInStyles2013.GetBySti(sti, false);
                if (prototype == null)
                    prototype = BuiltInStyles2007.GetBySti(sti, false);
                if (prototype == null)
                    prototype = BuiltInStyles2003.GetBySti(sti, false);
            }

            return prototype;
        }

        /// <summary>
        /// Get load format if it is not a GlossaryDocument.
        /// Need it to determine which BuildIn style collection (Word2003 or Word2007) to use.
        /// </summary>
        private LoadFormat GetLoadFormat()
        {
            // GlossaryDocument it is a part of DOCX document so we should return LoadFormat.Docx,
            // to use Word2007 BuildIn style collection.
            if (mDoc.NodeType == NodeType.GlossaryDocument)
                return LoadFormat.Docx;

            return ((Document)mDoc).OriginalLoadFormat;
        }

        /// <summary>
        /// Determines whether a style with specified <see cref="StyleIdentifier"/> is present in this style collection.
        /// </summary>
        internal bool Contains(StyleIdentifier styleIdentifier)
        {
            return mBuiltInStylesBySti.ContainsKey((int)styleIdentifier);
        }

        /// <summary>
        /// Fetches a style by istd.
        /// </summary>
        /// <param name="istd">The style to find.</param>
        /// <param name="defaultIstd">If the istd style is not found,
        /// returns the style specified by this parameter.
        /// Throws if the default style is not found.</param>
        internal Style FetchByIstd(int istd, int defaultIstd)
        {
            //Allow auto create by istd only makes sense if it is one of the fixed istds.
            Style style = GetByIstd(istd, (istd <= MaxFixedIstd));
            if (style != null)
                return style;

            //Allow auto create by istd only makes sense if it is one of the fixed istds.
            style = GetByIstd(defaultIstd, (defaultIstd <= MaxFixedIstd));
            if (style != null)
                return style;

            throw new InvalidOperationException("Cannot find a style with this istd.");
        }

        /// <summary>
        /// Gets style by name, attempts to instantiate built in styles, throws if cannot find or auto instantiate.
        /// </summary>
        internal Style FetchByName(string name)
        {
            Style style = this[name];
            if (style == null)
                throw new InvalidOperationException(string.Format("Cannot find style '{0}'.", name));
            return style;
        }

        /// <summary>
        /// Gets a style by style identifier, attempts to instantiate built in styles, throws if cannot find or auto instantiate.
        /// </summary>
        internal Style FetchBySti(StyleIdentifier sti)
        {
            Style style = this[sti];
            if (style == null)
                throw new InvalidOperationException("Cannot find a style with this style identifier.");
            return style;
        }

        /// <summary>
        /// Imports a style during style autocreation.
        /// </summary>
        internal Style ImportStyle(Style srcStyle)
        {
            ImportContext context = new ImportContext(srcStyle.Document, Document, ImportFormatMode.UseDestinationStyles);
            return ImportStyle(context, srcStyle);
        }

        /// <summary>
        /// Imports a style and adds it to this document. The exact action depends on the importFormatMode
        /// parameter. In either case imports all styles this style depends on and updates relevant istds.
        ///
        /// This method is too long, refactor.
        /// </summary>
        /// <param name="context">Current import data.</param>
        /// <param name="srcStyle">The style to import. If the style already belongs to this document, does nothing.</param>
        /// <returns>Returns the imported style that was added to this document.</returns>
        [JavaConvertCheckedExceptions]
        internal Style ImportStyle(ImportContext context, Style srcStyle)
        {
            if (srcStyle == null)
                throw new ArgumentNullException("srcStyle");

            // If the style belongs to this document already, just return it.
            if (srcStyle.Styles == this)
                return srcStyle;

            // WORDSNET-16135 Recurse to import base style.
            if (srcStyle.BasedOnIstd != StyleIndex.Nil)
            {
                int dstBasedOnIstd = GetDstBasedOnIstd(srcStyle, context);
                if (dstBasedOnIstd == StyleIndex.Nil)
                    ImportStyle(context, srcStyle.GetBaseStyle());
            }

            // If the style was already imported, return it.
            if (context.IsImported(srcStyle))
                return GetByIstd(context.ImportedIstds[srcStyle.Istd], false);

            // Start style import
            Style dstStyle;
            switch (context.ImportFormatMode)
            {
                case ImportFormatMode.UseDestinationStyles:
                case ImportFormatMode.KeepDifferentStyles:
                    dstStyle = ImportStyleUseDestinationStyles(context, srcStyle);
                    break;
                case ImportFormatMode.KeepSourceFormatting:
                    dstStyle = ImportStyleKeepSourceFormatting(context, srcStyle);
                    break;
                default:
                    throw new InvalidOperationException("Unknown import format action.");
            }

            return dstStyle;
        }

        /// <summary>
        /// If the source style is built in style, first tries to get a style by style identifier,
        /// if not found tries by name. For user defined styles just tries by name.
        /// Returns null if not found.
        /// </summary>
        internal Style FindLocaleIndependentMatch(Style srcStyle)
        {
            if (srcStyle.BuiltIn)
            {
                // WORDSNET-3658 For built in styles we must look by name if cannot find by style identifier
                // because a style with this name might still exist but with a different identifier.
                Style dstStyle = GetBySti(srcStyle.StyleIdentifier, false);
                if (dstStyle != null)
                    return dstStyle;
            }

            return GetByName(srcStyle.Name, false);
        }

        /// <summary>
        /// Returns style that matches a specified style and has the same type.
        /// </summary>
        /// <remarks>
        /// If the source style is built in style, first tries to get a style by style identifier.
        /// If not found, then tries find by name and aliases.
        /// </remarks>
        /// <returns>Style if found, otherwise null.</returns>
        internal Style FindLocaleIndependentMatchSameType(Style srcStyle)
        {
            if (srcStyle.BuiltIn)
            {
                // Search by style identifier.
                Style dstStyle = GetBySti(srcStyle.StyleIdentifier, false);
                if ((dstStyle != null) && (dstStyle.Type == srcStyle.Type))
                    return dstStyle;
            }

            return GetByNameSameType(srcStyle);
        }

        /// <summary>
        /// Finds style inside source document and imports it.
        /// KeepSourceFormatting means we must preserve formatting of source document.
        /// </summary>
        private Style ImportStyleKeepSourceFormatting(ImportContext context, Style srcStyle)
        {
            Style dstStyle;

            // Current implementation just recursively copies the style and all styles it depends on.
            if (StyleIndex.IsNonModifiable(srcStyle))
            {
                // Special handling for default paragraph font style and table normal because they cannot be modified in Microsoft Word.
                // These styles are almost always present so the normal scenario is just return the existing style from the destination document.
                // On rare occasions the document does not have this style yet, in this case the code below just imports the style normally.
                dstStyle = GetBySti(srcStyle.StyleIdentifier, false);
                if (dstStyle != null)
                    return dstStyle;
            }

            // Start the importing business.
            dstStyle = srcStyle.Clone();

            if (context.IsThemeFontsDifferent)
                Theme.Apply(context.SrcDoc.GetThemeInternal(), dstStyle.RunPr);
            if (context.IsThemeColorsDifferent)
                ThemeColorUpdater.Update(dstStyle, context.DstDoc.GetThemeInternal());

            if (FindLocaleIndependentMatch(srcStyle) == null)
            {
                // No match is found, no need to change name and sti.
                // Only need to change istd if it is not a fixed one.
                if (srcStyle.Istd > MaxFixedIstd)
                    dstStyle.SetIstd(GetNextFreeIstd());
            }
            else
            {
                // A match is found (either by name or by sti), have to change name, istd and sti to be valid in the destination.
                dstStyle.SetNameCore(GetUniqueStyleName(srcStyle.Name));
                dstStyle.SetIstd(GetNextFreeIstd());
                dstStyle.SetStyleIdentifier(StyleIdentifier.User);
            }

            Add(dstStyle);
            context.ImportedIstds[srcStyle.Istd] = dstStyle.Istd;

            // WORDSNET-24010 Reset all related styles of the importing style.
            dstStyle.BasedOnIstd = StyleIndex.Nil;
            dstStyle.NextIstd = StyleIndex.Nil;
            dstStyle.LinkedIstd = StyleIndex.Nil;

            // And now import all related styles.
            ImportRelatedStyles(srcStyle, dstStyle, context);

            if (!context.SrcStyles.IsBuiltinStylesTemplate)
                CalculateDifference(srcStyle, dstStyle, context);

            return dstStyle;
        }

        /// <summary>
        /// Finds style inside destination document or inside template and imports it.
        /// </summary>
        private Style ImportStyleUseDestinationStyles(ImportContext context, Style srcStyle)
        {
            Style dstStyle = FindLocaleIndependentMatch(srcStyle);

            // Style with such name or sti already exists.
            if (dstStyle != null)
            {
                // We have two options here: UseDestinationStyles and KeepDifferentStyles.
                // If it is UseDestinationStyles, then we just use existing style.
                if (context.ImportFormatMode == ImportFormatMode.UseDestinationStyles)
                    return dstStyle;

                // Otherwise, we should check first if there is equal style in this collection to reuse it.
                Style dstStyleImported = ImportStyleKeepSourceFormatting(context, srcStyle);
                // WORDSNET-21485 Don't search equal styles when 'KeepSourceNumbering' option is enabled
                // in order to always use the imported style.
                if (context.ImportFormatOptions.KeepSourceNumbering)
                    return dstStyleImported;

                dstStyle = FindEqualStyle(dstStyleImported);
                // There is no just imported style in destination yet, so let's use it.
                if (dstStyle == null)
                    return dstStyleImported;

                // Otherwise remove imported copy and return existing style as the result.
                dstStyleImported.Remove();

                // Replace imported style with existing style in cache.
                context.ImportedIstds[srcStyle.Istd] = dstStyle.Istd;

                // WORDSNET-17179 Update also linked style.
                if (srcStyle.LinkedIstd != StyleIndex.Nil)
                    context.ImportedIstds[srcStyle.LinkedIstd] = dstStyle.LinkedIstd;

                return dstStyle;
            }

            return ImportStyleKeepSourceFormatting(context, srcStyle);
        }

        /// <summary>
        /// Add the imported style to the document and to the cache.
        /// </summary>
        private void ImportRelatedStyles(Style srcStyle, Style dstStyle, ImportContext context)
        {
            if (srcStyle.BasedOnIstd != StyleIndex.Nil)
            {
                // WORDSNET-16135 Set proper base style.
                // Note, at this point all base styles should be already either imported or exist in destination collection.
                int dstBasedOnIstd = GetDstBasedOnIstd(srcStyle, context);
                Debug.Assert(dstBasedOnIstd != StyleIndex.Nil, string.Format("The base style for '{0}' must be already imported.", dstStyle.Name));
                dstStyle.BasedOnIstd = dstBasedOnIstd;
            }

            // Perform style type related import.
            if ((srcStyle.Type == StyleType.List) || (srcStyle.Type == StyleType.Paragraph))
                ImportListDefinition(context, srcStyle, dstStyle);

            // Recurse to import next paragraph style.
            if (srcStyle.NextIstd != StyleIndex.Nil)
                dstStyle.NextIstd = ImportStyle(context, srcStyle.GetNextStyle()).Istd;

            // Recurse to import linked style.
            if (srcStyle.LinkedIstd != StyleIndex.Nil)
                dstStyle.LinkedIstd = ImportStyle(context, srcStyle.GetLinkedStyle()).Istd;
        }

        /// <summary>
        /// Called during import of paragraph or list styles to import the list definition
        /// referenced by the style (if any).
        /// </summary>
        private static void ImportListDefinition(ImportContext context, Style srcStyle, Style dstStyle)
        {
            Debug.Assert((srcStyle.Type == StyleType.Paragraph) || (srcStyle.Type == StyleType.List));

            if (srcStyle.ParaPr.ListId == 0)
                return;

            // Import the list definitions.
            int srcListId = srcStyle.ParaPr.ListId;
            int dstListId = context.DstLists.ImportList(context, srcListId);
            dstStyle.ParaPr.ListId = dstListId;

            // For a list style, complete the bidirectional association between
            // the list style and list definition. Store the imported list style istd
            // in the list definition.
            if (dstStyle.Type == StyleType.List)
                dstStyle.List.ListDef.ListStyleIstd = dstStyle.Istd;
        }

        /// <summary>
        /// Ensures that we have Normal and DefaultParagraphFont styles.
        /// These are needed for DOCX and WordML export to work properly.
        /// If we don't have these styles, they could end up being auto created after
        /// the styles were written to XML already.
        /// </summary>
        internal void EnsureMinimum()
        {
            EnsureReservedValid(StyleIdentifier.Normal, "Normal");
            EnsureReservedValid(StyleIdentifier.DefaultParagraphFont, "Default Paragraph Font");
        }

        /// <summary>
        /// Corrects the style name case so that it matches the reference "Normal".
        /// Here MS Word behavior in the similar case is copied.
        /// </summary>
        /// <dev>
        /// It is possible to expand the method in the future (for example, to "Default Paragraph Font" and "Table normal" styles)
        /// if the necessity in this becomes clear.
        /// </dev>
        internal void FixUpCaseIssue()
        {
            Style normalStyle = GetByName("Normal", false);
            if ((normalStyle != null) || (GetBySti(StyleIdentifier.Normal, false) != null))
                return;

            foreach (KeyValuePair<string, Style> entry in mStylesByName)
            {
                string name = entry.Key;
                Style style = entry.Value;
                if ((style.BasedOnIstd == (int)StyleIdentifier.Nil) && (style.BaseStyleName == string.Empty) &&
                    name.ToUpper(CultureInfo.InvariantCulture).Equals("NORMAL", StringComparison.Ordinal))
                {
                    normalStyle = entry.Value;
                    break;
                }
            }

            if (normalStyle != null)
            {
                normalStyle.SetNameCore("Normal", true);
                normalStyle.SetStyleIdentifier(StyleIdentifier.Normal, true);
            }
        }

        /// <summary>
        /// Fixes any problems with the based on style identifiers. Call this from <see cref="DocumentPostLoader"/>.
        /// </summary>
        internal void FixUpBasedOnStyles()
        {
            // Copy to temp array at first because fixing might create new styles and invalidate iterator.
            List<Style> temp = new List<Style>();
            foreach (Style style in this)
                temp.Add(style);

            // It is better to do this separately from fixing circular references, because checking
            // for circular references will traverse the based on chains and might encounter a missing style
            // and I don't want to mix the fixing of two different problems.
            foreach (Style style in temp)
                style.FixUpBasedOnMissing();

            foreach (Style style in temp)
                style.FixUpBasedOnCircularReferences();
        }

        /// <summary>
        /// Removes all locale id attributes from all styles in the collection.
        /// This method is actually a hack, needed while we have Blank.doc AllStyles.doc embedded resources as a DOC files.
        /// If we change them to DOCX or WML later, then we will be able to edit them to remove the unneeded language settings.
        /// </summary>
        internal void RemoveLocaleIdsFromStyles()
        {
            foreach (Style style in this)
            {
                RunPr runPr = style.RunPr;
                runPr.Remove(FontAttr.LocaleId);
                runPr.Remove(FontAttr.LocaleIdBi);
                runPr.Remove(FontAttr.LocaleIdFarEast);
            }
        }

        /// <summary>
        /// Updates stylesheet from given template.
        /// </summary>
        internal void UpdateFromTemplate(Document template)
        {
            ImportContext context = new ImportContext(template, Document, ImportFormatMode.UseDestinationStyles);

            bool themesEqual = Theme.ThemeFontsEquals(mDoc.GetThemeInternal(), template.GetThemeInternal());

            foreach (Style dstStyle in Document.Styles)
            {
                StyleCollection styles = template.Styles;
                Style srcStyle = styles.FindLocaleIndependentMatch(dstStyle);
                if (srcStyle == null)
                    continue;

                // It seems that Word updates Normal in special way.
                if (dstStyle.StyleIdentifier == StyleIdentifier.Normal)
                {
                    dstStyle.RunPr = styles.DefaultRunPr.Clone();
                    dstStyle.ParaPr = styles.DefaultParaPr.Clone();

                    srcStyle.RunPr.ExpandTo(dstStyle.RunPr);
                    srcStyle.ParaPr.ExpandTo(dstStyle.ParaPr);
                }
                else
                {
                    dstStyle.RunPr = srcStyle.RunPr.Clone();
                    dstStyle.ParaPr = srcStyle.ParaPr.Clone();

                    if (dstStyle.ParaPr.ListId != 0)
                        dstStyle.ParaPr.ListId = context.DstLists.ImportList(context, srcStyle.ParaPr.ListId);

                    TableStyle dstTableStyle = dstStyle as TableStyle;
                    TableStyle srcTableStyle = srcStyle as TableStyle;

                    // Copy data related to table styles.
                    if (dstTableStyle != null && srcTableStyle != null)
                    {
                        dstTableStyle.TablePr = srcTableStyle.TablePr.Clone();
                        dstTableStyle.ClearConditionalStyles();

                        foreach (ConditionalStyle conditionalStyle in srcTableStyle.ConditionalStyles.DefinedStyles)
                            dstTableStyle.AddConditionalStyle(conditionalStyle.Clone());
                    }
                }

                if (!themesEqual)
                    Theme.Apply(template.GetThemeInternal(), dstStyle.RunPr);
            }
        }

        /// <summary>
        /// Copies source style with its Base, Linked and Next styles to this collection.
        /// </summary>
        internal int CopyStyle(Style srcStyle, ImportContext context)
        {
            // First look at already copied styles.
            int copiedIstd = context.ImportedIstds[srcStyle.Istd];
            if (!IntToIntDictionary.IsNullSubstitute(copiedIstd))
                return copiedIstd;

            // Copy style properties and add this style to the styles collection.
            Style dstStyle = CopyStyleCore(srcStyle, context);
            if (dstStyle == null)
                return StyleIndex.Nil;

            // WORDSNET-24640 Reset all related styles before copying.
            dstStyle.BasedOnIstd = StyleIndex.Nil;
            dstStyle.NextIstd = StyleIndex.Nil;
            dstStyle.LinkedIstd = StyleIndex.Nil;

            // Process base style.
            if (srcStyle.BasedOnIstd != StyleIndex.Nil)
                dstStyle.BasedOnIstd = CopyStyle(srcStyle.GetBaseStyle(), context);

            // Process linked style.
            if (srcStyle.LinkedIstd != StyleIndex.Nil)
                dstStyle.LinkedIstd = CopyStyle(srcStyle.GetLinkedStyle(), context);

            // Process next style.
            if (srcStyle.NextIstd != StyleIndex.Nil)
                dstStyle.NextIstd = CopyStyle(srcStyle.GetNextStyle(), context);

            // Process numbering.
            if ((srcStyle.Type == StyleType.List) || (srcStyle.Type == StyleType.Paragraph))
            {
                int srcListId = srcStyle.ParaPr.ListId;
                if (srcListId != 0)
                    dstStyle.ParaPr[ParaAttr.ListId] = Document.Lists.CopyList(srcListId, context);
            }

            return dstStyle.Istd;
        }

        /// <summary>
        /// Checks that style with given StyleIdentifier has appropriate StyleIndex.
        /// In case of it's not true or style is missing creates it from template.
        /// </summary>
        private void EnsureReservedValid(StyleIdentifier sti, string styleName)
        {
            Style style = GetByName(styleName, false);
            if ((style != null) && (style.StyleIdentifier != sti))
            {
                // This means that we have user style with predefined name. Rename it and continue.
                string newName = GetUniqueStyleName(styleName);
                style.SetNameCore(newName, true);
            }

            int istd = StyleIndex.StyleIdentifierToIstd(sti);

            style = GetByIstd(istd, true);
            if (style.StyleIdentifier != sti)
            {
                // Change style's Istd and update Model.
                style.SetIstd(GetNextFreeIstd(), true);

                // Create style from template.
                GetBySti(sti, true);
            }
        }

        /// <summary>
        /// Checks if styles has zero font size and corrects this.
        /// </summary>
        private static void RemoveZeroFontSize(RunPr runPr, int key)
        {
            if (runPr.Contains(key))
            {
                int value = (int)runPr[key];
                if (value == 0)
                    runPr.Remove(key);
            }
        }

        /// <summary>
        /// Finds style equal to the specified style.
        /// </summary>
        private Style FindEqualStyle(Style srcStyle)
        {
            foreach (Style style in this)
            {
                // WORDSNET-14587 When import with KeepDifferentStyles mode, we first import style to calculate
                // formatting difference and import related styles properly. Then we need to search for another equal style.
                // So, skip this style itself.
                if (ReferenceEquals(style, srcStyle))
                    continue;

                if (srcStyle.Equals(style))
                    return style;
            }

            return null;
        }

        /// <summary>
        /// Copies expanded attributes from the source style to
        /// the destination style and collapses them with expanded
        /// base attributes in destination collection. After that removes
        /// remained extra global defaults from the destination style.
        /// </summary>
        /// <remarks>See details in TestJira14588()</remarks>
        private void CalculateDifference(Style srcStyle, Style dstStyle, ImportContext context)
        {
            // Word does not modify 'DefaultParagraphFont' and 'TableNormal' styles.
            if (StyleIndex.IsNonModifiable(dstStyle))
                return;

            // Word resets base style to one of the 'TableNormal', 'Normal', 'DefaultParagraphFont',
            // depending on the style type.
            // WORDSNET-15319 First of all we have to reset base style and only then do collapse in CopyStyleAttributes() below.
            Style srcBaseStyle = srcStyle.GetBaseStyle();
            if (srcBaseStyle != null)
            {
                // If destination collection contains appropriate style,
                // Word uses it as a new base style.
                Style dstBaseStyle = FindLocaleIndependentMatch(srcBaseStyle);
                dstStyle.BasedOnIstd = (dstBaseStyle != null) ? dstBaseStyle.Istd : GetDefaultStyleIndex(dstStyle.Type);
            }

            CopyStyleAttributes(srcStyle, dstStyle, context);
        }

        /// <summary>
        /// Gets corresponding BasedOnIstd for base style of the specified style in destination collection.
        /// </summary>
        /// <remarks>If there is no appropriate base style in destination collection, then returns StyleIndex.Nil.</remarks>
        private int GetDstBasedOnIstd(Style srcStyle, ImportContext context)
        {
            Debug.Assert(srcStyle.BasedOnIstd != StyleIndex.Nil);

            Style srcBaseStyle = srcStyle.GetBaseStyle();

            // First try to find style among imported styles.
            int dstBasedOnIstd = context.ImportedIstds[srcBaseStyle.Istd];
            // If base style was not imported, then try to find same style in destination collection among existing styles.
            if (dstBasedOnIstd == int.MinValue)
            {
                Style dstBaseStyle;
                if (StyleIndex.IsNonModifiable(srcBaseStyle))
                {
                    dstBaseStyle = GetBySti(srcBaseStyle.StyleIdentifier, false);
                }
                else
                {
                    dstBaseStyle = FindLocaleIndependentMatch(srcBaseStyle);
                    if ((dstBaseStyle == null) && (context.ImportFormatMode == ImportFormatMode.KeepDifferentStyles))
                        dstBaseStyle = FindEqualStyle(srcBaseStyle);
                }

                if (dstBaseStyle != null)
                    dstBasedOnIstd = dstBaseStyle.Istd;
            }

            return IntToIntDictionary.IsNullSubstitute(dstBasedOnIstd) ? StyleIndex.Nil : dstBasedOnIstd;
        }

        /// <summary>
        /// Copies all styles from the source collection to this collection.
        /// </summary>
        private void CopyAll(StyleCollection srcStyles, ImportContext context)
        {
            foreach (Style srcStyle in srcStyles)
                CopyStyle(srcStyle, context);
        }

        /// <summary>
        /// Copies source style to this collection.
        /// </summary>
        private Style CopyStyleCore(Style srcStyle, ImportContext context)
        {
            Style dstStyle = FindLocaleIndependentMatch(srcStyle);

            while ((dstStyle != null) && (dstStyle.Type != srcStyle.Type))
            {
                // We here because source and destination styles have the different types.
                // But there can be a situation when destination style has aliases and some another source style with an
                // appropriate type will match this style. In that case such style will override destination style and then
                // current problematic style can be copied too. So, the easiest way here is just to update name and aliases of
                // the destination style with inappropriate type from the source style that will have the same style and
                // continue copying the problematic style.
                // Another way is to store such problematic styles in some collection of deferred styles and after all styles
                // are imported try to import them again. But such approach is much more complex. For example,
                // it will require to update all styles and lists with new imported istds.
                Style srcStyleSameType = srcStyle.Styles.FindLocaleIndependentMatchSameType(dstStyle);
                if (srcStyleSameType != null)
                {
                    UpdateNameAndAliases(dstStyle, srcStyleSameType);
                }
                else
                {
                    // Mimic Word and do not copy style that matches to a destination style with a different type.
                    WarningUtil.WarnDataLoss(Document.WarningCallback, WarningSource.Unknown, WarningStrings.CannotCopyStyle,
                        srcStyle.Name);

                    context.ImportedIstds[srcStyle.Istd] = StyleIndex.Nil;
                    return null;
                }

                dstStyle = FindLocaleIndependentMatch(srcStyle);
            }

            if (dstStyle != null)
            {
                CloneFormatting(srcStyle, dstStyle);
            }
            else
            {
                dstStyle = srcStyle.Clone();

                if (mStylesByIstd.ContainsKey(dstStyle.Istd))
                {
                    // Destination collection already contains the same istd, so get new one.
                    dstStyle.SetIstd(GetNextFreeIstd());
                }

                Add(dstStyle);
            }

            context.ImportedIstds[srcStyle.Istd] = dstStyle.Istd;
            return dstStyle;
        }

        /// <summary>
        /// Clones formatting from source to destination style.
        /// </summary>
        private static void CloneFormatting(Style srcStyle, Style dstStyle)
        {
            Debug.Assert(srcStyle.Type == dstStyle.Type);
            dstStyle.ClearCaches();
            UpdateNameAndAliases(srcStyle, dstStyle);
            dstStyle.CopyGenerics(srcStyle);

            dstStyle.RunPr = srcStyle.RunPr.Clone();
            if (dstStyle.Type == StyleType.Character)
                return;

            dstStyle.ParaPr = srcStyle.ParaPr.Clone();

            if (dstStyle.Type == StyleType.Table)
                TableStyle.CloneProperties((TableStyle)srcStyle, (TableStyle)dstStyle);
        }

        /// <summary>
        /// Updates name and all aliases from the source style.
        /// </summary>
        private static void UpdateNameAndAliases(Style srcStyle, Style dstStyle)
        {
            StyleCollection dstStyles = dstStyle.Styles;

            // Remove name and all aliases of the destination style.
            dstStyles.RemoveFromByNameList(dstStyle);

            // Restore style name from the source style.
            dstStyles.mStylesByName[srcStyle.Name] = dstStyle;

            // Add style aliases.
            foreach (string alias in srcStyle.Aliases)
                dstStyles.mStylesByName[alias] = dstStyle;

            // Update aliases cache.
            // WORDSNET-18574 Don't update cache if it does not exist.
            if (dstStyles.mStyleAliasesCache != null)
            {
                if (srcStyle.Styles.mStyleAliasesCache.ContainsKey(srcStyle))
                    dstStyles.mStyleAliasesCache[dstStyle] = srcStyle.Styles.mStyleAliasesCache[srcStyle];
                else
                    dstStyles.mStyleAliasesCache.Remove(dstStyle);
            }
        }

        /// <summary>
        /// Gets style of the same type as a specified style by name or alias, returns null if not found.
        /// </summary>
        /// <remarks>Checks also all aliases of a source style.</remarks>
        private Style GetByNameSameType(Style srcStyle)
        {
            for (int i = 0; i < mStylesByName.Count; i++)
            {
                string styleName = mStylesByName.GetKey(i);
                if ((styleName == srcStyle.Name) || ArrayUtil.Contains(srcStyle.Aliases, styleName))
                {
                    Style dstStyle = mStylesByName.GetByIndex(i);
                    if (dstStyle.Type == srcStyle.Type)
                        return dstStyle;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if StyleIdentifier.BalloonText has zero size and corrects it. See WORDSNET-9442
        /// </summary>
        private void FixBalloonStyles()
        {
            Style style = GetBySti(StyleIdentifier.BalloonText, false);

            if (style == null)
                return;

            RemoveZeroFontSize(style.RunPr, FontAttr.Size);
            RemoveZeroFontSize(style.RunPr, FontAttr.SizeBi);

            // Also look for linked BalloonTextChar style if exists.
            style = GetByIstd(style.LinkedIstd, false);

            if (style == null)
                return;

            RemoveZeroFontSize(style.RunPr, FontAttr.Size);
            RemoveZeroFontSize(style.RunPr, FontAttr.SizeBi);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDoc;
        private RunPr mDefaultRunPr = new RunPr();
        private ParaPr mDefaultParaPr = new ParaPr();
        /// <summary>
        /// A map of styles sorted by istd.
        /// </summary>
        private SortedIntegerListGeneric<Style> mStylesByIstd = new SortedIntegerListGeneric<Style>();

        /// <summary>
        /// A map of styles names to styles. Can have several names (aliases) referring to a single style.
        /// </summary>
        /// <remarks>
        /// WORDSNET-26754 Before the fix this was sorted by names list <see cref="SortedIntegerListGeneric{T}"/>.
        /// </remarks>
        private StringListGeneric<Style> mStylesByName = new StringListGeneric<Style>();

        /// <summary>
        /// The helper class to sort <see cref="StyleCollection.mStylesByName"/> collection by name.
        /// </summary>
        private class ComparerByName : IComparer<Style>
        {
            public int Compare(Style x, Style y)
            {
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// A map of styles sorted by style identifier. Note it forces key to be int.
        /// If the key is enum, it becomes memory and performance sucker since .NET starts to use reflection.
        /// </summary>
        private SortedIntegerListGeneric<Style> mBuiltInStylesBySti = new SortedIntegerListGeneric<Style>();
        private LatentStyles mLatentStyles = new LatentStyles();

        /// <summary>
        /// Do not access this variable directly. Access it via the corresponding property.
        /// </summary>
        private static Document gAllStylesCache2003;
        private static readonly object gAllStylesSyncRoot2003 = new object();
        private static Document gAllStylesCache2007;
        private static readonly object gAllStylesSyncRoot2007 = new object();
        private static Document gAllStylesCache2013;
        private static readonly object gAllStylesSyncRoot2013 = new object();

        private Font mDefaultFont;
        private ParagraphFormat mDefaultParagraphFormat;

        /// <summary>
        /// Cached map style to comma separated list of aliases.
        /// </summary>
        private Dictionary<Style, string> mStyleAliasesCache;

        /// <summary>
        /// Gets the number of reserved istds.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FixedIstdCount = 15;
        /// <summary>
        /// Gets the maximum reserved istd.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxFixedIstd = FixedIstdCount - 1;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxKnownSti2003 = 156;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxKnownSti2007 = 267;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int VersionOfBuiltInNames2007 = 7;

        internal const string NormalStyleName = "Normal";
    }
}
