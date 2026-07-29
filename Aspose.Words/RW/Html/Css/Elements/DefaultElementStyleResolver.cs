// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2017 by Nikolay Sezganov

using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.RW.Html.Css.SvgElementDefs;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Class that resolves default CSS style of an HTML or SVG element according to its name, attributes,
    /// and attributes' values.
    /// </summary>
    internal class DefaultElementStyleResolver
    {
        internal DefaultElementStyleResolver(
            CssDocumentMode documentMode,
            CssUserAgentFormatting omittedFormatting,
            bool applyFormattingAsMsWord)
        {
            mDocumentMode = documentMode;
            mOmittedFormatting = omittedFormatting;
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        /// <summary>
        /// Returns CSS declarations for the default CSS style of the HTML or SVG element.
        /// </summary>
        /// <param name="element">The element whose default style declarations are to be retuned.</param>
        /// <returns>
        /// A collection of CSS declarations applied to the specified element by default.
        /// If the element has default CSS style, an empty collection is returned.
        /// </returns>
        internal CssDeclarationCollection GetDeclarations(IElementProvider element)
        {
            FillElementDefs();

            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();

            if (element.ElementNamespace == W3CNamespaces.Svg)
                mSvgElementDef.ApplyDefaultCss(element, result, mOmittedFormatting);
            else
                GetHtmlElementDef(element).ApplyDefaultCss(element, result, mOmittedFormatting);

            return result.GetDeclarations();
        }

        /// <summary>
        /// Gets the definition of the specified element.
        /// </summary>
        /// <returns>
        /// The definition of the specified element, or the unknown element definition if the element is not supported.
        /// The returned value is never <c>null</c>.
        /// </returns>
        private void FillElementDefs()
        {
            if (mHtmlElementDefs != null)
                return;

            mHtmlElementDefs = new SortedStringListGeneric<HtmlElementDef>(false);
            mHtmlElementDefs.Add("a", new HtmlAElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("abbr", new HtmlAbbrElementDef());
            mHtmlElementDefs.Add("address", new HtmlAddressElementDef());
            mHtmlElementDefs.Add("article", new HtmlArticleElementDef());
            mHtmlElementDefs.Add("aside", new HtmlAsideElementDef());
            mHtmlElementDefs.Add("applet", new HtmlEmbedElementDef());
            mHtmlElementDefs.Add("b", new HtmlBElementDef());
            mHtmlElementDefs.Add("big", new HtmlBigElementDef());
            mHtmlElementDefs.Add("bdo", new HtmlBdoElementDef());
            mHtmlElementDefs.Add("blockquote", new HtmlBlockquoteElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("body", new HtmlBodyElementDef());
            mHtmlElementDefs.Add("br", new HtmlBRElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("caption", new HtmlCaptionElementDef());
            mHtmlElementDefs.Add("center", new HtmlCenterElementDef());
            mHtmlElementDefs.Add("cite", new HtmlCiteElementDef());
            mHtmlElementDefs.Add("code", new HtmlCodeElementDef());
            mHtmlElementDefs.Add("col", new HtmlColElementDef());
            mHtmlElementDefs.Add("colgroup", new HtmlColgroupElementDef());
            mHtmlElementDefs.Add("del", new HtmlDelElementDef());
            mHtmlElementDefs.Add("dfn", new HtmlDfnElementDef());
            mHtmlElementDefs.Add("dir", new HtmlDirElementDef());
            mHtmlElementDefs.Add("div", new HtmlDivElementDef());
            mHtmlElementDefs.Add("dd", new HtmlDdElementDef());
            mHtmlElementDefs.Add("dl", new HtmlDlElementDef());
            mHtmlElementDefs.Add("dt", new HtmlDtElementDef());
            mHtmlElementDefs.Add("em", new HtmlEmElementDef());
            mHtmlElementDefs.Add("embed", new HtmlEmbedElementDef());
            mHtmlElementDefs.Add("figcaption", new HtmlFigcaptionElementDef());
            mHtmlElementDefs.Add("figure", new HtmlFigureElementDef());
            mHtmlElementDefs.Add("font", new HtmlFontElementDef());
            mHtmlElementDefs.Add("footer", new HtmlFooterElementDef());
            mHtmlElementDefs.Add("form", new HtmlFormElementDef(mDocumentMode));
            mHtmlElementDefs.Add("h1", new HtmlH1ElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("h2", new HtmlH2ElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("h3", new HtmlH3ElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("h4", new HtmlH4ElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("h5", new HtmlH5ElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("h6", new HtmlH6ElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("header", new HtmlHeaderElementDef());
            mHtmlElementDefs.Add("hgroup", new HtmlHgroupElementDef());
            mHtmlElementDefs.Add("hr", new HtmlHRElementDef());
            mHtmlElementDefs.Add("html", new HtmlHtmlElementDef());
            mHtmlElementDefs.Add("i", new HtmlIElementDef());
            mHtmlElementDefs.Add("iframe", new HtmlIframeElementDef());
            mHtmlElementDefs.Add("img", new HtmlImgElementDef(mDocumentMode));
            mHtmlElementDefs.Add("input", new HtmlInputElementDef(mDocumentMode));
            mHtmlElementDefs.Add("ins", new HtmlInsElementDef());
            mHtmlElementDefs.Add("kbd", new HtmlKbdElementDef());
            mHtmlElementDefs.Add("legend", new HtmlLegendElementDef());
            mHtmlElementDefs.Add("li", new HtmlLIElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("listing", new HtmlListingElementDef());
            mHtmlElementDefs.Add("mark", new HtmlMarkElementDef());
            mHtmlElementDefs.Add("marquee", new HtmlMarqueeElementDef());
            mHtmlElementDefs.Add("menu", new HtmlMenuElementDef());
            mHtmlElementDefs.Add("nav", new HtmlNavElementDef());
            mHtmlElementDefs.Add("object", new HtmlImgElementDef(mDocumentMode));
            mHtmlElementDefs.Add("ol", new HtmlOLElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("p", new HtmlPElementDef());
            mHtmlElementDefs.Add("plaintext", new HtmlPlaintextElementDef());
            mHtmlElementDefs.Add("pre", new HtmlPreElementDef());
            mHtmlElementDefs.Add("s", new HtmlSElementDef());
            mHtmlElementDefs.Add("samp", new HtmlSampElementDef());
            mHtmlElementDefs.Add("section", new HtmlSectionElementDef());
            mHtmlElementDefs.Add("select", new HtmlSelectElementDef());
            mHtmlElementDefs.Add("small", new HtmlSmallElementDef());
            mHtmlElementDefs.Add("strike", new HtmlStrikeElementDef());
            mHtmlElementDefs.Add("strong", new HtmlStrongElementDef());
            mHtmlElementDefs.Add("sub", new HtmlSubElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("summary", new HtmlSummaryElementDef());
            mHtmlElementDefs.Add("sup", new HtmlSupElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("table", new HtmlTableElementDef(mDocumentMode));
            mHtmlElementDefs.Add("tbody", new HtmlTableSectionElementDef());
            mHtmlElementDefs.Add("td", new HtmlCellElementDef(mDocumentMode));
            mHtmlElementDefs.Add("textarea", new HtmlTextareaElementDef());
            mHtmlElementDefs.Add("tfoot", new HtmlTableSectionElementDef());
            mHtmlElementDefs.Add("th", new HtmlCellElementDef(mDocumentMode));
            mHtmlElementDefs.Add("thead", new HtmlTableSectionElementDef());
            mHtmlElementDefs.Add("tr", new HtmlTRElementDef());
            mHtmlElementDefs.Add("tt", new HtmlTtElementDef());
            mHtmlElementDefs.Add("u", new HtmlUElementDef());
            mHtmlElementDefs.Add("ul", new HtmlULElementDef(mApplyFormattingAsMsWord));
            mHtmlElementDefs.Add("var", new HtmlVarElementDef());
            mHtmlElementDefs.Add("video", new HtmlVideoElementDef());
            mHtmlElementDefs.Add("xmp", new HtmlXmpElementDef());

            mUnknownElementDef = new UnknownElementDef();

            mSvgElementDef = new SvgElementDef();
        }

        private ElementDef GetHtmlElementDef(IElementProvider element)
        {
            ElementDef def = mHtmlElementDefs.GetValueOrNull(element.ElementName);
            return (def != null)
                ? def
                : mUnknownElementDef;
        }

        /// <summary>
        /// Groups of user agent (default) CSS styles that are omitted from style resolution.
        /// </summary>
        private readonly CssUserAgentFormatting mOmittedFormatting;

        /// <summary>
        /// Unknown or unsupported by our CSS engine element definition.
        /// You should not register this class in <see cref="mHtmlElementDefs" />.
        /// </summary>
        private UnknownElementDef mUnknownElementDef;

        /// <summary>
        /// Supported HTML elements list.
        /// </summary>
        private SortedStringListGeneric<HtmlElementDef> mHtmlElementDefs;

        private readonly CssDocumentMode mDocumentMode;

        /// <summary>
        /// Any supported by our CSS engine SVG element definition.
        /// </summary>
        private SvgElementDef mSvgElementDef;

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </summary>
        private readonly bool mApplyFormattingAsMsWord;
    }
}
