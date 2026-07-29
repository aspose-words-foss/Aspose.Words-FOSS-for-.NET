// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/04/2013 by Victor Chebotok

using System;
using Aspose.Common;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Constructs an HTML document tree according to the HTML 5 Specification.
    /// </summary>
    /// <remarks>
    /// This class implements the algorithm described here: http://www.w3.org/TR/html5/syntax.html#tree-construction
    /// </remarks>
    internal class HtmlTreeConstructor
    {
        /// <summary>
        /// Private constructor to simulate static class.
        /// </summary>
        /// <param name="tokenizer">The tokenizer that emits tokens of the document.</param>
        /// <param name="isScriptingEnabled">Indicates whether support for scripts is enabled when parsing HTML.</param>
        /// <param name="supportSelfClosingNonHtmlTags">
        /// Indicates whether to support certain self-closing non-HTML tags.
        /// </param>
        private HtmlTreeConstructor(
            HtmlTokenizer tokenizer,
            bool isScriptingEnabled,
            bool supportSelfClosingNonHtmlTags)
        {
            Debug.Assert(tokenizer != null);

            mTokenizer = tokenizer;
            mIsScriptingEnabled = isScriptingEnabled;
            mSupportSelfClosingNonHtmlTags = supportSelfClosingNonHtmlTags;
        }

        /// <summary>
        /// Parses an HTML document and returns the HTML tree.
        /// </summary>
        /// <param name="tokenizer">The tokenizer that emits tokens of the document.</param>
        /// <param name="isScriptingEnabled">Indicates whether support for scripts is enabled when parsing HTML.</param>
        /// <param name="supportSelfClosingNonHtmlTags">
        /// Indicates whether to support certain self-closing non-HTML tags.
        /// </param>
        /// <returns>The root element of the document's HTML tree.</returns>
        internal static HtmlDocument Construct(
            HtmlTokenizer tokenizer,
            bool isScriptingEnabled,
            bool supportSelfClosingNonHtmlTags)
        {
            HtmlTreeConstructor constructor = new HtmlTreeConstructor(
                tokenizer,
                isScriptingEnabled,
                supportSelfClosingNonHtmlTags);
            constructor.Run();
            return new HtmlDocument(constructor.mRootElement, constructor.mDocumentMode);
        }

        /// <summary>
        /// Processes tokens of the document one by one until the end of file.
        /// </summary>
        private void Run()
        {
            mRootElement = null;
            mHeadElement = null;
            mFormElement = null;
            mOpenedElements.Clear();
            mDocumentMode = HtmlDocumentMode.Standards;
            mFramesetOk = true;
            mRemoveFirstLfFromText = false;

            mInsertionMode = InsertionMode.Initial;
            HtmlToken token;
            do
            {
                bool htmlContent = mOpenedElements.IsEmpty || CurrentElementIsInHtmlNamespace();
                token = mTokenizer.NextToken(htmlContent);

                // WORDSNET-10059 Leading line break character should be removed only if it goes right after a tag
                // that requires such behavior. If there is any other token between the tag and the text with leading line break,
                // the line break should be kept.
                if (token.Type != HtmlTokenType.Text)
                {
                    mRemoveFirstLfFromText = false;
                }

                bool reprocess = true;
                while (reprocess)
                {
                    reprocess = false;
                    switch (token.Type)
                    {
                        case HtmlTokenType.Text:
                            if (IsInHtmlContent(token))
                            {
                                ProcessText((HtmlTextToken)token);
                            }
                            else
                            {
                                ProcessTextInForeignContent((HtmlTextToken)token);
                            }
                            break;
                        case HtmlTokenType.Comment:
                            // Comments are dropped here.
                            break;
                        case HtmlTokenType.Doctype:
                            if (IsInHtmlContent(token))
                            {
                                ProcessDoctype((HtmlDoctypeToken)token);
                            }
                            break;
                        case HtmlTokenType.Tag:
                        {
                            HtmlTagToken tokenAsTag = (HtmlTagToken)token;
                            if (tokenAsTag.IsStart)
                            {
                                if (IsInHtmlContent(token))
                                {
                                    ProcessStartTag(tokenAsTag);
                                }
                                else
                                {
                                    reprocess = ProcessStartTagInForeignContent(tokenAsTag);
                                }
                            }
                            else
                            {
                                if (IsInHtmlContent(token))
                                {
                                    ProcessEndTag(tokenAsTag);
                                }
                                else
                                {
                                    ProcessEndTagInForeignContent(tokenAsTag);
                                }
                            }
                        }
                        break;
                        default:
                            ProcessEndOfFile();
                            break;
                    }
                }
            } while (token.Type != HtmlTokenType.EndOfFile);
        }

        private bool IsInHtmlContent(HtmlToken token)
        {
            // The alogrithm is described here:
            // http://www.w3.org/TR/html5/syntax.html#tree-construction

            if (mOpenedElements.IsEmpty)
            {
                return true;
            }

            if (CurrentElementIsInHtmlNamespace())
            {
                return true;
            }

            if (CurrentElementIsMathMLTextIntegrationPoint())
            {
                HtmlTagToken tokenAsTag = token as HtmlTagToken;
                if ((tokenAsTag != null) &&
                    tokenAsTag.IsStart &&
                    (tokenAsTag.Name != "mglyph") &&
                    (tokenAsTag.Name != "malignmark"))
                {
                    return true;
                }

                if (token.Type == HtmlTokenType.Text)
                {
                    return true;
                }
            }

            HtmlElementNode currentNode = mOpenedElements.GetLast();
            if ((currentNode.Namespace == W3CNamespaces.MathML) && (currentNode.Name == "annotation-xml"))
            {
                HtmlTagToken tokenAsTag = token as HtmlTagToken;
                if ((tokenAsTag != null) && tokenAsTag.IsStart && (tokenAsTag.Name == "svg"))
                {
                    return true;
                }
            }

            if (CurrentElementIsHtmlIntegrationPoint())
            {
                HtmlTagToken tokenAsTag = token as HtmlTagToken;
                if ((tokenAsTag != null) && tokenAsTag.IsStart)
                {
                    return true;
                }

                if (token.Type == HtmlTokenType.Text)
                {
                    return true;
                }
            }

            if (token.Type == HtmlTokenType.EndOfFile)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Processes <see cref="HtmlDoctypeToken"/> tokens.
        /// </summary>
        private void ProcessDoctype(HtmlDoctypeToken doctype)
        {
            if (mInsertionMode != InsertionMode.Initial)
            {
                return;
            }
            mInsertionMode = InsertionMode.BeforeHtml;

            if (doctype.ForceQuirks)
            {
                mDocumentMode = HtmlDocumentMode.Quirks;
                return;
            }

            if (doctype.Name != "html")
            {
                mDocumentMode = HtmlDocumentMode.Quirks;
                return;
            }

            // Although this array contains only constant strings, we decided not to move it to a static field
            // by the following reasons:
            //   a) code is easier to read when the array is declared near the place where it is used
            //   b) performance impact of array recreation is negligible, because this method is called very seldom
            //      (it parses DOCTYPE declarations and a well-formed HTML document contains no more than one such declaration).
            string[] quirkyPublicPrefixes = new string[]
            {
                "+//silmaril//dtd html pro v0r11 19970101//",
                "-//advasoft ltd//dtd html 3.0 aswedit + extensions//",
                "-//as//dtd html 3.0 aswedit + extensions//",
                "-//ietf//dtd html 2.0 level 1//",
                "-//ietf//dtd html 2.0 level 2//",
                "-//ietf//dtd html 2.0 strict level 1//",
                "-//ietf//dtd html 2.0 strict level 2//",
                "-//ietf//dtd html 2.0 strict//",
                "-//ietf//dtd html 2.0//",
                "-//ietf//dtd html 2.1e//",
                "-//ietf//dtd html 3.0//",
                "-//ietf//dtd html 3.2 final//",
                "-//ietf//dtd html 3.2//",
                "-//ietf//dtd html 3//",
                "-//ietf//dtd html level 0//",
                "-//ietf//dtd html level 1//",
                "-//ietf//dtd html level 2//",
                "-//ietf//dtd html level 3//",
                "-//ietf//dtd html strict level 0//",
                "-//ietf//dtd html strict level 1//",
                "-//ietf//dtd html strict level 2//",
                "-//ietf//dtd html strict level 3//",
                "-//ietf//dtd html strict//",
                "-//ietf//dtd html//",
                "-//metrius//dtd metrius presentational//",
                "-//microsoft//dtd internet explorer 2.0 html strict//",
                "-//microsoft//dtd internet explorer 2.0 html//",
                "-//microsoft//dtd internet explorer 2.0 tables//",
                "-//microsoft//dtd internet explorer 3.0 html strict//",
                "-//microsoft//dtd internet explorer 3.0 html//",
                "-//microsoft//dtd internet explorer 3.0 tables//",
                "-//netscape comm. corp.//dtd html//",
                "-//netscape comm. corp.//dtd strict html//",
                "-//o'reilly and associates//dtd html 2.0//",
                "-//o'reilly and associates//dtd html extended 1.0//",
                "-//o'reilly and associates//dtd html extended relaxed 1.0//",
                "-//softquad software//dtd hotmetal pro 6.0::19990601::extensions to html 4.0//",
                "-//softquad//dtd hotmetal pro 4.0::19971010::extensions to html 4.0//",
                "-//spyglass//dtd html 2.0 extended//",
                "-//sq//dtd html 2.0 hotmetal + extensions//",
                "-//sun microsystems corp.//dtd hotjava html//",
                "-//sun microsystems corp.//dtd hotjava strict html//",
                "-//w3c//dtd html 3 1995-03-24//",
                "-//w3c//dtd html 3.2 draft//",
                "-//w3c//dtd html 3.2 final//",
                "-//w3c//dtd html 3.2//",
                "-//w3c//dtd html 3.2s draft//",
                "-//w3c//dtd html 4.0 frameset//",
                "-//w3c//dtd html 4.0 transitional//",
                "-//w3c//dtd html experimental 19960712//",
                "-//w3c//dtd html experimental 970421//",
                "-//w3c//dtd w3 html//",
                "-//w3o//dtd w3 html 3.0//",
                "-//webtechs//dtd mozilla html 2.0//",
                "-//webtechs//dtd mozilla html//"
            };

            if (doctype.PublicId != null)
            {
                for (int i = 0; i < quirkyPublicPrefixes.Length; i++)
                {
                    if (doctype.PublicId.StartsWith(quirkyPublicPrefixes[i], StringComparison.OrdinalIgnoreCase))
                    {
                        mDocumentMode = HtmlDocumentMode.Quirks;
                        return;
                    }
                }
            }

            // Although this array contains only constant strings, we decided not to move it to a static field
            // by the following reasons:
            //   a) code is easier to read when the array is declared near the place where it is used
            //   b) performance impact of array recreation is negligible, because this method is called very seldom
            //      (it parses DOCTYPE declarations and a well-formed HTML document contains no more than one such declaration).
            string[] quirkyPublicIds = new string[]
            {
                "-//w3o//dtd w3 html strict 3.0//en//",
                "-/w3c/dtd html 4.0 transitional/en",
                "html"
            };

            if (doctype.PublicId != null)
            {
                for (int i = 0; i < quirkyPublicIds.Length; i++)
                {
                    if (doctype.PublicId.Equals(quirkyPublicIds[i], StringComparison.OrdinalIgnoreCase))
                    {
                        mDocumentMode = HtmlDocumentMode.Quirks;
                        return;
                    }
                }
            }

            if (doctype.SystemId == "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd")
            {
                mDocumentMode = HtmlDocumentMode.Quirks;
                return;
            }

            if ((doctype.SystemId == null) && (doctype.PublicId != null))
            {
                if (doctype.PublicId.StartsWith("-//w3c//dtd html 4.01 frameset//", StringComparison.OrdinalIgnoreCase) ||
                    doctype.PublicId.StartsWith("-//w3c//dtd html 4.01 transitional//", StringComparison.OrdinalIgnoreCase))
                {
                    mDocumentMode = HtmlDocumentMode.Quirks;
                    return;
                }
            }

            if (doctype.PublicId != null)
            {
                if (doctype.PublicId.StartsWith("-//w3c//dtd xhtml 1.0 frameset//", StringComparison.OrdinalIgnoreCase) ||
                    doctype.PublicId.StartsWith("-//w3c//dtd xhtml 1.0 transitional//", StringComparison.OrdinalIgnoreCase))
                {
                    mDocumentMode = HtmlDocumentMode.LimitedQuirks;
                    return;
                }
            }

            if ((doctype.SystemId != null) && (doctype.PublicId != null))
            {
                if (doctype.PublicId.StartsWith("-//w3c//dtd html 4.01 frameset//", StringComparison.OrdinalIgnoreCase) ||
                    doctype.PublicId.StartsWith("-//w3c//dtd html 4.01 transitional//", StringComparison.OrdinalIgnoreCase))
                {
                    mDocumentMode = HtmlDocumentMode.LimitedQuirks;
                    return;
                }
            }

            mDocumentMode = HtmlDocumentMode.Standards;
        }

        /// <summary>
        /// Processes start <see cref="HtmlTagToken"/> tokens.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppUseAlternativeSwitch]
        private void ProcessStartTag(HtmlTagToken tag)
        {
            bool reprocessToken = true;
            while (reprocessToken)
            {
                reprocessToken = false;

                switch (mInsertionMode)
                {
                    case InsertionMode.Initial:
                        // If the source of the HTML document is the 'srcdoc' attribute of an IFRAME element, the document should be
                        // switched to the Standards mode. But the IFRAME 'srcdoc' attribute is not supported now.
                        mDocumentMode = HtmlDocumentMode.Quirks;
                        mInsertionMode = InsertionMode.BeforeHtml;
                        reprocessToken = true;
                        break;
                    case InsertionMode.BeforeHtml:
                        if (tag.Name == "html")
                        {
                            mRootElement = CreateElement("html", W3CNamespaces.Xhtml, tag.Attributes, true);

                            // Get namespace prefix of VML elements. If VML namespace is declared, we will import VML content.
                            foreach (HtmlAttribute attribute in mRootElement.Attributes)
                            {
                                if (attribute.Value == NrxNamespaces.Vml)
                                {
                                    const string xmlnsPrefix = "xmlns:";
                                    if (attribute.Name.StartsWith(xmlnsPrefix, StringComparison.Ordinal))
                                    {
                                        string vmlNamespacePrefix = attribute.Name.Substring(xmlnsPrefix.Length);
                                        if (StringUtil.HasChars(vmlNamespacePrefix) && (vmlNamespacePrefix.IndexOf(':') < 0))
                                        {
                                            mVmlElementPrefix = vmlNamespacePrefix + ":";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            mRootElement = CreateElement("html", W3CNamespaces.Xhtml, new HtmlAttributeCollection(), true);
                            reprocessToken = true;
                        }
                        mInsertionMode = InsertionMode.BeforeHead;
                        break;
                    case InsertionMode.BeforeHead:
                        switch (tag.Name)
                        {
                            case "html":
                                AddAttributesToHtmlElement(tag.Attributes);
                                break;
                            case "head":
                                mHeadElement = InsertHtmlElement("head", tag.Attributes, true, false);
                                mInsertionMode = InsertionMode.InHead;
                                break;
                            default:
                                mHeadElement = InsertHtmlElement("head", new HtmlAttributeCollection(), true, false);
                                mInsertionMode = InsertionMode.InHead;
                                reprocessToken = true;
                                break;
                        }
                        break;
                    case InsertionMode.InHead:
                        reprocessToken = ProcessStartTagInHead(tag, false);
                        break;
                    case InsertionMode.InHeadNoscript:
                        switch (tag.Name)
                        {
                            case "html":
                                AddAttributesToHtmlElement(tag.Attributes);
                                break;
                            case "basefont":
                            case "bgsound":
                            case "link":
                                InsertHtmlElement(tag.Name, tag.Attributes, false, false);
                                break;
                            case "meta":
                                InsertHtmlElement(tag.Name, tag.Attributes, false, false);
                                // The charset information from META elements is ignored, because we use our own methods
                                // to determine the encoding of the HTML document.
                                break;
                            case "noframes":
                            case "style":
                                InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                                ParseRawText();
                                break;
                            case "head":
                            case "noscript":
                                // Ignore the token.
                                break;
                            default:
                                mOpenedElements.RemoveLast();
                                mInsertionMode = InsertionMode.InHead;
                                reprocessToken = true;
                                break;
                        }
                        break;
                    case InsertionMode.AfterHead:
                        switch (tag.Name)
                        {
                            case "html":
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                            case "body":
                                InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                                mFramesetOk = false;
                                mInsertionMode = InsertionMode.InBody;
                                break;
                            case "frameset":
                                InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                                mInsertionMode = InsertionMode.InFrameset;
                                break;
                            case "base":
                            case "basefont":
                            case "bgsound":
                            case "link":
                            case "meta":
                            case "noframes":
                            case "script":
                            case "style":
                            case "title":
                                mOpenedElements.Add(mHeadElement);
                                reprocessToken = ProcessStartTagInHead(tag, false);
                                mOpenedElements.Remove(mHeadElement);
                                break;
                            case "head":
                                // Ignore the token.
                                break;
                            default:
                                InsertHtmlElement("body", new HtmlAttributeCollection(), true, false);
                                mInsertionMode = InsertionMode.InBody;
                                reprocessToken = true;
                                break;
                        }
                        break;
                    case InsertionMode.InBody:
                        reprocessToken = ProcessStartTagInBody(tag, false);
                        break;
                    case InsertionMode.Text:
                        // Ignore the token.
                        break;
                    case InsertionMode.InTable:
                        reprocessToken = ProcessStartTagInTable(tag);
                        break;
                    case InsertionMode.InTableText:
                        InsertPendingCharacters();
                        reprocessToken = true;
                        break;
                    case InsertionMode.InCaption:
                        switch (tag.Name)
                        {
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "tbody":
                            case "td":
                            case "tfoot":
                            case "th":
                            case "thead":
                            case "tr":
                                if (HasElementInTableScope("caption", W3CNamespaces.Xhtml))
                                {
                                    GenerateImpliedEndTags();
                                    PopElementsUntil("caption");
                                    mActiveFormattingElements.CloseCurrentScope();
                                    mInsertionMode = InsertionMode.InTable;
                                    reprocessToken = true;
                                }
                                break;
                            default:
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                        }
                        break;
                    case InsertionMode.InColumnGroup:
                        switch (tag.Name)
                        {
                            case "html":
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                            case "col":
                                InsertHtmlElement(tag.Name, tag.Attributes, false, false);
                                break;
                            default:
                                if (!CurrentElementIsRootHtml())
                                {
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTable;
                                    reprocessToken = true;
                                }
                                break;
                        }
                        break;
                    case InsertionMode.InTableBody:
                        switch (tag.Name)
                        {
                            case "tr":
                                ClearStackBackToTableBodyContext();
                                InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                                mInsertionMode = InsertionMode.InRow;
                                break;
                            case "th":
                            case "td":
                                ClearStackBackToTableBodyContext();
                                InsertHtmlElement("tr", new HtmlAttributeCollection(), true, false);
                                mInsertionMode = InsertionMode.InRow;
                                reprocessToken = true;
                                break;
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "tbody":
                            case "tfoot":
                            case "thead":
                                if (HasTableBodyElementInTableScope())
                                {
                                    ClearStackBackToTableBodyContext();
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTable;
                                    reprocessToken = true;
                                }
                                break;
                            default:
                                reprocessToken = ProcessStartTagInTable(tag);
                                break;
                        }
                        break;
                    case InsertionMode.InRow:
                        switch (tag.Name)
                        {
                            case "th":
                            case "td":
                                ClearStackBackToRowContext();
                                InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                                mInsertionMode = InsertionMode.InCell;
                                mActiveFormattingElements.OpenNewScope();
                                break;
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "tbody":
                            case "tfoot":
                            case "thead":
                            case "tr":
                                if (HasElementInTableScope("tr", W3CNamespaces.Xhtml))
                                {
                                    ClearStackBackToRowContext();
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTableBody;
                                    reprocessToken = true;
                                }
                                break;
                            default:
                                reprocessToken = ProcessStartTagInTable(tag);
                                break;
                        }
                        break;
                    case InsertionMode.InCell:
                        switch (tag.Name)
                        {
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "tbody":
                            case "td":
                            case "tfoot":
                            case "th":
                            case "thead":
                            case "tr":
                                if (HasTableCellElementInTableScope())
                                {
                                    GenerateImpliedEndTags();
                                    PopElementsUntilCell();
                                    mActiveFormattingElements.CloseCurrentScope();
                                    mInsertionMode = InsertionMode.InRow;
                                    reprocessToken = true;
                                }
                                break;
                            default:
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                        }
                        break;
                    case InsertionMode.InSelect:
                        reprocessToken = ProcessStartTagInSelect(tag);
                        break;
                    case InsertionMode.InSelectInTable:
                        switch (tag.Name)
                        {
                            case "caption":
                            case "table":
                            case "tbody":
                            case "tfoot":
                            case "thead":
                            case "tr":
                            case "td":
                            case "th":
                                if (HasElementInSelectScope("select", W3CNamespaces.Xhtml))
                                {
                                    PopElementsUntil("select");
                                    ResetInsertionModeAppropriately();
                                }
                                reprocessToken = true;
                                break;
                            default:
                                reprocessToken = ProcessStartTagInSelect(tag);
                                break;
                        }
                        break;
                    case InsertionMode.AfterBody:
                    case InsertionMode.AfterAfterBody:
                        switch (tag.Name)
                        {
                            case "html":
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                            default:
                                mInsertionMode = InsertionMode.InBody;
                                reprocessToken = true;
                                break;
                        }
                        break;
                    case InsertionMode.InFrameset:
                        switch (tag.Name)
                        {
                            case "html":
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                            case "frameset":
                                InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                                break;
                            case "frame":
                                InsertHtmlElement(tag.Name, tag.Attributes, false, false);
                                break;
                            case "noframes":
                                reprocessToken = ProcessStartTagInHead(tag, false);
                                break;
                            default:
                                // Ignore the token.
                                break;
                        }
                        break;
                    case InsertionMode.AfterFrameset:
                        switch (tag.Name)
                        {
                            case "html":
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                            case "noframes":
                                reprocessToken = ProcessStartTagInHead(tag, false);
                                break;
                            default:
                                // Ignore the token.
                                break;
                        }
                        break;
                    case InsertionMode.AfterAfterFrameset:
                        switch (tag.Name)
                        {
                            case "html":
                                reprocessToken = ProcessStartTagInBody(tag, false);
                                break;
                            case "noframes":
                                reprocessToken = ProcessStartTagInHead(tag, false);
                                break;
                            default:
                                // Ignore the token.
                                break;
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Unknown insertion mode.");
                }
            }
        }

        /// <summary>
        /// Processes start <see cref="HtmlTagToken"/> tokens in the 'in head' insertion mode.
        /// </summary>
        private bool ProcessStartTagInHead(HtmlTagToken tag, bool fosterParent)
        {
            bool reprocessToken = false;
            switch (tag.Name)
            {
                case "html":
                    AddAttributesToHtmlElement(tag.Attributes);
                    break;
                case "base":
                case "basefont":
                case "bgsound":
                case "command":
                case "link":
                    InsertHtmlElement(tag.Name, tag.Attributes, false, fosterParent);
                    break;
                case "meta":
                    InsertHtmlElement(tag.Name, tag.Attributes, false, fosterParent);
                    // The charset information from META elements is ignored, because we use our own methods to determine
                    // the encoding of the HTML document.
                    break;
                case "title":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    ParseRcdata();
                    break;
                case "noscript":
                    // WORDSNET-21203 Skip any tags inside &lt;noscript&gt; if IgnoreNoScriptTag option was set.
                    if (mIsScriptingEnabled)
                    {
                        InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                        ParseRawText();
                    }
                    else
                    {
                        InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                        mInsertionMode = InsertionMode.InHeadNoscript;
                    }
                    break;
                case "noframes":
                case "style":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    ParseRawText();
                    break;
                case "script":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    ParseScriptData();
                    break;
                case "head":
                    // Ignore the token.
                    break;
                default:
                    mOpenedElements.RemoveLast();
                    mInsertionMode = InsertionMode.AfterHead;
                    reprocessToken = true;
                    break;
            }
            return reprocessToken;
        }

        /// <summary>
        /// Processes start <see cref="HtmlTagToken"/> tokens in the 'in select' insertion mode.
        /// </summary>
        private bool ProcessStartTagInSelect(HtmlTagToken tag)
        {
            bool reprocessToken = false;
            switch (tag.Name)
            {
                case "html":
                    reprocessToken = ProcessStartTagInBody(tag, false);
                    break;
                case "option":
                    if (CurrentElementIsOption())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    break;
                case "optgroup":
                    if (CurrentElementIsOption())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    if (CurrentElementIsOptgroup())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    break;
                case "select":
                    if (HasElementInSelectScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        PopElementsUntil("select");
                        ResetInsertionModeAppropriately();
                    }
                    break;
                case "input":
                case "keygen":
                case "textarea":
                    if (HasElementInSelectScope("select", W3CNamespaces.Xhtml))
                    {
                        PopElementsUntil("select");
                        ResetInsertionModeAppropriately();
                        reprocessToken = true;
                    }
                    break;
                case "script":
                    reprocessToken = ProcessStartTagInHead(tag, false);
                    break;
                default:
                    // Ignore the token.
                    break;
            }
            return reprocessToken;
        }

        /// <summary>
        /// Processes start <see cref="HtmlTagToken"/> tokens in the 'in table' insertion mode.
        /// </summary>
        private bool ProcessStartTagInTable(HtmlTagToken tag)
        {
            bool reprocessToken = false;
            switch (tag.Name)
            {
                case "caption":
                    ClearStackBackToTableContext();
                    mActiveFormattingElements.OpenNewScope();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    mInsertionMode = InsertionMode.InCaption;
                    break;
                case "colgroup":
                    ClearStackBackToTableContext();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    mInsertionMode = InsertionMode.InColumnGroup;
                    break;
                case "col":
                    ClearStackBackToTableContext();
                    InsertHtmlElement("colgroup", new HtmlAttributeCollection(), true, false);
                    mInsertionMode = InsertionMode.InColumnGroup;
                    reprocessToken = true;
                    break;
                case "tbody":
                case "tfoot":
                case "thead":
                    ClearStackBackToTableContext();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    mInsertionMode = InsertionMode.InTableBody;
                    break;
                case "td":
                case "th":
                case "tr":
                    ClearStackBackToTableContext();
                    InsertHtmlElement("tbody", new HtmlAttributeCollection(), true, false);
                    mInsertionMode = InsertionMode.InTableBody;
                    reprocessToken = true;
                    break;
                case "table":
                    if (HasElementInTableScope("table", W3CNamespaces.Xhtml))
                    {
                        PopElementsUntil("table");
                        ResetInsertionModeAppropriately();
                        reprocessToken = true;
                    }
                    break;
                case "style":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    ParseRawText();
                    break;
                case "script":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, false);
                    ParseScriptData();
                    break;
                case "input":
                {
                    string type = tag.Attributes.GetAttributeValue("type", string.Empty);
                    if (!string.Equals(type, "hidden", StringComparison.OrdinalIgnoreCase))
                    {
                        reprocessToken = ProcessStartTagInBody(tag, true);
                    }
                    else
                    {
                        InsertHtmlElement(tag.Name, tag.Attributes, false, false);
                    }
                    break;
                }
                case "form":
                    if (mFormElement == null)
                    {
                        mFormElement = InsertHtmlElement(tag.Name, tag.Attributes, false, false);
                    }
                    break;
                default:
                    reprocessToken = ProcessStartTagInBody(tag, true);
                    break;
            }
            return reprocessToken;
        }

        /// <summary>
        /// Processes start <see cref="HtmlTagToken"/> tokens in the 'in body' insertion mode.
        /// </summary>
        /// <param name="tag">The token being processed.</param>
        /// <param name="fosterParent">
        /// Insert new elements into the foster parent element instead of the current element,
        /// when the current element is a part of a table.
        /// </param>
        private bool ProcessStartTagInBody(HtmlTagToken tag, bool fosterParent)
        {
            bool reprocessToken = false;

            // Check if the current tag starts with the VML prefix.
            string vmlElementName = GetLocalVmlName(tag.Name);
            if (vmlElementName != tag.Name)
            {
                ReconstructActiveFormattingElements(fosterParent);
                InsertElement(tag.Name, NrxNamespaces.Vml, tag.Attributes, !tag.IsSelfClosing, fosterParent);
                return false;
            }

            switch (tag.Name)
            {
                case "html":
                    AddAttributesToHtmlElement(tag.Attributes);
                    break;
                case "base":
                case "basefont":
                case "bgsound":
                case "command":
                case "link":
                case "meta":
                case "noframes":
                case "script":
                case "style":
                case "title":
                    reprocessToken = ProcessStartTagInHead(tag, fosterParent);
                    break;
                case "body":
                    if (CheckNameOfOpenedElement("body", W3CNamespaces.Xhtml, 1))
                    {
                        mFramesetOk = false;
                        AddAttributesToOpenedElement(tag.Attributes, 1);
                    }
                    break;
                case "frameset":
                    if (CheckNameOfOpenedElement("body", W3CNamespaces.Xhtml, 1) && mFramesetOk)
                    {
                        HtmlNode bodyElement = GetOpenedElement(1);
                        Debug.Assert(bodyElement != null);
                        if (bodyElement.Parent != null)
                        {
                            bodyElement.Parent.Children.Remove(bodyElement);
                        }

                        // Pop all opened elements but the root "html" element.
                        Debug.Assert(!mOpenedElements.IsEmpty);
                        mOpenedElements.RemoveRange(1);

                        InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                        mInsertionMode = InsertionMode.InFrameset;
                    }
                    break;
                case "address":
                case "article":
                case "aside":
                case "blockquote":
                case "center":
                case "details":
                case "dialog":
                case "dir":
                case "div":
                case "dl":
                case "fieldset":
                case "figcaption":
                case "figure":
                case "footer":
                case "header":
                case "hgroup":
                case "menu":
                case "nav":
                case "ol":
                case "p":
                case "section":
                case "summary":
                case "ul":
                    CloseParagraphIfOpened();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    break;
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    CloseParagraphIfOpened();
                    if (CurrentElementNameIsHeader())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    break;
                case "pre":
                case "listing":
                    CloseParagraphIfOpened();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mRemoveFirstLfFromText = true;
                    mFramesetOk = false;
                    break;
                case "form":
                    if (mFormElement == null)
                    {
                        CloseParagraphIfOpened();
                        mFormElement = InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    }
                    break;
                case "li":
                    mFramesetOk = false;
                    CloseOpenedItemElements("li", W3CNamespaces.Xhtml);
                    CloseParagraphIfOpened();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    break;
                case "dd":
                case "dt":
                    mFramesetOk = false;
                    CloseOpenedDdAndDtElements();
                    CloseParagraphIfOpened();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    break;
                case "plaintext":
                    CloseParagraphIfOpened();
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mTokenizer.SwitchToPlaintextState();
                    break;
                case "button":
                    if (HasElementInScope("button", W3CNamespaces.Xhtml))
                    {
                        GenerateImpliedEndTags();
                        PopElementsUntil("button");
                        reprocessToken = true;
                    }
                    else
                    {
                        ReconstructActiveFormattingElements(fosterParent);
                        InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                        mFramesetOk = false;
                    }
                    break;
                case "a":
                {
                    HtmlElementNode activeElement = mActiveFormattingElements.FindInCurrentScope("a");
                    if (activeElement != null)
                    {
                        CloseFormattingElement("a");
                        mActiveFormattingElements.Remove(activeElement);
                        mOpenedElements.Remove(activeElement);
                    }
                    ReconstructActiveFormattingElements(fosterParent);
                    HtmlElementNode htmlElement = InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mActiveFormattingElements.Add(htmlElement);
                    break;
                }
                case "b":
                case "big":
                case "code":
                case "em":
                case "font":
                case "i":
                case "s":
                case "small":
                case "strike":
                case "strong":
                case "tt":
                case "u":
                {
                    ReconstructActiveFormattingElements(fosterParent);
                    HtmlElementNode htmlElement = InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mActiveFormattingElements.Add(htmlElement);
                    break;
                }
                case "nobr":
                {
                    ReconstructActiveFormattingElements(fosterParent);
                    if (HasElementInScope("nobr", W3CNamespaces.Xhtml))
                    {
                        CloseFormattingElement("nobr");
                        ReconstructActiveFormattingElements(fosterParent);
                    }
                    HtmlElementNode htmlElement = InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mActiveFormattingElements.Add(htmlElement);
                    break;
                }
                case "applet":
                case "marquee":
                case "object":
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mActiveFormattingElements.OpenNewScope();
                    mFramesetOk = false;
                    break;
                case "table":
                    if (mDocumentMode != HtmlDocumentMode.Quirks)
                    {
                        CloseParagraphIfOpened();
                    }
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mFramesetOk = false;
                    mInsertionMode = InsertionMode.InTable;
                    break;
                case "area":
                case "br":
                case "embed":
                case "img":
                case "keygen":
                case "wbr":
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement(tag.Name, tag.Attributes, false, fosterParent);
                    mFramesetOk = false;
                    break;
                case "image":
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement("img", tag.Attributes, false, fosterParent);
                    mFramesetOk = false;
                    break;
                case "input":
                {
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement(tag.Name, tag.Attributes, false, fosterParent);
                    HtmlAttribute typeAttribute = tag.Attributes["type"];
                    if ((typeAttribute == null) ||
                        !string.Equals(typeAttribute.Value, "hidden", StringComparison.OrdinalIgnoreCase))
                    {
                        mFramesetOk = false;
                    }
                    break;
                }
                case "param":
                case "source":
                case "track":
                    InsertHtmlElement(tag.Name, tag.Attributes, false, fosterParent);
                    break;
                case "hr":
                    CloseParagraphIfOpened();
                    InsertHtmlElement(tag.Name, tag.Attributes, false, fosterParent);
                    mFramesetOk = false;
                    break;
                case "isindex":
                    if (mFormElement == null)
                    {
                        CloseParagraphIfOpened();

                        HtmlAttributeCollection formAttributes = new HtmlAttributeCollection();
                        HtmlAttribute actionAttribute = tag.Attributes["action"];
                        if (actionAttribute != null)
                        {
                            formAttributes.Add(actionAttribute);
                        }
                        mFormElement = InsertHtmlElement("form", formAttributes, true, fosterParent);

                        InsertHtmlElement("hr", new HtmlAttributeCollection(), false, false);
                        ReconstructActiveFormattingElements(false);
                        InsertHtmlElement("label", new HtmlAttributeCollection(), true, false);

                        HtmlAttribute promptAttribute = tag.Attributes["prompt"];
                        if (promptAttribute != null)
                        {
                            if (StringUtil.HasChars(promptAttribute.Value))
                            {
                                InsertIntoCurrentNode(promptAttribute.Value);
                            }
                        }
                        else
                        {
                            InsertIntoCurrentNode("This is a searchable index. Enter search keywords: ");
                        }

                        HtmlAttributeCollection inputAttributes = new HtmlAttributeCollection();
                        foreach (HtmlAttribute tagAttribute in tag.Attributes)
                        {
                            if ((tagAttribute.Name != "name") &&
                                (tagAttribute.Name != "action") &&
                                (tagAttribute.Name != "prompt"))
                            {
                                inputAttributes.Add(tagAttribute);
                            }
                        }
                        inputAttributes.Add(new HtmlAttribute("name", "isindex"));
                        InsertHtmlElement("input", inputAttributes, false, false);

                        CloseOtherElement("label");
                        InsertHtmlElement("hr", new HtmlAttributeCollection(), false, false);

                        GenerateImpliedEndTags();
                        mOpenedElements.Remove(mFormElement);
                        mFormElement = null;

                        mFramesetOk = false;
                    }
                    break;
                case "textarea":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mRemoveFirstLfFromText = true;
                    mFramesetOk = false;
                    ParseRcdata();
                    break;
                case "xmp":
                    CloseParagraphIfOpened();
                    ReconstructActiveFormattingElements(fosterParent);
                    mFramesetOk = false;
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    ParseRawText();
                    break;
                case "iframe":
                    mFramesetOk = false;
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    ParseRawText();
                    break;
                case "noembed":
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    ParseRawText();
                    break;
                case "noscript":
                    if (mIsScriptingEnabled)
                    {
                        InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                        ParseRawText();
                    }
                    else
                    {
                        ReconstructActiveFormattingElements(fosterParent);
                        InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    }
                    break;
                case "select":
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    mFramesetOk = false;
                    switch (mInsertionMode)
                    {
                        case InsertionMode.InCell:
                        case InsertionMode.InRow:
                        case InsertionMode.InTableBody:
                        case InsertionMode.InCaption:
                        case InsertionMode.InTable:
                            mInsertionMode = InsertionMode.InSelectInTable;
                            break;
                        default:
                            mInsertionMode = InsertionMode.InSelect;
                            break;
                    }
                    break;
                case "optgroup":
                case "option":
                    if (CurrentElementIsOption())
                    {
                        CloseOpenedElements("option");
                    }
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    break;
                case "rp":
                case "rt":
                    if (HasElementInScope("ruby", W3CNamespaces.Xhtml))
                    {
                        GenerateImpliedEndTags();
                    }
                    InsertHtmlElement(tag.Name, tag.Attributes, true, fosterParent);
                    break;
                case "math":
                    ReconstructActiveFormattingElements(fosterParent);
                    AdjustMathMLAttributes(tag.Attributes);

                    // Foreign attributes are not adjusted here, because namespaces are not supported for attributes.

                    InsertElement(tag.Name, W3CNamespaces.MathML, tag.Attributes, !tag.IsSelfClosing, fosterParent);
                    break;
                case "svg":
                    ReconstructActiveFormattingElements(fosterParent);
                    AdjustSvgAttributes(tag.Attributes);

                    // Foreign attributes are not adjusted here, because namespaces are not supported for attributes.

                    InsertElement(tag.Name, W3CNamespaces.Svg, tag.Attributes, !tag.IsSelfClosing, fosterParent);
                    break;
                case "caption":
                case "col":
                case "colgroup":
                case "frame":
                case "head":
                case "tbody":
                case "td":
                case "tfoot":
                case "th":
                case "thead":
                case "tr":
                    // The token is ignored.
                    break;
                default:
                {
                    ReconstructActiveFormattingElements(fosterParent);

                    // RK According to the HTML5 standard, unknown elements are Normal Elements and do not support self-closing
                    // syntax. So if the element is unknown and even if it is self-closing, we have to always open it
                    // as a parent element. Indeed, this is what Chrome and IE do.

                    // WORDSNET-23332 Self-closing "mbp:pagebreak" was added to exclusions, because large number of such
                    // open elements led to performance issues when loading MOBI documents.
                    // We've seen MOBI documents where text is stored as a single large HTML document with lots
                    // of "mbp:pagebreak" that split it into pages. MOBI readers are able to show such documents without issues,
                    // because apparently they handle "mbp:pagebreak" in a special manner and are able to somehow stop parsing
                    // HTML at such elements in order to split the document and show one page at a time.
                    // If we, however, parse such HTML according to HTML5 rules, we end up with a HTML tree with huge nesting
                    // depth that we're unable to process in a reasonable time.
                    bool isClosed = mSupportSelfClosingNonHtmlTags && tag.IsSelfClosing && (tag.Name == "mbp:pagebreak");

                    InsertHtmlElement(tag.Name, tag.Attributes, !isClosed, fosterParent);
                    break;
                }
            }

            return reprocessToken;
        }

        /// <summary>
        /// Processes start <see cref="HtmlTagToken"/> tokens in foreign content.
        /// </summary>
        /// <remarks>
        /// For details see http://www.w3.org/TR/html5/syntax.html#parsing-main-inforeign
        /// </remarks>
        private bool ProcessStartTagInForeignContent(HtmlTagToken tag)
        {
            bool reprocess = false;
            switch (tag.Name)
            {
                case "b":
                case "big":
                case "blockquote":
                case "body":
                case "br":
                case "center":
                case "code":
                case "dd":
                case "div":
                case "dl":
                case "dt":
                case "em":
                case "embed":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "head":
                case "hr":
                case "i":
                case "img":
                case "li":
                case "listing":
                case "menu":
                case "meta":
                case "nobr":
                case "ol":
                case "p":
                case "pre":
                case "ruby":
                case "s":
                case "small":
                case "span":
                case "strong":
                case "strike":
                case "sub":
                case "sup":
                case "table":
                case "tt":
                case "u":
                case "ul":
                case "var":
                    mOpenedElements.RemoveLast();
                    ClearStackBackToHtmlContent();
                    reprocess = true;
                    break;
                case "font":
                    if (HasColorFaceOrSizeAttribute(tag.Attributes))
                    {
                        mOpenedElements.RemoveLast();
                        ClearStackBackToHtmlContent();
                        reprocess = true;
                    }
                    else
                    {
                        string ns = mOpenedElements.GetLast().Namespace;
                        if (ns == W3CNamespaces.MathML)
                        {
                            AdjustMathMLAttributes(tag.Attributes);
                        }
                        string adjustedName;
                        if (ns == W3CNamespaces.Svg)
                        {
                            adjustedName = AdjustSvgName(tag.Name);
                            AdjustSvgAttributes(tag.Attributes);
                        }
                        else
                        {
                            adjustedName = tag.Name;
                        }

                        // Foreign attributes are not adjusted here, because namespaces are not supported for attributes.

                        InsertElement(adjustedName, ns, tag.Attributes, !tag.IsSelfClosing, false);
                    }
                    break;
                default:
                {
                    string ns = mOpenedElements.GetLast().Namespace;
                    if (ns == W3CNamespaces.MathML)
                    {
                        AdjustMathMLAttributes(tag.Attributes);
                    }
                    string adjustedName;
                    if (ns == W3CNamespaces.Svg)
                    {
                        adjustedName = AdjustSvgName(tag.Name);
                        AdjustSvgAttributes(tag.Attributes);
                    }
                    else
                    {
                        adjustedName = tag.Name;
                    }

                    // Foreign attributes are not adjusted here, because namespaces are not supported for attributes.

                    InsertElement(adjustedName, ns, tag.Attributes, !tag.IsSelfClosing, false);
                    break;
                }
            }
            return reprocess;
        }

        /// <summary>
        /// Processes end <see cref="HtmlTagToken"/> tokens.
        /// </summary>
        private void ProcessEndTag(HtmlTagToken tag)
        {
            bool reprocessToken = true;
            while (reprocessToken)
            {
                reprocessToken = false;

                switch (mInsertionMode)
                {
                    case InsertionMode.Initial:
                    {
                        // If the source of the HTML document is the 'srcdoc' attribute of an IFRAME element, the document should be
                        // switched to the Standards mode. But the IFRAME 'srcdoc' attribute is not supported now.
                        mDocumentMode = HtmlDocumentMode.Quirks;
                        mInsertionMode = InsertionMode.BeforeHtml;
                        reprocessToken = true;
                        break;
                    }
                    case InsertionMode.BeforeHtml:
                    {
                        if (tag.Name == "head" ||
                            tag.Name == "body" ||
                            tag.Name == "html" ||
                            tag.Name == "br")
                        {
                            mRootElement = CreateElement("html", W3CNamespaces.Xhtml, new HtmlAttributeCollection(), true);
                            mInsertionMode = InsertionMode.BeforeHead;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.BeforeHead:
                    {
                        if (tag.Name == "head" ||
                            tag.Name == "body" ||
                            tag.Name == "html" ||
                            tag.Name == "br")
                        {
                            mHeadElement = InsertHtmlElement("head", new HtmlAttributeCollection(), true, false);
                            mInsertionMode = InsertionMode.InHead;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.InHead:
                    {
                        switch (tag.Name)
                        {
                            case "head":
                                mOpenedElements.RemoveLast();
                                mInsertionMode = InsertionMode.AfterHead;
                                break;
                            case "body":
                            case "html":
                            case "br":
                                mOpenedElements.RemoveLast();
                                mInsertionMode = InsertionMode.AfterHead;
                                reprocessToken = true;
                                break;
                            default:
                                // Other end tags are ignored inside <head> elements.
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InHeadNoscript:
                    {
                        switch (tag.Name)
                        {
                            case "noscript":
                                mOpenedElements.RemoveLast();
                                mInsertionMode = InsertionMode.InHead;
                                break;
                            case "br":
                                mOpenedElements.RemoveLast();
                                mInsertionMode = InsertionMode.InHead;
                                reprocessToken = true;
                                break;
                            default:
                                // Other end tags are ignored inside <noscript> elements.
                                break;
                        }
                        break;
                    }
                    case InsertionMode.AfterHead:
                    {
                        switch (tag.Name)
                        {
                            case "body":
                            case "html":
                            case "br":
                                InsertHtmlElement("body", new HtmlAttributeCollection(), true, false);
                                mInsertionMode = InsertionMode.InBody;
                                reprocessToken = true;
                                break;
                            default:
                                // Other end tags are ignored between <head> and <body> elements.
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InBody:
                    {
                        reprocessToken = ProcessEndTagInBody(tag, false);
                        break;
                    }
                    case InsertionMode.Text:
                    {
                        mOpenedElements.RemoveLast();
                        mInsertionMode = mOriginalInsertionMode;
                        break;
                    }
                    case InsertionMode.InTable:
                    {
                        reprocessToken = ProcessEndTagInTable(tag);
                        break;
                    }
                    case InsertionMode.InTableText:
                    {
                        InsertPendingCharacters();
                        reprocessToken = true;
                        break;
                    }
                    case InsertionMode.InCaption:
                    {
                        switch (tag.Name)
                        {
                            case "caption":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    GenerateImpliedEndTags();
                                    PopElementsUntil("caption");
                                    mActiveFormattingElements.CloseCurrentScope();
                                    mInsertionMode = InsertionMode.InTable;
                                }
                                break;
                            case "table":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    GenerateImpliedEndTags();
                                    PopElementsUntil("caption");
                                    mActiveFormattingElements.CloseCurrentScope();
                                    mInsertionMode = InsertionMode.InTable;
                                    reprocessToken = true;
                                }
                                break;
                            case "body":
                            case "col":
                            case "colgroup":
                            case "html":
                            case "tbody":
                            case "td":
                            case "tfoot":
                            case "th":
                            case "thead":
                            case "tr":
                                // Ignore the token.
                                break;
                            default:
                                reprocessToken = ProcessEndTagInBody(tag, false);
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InColumnGroup:
                    {
                        switch (tag.Name)
                        {
                            case "colgroup":
                                if (!CurrentElementIsRootHtml())
                                {
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTable;
                                }
                                break;
                            case "col":
                                // Ignore the token.
                                break;
                            default:
                                if (!CurrentElementIsRootHtml())
                                {
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTable;
                                    reprocessToken = true;
                                }
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InTableBody:
                    {
                        switch (tag.Name)
                        {
                            case "tbody":
                            case "tfoot":
                            case "thead":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    ClearStackBackToTableBodyContext();
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTable;
                                }
                                break;
                            case "table":
                                if (HasTableBodyElementInTableScope())
                                {
                                    ClearStackBackToTableBodyContext();
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTable;
                                    reprocessToken = true;
                                }
                                break;
                            case "body":
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "html":
                            case "td":
                            case "th":
                            case "tr":
                                // Ignore the token.
                                break;
                            default:
                                reprocessToken = ProcessEndTagInTable(tag);
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InRow:
                    {
                        switch (tag.Name)
                        {
                            case "tr":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    ClearStackBackToRowContext();
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTableBody;
                                }
                                break;
                            case "table":
                                if (HasElementInTableScope("tr", W3CNamespaces.Xhtml))
                                {
                                    ClearStackBackToRowContext();
                                    mOpenedElements.RemoveLast();
                                    mInsertionMode = InsertionMode.InTableBody;
                                    reprocessToken = true;
                                }
                                break;
                            case "tbody":
                            case "tfoot":
                            case "thead":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    if (HasElementInTableScope("tr", W3CNamespaces.Xhtml))
                                    {
                                        ClearStackBackToRowContext();
                                        mOpenedElements.RemoveLast();
                                        mInsertionMode = InsertionMode.InTableBody;
                                    }
                                    reprocessToken = true;
                                }
                                break;
                            case "body":
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "html":
                            case "td":
                            case "th":
                                // Ignore the token.
                                break;
                            default:
                                reprocessToken = ProcessEndTagInTable(tag);
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InCell:
                    {
                        switch (tag.Name)
                        {
                            case "td":
                            case "th":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    GenerateImpliedEndTags();
                                    PopElementsUntilCell();
                                    mActiveFormattingElements.CloseCurrentScope();
                                    mInsertionMode = InsertionMode.InRow;
                                }
                                break;
                            case "body":
                            case "caption":
                            case "col":
                            case "colgroup":
                            case "html":
                                // Ignore the token.
                                break;
                            case "table":
                            case "tbody":
                            case "tfoot":
                            case "thead":
                            case "tr":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    GenerateImpliedEndTags();
                                    PopElementsUntilCell();
                                    mActiveFormattingElements.CloseCurrentScope();
                                    mInsertionMode = InsertionMode.InRow;
                                    reprocessToken = true;
                                }
                                break;
                            default:
                                reprocessToken = ProcessEndTagInBody(tag, false);
                                break;
                        }
                        break;
                    }
                    case InsertionMode.InSelect:
                    {
                        ProcessEndTagInSelect(tag);
                        break;
                    }
                    case InsertionMode.InSelectInTable:
                    {
                        switch (tag.Name)
                        {
                            case "caption":
                            case "table":
                            case "tbody":
                            case "tfoot":
                            case "thead":
                            case "tr":
                            case "td":
                            case "th":
                                if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                                {
                                    if (HasElementInSelectScope("select", W3CNamespaces.Xhtml))
                                    {
                                        PopElementsUntil("select");
                                        ResetInsertionModeAppropriately();
                                    }
                                    reprocessToken = true;
                                }
                                break;
                            default:
                                ProcessEndTagInSelect(tag);
                                break;
                        }
                        break;
                    }
                    case InsertionMode.AfterBody:
                    {
                        if (tag.Name == "html")
                        {
                            // If the HTML document being parsed is a fragment, the token should be ignored, but parsing
                            // of HTML fragments is not supported now.
                            mInsertionMode = InsertionMode.AfterAfterBody;
                        }
                        else
                        {
                            mInsertionMode = InsertionMode.InBody;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.InFrameset:
                    {
                        switch (tag.Name)
                        {
                            case "frameset":
                                if (!CurrentElementIsRootHtml())
                                {
                                    mOpenedElements.RemoveLast();

                                    // The following code block must not be executed during parsing of HTML fragments,
                                    // but parsing of HTML fragments is not currently supported, and the block is always executed.
                                    if (!CurrentElementIsFrameset())
                                    {
                                        mInsertionMode = InsertionMode.AfterFrameset;
                                    }
                                }
                                break;
                            default:
                                // Ignore the token.
                                break;
                        }
                        break;
                    }
                    case InsertionMode.AfterFrameset:
                    {
                        switch (tag.Name)
                        {
                            case "html":
                                mInsertionMode = InsertionMode.AfterAfterFrameset;
                                break;
                            default:
                                // Ignore the token.
                                break;
                        }
                        break;
                    }
                    case InsertionMode.AfterAfterBody:
                    {
                        mInsertionMode = InsertionMode.InBody;
                        reprocessToken = true;
                        break;
                    }
                    case InsertionMode.AfterAfterFrameset:
                    {
                        // Ignore the token.
                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException("Unknown insertion mode.");
                    }
                }
            }
        }

        /// <summary>
        /// Processes end <see cref="HtmlTagToken"/> tokens in the 'in select' insertion mode.
        /// </summary>
        private void ProcessEndTagInSelect(HtmlTagToken tag)
        {
            switch (tag.Name)
            {
                case "optgroup":
                    if (CurrentElementIsOption() && PreviousElementIsOptgroup())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    if (CurrentElementIsOptgroup())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    break;
                case "option":
                    if (CurrentElementIsOption())
                    {
                        mOpenedElements.RemoveLast();
                    }
                    break;
                case "select":
                    if (HasElementInSelectScope("select", W3CNamespaces.Xhtml))
                    {
                        PopElementsUntil("select");
                        ResetInsertionModeAppropriately();
                    }
                    break;
                default:
                    // Ignore the token.
                    break;
            }
        }

        /// <summary>
        /// Processes end <see cref="HtmlTagToken"/> tokens in the 'in table' insertion mode.
        /// </summary>
        private bool ProcessEndTagInTable(HtmlTagToken tag)
        {
            bool reprocessToken = false;

            switch (tag.Name)
            {
                case "table":
                    if (HasElementInTableScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        PopElementsUntil("table");
                        ResetInsertionModeAppropriately();
                    }
                    break;
                case "body":
                case "caption":
                case "col":
                case "colgroup":
                case "html":
                case "tbody":
                case "td":
                case "tfoot":
                case "th":
                case "thead":
                case "tr":
                    // Ignore the token.
                    break;
                default:
                    reprocessToken = ProcessEndTagInBody(tag, true);
                    break;
            }

            return reprocessToken;
        }

        /// <summary>
        /// Processes end <see cref="HtmlTagToken"/> tokens in the 'in body' insertion mode.
        /// </summary>
        /// <param name="tag">The token being processed.</param>
        /// <param name="fosterParent">
        /// Insert new elements into the foster parent element instead of the current element,
        /// when the current element is a part of a table.
        /// </param>
        private bool ProcessEndTagInBody(HtmlTagToken tag, bool fosterParent)
        {
            bool reprocessToken = false;

            switch (tag.Name)
            {
                case "body":
                    if (HasElementInScope("body", W3CNamespaces.Xhtml))
                    {
                        mInsertionMode = InsertionMode.AfterBody;
                    }
                    break;
                case "html":
                    if (HasElementInScope("body", W3CNamespaces.Xhtml))
                    {
                        mInsertionMode = InsertionMode.AfterBody;
                        reprocessToken = true;
                    }
                    break;
                case "address":
                case "article":
                case "aside":
                case "blockquote":
                case "button":
                case "center":
                case "details":
                case "dialog":
                case "dir":
                case "div":
                case "dl":
                case "fieldset":
                case "figcaption":
                case "figure":
                case "footer":
                case "header":
                case "hgroup":
                case "listing":
                case "menu":
                case "nav":
                case "ol":
                case "pre":
                case "section":
                case "summary":
                case "ul":
                    if (HasElementInScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        GenerateImpliedEndTags();
                        PopElementsUntil(tag.Name);
                    }
                    break;
                case "form":
                {
                    HtmlElementNode formElement = mFormElement;
                    mFormElement = null;
                    if ((formElement != null) && HasElementInScope(formElement))
                    {
                        GenerateImpliedEndTags();
                        mOpenedElements.Remove(formElement);
                    }
                    break;
                }
                case "p":
                    if (HasElementInButtonScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        CloseElement(tag.Name);
                    }
                    else
                    {
                        InsertHtmlElement("p", new HtmlAttributeCollection(), true, fosterParent);
                        reprocessToken = true;
                    }
                    break;
                case "li":
                    if (HasElementInListItemScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        CloseElement(tag.Name);
                    }
                    break;
                case "dd":
                case "dt":
                    if (HasElementInScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        CloseElement(tag.Name);
                    }
                    break;
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    if (HasHeadingElementInScope())
                    {
                        GenerateImpliedEndTags();
                        PopElementsUntilHeading();
                    }
                    break;
                case "a":
                case "b":
                case "big":
                case "code":
                case "em":
                case "font":
                case "i":
                case "nobr":
                case "s":
                case "small":
                case "strike":
                case "strong":
                case "tt":
                case "u":
                    CloseFormattingElement(tag.Name);
                    break;
                case "applet":
                case "marquee":
                case "object":
                    if (HasElementInScope(tag.Name, W3CNamespaces.Xhtml))
                    {
                        GenerateImpliedEndTags();
                        PopElementsUntil(tag.Name);
                        mActiveFormattingElements.CloseCurrentScope();
                    }
                    break;
                case "br":
                    ReconstructActiveFormattingElements(fosterParent);
                    InsertHtmlElement(tag.Name, new HtmlAttributeCollection(), false, fosterParent);
                    mFramesetOk = false;
                    break;
                default:
                    CloseOpenedElements(tag.Name);
                    break;
            }

            return reprocessToken;
        }

        /// <summary>
        /// Processes end <see cref="HtmlTagToken"/> tokens in foreign content.
        /// </summary>
        /// <remarks>
        /// For details see http://www.w3.org/TR/html5/syntax.html#parsing-main-inforeign
        /// </remarks>
        private void ProcessEndTagInForeignContent(HtmlTagToken tag)
        {
            // The element name might be adjusted from lowercase to mixed case, and tag names are always lowercase.
            if (mOpenedElements.GetLast().Name.ToLowerInvariant() == tag.Name)
            {
                // Normal case. The end tag matches the current opened element.
                mOpenedElements.RemoveLast();
            }
            else
            {
                // Error case. The end tag does not match the current opened element.
                // Try to find an opened element for the ened tag. Maybe fall out of foreign context in process.

                // The current element has just been examined, and the loop skips it.
                for (int i = mOpenedElements.Count - 2; i >= 0; --i)
                {
                    HtmlElementNode node = mOpenedElements[i];

                    if (node.Namespace == W3CNamespaces.Xhtml)
                    {
                        // Fall out of foreign context back into HTML context.
                        ProcessEndTag(tag);
                        break;
                    }

                    // The element name might be adjusted from lowercase to mixed case, and tag names are always lowercase.
                    if (node.Name.ToLowerInvariant() == tag.Name)
                    {
                        // Found an opened element that matches the end tag. Close the element.
                        mOpenedElements.RemoveRange(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes <see cref="HtmlTextToken"/> tokens.
        /// </summary>
        private void ProcessText(HtmlTextToken text)
        {
            Debug.Assert(text.Text.Length > 0);

            HtmlText processedText = new HtmlText(text.Text);

            if (mRemoveFirstLfFromText)
            {
                processedText.RemoveLeadingLineFeedChar();
                mRemoveFirstLfFromText = false;
                if (processedText.IsEmpty)
                {
                    return;
                }
            }

            bool reprocessToken = true;
            while (reprocessToken)
            {
                reprocessToken = false;

                switch (mInsertionMode)
                {
                    case InsertionMode.Initial:
                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            // If the source of the HTML document is the 'srcdoc' attribute of an IFRAME element, the document should be
                            // switched to the Standards mode. But the IFRAME 'srcdoc' attribute is not supported now.
                            mDocumentMode = HtmlDocumentMode.Quirks;
                            mInsertionMode = InsertionMode.BeforeHtml;
                            reprocessToken = true;
                        }
                        break;
                    case InsertionMode.BeforeHtml:
                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            mRootElement = CreateElement("html", W3CNamespaces.Xhtml, new HtmlAttributeCollection(), true);
                            mInsertionMode = InsertionMode.BeforeHead;
                            reprocessToken = true;
                        }
                        break;
                    case InsertionMode.BeforeHead:
                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            mHeadElement = InsertHtmlElement("head", new HtmlAttributeCollection(), true, false);
                            mInsertionMode = InsertionMode.InHead;
                            reprocessToken = true;
                        }
                        break;
                    case InsertionMode.InHead:
                    {
                        string leadingWhitespace = processedText.GetLeadingWhitespace();
                        if (leadingWhitespace != string.Empty)
                        {
                            InsertIntoCurrentNode(leadingWhitespace);
                        }
                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            mOpenedElements.RemoveLast();
                            mInsertionMode = InsertionMode.AfterHead;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.InHeadNoscript:
                    {
                        string leadingWhitespace = processedText.GetLeadingWhitespace();
                        if (leadingWhitespace != string.Empty)
                        {
                            InsertIntoCurrentNode(leadingWhitespace);
                        }
                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            mOpenedElements.RemoveLast();
                            mInsertionMode = InsertionMode.InHead;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.AfterHead:
                    {
                        string leadingWhitespace = processedText.GetLeadingWhitespace();
                        if (leadingWhitespace != string.Empty)
                        {
                            InsertIntoCurrentNode(leadingWhitespace);
                        }
                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            InsertHtmlElement("body", new HtmlAttributeCollection(), true, false);
                            mInsertionMode = InsertionMode.InBody;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.InBody:
                    case InsertionMode.InCaption:
                    case InsertionMode.InCell:
                        processedText.StripNullCharacters();
                        if (!processedText.IsEmpty)
                        {
                            ReconstructActiveFormattingElements(false);
                            InsertIntoCurrentNode(processedText);
                            if (processedText.ContainsAnythingButWhitespace())
                            {
                                mFramesetOk = false;
                            }
                        }
                        break;
                    case InsertionMode.Text:
                        if (!processedText.IsEmpty)
                        {
                            InsertIntoCurrentNode(processedText);
                        }
                        break;
                    case InsertionMode.InTable:
                    case InsertionMode.InTableBody:
                    case InsertionMode.InRow:
                        if (CurrentElementIsTableSection())
                        {
                            mPendingTableCharacters.Clear();
                            mOriginalInsertionMode = mInsertionMode;
                            mInsertionMode = InsertionMode.InTableText;
                            reprocessToken = true;
                        }
                        else
                        {
                            processedText.StripNullCharacters();
                            if (!processedText.IsEmpty)
                            {
                                ReconstructActiveFormattingElements(false);
                                InsertIntoCurrentNode(processedText);
                                if (processedText.ContainsAnythingButWhitespace())
                                {
                                    mFramesetOk = false;
                                }
                            }
                        }
                        break;
                    case InsertionMode.InTableText:
                        processedText.StripNullCharacters();
                        if (!processedText.IsEmpty)
                        {
                            mPendingTableCharacters.Append(processedText);
                        }
                        break;
                    case InsertionMode.InColumnGroup:
                    {
                        string leadingWhitespace = processedText.GetLeadingWhitespace();
                        if (leadingWhitespace != string.Empty)
                        {
                            InsertIntoCurrentNode(leadingWhitespace);
                        }

                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            if (mOpenedElements.Count > 1)
                            {
                                mOpenedElements.RemoveLast();
                                mInsertionMode = InsertionMode.InTable;
                                reprocessToken = true;
                            }
                        }
                        break;
                    }
                    case InsertionMode.InSelect:
                    case InsertionMode.InSelectInTable:
                        processedText.StripNullCharacters();
                        if (!processedText.IsEmpty)
                        {
                            InsertIntoCurrentNode(processedText);
                        }
                        break;
                    case InsertionMode.AfterBody:
                    case InsertionMode.AfterAfterBody:
                    {
                        string leadingWhitespace = processedText.GetLeadingWhitespace();
                        if (leadingWhitespace != string.Empty)
                        {
                            ReconstructActiveFormattingElements(false);
                            InsertIntoCurrentNode(leadingWhitespace);
                            mFramesetOk = true;
                        }

                        processedText.TrimLeadingWhitespace();
                        if (!processedText.IsEmpty)
                        {
                            mInsertionMode = InsertionMode.InBody;
                            reprocessToken = true;
                        }
                        break;
                    }
                    case InsertionMode.InFrameset:
                    case InsertionMode.AfterFrameset:
                    {
                        string whitespaces = processedText.GetAllWhitespaces();
                        if (whitespaces != string.Empty)
                        {
                            ReconstructActiveFormattingElements(false);
                            InsertIntoCurrentNode(whitespaces);
                        }
                        processedText.Clear();
                        break;
                    }
                    case InsertionMode.AfterAfterFrameset:
                    {
                        string whitespaces = processedText.GetAllWhitespaces();
                        if (whitespaces != string.Empty)
                        {
                            ReconstructActiveFormattingElements(false);
                            InsertIntoCurrentNode(whitespaces);
                        }
                        processedText.Clear();
                        break;
                    }
                    default:
                        throw new InvalidOperationException("Unknown insertion mode.");
                }
            }
        }

        /// <summary>
        /// Processes <see cref="HtmlTextToken"/> tokens in foreign content.
        /// </summary>
        /// <remarks>
        /// For details see http://www.w3.org/TR/html5/syntax.html#parsing-main-inforeign
        /// </remarks>
        private void ProcessTextInForeignContent(HtmlTextToken text)
        {
            HtmlText processedText = new HtmlText(text.Text);
            if (processedText.ContainsAnythingButWhitespaceOrNull())
            {
                mFramesetOk = false;
            }
            processedText.ReplaceNullCharacters();
            InsertIntoCurrentNode(processedText);
        }

        /// <summary>
        /// Processes <see cref="HtmlEndOfFileToken"/> token.
        /// </summary>
        private void ProcessEndOfFile()
        {
            bool reprocessToken = true;
            while (reprocessToken)
            {
                reprocessToken = false;

                switch (mInsertionMode)
                {
                    case InsertionMode.Initial:
                        // If the source of the HTML document is the 'srcdoc' attribute of an IFRAME element, the document should be
                        // switched to the Standards mode. But the IFRAME 'srcdoc' attribute is not supported now.
                        mDocumentMode = HtmlDocumentMode.Quirks;
                        mInsertionMode = InsertionMode.BeforeHtml;
                        reprocessToken = true;
                        break;
                    case InsertionMode.BeforeHtml:
                        mRootElement = CreateElement("html", W3CNamespaces.Xhtml, new HtmlAttributeCollection(), true);
                        mInsertionMode = InsertionMode.BeforeHead;
                        reprocessToken = true;
                        break;
                    case InsertionMode.BeforeHead:
                        mHeadElement = InsertHtmlElement("head", new HtmlAttributeCollection(), true, false);
                        mInsertionMode = InsertionMode.InHead;
                        reprocessToken = true;
                        break;
                    case InsertionMode.InHead:
                        mOpenedElements.RemoveLast();
                        mInsertionMode = InsertionMode.AfterHead;
                        reprocessToken = true;
                        break;
                    case InsertionMode.InHeadNoscript:
                        mOpenedElements.RemoveLast();
                        mInsertionMode = InsertionMode.InHead;
                        reprocessToken = true;
                        break;
                    case InsertionMode.AfterHead:
                        InsertHtmlElement("body", new HtmlAttributeCollection(), true, false);
                        mInsertionMode = InsertionMode.InBody;
                        reprocessToken = true;
                        break;
                    case InsertionMode.Text:
                        mOpenedElements.RemoveLast();
                        mInsertionMode = mOriginalInsertionMode;
                        reprocessToken = true;
                        break;
                    case InsertionMode.InTableText:
                        InsertPendingCharacters();
                        reprocessToken = true;
                        break;
                    case InsertionMode.InColumnGroup:
                        if (mOpenedElements.Count == 1)
                        {
                            mOpenedElements.Clear();
                        }
                        else
                        {
                            mOpenedElements.RemoveLast();
                            mInsertionMode = InsertionMode.InTable;
                            reprocessToken = true;
                        }
                        break;
                    case InsertionMode.InBody:
                    case InsertionMode.InTable:
                    case InsertionMode.InCaption:
                    case InsertionMode.InTableBody:
                    case InsertionMode.InRow:
                    case InsertionMode.InCell:
                    case InsertionMode.InSelect:
                    case InsertionMode.InSelectInTable:
                    case InsertionMode.AfterBody:
                    case InsertionMode.InFrameset:
                    case InsertionMode.AfterFrameset:
                    case InsertionMode.AfterAfterBody:
                    case InsertionMode.AfterAfterFrameset:
                        mOpenedElements.Clear();
                        break;
                    default:
                        throw new InvalidOperationException("Unknown insertion mode.");
                }
            }
        }

        private void ReconstructActiveFormattingElements(bool fosterParent)
        {
            // The algorithm is described here:
            // http://www.w3.org/TR/html5/syntax.html#reconstruct-the-active-formatting-elements

            // Steps 1 - 6.
            // Find the latest marker or opened element.
            int index = mActiveFormattingElements.Count - 1;
            while (!FormattingElementIsMarkerOrOpened(index))
            {
                --index;
            }

            // Steps 7 - 11.
            // Reopen formatting elements that follow the latest marker or opened element.
            ++index;
            while (index < mActiveFormattingElements.Count)
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < mActiveFormattingElements.Count);

                HtmlElementNode entry = mActiveFormattingElements[index];
                Debug.Assert(entry != null);

                HtmlElementNode newElement = InsertHtmlElement(entry.Name, entry.Attributes, true, fosterParent);
                mActiveFormattingElements.Replace(index, newElement);

                ++index;
            }
        }

        private void CloseFormattingElement(string name)
        {
            // The algorithm is described here:
            // http://www.w3.org/TR/html5/syntax.html#adoptionAgency

            // Steps 1 - 3.
            const int maxOuterLoopCount = 8;
            for (int outerLoopCounter = 0; outerLoopCounter < maxOuterLoopCount; outerLoopCounter++)
            {
                // Step 4.
                HtmlElementNode formattingElement = mActiveFormattingElements.FindInCurrentScope(name);
                if (formattingElement == null)
                {
                    CloseOtherElement(name);
                    return;
                }

                int index = mOpenedElements.IndexOf(formattingElement);
                if (index < 0)
                {
                    mActiveFormattingElements.Remove(formattingElement);
                    return;
                }

                if (!HasElementInScope(formattingElement))
                {
                    return;
                }

                // Step 5.
                HtmlElementNode furthestBlock = null;
                int furthestBlockIndex = -1;
                for (int i = index + 1; i < mOpenedElements.Count; i++)
                {
                    HtmlElementNode item = mOpenedElements[i];
                    if (IsInSpecialCategory(item))
                    {
                        furthestBlock = item;
                        furthestBlockIndex = i;
                        break;
                    }
                }

                // Step 6.
                if (furthestBlock == null)
                {
                    mOpenedElements.RemoveRange(index);
                    mActiveFormattingElements.Remove(formattingElement);
                    return;
                }

                // Step 7.
                // The first element is 'html'.
                Debug.Assert(index > 0);
                HtmlElementNode commonAncestor = mOpenedElements[index - 1];

                // Step 8.
                int bookmark = mActiveFormattingElements.IndexOf(formattingElement);
                Debug.Assert(bookmark >= 0);

                // Step 9.
                Debug.Assert(furthestBlockIndex > 0);
                int nodeIndex = furthestBlockIndex;
                HtmlElementNode lastNode = furthestBlock;

                // Steps 9.1 - 9.3.
                const int maxInnerLoopCount = 3;
                for (int innerLoopCounter = 0; innerLoopCounter < maxInnerLoopCount; innerLoopCounter++)
                {
                    // Step 9.4.
                    --nodeIndex;
                    HtmlElementNode node = mOpenedElements[nodeIndex];

                    // Step 9.5.
                    int nodeIndexInListOfActiveFormattingElements = mActiveFormattingElements.IndexOf(node);
                    if (nodeIndexInListOfActiveFormattingElements < 0)
                    {

                        mOpenedElements.Remove(node);
                        continue;
                    }

                    // Step 9.6.
                    if (node == formattingElement)
                    {
                        break;
                    }

                    // Step 9.7.
                    HtmlElementNode newNode = CreateElement(node.Name, node.Namespace, node.Attributes, false);
                    mActiveFormattingElements.Replace(nodeIndexInListOfActiveFormattingElements, newNode);
                    mOpenedElements[nodeIndex] = newNode;
                    node = newNode;

                    // Step 9.8.
                    if (lastNode == furthestBlock)
                    {
                        bookmark = nodeIndexInListOfActiveFormattingElements;
                    }

                    // 9.9.
                    if (lastNode.Parent != null)
                    {
                        lastNode.Parent.Children.Remove(lastNode);
                    }
                    node.Children.Add(lastNode);

                    // 9.10.
                    lastNode = node;
                }

                // Step 10.
                if (lastNode.Parent != null)
                {
                    lastNode.Parent.Children.Remove(lastNode);
                }
                if (IsTableSection(commonAncestor))
                {
                    InsertIntoFosterParent(lastNode);
                }
                else
                {
                    commonAncestor.Children.Add(lastNode);
                }

                // Step 11.
                HtmlElementNode newFormattingElement = CreateElement(formattingElement.Name, formattingElement.Namespace, formattingElement.Attributes, false);

                // Step 12.
                newFormattingElement.Children.MoveRange(furthestBlock.Children);

                // Step 13.
                furthestBlock.Children.Add(newFormattingElement);

                // Step 14.
                mActiveFormattingElements.Remove(formattingElement);
                mActiveFormattingElements.Insert(bookmark, newFormattingElement);

                // Step 15.
                mOpenedElements.Remove(formattingElement);
                furthestBlockIndex = mOpenedElements.IndexOf(furthestBlock);
                Debug.Assert(furthestBlockIndex >= 0);
                mOpenedElements.Insert(furthestBlockIndex + 1, newFormattingElement);
            }
        }

        private bool FormattingElementIsMarkerOrOpened(int formattingElementIndex)
        {
            // Assume there are markers before the first element and after the last element in the list
            // of active formatting elements. This assumption helps to simplify loops.
            if ((formattingElementIndex < 0) || (formattingElementIndex >= mActiveFormattingElements.Count))
            {
                return true;
            }

            HtmlElementNode element = mActiveFormattingElements[formattingElementIndex];
            return (element == null) || mOpenedElements.Contains(element);
        }

        private void ResetInsertionModeAppropriately()
        {
            // The algorithm is described here:
            // http://www.w3.org/TR/html5/syntax.html#reset-the-insertion-mode-appropriately

            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                // If the HTML document being parsed is a fragment, the context element should be used instead of
                // the first opened element, but parsing of HTML fragments is not supported now.

                HtmlElementNode node = mOpenedElements[i];
                if (node.Namespace == W3CNamespaces.Xhtml)
                {
                    switch (node.Name)
                    {
                        case "select":
                            mInsertionMode = InsertionMode.InSelect;
                            return;
                        case "td":
                        case "th":
                            if (i > 0)
                            {
                                mInsertionMode = InsertionMode.InCell;
                                return;
                            }
                            break;
                        case "tr":
                            mInsertionMode = InsertionMode.InRow;
                            return;
                        case "tbody":
                        case "thead":
                        case "tfoot":
                            mInsertionMode = InsertionMode.InTableBody;
                            return;
                        case "caption":
                            mInsertionMode = InsertionMode.InCaption;
                            return;
                        case "colgroup":
                            mInsertionMode = InsertionMode.InColumnGroup;
                            return;
                        case "table":
                            mInsertionMode = InsertionMode.InTable;
                            return;
                        case "head":
                        case "body":
                            mInsertionMode = InsertionMode.InBody;
                            return;
                        case "frameset":
                            mInsertionMode = InsertionMode.InFrameset;
                            return;
                        case "html":
                            mInsertionMode = InsertionMode.BeforeHead;
                            return;
                        default:
                            // Other elements don't require special processing; they reset the insertion mode to 'in body'.
                            break;
                    }
                }
            }
            mInsertionMode = InsertionMode.InBody;
        }

        private void ParseRawText()
        {
            mTokenizer.SwitchToRawtextState();
            mOriginalInsertionMode = mInsertionMode;
            mInsertionMode = InsertionMode.Text;
        }

        private void ParseScriptData()
        {
            mTokenizer.SwitchToScriptDataState();
            mOriginalInsertionMode = mInsertionMode;
            mInsertionMode = InsertionMode.Text;
        }

        private void ParseRcdata()
        {
            mTokenizer.SwitchToRcdataState();
            mOriginalInsertionMode = mInsertionMode;
            mInsertionMode = InsertionMode.Text;
        }

        private void CloseParagraphIfOpened()
        {
            if (HasElementInButtonScope("p", W3CNamespaces.Xhtml))
            {
                CloseElement("p");
            }
        }

        private void CloseElement(string name)
        {
            GenerateImpliedEndTagsExcept(name);
            PopElementsUntil(name);
        }

        private void CloseOpenedElements(string name)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if (element.Name == name)
                {
                    GenerateImpliedEndTagsExcept(name);
                    PopElementsUntil(name);
                    return;
                }
                if (IsInSpecialCategory(element))
                {
                    return;
                }
            }
        }

        private void AddAttributesToHtmlElement(HtmlAttributeCollection attributes)
        {
            AddAttributesToOpenedElement(attributes, 0);
        }

        private void AddAttributesToOpenedElement(HtmlAttributeCollection attributes, int elementIndex)
        {
            HtmlElementNode openedElement = mOpenedElements[elementIndex];
            foreach (HtmlAttribute entry in attributes)
            {
                openedElement.Attributes.Add(entry);
            }
        }

        private HtmlElementNode GetOpenedElement(int index)
        {
            if ((index >= 0) && (index < mOpenedElements.Count))
            {
                return mOpenedElements[index];
            }
            else
            {
                return null;
            }
        }

        private HtmlElementNode GetLastOpenedTable()
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Namespace == W3CNamespaces.Xhtml) && (element.Name == "table"))
                {
                    return element;
                }
            }
            return null;
        }

        private void PopElementsUntil(string name)
        {
            while (!mOpenedElements.IsEmpty)
            {
                HtmlElementNode poppedElement = mOpenedElements.GetLast();
                mOpenedElements.RemoveLast();
                if (poppedElement.Name == name)
                {
                    return;
                }
            }
            // Should not get here.
            Debug.Assert(false);
        }

        private void PopElementsUntilHeading()
        {
            while (!mOpenedElements.IsEmpty)
            {
                HtmlElementNode poppedElement = mOpenedElements.GetLast();
                mOpenedElements.RemoveLast();
                if (IsHeadingElement(poppedElement))
                {
                    return;
                }
            }
            // Should not get here.
            Debug.Assert(false);
        }

        private void PopElementsUntilCell()
        {
            while (!mOpenedElements.IsEmpty)
            {
                HtmlElementNode poppedElement = mOpenedElements.GetLast();
                mOpenedElements.RemoveLast();
                if ((poppedElement.Name == "td") || (poppedElement.Name == "th"))
                {
                    return;
                }
            }
            // Should not get here.
            Debug.Assert(false);
        }

        private void ClearStackBackToTableContext()
        {
            while (!CurrentElementIsTableContext())
            {
                mOpenedElements.RemoveLast();
            }
        }

        private void ClearStackBackToTableBodyContext()
        {
            while (!CurrentElementIsTableBodyContext())
            {
                mOpenedElements.RemoveLast();
            }
        }

        private void ClearStackBackToRowContext()
        {
            while (!CurrentElementIsRowContext())
            {
                mOpenedElements.RemoveLast();
            }
        }

        private void ClearStackBackToHtmlContent()
        {
            while (!CurrentElementIsHtmlContent())
            {
                mOpenedElements.RemoveLast();
            }
        }

        private bool CurrentElementIsHtmlContent()
        {
            return CurrentElementIsMathMLTextIntegrationPoint() ||
                CurrentElementIsHtmlIntegrationPoint() ||
                CurrentElementIsInHtmlNamespace();
        }

        private bool CurrentElementIsInHtmlNamespace()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return currentElement.Namespace == W3CNamespaces.Xhtml;
        }

        private bool CurrentElementIsHtmlIntegrationPoint()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            switch (currentElement.Namespace)
            {
                case W3CNamespaces.MathML:
                {
                    if (currentElement.Name == "annotation-xml")
                    {
                        string encoding = currentElement.Attributes.GetAttributeValue("encoding", "").ToLowerInvariant();
                        if ((encoding == "text/html") || (encoding == "application/xhtml+xml"))
                        {
                            return true;
                        }
                    }
                    break;
                }
                case W3CNamespaces.Svg:
                {
                    if ((currentElement.Name == "foreignObject") ||
                        (currentElement.Name == "desc") ||
                        (currentElement.Name == "title"))
                    {
                        return true;
                    }
                    break;
                }
                case NrxNamespaces.Vml:
                {
                    // VML text boxes can contain HTML content.
                    string localVmlName = GetLocalVmlName(currentElement.Name);
                    if (localVmlName == "textbox")
                    {
                        return true;
                    }
                    break;
                }
                default:
                {
                    // Elements from other namespaces are not HTML integration points.
                    break;
                }
            }
            return false;
        }

        private bool CurrentElementIsMathMLTextIntegrationPoint()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.MathML) &&
                ((currentElement.Name == "mi") ||
                (currentElement.Name == "mo") ||
                (currentElement.Name == "mn") ||
                (currentElement.Name == "ms") ||
                (currentElement.Name == "mtext"));
        }

        private bool CheckNameOfOpenedElement(string name, string ns, int index)
        {
            HtmlElementNode element = GetOpenedElement(index);
            return (element != null) && (element.Name == name) && (element.Namespace == ns);
        }

        private bool HasElementInScope(string name, string ns)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Name == name) && (element.Namespace == ns))
                {
                    return true;
                }
                if (IsScopeElement(element))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasElementInScope(HtmlElementNode element)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode item = mOpenedElements[i];
                if (item == element)
                {
                    return true;
                }
                if (IsScopeElement(item))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasHeadingElementInScope()
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if (IsHeadingElement(element))
                {
                    return true;
                }
                if (IsScopeElement(element))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasElementInButtonScope(string name, string ns)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Name == name) && (element.Namespace == ns))
                {
                    return true;
                }
                if (IsScopeElement(element))
                {
                    return false;
                }
                if ((element.Name == "button") && (element.Namespace == ns))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasElementInListItemScope(string name, string ns)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Name == name) && (element.Namespace == ns))
                {
                    return true;
                }
                if (IsScopeElement(element))
                {
                    return false;
                }
                if (((element.Name == "ol") || (element.Name == "ul")) && (element.Namespace == ns))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasTableBodyElementInTableScope()
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if (((element.Name == "tbody") || (element.Name == "tfoot") || (element.Name == "thead")) &&
                    (element.Namespace == W3CNamespaces.Xhtml))
                {
                    return true;
                }
                if (((element.Name == "html") || (element.Name == "table")) &&
                    (element.Namespace == W3CNamespaces.Xhtml))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasTableCellElementInTableScope()
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if (((element.Name == "td") || (element.Name == "th")) &&
                    (element.Namespace == W3CNamespaces.Xhtml))
                {
                    return true;
                }
                if (((element.Name == "html") || (element.Name == "table")) &&
                    (element.Namespace == W3CNamespaces.Xhtml))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasElementInTableScope(string name, string ns)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Name == name) &&
                    (element.Namespace == ns))
                {
                    return true;
                }
                if (((element.Name == "html") || (element.Name == "table")) &&
                    (element.Namespace == W3CNamespaces.Xhtml))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private bool HasElementInSelectScope(string name, string ns)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Name == name) &&
                    (element.Namespace == ns))
                {
                    return true;
                }
                // The following conditions are exclusive.
                // The scope consists of ALL elements EXCEPT 'option' and 'optgroup'.
                if (((element.Name != "option") && (element.Name != "optgroup"))
                    || (element.Namespace != W3CNamespaces.Xhtml))
                {
                    return false;
                }
            }
            // Should never get here.
            Debug.Assert(false);
            return false;
        }

        private void GenerateImpliedEndTags()
        {
            // An element's name cannot be empty, so an empty string here means "do not exclude any names".
            GenerateImpliedEndTagsExcept("");
        }

        private void GenerateImpliedEndTagsExcept(string excludedName)
        {
            // There will be no infinite loop, because the "html" element is not in the list.
            bool endTagIsImplied;
            do
            {
                HtmlElementNode currentElement = mOpenedElements.GetLast();
                endTagIsImplied =
                    currentElement.Namespace == W3CNamespaces.Xhtml &&
                    (currentElement.Name == "dd" ||
                    currentElement.Name == "dt" ||
                    currentElement.Name == "li" ||
                    currentElement.Name == "option" ||
                    currentElement.Name == "optgroup" ||
                    currentElement.Name == "p" ||
                    currentElement.Name == "rp" ||
                    currentElement.Name == "rt");
                endTagIsImplied = endTagIsImplied && (currentElement.Name != excludedName);
                if (endTagIsImplied)
                {
                    mOpenedElements.RemoveLast();
                }
            } while (endTagIsImplied);
        }

        private bool CurrentElementNameIsHeader()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return IsHeadingElement(currentElement);
        }

        private bool CurrentElementIsTableSection()
        {
            return IsTableSection(mOpenedElements.GetLast());
        }

        private bool CurrentElementIsOption()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                (currentElement.Name == "option");
        }

        private bool CurrentElementIsOptgroup()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                (currentElement.Name == "optgroup");
        }

        private bool PreviousElementIsOptgroup()
        {
            if (mOpenedElements.Count > 1)
            {
                HtmlElementNode element = mOpenedElements[mOpenedElements.Count - 2];
                return (element.Namespace == W3CNamespaces.Xhtml) && (element.Name == "optgroup");
            }
            else
            {
                return false;
            }
        }

        private bool CurrentElementIsRootHtml()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                (currentElement.Name == "html") &&
                (mOpenedElements.Count == 1);
        }

        private bool CurrentElementIsFrameset()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                (currentElement.Name == "frameset");
        }

        private bool CurrentElementIsTableContext()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                ((currentElement.Name == "table") ||
                (currentElement.Name == "html"));
        }

        private bool CurrentElementIsTableBodyContext()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                ((currentElement.Name == "tbody") ||
                (currentElement.Name == "tfoot") ||
                (currentElement.Name == "thead") ||
                (currentElement.Name == "html"));
        }

        private bool CurrentElementIsRowContext()
        {
            HtmlElementNode currentElement = mOpenedElements.GetLast();
            return (currentElement.Namespace == W3CNamespaces.Xhtml) &&
                ((currentElement.Name == "tr") ||
                (currentElement.Name == "html"));
        }

        private void CloseOpenedItemElements(string name, string ns)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if ((element.Name == name) && (element.Namespace == ns))
                {
                    GenerateImpliedEndTagsExcept("li");
                    PopElementsUntil("li");
                    return;
                }
                if (IsInSpecialCategoryExceptAddressDivP(element))
                {
                    return;
                }
            }
        }

        private void CloseOpenedDdAndDtElements()
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode element = mOpenedElements[i];
                if (((element.Name == "dd") || (element.Name == "dt")) && (element.Namespace == W3CNamespaces.Xhtml))
                {
                    GenerateImpliedEndTagsExcept(element.Name);
                    PopElementsUntil(element.Name);
                    return;
                }
                if (IsInSpecialCategoryExceptAddressDivP(element))
                {
                    return;
                }
            }
        }

        private void CloseOtherElement(string name)
        {
            for (int i = mOpenedElements.Count - 1; i >= 0; i--)
            {
                HtmlElementNode item = mOpenedElements[i];
                if (item.Name == name)
                {
                    GenerateImpliedEndTagsExcept(name);
                    mOpenedElements.RemoveRange(i);
                    break;
                }
                else if (IsInSpecialCategory(item))
                {
                    break;
                }
            }
        }

        private HtmlElementNode CreateElement(string name, string ns, HtmlAttributeCollection attributes, bool opened)
        {
            HtmlElementNode element = new HtmlElementNode(name, ns, attributes);

            if (opened)
            {
                mOpenedElements.Add(element);
            }

            return element;
        }

        private HtmlElementNode InsertHtmlElement(string name, HtmlAttributeCollection attributes, bool opened,
            bool fosterParentIfInTable)
        {
            return InsertElement(name, W3CNamespaces.Xhtml, attributes, opened, fosterParentIfInTable);
        }

        private HtmlElementNode InsertElement(string name, string ns, HtmlAttributeCollection attributes, bool opened,
            bool fosterParentIfInTable)
        {
            HtmlElementNode element = new HtmlElementNode(name, ns, attributes);

            if (fosterParentIfInTable && CurrentElementIsTableSection())
            {
                InsertIntoFosterParent(element);
            }
            else
            {
                InsertIntoCurrentNode(element);
            }

            if (opened)
            {
                mOpenedElements.Add(element);
            }

            return element;
        }

        private void InsertIntoCurrentNode(string text)
        {
            Debug.Assert(StringUtil.HasChars(text));

            InsertIntoCurrentNode(new HtmlTextNode(text));
        }

        private void InsertIntoCurrentNode(HtmlText text)
        {
            Debug.Assert(text != null);

            InsertIntoCurrentNode(text.GetAsString());
        }

        private void InsertIntoFosterParent(HtmlText text)
        {
            Debug.Assert(text != null);
            Debug.Assert(!text.IsEmpty);

            InsertIntoFosterParent(new HtmlTextNode(text.GetAsString()));
        }

        private void InsertIntoCurrentNode(HtmlNode node)
        {
            Debug.Assert(node != null);

            HtmlElementNode currentNode = mOpenedElements.GetLast();
            currentNode.Children.Add(node);
        }

        private void InsertIntoFosterParent(HtmlNode node)
        {
            Debug.Assert(node != null);

            HtmlElementNode lastOpenedTable = GetLastOpenedTable();
            if (lastOpenedTable == null)
            {
                HtmlElementNode htmlElement = mOpenedElements[0];
                htmlElement.Children.Add(node);
            }
            else
            {
                HtmlElementNode parent = lastOpenedTable.Parent;
                if (parent == null)
                {
                    int lastOpenedTableIndex = mOpenedElements.IndexOf(lastOpenedTable);
                    parent = mOpenedElements[lastOpenedTableIndex - 1];
                }
                int newNodeIndex = parent.Children.IndexOf(lastOpenedTable);
                if (newNodeIndex >= 0)
                {
                    parent.Children.Insert(newNodeIndex, node);
                }
                else
                {
                    parent.Children.Add(node);
                }
            }
        }

        private void InsertPendingCharacters()
        {
            if (mPendingTableCharacters.ContainsAnythingButWhitespace())
            {
                ReconstructActiveFormattingElements(true);
                if (CurrentElementIsTableSection())
                {
                    InsertIntoFosterParent(mPendingTableCharacters);
                }
                else
                {
                    InsertIntoCurrentNode(mPendingTableCharacters);
                }
                mFramesetOk = false;
            }
            else
            {
                InsertIntoCurrentNode(mPendingTableCharacters);
            }
            mInsertionMode = mOriginalInsertionMode;
        }

        private bool IsScopeElement(HtmlElementNode element)
        {
            // VML text boxes can contain HTML content.
            if (element.Namespace == NrxNamespaces.Vml)
            {
                string localVmlName = GetLocalVmlName(element.Name);
                if (localVmlName == "textbox")
                {
                    return true;
                }
            }

            switch (element.Name)
            {
                case "applet":
                case "caption":
                case "html":
                case "table":
                case "td":
                case "th":
                case "marquee":
                case "object":
                    return element.Namespace == W3CNamespaces.Xhtml;
                case "mi":
                case "mo":
                case "mn":
                case "ms":
                case "mtext":
                case "annotation-xml":
                    return element.Namespace == W3CNamespaces.MathML;
                case "foreignObject":
                case "desc":
                case "title":
                    return element.Namespace == W3CNamespaces.Svg;
                default:
                    return false;
            }
        }

        private static bool IsHeadingElement(HtmlElementNode element)
        {
            Debug.Assert(element != null);
            if (element.Namespace == W3CNamespaces.Xhtml)
            {
                return
                    (element.Name == "h1") ||
                    (element.Name == "h2") ||
                    (element.Name == "h3") ||
                    (element.Name == "h4") ||
                    (element.Name == "h5") ||
                    (element.Name == "h6");
            }
            else
            {
                return false;
            }
        }

        private static bool IsTableSection(HtmlElementNode element)
        {
            if (element.Namespace == W3CNamespaces.Xhtml)
            {
                return
                    (element.Name == "table") ||
                    (element.Name == "thead") ||
                    (element.Name == "tbody") ||
                    (element.Name == "tfoot") ||
                    (element.Name == "tr");
            }
            else
            {
                return false;
            }
        }

        private static bool IsInSpecialCategoryExceptAddressDivP(HtmlElementNode element)
        {
            // The namespace is checked in the first method.
            return
                IsInSpecialCategory(element) &&
                (element.Name != "address") &&
                (element.Name != "div") &&
                (element.Name != "p");
        }

        private static bool IsInSpecialCategory(HtmlElementNode element)
        {
            switch (element.Name)
            {
                case "address":
                case "applet":
                case "area":
                case "article":
                case "aside":
                case "base":
                case "basefont":
                case "bgsound":
                case "blockquote":
                case "body":
                case "br":
                case "button":
                case "caption":
                case "center":
                case "col":
                case "colgroup":
                case "command":
                case "dd":
                case "details":
                case "dir":
                case "div":
                case "dl":
                case "dt":
                case "embed":
                case "fieldset":
                case "figcaption":
                case "figure":
                case "footer":
                case "form":
                case "frame":
                case "frameset":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "head":
                case "header":
                case "hgroup":
                case "hr":
                case "html":
                case "iframe":
                case "img":
                case "input":
                case "isindex":
                case "li":
                case "link":
                case "listing":
                case "marquee":
                case "menu":
                case "meta":
                case "nav":
                case "noembed":
                case "noframes":
                case "noscript":
                case "object":
                case "ol":
                case "p":
                case "param":
                case "plaintext":
                case "pre":
                case "script":
                case "section":
                case "select":
                case "source":
                case "style":
                case "summary":
                case "table":
                case "tbody":
                case "td":
                case "textarea":
                case "tfoot":
                case "th":
                case "thead":
                case "tr":
                case "track":
                case "ul":
                case "wbr":
                case "xmp":
                    return element.Namespace == W3CNamespaces.Xhtml;
                case "mi":
                case "mo":
                case "mn":
                case "ms":
                case "mtext":
                    return element.Namespace == W3CNamespaces.MathML;
                case "foreignObject":
                case "desc":
                    return element.Namespace == W3CNamespaces.Svg;
                case "title":
                    return
                        element.Namespace == W3CNamespaces.Xhtml ||
                        element.Namespace == W3CNamespaces.Svg;
                default:
                    return false;
            }
        }

        private static string AdjustSvgName(string name)
        {
            switch (name)
            {
                case "altglyph":
                    return "altGlyph";
                case "altglyphdef":
                    return "altGlyphDef";
                case "altglyphitem":
                    return "altGlyphItem";
                case "animatecolor":
                    return "animateColor";
                case "animatemotion":
                    return "animateMotion";
                case "animatetransform":
                    return "animateTransform";
                case "clippath":
                    return "clipPath";
                case "feblend":
                    return "feBlend";
                case "fecolormatrix":
                    return "feColorMatrix";
                case "fecomponenttransfer":
                    return "feComponentTransfer";
                case "fecomposite":
                    return "feComposite";
                case "feconvolvematrix":
                    return "feConvolveMatrix";
                case "fediffuselighting":
                    return "feDiffuseLighting";
                case "fedisplacementmap":
                    return "feDisplacementMap";
                case "fedistantlight":
                    return "feDistantLight";
                case "feflood":
                    return "feFlood";
                case "fefunca":
                    return "feFuncA";
                case "fefuncb":
                    return "feFuncB";
                case "fefuncg":
                    return "feFuncG";
                case "fefuncr":
                    return "feFuncR";
                case "fegaussianblur":
                    return "feGaussianBlur";
                case "feimage":
                    return "feImage";
                case "femerge":
                    return "feMerge";
                case "femergenode":
                    return "feMergeNode";
                case "femorphology":
                    return "feMorphology";
                case "feoffset":
                    return "feOffset";
                case "fepointlight":
                    return "fePointLight";
                case "fespecularlighting":
                    return "feSpecularLighting";
                case "fespotlight":
                    return "feSpotLight";
                case "fetile":
                    return "feTile";
                case "feturbulence":
                    return "feTurbulence";
                case "foreignobject":
                    return "foreignObject";
                case "glyphref":
                    return "glyphRef";
                case "lineargradient":
                    return "linearGradient";
                case "radialgradient":
                    return "radialGradient";
                case "textpath":
                    return "textPath";
                default:
                    return name;
            }
        }

        /// <summary>
        /// Gets local VML element name, without the namespace prefix.
        /// </summary>
        private string GetLocalVmlName(string name)
        {
            if (StringUtil.HasChars(mVmlElementPrefix) && name.StartsWith(mVmlElementPrefix, StringComparison.Ordinal))
            {
                string localName = name.Substring(mVmlElementPrefix.Length);
                if (StringUtil.HasChars(localName))
                {
                    return localName;
                }
            }
            return name;
        }

        private static void AdjustMathMLAttributes(HtmlAttributeCollection attributes)
        {
            attributes.Rename("definitionurl", "definitionURL");
        }

        private static void AdjustSvgAttributes(HtmlAttributeCollection attributes)
        {
            attributes.Rename("attributename", "attributeName");
            attributes.Rename("attributetype", "attributeType");
            attributes.Rename("basefrequency", "baseFrequency");
            attributes.Rename("baseprofile", "baseProfile");
            attributes.Rename("calcmode", "calcMode");
            attributes.Rename("clippathunits", "clipPathUnits");
            attributes.Rename("contentscripttype", "contentScriptType");
            attributes.Rename("contentstyletype", "contentStyleType");
            attributes.Rename("diffuseconstant", "diffuseConstant");
            attributes.Rename("edgemode", "edgeMode");
            attributes.Rename("externalresourcesrequired", "externalResourcesRequired");
            attributes.Rename("filterres", "filterRes");
            attributes.Rename("filterunits", "filterUnits");
            attributes.Rename("glyphref", "glyphRef");
            attributes.Rename("gradienttransform", "gradientTransform");
            attributes.Rename("gradientunits", "gradientUnits");
            attributes.Rename("kernelmatrix", "kernelMatrix");
            attributes.Rename("kernelunitlength", "kernelUnitLength");
            attributes.Rename("keypoints", "keyPoints");
            attributes.Rename("keysplines", "keySplines");
            attributes.Rename("keytimes", "keyTimes");
            attributes.Rename("lengthadjust", "lengthAdjust");
            attributes.Rename("limitingconeangle", "limitingConeAngle");
            attributes.Rename("markerheight", "markerHeight");
            attributes.Rename("markerunits", "markerUnits");
            attributes.Rename("markerwidth", "markerWidth");
            attributes.Rename("maskcontentunits", "maskContentUnits");
            attributes.Rename("maskunits", "maskUnits");
            attributes.Rename("numoctaves", "numOctaves");
            attributes.Rename("pathlength", "pathLength");
            attributes.Rename("patterncontentunits", "patternContentUnits");
            attributes.Rename("patterntransform", "patternTransform");
            attributes.Rename("patternunits", "patternUnits");
            attributes.Rename("pointsatx", "pointsAtX");
            attributes.Rename("pointsaty", "pointsAtY");
            attributes.Rename("pointsatz", "pointsAtZ");
            attributes.Rename("preservealpha", "preserveAlpha");
            attributes.Rename("preserveaspectratio", "preserveAspectRatio");
            attributes.Rename("primitiveunits", "primitiveUnits");
            attributes.Rename("refx", "refX");
            attributes.Rename("refy", "refY");
            attributes.Rename("repeatcount", "repeatCount");
            attributes.Rename("repeatdur", "repeatDur");
            attributes.Rename("requiredextensions", "requiredExtensions");
            attributes.Rename("requiredfeatures", "requiredFeatures");
            attributes.Rename("specularconstant", "specularConstant");
            attributes.Rename("specularexponent", "specularExponent");
            attributes.Rename("spreadmethod", "spreadMethod");
            attributes.Rename("startoffset", "startOffset");
            attributes.Rename("stddeviation", "stdDeviation");
            attributes.Rename("stitchtiles", "stitchTiles");
            attributes.Rename("surfacescale", "surfaceScale");
            attributes.Rename("systemlanguage", "systemLanguage");
            attributes.Rename("tablevalues", "tableValues");
            attributes.Rename("targetx", "targetX");
            attributes.Rename("targety", "targetY");
            attributes.Rename("textlength", "textLength");
            attributes.Rename("viewbox", "viewBox");
            attributes.Rename("viewtarget", "viewTarget");
            attributes.Rename("xchannelselector", "xChannelSelector");
            attributes.Rename("ychannelselector", "yChannelSelector");
            attributes.Rename("zoomandpan", "zoomAndPan");
        }

        private static bool HasColorFaceOrSizeAttribute(HtmlAttributeCollection attributes)
        {
            return (attributes["color"] != null) ||
                (attributes["face"] != null) ||
                (attributes["size"] != null);
        }

        /// <summary>
        /// The insertion modes of the HTML tree constructor.
        /// </summary>
        private enum InsertionMode
        {
            Initial,
            BeforeHtml,
            BeforeHead,
            InHead,
            InHeadNoscript,
            AfterHead,
            InBody,
            Text,
            InTable,
            InTableText,
            InCaption,
            InColumnGroup,
            InTableBody,
            InRow,
            InCell,
            InSelect,
            InSelectInTable,
            AfterBody,
            InFrameset,
            AfterFrameset,
            AfterAfterBody,
            AfterAfterFrameset
        }

        /// <summary>
        /// The tokenizer that parses the HTML document into tokens.
        /// </summary>
        private readonly HtmlTokenizer mTokenizer;

        /// <summary>
        /// Namespace prefix of VML element names in this document. <c>null</c> if we should skip VML content.
        /// </summary>
        private string mVmlElementPrefix;

        /// <summary>
        /// The current insertion mode of the tree constructor.
        /// </summary>
        private InsertionMode mInsertionMode;

        /// <summary>
        /// The insertion mode to which the tree constructor must switch back after processing a text token.
        /// </summary>
        private InsertionMode mOriginalInsertionMode;

        /// <summary>
        /// The quirks mode of the HTML document.
        /// </summary>
        private HtmlDocumentMode mDocumentMode;

        /// <summary>
        /// Flag that indicates that the HTML document can contain frames.
        /// </summary>
        private bool mFramesetOk;

        /// <summary>
        /// Flag that indicates that if the text of a node starts with a LF character (U+00A0), this character
        /// must be removed.
        /// </summary>
        private bool mRemoveFirstLfFromText;

        /// <summary>
        /// The root element of the document's HTML tree.
        /// </summary>
        private HtmlElementNode mRootElement;

        /// <summary>
        /// The head element of the document's HTML tree.
        /// </summary>
        private HtmlElementNode mHeadElement;

        /// <summary>
        /// The current (opened) form element.
        /// </summary>
        private HtmlElementNode mFormElement;

        /// <summary>
        /// The list of HTML elements that are still opened. The most recently opened element comes last.
        /// </summary>
        private readonly HtmlOpenedElementList mOpenedElements = new HtmlOpenedElementList();

        /// <summary>
        /// The list of active formatting elements. The most recently opened element comes last.
        /// </summary>
        private readonly HtmlActiveFormattingElementList mActiveFormattingElements = new HtmlActiveFormattingElementList();

        /// <summary>
        /// The temporary buffer that holds text of a table element until it is inserted to an appropriate
        /// place in the HTML tree.
        /// </summary>
        private readonly HtmlText mPendingTableCharacters = new HtmlText();

        /// <summary>
        /// Indicates whether to parse &lt;noscript&gt; Html tags.
        /// </summary>
        private readonly bool mIsScriptingEnabled;

        /// <summary>
        /// Indicates whether to support certain self-closing non-HTML tags that are used, for example, in MOBI documents.
        /// </summary>
        private readonly bool mSupportSelfClosingNonHtmlTags;
    }
}
