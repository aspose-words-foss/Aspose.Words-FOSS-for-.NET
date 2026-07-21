// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2013 by Alexey Butalov

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Aspose.Drawing;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to create CSS engine classes.
    /// </summary>
    internal class CssClassFactory
    {
        /// <summary>
        /// Constructor. Creates a factory for the Standards mode with no omitted formatting and with black body text color.
        /// </summary>
        internal CssClassFactory()
            : this(CssDocumentMode.Standards)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Constructor. Creates a factory with no omitted formatting and with black body text color.
        /// </summary>
        /// <param name="documentMode">Document mode.</param>
        internal CssClassFactory(CssDocumentMode documentMode)
            : this(documentMode, CssUserAgentFormatting.None, DrColor.Black, false)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="documentMode">Document mode.</param>
        /// <param name="omittedUserAgentFormatting">
        /// Groups of user agent (default) CSS styles that are omitted from style resolution.
        /// </param>
        /// <param name="defaultQuirksBodyColor">
        /// Defines the body color for quirks mode.
        /// </param>
        /// <param name="applyFormattingAsMsWord">
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </param>
        internal CssClassFactory(
            CssDocumentMode documentMode,
            CssUserAgentFormatting omittedUserAgentFormatting,
            DrColor defaultQuirksBodyColor,
            bool applyFormattingAsMsWord)
        {
            mDocumentMode = documentMode;
            mOmittedUserAgentFormatting = omittedUserAgentFormatting;
            mDefaultQuirksBodyColor = defaultQuirksBodyColor;
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        /// <summary>
        /// Creates a new instance of <see cref="CssReader" /> class.
        /// </summary>
        /// <param name="baseUri">Base URI of the document. The base URI is used to resolve relative URI of external CSS documents.</param>
        /// <param name="htmlResourceLoader">HtmlResourceLoader used for loading resources.
        /// This is needed for @import rules when importing from container formats (MHTML).</param>
        internal CssReader CreateCssReader(
            string baseUri,
            HtmlResourceLoader htmlResourceLoader)
        {
            return new CssReader(baseUri, htmlResourceLoader, mDocumentMode, mApplyFormattingAsMsWord);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DocumentFormatter" /> class.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="defaultParagraphIstd">
        /// Index of the style that all paragraph styles imported from CSS will be based on.
        /// </param>
        /// <param name="defaultFontIstd">
        /// Index of the style that all font styles imported from CSS will be based on.
        /// </param>
        /// <param name="cssStyleTracker">Instance of <see cref="CssStyleTracker"/></param>
        /// <param name="useHtmlBlocks">
        /// Indicates whether borders and margins of block-level HTML elements are stored using <see cref="HtmlBlock"/>
        /// nodes, as opposed to storing them on child paragraphs.
        /// </param>
        /// <param name="cssFontFaceProvider">Instance of <see cref="CssFontFaceProvider"/> or <c>null</c>.</param>
        internal DocumentFormatter CreateDocumentFormatter(
            Document document,
            int defaultParagraphIstd,
            int defaultFontIstd,
            CssStyleTracker cssStyleTracker,
            bool useHtmlBlocks,
            CssFontFaceProvider cssFontFaceProvider)
        {
            return new DocumentFormatter(
                document,
                defaultParagraphIstd,
                defaultFontIstd,
                mOmittedUserAgentFormatting,
                cssStyleTracker,
                useHtmlBlocks,
                cssFontFaceProvider);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CssResolver" /> class.
        /// </summary>
        /// <param name="documentRules">CSS rules of a HTML document.</param>
        internal CssResolver CreateCssResolver(ICollection<CssStyleRule> documentRules)
        {
            return new CssResolver(
                documentRules,
                CreateDefaultElementStyleResolver(),
                CreateCascadingOrderResolver(),
                CreateCssInterdependencyResolver(),
                CreateComputedDeclarationResolver(),
                mDocumentMode,
                mDefaultQuirksBodyColor);
        }

        /// <summary>
        /// Creates a new instance of <see cref="HtmlElementCategorizer" /> class.
        /// </summary>
        internal HtmlElementCategorizer CreateHtmlElementCategorizer()
        {
            return new HtmlElementCategorizer(this);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CssCascadingOrderResolver" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Non-static to provide a consistent class interface where all methods are called in the same way.")]
        internal CssCascadingOrderResolver CreateCascadingOrderResolver()
        {
            return new CssCascadingOrderResolver();
        }

        /// <summary>
        /// Creates a new instance of <see cref="CssComputedDeclarationResolver" /> class.
        /// </summary>
        internal CssComputedDeclarationResolver CreateComputedDeclarationResolver()
        {
            return new CssComputedDeclarationResolver(mApplyFormattingAsMsWord);
        }

        internal CssStyleTracker CreateStyleTracker(
            IList<CssStyleRule> cssReaderRules,
            IList<CssPageRule> cssReaderPageRules,
            string htmlElementInlineCssStyle,
            string bodyElementInlineCssStyle,
            CssResolver cssResolver,
            bool applyFormattingAsMsWord,
            bool useHtmlBlocks)
        {
            return new CssStyleTracker(
                cssReaderRules,
                cssReaderPageRules,
                htmlElementInlineCssStyle,
                bodyElementInlineCssStyle,
                cssResolver,
                new HtmlElementCategorizer(this),
                new HtmlAttributeResolver(),
                cssResolver.DefaultElementStyleResolver,
                CreateComputedDeclarationResolver(),
                CreateCssBoxModel(),
                applyFormattingAsMsWord,
                useHtmlBlocks);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultElementStyleResolver" /> class.
        /// </summary>
        internal DefaultElementStyleResolver CreateDefaultElementStyleResolver()
        {
            return new DefaultElementStyleResolver(mDocumentMode, mOmittedUserAgentFormatting, mApplyFormattingAsMsWord);
        }

        /// <summary>
        /// Gets an instance of <see cref="CssClassFactory"/> with <see cref="CssDocumentMode.Standards"/> mode.
        /// </summary>
        internal static CssClassFactory StandardsFactory
        {
            get
            {
                // Double-checked locking pattern.
                if (gStandardsFactory == null)
                {
                    lock (gStandardsFactoryLock)
                    {
                        if (gStandardsFactory == null)
                        {
                            gStandardsFactory = new CssClassFactory(CssDocumentMode.Standards);
                        }
                    }
                }

                return gStandardsFactory;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="CssInterdependencyResolver" /> class.
        /// </summary>
        private CssInterdependencyResolver CreateCssInterdependencyResolver()
        {
            return new CssInterdependencyResolver(mOmittedUserAgentFormatting);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CssBoxModel" /> class.
        /// </summary>
        private CssBoxModel CreateCssBoxModel()
        {
            bool applyDefaultParagraphFormatting = (mOmittedUserAgentFormatting & CssUserAgentFormatting.Paragraph) == 0;
            return new CssBoxModel(
                mDocumentMode,
                applyDefaultParagraphFormatting,
                mApplyFormattingAsMsWord);
        }

        private readonly CssDocumentMode mDocumentMode;

        /// <summary>
        /// Groups of user agent (default) CSS styles that are omitted from style resolution.
        /// </summary>
        private readonly CssUserAgentFormatting mOmittedUserAgentFormatting;

        /// <summary>
        /// Defines the body color for quirks mode.
        /// </summary>
        private readonly DrColor mDefaultQuirksBodyColor;

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </summary>
        private readonly bool mApplyFormattingAsMsWord;

        /// <summary>
        /// Instance of <see cref="CssClassFactory"/> with <see cref="CssDocumentMode.Standards"/> mode.
        /// </summary>
        private static volatile CssClassFactory gStandardsFactory;

        private static readonly object gStandardsFactoryLock = new object();
    }
}
