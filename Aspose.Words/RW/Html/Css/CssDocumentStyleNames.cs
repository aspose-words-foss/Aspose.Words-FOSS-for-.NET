// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/04/2024 by Victor Chebotok

using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Converts names of built-in document styles to CSS identifiers and vice versa for HTML round trip.
    /// </summary>
    internal static class CssDocumentStyleNames
    {
        static CssDocumentStyleNames()
        {
            // CSS identifiers that we use are case-insensitive.
            gParagraphStyleNamesMap = new StringToIntBidirectionalMap(false);
            gCharacterStyleNamesMap = new StringToIntBidirectionalMap(false);
            gCharacterStyleIdentifiers = new IntList();

            AddMapEntry(StyleIdentifier.BalloonText, "balloon-text", false);
            AddMapEntry(StyleIdentifier.BodyText, "body-text", false);
            AddMapEntry(StyleIdentifier.BodyText2, "body-text2", false);
            AddMapEntry(StyleIdentifier.BodyText3, "body-text3", false);
            AddMapEntry(StyleIdentifier.BodyText1I, "body-text-first-indent", false);
            AddMapEntry(StyleIdentifier.BodyText1I2, "body-text-first-indent2", false);
            AddMapEntry(StyleIdentifier.BodyTextInd, "body-text-indent", false);
            AddMapEntry(StyleIdentifier.BodyTextInd2, "body-text-indent2", false);
            AddMapEntry(StyleIdentifier.BodyTextInd3, "body-text-indent3", false);
            AddMapEntry(StyleIdentifier.Closing, "closing", false);
            AddMapEntry(StyleIdentifier.CommentSubject, "comment-subject", false);
            AddMapEntry(StyleIdentifier.CommentText, "comment-text", false);
            AddMapEntry(StyleIdentifier.Date, "date", false);
            AddMapEntry(StyleIdentifier.DocumentMap, "document-map", false);
            AddMapEntry(StyleIdentifier.EmailSignature, "email-signature", false);
            AddMapEntry(StyleIdentifier.EndnoteText, "endnote-text", false);
            AddMapEntry(StyleIdentifier.Footer, "footer", false);
            AddMapEntry(StyleIdentifier.FootnoteText, "footnote-text", false);
            AddMapEntry(StyleIdentifier.Header, "header", false);
            AddMapEntry(StyleIdentifier.Heading1, "heading1", false);
            AddMapEntry(StyleIdentifier.Heading2, "heading2", false);
            AddMapEntry(StyleIdentifier.Heading3, "heading3", false);
            AddMapEntry(StyleIdentifier.Heading4, "heading4", false);
            AddMapEntry(StyleIdentifier.Heading5, "heading5", false);
            AddMapEntry(StyleIdentifier.Heading6, "heading6", false);
            AddMapEntry(StyleIdentifier.Heading7, "heading7", false);
            AddMapEntry(StyleIdentifier.Heading8, "heading8", false);
            AddMapEntry(StyleIdentifier.Heading9, "heading9", false);
            AddMapEntry(StyleIdentifier.HtmlAddress, "html-address", false);
            AddMapEntry(StyleIdentifier.HtmlPreformatted, "html-preformatted", false);
            AddMapEntry(StyleIdentifier.IntenseQuote, "intense-quote", false);
            AddMapEntry(StyleIdentifier.Macro, "macro", false);
            AddMapEntry(StyleIdentifier.MessageHeader, "message-header", false);
            AddMapEntry(StyleIdentifier.NoteHeading, "note-heading", false);
            AddMapEntry(StyleIdentifier.PlainText, "plain-text", false);
            AddMapEntry(StyleIdentifier.Quote, "quote", false);
            AddMapEntry(StyleIdentifier.Salutation, "salutation", false);
            AddMapEntry(StyleIdentifier.Signature, "signature", false);
            AddMapEntry(StyleIdentifier.Subtitle, "subtitle", false);
            AddMapEntry(StyleIdentifier.Title, "title", false);

            AddMapEntry(StyleIdentifier.Bibliography, "bibliography", false);
            AddMapEntry(StyleIdentifier.BlockText, "block-text", false);
            AddMapEntry(StyleIdentifier.Caption, "caption", false);
            AddMapEntry(StyleIdentifier.EnvelopeAddress, "envelope-address", false);
            AddMapEntry(StyleIdentifier.EnvelopeReturn, "envelope-return", false);
            AddMapEntry(StyleIdentifier.Index1, "index1", false);
            AddMapEntry(StyleIdentifier.Index2, "index2", false);
            AddMapEntry(StyleIdentifier.Index3, "index3", false);
            AddMapEntry(StyleIdentifier.Index4, "index4", false);
            AddMapEntry(StyleIdentifier.Index5, "index5", false);
            AddMapEntry(StyleIdentifier.Index6, "index6", false);
            AddMapEntry(StyleIdentifier.Index7, "index7", false);
            AddMapEntry(StyleIdentifier.Index8, "index8", false);
            AddMapEntry(StyleIdentifier.Index9, "index9", false);
            AddMapEntry(StyleIdentifier.IndexHeading, "index-heading", false);
            AddMapEntry(StyleIdentifier.List, "list", false);
            AddMapEntry(StyleIdentifier.List2, "list2", false);
            AddMapEntry(StyleIdentifier.List3, "list3", false);
            AddMapEntry(StyleIdentifier.List4, "list4", false);
            AddMapEntry(StyleIdentifier.List5, "list5", false);
            AddMapEntry(StyleIdentifier.ListBullet, "list-bullet", false);
            AddMapEntry(StyleIdentifier.ListBullet2, "list-bullet2", false);
            AddMapEntry(StyleIdentifier.ListBullet3, "list-bullet3", false);
            AddMapEntry(StyleIdentifier.ListBullet4, "list-bullet4", false);
            AddMapEntry(StyleIdentifier.ListBullet5, "list-bullet5", false);
            AddMapEntry(StyleIdentifier.ListContinue, "list-continue", false);
            AddMapEntry(StyleIdentifier.ListContinue2, "list-continue2", false);
            AddMapEntry(StyleIdentifier.ListContinue3, "list-continue3", false);
            AddMapEntry(StyleIdentifier.ListContinue4, "list-continue4", false);
            AddMapEntry(StyleIdentifier.ListContinue5, "list-continue5", false);
            AddMapEntry(StyleIdentifier.ListNumber, "list-number", false);
            AddMapEntry(StyleIdentifier.ListNumber2, "list-number2", false);
            AddMapEntry(StyleIdentifier.ListNumber3, "list-number3", false);
            AddMapEntry(StyleIdentifier.ListNumber4, "list-number4", false);
            AddMapEntry(StyleIdentifier.ListNumber5, "list-number5", false);
            AddMapEntry(StyleIdentifier.ListParagraph, "list-paragraph", false);
            AddMapEntry(StyleIdentifier.NoSpacing, "no-spacing", false);
            AddMapEntry(StyleIdentifier.Normal, "normal", false);
            AddMapEntry(StyleIdentifier.NormalWeb, "normal-web", false);
            AddMapEntry(StyleIdentifier.NormalIndent, "normal-indent", false);
            AddMapEntry(StyleIdentifier.TableOfAuthorities, "table-of-authorities", false);
            AddMapEntry(StyleIdentifier.TableOfFigures, "table-of-figures", false);
            AddMapEntry(StyleIdentifier.ToaHeading, "toa-heading", false);
            AddMapEntry(StyleIdentifier.Toc1, "toc1", false);
            AddMapEntry(StyleIdentifier.Toc2, "toc2", false);
            AddMapEntry(StyleIdentifier.Toc3, "toc3", false);
            AddMapEntry(StyleIdentifier.Toc4, "toc4", false);
            AddMapEntry(StyleIdentifier.Toc5, "toc5", false);
            AddMapEntry(StyleIdentifier.Toc6, "toc6", false);
            AddMapEntry(StyleIdentifier.Toc7, "toc7", false);
            AddMapEntry(StyleIdentifier.Toc8, "toc8", false);
            AddMapEntry(StyleIdentifier.Toc9, "toc9", false);
            AddMapEntry(StyleIdentifier.TocHeading, "toc-heading", false);
            AddMapEntry(StyleIdentifier.Revision, "revision", false);

            AddMapEntry(StyleIdentifier.BookTitle, "book-title", true);
            AddMapEntry(StyleIdentifier.CommentReference, "comment-reference", true);
            AddMapEntry(StyleIdentifier.Emphasis, "emphasis", true);
            AddMapEntry(StyleIdentifier.EndnoteReference, "endnote-reference", true);
            AddMapEntry(StyleIdentifier.FollowedHyperlink, "followed-hyperlink", true);
            AddMapEntry(StyleIdentifier.FootnoteReference, "footnote-reference", true);
            AddMapEntry(StyleIdentifier.Hashtag, "hashtag", true);
            AddMapEntry(StyleIdentifier.HtmlAcronym, "html-acronym", true);
            AddMapEntry(StyleIdentifier.HtmlCite, "html-cite", true);
            AddMapEntry(StyleIdentifier.HtmlCode, "html-code", true);
            AddMapEntry(StyleIdentifier.HtmlDefinition, "html-definition", true);
            AddMapEntry(StyleIdentifier.HtmlKeyboard, "html-keyboard", true);
            AddMapEntry(StyleIdentifier.HtmlSample, "html-sample", true);
            AddMapEntry(StyleIdentifier.HtmlTypewriter, "html-typewriter", true);
            AddMapEntry(StyleIdentifier.HtmlVariable, "html-variable", true);
            AddMapEntry(StyleIdentifier.Hyperlink, "hyperlink", true);
            AddMapEntry(StyleIdentifier.IntenseEmphasis, "intense-emphasis", true);
            AddMapEntry(StyleIdentifier.IntenseReference, "intense-reference", true);
            AddMapEntry(StyleIdentifier.LineNumber, "line-number", true);
            AddMapEntry(StyleIdentifier.Mention, "mention", true);
            AddMapEntry(StyleIdentifier.PageNumber, "page-number", true);
            AddMapEntry(StyleIdentifier.PlaceholderText, "placeholder-text", true);
            AddMapEntry(StyleIdentifier.SmartHyperlink, "smart-hyperlink", true);
            AddMapEntry(StyleIdentifier.SmartLink, "smart-link", true);
            AddMapEntry(StyleIdentifier.Strong, "strong", true);
            AddMapEntry(StyleIdentifier.SubtleEmphasis, "subtle-emphasis", true);
            AddMapEntry(StyleIdentifier.SubtleReference, "subtle-reference", true);
            AddMapEntry(StyleIdentifier.UnresolvedMention, "unresolved-mention", true);
        }

        internal static StyleIdentifier CssStyleNameToStyleIdentifier(CssPropertyValue styleNamePropertyValue)
        {
            if ((styleNamePropertyValue.Count == 1) &&
                (styleNamePropertyValue.FirstValue.ValueType == CssValueType.Identifier))
            {
                string cssStyleName = ((CssIdentifierValue)styleNamePropertyValue.FirstValue).Value;

                int styleIdentifierValue = gParagraphStyleNamesMap.TryGetValue(cssStyleName);
                if (!StringToIntBidirectionalMap.IsNullSubstitute(styleIdentifierValue))
                {
                    return (StyleIdentifier)styleIdentifierValue;
                }

                styleIdentifierValue = gCharacterStyleNamesMap.TryGetValue(cssStyleName);
                if (!StringToIntBidirectionalMap.IsNullSubstitute(styleIdentifierValue))
                {
                    return (StyleIdentifier)styleIdentifierValue;
                }
            }

            return StyleIdentifier.Nil;
        }

        internal static string StyleIdentifierToCssStyleName(StyleIdentifier styleIdentifier)
        {
            string identifier = gParagraphStyleNamesMap.TryGetValue((int)styleIdentifier);
            if (identifier == null)
            {
                identifier = gCharacterStyleNamesMap.TryGetValue((int)styleIdentifier);
            }
            return identifier;
        }

        internal static bool ValidateStyleType(StyleIdentifier styleIdentifier, StyleType requestedStyleType)
        {
            int key = (int)styleIdentifier;
            if (gParagraphStyleNamesMap.ContainsValue(key))
            {
                return requestedStyleType == StyleType.Paragraph;
            }
            if (gCharacterStyleNamesMap.ContainsValue(key))
            {
                return requestedStyleType == StyleType.Character;
            }
            return false;
        }

        internal static StyleIdentifier[] GetCharacterStyleIdentifiers()
        {
            StyleIdentifier[] identifiers = new StyleIdentifier[gCharacterStyleIdentifiers.Count];
            for (int i = 0; i < gCharacterStyleIdentifiers.Count; i++)
            {
                identifiers[i] = (StyleIdentifier)gCharacterStyleIdentifiers[i];
            }
            return identifiers;
        }

        private static void AddMapEntry(StyleIdentifier styleIdentifier, string cssIdentifier, bool isCharacterStyle)
        {
            if (isCharacterStyle)
            {
                gCharacterStyleNamesMap.AddEntry((int)styleIdentifier, cssIdentifier);
                gCharacterStyleIdentifiers.Add((int)styleIdentifier);
            }
            else
            {
                gParagraphStyleNamesMap.AddEntry((int)styleIdentifier, cssIdentifier);
            }
        }

        /// <summary>
        /// Maps CSS identifiers that we use for built-in paragraph styles to the corresponding <see cref="StyleIdentifier"/>
        /// values.
        /// </summary>
        /// <remarks>Keys are CSS identifiers. Values are <see cref="StyleIdentifier"/> values casted to int.</remarks>
        private static readonly StringToIntBidirectionalMap gParagraphStyleNamesMap;

        /// <summary>
        /// Maps CSS identifiers that we use for built-in character styles to the corresponding <see cref="StyleIdentifier"/>
        /// values.
        /// </summary>
        /// <remarks>Keys are CSS identifiers. Values are <see cref="StyleIdentifier"/> values casted to int.</remarks>
        private static readonly StringToIntBidirectionalMap gCharacterStyleNamesMap;

        /// <summary>
        /// List of built-in character style identifiers.
        /// </summary>
        /// <remarks>Stores <see cref="StyleIdentifier"/> values casted to int.</remarks>
        private static readonly IntList gCharacterStyleIdentifiers;
    }
}
