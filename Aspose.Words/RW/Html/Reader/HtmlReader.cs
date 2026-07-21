// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.Html.Reader.CommonBorder;
using Aspose.Words.RW.HtmlCommon;
using Aspose.Words.Tables;
using Color = System.Drawing.Color;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Imports HTML file into a Document object.
    /// </summary>
    internal class HtmlReader : IHtmlNodeProcessor
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="settings">HTML reader settings.</param>
        /// <param name="loadFormat">Format of the document that this HTML belongs to.</param>
        /// <param name="resourceLoader">Resource loader.</param>
        internal HtmlReader(
            HtmlReaderSettings settings,
            LoadFormat loadFormat,
            HtmlResourceLoader resourceLoader)
            : this(settings, loadFormat, resourceLoader, null)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="settings">HTML reader settings.</param>
        /// <param name="loadFormat">Format of the document that this HTML belongs to.</param>
        /// <param name="resourceLoader">Resource loader.</param>
        /// <param name="bookmarkNames">Existing bookmark names to pass to the hyperlink resolver.
        /// Can be <b>null</b>. Bookmark names are expected to be in lower case.</param>
        internal HtmlReader(
            HtmlReaderSettings settings,
            LoadFormat loadFormat,
            HtmlResourceLoader resourceLoader,
            HashSetGeneric<string> bookmarkNames)
        {
            Debug.Assert(settings != null);
            Debug.Assert(resourceLoader != null);
            Debug.Assert((loadFormat == LoadFormat.Html) ||
                (loadFormat == LoadFormat.Mhtml) ||
                (loadFormat == LoadFormat.Chm) ||
                (loadFormat == LoadFormat.Epub));

            mLoadFormat = loadFormat;
            mResourceLoader = resourceLoader;

            mTableNestingLevel = 0;

            // Note that the following two modules implement alternative ways of preserving borders and margins of block-level
            // elements.
            mCommonBorderResolver = new HtmlCommonBorderResolver(settings.UseHtmlBlocks);
            mHtmlBlockReader = new HtmlBlockReader(!settings.UseHtmlBlocks, settings.ApplyFormattingAsMsWord);

            mComments = new IntToObjDictionary<Comment>();
            mCommentRangeEnds = new IntToObjDictionary<CommentRangeEnd>();
            mFullyProcessedComments = new List<int>();
            mFootnotes = new IntToObjDictionary<Footnote>();
            mEndnotes = new IntToObjDictionary<Footnote>();
            mFieldNodesStack = new Stack<FieldChar>();

            // Note that by default we don't support IE conditional comments (the features field is null), because they
            // are not supported by modern browsers. However, this feature must be supported if we want to correctly process
            // VML images.
            if (settings.SupportVml)
            {
                mSupportedFeatures = new Features();
                mSupportedFeatures.Add(new Feature("vml", 1, 0));
                // VML shapes exported by MS Word can contain some parts (for example, tables) marked with <!if !mso>
                // and intended for aligning content in browsers only. MS Word itself omits these parts on import, and so do we.
                mSupportedFeatures.Add(new Feature("mso", 1, 0));
            }
            mSettings = settings;

            mListReaderManager = new HtmlListReaderManager();
            mHtmlHyperlinkResolver = new HtmlHyperlinkResolver(settings.HyperlinkProcessor, bookmarkNames);
            mFixedPageFormatDetector = new HtmlFixedPageDetector();
        }

        /// <summary>
        /// Reads HTML document from stream into the model.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding">Can be null, it will trigger autodetection.</param>
        /// <param name="doc">Document</param>
        /// <param name="reuseExistingBookmarks">
        /// Indicates whether to re-use bookmarks that already exist in the target document.
        /// </param>
        internal void Read(
            Stream stream,
            Encoding encoding,
            Document doc,
            bool reuseExistingBookmarks)
        {
            Read(stream, encoding, new DocumentBuilder(doc), reuseExistingBookmarks);
        }

        /// <summary>
        /// Reads HTML document from stream into the model associated with builder.
        /// Used to concatenate multiple documents in MHTML.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding">Can be null, it will trigger autodetection.</param>
        /// <param name="builder">Document builder.</param>
        /// <param name="reuseExistingBookmarks">
        /// Indicates whether to re-use bookmarks that already exist in the target document.
        /// </param>
        internal void Read(
            Stream stream,
            Encoding encoding,
            DocumentBuilder builder,
            bool reuseExistingBookmarks)
        {
            // Remember the current section. It is used later to limit the number of sections we need to process while removing
            // trailing empty paragraphs.
            Section startSection = builder.CurrentSection;

            // WORDSNET-20364 Set MS Word version for document if it is not altChunk.
            // AltChunk document copies MS Word version from original document.
            // Note that here we rely on the fact that the "apply formatting as MS Word" flag is only set when we
            // load a HTML altChunk in another document and this flag is cleared in all other scenarios. If this behavior of
            // that flag changes at some point in the future, we'll need to use other method of checking whether we're loading
            // a HTML altChunk.
            if (!mSettings.ApplyFormattingAsMsWord)
                builder.Document.CompatibilityOptions.OptimizeFor(mSettings.MswVersion);

            // WORDSNET-13221 SPEED. Accessing the external font cache often implies enumerating all system fonts, which
            // is a lengthy operation (especially if font files are not in the in-memory file cache). However, we don't need
            // information from the font cache until we start processing the HTML tree and we can do two lengthy operations
            // in parallel: enumerate system fonts and prepare the HTML tree (parse HTML and collect CSS rules).
            builder.Document.EffectiveFontSettings.PreloadFontsInBackground();

            // In certain scenarios (for example, during import of MHTML document parts) this method is used to insert
            // an HTML part into a document. In these scenarios we must preserve font formatting after insertion of HTML part.
            builder.PushRunPr();

            // Detect and correct the encoding of the HTML document.
            encoding = HtmlEncodingDetector.Detect(encoding, stream);

            // If the document encoding is ISO-8859-8 (Hebrew visually ordered text),
            // we assume that all text of this document is in visual order.
            bool textIsVisuallyOrdered = StringUtil.EqualsIgnoreCase(encoding.WebName, "iso-8859-8");

            // Note that when we import formatting as MS Word we disable certain default CSS (user-agent stylesheet) rules,
            // because in MS Word they are applied via styles.
            Init(
                builder,
                true,
                mSettings.ApplyFormattingAsMsWord,
                textIsVisuallyOrdered);

            HtmlDocument srcDoc = BuildHtmlTree(stream, encoding);

            ProcessCssRules(srcDoc);

            mHtmlHyperlinkResolver.Init(builder, mBidiTextArranger, reuseExistingBookmarks);

            mHtmlBlockReader.Init(builder);

            ProcessHtmlTree(srcDoc.Root, true);

            mHtmlHyperlinkResolver.ProcessBookmarksAndHyperlinks(mSettings.KeepUnreferencedIdBookmarks);

            // WORDSNET-19233 Remove empty paragraphs at the end of sections.
            // WORDSNET-27419 SPEED We used to iterate through all sections of the document in the process but it is faster to
            // check only the sections that were imported from HTML. This may increase performance in scenarios where
            // a relatively small HTML document is appended to a huge resulting document (which is the case when loading CHM or
            // EPUB with a lot of topics and concatenating them into a single document).
            RemoveEmptyParagraphs(startSection);

            // End of the document ends the last sequence of inline elements.
            mBidiTextArranger.RearrangeAndWriteText();

            // Restore saved font formatting. Also see the comment above where font formatting is saved.
            builder.PopRunPr();

            mParagraphArranger.DebugAssertHasNoRememberedParagraphs();

            Debug.Assert(mApplyDocumentWideFormatting);
            mStyleResolver.PostUpdateDocumentWideFormatting();

            WarnIfDocumentIsFixedPageHtml();
        }

        /// <summary>
        /// Reads HTML document from string and inserts at current position of builder.
        /// </summary>
        /// <remarks>
        /// This overloaded method is designed to insert an HTML fragment into an existing document, not to load a complete
        /// HTML document from a file. It implements certain features specific to HTML fragment processing
        /// (formatting preservation and merging, style isolation).
        /// </remarks>
        internal void Read(
            string html,
            DocumentBuilder builder,
            HtmlInsertOptions options,
            bool reuseExistingBookmarks)
        {
            // Remember if we are inserting HTML at the end of a paragraph. In other words, that there's no content between
            // the place where HTML is being inserted and the end of the corresponding paragraph. We use this information later
            // when we decide if we need to insert a paragraph break after the last block imported from HTML.
            // Note that in some mail merge scenarios there can be empty runs between the cursor and the end of the current
            // paragraph. These runs are removed upon saving the document so we ignore them here too.
            bool htmlIsInsertedAtEndOfParagraph = IsAtTheEndOfParagraphWithOptionalEmptyRuns(builder);

            // Paragraph formatting should be preserved after insertion of an HTML fragment.
            ParaPr paraPrBeforeInsertHtml = builder.GetParaPrCopy();
            RunPr paragraphBreakRunPrBeforeInsertHtml = builder.CurrentParagraph.ParagraphBreakRunPr.Clone();

            // WORDSNET-1653 Font formatting should be preserved after insertion of an HTML fragment.
            builder.PushRunPr();

            bool useDocumentBuilderFormatting = (options & HtmlInsertOptions.UseBuilderFormatting) != 0;
            Init(builder, false, useDocumentBuilderFormatting, false);
            HtmlDocument srcDoc = HtmlDocument.Load(
                html,
                mSupportedFeatures,
                mSettings.IgnoreNoscriptElements,
                mSettings.SupportSelfClosingNonHtmlTags);

            ProcessCssRules(srcDoc);

            mHtmlHyperlinkResolver.Init(builder, mBidiTextArranger, reuseExistingBookmarks);

            // WORDSNET-23654 Take into account the current paragraph and created HTML blocks
            // during inserting the HTML document.
            mHtmlBlockReader.Init(builder);

            ProcessHtmlTree(srcDoc.Root, true);

            mHtmlHyperlinkResolver.ProcessBookmarksAndHyperlinks(mSettings.KeepUnreferencedIdBookmarks);

            // End of the document ends the last sequence of inline elements.
            mBidiTextArranger.RearrangeAndWriteText();

            // WORDSNET-682 Inserting bulleted list with InsertHtml leaves list open-ended.
            // Might need to flush the last paragraph (insert a paragraph break), because we didn't explicitly start a paragraph
            // at the end of the last block element.
            // The old behavior is to always start a new paragraph after the last block imported from HTML. The newer behavior
            // is more inteligent and it doesn't start a new paragraph if HTML is inserted at the end of a paragraph, because
            // this results in an extra empty paragraph after inserted HTML. The old behavior is used by default for historical
            // reasons.
            // To preserve paragraph formatting after insertion of an HTML fragment the paragraph we add here
            // should be formatted like the target paragraph into which the HTML fragment is being inserted.
            if (!htmlIsInsertedAtEndOfParagraph || ((options & HtmlInsertOptions.RemoveLastEmptyParagraph) == 0))
            {
                mParagraphArranger.FlushLastParagraphIfNeeded(paraPrBeforeInsertHtml, paragraphBreakRunPrBeforeInsertHtml);
            }

            // Restore the previously saved font formatting.
            builder.PopRunPr();

            mParagraphArranger.DebugAssertHasNoRememberedParagraphs();

            WarnIfDocumentIsFixedPageHtml();
        }

        /// <summary>
        /// Returns ends of bookmarks created during import HTML document.
        /// </summary>
        internal StringToObjDictionary<BookmarkEnd> BookmarkEnds { get { return mHtmlHyperlinkResolver.BookmarkEnds; } }

        private HtmlDocument BuildHtmlTree(
            Stream stream,
            Encoding encoding)
        {
            HtmlDocument htmlDocument = null;
            if (mSettings.ParseAsXhtml)
            {
                try
                {
                    htmlDocument = HtmlDocument.LoadXhtml(stream);
                }
                catch (XmlException)
                {
                    // The input document is not XML. Try to parse it as HTML.
                    mSettings.ParseAsXhtml = false;
                    stream.Position = 0;
                }
            }

            if (htmlDocument == null)
            {
                htmlDocument = HtmlDocument.Load(
                    stream,
                    encoding,
                    mSupportedFeatures,
                    mSettings.IgnoreNoscriptElements,
                    mSettings.SupportSelfClosingNonHtmlTags);
            }

            return htmlDocument;
        }

        /// <summary>
        /// Checks if the cursor of the document builder is at the end of a paragraph. Ignores empty runs between the cursor
        /// and the end of the paragraph.
        /// </summary>
        private static bool IsAtTheEndOfParagraphWithOptionalEmptyRuns(DocumentBuilder builder)
        {
            Node currentNode = builder.CurrentNode;
            // Skip empty runs that have no text.
            while ((currentNode is Run) && !StringUtil.HasChars(((Run)currentNode).Text))
            {
                currentNode = currentNode.NextSibling;
            }
            return currentNode == null;
        }

        private void Init(
            DocumentBuilder builder,
            bool applyDocumentWideFormatting,
            bool useDocumentBuilderFormatting,
            bool textIsVisuallyOrdered)
        {
            // HTML reader instances can be used only once to read a single HTML document. The following check prevents invalid
            // usage of the class.
            if (mBuilder != null)
            {
                throw new InvalidOperationException("Attempt to re-use a HTML reader instance.");
            }

            mBuilder = builder;
            mBaseUri = builder.Document.BaseUri;
            if (mBaseUri == null)
            {
                mBaseUri = string.Empty;
            }

            mApplyDocumentWideFormatting = applyDocumentWideFormatting;
            mUseDocumentBuilderFormatting = useDocumentBuilderFormatting;

            mBaseRunPr = useDocumentBuilderFormatting
                ? builder.GetRunPrCopy()
                : new RunPr();

            mBaseParaPr = useDocumentBuilderFormatting
                ? builder.GetParaPrCopy()
                : new ParaPr();

            mBaseParagraphLeftIndent = useDocumentBuilderFormatting
                ? builder.ParagraphFormat.LeftIndent
                : 0;

            mTextIsInVisualOrder = textIsVisuallyOrdered;
            mBidiTextArranger = new HtmlBidiTextArranger(mBuilder, textIsVisuallyOrdered, false);

            mVmlShapeReader = new HtmlVmlShapeReader(builder, mResourceLoader);
            mHtmlFontFallbackUtil = new HtmlFontFallbackUtil(builder.Document.FontProvider);
        }

        /// <summary>
        /// See <see cref="ProcessHtmlTree"/>.
        /// </summary>
        void IHtmlNodeProcessor.ProcessCell(HtmlElementNode cellNode)
        {
            ProcessHtmlTree(cellNode, false);

            // WORDSNET-16399 Empty table cells collapsed after HTML round-trip if they contained an empty paragraph
            // with "-aw-import: ignore" and had "CellPr.HideMark=true".
            // WORDSNET-27490 MS Word clears the HideMark flag for cells with an empty paragraph at the end.
            // WORDSNET-27752 If a cell ends with a line break, the HideMark flag should also be cleared in order to make
            // the line after the line break always visible.
            Cell cell = mBuilder.CurrentParagraph.ParentCell;
            Paragraph lastParagraph = cell.LastParagraph;
            if (mParagraphArranger.IsParagraphEmptyInHtml(lastParagraph))
            {
                cell.CellPr.HideMark = false;
            }
            else
            {
                Run lastRun = lastParagraph.GetLastRun(true);
                if ((lastRun != null) &&
                    lastRun.Text.EndsWith(ControlChar.LineBreak, StringComparison.Ordinal))
                {
                    cell.CellPr.HideMark = false;
                }
            }

            // WORDSNET-9028 Extra paragraph is added to table's cell so cell becomes higher.
            if ((lastParagraph.GetChildNodes(NodeType.Any, false).Count == 0) &&
                (lastParagraph.PreviousNonAnnotationSibling is Paragraph) &&
                !mParagraphArranger.IsParagraphEmptyInHtml(lastParagraph))
            {
                mBuilder.MoveTo(lastParagraph.PreviousNonAnnotationSibling);
                lastParagraph.Remove();
            }
        }

        /// <summary>
        /// Recursive processes HTML nodes and load all CSS rules.
        /// </summary>
        private void ProcessCssRules(HtmlDocument htmlDocument)
        {
            CssUserAgentFormatting omittedUserAgentFormatting = CssUserAgentFormatting.None;

            // If font formatting of imported text should be based on target font formatting, default font CSS styles
            // should not be applied to the text to preserve the target font formatting.
            // The same applies to paragraph formatting of the text and to default paragraph CSS styles.
            if (mUseDocumentBuilderFormatting)
            {
                omittedUserAgentFormatting |= CssUserAgentFormatting.Font;
                omittedUserAgentFormatting |= CssUserAgentFormatting.Paragraph;
            }

            // By default the color in quirks mode for body tag should be black.
            // In case a HTML fragment is inserted into a document builder using its formatting, this color must be undefined.
            DrColor defaultQuirksBodyColor = mUseDocumentBuilderFormatting ? null : DrColor.Black;

            CssClassFactory classFactory = new CssClassFactory(
               CssUtil.HtmlDocumentModeToCssMode(htmlDocument.Mode),
               omittedUserAgentFormatting,
               defaultQuirksBodyColor,
               mSettings.ApplyFormattingAsMsWord);

            CssReader cssReader = classFactory.CreateCssReader(mBaseUri, mResourceLoader);
            cssReader.ReadAllRules(htmlDocument.Root);
            mCssRules = cssReader.StyleRules;

            // Paragraph style may affect character formatting, so we have to use builder paragraph style when we need
            // to preserve character formatting specified in the document builder.
            int defaultParagraphIstd = mUseDocumentBuilderFormatting
                ? mBuilder.ParagraphFormat.Style.Istd
                : StyleIndex.Normal;

            int defaultFontIstd = mUseDocumentBuilderFormatting
                ? mBuilder.Font.Style.Istd
                : StyleIndex.DefaultParagraphFont;

            mCssResolver = classFactory.CreateCssResolver(cssReader.StyleRules);

            HtmlElementNode htmlElement = htmlDocument.Root.FindSingleElementByName("html");
            string htmlElementInlineCssStyle = (htmlElement != null)
                ? htmlElement.Attributes.GetAttributeValue("style", string.Empty)
                : string.Empty;

            if (htmlElement != null)
            {
                HtmlElementNode bodyElement = htmlElement.FindSingleElementByName("body");
                string bodyElementInlineCssStyle = (bodyElement != null)
                    ? bodyElement.Attributes.GetAttributeValue("style", string.Empty)
                    : string.Empty;

                mCssStyleTracker = classFactory.CreateStyleTracker(
                    cssReader.StyleRules,
                    cssReader.PageRules,
                    htmlElementInlineCssStyle,
                    bodyElementInlineCssStyle,
                    mCssResolver,
                    mSettings.ApplyFormattingAsMsWord,
                    mSettings.UseHtmlBlocks);

                // CSS @font-face rules are imported into document's font info collection, which affects the whole document.
                // If we're not allowed to modify document-wise formatting, we cannot add or modify font infos.
                CssFontFaceProvider fontFaceProvider = (mApplyDocumentWideFormatting && mSettings.SupportFontFaceRules)
                    ? new CssFontFaceProvider(
                        cssReader.FontFaceRules,
                        mBaseUri,
                        mResourceLoader,
                        mBuilder.Document)
                    : null;

                mStyleResolver = mSettings.ApplyFormattingAsMsWord
                    ? new DocumentFormatter(mBuilder.Document, mCssStyleTracker, true)
                    : new DocumentFormatter(
                        mBuilder.Document,
                        defaultParagraphIstd,
                        defaultFontIstd,
                        omittedUserAgentFormatting,
                        mCssStyleTracker,
                        mSettings.UseHtmlBlocks,
                        fontFaceProvider);
            }

            mHtmlControlReader = (mSettings.PreferredControlType == HtmlControlType.StructuredDocumentTag)
                ? (HtmlControlReader)new HtmlControlAsSdtReader(mBuilder, mStyleResolver)
                : new HtmlControlAsFormFieldReader(mBuilder, mStyleResolver);

            mElementCategorizer = classFactory.CreateHtmlElementCategorizer();

            // WORDSNET-6238 Changes to built-in document styles are not allowed when we insert an HTML fragment
            // into a document, which serves as a template.
            if (mApplyDocumentWideFormatting)
            {
                mStyleResolver.UpdateDocumentWideFormatting();
                mStyleResolver.UpdateDefaultSectionProperties(mBuilder.CurrentSection);
            }

            // The paragraph arranger is allowed to reuse the current list item only if we're inserting a HTML fragment
            // into the document (as opposed to loading a whole HTML document into a blank model).
            bool canReuseListItem = !mApplyDocumentWideFormatting;

            // WORDSNET-21193 HtmlParagraphArranger is created with HtmlListReaderManager to work with
            // CurrentListReader.
            mParagraphArranger = new HtmlParagraphArranger(
                this,
                mBidiTextArranger,
                mBuilder,
                mStyleResolver,
                mCommonBorderResolver,
                mBaseParaPr,
                mListReaderManager,
                mHtmlBlockReader,
                canReuseListItem);

            // WORDSNET-8594 Added support for fixed-width spans in MHTML documents.
            // WORDSNET-25538 Enhanced support for fixed-width spans in MHTML. Added support for spans with multiple children
            // and nested fixed-width spans.
            mFixedWidthSpanReader = new HtmlFixedWidthSpanReader(
                mLoadFormat,
                mStyleResolver,
                mBidiTextArranger,
                mBuilder.Document.FontProvider);
        }

        /// <summary>
        /// Processes all nodes of the specified HTML tree in the depth-first manner.
        /// </summary>
        /// <param name="root">
        /// The root element of the HTML tree to process.
        /// </param>
        /// <param name="resolveRootStyle">
        /// Indicates whether CSS styles of the root element must be resolved. CSS styles of other nodes in the sub-tree are
        /// always resolved regardless of this parameter.
        /// </param>
        private void ProcessHtmlTree(HtmlElementNode root, bool resolveRootStyle)
        {
            HtmlTreeEnumerator enumerator = new HtmlTreeEnumerator(root);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is HtmlElementNode)
                {
                    HtmlElementNode elementNode = (HtmlElementNode)enumerator.Current;
                    bool resolveStyle = (enumerator.Current != root) || resolveRootStyle;

                    if (enumerator.IsStart)
                    {
                        if (resolveStyle)
                        {
                            mStyleResolver.PushElement(elementNode, true);
                        }

                        if (HandleElement(elementNode, true) != HandleNodeAction.HandledTraverseChildren)
                        {
                            enumerator.DontEnumerateCurrentSubTree();
                            // The current element won't be visited for the second time.
                            if (resolveStyle)
                            {
                                mStyleResolver.PopElement();
                            }
                        }
                        else
                        {
                            // A ::before pseudo-element, if any exists, becomes the first child of the current element.
                            ProcessPseudoElement(HtmlElementPart.Before);
                        }
                    }
                    else
                    {
                        // An ::after pseudo-element, if any exists, becomes the last child of the current element.
                        ProcessPseudoElement(HtmlElementPart.After);

                        HandleElement(elementNode, false);

                        if (resolveStyle)
                        {
                            mStyleResolver.PopElement();
                        }
                    }
                }
                else if (enumerator.Current is HtmlTextNode)
                {
                    Debug.Assert(enumerator.IsStart);
                    HtmlTextNode textNode = (HtmlTextNode)enumerator.Current;

                    // Ignore last text node containing only whitespaces.
                    if ((textNode.NextSibling == null) &&
                        mStyleResolver.IsBlockLevelElement() &&
                        !ContainsMeaningfulText(textNode))
                    {
                        continue;
                    }

                    HandleText(textNode.Text);
                }
            }
        }

        /// <summary>
        /// Processes a pseudo-element node.
        /// </summary>
        private void ProcessPseudoElement(HtmlElementPart part)
        {
            Debug.Assert(part != HtmlElementPart.Element);

            // ::before pseudo-elements on list items are handled by the list reader, because they might be treated as
            // list item markers.
            bool isListItemMarker = (mStyleResolver.CurrentElement.ElementName == "li") && (part == HtmlElementPart.Before);
            if (isListItemMarker)
            {
                return;
            }

            // We still must try to switch to the part, even if we won't process its content here, because switching to a part
            // updates CSS counters. If we skip switching, values of related counters may become incorrect.
            if (mStyleResolver.SwitchToPart(part, true))
            {

                HandleText(mStyleResolver.GetGeneratedContent());
                mStyleResolver.SwitchToPart(HtmlElementPart.Element, false);
            }
        }

        /// <summary>
        /// Called on start AND/OR end of all HTML elements.
        /// NOTE This method can be called twice for start AND end of the element, depending on what you return in the result.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="isStart">
        /// This is true when called for processing of the element start.
        /// False when called for processing of the element end.
        /// </param>
        private HandleNodeAction HandleElement(
            HtmlElementNode element,
            bool isStart)
        {
            // WORDSNET-25035 Detect if we're loading a fixed-page HTML document and warn customers about formatting loss.
            if (mBuilder.Document.WarningCallback != null)
            {
                mFixedPageFormatDetector.ProcessHtmlElementNode(
                    element.Name,
                    element.Namespace,
                    mStyleResolver.ElementDeclarations);
            }

            // Ruby elements are imported as inline-level nodes and they cannot contain block-level content. That's why we have
            // to close the current ruby element if we encounter a block-level element.
            if ((mRubyReader != null) && mStyleResolver.IsBlockLevelElement())
            {
                mRubyReader.Flush();
                mRubyReader = null;
            }

            // Inline contexts (sequences of adjacent inline elements) are bounded by blocks, breaks, and inline separators
            // (inline elements that are not transparent to bidi algorithm and separate inline contexts before and after them).
            if (mStyleResolver.IsBlockLevelElement() || IsBidiContextSeparator(element))
            {
                mBidiTextArranger.RearrangeAndWriteText();
            }

            if (isStart)
            {
                InsertPostponedPageBreakAfterIfNeeded();
            }

            if ((element.Namespace == NrxNamespaces.Vml) &&
                (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible))
            {
                if (element.Name.EndsWith("textbox", StringComparison.Ordinal))
                {
                    // MS Word ignores textbox elements outside other VML elements but we accept documents containing nothing
                    // but a textbox.
                    return ((element.Parent.Name == "body") && (element.Parent.Children.Count == 1))
                        ? HandleNodeAction.HandledTraverseChildren
                        : HandleNodeAction.HandledSkipChildren;
                }
                mHtmlHyperlinkResolver.StartInlineBookmarkIfNeeded(element);
                HandleNodeAction vmlHandleNodeResult = mVmlShapeReader.HandleVml(element);
                mHtmlHyperlinkResolver.EndInlineBookmarkIfNeeded(element);
                return vmlHandleNodeResult;
            }

            HandleNodeAction result = HandleDisplayInvariantElement(element, isStart, mStyleResolver.ElementDisplayType);

            if (result != HandleNodeAction.NotHandled)
                return result;

            // Depending on 'display' property some elements may changes its properties.
            // It depends on its context too. For example tag 'li' loses its properties outside list and behaves as block-level.
            result = HandleDisplayListItemElement(element, isStart);
            if (result != HandleNodeAction.NotHandled)
                return result;

            result = HandleDisplayTableElement(element, isStart);
            if (result != HandleNodeAction.NotHandled)
                return result;

            result = HandleDisplayBlockElement(element, isStart);
            if (result != HandleNodeAction.NotHandled)
                return result;

            result = HandleDisplayInlineElement(element, isStart);
            if (result != HandleNodeAction.NotHandled)
                return result;

            // Using 'table-columns' or 'table-column-group' outside table context leads to element hiding.
            if (IsTableColumnOrColumnGroupElement())
                return HandleNodeAction.HandledSkipChildren;

            // Do nothing special for unknown elements. Allow them to be processed.
            return HandleNodeAction.HandledTraverseChildren;
        }

        /// <summary>
        /// Processing elements which do not change their properties depending on the 'display' property.
        /// </summary>
        private HandleNodeAction HandleDisplayInvariantElement(
            HtmlElementNode element,
            bool isStart,
            CssDisplayType displayType)
        {
            switch (element.Name)
            {
                case "html":
                {
                    if (isStart)
                        mVmlShapeReader.HandleHtml(element);
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "head":
                {
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "title":
                {
                    if (isStart)
                        mBuilder.Document.BuiltInDocumentProperties["Title"].FromString(element.GetInnerText());
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "meta":
                {
                    if (isStart)
                        HandleMeta(element);
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "base":
                {
                    // WORDSNET-21682 Previously, URI from the 'href' attribute of the 'base' tag always overrode
                    // the base URI specified in load options. Now we combine those URIs, because URI from 'href'
                    // may be relative.
                    if (isStart)
                    {
                        string href = HtmlUtil.ValidateUri(element.Attributes.GetAttributeValue("href", string.Empty));
                        mBaseUri = UriUtil.ConstructUnescapedAbsoluteUri(mBaseUri, href);
                    }

                    // WORDSNET-21522 Attribute "target" of the "base" element should be preserved in a custom document
                    // property. This makes it possible to restore that attribute value upon saving to HTML. MS Word saves
                    // the default "target" value in that custom property too.
                    string target = element.Attributes.GetAttributeValue("target", string.Empty);
                    if (!string.IsNullOrEmpty(target) &&
                        mApplyDocumentWideFormatting &&
                        !mBuilder.Document.CustomDocumentProperties.Contains(HtmlUtil.BaseTargetDocumentProperty))
                    {
                        mBuilder.Document.CustomDocumentProperties.Add(HtmlUtil.BaseTargetDocumentProperty, target);
                    }

                    return HandleNodeAction.HandledSkipChildren;
                }
                // Script element should be ignored.
                case "script":
                {
                    // WORDSNET-19023 Warn the user about the presence of unsupported scripts.
                    if (!mIgnoredAnyScriptNodes)
                    {
                        Warn(WarningType.UnexpectedContent,
                            "HTML client-side scripts are not supported. Scripts have been ignored.");
                        mIgnoredAnyScriptNodes = true;
                    }
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "noscript":
                {
                    // "<noscript>" elements are invisible in browsers when scripting is enabled.
                    return mSettings.IgnoreNoscriptElements
                        ? HandleNodeAction.HandledSkipChildren
                        : HandleNodeAction.NotHandled;
                }
                // Elements that should be ignored.
                // 'style' and 'link' are already processed in the first pass.
                case "style":
                case "link":
                {
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "body":
                {
                    // 'body' element is handled for any value of mStyleResolver.ElementDisplayState, because:
                    // 1) Its nested elements can be visible.
                    // 2) Text nodes with HtmlElementDisplayState.None (display:none) or HtmlElementDisplayState.Hidden ('visibility:hidden')
                    // display states are imported to model as hidden runs.
                    HandleBody(isStart);
                    return HandleNodeAction.HandledTraverseChildren;
                }

                // Form Elements
                case "input":
                {
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledTraverseChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    if (isStart)
                    {
                        HandleInput(element);
                    }
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "select":
                {
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledTraverseChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    HandleSelect(element, isStart);

                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "option":
                {
                    // Using 'table-columns' or 'table-column-group' outside table context leads to element hiding.
                    if (!IsTableColumnOrColumnGroupElement() &&
                        (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible))
                    {
                        HandleOption(element, isStart);
                    }
                    return HandleNodeAction.HandledSkipChildren;
                }
                // Special elements
                case "a":
                {
                    // Using 'table-columns' or 'table-column-group' outside table context leads to element hiding.
                    if (IsTableColumnOrColumnGroupElement())
                        return HandleNodeAction.HandledSkipChildren;

                    // WORDSNET-21612 We used to ignore hidden anchors here but now we always import them, because MS Word
                    // does so. Actually, MS Word always imports other hidden elements, too, but we don't fully mimic its
                    // behavior. The reason is that we already have some requests from customers (for example, WORDSNET-8381)
                    // asking us to ignore certain hidden elements on import. As a result, this fix has been implemented only
                    // for hidden anchors.
                    HandleAnchor(element, isStart);

                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "br":
                {
                    if (isStart && (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible))
                        HandleBreak(element);
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "img":
                {
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledTraverseChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    if (isStart)
                    {
                        HandleImage(element);
                    }

                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "svg":
                    throw new NotImplementedException("FOSS");
                case "math":
                {
                    if (IsTableColumnOrColumnGroupElement() ||
                        (displayType == CssDisplayType.TableRow) ||
                        (displayType == CssDisplayType.TableRowGroup) ||
                        (displayType == CssDisplayType.TableFooterGroup))
                    {
                        return HandleNodeAction.HandledSkipChildren;
                    }

                    if (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible)
                        HandleMathMl(element);

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    return HandleNodeAction.HandledSkipChildren;
                }
                case "embed":
                {
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledTraverseChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    if (isStart)
                    {
                        HandleEmbed(element);
                    }

                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "object":
                {
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledTraverseChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    return isStart
                        ? HandleObject(element)
                        : HandleNodeAction.HandledTraverseChildren;
                }
                case "iframe":
                {
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledSkipChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    if (isStart)
                    {
                        HandleIframe(element);
                    }

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    // WORDSNET-10462 IFRAME contents should be ignored, since it is only visible in legacy browsers
                    // that do not support IFRAME elements. We return false here to prevent processing of IFRAME's children.
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "plaintext":
                {
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "textarea":
                {
                    // Using 'table-columns' or 'table-column-group' outside table context leads to element hiding.
                    if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                        return HandleNodeAction.HandledSkipChildren;

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    HandleTextarea(isStart);

                    mParagraphArranger.SetIsParaStartNeededIfNodeIsBlockLevel();

                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "frameset":
                {
                    if (mLoadFormat == LoadFormat.Chm)
                    {
                        Warn(WarningType.DataLoss, "HTML frames are not supported when loading a CHM document.");
                    }
                    else
                    {
                        // We delegate frameset reading to HtmlFramesetReader class and skip this element in HtmlReader.
                        HtmlFramesetReader framesetReader = new HtmlFramesetReader(mBuilder.Document, mBaseUri, mResourceLoader);
                        framesetReader.Read(element);
                    }
                    return HandleNodeAction.HandledSkipChildren;
                }
                // Table elements
                case "table":
                {
                    if (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible)
                        HandleTable(element);
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "thead":
                case "tbody":
                case "tfoot":
                case "colgroup":
                case "col":
                {
                    // These nodes are processed in HtmlTable.
                    return HandleNodeAction.HandledSkipChildren;
                }
                case "th":
                case "td":
                case "caption":
                {
                    if (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible)
                        HandleCell(isStart);
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "ul":
                case "ol":
                {
                    // Using 'table-columns' or 'table-column-group' outside table context leads to element hiding.
                    if (IsTableColumnOrColumnGroupElement())
                    {
                        return HandleNodeAction.HandledSkipChildren;
                    }

                    // WORDSNET-22044 We used to ignore hidden lists here but now we always import them, because MS Word
                    // does so.
                    // Also lists with any display type will be handled.
                    HandleList(isStart);
                    if (!isStart)
                    {
                        // If the list is empty then we should insert a bookmark after the current paragraph.
                        // The bookmark will be inserted later when all hyperlinks will be collected.
                        if (!mBuilder.CurrentParagraph.IsListItem)
                        {
                            mHtmlHyperlinkResolver.AddPendingBookmarkIfNeeded(element);
                        }
                        else
                        {
                            // The start of bookmark was created during handling the first list item.
                            mHtmlHyperlinkResolver.AddPendingEndBookmarkIfNeeded(element);
                        }
                    }
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "ruby":
                {
                    // In AW document model, ruby nodes cannot nest inside other ruby nodes. That's why we have to flattern
                    // ruby elements and end the current ruby node before starting another one.
                    if (mRubyReader != null)
                    {
                        mRubyReader.Flush();
                        mRubyReader = null;
                    }

                    if (isStart)
                    {
                        mRubyReader = new HtmlRubyReader(mBuilder, mStyleResolver.ElementDeclarations, mTextIsInVisualOrder);
                    }

                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "rp":
                {
                    // <rp> elements inside <ruby> are not visible in browsers that support <ruby> elements.
                    // However, <rp> outside <ruby> are treated as normal inline elements.
                    return (mRubyReader == null)
                        ? HandleNodeAction.HandledTraverseChildren
                        : HandleNodeAction.HandledSkipChildren;
                }
                case "rt":
                {
                    if (mRubyReader != null)
                    {
                        if (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.None)
                        {
                            // Like MS Word, we simply ignore <rt> elements with 'display:none'. If the current <ruby> element
                            // contains no other <rt> elements that are visible, the <ruby> element will be treated as a usual
                            // <span> without a ruby annotation.
                            return HandleNodeAction.HandledSkipChildren;
                        }
                        mRubyReader.IsWritingTopText = isStart;
                    }
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "dl":
                case "dt":
                case "dd":
                {
                    // Using 'table-columns' or 'table-column-group' outside table context leads to element hiding.
                    if (IsTableColumnOrColumnGroupElement())
                        return HandleNodeAction.HandledSkipChildren;

                    if (mStyleResolver.IsBlockLevelElement())
                        HandleDescriptionList(isStart);
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "ins":
                {
                    // Regardless of how the tag is processed(inline, block, etc) it is necessary to make preparations.
                    if (isStart && (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible))
                        HandleRevision(mStyleResolver.ElementInsertionRevision);
                    return HandleNodeAction.NotHandled;
                }
                case "del":
                {
                    // Regardless of how the tag is processed(inline, block, etc) it is necessary to make preparations.
                    if (isStart && (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible))
                        HandleRevision(mStyleResolver.ElementDeletionRevision);
                    return HandleNodeAction.NotHandled;
                }
                case "mbp:pagebreak":
                {
                    // mbp:pagebreak, mbp:nu & mbp:section are custom tags used in MOBI and AZW3 documents.
                    if (isStart)
                    {
                        mBidiTextArranger.RearrangeAndWriteText();

                        // The new break is inserted into a separate run and we need to apply its formatting appropriately.
                        ApplyFontFormatting(true);

                        mBuilder.InsertBreakCore(BreakType.PageBreak, false);
                    }

                    return HandleNodeAction.HandledTraverseChildren;
                }

                default:
                    return HandleNodeAction.NotHandled;
            }
        }

        private HandleNodeAction HandleDisplayListItemElement(HtmlElementNode node, bool isStart)
        {
            if (!IsListItemElement())
                return HandleNodeAction.NotHandled;

            // WORDSNET-22044 We used to ignore hidden list items here but now we always import them, because MS Word
            // does so. But unlike MS Word we import such list items as hidden.
            HandleListItem(node, isStart);
            return HandleNodeAction.HandledTraverseChildren;
        }

        private HandleNodeAction HandleDisplayTableElement(HtmlElementNode node, bool isStart)
        {
            if (!IsTableElement())
                return HandleNodeAction.NotHandled;

            if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
                return HandleNodeAction.HandledSkipChildren;

            if (HandleTable(node))
                return HandleNodeAction.HandledSkipChildren;

            // If the table after import does not have rows and columns,
            // we import it as a block-level or inline element depending on "display" property.
            if (mStyleResolver.ElementDisplayType == CssDisplayType.Table)
                HandleParagraph(isStart);
            else if (mStyleResolver.ElementDisplayType == CssDisplayType.InlineTable)
                HandleSpan(node, isStart);

            return HandleNodeAction.HandledTraverseChildren;
        }

        private HandleNodeAction HandleDisplayBlockElement(HtmlElementNode node, bool isStart)
        {
            if (!IsBlockElement() || IsImageSpan())
                return HandleNodeAction.NotHandled;

            switch (node.Name)
            {
                case "div":
                {
                    // 'div' element is handled for any value of mStyleResolver.ElementDisplayState, because:
                    // 1) Its nested elements can be visible.
                    // 2) Text nodes with HtmlElementDisplayState.None (display:none)
                    //    or HtmlElementDisplayState.Hidden ('visibility:hidden') display states are imported to model
                    //    as hidden runs.
                    if (IsIgnoredHeaderFooterDiv(node, isStart))
                    {
                        // Header (footer) divs are not imported from HTML in the following cases:
                        // 1. If the header (footer) is linked.
                        //    Linked headers and footers are duplicated during export to HTML.
                        //    During import we ignore the duplicates completely, because in the document model a missing header
                        //    (footer) means 'use the header (footer) of the previous section'.
                        // 2. If the current section already has a header (footer) of the same type as the type
                        //    of header (footer) being imported. Related issues are:
                        //    WORDSNET-10597 If the HTML document is malformed and does not contain section breaks,
                        //    we will end up inserting all headers and footers into the same section and an exception will
                        //    occur, because only one header and footer of each type is allowed per section.
                        //    WORDSNET-21803 Discard content of duplicate headers or footers.
                        return HandleNodeAction.HandledSkipChildren;
                    }
                    HandleDiv(node, isStart);
                    if (isStart)
                    {
                        mHtmlHyperlinkResolver.AddPendingStartBookmarkIfNeeded(node);
                    }
                    else
                    {
                        mHtmlHyperlinkResolver.AddPendingEndBookmarkIfNeeded(node);
                    }
                    return HandleNodeAction.HandledTraverseChildren;
                }
                case "hr":
                {
                    if (isStart && (mStyleResolver.ElementDisplayState == HtmlElementDisplayState.Visible))
                    {
                        HandleHorizontalRule(node);
                        // Add a bookmark around the horizontal rule.
                        mHtmlHyperlinkResolver.AddPendingStartBookmarkIfNeeded(node);
                        mHtmlHyperlinkResolver.AddPendingEndBookmarkIfNeeded(node);

                    }
                    return HandleNodeAction.HandledTraverseChildren;
                }
                default:
                {
                    // Nothing to do.
                    break;
                }
            }

            // These elements are handled for any value of mStyleResolver.ElementDisplayState, because:
            // 1) Its nested elements can be visible.
            // 2) Text nodes with HtmlElementDisplayState.None (display:none) or HtmlElementDisplayState.Hidden ('visibility:hidden')
            // display states are imported to model as hidden runs.
            HandleParagraph(isStart);
            if (isStart)
            {
                mHtmlHyperlinkResolver.AddPendingStartBookmarkIfNeeded(node);
            }
            else
            {
                mHtmlHyperlinkResolver.AddPendingEndBookmarkIfNeeded(node);
            }

            return HandleNodeAction.HandledTraverseChildren;
        }

        private bool IsImageSpan()
        {
            return mStyleResolver.ElementDeclarations.GetIdentifier("position") == "absolute" &&
                MathUtil.IsZero(mStyleResolver.ElementDeclarations.GetLength("height"));
        }

        private HandleNodeAction HandleDisplayInlineElement(HtmlElementNode node, bool isStart)
        {
            return IsInlineElement()
                ? HandleSpan(node, isStart)
                : HandleNodeAction.NotHandled;
        }

        /// <summary>
        /// Returns <c>true</c> if the current element has "table" or "inline-table" value in "display" property.
        /// </summary>
        /// <returns></returns>
        private bool IsTableElement()
        {
            switch (mStyleResolver.ElementDisplayType)
            {
                case CssDisplayType.Table:
                case CssDisplayType.InlineTable:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the current element can be interpreted as a "block-level".
        /// </summary>
        private bool IsBlockElement()
        {
            string elementName = mStyleResolver.CurrentElement.ElementName;

            switch (mStyleResolver.ElementDisplayType)
            {
                case CssDisplayType.None:
                    return HtmlElementCategorizer.IsBlockElementByDefault(elementName);
                case CssDisplayType.Block:
                case CssDisplayType.ListItem:
                case CssDisplayType.RunIn:
                // Some tables 'display' values outside table context behave as block-level elements.
                case CssDisplayType.TableCaption:
                case CssDisplayType.TableHeaderGroup:
                case CssDisplayType.TableFooterGroup:
                case CssDisplayType.TableRowGroup:
                case CssDisplayType.TableRow:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the current element can be interpreted as a "inline".
        /// </summary>
        private bool IsInlineElement()
        {
            string elementName = mStyleResolver.CurrentElement.ElementName;

            switch (mStyleResolver.ElementDisplayType)
            {
                case CssDisplayType.None:
                    return !HtmlElementCategorizer.IsBlockElementByDefault(elementName);
                case CssDisplayType.Inline:
                case CssDisplayType.InlineBlock:
                // Element with 'table-cell' display value outside table context has the same behavior as 'inline'.
                case CssDisplayType.TableCell:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the current element can be interpreted as a "list-item".
        /// </summary>
        private bool IsListItemElement()
        {
            string elementName = mStyleResolver.CurrentElement.ElementName;

            if (HtmlUtil.IsParagraphOrHeadingElement(elementName))
            {
                CssDeclaration importBehaviorDeclaration = mStyleResolver.ElementDeclarations[HtmlConstants.Import];
                if ((importBehaviorDeclaration != null) && importBehaviorDeclaration.Value.Equals(CssValue.ListItem))
                {
                    return true;
                }
            }

            if (mStyleResolver.ElementDisplayType == CssDisplayType.ListItem)
            {
                IElementProvider parentNode = mStyleResolver.CurrentElement.GetParentElement();
                // This is list-item:
                // If it is inside list.
                bool isInsideList = (CurrentListReader != null) && (CurrentListReader.CurrentLevelNumber > -1);

                // If 'li' is inside 'body' and outside list (list will be created).
                bool isLiOutsideList = (elementName == "li") &&
                    ((CurrentListReader == null) || (CurrentListReader.CurrentLevelNumber == -1)) &&
                    IsBodyElement(parentNode);

                // If it is "display: list-item" element in any block-level element exclude "body".
                bool isListItemInsideBlock = (elementName != "li") &&
                    mStyleResolver.ParentElementIsBlockLevel() &&
                    !IsBodyElement(parentNode);

                // WORDSNET-22044 If a list item is inside a list with any CSS display type.
                bool isListItemInsideList = (elementName == "li") &&
                    (mStyleResolver.CurrentElement.GetParentElement().ElementName == "ol" ||
                    mStyleResolver.CurrentElement.GetParentElement().ElementName == "ul");

                return isInsideList || isLiOutsideList || isListItemInsideBlock || isListItemInsideList;
            }

            // If it is block-level <li> which has css counter.
            return (elementName == "li") && IsBlockElement() && HasCssCounter();
        }

        /// <summary>
        /// Returns <c>true</c> if the current element has "table-column" or "table-column-group" value in "display" property.
        ///
        /// </summary>
        private bool IsTableColumnOrColumnGroupElement()
        {
            return (mStyleResolver.ElementDisplayType == CssDisplayType.TableColumn) ||
                   (mStyleResolver.ElementDisplayType == CssDisplayType.TableColumnGroup);
        }

        private static bool IsBodyElement(IElementProvider elementProvider)
        {
            return (elementProvider != null) && (elementProvider.ElementName == "body");
        }

        /// <summary>
        /// Called for table nodes.
        /// </summary>
        /// <returns><c>false</c> if table is empty and <c>true</c> if table contains rows and columns.</returns>
        private bool HandleTable(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            HtmlTable htmlTable = HtmlTable.Create(node, mCssStyleTracker, mSettings.ParseAsXhtml);
            if ((htmlTable.RowCount == 0) || (htmlTable.ColumnCount == 0))
            {
                // HTML reader doesn't create a table if the corresponding HTML table contains only an empty row
                // or only an empty caption. At this case we create bookmarks for the table, empty rows and empty captions.
                mHtmlHyperlinkResolver.AddPendingBookmarkIfNeeded(node);

                for (int i = 0; i < htmlTable.RowCount; ++i)
                {
                    mHtmlHyperlinkResolver.AddPendingBookmarkIfNeeded(htmlTable[i].Node);
                }

                for (int i = 0; i < htmlTable.CaptionCount; ++i)
                {
                    mHtmlHyperlinkResolver.AddPendingBookmarkIfNeeded(htmlTable.GetCaption(i).Node);
                }
                return false;
            }

            // If there is an empty paragraph just before the table in the HTML document, we should make sure it remains
            // an empty paragraph in the resulting document and is not used to store the table.
            mParagraphArranger.CloseCurrentParagraphIfEmptyInHtml();

            // We should insert empty paragraph before table in empty list item.
            // WORDSNET-27545 Insert an empty paragraph only if the table will actually be created.
            mParagraphArranger.InsertEmptyParagraphToListItemIfNeeded();

            mTableNestingLevel++;

            // WORDSNET-19132 A table defines a new context for list numbering.
            mListReaderManager.OpenNewContext();

            HtmlTableReader tableReader = new HtmlTableReader(
                mBuilder,
                mCssStyleTracker,
                mStyleResolver,
                mCommonBorderResolver,
                this,
                mSettings.ApplyFormattingAsMsWord,
                mHtmlBlockReader,
                mHtmlHyperlinkResolver);
            Table table = tableReader.ProcessTable(htmlTable, mTableNestingLevel == 1, node);

            // Close the list numbering context defined by this table.
            mListReaderManager.CloseContext();

            // WORDSNET-7798 Here we have to insert an empty paragraph between adjacent tables
            // or MS Word will concatenate them and change layout.
            mHtmlBlockReader.InsertSeparatorParagraphBeforeTable(table);

            mTableNestingLevel--;
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if the current element has counter in css "content" property.
        /// <remarks>This feature is described in WORDSNET-10850.</remarks>
        /// </summary>
        private bool HasCssCounter()
        {
            CssDeclaration declaration = mStyleResolver.BeforePseudoElementDeclarations["content"];
            if (declaration == null)
                return false;

            CssValue value = declaration.Value.FirstValue;

            if (!(value is CssFunctionValue))
                return false;

            CssFunctionValue funcValue = (CssFunctionValue)value;
            return (funcValue.Name == "counter") || (funcValue.Name == "counters");
        }

        private void HandleBody(bool isStart)
        {
            if (isStart)
            {
                bool htmlBlockStarted = mHtmlBlockReader.StartHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mStyleResolver.ElementDisplayType);

                // CSS style of <body> affects formatting of the whole document. However, when we insert an HTML fragment
                // into an existing template document, we do not want the fragment to affect formatting of the whole document.
                // WORDSNET-9421 When an HTML fragment is inserted into an existing document, CSS style of <body>
                // affects formatting of the paragraph currently selected in the target document.
                if (mApplyDocumentWideFormatting)
                {
                    mStyleResolver.ToDocument(mBuilder.Document, GetBackgroundImageBytes());
                    ApplyParagraphFormatting();
                }

                mHtmlBlockReader.UpdateCurrentParagraph(htmlBlockStarted);
            }
            else
            {
                mHtmlBlockReader.EndHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mBidiTextArranger);
            }
        }

        private byte[] GetBackgroundImageBytes()
        {
            string imageUri = mStyleResolver.ElementDeclarations.GetUri("background-image");
            return StringUtil.HasChars(imageUri)
                ? mResourceLoader.LoadImage(mBaseUri, imageUri, false)
                : null;
        }

        private void HandleRevision(EditRevision revision)
        {
            Debug.Assert(revision != null);

            string author = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.RevisionAuthor);
            if (author != string.Empty)
                revision.Author = author;
            string dateTimeStr = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.RevisionDateTime);
            DateTime dateTime = FormatterPal.XmlToDateTime(dateTimeStr);
            if (dateTime != DateTime.MinValue)
                revision.DateTime = dateTime;
        }

        private void HandleInput(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);
            mParagraphArranger.StartParaIfNeeded();
            string type = node.Attributes.GetAttributeValue("type", "text").ToLowerInvariant();
            if (type == "image")
            {
                HandleImage(node);
            }
            else
            {
                mHtmlControlReader.HandleInput(node, type);
            }
        }

        private void HandleSelect(HtmlElementNode node, bool isStart)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            if (isStart)
            {
                mParagraphArranger.StartParaIfNeeded();
                Debug.Assert(mOptionElements.Count == 0);
            }
            else
            {
                mHtmlControlReader.HandleSelect(node, mOptionElements);
                mOptionElements.Clear();
            }
        }

        private void HandleOption(HtmlElementNode node, bool isStart)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            if (!isStart)
                return;

            // We can't add item with "disable" option in Form Field "FORMDROPDOWN" and SDT,
            // because item has not same option.
            // In HTML this item is displayed but it could not be selected
            if (node.Attributes["disabled"] != null)
                return;

            // IE, Opera display text of "label" in dropdown item if attribute exists
            // Firefox, Chrome always display inner text.
            // http://www.w3.org/TR/1999/REC-html401-19991224/interact/forms.html#adef-label-OPTION
            string label = node.Attributes.GetAttributeValue("label", "");
            string innerText = StringUtil.HasChars(label)
                ? label
                : HtmlUtil.RemoveWhitespaces(node.GetInnerText(), true);

            bool isSelected = node.Attributes["selected"] != null;
            string value = node.Attributes.GetAttributeValue("value", "");

            // This collection will be used for import of 'select' element.
            mOptionElements.Add(new HtmlOptionElementInfo(innerText, value, isSelected));
        }

        /// <summary>
        /// Called for text nodes.
        /// </summary>
        private void HandleText(string text)
        {
            if (mIsIgnoreHyperlinkContent)
                return;

            if (mIsFootnoteReferenceText)
            {
                SetFootnoteInfo(text);
                return;
            }

            InsertPostponedPageBreakAfterIfNeeded();

            if (mStyleResolver.IsPreformatted() ||
                mStyleResolver.IsPreformattedWithLine() ||
                mStyleResolver.IsPreformattedWithWrap())
            {

                mParagraphArranger.StartParaIfNeeded();
                ApplyFontFormatting(true);

                // Preformatted text can be multi-line, and we process it line-by-line.
                // The HTML parser guarantees that only '\n' characters will split lines of text.
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    // Insert additional paragraphs for the second and all the next lines of text.
                    if (i > 0)
                    {
                        // Each additional paragraph starts a new inline context.
                        mBidiTextArranger.RearrangeAndWriteText();

                        mParagraphArranger.InsertParagraphImmediately();
                        mBuilder.ParagraphFormat.ClearFormatting();
                        ApplyParagraphFormatting();
                        // WORDSNET-24755 Set the HTML block for a new paragraph created by multi-lined text.
                        mHtmlBlockReader.UpdateCurrentParagraph(true);
                        mCommonBorderResolver.CollectParagraphWithBorder(mBuilder.CurrentParagraph, mStyleResolver.ElementDeclarations);
                    }

                    // WORDSNET-9333 Lines of preformatted text have zero space between them.
                    // We have to manually zero space between the paragraphs containing the lines,
                    // but left space before the first paragraph and after the last paragraph unchanged.
                    bool isFirstLine = i == 0;
                    if (!isFirstLine)
                    {
                        mBuilder.ParagraphFormat.SpaceBefore = 0;
                    }
                    bool isLastLine = i == (lines.Length - 1);
                    if (!isLastLine)
                    {
                        mBuilder.ParagraphFormat.SpaceAfter = 0;
                    }

                    // Remove control characters, because they are invisible in browsers but may have special meaning
                    // in the document model.
                    // If the text has the 'white-space:pre-line' style, we should also collapse sequences of whitespace.
                    string lineText = lines[i];
                    lineText = mStyleResolver.IsPreformattedWithLine()
                        ? HtmlUtil.RemoveControlCharsAndWhitespaces(lineText, IsFullLeftTrim())
                        : HtmlUtil.RemoveControlCharacters(lineText);

                    if (StringUtil.HasChars(lineText))
                    {
                        WriteText(lineText);
                    }
                }
            }
            else
            {
                string originalText = text;
                text = HtmlUtil.RemoveControlCharsAndWhitespaces(text, IsFullLeftTrim());
                if (StringUtil.HasChars(text))
                {
                    mParagraphArranger.StartParaIfNeeded();
                    ApplyFontFormatting(true);
                    WriteText(text);

                    // If we're processing a span that has width specified, we take into account width of text we've just
                    // written.
                    mFixedWidthSpanReader.ProcessText(text, mBuilder.Font);
                }
                else if (HtmlUtil.ContainsAnythingButWhitespaces(originalText, true))
                {
                    // WORDSNET-27752 We get here if the text contains whitespace and control characters only. After filtering
                    // the text becomes empty, but the paragraph must not be removed, because control characters make it
                    // non-empty and visible in HTML.
                    mParagraphArranger.MarkCurrentParagraphAsEmptyInHtml();
                }
            }
        }

        private void SetFootnoteInfo(string referenceText)
        {
            mStyleResolver.ToFont(mBuilder.Font, mBuilder.CurrentParagraph.ParagraphStyle);
            mCurrentFootnote.SetRunPrInternal(mBuilder.GetRunPrCopy());

            // Here we simply ignore hyperlink formatting for now.
            mCurrentFootnote.Font.Underline = Underline.None;
            mCurrentFootnote.Font.Color = Color.Black;

            if (mCurrentFootnote.IsAuto)
            {
                SpecialChar footnoteNumber = new SpecialChar(mBuilder.Document, ControlChar.FootnoteRefChar, mCurrentFootnote.RunPr.Clone());
                footnoteNumber.Font.StyleIdentifier = FootnoteUtil.GetFootnoteReferenceStyleIdentifier(mCurrentFootnote);
                mCurrentFootnote.FirstParagraph.PrependChild(footnoteNumber);
            }
            else
            {
                referenceText = HtmlUtil.RemoveWhitespaces(referenceText, IsFullLeftTrim());
                mCurrentFootnote.ReferenceMark = referenceText.Substring(1, referenceText.Length - 2);

                Run run = new Run(mBuilder.Document, mCurrentFootnote.ReferenceMark, mCurrentFootnote.RunPr.Clone());
                run.Font.StyleIdentifier = FootnoteUtil.GetFootnoteReferenceStyleIdentifier(mCurrentFootnote);
                mCurrentFootnote.FirstParagraph.PrependChild(run);
            }
        }

        /// <summary>
        /// Writes inline text. Adjacent inline text is rearranged according to the Unicode bidirectional algorithm.
        /// </summary>
        /// <param name="text">Text to write.</param>
        private void WriteText(string text)
        {
            Debug.Assert(StringUtil.HasChars(text));

            bool dontCollapseLastSpace = false;
            CssDeclaration importBehaviorDeclaration = mStyleResolver.ElementDeclarations[HtmlConstants.Import];
            if (importBehaviorDeclaration != null)
            {
                // Spans with "space-replacement" role are used to preserve a sequence of spaces in HTML so they are converted
                // back on import. These spans contain non-break spaces that should be replaced on regular space chars.
                if (importBehaviorDeclaration.Value.Equals(CssValue.Spaces) && IsMatchedWithSpaceReplacementPattern(text))
                {
                    text = new string(' ', text.Length);
                    dontCollapseLastSpace = true;
                }
            }

            RunPr baseRunPr = mBuilder.GetRunPrCopy();

            // WORDSNET-13782 Characters 'NON-BREAKING HYPHEN' (U+2011) and 'SOFT HYPHEN' (U+00AD) have special
            // internal representations in Word. If we don't replace these characters on import, they will be rendered
            // incorrectly (with wrong glyphs and/or fonts) because of font fallback.
            text = text.Replace(ControlChar.UnicodeNonBreakingHyphenChar, ControlChar.NonBreakingHyphenChar);
            text = text.Replace(ControlChar.UnicodeOptionalHyphenChar, ControlChar.OptionalHyphenChar);

            // WORDSNET-10430 MS Word 2010 and older fail to use a fallback font when the run font contains no glyphs
            // for some characters. In this case, we have to choose fallback fonts ourselves and use it instead of the run font.
            HtmlFontFallbackRange[] fontFallbackRanges = mHtmlFontFallbackUtil.GetFontFallbackRanges(text, mBuilder.Font);
            foreach (HtmlFontFallbackRange fontFallbackRange in fontFallbackRanges)
            {
                RunPr runPr = baseRunPr;

                // Replace the run font with a fallback font if needed.
                if (fontFallbackRange.NeedsFontFallback)
                {
                    runPr = runPr.Clone();
                    Font font = Font.MakeFont(runPr, mBuilder.Document);
                    font.Name = fontFallbackRange.FallbackFontName;
                }

                if (mRubyReader != null)
                {
                    mRubyReader.WriteText(fontFallbackRange.Text, runPr, mStyleResolver.GetActiveBidiLevels(),
                        dontCollapseLastSpace);
                }
                else
                {
                    // WORDSNET-16568 If an inline HTML element (except for <a> elements) has an 'id' attribute,
                    // we should create a bookmark before the text of that element.
                    Bookmark bookmark = null;
                    if ((mStyleResolver.ElementDisplayType == CssDisplayType.Inline) &&
                        (mStyleResolver.CurrentElement.ElementName != "a"))
                    {
                        // Since, the inline HTML elements will be written later we just pass a new bookmark to the bidi
                        // text arranger. The newly created bookmark is inserted into the current paragraph but it will be moved
                        // before the corresponding run when bookmarks and hyperlinks are processed.
                        BookmarkStart bookmarkStart = mHtmlHyperlinkResolver.StartInlineBookmarkIfNeeded(mStyleResolver.CurrentElement);
                        BookmarkEnd bookmarkEnd = mHtmlHyperlinkResolver.EndInlineBookmarkIfNeeded(mStyleResolver.CurrentElement);
                        if ((bookmarkStart != null) && (bookmarkEnd != null))
                            bookmark = new Bookmark(bookmarkStart, bookmarkEnd);
                    }

                    // The text is not written immediately. Instead, it is appended to the bidirectional text arranger.
                    // Text from adjacent inline elements is processed as a whole at the end of the paragraph.
                    mBidiTextArranger.Append(fontFallbackRange.Text, runPr, mStyleResolver.GetActiveBidiLevels(),
                       dontCollapseLastSpace, bookmark);
                }
            }
        }

        private static bool IsMatchedWithSpaceReplacementPattern(string text)
        {
            if ((text.Length == 1) && (text == ControlChar.NonBreakingSpace))
                return true;

            if (text[text.Length - 1] != ' ')
                return false;

            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i] != ControlChar.NonBreakingSpaceChar)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Writes a Unicode direction mark (LRM or RLM) according to the current inline direction.
        /// </summary>
        private void WriteDirectionMark()
        {
            // The text is not written immediately. Instead, it is appended to the bidirectional arranger.
            // Text from adjacent inline elements is processed as a whole at the end of the paragraph.
            mBidiTextArranger.AppendDirectionMark(mStyleResolver.GetActiveBidiLevels());
        }

        private bool IsFullLeftTrim()
        {
            // We need to check preceding inline text. The text can still be in any of inline buffers or it can be written
            // to the document already.

            // Examine the ruby reader buffer if it contains any text.
            if ((mRubyReader != null) && (!mRubyReader.IsEmpty))
            {
                return mRubyReader.CollapseWhitespaceAfterText();
            }

            // Examine the bidi buffer if it is not empty.
            if (!mBidiTextArranger.IsEmpty)
            {
                return mBidiTextArranger.CollapseWhitespaceAfterText();
            }

            // If all buffers are empty, preceding inline text (if any) has already been written to the document,
            // and we need to examine the document nodes.
            if (mBidiTextArranger.IsEmpty && ((mRubyReader == null) || mRubyReader.IsEmpty))
            {
                // We only remove leading whitespace completely if:
                return mParagraphArranger.IsParaStartNeeded || // this text is not in a block and will start a new paragraph
                    mBuilder.IsAtStartOfParagraph || // the previous character is paragraph break
                    HtmlUtil.IsParagraphHasOnlyFloatingShapes(mBuilder.CurrentParagraph) ||
                    IsLastCharacterSpaceOrBR(); // the previous character is space or line break
            }

            // No inline text has been written yet so there is no preceding space and full left trim is not required
            // for the text being processed.
            return false;
        }

        private bool IsLastCharacterSpaceOrBR()
        {
            Node curNode = mBuilder.CurrentNode;
            Paragraph curPara = mBuilder.CurrentParagraph;

            Run lastRun = null;
            bool usePrevLastNode = (mLastNodeInCurParagraph != null) && (curPara == mLastNodeInCurParagraph.ParentNode);
            Node startNode = usePrevLastNode
                ? mLastNodeInCurParagraph
                : curPara.FirstChild;

            Node lastNode = null;
            for (Node node = startNode; node != curNode; node = node.NextPreOrder(curPara))
            {
                lastNode = node;
                if (node.NodeType == NodeType.Run)
                    lastRun = (Run)node;
            }

            // WORDSNET-2936 White space after IMG tag is missed upon inserting HTML.
            // In case an image is contained in the middle of runs
            // the full left trim must not be applied to the run after the image.
            if ((lastNode != null) && (lastNode.NodeType == NodeType.Shape))
                return false;

            mLastNodeInCurParagraph = lastRun;

            bool result = false;
            if (lastRun != null)
            {
                string text = lastRun.GetText();
                if (StringUtil.HasChars(text))
                {
                    char lastChar = text[text.Length - 1];
                    result = (lastChar == ControlChar.SpaceChar) || (lastChar == ControlChar.LineBreakChar) ||
                        (lastChar == ControlChar.PageBreakChar) || (lastChar == ControlChar.ColumnBreakChar);
                }
            }

            return result;
        }

        private void HandleDiv(HtmlElementNode node, bool isStart)
        {
            // I don't remove spaces before after DIV because some tests,
            // for example TestNoCss.html do not look good.
            // It is a bit tricky to deal with DIV and spaces, will research later.

            if (IsElementIgnoredOnRoundtrip())
                return;

            if (HandleCommentDiv(node, isStart))
                return;

            if (HandleFootnoteDiv(mFootnotes, HtmlConstants.FootnoteIdPrefix, node, isStart))
                return;

            if (HandleFootnoteDiv(mEndnotes, HtmlConstants.EndnoteIdPrefix, node, isStart))
                return;

            if (HandleHeaderFooterDiv(node, isStart))
                return;

            if (HandleStructuredDocumentTagNode(node, isStart))
                return;

            if (isStart)
            {
                // CurrentStory can be null, for example, when we read contents of a text box imported from VML.
                // WORDSNET-21618 CurrentSection can be null when the HTML document has more than one footer or header
                // of the same type. In that case, the HTML reader discards all duplicate headers and footers.
                // WORDSNET-25722 We shouldn't apply formatting to the current section if we're inserting HTML into a header
                // or footer node. We're applying it only if we're inserting into the body (main text) story.
                Body body = mBuilder.CurrentStory as Body;
                if ((body != null) && (body.ParentSection != null))
                {
                    mStyleResolver.ToSection(body.ParentSection);
                }

                mCommonBorderResolver.StartContainer(mStyleResolver.ElementDeclarations);
                mParagraphArranger.StartPara();
                mHtmlBlockReader.StartHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mStyleResolver.ElementDisplayType);

                // WORDSNET-7919 Some DIV elements are used instead of paragraph (P) elements and contain plain text.
                // Apply CSS formatting to paragraphs created for such DIVs.
                ApplyParagraphFormatting();

                mHtmlBlockReader.UpdateCurrentParagraph(true);

                mCommonBorderResolver.CollectParagraphWithoutBorder(mBuilder.CurrentParagraph);
                InsertPageBreakBefore();
            }
            else
            {
                mCommonBorderResolver.EndContainer();

                // WORDSNET-13137 Div is not imported into Aspose.Words DOM.
                // The empty div with non-zero height can be imported as
                // an empty paragraph. MSW omits empty divs in this case.
                if (IsDivEmpty(node))
                {
                    double height = mStyleResolver.ElementDeclarations.GetLength("height");
                    double minHeight = mStyleResolver.ElementDeclarations.GetLength("min-height");
                    double maxHeight = System.Math.Max(height, minHeight);
                    if (maxHeight > 0)
                    {
                        mBuilder.Font.Size = maxHeight;
                        mParagraphArranger.InsertParagraphImmediately();
                    }
                }

                mParagraphArranger.EndPara();
                InsertPageBreakAfter();

                mHtmlBlockReader.EndHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mBidiTextArranger);
            }
        }

        private bool IsDivEmpty(HtmlElementNode node)
        {
            if (node.Children.Count == 0)
                return true;

            if ((node.Children.Count == 1) && (node.Children[0] is HtmlTextNode))
            {
                string text = ((HtmlTextNode)node.Children[0]).Text;
                text = HtmlUtil.RemoveControlCharsAndWhitespaces(text, IsFullLeftTrim());
                return string.IsNullOrEmpty(text);
            }
            return false;
        }

        private void HandleDescriptionList(bool isStart)
        {
            if (isStart)
            {
                mCommonBorderResolver.StartContainer(mStyleResolver.ElementDeclarations);
                mParagraphArranger.StartPara();
                // WORDSNET-7919 Some DIV elements are used instead of paragraph (P) elements and contain plain text.
                // Apply CSS formatting to paragraphs created for such DIVs.
                ApplyParagraphFormatting();
                mCommonBorderResolver.CollectParagraphWithoutBorder(mBuilder.CurrentParagraph);
            }
            else
            {
                mCommonBorderResolver.EndContainer();
                mParagraphArranger.EndPara();
            }
        }

        /// <summary>
        /// Inserts page breaks according to CSS "page-break-before" property.
        /// </summary>
        private void InsertPageBreakBefore()
        {
            CssDeclaration pageBreakBefore = mStyleResolver.ElementDeclarations["page-break-before"];
            if (pageBreakBefore == null)
                return;

            // "page-break-before: always" is already handled in CssPageBreakBeforePropertyDef.ToParagraphFormat.
            if (pageBreakBefore.Value.Equals(CssValue.Auto))
            {
                mBuilder.InsertBreakCore(BreakType.SectionBreakContinuous, false);
            }
            else if (pageBreakBefore.Value.Equals(CssValue.Left))
            {
                mBuilder.InsertBreakCore(BreakType.SectionBreakEvenPage, false);
            }
            else if (pageBreakBefore.Value.Equals(CssValue.Right))
            {
                mBuilder.InsertBreakCore(BreakType.SectionBreakOddPage, false);
            }
            else if (pageBreakBefore.Value.Equals(CssValue.Avoid))
            {
                Node prevNode = mBuilder.CurrentParagraph.PreviousNonAnnotationSibling;
                if (prevNode is Paragraph)
                    ((Paragraph)prevNode).ParagraphFormat.KeepWithNext = true;
            }
        }

        /// <summary>
        /// Inserts page breaks according to CSS "page-break-after" property.
        /// </summary>
        private void InsertPageBreakAfter()
        {
            CssDeclaration pageBreakAfter = mStyleResolver.ElementDeclarations["page-break-after"];
            if (pageBreakAfter == null)
                return;

            // "page-break-after: avoid" is already handled in CssPageBreakAfterPropertyDef.ToParagraphFormat.
            if (pageBreakAfter.Value.Equals(CssValue.Auto))
                mBuilder.InsertBreakCore(BreakType.SectionBreakContinuous, false);
            if (pageBreakAfter.Value.Equals(CssValue.Always))
            {
                // WORDSNET-12102 Extra blank page at the end of a document imported from HTML.
                // Most browsers don't create a new page after the last block of the document event
                // if it has the 'page-break-after:always' style. Page break is inserted only in case
                // there is an opening tag or text after the break.
                mPostponedPageBreakAfterAlways = true;
            }
            else if (pageBreakAfter.Value.Equals(CssValue.Left))
            {
                mBuilder.InsertBreakCore(BreakType.SectionBreakEvenPage, false);
            }
            else if (pageBreakAfter.Value.Equals(CssValue.Right))
            {
                mBuilder.InsertBreakCore(BreakType.SectionBreakOddPage, false);
            }
        }

        private void InsertPostponedPageBreakAfterIfNeeded()
        {
            if (mPostponedPageBreakAfterAlways)
            {
                mBuilder.InsertBreakCore(BreakType.SectionBreakNewPage, false);
                mPostponedPageBreakAfterAlways = false;
            }
        }

        private bool HandleCommentDiv(HtmlElementNode node, bool isStart)
        {
            string divId = node.Attributes.GetAttributeValue("id", "");
            if (divId.StartsWith(HtmlConstants.CommentIdPrefix, StringComparison.Ordinal))
            {
                int commentId = TryParseCommentIdNumber(divId);
                if (commentId <= 0)
                {
                    if (isStart)
                    {
                        Warn(WarningType.MajorFormattingLoss,
                            string.Format(
                                "Can't parse comment id from '{0}'. Positive integer is expected after prefix '{1}'. " +
                                "Comment's content has been treated as regular document's subtree.",
                                divId, HtmlConstants.CommentIdPrefix));
                    }

                    return true;
                }

                if (mComments[commentId] == null)
                {
                    if (isStart)
                    {
                        Warn(WarningType.MajorFormattingLoss,
                            string.Format("There is no comment with comment id '{0}'. Comment's content has been treated as regular document's subtree.",
                            commentId));
                    }

                    return true;
                }

                if (isStart)
                {
                    Comment currentComment = mComments[commentId];
                    string author = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.CommentAuthor);
                    string initial = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.CommentInitial);
                    string dateTimeStr = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.CommentDateTime);
                    if ((author == string.Empty) && (initial == string.Empty) && (dateTimeStr == string.Empty))
                    {
                        // Try to read roundtrip information saved in old data-* format.
                        author = node.Attributes.GetAttributeValue(HtmlConstants.CommentAuthorDataAttribute, string.Empty);
                        initial = node.Attributes.GetAttributeValue(HtmlConstants.CommentInitialDataAttribute, string.Empty);
                        dateTimeStr = node.Attributes.GetAttributeValue(HtmlConstants.CommentDateTimeDataAttribute, string.Empty);
                    }

                    currentComment.Author = author;
                    currentComment.Initial = initial;
                    DateTime dateTime = FormatterPal.XmlToDateTime(dateTimeStr);
                    if (dateTime != DateTime.MinValue)
                        currentComment.LocalDateTime = dateTime;

                    int parentCommentId = TryParseCommentIdNumber(mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.CommentParentId));
                    if (parentCommentId > 0)
                    {
                        Comment parent = mComments[parentCommentId];
                        // Protect against references to unknown comments and references to self.
                        if ((parent != null) && (parent.Id != currentComment.Id))
                        {
                            currentComment.ParentId = parent.Id;
                        }
                    }

                    bool isDone = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.CommentDone) == HtmlConstants.CommentDoneYes;
                    currentComment.Done = isDone;

                    mParagraphArranger.RememberCurrentParagraphAndMoveTo(currentComment.FirstParagraph);
                }
                else
                {
                    mParagraphArranger.RestoreCurrentParagraphIfNeeded();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a positive integer number from a comment id like '_cmnt123'.
        /// In case of an error, returns <see cref="int.MinValue"/>.
        /// </summary>
        private static int TryParseCommentIdNumber(string commentId)
        {
            if (StringUtil.HasChars(commentId) && (commentId.Length > HtmlConstants.CommentIdPrefix.Length))
            {
                string numberSubstring = commentId.Substring(HtmlConstants.CommentIdPrefix.Length);
                int number = FormatterPal.TryParseInt(numberSubstring);
                if (number > 0)
                {
                    return number;
                }
            }
            return int.MinValue;
        }

        private bool HandleFootnoteDiv(IntToObjDictionary<Footnote> footnotes, string footnoteIdPrefix, HtmlElementNode node, bool isStart)
        {
            string divId = node.Attributes.GetAttributeValue("id", "");
            if (divId.StartsWith(footnoteIdPrefix, StringComparison.Ordinal))
            {
                int footnoteId = FormatterPal.TryParseInt(divId.Substring(footnoteIdPrefix.Length));
                if ((footnoteId == int.MinValue) || (footnoteId <= 0))
                {
                    if (isStart)
                    {
                        Warn(WarningType.MajorFormattingLoss,
                            string.Format(
                                "Can't parse footnote id from '{0}'. Positive integer is expected after prefix '{1}'. " +
                                "Footnotes's content has been treated as regular document's subtree.",
                                divId, footnoteIdPrefix));
                    }

                    return true;
                }

                Footnote footnote = footnotes[footnoteId];
                if (footnote == null)
                {
                    if (isStart)
                    {
                        Warn(WarningType.MajorFormattingLoss,
                            string.Format(
                                "There is no footnote with footnote id '{0}'. Footnote's content has been treated as regular document's subtree.",
                                footnoteId));
                    }

                    return true;
                }

                if (isStart)
                {
                    mCurrentFootnote = footnote;

                    double footnoteIsAutoValue = mStyleResolver.ElementDeclarations.GetNumber(HtmlConstants.FootnoteIsAuto);
                    int footnoteIsAuto = MathUtil.IsMinValue(footnoteIsAutoValue)
                        ? node.Attributes.GetAttributeValue(HtmlConstants.FootnoteIsAutoAttribute, -1)
                        : (int)footnoteIsAutoValue;

                    if ((footnoteIsAuto != 0) && (footnoteIsAuto != 1))
                    {
                        Warn(WarningType.MinorFormattingLoss,
                            string.Format(
                                "Can't determine if footnote with footnote id '{0}' is auto. Footnote has been treated as having custom mark.",
                                footnoteId));
                        mCurrentFootnote.IsAuto = false;
                    }
                    else
                    {
                        mCurrentFootnote.IsAuto = footnoteIsAuto != 0;
                    }

                    mParagraphArranger.RememberCurrentParagraphAndMoveTo(mCurrentFootnote.FirstParagraph);
                }
                else
                {
                    mCurrentFootnote = null;
                    mParagraphArranger.RestoreCurrentParagraphIfNeeded();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the HTML element being processed represents a header/footer that
        /// cannot be imported into the document model.
        /// </summary>
        private bool IsIgnoredHeaderFooterDiv(HtmlElementNode node, bool isStart)
        {
            // This method processes only opening div tags.
            if (!isStart)
            {
                return false;
            }

            // Divs that represent linked headers/footers must be ignored. In the document model, if a header/footer is null,
            // it is inherited from the previous section by default (becomes linked).
            string headerFooterTypeValue = GetHeaderFooterTypeValue(node);
            if (headerFooterTypeValue == HtmlConstants.HeaderFooterTypeLinked)
            {
                return true;
            }

            // If we cannot parse header/footer type, it is not a header/footer div.
            HeaderFooterType headerFooterType;
            if (!TryParseHeaderFooterType(headerFooterTypeValue, out headerFooterType))
            {
                return false;
            }

            // WORDSNET-25722 We must process headers and footers only if we're inserting into the document's body (main text)
            // story. If we're inserting into any other story (for example, into a existing header or a footer), we must ignore
            // header/footer divs declared in HTML.
            // Note that CurrentStory may also be null here.
            Body body = mBuilder.CurrentStory as Body;
            if (body == null)
            {
                return true;
            }

            Section currentSection = body.ParentSection;
            if (currentSection == null)
            {
                return true;
            }

            // If there is already another header/footer of this type in the current section, the header/footer div
            // must be discarded.
            bool isDuplicate = currentSection.HeadersFooters[headerFooterType] != null;
            if (isDuplicate)
            {
                string warning = string.Format(
                    "A duplicate header/footer of type '{0}' has been ignored.",
                    headerFooterTypeValue);
                Warn(WarningType.DataLoss, warning);
            }
            return isDuplicate;
        }

        /// <summary>
        /// Parses &lt;div&gt; elements containing document headers and footers.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the element was parsed as a header or a footer.
        /// <c>false</c> if the element is a regular &lt;div&gt; (contains neither a header nor a footer) and should be parsed
        /// elsewhere.
        /// </returns>
        /// <remarks>
        /// This method does not handle linked headers and footers, which are duplicated during export to HTML.
        /// </remarks>
        private bool HandleHeaderFooterDiv(HtmlElementNode node, bool isStart)
        {
            string headerFooterTypeValue = GetHeaderFooterTypeValue(node);
            HeaderFooterType headerFooterType;
            if (!TryParseHeaderFooterType(headerFooterTypeValue, out headerFooterType))
                return false;

            if (isStart)
            {
                // A header (footer) instance is created here, but it will be inserted into the document only after
                // the whole header (footer) is parsed. This allows to simply discard linked headers (footers).
                HeaderFooter headerFooter = new HeaderFooter(mBuilder.Document, headerFooterType);
                headerFooter.AppendChild(new Paragraph(mBuilder.Document));
                mBuilder.CurrentSection.HeadersFooters.Add(headerFooter);

                // During export to HTML, the user decided that first-page header and footer were more important than primary
                // ones. We reflect this decision in the imported document.
                string differentFirstPageValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.HeaderFooterDifferentFirstPage);
                if ((differentFirstPageValue == HtmlConstants.HeaderFooterDifferentFirstPageTrue) ||
                    (headerFooterType == HeaderFooterType.HeaderFirst) || (headerFooterType == HeaderFooterType.FooterFirst))
                {
                    mBuilder.CurrentSection.SectPr.DifferentFirstPageHeaderFooter = true;
                }

                mParagraphArranger.RememberCurrentParagraphAndMoveTo(headerFooter.FirstParagraph);
            }
            else
            {
                mParagraphArranger.RestoreCurrentParagraphIfNeeded();
            }

            return true;
        }

        /// <summary>
        /// Called for simple block level elements. Starts new paragraph when needed and applies formatting.
        /// </summary>
        private void HandleParagraph(bool isStart)
        {
            if (isStart)
            {
                mParagraphArranger.StartPara();

                bool htmlBlockStarted = mHtmlBlockReader.StartHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mStyleResolver.ElementDisplayType);

                ApplyParagraphFormatting();

                mCommonBorderResolver.CollectParagraphWithBorder(
                            mBuilder.CurrentParagraph,
                            mStyleResolver.ElementDeclarations);

                mHtmlBlockReader.UpdateCurrentParagraph(htmlBlockStarted);
            }
            else
            {
                mParagraphArranger.EndPara();
                mHtmlBlockReader.EndHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mBidiTextArranger);
            }
        }

        /// <summary>
        /// Applies font formatting from CSS styles to the document builder font.
        /// </summary>
        internal void ApplyFontFormatting(bool allowTextDecoration)
        {
            // When we are at the end of the current paragraph or it is empty, setting its font also affects the paragraph break font.
            // We do not want to change the paragraph break font, because it was formatted correctly when the paragraph was
            // inserted, so we save the paragraph break font and restore it later.
            RunPr savedParagraphBreakRunPr = (mBuilder.IsAtEndOfParagraph || mBuilder.CurrentParagraph.IsEmptyOrContainsOnlyCrossAnnotation)
                                                            ? mBuilder.CurrentParagraph.ParagraphBreakRunPr.Clone()
                                                            : null;

            // Start building new font formatting upon the base font formatting.
            mBuilder.SetFont(mBaseRunPr, true);

            // WORDSNET-12199 If style without CSS declarations was applied we should expand style's RunPr properties to current RunPr.
            if (mStyleResolver.StyleWithoutDeclarations != null)
            {
                RunPr runPr = mBuilder.GetRunPrCopy();
                mStyleResolver.StyleWithoutDeclarations.RunPr.ExpandTo(runPr);
                mBuilder.SetFont(runPr, false);
            }

            // Apply CSS formatting.
            mStyleResolver.ToFont(mBuilder.Font, mBuilder.CurrentParagraph.ParagraphStyle);

            if (mStyleResolver.ElementDisplayState != HtmlElementDisplayState.Visible)
            {
                mBuilder.Font.Hidden = true;
            }

            // Revisions.
            EditRevision insertionRevision = mStyleResolver.ElementInsertionRevision;
            if (insertionRevision != null)
            {
                ((IRunAttrSource)mBuilder).SetRunAttr(RevisionAttr.InsertRevision, insertionRevision);
            }
            EditRevision deletionRevision = mStyleResolver.ElementDeletionRevision;
            if (deletionRevision != null)
            {
                ((IRunAttrSource)mBuilder).SetRunAttr(RevisionAttr.DeleteRevision, deletionRevision);
            }

            // CSS rules disallow text decoration on inline elements that are not text or whitespace (for example, images).
            // However, we apply text decoration to that elements if the user explicitly asks us to do so.
            if (!allowTextDecoration)
            {
                mBuilder.Font.Underline = Underline.None;
                mBuilder.Font.StrikeThrough = false;
            }

            // WORDSNET-12924 Like MS Word, we enable kerning for text imported with the 'Heading 1' style. Because
            // there is no CSS counterpart for kerning, we have to apply it directly.
            if (!mUseDocumentBuilderFormatting &&
                !mSettings.ApplyFormattingAsMsWord &&
                (mBuilder.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1))
            {
                mBuilder.Font.Kerning = 18;
            }

            // If font formatting of the target document should not affect formatting of text imported from HTML,
            // we have to override target font formatting, and we do so by collapsing direct font formatting
            // with default paragraph formatting and with formatting specified by paragraph and font styles.
            // After collapse:
            // 1. All formatting specified by styles that is duplicated in direct formatting will be removed from
            //    direct formatting.
            // 2. All formatting specified by styles that is not present in direct formatting will be explicitly reset
            //    to default in direct formatting.
            if (!mUseDocumentBuilderFormatting && !mSettings.ApplyFormattingAsMsWord)
            {
                RunPr styleRunPr = mBuilder.ParagraphFormat.Style.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
                mBuilder.Font.Style.ExpandRunPr(styleRunPr, RunPrExpandFlags.Normal);
                RunPr uncollapsedDirectRunPr = mBuilder.GetRunPrCopy();

                RunPr collapsedDirectRunPr = mBuilder.GetRunPrCopy();
                collapsedDirectRunPr.ThemeColorInheritanceHack(styleRunPr);
                collapsedDirectRunPr.Collapse(styleRunPr, FontAttr.Istd);

                // WORDSNET-21435 Toggle attributes didn't resolve correctly if they were defined both in
                // Character style and Paragraph (or base) style, because values from different style levels are processed
                // in a special way for toggle attributes: instead of overriding each other, they are XORed together.
                // For details, see ECMA-376-1:2016, section 17.7.3 "Toggle properties".
                // Since the direct value of an attribute always overrides the value inherited from styles or document default,
                // we preserve values of toggle attributes in direct formatting (don't collapse them) in case styles or document
                // defaults also define a value for that attribute.
                // The fix is applied if the text has non-default character style, because we don't support table styles yet
                // and toggle properties can only be inherited from paragraph style or character style. The problematic case
                // is when both paragraph style and character style set the toggle property value to "true". If the character
                // style is default, it sets toggle property values to "false" and that causes no problem.
                if (collapsedDirectRunPr.Istd != StyleIndex.DefaultParagraphFont)
                {
                    foreach (int toggleAttribute in RunPr.ToggleAttributes)
                    {
                        object attributeValue = uncollapsedDirectRunPr.GetDirectAttr(toggleAttribute);
                        if ((attributeValue != null) && !collapsedDirectRunPr.ContainsKey(toggleAttribute))
                        {
                            collapsedDirectRunPr.SetAttr(toggleAttribute, attributeValue);
                        }
                    }
                }

                mBuilder.SetFont(collapsedDirectRunPr, false);
            }

            // Preserve the paragraph break font if needed. See the comment above, where the paragraph break font is saved.
            if (savedParagraphBreakRunPr != null)
            {
                mBuilder.CurrentParagraph.ParagraphBreakRunPr = savedParagraphBreakRunPr;
            }
        }

        /// <summary>
        /// Applies paragraph formatting from CSS styles to the current paragraph.
        /// </summary>
        internal void ApplyParagraphFormatting()
        {
            // Here we indent paragraphs created from <li> elements and also reindent the paragraphs if an <li> element contains
            // other block-level elements (for example, <p> - see WORDSNET-11567).
            if ((CurrentListReader != null) && (!mSettings.ApplyFormattingAsMsWord))
            {
                CurrentListReader.IndentCurrentParagraphAsListItemIfNeeded();
            }

            // Apply CSS formatting.
            mStyleResolver.ToParagraphFormat(mBuilder.CurrentParagraph);

            // WORDSNET-7347 visually ordered RTL paragraphs are declared as LTR paragraphs in HTML.
            if (mTextIsInVisualOrder)
            {
                switch (mBuilder.ParagraphFormat.Alignment)
                {
                    case ParagraphAlignment.Left:
                        mBuilder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                        break;
                    case ParagraphAlignment.Right:
                        mBuilder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                        break;
                    default:
                        // Ignore.
                        break;
                }
                mBuilder.ParagraphFormat.Bidi = true;
            }

            // If paragraph formatting of the target document should not affect formatting of text imported from HTML,
            // we have to override target paragraph formatting, and we do so by collapsing direct paragraph formatting
            // with formatting specified by paragraph style and with default paragraph formatting.
            // After this:
            // 1. All formatting specified by styles and duplicated in direct formatting will be removed from
            //    direct formatting.
            // 2. All formatting specified by styles and not present in direct formatting will be explicitly reset
            //    to default in direct formatting.
            if (!mUseDocumentBuilderFormatting &&
                !mSettings.ApplyFormattingAsMsWord)
            {
                ParaPr styleParaPr = mBuilder.ParagraphFormat.Style.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);
                ParaPr directParaPr = mBuilder.GetParaPrCopy();
                directParaPr.Collapse(styleParaPr, ParaAttr.Istd);
                mBuilder.CurrentParagraph.ParaPr = directParaPr;
            }
        }

        /// <summary>
        /// This is called by the table reader.
        /// </summary>
        /// <remarks>
        /// It's not very nice that part of table processing happens inside table reader and part here,
        /// but I don't see what I can move to make it all happen in one place.
        /// </remarks>
        private void HandleCell(bool isStart)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            // Ignore cell tags outside table.
            if (mTableNestingLevel == 0)
                return;

            if (isStart)
            {
                //In reality, whole StartPara and EndPara are not needed here, only need to raise
                //the flag that paragraph start is required.
                mParagraphArranger.StartPara();
                ApplyParagraphFormatting();
            }
            else
            {
                mParagraphArranger.EndPara();
            }
        }

        /// <summary>
        /// Handles A element to create a bookmark and/or hyperlink.
        /// </summary>
        private void HandleAnchor(HtmlElementNode node, bool isStart)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            // WORDSNET-27579 Inline-level <a> elements may map to the "Hyperlink" style so their style needs to be processed
            // and remembered for later use by child elements.
            if (IsInlineElement())
            {
                if (isStart)
                {
                    mStyleResolver.HandleSpanStart();
                }
                else
                {
                    mStyleResolver.HandleSpanEnd();
                }
            }

            //It could be a bookmark or a hyperlink.
            string href = HtmlUtil.ValidateUri(node.Attributes.GetAttributeValue("href", string.Empty));
            if (isStart)
                mHtmlHyperlinkResolver.AddHyperlink(href);

            if ((href != string.Empty) && !UriUtil.IsSubAddressOnly(href))
                href = UriUtil.ConstructAbsoluteUri(mBaseUri, href);
            string bookmarkName = node.Attributes.GetAttributeValue("name", "");

            // If no "name" specified try "id". "name" is marked as obsolete but used by MS Word.
            if (!StringUtil.HasChars(bookmarkName))
            {
                bookmarkName = node.Attributes.GetAttributeValue("id", "");
            }

            if (StringUtil.HasChars(bookmarkName) && (mSettings.HyperlinkProcessor != null))
            {
                bookmarkName = mSettings.HyperlinkProcessor.MapBookmarkName(bookmarkName);
            }

            if (HandleCommentAnchor(bookmarkName, href, isStart))
                return;

            if (HandleFootnoteAnchor(
                FootnoteType.Footnote,
                mFootnotes,
                HtmlConstants.FootnoteReferenceIdPrefix,
                HtmlConstants.FootnoteHyperlinkHrefPrefix,
                HtmlConstants.FootnoteReferenceHyperlinkHrefPrefix,
                bookmarkName,
                href,
                isStart))
            {
                return;
            }

            if (HandleFootnoteAnchor(
                FootnoteType.Endnote,
                mEndnotes,
                HtmlConstants.EndnoteReferenceIdPrefix,
                HtmlConstants.EndnoteHyperlinkHrefPrefix,
                HtmlConstants.EndnoteReferenceHyperlinkHrefPrefix,
                bookmarkName,
                href,
                isStart))
            {
                return;
            }

            if (IsElementIgnoredOnRoundtrip())
                return;

            bool parentNodeIsParagraph = node.Parent.Name == "p";
            if (isStart)
            {
                if (mStyleResolver.IsBlockLevelElement())
                {
                    mParagraphArranger.StartPara();
                    ApplyParagraphFormatting();
                    mHtmlBlockReader.UpdateCurrentParagraph(false);
                }

                if (StringUtil.HasChars(href))
                {
                    if (IsValidHyperlinkHref(href))
                    {
                        if (mSettings.HyperlinkProcessor != null)
                        {
                            href = mSettings.HyperlinkProcessor.MapHyperlinkHref(href);
                        }

                        string target = node.Attributes.GetAttributeValue("target", string.Empty);
                        string screenTip = node.Attributes.GetAttributeValue("title", "");
                        // The screen tip's length limit has been chosen arbitrarily. MS Word shows about 2048 characters
                        // of screen tip text at most, Chrome shows 1024 characters, IE even less. Firefox and Opera do not limit
                        // the screen tip's length.
                        const int maxScreenTipLength = 2048;
                        if (screenTip.Length > maxScreenTipLength)
                            screenTip = screenTip.Substring(0, maxScreenTipLength - 1) + "…";

                        mBuilder.StartHyperlink(href, target, screenTip);
                    }
                    else
                    {
                        Warn(WarningType.DataLoss, "The hyperlink '" + href + "' is not supported and it has been ignored.");
                    }
                }

                mHtmlHyperlinkResolver.StartAnchorBookmarkIfNeeded(bookmarkName, parentNodeIsParagraph, false);
            }
            else
            {
                if (mStyleResolver.IsBlockLevelElement())
                {
                    mParagraphArranger.EndPara();
                }

                // End the hyperlink field in the model if it has been created for this anchor.
                if (StringUtil.HasChars(href) && IsValidHyperlinkHref(href))
                {
                    mBuilder.EndHyperlink();
                }

                mHtmlHyperlinkResolver.EndAnchorBookmarkIfNeeded(bookmarkName, parentNodeIsParagraph, false);
            }
        }

        private bool IsElementIgnoredOnRoundtrip()
        {
            string ignore = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.IgnoreOnRoundtrip);
            return ignore == HtmlConstants.IgnoreOnRoundtripTrue;
        }

        private bool HandleCommentAnchor(string bookmarkName, string href, bool isStart)
        {
            if (href.StartsWith(HtmlConstants.CommentHyperlinkHrefPrefix, StringComparison.Ordinal) ||
                href.StartsWith(HtmlConstants.CommentReferenceHyperlinkHrefPrefix, StringComparison.Ordinal))
            {
                mIsIgnoreHyperlinkContent = isStart;
                return true;
            }

            if (bookmarkName.StartsWith(HtmlConstants.CommentReferenceIdPrefix, StringComparison.Ordinal))
            {
                int commentId = FormatterPal.TryParseInt(bookmarkName.Substring(HtmlConstants.CommentReferenceIdPrefix.Length));
                if (commentId == int.MinValue)
                {
                    if (isStart)
                    {
                        Warn(WarningType.DataLoss,
                             string.Format(
                                 "Can't parse comment id from '{0}' in anchor element. " +
                                 "Positive integer is expected after prefix '{1}'. " +
                                 "Comment possibly has been lost.",
                                 bookmarkName, HtmlConstants.CommentReferenceIdPrefix));
                    }
                    return true;
                }

                if (isStart)
                {
                    if (!mComments.ContainsKey(commentId))
                        StartComment(commentId);
                }
                else
                {
                    if (!mFullyProcessedComments.Contains(commentId))
                        EndComment(commentId);
                }

                return true;
            }

            return false;
        }

        private bool HandleFootnoteAnchor(
            FootnoteType footnoteType,
            IntToObjDictionary<Footnote> footnotes,
            string footnoteReferenceIdPrefix,
            string footnoteHyperlinkHrefPrefix,
            string footnoteReferenceHyperlinkHrefPrefix,
            string bookmarkName,
            string href,
            bool isStart)
        {
            // WORDSNET-10535 Some footnote should be currently in processing in order to deal with footnote's reference text.
            if (href.StartsWith(footnoteReferenceHyperlinkHrefPrefix, StringComparison.Ordinal) && (mCurrentFootnote != null))
            {
                int footnoteId = FormatterPal.TryParseInt(href.Substring(footnoteReferenceHyperlinkHrefPrefix.Length));
                if ((footnoteId == int.MinValue) || (footnotes[footnoteId] == null))
                    return false;

                mIsFootnoteReferenceText = isStart;
                return true;
            }

            if (href.StartsWith(footnoteHyperlinkHrefPrefix, StringComparison.Ordinal))
            {
                int footnoteId = FormatterPal.TryParseInt(href.Substring(footnoteHyperlinkHrefPrefix.Length));
                if ((footnoteId == int.MinValue) || (footnotes[footnoteId] == null))
                    return false;

                mIsIgnoreHyperlinkContent = isStart;
                return true;
            }

            if (bookmarkName.StartsWith(footnoteReferenceIdPrefix, StringComparison.Ordinal))
            {
                int footnoteId = FormatterPal.TryParseInt(bookmarkName.Substring(footnoteReferenceIdPrefix.Length));
                if (footnoteId == int.MinValue)
                {
                    if (isStart)
                    {
                        Warn(WarningType.DataLoss,
                            string.Format(
                                "Can't parse footnote id from '{0}'. Positive integer is expected after prefix '{1}'. Footnote has been lost.",
                                bookmarkName, footnoteReferenceIdPrefix));
                    }

                    return true;
                }

                // WORDSNET-11705 We ignore duplicate footnotes.
                if (footnotes.ContainsKey(footnoteId))
                {
                    Warn(WarningType.DataLoss,
                        string.Format(
                            "Document already contains footnote with footnote id '{0}'. Footnote has been lost.", footnoteId));
                    return false;
                }

                if (!isStart)
                {
                    // WORDSNET-21724 Previously, HTML reader inserted a footnote at the end of current paragraph.
                    // Currently, it inserts the footnote before the document builder's cursor.
                    Footnote footnote = mBuilder.InsertFootnote(footnoteType, null, string.Empty);
                    footnote.FirstParagraph.RemoveAllChildren();
                    footnotes.Add(footnoteId, footnote);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts comment in model.
        /// </summary>
        private void StartComment(int commentId)
        {
            Comment comment = new Comment(mBuilder.Document);
            mComments.Add(commentId, comment);
            CommentRangeStart commentRangeStart = new CommentRangeStart(mBuilder.Document, comment.Id);
            mBuilder.InsertNode(commentRangeStart);
            comment.AppendChild(new Paragraph(mBuilder.Document));
        }

        /// <summary>
        /// Ends comment in model.
        /// </summary>
        private void EndComment(int commentId)
        {
            Comment comment = mComments[commentId];
            CommentRangeEnd commentRangeEnd;
            if (mCommentRangeEnds.ContainsKey(commentId))
            {
                commentRangeEnd = mCommentRangeEnds[commentId];
                commentRangeEnd.Remove();
                comment.Remove();
            }
            else
            {
                commentRangeEnd = new CommentRangeEnd(mBuilder.Document, comment.Id);
                mCommentRangeEnds.Add(commentId, commentRangeEnd);
            }

            mBuilder.InsertNode(commentRangeEnd);
            mBuilder.InsertNode(comment);
        }

        /// <summary>
        /// Handles a META node.
        /// </summary>
        private void HandleMeta(HtmlElementNode node)
        {
            string name = node.Attributes.GetAttributeValue("name", "");
            string content = node.Attributes.GetAttributeValue("content", "");

            if (StringUtil.EqualsIgnoreCase(name, "description"))
                mBuilder.Document.BuiltInDocumentProperties["Subject"].FromString(content);
            else if (StringUtil.EqualsIgnoreCase(name, "keywords"))
                mBuilder.Document.BuiltInDocumentProperties["Keywords"].FromString(content);
        }

        /// <summary>
        /// Handles BR node. It can represent a section, a page or a line break.
        /// </summary>
        private void HandleBreak(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            // It is a fast fix for processing of single and double <br> elements at the end of blocks.
            // In MS Word, the first break is ignored and the second break increases paragraph spacing.
            if (mSettings.ApplyFormattingAsMsWord && HandleBreaksAtBlockEnd(node))
                return;

            string pageBreakBefore = mStyleResolver.ElementDeclarations.GetIdentifier("page-break-before").ToLowerInvariant();
            string msoBreakType = mStyleResolver.ElementDeclarations.GetIdentifier("mso-break-type").ToLowerInvariant();
            string msoColumnBreakBefore = mStyleResolver.ElementDeclarations.GetIdentifier("mso-column-break-before").ToLowerInvariant();

            BreakType breakType;
            if (msoBreakType == "section-break")
            {
                // It's a section break. Currently only new page and continuous are supported.
                switch (pageBreakBefore)
                {
                    case "always":
                        breakType = BreakType.SectionBreakNewPage;
                        break;
                    case "left":
                        breakType = BreakType.SectionBreakEvenPage;
                        break;
                    case "right":
                        breakType = BreakType.SectionBreakOddPage;
                        break;
                    default:
                        // It's either auto or avoid, let's see mso-column-break-before.
                        breakType = (msoColumnBreakBefore == "always")
                            ? BreakType.SectionBreakNewColumn
                            : BreakType.SectionBreakContinuous;
                        break;
                }
            }
            else
            {
                // It's either page break or line break or column break.
                switch (pageBreakBefore)
                {
                    case "always":
                        breakType = BreakType.PageBreak;
                        break;
                    default:
                        breakType = (msoColumnBreakBefore == "always")
                            ? BreakType.ColumnBreak
                            : BreakType.LineBreak;
                        break;
                }
            }

            switch (breakType)
            {
                case BreakType.LineBreak:
                {
                    // WORDSNET-2562 In HTML, <br> elements not always insert a line break. For example, the last <br>
                    // in a block is ignored by browsers.
                    if (IsLineBreakAppropriate(node))
                    {
                        // Line break is an inline entity, it belongs to a paragraph. So we need to start it.
                        mParagraphArranger.StartParaIfNeeded();

                        // WORDSNET-15597 Enable CompatibilityOptions.DoNotExpandShiftReturn option
                        // allows the justified text that contains 'br' after import in MSW to look like in the browser.
                        // If html is inserted with help DocumentBuilder.InsertHtml() we shouldn't change this option.
                        if (mApplyDocumentWideFormatting && (mBuilder.ParagraphFormat.Alignment == ParagraphAlignment.Justify))
                        {
                            mBuilder.Document.CompatibilityOptions.DoNotExpandShiftReturn = true;
                        }

                        // The new break is inserted into a separate run and we need to apply its formatting appropriately.
                        ApplyFontFormatting(true);

                        // We're manually creating the line break run, because it will get additional formatting that should not
                        // propagate onto next runs or onto the paragraph break character.
                        Run lineBreakRun = new Run(mBuilder.Document, ControlChar.LineBreak, mBuilder.GetRunPrCopy());

                        // WORDSNET-10824 Provide break's clear behavior.
                        ApplyLineBreakClear(lineBreakRun);

                        mBuilder.InsertNode(lineBreakRun);
                    }
                    else if (IsOnlyBlockNeighbours(node))
                    {
                        // Line break is an inline entity, it belongs to a paragraph. So we need to start it.
                        mParagraphArranger.StartParaIfNeeded();
                        mHtmlBlockReader.MarkParagraphAsEmptyInHtml(mBuilder.CurrentParagraph);
                        if (mStyleResolver.GetLineBreakClear() != LineBreakClear.None)
                        {
                            // The new break is inserted into a separate run and we need to apply its formatting appropriately.
                            ApplyFontFormatting(true);

                            // We're manually creating the line break run, because it will get additional formatting that
                            // should not propagate onto next runs or onto the paragraph break character.
                            Run lineBreakRun = new Run(mBuilder.Document, ControlChar.LineBreak, mBuilder.GetRunPrCopy());

                            // WORDSNET-10824 Provide break's clear behavior.
                            ApplyLineBreakClear(lineBreakRun);

                            // WORDSNET-10825 We must provide break's clear behavior but do not add additional vertical space.
                            // Here is a simple workaround. We add break with minimum allowed font size.
                            // Visually this is almost unnoticeable.
                            // FOSS: WordUtil.MinFontSize (= 0.5pt, MS Word minimum allowed font size) was removed.
                            int minFontSizeHalfPoints = ConvertUtilCore.PointToHalfPoint(0.5);
                            lineBreakRun.RunPr.Size = minFontSizeHalfPoints;
                            lineBreakRun.RunPr.SizeBi = minFontSizeHalfPoints;

                            mBuilder.InsertNode(lineBreakRun);
                        }
                        else
                        {
                            mParagraphArranger.InsertParagraphImmediately();
                        }
                    }
                    else if (IsNullOrInsignificantWhitespace(node.NextSibling) &&
                        IsNullOrInsignificantWhitespace(node.PreviousSibling))
                    {
                        // If a block-level element (paragraph) contains nothing but one <br>, browsers handle the <br> element
                        // not as a line break but rather as a placeholder that prevents the block from collapsing. For example,
                        // "<p><br></p>" creates a uncollapsed empty paragraph in browsers.
                        mParagraphArranger.MarkCurrentParagraphAsEmptyInHtml();
                        mHtmlBlockReader.MarkParagraphAsEmptyInHtml(mBuilder.CurrentParagraph);
                    }
                    break;
                }
                case BreakType.PageBreak:
                case BreakType.ColumnBreak:
                {
                    // WORDSNET-9492 If a break is outside any paragraph, it will be inserted to
                    // the paragraph which follows it.
                    mParagraphArranger.StartParaIfNeeded();

                    // The new break is inserted into a separate run and we need to apply its formatting appropriately.
                    ApplyFontFormatting(true);

                    mBuilder.InsertBreakCore(breakType, false);
                    break;
                }
                case BreakType.SectionBreakNewPage:
                {
                    // WORDSNET-17504 Keeps settings for the previous paragraph and the paragraph itself to restore it
                    // after break inserting.
                    ParaPr paraPr = mBuilder.GetParaPrCopy();
                    Paragraph paragraph = mBuilder.CurrentParagraph;

                    // If a break is outside any paragraph, it will be inserted to
                    // the paragraph which follows it.
                    mParagraphArranger.StartParaIfNeeded();
                    mBuilder.InsertBreakCore(breakType, false);

                    paragraph.ParaPr = paraPr;
                    break;
                }
                default:
                {
                    // Any node can follow the break. There is no need to start another paragraph. Just reset.
                    mParagraphArranger.IsParaStartNeeded = false;
                    mBuilder.InsertBreakCore(breakType, false);
                    break;
                }
            }
            mHtmlBlockReader.UpdateCurrentParagraph(false);
        }

        private void ApplyLineBreakClear(Run lineBreakRun)
        {
            LineBreakClear lineBreakClear = mStyleResolver.GetLineBreakClear();
            if (lineBreakClear != LineBreakClear.None)
            {
                lineBreakRun.RunPr.LineBreakClear = lineBreakClear;
            }
        }

        /// <summary>
        /// Determines whether current node is between two block nodes.
        /// </summary>
        /// <remarks>
        /// Needed to properly create empty paragraphs in places of lonely line breaks. Probably we can do this better
        /// but there is no evident way.
        /// This should be also recursive since we can have a sequence like this: [span][br][/span] (rare case).
        /// See WORDSNET-3386.
        /// </remarks>
        private bool IsOnlyBlockNeighbours(HtmlNode node)
        {
            // Are there any non-block nodes under the same parent near the current one?
            HtmlNode next = node;
            while ((next = next.NextSibling) != null)
            {
                if (IsAnythingInline(next))
                    return false;
                if (next is HtmlElementNode)
                    break;
            }

            HtmlNode prev = node;
            while ((prev = prev.PreviousSibling) != null)
            {
                if (IsAnythingInline(prev))
                    return false;
                if (prev is HtmlElementNode)
                    break;
            }

            // <br> elements are processed as breaks only if there is some content to "break". In other words, there must be
            // anything before and/or after the <br>.
            return (next != null) || (prev != null);
        }

        private bool IsAnythingInline(HtmlNode node)
        {
            if (node is HtmlTextNode)
            {
                return ContainsMeaningfulText((HtmlTextNode)node);
            }
            else if (node is HtmlElementNode)
            {
                HtmlElementNode elementNode = (HtmlElementNode)node;
                // WORDSNET-22838 All children of the inline element should be inline too.
                return IsInlineElement(elementNode) && AllChildrenAreInline(elementNode);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether line break is appropriate at place of inserting this node.
        /// </summary>
        private bool IsLineBreakAppropriate(HtmlElementNode node)
        {
            // Are there any more sibling elements under the same parent?
            HtmlNode next = node;
            while ((next = next.NextSibling) != null)
            {
                if (next is HtmlElementNode)
                {
                    HtmlElementNode nextElementNode = (HtmlElementNode)next;
                    // <style> and <title> are not visible by default ("display:none") and have no effect on line breaks.
                    // Note that this is a quick fix. We'd better analyze actual "display" values to handle other cases where
                    // elements get invisible.
                    string name = nextElementNode.Name;
                    if ((name == "style") || (name == "title"))
                        continue;

                    // Any inline element at the same level should be separated.
                    // WORDSNET-22838 All children of the inline element should be inline too.
                    if (IsInlineElement(nextElementNode) && AllChildrenAreInline(nextElementNode))
                    {
                        return true;
                    }

                    // Non-inline element occurred first. Should see in the parent.
                    break;
                }

                // Any non-empty text should be also separated.
                if ((next is HtmlTextNode) && ContainsMeaningfulText((HtmlTextNode)next))
                {
                    return true;
                }
            }

            // The last break at the the end of a block (including "inline-block") is ignored in browsers.
            HtmlElementNode parentNode = node.Parent;
            if ((parentNode == null) ||
                !IsInlineElement(parentNode) ||
                (mStyleResolver.ParentElementDisplayType() == CssDisplayType.InlineBlock))
            {
                return false;
            }

            // For inline elements recurse to the parent.
            return IsLineBreakAppropriate(parentNode);
        }

        /// <summary>
        /// Determines whether the node is <c>null</c> or is an insignificant whitespace node (is not a meaningful text node).
        /// </summary>
        private bool IsNullOrInsignificantWhitespace(HtmlNode node)
        {
            return (node == null) ||
                ((node is HtmlTextNode) && !ContainsMeaningfulText((HtmlTextNode)node));
        }

        /// <summary>
        /// Checks whether it's an inline element.
        /// Queries from the HtmlCssReader and defaults to static list. Maybe we'll simplify this after deploying default style sheets.
        /// </summary>
        private bool IsInlineElement(HtmlElementNode node)
        {
            return !mElementCategorizer.IsBlockLevelElement(node, mCssRules);
        }

        /// <summary>
        /// Checks whether all children of the specified node are inline.
        /// </summary>
        private bool AllChildrenAreInline(HtmlElementNode node)
        {
            foreach (HtmlNode child in node.Children)
            {
                HtmlElementNode childAsElement = child as HtmlElementNode;
                if (childAsElement == null)
                {
                    continue;
                }

                if (!IsInlineElement(childAsElement) || !AllChildrenAreInline(childAsElement))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Special processing for single and double BR elements at end of block-level elements. Mimics MS Word's behavior.
        /// </summary>
        private bool HandleBreaksAtBlockEnd(HtmlElementNode currentBreakNode)
        {
            if (mStyleResolver.ParentElementDisplayType() == CssDisplayType.Inline)
                return false;

            // The following code recognizes both <br><br> pairs and single <br> elements. In the latter case leading and
            // trailing nodes will reference the same HTML node.
            HtmlNode leadingBreak = IsBreak(currentBreakNode.PreviousSibling)
                ? currentBreakNode.PreviousSibling
                : currentBreakNode;
            HtmlNode trailingBreak = IsBreak(leadingBreak.NextSibling)
                ? leadingBreak.NextSibling
                : currentBreakNode;

            // This method doesn't support the case where a block contains nothing but breaks.
            if (leadingBreak.PreviousSibling == null)
            {
                return false;
            }

            // Breaks must occur at the end of the block.
            return trailingBreak.NextSibling == null;
        }

        private static bool IsBreak(HtmlNode node)
        {
            if (node == null)
            {
                return false;
            }
            HtmlElementNode nodeAsElement = node as HtmlElementNode;
            return (nodeAsElement != null) && (nodeAsElement.Name == "br");
        }

        /// <summary>
        /// Handles ordered or unordered native list
        /// </summary>
        private void HandleList(bool isStart)
        {
            if (isStart)
            {
                CreateListReaderIfNeeded();

                CurrentListReader.BeginLevel(mStyleResolver.CurrentElement.ElementName);

                if (IsImplicitListDisplayType(mStyleResolver.ElementDisplayType))
                    return;

                // WORDSNET-12225 Html to Docx conversion issue with display : inline style.
                // In case a list contains inline elements they must fit a separate paragraph.
                mParagraphArranger.EndPara();
            }
            else
            {
                // Its presence should be guaranteed by parser
                Debug.Assert(CurrentListReader != null);
                CurrentListReader.EndLevel();
            }
        }

        private void HandleListItem(HtmlElementNode node, bool isStart)
        {
            if (isStart)
            {
                // Usually, ::before pseudo-elements are processed (and corresponding CSS counters are updated) after
                // the main HTML element. However, ::before pseudo-elements on <li> elements are treated as list item
                // markers and in order to import them correctly we need to know values of CSS counters earlier,
                // when processing the main <li> element. We switch to ::before and immediately back to the main element
                // so that the counters will be updated.
                mStyleResolver.SwitchToPart(HtmlElementPart.Before, true);
                mStyleResolver.SwitchToPart(HtmlElementPart.Element, false);

                CreateListReaderIfNeeded();

                mParagraphArranger.StartPara();

                // Start of bookmark for the list. The end of bookmark will be added during handling the end of the list.
                if (!CurrentListReader.CurrentListHasListItems)
                    mHtmlHyperlinkResolver.AddPendingStartBookmarkIfNeeded(node.Parent);

                mHtmlBlockReader.StartHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mStyleResolver.ElementDisplayType);

                CurrentListReader.BeginItem(node);

                ApplyParagraphFormatting();
                mCommonBorderResolver.CollectParagraphWithBorder(mBuilder.CurrentParagraph, mStyleResolver.ElementDeclarations);

                // The list item may be a HTML block if 'div' or 'blockquote' element has `display:list-item` CSS property
                // or the list item may be inside already created the HTML block.
                mHtmlBlockReader.UpdateCurrentParagraph(true);

                mHtmlHyperlinkResolver.StartInlineBookmarkIfNeeded(node);
            }
            else
            {
                mHtmlHyperlinkResolver.EndInlineBookmarkIfNeeded(node);

                mParagraphArranger.EndPara();
                Debug.Assert(CurrentListReader != null);

                CurrentListReader.EndItem();

                mHtmlBlockReader.EndHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mBidiTextArranger);
            }
        }

        private void CreateListReaderIfNeeded()
        {
            if (CurrentListReader == null)
            {
                // WORDSNET-12935 In UseBuilderFormatting mode we don't want to override default positions of list item
                // markers.
                bool adjustMarkerPositions = !mUseDocumentBuilderFormatting;

                mListReaderManager.CreateListReader(
                    mBuilder,
                    mStyleResolver,
                    mResourceLoader,
                    mBaseUri,
                    mBaseParagraphLeftIndent,
                    adjustMarkerPositions);
            }
        }

        /// <summary>
        /// Handles inline-level HTML elements like SPAN, B, I, U and others.
        /// </summary>
        private HandleNodeAction HandleSpan(HtmlElementNode node, bool isStart)
        {
            // '-aw-import: ignore' spans shouldn't be imported back.
            CssDeclaration importBehaviorDeclaration = mStyleResolver.ElementDeclarations[HtmlConstants.Import];

            if ((importBehaviorDeclaration != null) && importBehaviorDeclaration.Value.Equals(CssValue.Ignore))
            {
                // We also use '-aw-import: ignore' spans containing just "&nbsp;" to prevent empty paragraphs
                // from collapsing in HTML browsers.
                if (node.GetInnerText() == ControlChar.NonBreakingSpace)
                {
                    mParagraphArranger.MarkCurrentParagraphAsEmptyInHtml();

                    // WORDSNET-21377 MS Word applies element's font style to an empty paragraph. Mimic this behavior.
                    // The font style should be applied if the span contains any other CSS besides "-aw-import:ignore".
                    if (mStyleResolver.ElementDeclarations.Count > 1)
                    {
                        mStyleResolver.HandleSpanStart();
                        mStyleResolver.ToFont(mBuilder.Font, mBuilder.CurrentParagraph.ParagraphStyle);
                        mStyleResolver.HandleSpanEnd();
                    }
                }

                // All other such spans are simply ignored.
                return HandleNodeAction.HandledSkipChildren;
            }

            if (HandleStructuredDocumentTagNode(node, isStart))
                return HandleNodeAction.HandledTraverseChildren;

            if (isStart)
            {
                // Sometimes a 'div' element has other display type.
                bool htmlBlockStarted = mHtmlBlockReader.StartHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mStyleResolver.ElementDisplayType);

                // WORDSNET-21063 Update only an empty paragraph to prevent setting the HTML block
                // for a preceding paragraph.
                if (htmlBlockStarted && (mBuilder.CurrentParagraph.Count == 0))
                    mHtmlBlockReader.UpdateCurrentParagraph(true);

                // Insert a bookmark if the span doesn't contain meaningful text (empty).
                if (!node.ContainsText())
                {
                    mHtmlHyperlinkResolver.StartInlineBookmarkIfNeeded(node);
                    mHtmlHyperlinkResolver.EndInlineBookmarkIfNeeded(node);
                }

                if (HandleBookmarkSpan(node))
                    return HandleNodeAction.HandledSkipChildren;
                if (HandleCommentSpan())
                    return HandleNodeAction.HandledSkipChildren;
                if (HandleTabEmulationSpan(node))
                    return HandleNodeAction.HandledSkipChildren;
                if (HandleFieldSpan())
                    return HandleNodeAction.HandledSkipChildren;

                mStyleResolver.HandleSpanStart();

                mFixedWidthSpanReader.HandleSpanStart(node);
            }
            else
            {
                // If needed, pad the current fixed-width span with space characters up to its desired width.
                ApplyFontFormatting(true);
                string padding = mFixedWidthSpanReader.HandleSpanEnd(node, mBuilder.Font);
                if (StringUtil.HasChars(padding))
                {
                    WriteText(padding);
                    // There may be enclosing fixed-width spans that are still opened, so we take padding width into account.
                    mFixedWidthSpanReader.ProcessText(padding, mBuilder.Font);
                }

                HandleDirectionMarkSpan(node);
                mStyleResolver.HandleSpanEnd();

                mHtmlBlockReader.EndHtmlBlock(
                    mStyleResolver.CurrentElement.ElementName,
                    mStyleResolver.ElementDeclarations,
                    mBidiTextArranger);
            }
            return HandleNodeAction.HandledTraverseChildren;
        }

        private bool HandleStructuredDocumentTagNode(HtmlElementNode node, bool isStart)
        {
            CssDeclaration tagDeclaration = mStyleResolver.ElementDeclarations[HtmlConstants.SdtTag];
            if (tagDeclaration == null)
                return false;

            mBidiTextArranger.RearrangeAndWriteText();

            if (isStart)
            {
                StructuredDocumentTag sdt = (node.Name == "span")
                    ? new StructuredDocumentTag(mBuilder.Document, SdtType.PlainText, MarkupLevel.Inline)
                    : new StructuredDocumentTag(mBuilder.Document, SdtType.RichText, MarkupLevel.Block);
                sdt.Tag = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.SdtTag);
                sdt.Title = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.SdtTitle);
                string sdtContent = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.SdtContent);
                sdt.IsShowingPlaceholderText = sdtContent == HtmlConstants.SdtContentPlaceholder;
                sdt.GetChildNodes(NodeType.Any, false).Clear();

                // WORDSNET-20553 Move all bookmarks before inline SDT to parent paragraph.
                // If current paragraph contains nodes with another types then bookmarks are not moved.
                if (mBuilder.CurrentParagraph.HasChildNodes)
                {
                    bool possibleToMove = true;
                    foreach (Node childNode in mBuilder.CurrentParagraph.GetChildNodes(NodeType.Any, false))
                    {
                        if ((childNode.NodeType != NodeType.BookmarkStart) &&
                           (childNode.NodeType != NodeType.BookmarkEnd))
                        {
                            possibleToMove = false;
                            break;
                        }
                    }
                    if (possibleToMove)
                    {
                        while (mBuilder.CurrentParagraph.HasChildNodes)
                        {
                            Node firstChild = mBuilder.CurrentParagraph.FirstChild;
                            firstChild.Remove();
                            mBuilder.CurrentParagraph.InsertPrevious(firstChild);
                        }
                    }
                }

                if (sdt.Level == MarkupLevel.Inline)
                {
                    Node firstNode = sdt.AppendChild(new Run(mBuilder.Document));
                    mBuilder.InsertNode(sdt);
                    mParagraphArranger.RememberCurrentParagraphAndMoveTo(firstNode);
                }
                else
                {
                    mParagraphArranger.StartParaIfNeeded();
                    sdt.AppendChild(new Paragraph(mBuilder.Document));
                    if (!mBuilder.IsAtStartOfParagraph)
                    {
                        Debug.Assert(mBidiTextArranger.IsEmpty);
                        mParagraphArranger.InsertParagraphImmediately();
                    }
                    mBuilder.CurrentParagraph.InsertPrevious(sdt);
                    mParagraphArranger.RememberCurrentParagraphAndMoveTo(sdt);
                }
            }
            else
            {
                // WORDSNET-20553 Move all bookmarks inside inline SDT to parent paragraph.
                StructuredDocumentTag sdt = mBuilder.CurrentParagraph.FirstChild as StructuredDocumentTag;
                if (sdt != null)
                {
                    while (sdt.HasChildNodes)
                    {
                        Node firstChild = sdt.FirstChild;
                        if ((firstChild.NodeType != NodeType.BookmarkStart) &&
                               (firstChild.NodeType != NodeType.BookmarkEnd))
                        {
                            break;
                        }
                        firstChild.Remove();
                        mBuilder.CurrentParagraph.InsertPrevious(firstChild);
                    }
                }

                // WORDSNET-20553 Move all bookmarks from the first SDT's paragraph to SDT's parent node.
                sdt = mBuilder.CurrentParagraph.ParentNode as StructuredDocumentTag;
                if ((sdt != null) &&
                    (sdt.FirstChild != null) &&
                    (sdt.FirstChild.NodeType == NodeType.Paragraph))
                {
                    Paragraph firstParagraph = (Paragraph)sdt.FirstChild;
                    while (firstParagraph.HasChildNodes)
                    {
                        Node firstChild = firstParagraph.FirstChild;
                        if ((firstChild.NodeType != NodeType.BookmarkStart) &&
                               (firstChild.NodeType != NodeType.BookmarkEnd))
                        {
                            break;
                        }
                        firstChild.Remove();
                        sdt.InsertPrevious(firstChild);
                    }
                }

                // WORDSNET-20108 Set the language of a whole structured document tag to 'HebrewIsrael' if its first run
                // is in 'HebrewIsrael'.
                UpdateSdtNodeBidiLocale();

                mParagraphArranger.RestoreCurrentParagraphIfNeeded();
            }

            return true;
        }

        private void HandleDirectionMarkSpan(HtmlElementNode node)
        {
            // WORDSNET-12119 A quirk exists in all modern browsers (Chrome, Firefox, IE) related to processing of
            // explicit embedding levels in the Unicode bidirectional algorithm (UBA). According to the HTML specification,
            // an empty span with 'dir' attribute should be treated as a LRE or RLE character that is immediately followed
            // by a PDF character. For example the following span
            //    <span dir=rtl></span>
            // is treated as a sequence RLE+PDF, which has no effect on surrounding text, according to UBA. However, this span
            // does affect surrounding text in modern browsers.
            // The exact way in which browsers process such spans is unclear and no official documentation exists regarding
            // that quirk (at least, it is hard to find). However, our tests show that browsers' behavior can be reproduced
            // if such spans are converted to Unicode direction marks (LRM or RLM) before passing them to UBA.
            // In other words, we treat
            //   <span dir=ltr></span>  as LRE+LRM+PDF
            //   <span dir=rtl></span>  as RLE+RLM+PDF
            // Note that this applies not only to spans but to any inline elements too.
            if ((mStyleResolver.ElementDeclarations.GetIdentifier("unicode-bidi") == "embed") && (!node.ContainsText()))
            {
                WriteDirectionMark();
            }
        }

        /// <summary>
        /// Handles span that is a bookmark's start or bookmark's end mark written by HTML Writer.
        /// </summary>
        /// <returns><c>true</c> if the span node has been processed successfully; <c>false</c> otherwise.</returns>
        private bool HandleBookmarkSpan(HtmlElementNode node)
        {
            bool parentNodeIsParagraph = (node.Parent != null) && (node.Parent.Name == "p");

            string bookmarkName = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.BookmarkStart);
            if (bookmarkName != string.Empty)
            {
                mHtmlHyperlinkResolver.StartAnchorBookmarkIfNeeded(bookmarkName, parentNodeIsParagraph, true);
                return true;
            }

            bookmarkName = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.BookmarkEnd);
            if (bookmarkName != string.Empty)
            {
                mHtmlHyperlinkResolver.EndAnchorBookmarkIfNeeded(bookmarkName, parentNodeIsParagraph, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles span that is a comment's start or comment's end mark written by HTML Writer.
        /// </summary>
        /// <returns><c>true</c> if the span node has been processed successfully; <c>false</c> otherwise.</returns>
        private bool HandleCommentSpan()
        {
            return HandleCommentStartSpan() || HandleCommentEndSpan();
        }

        /// <summary>
        /// Handles span that is a comment's start mark written by HTML Writer.
        /// </summary>
        /// <returns><c>true</c> if the span node has been processed successfully; <c>false</c> otherwise.</returns>
        private bool HandleCommentStartSpan()
        {
            string commentStartName = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.CommentStart);
            if (commentStartName == string.Empty)
            {
                return false;
            }

            int commentId = FormatterPal.TryParseInt(commentStartName.Substring(HtmlConstants.CommentReferenceIdPrefix.Length));
            if (commentId == int.MinValue)
            {
                Warn(WarningType.DataLoss,
                    string.Format(
                        "Can't parse comment id from '{0}' in span element. " +
                        "Positive integer is expected after prefix '{1}'. " +
                        "Original comment's start position possibly has been lost.",
                        commentStartName, HtmlConstants.CommentReferenceIdPrefix));
            }
            else
            {
                if (!mComments.ContainsKey(commentId))
                {
                    // Comment boundaries are hard to preserve during reordering by the Unicode bidirectional algorithm (UBA),
                    // so at the moment comment contents are isolated from the UBA to make sure it is reordered as a whole.
                    mBidiTextArranger.RearrangeAndWriteText();
                    StartComment(commentId);
                }
            }

            return true;
        }

        /// <summary>
        /// Handles span that is a comment's end mark written by HTML Writer.
        /// </summary>
        /// <returns><c>true</c> if the span node has been processed successfully; <c>false</c> otherwise.</returns>
        private bool HandleCommentEndSpan()
        {
            string commentEndName = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.CommentEnd);
            if (commentEndName == string.Empty)
            {
                return false;
            }

            int commentId = FormatterPal.TryParseInt(commentEndName.Substring(HtmlConstants.CommentReferenceIdPrefix.Length));
            if (commentId == int.MinValue)
            {
                Warn(WarningType.DataLoss,
                    string.Format
                    ("Can't parse comment id from '{0}' in span element. " +
                    "Positive integer is expected after prefix '{1}'. " +
                    "Original comment's end position possibly has been lost.",
                    commentEndName, HtmlConstants.CommentReferenceIdPrefix));
            }
            else
            {
                if (mComments.ContainsKey(commentId))
                {
                    // Comment boundaries are hard to preserve during reordering by the Unicode bidirectional algorithm (UBA),
                    // so at the moment comment contents are isolated from the UBA to make sure it is reordered as a whole.
                    mBidiTextArranger.RearrangeAndWriteText();

                    EndComment(commentId);
                    mFullyProcessedComments.Add(commentId);
                }
            }

            return true;
        }

        /// <summary>
        /// Handles span that is a tab stop emulation written by HTML Writer.
        /// </summary>
        /// <returns><c>true</c> if the span node has been processed successfully; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Examine HtmlSpanWriter.WriteTabStop(Font) (removed from FOSS build) to see how we represent tab stops in HTML.
        /// </remarks>
        private bool HandleTabEmulationSpan(HtmlElementNode element)
        {
            string firstChildText = ((element.Children.Count == 1) && (element.Children[0] is HtmlTextNode))
                ? ((HtmlTextNode)element.Children[0]).Text
                : string.Empty;

            // WORDSNET-17074 Previously, we used a fixed whitespace string to represent tabstops.
            if (firstChildText == HtmlConstants.TabStopReplacement)
            {
                WriteText("\t");
                return true;
            }

            CssDeclaration displayDeclaration = mStyleResolver.ElementDeclarations["display"];
            if ((displayDeclaration == null) || !displayDeclaration.Value.Equals(CssValue.InlineBlock))
                return false;

            double width = mStyleResolver.ElementDeclarations.GetLength("width");
            if (width < 0)
                return false;

            // WORDSNET-22645 Sometimes customers edit HTML generated by AW and write arbitrary text right into
            // tab stop emulation spans. Because that text is then gets visible in browsers, we decided to add the following
            // check. If the fill text of a tab stop emulation span contains anything but tab stop fill characters used by our
            // HTML writer, the text must have been edited manually and we should process the span as normal text instead of
            // a tab stop replacement.
            // Note that leading and trailing new line characters are also ignored, so if text of a tab emulation span is
            // formatted, it is still recognized as tab stop.
            char[] tabStopFillCharactersUsedByHtmlWriter = new char[] { ' ', '\u00A0', '.', '-', '_', '·', '\n' };
            string trimmedFirstChildText = firstChildText.Trim(tabStopFillCharactersUsedByHtmlWriter);
            if (StringUtil.HasChars(trimmedFirstChildText))
            {
                return false;
            }

            string tabStopAlignValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.TabStopAlignment);
            double tabStopPosition = mStyleResolver.ElementDeclarations.GetLength(HtmlConstants.TabStopPosition);
            if ((tabStopAlignValue != string.Empty) && (tabStopPosition > 0))
            {
                // This span is a tab stop emulation produced by AW HTML Export and it contains a tab stop roundtrip info.
                TabAlignment tabStopAlignment;
                if (HtmlUtil.ParseTapStopAlignment(tabStopAlignValue, out tabStopAlignment))
                {
                    TabLeader tabStopLeader = TabLeader.None;
                    string tabStopLeaderValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.TabStopLeader);
                    if ((tabStopLeaderValue == string.Empty) ||
                        HtmlUtil.ParseTapStopLeader(tabStopLeaderValue, out tabStopLeader))
                    {
                        ParaPr paraPr = mBuilder.CurrentParagraph.ParaPr;
                        if (paraPr.TabStops == null)
                            paraPr.TabStops = new TabStopCollection();
                        paraPr.TabStops.Add(tabStopPosition, tabStopAlignment, tabStopLeader);
                    }
                }
            }
            else if ((firstChildText != ControlChar.NonBreakingSpace) && (element.Children.Count != 0))
            {
                // This span isn't an AW HTML Export tab stop emulation and should be processed as a normal span.
                // Before WORDSNET-14080 we emulated tabs with empty spans. We also support this emulation for backward
                // compatibility.
                return false;
            }

            // WORDSNET-10025 Support HTML round-trip of underline tab characters.
            mStyleResolver.ToFont(mBuilder.Font, mBuilder.CurrentParagraph.ParagraphStyle);
            WriteText("\t");
            return true;
        }

        /// <summary>
        /// Handles span that is a field representation written by HTML Writer.
        /// </summary>
        /// <returns><c>true</c> if the span node has been processed successfully; <c>false</c> otherwise.</returns>
        private bool HandleFieldSpan()
        {
            // We don't decide here, which fields are correct and which aren't, we load fields as is.
            // Fields' validation is done during document's post loading actions.

            string fieldStartValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.FieldStart);
            if (fieldStartValue == HtmlConstants.FieldStartTrue)
            {
                mBidiTextArranger.RearrangeAndWriteText();

                FieldStart fieldStart = mBuilder.InsertFieldStart(FieldType.FieldPage);
                mFieldNodesStack.Push(fieldStart);

                return true;
            }

            string fieldCodeValue = mStyleResolver.ElementDeclarations.GetString(HtmlConstants.FieldCode);
            if (StringUtil.HasChars(fieldCodeValue))
            {
                // WORDSNET-12277 If the text buffer contains some text, it is time to write it to the resulting document.
                // Otherwise, text that comes before the field in the source HTML document will be written after the field
                // in the resulting document.
                mBidiTextArranger.RearrangeAndWriteText();

                mBuilder.InsertFieldCode(fieldCodeValue);

                return true;
            }

            string fieldSeparatorValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.FieldSeparator);
            if (fieldSeparatorValue == HtmlConstants.FieldSeparatorTrue)
            {
                mBidiTextArranger.RearrangeAndWriteText();

                FieldSeparator fieldSeparator = mBuilder.InsertFieldSeparator(FieldType.FieldNone);
                mFieldNodesStack.Push(fieldSeparator);

                return true;
            }

            string fieldEndValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.FieldEnd);
            if (fieldEndValue == HtmlConstants.FieldEndTrue)
            {
                mBidiTextArranger.RearrangeAndWriteText();

                FieldBundle field = new FieldBundle();

                field.Separator = (FieldSeparator)mFieldNodesStack.PopIfInstanceOf(typeof(FieldSeparator));
                field.Start = (FieldStart)mFieldNodesStack.PopIfInstanceOf(typeof(FieldStart));
                field.End = mBuilder.InsertFieldEnd(FieldType.FieldNone, field.Separator != null);

                // WORDSNET-15311 MS Word hides field result for fields with FieldType.None, when displaying DOC documents,
                // so must we set actual FieldType for the field.
                if ((field.Start != null) && ((field.Separator != null) || (field.End != null)))
                    field.DetermineFieldType();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles IMG element.
        /// </summary>
        private void HandleImage(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);
            string imageUri = HtmlUtil.ValidateUri(node.Attributes.GetAttributeValue("src", string.Empty));

            // In the mobi format we don't have src but we have recindex instead.
            if (string.IsNullOrEmpty(imageUri))
                imageUri = node.Attributes.GetAttributeValue("recindex", "");

            ProcessImage(imageUri, node);
        }

        /// <summary>
        /// Handles MathML.
        /// </summary>
        private void HandleMathMl(HtmlElementNode element)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            if (mStyleResolver.IsDisplayedAsBlock() || (mStyleResolver.ElementDisplayType == CssDisplayType.RunIn))
            {
                mParagraphArranger.IsParaStartNeeded = true;
            }

            mParagraphArranger.StartParaIfNeeded();

            // FOSS
        }

        /// <summary>
        /// Handles EMBED element.
        /// </summary>
        private void HandleEmbed(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);
            string resourceUri = HtmlUtil.ValidateUri(node.Attributes.GetAttributeValue("src", string.Empty));
            if (IsImageInSupportedFormat(resourceUri))
                ProcessImage(resourceUri, node);
        }

        /// <summary>
        /// Handles OBJECT element.
        /// </summary>
        private HandleNodeAction HandleObject(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);
            string resourceUri = HtmlUtil.ValidateUri(node.Attributes.GetAttributeValue("data", string.Empty));
            if (IsImageInSupportedFormat(resourceUri))
            {
                // WORDSNET-16978 Fallback content of an image <object> element is now processed only if the image
                // is unavailable and cannot be loaded.
                byte[] imageBytes = mResourceLoader.LoadImage(mBaseUri, resourceUri, false);
                if (imageBytes != null)
                {
                    ProcessImage(imageBytes, resourceUri, node);
                    return HandleNodeAction.HandledSkipChildren;
                }
            }

            // The data can't be imported. Import the fallback content instead.
            return HandleNodeAction.HandledTraverseChildren;
        }

        /// <summary>
        /// Handles IFRAME element.
        /// </summary>
        private void HandleIframe(HtmlElementNode node)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);
            string resourceUri = HtmlUtil.ValidateUri(node.Attributes.GetAttributeValue("src", string.Empty));
            if (IsImageInSupportedFormat(resourceUri))
                ProcessImage(resourceUri, node);
        }

        private void HandleTextarea(bool isStart)
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            mParagraphArranger.StartParaIfNeeded();

            if (isStart)
            {
                Shape shape = new Shape(mBuilder.Document, ShapeType.TextBox);
                shape.WrapType = WrapType.Inline;

                // Approximately taken default padding value.
                const double defaultTextareaPadding = 2;
                double paddingTop = mStyleResolver.ElementDeclarations.GetLength("padding-top");
                shape.TextBox.InternalMarginTop = !MathUtil.IsMinValue(paddingTop)
                                                        ? paddingTop
                                                        : defaultTextareaPadding;
                double paddingRight = mStyleResolver.ElementDeclarations.GetLength("padding-right");
                shape.TextBox.InternalMarginRight = !MathUtil.IsMinValue(paddingRight)
                                                            ? paddingRight
                                                            : defaultTextareaPadding;
                double paddingBottom = mStyleResolver.ElementDeclarations.GetLength("padding-bottom");
                shape.TextBox.InternalMarginBottom = !MathUtil.IsMinValue(paddingBottom)
                                                            ? paddingBottom
                                                            : defaultTextareaPadding;
                double paddingLeft = mStyleResolver.ElementDeclarations.GetLength("padding-left");
                shape.TextBox.InternalMarginLeft = !MathUtil.IsMinValue(paddingLeft)
                                                        ? paddingLeft
                                                        : defaultTextareaPadding;

                // Different browsers have different default size for textarea window.
                // So these are approximately taken values.
                const double defaultTextareaWidth = 144;
                const double defaultTextareaHeight = 28;
                double width = mStyleResolver.ElementDeclarations.GetLength("width");
                shape.Width = !MathUtil.AreEqual(width, double.MinValue) ? width : defaultTextareaWidth;
                double height = mStyleResolver.ElementDeclarations.GetLength("height");
                shape.Height = !MathUtil.AreEqual(height, double.MinValue) ? height : defaultTextareaHeight;

                // WORDSNET-14876 Remove border from textarea if "border:none" or "border:hidden" style is specified.
                if (IsBorderExplicitlyHidden("border-top-style") && IsBorderExplicitlyHidden("border-right-style") &&
                    IsBorderExplicitlyHidden("border-bottom-style") && IsBorderExplicitlyHidden("border-left-style"))
                {
                    shape.Stroked = false;
                }

                Paragraph para = new Paragraph(mBuilder.Document);
                shape.AppendChild(para);

                mBuilder.InsertNode(shape);
                mParagraphArranger.RememberCurrentParagraphAndMoveTo(para);
            }
            else
            {
                mParagraphArranger.RestoreCurrentParagraphIfNeeded();
            }
        }

        private bool IsBorderExplicitlyHidden(string borderStylePropertyName)
        {
            CssDeclaration borderStyleDeclaration = mStyleResolver.ElementDeclarations[borderStylePropertyName];
            return (borderStyleDeclaration != null) &&
                   (borderStyleDeclaration.Value.Equals(CssValue.None) ||
                    borderStyleDeclaration.Value.Equals(CssValue.Hidden));
        }

        private void ProcessImage(string imageUri, HtmlElementNode node)
        {
            byte[] imageBytes = mResourceLoader.LoadImage(mBaseUri, imageUri);
            ProcessImage(imageBytes, imageUri, node);
        }

        private void ProcessImage(byte[] imageBytes, string imageUri, HtmlElementNode element)
        {
            SizeD svgImageSize = null;
            // WORDSNET-7502 If an image is not inline, it must be put into a new paragraph.
            mParagraphArranger.StartParaIfNeeded();

            // Apply font formatting. According to CSS rules, text decoration is not applied to images. However, we apply
            // text decoration to images that are inserted into a target document that has text decoration, if the
            // user explicitly asks us to keep it.
            ApplyFontFormatting(mUseDocumentBuilderFormatting);

            Shape shape;
            if (imageBytes != null)
            {
                // WORDSNET-18329 The exception CantCreateBitmapException occurs when we try to load a malformed image.
                // Replace such image by the red-cross image.
                try
                {
                    // WORDSNET-27384 EXIF attributes related to image orientation should not be considered for MHTML.
                    shape = mBuilder.InsertImage(
                        imageBytes,
                        RelativeHorizontalPosition.Column,
                        0,
                        RelativeVerticalPosition.Paragraph,
                        0,
                        -1,
                        -1,
                        WrapType.Inline,
                        mLoadFormat != LoadFormat.Mhtml);
                }
                catch (CantCreateBitmapException)
                {
                    imageBytes = ImageUtil.GetNoImageBytes();
                    shape = mBuilder.InsertImage(imageBytes);
                }
                Debug.Assert(shape != null);

                // FOSS: ImageUtil.IsNotImageBytes(byte[]) was removed; inline equivalent check.
                byte[] noImageBytesRef = ImageUtil.GetNoImageBytes();
                if (imageBytes.Length == noImageBytesRef.Length &&
                    Aspose.ArrayUtil.CompareBytes(imageBytes, noImageBytesRef, imageBytes.Length))
                    Warn(WarningType.DataLoss, WarningStrings.ImageReplacedWithPlaceholder);

            }
            else
            {
                shape = new Shape(mBuilder.Document, ShapeType.Image);
                shape.WrapType = WrapType.Inline;
                shape.ImageData.SourceFullName = UriUtil.ConstructUnescapedAbsoluteUri(mBaseUri, imageUri);
                mBuilder.InsertNode(shape);
            }

            SetShapeSize(shape, imageBytes, svgImageSize);

            // Here we should have one of the possibilities:
            // 1. Image created from HtmlResourceLoader
            // 2. Image created from uri directly(for corrupt images)
            // 3. Image with "no-image" picture
            // 4. Image created as linked image

            Debug.Assert(shape != null);

            // WORDSNET-8258 Handle 'alt' attribute.
            string altText = element.Attributes.GetAttributeValue("alt", "");
            shape.SetShapeAttrInternal(ShapeAttr.ShapeDescription, altText);
            // WORDSNET-5710 While converting html to doc, image properties not available.
            if (element.Attributes["title"] != null)
            {
                string titleText = element.Attributes.GetAttributeValue("title", "");
                shape.SetShapeAttrInternal(ShapeAttr.ImageTitle, titleText);
            }

            // WORDSNET-21511 Remove border to prevent double border around an image.
            // Image borders are applied to image borders on VML shapes or to stroke on DML shapes. However, when the shape
            // is inserted, from font properties of the document builder are copied to RunPr of the shape, including borders.
            // As a result, the shape ends up with duplicate border: one from shape properties and another from font properties.
            // Since it is uneasy to not apply font borders or to not copy them to shape's RunPr, we decided to remove already
            // applied font borders from shapes afterwards. This fix doesn't look pretty and we might want to improve it later
            // but the results it produces look close to what is rendered in HTML browsers.
            if (shape.RunPr.Border.IsVisible)
                shape.RunPr.Remove(FontAttr.Border);

            mStyleResolver.ToShape(shape);

            // WORDSNET-10097 Shape becomes shifted down after DOCX-HTMl-DOCX round-trip.
            string wrapTypeValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.ImageWrapType);
            WrapType wrapType;
            if ((wrapTypeValue != string.Empty) && HtmlUtil.ParseWrapType(wrapTypeValue, out wrapType))
            {
                shape.WrapType = wrapType;
                string relHPosValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.ImageRelativeHPos);
                RelativeHorizontalPosition relHPos;
                if ((relHPosValue != string.Empty) && HtmlUtil.ParseRelativeHorizontalPosition(relHPosValue, out relHPos))
                {
                    shape.RelativeHorizontalPosition = relHPos;
                    // WORDSNET-28986 Since shape alignment has higher priority than its relative position, we need to reset
                    // its value to default.
                    shape.HorizontalAlignment = HorizontalAlignment.Default;
                }
                string relVPosValue = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.ImageRelativeVPos);
                RelativeVerticalPosition relVPos;
                if ((relVPosValue != string.Empty) && HtmlUtil.ParseRelativeVerticalPosition(relVPosValue, out relVPos))
                {
                    shape.RelativeVerticalPosition = relVPos;
                    // WORDSNET-28986 Since shape alignment has higher priority than its relative position, we need to reset
                    // its value to default.
                    shape.VerticalAlignment = VerticalAlignment.Default;
                }
                double topValue = mStyleResolver.ElementDeclarations.GetLength(HtmlConstants.ImageTopPos);
                if (!MathUtil.IsMinValue(topValue))
                    shape.Top = topValue;
                double leftValue = mStyleResolver.ElementDeclarations.GetLength(HtmlConstants.ImageLeftPos);
                if (!MathUtil.IsMinValue(leftValue))
                    shape.Left = leftValue;
            }

            // WORDSNET-10651 Floating images are not affected by hyperlink fields. To make floating images hyperlinked
            // we must set the href of the shape. To find the hyperlink address, here we check only the parent element
            // of the image. Although it is enough for WORDSNET-10651, a more general solution would check all ancestors.
            if (!shape.IsInline)
            {
                HtmlElementNode parent = element.Parent;
                if ((parent != null) && (parent.Name == "a"))
                {
                    string href = HtmlUtil.ValidateUri(parent.Attributes.GetAttributeValue("href", string.Empty));
                    if (StringUtil.HasChars(href))
                    {
                        shape.HRef = href;
                    }
                }
            }

            // WORDSNET-16021 Image inside a table cell is cut if para line spacing equals to zero
            // and spacing rule is Multiple.
            if (mBuilder.CurrentParagraph.IsInCell &&
                MathUtil.IsZero(mBuilder.CurrentParagraph.ParagraphFormat.LineSpacing) &&
                (mBuilder.CurrentParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Multiple))
            {
                // Change line spacing to 'Single'.
                mBuilder.CurrentParagraph.ParagraphFormat.LineSpacing = 12;
            }
        }

        private void SetShapeSize(
            Shape shape,
            byte[] imageBytes,
            SizeD svgImageSize)
        {
            //Case in which there is no picture but there is its size and placeholder.
            if (imageBytes == null)
            {
                HtmlImageSizeCalculator size = new HtmlImageSizeCalculator(
                    null,
                    null,
                    mStyleResolver.ElementDeclarations);
                if ((size.CssWidth > 0) || (size.CssHeight > 0))
                {
                    shape.SetSizeSafe(size.CssWidth, size.CssHeight);
                }
                return;
            }

            ImageSizeCore originalSizeInfo = ImageUtil.GetImageSize(imageBytes);
            SizeD originalSize = svgImageSize != null ? svgImageSize : ImageSizeToSizeD(originalSizeInfo);

            SizeD containerSize = GetContainerSize(shape);
            if (containerSize == null)
            {
                containerSize = ShapeSizeValidationHelper.GetContainerSize(
                    shape,
                    (Paragraph)shape.GetAncestor(NodeType.Paragraph));
            }

            HtmlImageSizeCalculator imageSizeCalculator = new HtmlImageSizeCalculator(
                originalSize,
                containerSize,
                mStyleResolver.ElementDeclarations);
            CssDisplayType parentDisplayType = mStyleResolver.ParentElementDisplayType();
            SizeD imageSize = imageSizeCalculator.Calculate(parentDisplayType);

            if ((imageSize != null) && imageSizeCalculator.HasAnyCssSizeProperties())
            {
                shape.SetSizeSafe(imageSize.Width, imageSize.Height);
            }
            // WORDSNET-23606 MS Word always use the original size of the image during import the alt-chunk document.
            else if (mSettings.ApplyFormattingAsMsWord)
            {
                if ((originalSize != null) &&
                    (originalSizeInfo != null) &&
                    (!MathUtil.AreEqual(originalSize.Width, shape.Width, 1.0) ||
                    !MathUtil.AreEqual(originalSize.Height, shape.Height, 1.0)))
                {
                    shape.SetSizeSafe(originalSizeInfo.WidthPoints, originalSizeInfo.HeightPoints);
                }
            }
            // WORDSNET-9671 Images with non-standard resolution (other than 96 dpi) have incorrect size after
            // they are imported from HTML. This happens because DocumentBuilder and HTML browsers use different resolutions
            // for such images. DocumentBuilder takes the actual resolution from image files, and HTML browsers always
            // use the standard (96 dpi) resolution. Here we resize images with non-standard resolution to make them
            // the same size as they are in HTML browsers.
            else if ((originalSizeInfo != null) &&
                (!MathUtil.AreEqual(originalSizeInfo.HorizontalResolution, ImageConstants.StandardResolution, 1.0) ||
                !MathUtil.AreEqual(originalSizeInfo.VerticalResolution, ImageConstants.StandardResolution, 1.0)))
            {
                shape.SetSizeSafe(ConvertUtilCore.PixelToPoint(originalSizeInfo.Width, ImageConstants.StandardResolution),
                    ConvertUtilCore.PixelToPoint(originalSizeInfo.Height, ImageConstants.StandardResolution));
            }
        }

        private static SizeD GetContainerSize(Shape shape)
        {
            Paragraph parentParagraph = shape.ParentNode as Paragraph;
            if (parentParagraph != null)
            {
                Cell parentCell = parentParagraph.ParentCell;
                if (parentCell != null)
                {
                    ParagraphFormat paragraphFormat = parentParagraph.ParagraphFormat;
                    double paragraphIndents = paragraphFormat.LeftIndent + paragraphFormat.RightIndent;

                    CellFormat cellFormat = parentCell.CellFormat;
                    PreferredWidth cellWidth = cellFormat.PreferredWidth;
                    if (cellWidth.IsFixed)
                    {
                        return new SizeD(cellWidth.Value - paragraphIndents, ConvertUtilCore.MaxSizePoint);
                    }
                }
            }

            return null;
        }

        private static SizeD ImageSizeToSizeD(ImageSizeCore originalSize)
        {
            if (originalSize == null)
                return null;

            return new SizeD(
                ConvertUtil.PixelToPoint(originalSize.Width, ImageConstants.StandardResolution),
                ConvertUtil.PixelToPoint(originalSize.Height, ImageConstants.StandardResolution));
        }

        private bool IsImageInSupportedFormat(string resourceUri)
        {
            FileFormat fileFormat = FileFormatCore.FromExt(UriUtil.GetExtension(mBaseUri, resourceUri));
            if (fileFormat == FileFormat.Unknown)
                return false;

            string contentType = FileFormatCore.ToContentType(fileFormat);
            return contentType.StartsWith("image/", StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads a horizontal rule. Accepts both HTML and CSS attributes.
        /// </summary>
        private void HandleHorizontalRule(HtmlElementNode node)
        {
            if (HandleFootnoteEndnoteHorizontalRule(node))
                return;

            // WORDSNET-18289 Temporarily removes an HR element from CssStyleTracker to prevent element style applying
            // to an inserted paragraph.
            mStyleResolver.PopElement();

            // HR always starts a new paragraph and requires another new paragraph to be started after it.
            mParagraphArranger.StartParaIfNeeded();

            // WORDSNET-18289 Adds an HR element to CssStyleTracker after new paragraph adding.
            // We re-process the element and have already updated CSS counters on it earlier.
            mStyleResolver.PushElement(node, false);

            Shape shape = Shape.CreateHorizontalRule(mBuilder.Document);
            mBuilder.InsertNode(shape);
            mStyleResolver.ToHorizontalRule(shape);
            mParagraphArranger.IsParaStartNeeded = true;
        }

        /// <summary>
        /// Handles horizontal rule span that is a footnote or endnote delimiter rule written by HTML Writer.
        /// </summary>
        /// <param name="node"></param>
        /// <returns><c>true</c> if the horizontal rule node has been processed successfully; <c>false</c> otherwise.</returns>
        private bool HandleFootnoteEndnoteHorizontalRule(HtmlElementNode node)
        {
            CssDeclarationCollection nodeDeclarations = mStyleResolver.ElementDeclarations;
            // This style is used for any footnote or endnote horizontal rule.
            if (!(MathUtil.AreEqual(nodeDeclarations.GetPercentage("width"), 33) &&
                MathUtil.AreEqual(nodeDeclarations.GetLength("height"), 0.75) &&
                (nodeDeclarations.GetIdentifier("text-align") == "left")))
            {
                return false;
            }

            int footnoteType = -1;
            int numberStyle = -1;
            int startNumber = -1;

            // Try to read footnote\endnote roundtrip information saved in new -aw-* format.
            double footnoteTypeValue = nodeDeclarations.GetNumber(HtmlConstants.FootnoteType);
            if (!MathUtil.IsMinValue(footnoteTypeValue))
            {
                footnoteType = (int)footnoteTypeValue;
                double numberStyleValue = nodeDeclarations.GetNumber(HtmlConstants.FootnoteNumberStyle);
                if (!MathUtil.IsMinValue(numberStyleValue))
                {
                    numberStyle = (int)numberStyleValue;
                    double startNumberValue = nodeDeclarations.GetNumber(HtmlConstants.FootnoteStartNumber);
                    if (!MathUtil.IsMinValue(startNumberValue))
                        startNumber = (int)startNumberValue;
                }
            }

            // Try to read footnote\endnote roundtrip information saved in old data-* format.
            if ((footnoteType == -1) && (numberStyle == -1) && (startNumber == -1))
            {
                footnoteType = node.Attributes.GetAttributeValue(HtmlConstants.FootnoteTypeAttribute, -1);
                if (footnoteType != -1)
                {
                    numberStyle = node.Attributes.GetAttributeValue(HtmlConstants.FootnoteNumberStyleAttribute, -1);
                    if (numberStyle != -1)
                        startNumber = node.Attributes.GetAttributeValue(HtmlConstants.FootnoteStartNumberAttribute, -1);
                }
            }

            if ((footnoteType != -1) && (numberStyle != -1) && (startNumber != -1))
            {
                switch (footnoteType)
                {
                    case (int)FootnoteType.Footnote:
                    {
                        mBuilder.Document.FootnoteOptions.NumberStyle = (NumberStyle)numberStyle;
                        mBuilder.Document.FootnoteOptions.StartNumber = startNumber;
                        break;
                    }
                    case (int)FootnoteType.Endnote:
                    {
                        mBuilder.Document.EndnoteOptions.NumberStyle = (NumberStyle)numberStyle;
                        mBuilder.Document.EndnoteOptions.StartNumber = startNumber;
                        break;
                    }
                    default:
                    {
                        // Ignore unknown values.
                        break;
                    }
                }
            }
            return true;
        }

        private void WarnIfDocumentIsFixedPageHtml()
        {
            // WORDSNET-25035 Detect if we're loading a fixed-page HTML document and warn customers about formatting loss.
            if (mFixedPageFormatDetector.IsFixedPageHtml)
            {
                Warn(WarningType.MajorFormattingLoss,
                    "The document is fixed-page HTML. Its structure may not be loaded correctly.");
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, string description)
        {
            if (mBuilder.Document.WarningCallback != null)
                mBuilder.Document.WarningCallback.Warning(new WarningInfo(warningType, WarningSource.Html, description));
        }

        /// <summary>
        /// Determines whether the text node contains anything but ignored whitespaces.
        /// </summary>
        private bool ContainsMeaningfulText(HtmlTextNode textNode)
        {
            if (mStyleResolver.IsPreformatted() || mStyleResolver.IsPreformattedWithWrap())
            {
                Debug.Assert(StringUtil.HasChars(textNode.Text));
                return true;
            }
            bool newlinesAreWhitespaces = !mStyleResolver.IsPreformattedWithLine();
            return HtmlUtil.ContainsAnythingButWhitespaces(textNode.Text, newlinesAreWhitespaces);
        }

        /// <summary>
        /// Remove empty paragraphs at the end of sections, starting from the specified section.
        /// </summary>
        /// <remarks>
        /// The paragraph should be removed if:
        /// - it's the last paragraph in a section;
        /// - it's not the first paragraph after a table;
        /// - it's not explicitly marked empty in HTML using AW-specific CSS properties;
        /// - it's not a section break paragraph;
        /// - it's not a list item;
        /// - it's not the only paragraph in the section.
        /// </remarks>
        private void RemoveEmptyParagraphs(Section firstSection)
        {
            Debug.Assert(firstSection != null);

            Paragraph builderParagraph = mBuilder.CurrentParagraph;
            bool builderParagraphRemoved = false;
            Body lastSectionBody = null;
            Section section = firstSection;
            while (section != null)
            {
                lastSectionBody = section.Body;

                while (true)
                {
                    Paragraph lastParagraph = lastSectionBody.LastChild as Paragraph;
                    if ((lastParagraph == null) || lastParagraph.IsFirstChild)
                    {
                        break;
                    }

                    Node previousNode = lastParagraph.PreviousNonAnnotationSibling;
                    bool isRightAfterTable = (previousNode != null) && (previousNode.NodeType == NodeType.Table);

                    if (lastParagraph.HasChildNodes ||
                        mParagraphArranger.IsParagraphEmptyInHtml(lastParagraph) ||
                        isRightAfterTable ||
                        lastParagraph.IsSectionBreakParagraph ||
                        lastParagraph.IsListItem)
                    {
                        break;
                    }

                    if (lastParagraph == builderParagraph)
                    {
                        builderParagraphRemoved = true;
                    }
                    lastParagraph.Remove();
                }

                section = section.NextSibling as Section;
            }
            // WORDSNET-22736 Removing the last paragraph resets the cursor position of the document builder, so we must move
            // the cursor to the end of the document. This is to make sure that for multi-part documents (MHTML) the next part
            // is imported after the end of the current part.
            if (builderParagraphRemoved)
            {
                // Append a temporary paragraph to the last section and use it as a starting point to find the actual last
                // paragraph of the section. This looks silly but there doesn't seem to exist other efficient and obvious way
                // of doing it.
                Paragraph dummyParagraph = lastSectionBody.AppendParagraph(string.Empty);
                Paragraph lastParagraphOfLastSectionBody = (Paragraph)dummyParagraph.PreviousPreOrderOfType(
                    lastSectionBody,
                    NodeType.Paragraph);
                dummyParagraph.Remove();

                // WORDSNET-27419 SPEED We used to always call DocumentBuilder.MoveToDocumentEnd() here to move the cursor
                // to the end of the document. However, this method proved to be increasingly slow in the scenario where we
                // import a large number of HTML documents appending them one after another in a single document. This is what
                // we do when we load CHM or EPUB. Now we use another technique to quickly find the last paragraph of
                // the document and move the builder's cursor there.
                if (lastParagraphOfLastSectionBody != null)
                {
                    mBuilder.MoveTo(lastParagraphOfLastSectionBody);
                }
                else
                {
                    // If something strange happens and we cannot find a paragraph to move the cursor to, we resort to invoking
                    // DocumentBuilder.MoveToDocumentEnd(). It will be slow but it is able to correctly handle most situations
                    // that may occur.
                    mBuilder.MoveToDocumentEnd();
                }
            }
        }

        private void UpdateSdtNodeBidiLocale()
        {
            // Check if there is a block-level SDT.
            StructuredDocumentTag sdt = mBuilder.CurrentParagraph.ParentNode as StructuredDocumentTag;
            if (sdt == null)
            {
                // Check if there is an inline-level SDT.
                sdt = mBuilder.CurrentParagraph.FirstChild as StructuredDocumentTag;
            }
            if (sdt == null)
            {
                // No SDT node found. Nothing to process.
                return;
            }

            UpdateSdtNodeBidiLocale(sdt);
        }

        private static void UpdateSdtNodeBidiLocale(StructuredDocumentTag sdt)
        {
            if (sdt.ContentsRunPr.GetDirectAttr(FontAttr.LocaleIdBi) != null)
            {
                return;
            }

            Node firstChildOfSdt = sdt.FirstChild;
            if (firstChildOfSdt == null)
            {
                return;
            }

            if (firstChildOfSdt.NodeType == NodeType.StructuredDocumentTag)
            {
                // Process a nested structured document tag.
                StructuredDocumentTag nestedSdt = (StructuredDocumentTag)firstChildOfSdt;
                UpdateSdtNodeBidiLocale(nestedSdt);
                UpdateSdtRunPrBidiLocale(sdt.ContentsRunPr, nestedSdt.ContentsRunPr);
            }
            else if (firstChildOfSdt.NodeType == NodeType.Paragraph)
            {
                // Process a block-level structured document tag.
                // The first run may be in the paragraph or in the nested structured document tag.
                Run firstRun = ((Paragraph)firstChildOfSdt).FirstRun;
                if (firstRun != null)
                {
                    UpdateSdtRunPrBidiLocale(sdt.ContentsRunPr, firstRun.RunPr);
                }
            }
            else if (firstChildOfSdt.NodeType == NodeType.Run)
            {
                // Process an inline-level structured document tag.
                UpdateSdtRunPrBidiLocale(sdt.ContentsRunPr, ((Run)firstChildOfSdt).RunPr);
            }
        }

        private string GetHeaderFooterTypeValue(HtmlElementNode node)
        {
            string headerFooterType = mStyleResolver.ElementDeclarations.GetIdentifier(HtmlConstants.HeaderFooterType);
            if (headerFooterType == string.Empty)
                headerFooterType = node.Attributes.GetAttributeValue(HtmlConstants.HeaderFooterTypeAttribute, string.Empty);

            return headerFooterType;
        }

        /// <summary>
        /// Sets the bidi locale of a SDT to match the bidi locale of the first run inside it.
        /// </summary>
        private static void UpdateSdtRunPrBidiLocale(RunPr sdtRunPr, RunPr referenceRunPr)
        {
            if (!referenceRunPr.Bidi.ToBool())
                return;

            // Always set bidi if the reference run is bidi.
            sdtRunPr.Bidi = AttrBoolEx.True;

            if (referenceRunPr.LocaleIdBi == (int)Language.HebrewIsrael)
            {
                sdtRunPr.LocaleIdBi = referenceRunPr.LocaleIdBi;
            }
        }

        /// <summary>
        /// Returns a value indicating whether an HTML element always separates bidi contexts before and after them.
        /// </summary>
        /// <remarks>
        /// Most inline elements are transparent to bidirectional text reordering. However, some elements always act
        /// as bidi context separators.
        /// </remarks>
        private static bool IsBidiContextSeparator(HtmlElementNode element)
        {
            return (element.Namespace == NrxNamespaces.Vml) ||
                   (Array.IndexOf(gBidiContextSeparatorElements, element.Name) >= 0);
        }

        private static bool TryParseHeaderFooterType(string headerFooterTypeValue, out HeaderFooterType headerFooterType)
        {
            switch (headerFooterTypeValue)
            {
                case HtmlConstants.HeaderFooterTypeHeaderPrimary:
                    headerFooterType = HeaderFooterType.HeaderPrimary;
                    break;
                case HtmlConstants.HeaderFooterTypeHeaderFirst:
                    headerFooterType = HeaderFooterType.HeaderFirst;
                    break;
                case HtmlConstants.HeaderFooterTypeFooterPrimary:
                    headerFooterType = HeaderFooterType.FooterPrimary;
                    break;
                case HtmlConstants.HeaderFooterTypeFooterFirst:
                    headerFooterType = HeaderFooterType.FooterFirst;
                    break;
                default:
                    // Set default value.
                    headerFooterType = HeaderFooterType.HeaderEven;
                    return false;
            }
            return true;
        }

        private HtmlListReader CurrentListReader
        {
            get { return mListReaderManager.CurrentListReader; }
        }

        /// <summary>
        /// Features that are considered supported when loading HTML documents. These features affect parsing of IE conditional
        /// comments.
        /// </summary>
        private readonly Features mSupportedFeatures;

        /// <summary>
        /// Caches all loaded resources and provides access to them which is independent on whether a resource already loaded or not yet.
        /// </summary>
        private readonly HtmlResourceLoader mResourceLoader;

        /// <summary>
        /// Always have value of concrete load format unlike LoadOptions.LoadFormat which can have LoadFormat.Auto value.
        /// </summary>
        private readonly LoadFormat mLoadFormat;

        /// <summary>
        /// The last checked node in method IsLastCharacterSpaceOrBR() for current paragraph.
        /// Used for quick search of last node in paragraph. See WORDSNET-18910.
        /// In the future we can consider checking last added character when creating a run or adding a break.
        /// </summary>
        private Node mLastNodeInCurParagraph;

        /// <summary>
        /// We use this builder to build the document.
        /// </summary>
        private DocumentBuilder mBuilder;

        /// <summary>
        /// Can be given in the constructor or read from HTML. Used to construct full path for images that specify relative path.
        /// </summary>
        private string mBaseUri;

        private int mTableNestingLevel;

        private readonly HtmlListReaderManager mListReaderManager;

        private HtmlFixedWidthSpanReader mFixedWidthSpanReader;

        /// <summary>
        /// Indicates that text of the HTML document flows in visual order
        /// (RTL text is reordered and reversed manually by an author of the document).
        /// </summary>
        private bool mTextIsInVisualOrder;
        private CssResolver mCssResolver;
        private CssStyleTracker mCssStyleTracker;
        private DocumentFormatter mStyleResolver;
        private HtmlElementCategorizer mElementCategorizer;
        private IList<CssStyleRule> mCssRules;
        private readonly HtmlCommonBorderResolver mCommonBorderResolver;
        private bool mIsIgnoreHyperlinkContent;
        private bool mIsFootnoteReferenceText;
        private Footnote mCurrentFootnote;
        private readonly IntToObjDictionary<Comment> mComments;
        private readonly IntToObjDictionary<CommentRangeEnd> mCommentRangeEnds;
        private readonly List<int> mFullyProcessedComments;
        private readonly IntToObjDictionary<Footnote> mFootnotes;
        private readonly IntToObjDictionary<Footnote> mEndnotes;

        /// <summary>
        /// Indicates whether HTML reader is allowed to modify document-wide formatting, such as page setup or existing document
        /// styles.
        /// </summary>
        private bool mApplyDocumentWideFormatting;

        /// <summary>
        /// Indicates that font and paragraph formatting specified in the target document builder should be used as base
        /// formatting for content inserted from HTML.
        /// </summary>
        private bool mUseDocumentBuilderFormatting;

        /// <summary>
        /// Base paragraph formatting to which all paragraph formatting imported from CSS is applied.
        /// If formatting merging is enabled, base formatting is the formatting in the point of a document where an HTML
        /// fragment is being inserted.
        /// If formatting merging is disabled, base formatting is empty.
        /// </summary>
        private ParaPr mBaseParaPr;

        /// <summary>
        /// Left indent of a paragraph where an HTML fragment is being inserted into (in case the reader is being executed
        /// as a result of calling <see cref="DocumentBuilder.InsertHtml(string)"/>. This value is used to correctly indent
        /// imported list items. If we convert a whole HTML document, this indent is zero and doesn't affect the result.
        /// </summary>
        private double mBaseParagraphLeftIndent;

        /// <summary>
        /// Base font formatting to which all font formatting imported from CSS is applied.
        /// If formatting merging is enabled, base formatting is the font formatting in the point of a document where an HTML
        /// fragment is being inserted.
        /// If formatting merging is disabled, base formatting is empty.
        /// </summary>
        private RunPr mBaseRunPr;

        /// <summary>
        /// Buffer for bidi text reordering.
        /// </summary>
        /// <remarks>
        /// Accumulates text of adjacent inline elements, reorders it using the Unicode bidirectional algorithm and writes
        /// reordered text to document builder.
        /// </remarks>
        private HtmlBidiTextArranger mBidiTextArranger;

        /// <summary>
        /// Elements that are not transparent to bidirectional text reordering and separate inline bidi contexts before
        /// and after them.
        /// </summary>
        private static readonly string[] gBidiContextSeparatorElements = new string[]
        {
            // Breaks introduce new paragraphs with respect to the Unicode bidirectional algorithm.
            "br",
            // The following elements should be transparent to bidi reordering when they are inline,
            // but we do not support this yet.
            "input",
            "img",
            "svg",
            "math",
            "object",
            "embed",
            "iframe",
            "select",
            "option",
            "textarea",
            // Anchor boundaries are hard to preserve during reordering by the Unicode bidirectional algorithm (UBA),
            // so at the moment anchor contents are isolated from the UBA to make sure it is reordered as a whole.
            "a",
            // Ruby is bidi separator in IE and MS Word, but seems not in Firefox and Chrome.
            "ruby",
            // There is no counterpart for inline tables in our document model. Tables are always imported as blocks.
            "table",
            // In HTML, 'display:inline' seems to have no effect on table parts. They are always rendered as blocks.
            "caption",
            "th",
            "td"
        };

        /// <summary>
        /// Indicates whether the specified "href" is a valid address for a hyperlink in the source document.
        /// </summary>
        private bool IsValidHyperlinkHref(string href)
        {
            return (mSettings.HyperlinkProcessor == null) ||
                mSettings.HyperlinkProcessor.IsValidHyperlinkHref(href);
        }

        private static bool IsImplicitListDisplayType(CssDisplayType displayType)
        {
            return (displayType == CssDisplayType.Inline) ||
                (displayType == CssDisplayType.TableHeaderGroup) ||
                (displayType == CssDisplayType.TableFooterGroup) ||
                (displayType == CssDisplayType.TableRow) ||
                (displayType == CssDisplayType.TableRowGroup) ||
                (displayType == CssDisplayType.InlineBlock);
        }

        /// <summary>
        /// Indicates whether we have ignored any script nodes yet and warned the user about it.
        /// </summary>
        private bool mIgnoredAnyScriptNodes;

        private bool mPostponedPageBreakAfterAlways;
        private HtmlVmlShapeReader mVmlShapeReader;
        private HtmlRubyReader mRubyReader;
        private HtmlFontFallbackUtil mHtmlFontFallbackUtil;

        /// <summary>
        /// Settings that change certain aspects of the HTML loading process.
        /// </summary>
        private readonly HtmlReaderSettings mSettings;

        /// <summary>
        /// Imports 'input' and 'select' elements as Form Fild/SDT nodes.
        /// </summary>
        private HtmlControlReader mHtmlControlReader;

        /// <summary>
        /// Contains information about 'option' elements required for 'select' element import.
        /// </summary>
        private readonly List<HtmlOptionElementInfo> mOptionElements = new List<HtmlOptionElementInfo>();

        private HtmlParagraphArranger mParagraphArranger;

        /// <summary>
        /// Remembers created field start and separator nodes. Pops on field end.
        /// </summary>
        private readonly Stack<FieldChar> mFieldNodesStack;

        private readonly HtmlBlockReader mHtmlBlockReader;

        private readonly HtmlHyperlinkResolver mHtmlHyperlinkResolver;

        private readonly HtmlFixedPageDetector mFixedPageFormatDetector;
    }
}
