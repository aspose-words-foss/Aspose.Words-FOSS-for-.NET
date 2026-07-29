// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2020 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Words.Lists;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Removes duplicate styles with replacing references to them.
    /// </summary>
    internal class DuplicateStyleRemover : IstdVisitor
    {
        /// <summary>
        /// Removes duplicate styles in the specified document.
        /// </summary>
        internal static void Execute(DocumentBase doc)
        {
            DuplicateStyleRemover remover = new DuplicateStyleRemover();
            remover.ExecuteCore(doc);
        }

        protected override void OnRunPr(RunPr runPr)
        {
            ReplaceStyle(runPr, FontAttr.Istd);
        }

        protected override void OnParaPr(ParaPr paraPr)
        {
            ReplaceStyle(paraPr, ParaAttr.Istd);
        }

        protected override void OnRowPr(TablePr tablePr)
        {
            ReplaceStyle(tablePr, TableAttr.Istd);
        }

        protected override void OnDocPr(DocPr docPr)
        {
            int newIstd;

            if (mReplacingStyleMap.TryGetValue(docPr.ClickTypeParaStyleIstd, out newIstd))
                docPr.ClickTypeParaStyleIstd = newIstd;

            if (mReplacingStyleMap.TryGetValue(docPr.DefaultTableStyleIstd, out newIstd))
                docPr.DefaultTableStyleIstd = newIstd;
        }

        /// <summary>
        /// Removes duplicate styles in the specified document.
        /// </summary>
        private void ExecuteCore(DocumentBase doc)
        {
            FillReplacingStyleMap(doc.Styles);

            if (mStylesToRemove.Count == 0)
                return;

            foreach (Style style in mStylesToRemove)
                doc.Styles.RemoveCore(style);

            // Visit all document content to replace style references.
            Run(doc);

            ReplaceBaseLinkedAndNextStyles(doc);
            ReplaceListStyles(doc);
        }

        /// <summary>
        /// Replaces references to base, linked and next style in document styles.
        /// </summary>
        private void ReplaceBaseLinkedAndNextStyles(DocumentBase doc)
        {
            foreach (Style style in doc.Styles)
            {
                int newIstd;

                if ((style.LinkedIstd != StyleIndex.Nil) && mReplacingStyleMap.TryGetValue(style.LinkedIstd, out newIstd))
                    style.LinkedIstd = newIstd;

                if ((style.BasedOnIstd != StyleIndex.Nil) && mReplacingStyleMap.TryGetValue(style.BasedOnIstd, out newIstd))
                    style.BasedOnIstd = newIstd;

                if ((style.NextIstd != StyleIndex.Nil) && mReplacingStyleMap.TryGetValue(style.NextIstd, out newIstd))
                    style.NextIstd = newIstd;
            }
        }

        /// <summary>
        /// Replaces style references in lists.
        /// </summary>
        private void ReplaceListStyles(DocumentBase doc)
        {
            foreach (ListDef listDef in doc.Lists.ListDefs)
            {
                int newIstd;
                if (mReplacingStyleMap.TryGetValue(listDef.ListStyleIstd, out newIstd))
                    listDef.ListStyleIstd = newIstd;

                foreach (ListLevel level in listDef.Levels)
                {
                    if (mReplacingStyleMap.TryGetValue(level.ParaStyleIstd, out newIstd))
                        level.ParaStyleIstd = newIstd;
                }
            }
        }

        /// <summary>
        /// Fills the map of correspondence between replacing style Istd and new style Istd.
        /// </summary>
        private void FillReplacingStyleMap(StyleCollection styles)
        {
            Dictionary<int, List<Style>> hashes = GetStylesByHashMap(styles);
            List<Style> equalStyles = new List<Style>();

            foreach (List<Style> list in hashes.Values)
            {
                if (list.Count == 1)
                    continue; // No style duplicates.

                while (ExtractEqualStyles(list, equalStyles))
                    AddToReplacingStyleMap(equalStyles);
            }
        }

        /// <summary>
        /// Loops through the styles and extracts the first non-null style and all styles that are equal to it, if any.
        /// Returns a flag indicating whether any styles have been extracted.
        /// </summary>
        /// <remarks>
        /// The extracted styles are replaced with <c>null</c> in the input style list.
        /// The output list of styles is ordered: the first item is the style to be preserved in the document.
        /// </remarks>
        private static bool ExtractEqualStyles(List<Style> styles, List<Style> extractedEqualStyles)
        {
            extractedEqualStyles.Clear();
            int minIstdStyleIndex = -1; // Index of a style that has the smallest Istd.

            for (int i = 0; i < styles.Count; i++)
            {
                Style style = styles[i];
                if (style == null)
                    continue;

                // Skip a style if it doesn't equal to the first one.
                if ((extractedEqualStyles.Count > 0) && !style.Equals(extractedEqualStyles[0]))
                    continue;

                // Let's keep a style with the lowest Istd so that built-in styles take precedence.
                if (CanReplaceWithStyle(style) &&
                    ((minIstdStyleIndex < 0) || (extractedEqualStyles[minIstdStyleIndex].Istd > style.Istd)))
                {
                    minIstdStyleIndex = extractedEqualStyles.Count;
                }

                extractedEqualStyles.Add(style);

                styles[i] = null;
            }

            bool hasExtracted = extractedEqualStyles.Count > 0;

            if (minIstdStyleIndex < 0)
            {
                // There is no style that can be used to replace other styles with: clear the list.
                extractedEqualStyles.Clear();
            }
            else if (minIstdStyleIndex < 0)
            {
                // Move the style that will be used to replace other styles to the top of the list.
                Style tmp = extractedEqualStyles[0];
                extractedEqualStyles[0] = extractedEqualStyles[minIstdStyleIndex];
                extractedEqualStyles[minIstdStyleIndex] = tmp;
            }

            return hasExtracted;
        }

        /// <summary>
        /// Add styles to the style replacement map. The first item of the list is a style to preserve.
        /// </summary>
        private void AddToReplacingStyleMap(List<Style> styles)
        {
            if (styles.Count < 2)
                return; // No duplicate styles.

            int istdToReplaceWith = styles[0].Istd;

            foreach (Style style in styles)
            {
                if ((style.Istd == istdToReplaceWith) || !CanReplaceStyle(style))
                    continue;

                mReplacingStyleMap.Add(style.Istd, istdToReplaceWith);
                mStylesToRemove.Add(style);
            }
        }

        /// <summary>
        /// Gets a map of styles by their hash codes.
        /// </summary>
        private static Dictionary<int, List<Style>> GetStylesByHashMap(StyleCollection styles)
        {
            Dictionary<int, List<Style>> map = new Dictionary<int, List<Style>>();

            foreach (Style style in styles)
            {
                if (!CanReplaceWithStyle(style) && !CanReplaceStyle(style))
                    continue;

                int hashCode = GetStyleHashCodeForEquality(style);

                List<Style> list = map.GetValueOrNull(hashCode);
                if (list == null)
                {
                    list = new List<Style>();
                    map[hashCode] = list;
                }

                list.Add(style);
            }

            return map;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified style can be replaced with another style.
        /// </summary>
        private static bool CanReplaceStyle(Style style)
        {
            if (style.StyleIdentifier != StyleIdentifier.User)
                return false;

            // Let's do not remove styles that are linked to system styles. For example, 'Footer Char'. Such character
            // styles have no special style identifier.
            if (style.LinkedIstd != StyleIndex.Nil)
            {
                Style linkedStyle = style.Document.Styles.GetByIstd(style.LinkedIstd, false);
                return (linkedStyle == null) || (linkedStyle.StyleIdentifier == StyleIdentifier.User);
            }

            return true;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified style can be used to replace another style.
        /// </summary>
        private static bool CanReplaceWithStyle(Style style)
        {
            // Let's do not replace custom styles with special styles, e.g. with Heading, CommentReference and so on.
            // Since assignment of some styles may be an indicator of a certain type of a run/paragraph.

            if (!CanReplaceWithStyleCore(style))
                return false;

            // Let's exclude styles that are linked to the special styles. For example, 'Footer Char'. Such character
            // styles have no special style identifier.
            if (style.LinkedIstd != StyleIndex.Nil)
            {
                Style linkedStyle = style.Document.Styles.GetByIstd(style.LinkedIstd, false);
                return (linkedStyle == null) || CanReplaceWithStyleCore(linkedStyle);
            }

            return true;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified style can be used to replace another style.
        /// </summary>
        private static bool CanReplaceWithStyleCore(Style style)
        {
            // Let's do not replace custom styles with special styles, e.g. with Heading, CommentReference and so on.
            // Since assignment of some styles may be an indicator of a certain type of a run/paragraph.
            switch (style.StyleIdentifier)
            {
                case StyleIdentifier.BookTitle:
                case StyleIdentifier.CommentReference:
                case StyleIdentifier.Emphasis:
                case StyleIdentifier.EndnoteReference:
                case StyleIdentifier.FollowedHyperlink:
                case StyleIdentifier.FootnoteReference:
                case StyleIdentifier.HtmlAcronym:
                case StyleIdentifier.HtmlCite:
                case StyleIdentifier.HtmlCode:
                case StyleIdentifier.HtmlDefinition:
                case StyleIdentifier.HtmlKeyboard:
                case StyleIdentifier.HtmlSample:
                case StyleIdentifier.HtmlTypewriter:
                case StyleIdentifier.HtmlVariable:
                case StyleIdentifier.Hyperlink:
                case StyleIdentifier.IntenseEmphasis:
                case StyleIdentifier.IntenseReference:
                case StyleIdentifier.LineNumber:
                case StyleIdentifier.PageNumber:
                case StyleIdentifier.PlaceholderText:
                case StyleIdentifier.SubtleEmphasis:
                case StyleIdentifier.SubtleReference:
                case StyleIdentifier.BalloonText:
                case StyleIdentifier.Closing:
                case StyleIdentifier.CommentSubject:
                case StyleIdentifier.CommentText:
                case StyleIdentifier.Date:
                case StyleIdentifier.DocumentMap:
                case StyleIdentifier.EmailSignature:
                case StyleIdentifier.EndnoteText:
                case StyleIdentifier.Footer:
                case StyleIdentifier.FootnoteText:
                case StyleIdentifier.Header:
                case StyleIdentifier.Heading1:
                case StyleIdentifier.Heading2:
                case StyleIdentifier.Heading3:
                case StyleIdentifier.Heading4:
                case StyleIdentifier.Heading5:
                case StyleIdentifier.Heading6:
                case StyleIdentifier.Heading7:
                case StyleIdentifier.Heading8:
                case StyleIdentifier.Heading9:
                case StyleIdentifier.HtmlAddress:
                case StyleIdentifier.HtmlTopOfForm:
                case StyleIdentifier.HtmlBottomOfForm:
                case StyleIdentifier.HtmlPreformatted:
                case StyleIdentifier.IntenseQuote:
                case StyleIdentifier.Macro:
                case StyleIdentifier.MessageHeader:
                case StyleIdentifier.NoteHeading:
                case StyleIdentifier.PlainText:
                case StyleIdentifier.Quote:
                case StyleIdentifier.Salutation:
                case StyleIdentifier.Signature:
                case StyleIdentifier.Subtitle:
                case StyleIdentifier.Title:
                case StyleIdentifier.Bibliography:
                case StyleIdentifier.BlockText:
                case StyleIdentifier.Caption:
                case StyleIdentifier.EnvelopeAddress:
                case StyleIdentifier.EnvelopeReturn:
                case StyleIdentifier.Index1:
                case StyleIdentifier.Index2:
                case StyleIdentifier.Index3:
                case StyleIdentifier.Index4:
                case StyleIdentifier.Index5:
                case StyleIdentifier.Index6:
                case StyleIdentifier.Index7:
                case StyleIdentifier.Index8:
                case StyleIdentifier.Index9:
                case StyleIdentifier.IndexHeading:
                case StyleIdentifier.List:
                case StyleIdentifier.List2:
                case StyleIdentifier.List3:
                case StyleIdentifier.List4:
                case StyleIdentifier.List5:
                case StyleIdentifier.ListBullet:
                case StyleIdentifier.ListBullet2:
                case StyleIdentifier.ListBullet3:
                case StyleIdentifier.ListBullet4:
                case StyleIdentifier.ListBullet5:
                case StyleIdentifier.ListContinue:
                case StyleIdentifier.ListContinue2:
                case StyleIdentifier.ListContinue3:
                case StyleIdentifier.ListContinue4:
                case StyleIdentifier.ListContinue5:
                case StyleIdentifier.ListNumber:
                case StyleIdentifier.ListNumber2:
                case StyleIdentifier.ListNumber3:
                case StyleIdentifier.ListNumber4:
                case StyleIdentifier.ListNumber5:
                case StyleIdentifier.ListParagraph:
                case StyleIdentifier.NoSpacing:
                case StyleIdentifier.NormalWeb:
                case StyleIdentifier.NormalIndent:
                case StyleIdentifier.TableOfAuthorities:
                case StyleIdentifier.TableOfFigures:
                case StyleIdentifier.ToaHeading:
                case StyleIdentifier.Toc1:
                case StyleIdentifier.Toc2:
                case StyleIdentifier.Toc3:
                case StyleIdentifier.Toc4:
                case StyleIdentifier.Toc5:
                case StyleIdentifier.Toc6:
                case StyleIdentifier.Toc7:
                case StyleIdentifier.Toc8:
                case StyleIdentifier.Toc9:
                case StyleIdentifier.TocHeading:
                case StyleIdentifier.Revision:
                case StyleIdentifier.OutlineList1:
                case StyleIdentifier.OutlineList2:
                case StyleIdentifier.OutlineList3:
                case StyleIdentifier.NoList:
                case StyleIdentifier.Nil:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Replaces a style definition in the specified property collection.
        /// </summary>
        /// <remarks>
        /// The <paramref name="styleAttribute"/> parameter is an attribute that is used to store style Istd values.
        /// </remarks>
        private void ReplaceStyle(AttrCollection properties, int styleAttribute)
        {
            object value = properties.GetDirectAttr(styleAttribute);
            if (value == null)
                return;

            int istd = (int)value;
            int newIstd;
            if (mReplacingStyleMap.TryGetValue(istd, out newIstd))
                properties.SetAttr(styleAttribute, newIstd);
        }

        /// <summary>
        /// Calculates hash code that is compatible with the <see cref="Style.Equals(Style)"/> method.
        /// </summary>
        /// <dev>
        /// Some style properties are not included in hash calculation similar to the Style.Equal method. Because of this
        /// and due to complexity of the code, it is not implemented as Style.GetHashCode, but is placed in this class.
        /// The method should be changed together with the Style.Equals methods.
        /// Only properties that affect visual representation of a style are included into the calculation.
        /// </dev>
        internal static int GetStyleHashCodeForEquality(Style style)
        {
            return GetStyleHashCodeForEquality(style, true);
        }

        /// <summary>
        /// Calculates hash code with ability to define whether base/linked/next styles should be processed.
        /// </summary>
        /// <remarks>
        /// The returned hash code is compatible with the <see cref="Style.Equals(Style)"/> method.
        /// </remarks>
        /// <dev>
        /// Some style properties are not included in hash calculation similar to the Style.Equal method. Because of this
        /// and due to complexity of the code, it is not implemented as Style.GetHashCode, but is placed in this class.
        /// The method should be changed together with the Style.Equals methods.
        /// Only properties that affect visual representation of a style are included into the calculation.
        /// </dev>
        private static int GetStyleHashCodeForEquality(Style style, bool includeReferencedStyles)
        {
            int hashCode = style.Type.GetHashCode();

            hashCode = (hashCode * 397) ^ style.IsHeading.GetHashCode();
            hashCode = (hashCode * 397) ^ style.IsQuickStyle.GetHashCode();
            hashCode = (hashCode * 397) ^ style.AutomaticallyUpdate.GetHashCode();
            hashCode = (hashCode * 397) ^ style.Locked.GetHashCode();
            hashCode = (hashCode * 397) ^ style.UnhideWhenUsed.GetHashCode();
            hashCode = (hashCode * 397) ^ style.PersonalCompose.GetHashCode();
            hashCode = (hashCode * 397) ^ style.PersonalReply.GetHashCode();
            hashCode = (hashCode * 397) ^ style.Personal.GetHashCode();
            hashCode = (hashCode * 397) ^ style.LidsSet.GetHashCode();
            hashCode = (hashCode * 397) ^ style.IsTopLevelParaStyle.GetHashCode();
            hashCode = (hashCode * 397) ^ style.HasRevisions.GetHashCode();

            switch (style.Type)
            {
                case StyleType.Character:
                    hashCode = (hashCode * 397) ^ style.RunPr.GetHashCode(Style.ComparisonIgnorableKeys);
                    break;
                case StyleType.Paragraph:
                {
                    hashCode = (hashCode * 397) ^ style.RunPr.GetHashCode(Style.ComparisonIgnorableKeys);
                    hashCode = (hashCode * 397) ^ style.ParaPr.GetHashCode(Style.ComparisonIgnorableKeys);
                    break;
                }
                case StyleType.Table:
                {
                    hashCode = (hashCode * 397) ^ style.RunPr.GetHashCode(Style.ComparisonIgnorableKeys);
                    hashCode = (hashCode * 397) ^ style.ParaPr.GetHashCode(Style.ComparisonIgnorableKeys);

                    TableStyle tableStyle = (TableStyle)style;

                    foreach (ConditionalStyle conditionalStyle in tableStyle.ConditionalStyles.DefinedStyles)
                    {
                        if (conditionalStyle.HasFormatting)
                            hashCode = (hashCode * 397) ^ conditionalStyle.GetHashCode();
                    }

                    hashCode = (hashCode * 397) ^ tableStyle.CellPr.GetHashCode(Style.ComparisonIgnorableKeys);
                    hashCode = (hashCode * 397) ^ tableStyle.RowPr.GetHashCode(Style.ComparisonIgnorableKeys);
                    hashCode = (hashCode * 397) ^ tableStyle.TablePr.GetHashCode(Style.ComparisonIgnorableKeys);
                    break;
                }
                case StyleType.List:
                    // List hash is taken below.
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unexpected style type '{0}'.", style.Type));
            }

            List list = style.GetListInternal();
            if (list != null)
                hashCode = (hashCode * 397) ^ list.GetHashCode();

            if (!includeReferencedStyles)
                return hashCode;

            // To prevent cycling, collect all referenced styles and take their hash codes with includeReferencedStyles
            // set to 'false'.

            List<Style> referencedStyles = new List<Style>();

            const int maxNesting = 4;
            CollectReferencedStyles(style, referencedStyles, maxNesting, true);

            Dictionary<int, int> hashByIstd = new Dictionary<int, int>();
            hashByIstd.Add(style.Istd, hashCode);

            foreach (Style referencedStyle in referencedStyles)
            {
                int hash;
                if (!hashByIstd.TryGetValue(referencedStyle.Istd, out hash))
                {
                    hash = GetStyleHashCodeForEquality(referencedStyle, false);
                    hashByIstd.Add(referencedStyle.Istd, hash);
                }

                hashCode = (hashCode * 397) ^ hash;
            }

            return hashCode;
        }

        /// <summary>
        /// Recursively collects all base, linked and next styles of the specified style.
        /// </summary>
        /// <param name="style">A style to collect references of.</param>
        /// <param name="styleList">A destination style list.</param>
        /// <param name="nesting">A variable that is decreased on each recursion. It is used to prevent cycling: when
        /// its value becomes 0, recursion is stopped.</param>
        /// <param name="takeAllBaseStyles">When <c>true</c>, all base styles are collected even if
        /// <paramref name="nesting"/> is 0.</param>
        private static void CollectReferencedStyles(Style style, List<Style> styleList, int nesting,
            bool takeAllBaseStyles)
        {
            if (takeAllBaseStyles || nesting > 0)
                CollectReferencedStyle(style.GetBaseStyle(), styleList, nesting, takeAllBaseStyles);

            if (nesting > 0)
            {
                CollectReferencedStyle(style.GetLinkedStyle(), styleList, nesting, false);
                CollectReferencedStyle(style.GetNextStyle(), styleList, nesting, false);
            }
        }

        /// <summary>
        /// Adds the specified style and its base, linked and next styles to the specified style list with recursion if
        /// the nesting value is not achieved 0 value yet.
        /// </summary>
        /// <param name="style">A style to add to the list.</param>
        /// <param name="styleList">A destination style list.</param>
        /// <param name="nesting">A variable that is decreased on each recursion. It is used to prevent cycling: when
        /// its value becomes 0, recursion is stopped.</param>
        /// <param name="takeAllBaseStyles">When <c>true</c>, all base styles are collected even if
        /// <paramref name="nesting"/> is 0.</param>
        private static void CollectReferencedStyle(Style style, List<Style> styleList, int nesting,
            bool takeAllBaseStyles)
        {
            if (style == null)
                return;

            styleList.Add(style);

            if (takeAllBaseStyles || (nesting > 0))
                CollectReferencedStyles(style, styleList, nesting - 1, takeAllBaseStyles);
        }

        /// <summary>
        /// The map of styles to replace: old Istd to new Istd.
        /// </summary>
        private readonly Dictionary<int, int> mReplacingStyleMap = new Dictionary<int, int>();

        private readonly List<Style> mStylesToRemove = new List<Style>();
    }
}
