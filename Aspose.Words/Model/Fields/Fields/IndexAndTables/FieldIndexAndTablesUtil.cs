// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2013 by Ivan Lyagin

using System;
using System.IO;
using Aspose.Drawing.Fonts;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides utility methods shared between INDEX, TOC and TOA fields.
    /// </summary>
    internal class FieldIndexAndTablesUtil
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private FieldIndexAndTablesUtil()
        {
            // Hide from using.
        }

        /// <summary>
        /// Prepares and returns a <see cref="DocumentBuilder"/> instance used to write an INDEX, TOC or TOA field result.
        /// </summary>
        internal static DocumentBuilder GetDocumentBuilder(Field field)
        {
            DocumentBuilder builder = new DocumentBuilder(field.FetchDocument());

            MoveOutInlineSdt(field);

            if (RequiresSplitResultFromTrailingNodes(field) && !IsFirstNonZeroLengthChild(field))
            {
                // WORDSNET-9673 If field occupies more than one paragraphs, insert paragraph with attributes from first one.
                if (field.Separator.ParentNode != field.End.ParentNode)
                    builder.MoveTo(field.Separator.ParentParagraph);
                else
                    builder.MoveTo(field.End);

                ConvertOuterInlineSdtToBlock(field);

                builder.Writeln();
            }

            builder.MoveTo(field.End);

            return builder;
        }

        private static void MoveOutInlineSdt(Field field)
        {
            FieldStart start = field.Start;
            StructuredDocumentTag sdt = start.ParentNode as StructuredDocumentTag;
            if (sdt == null)
                return;

            Debug.Assert(sdt.Level == MarkupLevel.Inline);

            if (sdt.IsFirstNonZeroLengthChild)
                return;

            sdt.InsertNext(field.Start, null);
        }

        internal static bool IsFirstNonZeroLengthChild(Field field)
        {
            FieldStart start = field.Start;
            if (!start.IsFirstNonZeroLengthChild)
                return false;

            StructuredDocumentTag sdt = start.ParentNode as StructuredDocumentTag;
            if (sdt == null)
                return true;

            Debug.Assert(sdt.Level == MarkupLevel.Inline);

            return sdt.IsFirstNonZeroLengthChild;
        }

        internal static void ConvertOuterInlineSdtToBlock(Field field)
        {
#if CPLUSPLUS
            StructuredDocumentTag sdt = (StructuredDocumentTag)field.Start.GetAncestorOf<StructuredDocumentTag>();
#else
            StructuredDocumentTag sdt = (StructuredDocumentTag)field.Start.GetAncestor(typeof(StructuredDocumentTag));
#endif

            if (sdt == null)
                return;

            if (sdt.Level != MarkupLevel.Inline)
                return;

            sdt.ConvertToBlock();
        }

        private static bool RequiresSplitResultFromTrailingNodes(Field field)
        {
            switch (field.Type)
            {
                case FieldType.FieldIndex:
                case FieldType.FieldTOC:
                    return true;
                case FieldType.FieldTOA:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Creates a paragraph to write an INDEX, TOC or TOA entry and sets its properties.
        /// </summary>
        internal static void SetUpEntryParagraph(Field field, DocumentBuilder builder, StyleIdentifier styleIdentifier)
        {
            // One entry - one paragraph. Also we separate the entry paragraph from the End's parent.
            builder.MoveTo(field.End);
            builder.Writeln();
            builder.MoveTo(builder.CurrentParagraph.PreviousNonAnnotationSibling);

            ParaPr fieldStartParaPr =  field.Start.ParentParagraph.ParaPr.Clone();

            bool isBidiTextSupported = builder.Document.FieldOptions.IsBidiTextSupportedOnUpdate;
            bool isBidiParagraph = isBidiTextSupported && builder.ParagraphFormat.Bidi;

            // Clear paragraph format but restore BIDI.
            builder.ParagraphFormat.ClearFormatting();
            builder.ParagraphFormat.StyleIdentifier = styleIdentifier;
            if (isBidiParagraph)
                builder.ParagraphFormat.Bidi = true;

            bool isBidiParagraphBreak = isBidiTextSupported && builder.CurrentParagraph.ParagraphBreakFont.Bidi;

            // Clear paragraph break format but restore BIDI.
            builder.CurrentParagraph.ParagraphBreakFont.ClearFormatting();
            if (isBidiParagraphBreak)
                builder.CurrentParagraph.ParagraphBreakFont.Bidi = true;

            // andrnosk: WORDSNET-8500 Copy frame attributes to TOC entry paragraph,
            // we have to preserve frame related attributes.
            fieldStartParaPr.MirrorTo(builder.CurrentParagraph.ParaPr, gFrameAttrs);
        }

        /// <summary>
        /// Sets font properties for an INDEX or TOC entry's paragraph break or tab stop.
        /// </summary>
        internal static void SetFontForTabStopOrParagraphBreak(Document document, IRunAttrSource runAttrSource)
        {
            bool isBidiTextSupported = document.FieldOptions.IsBidiTextSupportedOnUpdate;

            if (document.GetThemeInternal() != null)
            {
                runAttrSource.SetRunAttr(FontAttr.NameAscii, ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi));
                runAttrSource.SetRunAttr(FontAttr.NameOther, ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi));

                if (isBidiTextSupported)
                {
                    runAttrSource.SetRunAttr(FontAttr.NameBi, ComplexFontName.FromTheme(ThemeFontCore.MinorBidi));
                    runAttrSource.SetRunAttr(FontAttr.NameFarEast, ComplexFontName.FromTheme(ThemeFontCore.MinorEastAsia));
                }
            }
            else
            {
                runAttrSource.SetRunAttr(FontAttr.NameAscii, ComplexFontName.FromName(DefaultEntryFontName));
                runAttrSource.SetRunAttr(FontAttr.NameOther, ComplexFontName.FromName(DefaultEntryFontName));

                if (isBidiTextSupported)
                {
                    runAttrSource.SetRunAttr(FontAttr.NameBi, ComplexFontName.FromName(DefaultEntryFontNameBi));
                    runAttrSource.SetRunAttr(FontAttr.NameFarEast, ComplexFontName.FromName(DefaultEntryFontName));
                }
            }

            runAttrSource.SetRunAttr(FontAttr.Size, DefaultEntryFontSize);
            if (isBidiTextSupported)
                runAttrSource.SetRunAttr(FontAttr.SizeBi, DefaultEntryFontSize);

            runAttrSource.SetRunAttr(FontAttr.NoProofing, AttrBoolEx.True);
        }

        /// <summary>
        /// Checks whether the current paragraph of the specified document builder has a predefined tab stop
        /// for an INDEX, TOC or TOA entry paragraph and appends one if it has not.
        /// </summary>
        internal static void EnsureEntryParagraphTabStop(DocumentBuilder builder, Paragraph sourceParagraph)
        {
            if (IsInsideFloatingTable(builder.CurrentParagraph))
                return;

            if (ExtractRightTabStop(builder.CurrentParagraph) != null)
                return;

            Section section = builder.CurrentSection;
            int columnsCount = section.SectPr.ColumnsCount;

            // WORDSNET-8152 Use ContentWidth instead of PageWidth - (LeftMargin + RightMargin)
            // because it takes into account a Gutter.
            double contentWidth = section.PageSetup.ContentWidth;
            double tabStopPosition = contentWidth;
            if (columnsCount > 1)
            {
                // WORDSNET-8457 If the parent section has multiple columns, we need to consider spaces between them and
                // distribute available content width between the columns equally.
                double columnsSpacing = ConvertUtilCore.TwipToPoint(section.SectPr.ColumnsSpacing);
                tabStopPosition = (contentWidth - columnsSpacing*(columnsCount - 1))/columnsCount;
            }

            // WORDSNET-12456 The first paragraph`s tab leader determines TOC's tab leaders.
            TabStop stop = ExtractRightTabStop(sourceParagraph);
            TabLeader leader = (stop != null) ? stop.Leader : TabLeader.Dots;

            tabStopPosition -= 0.5; // MS Word does it.

            AddTabStopToEntryParagraph(builder.CurrentParagraph, tabStopPosition, TabAlignment.Right, leader);
        }

        private static bool IsInsideFloatingTable(Paragraph paragraph)
        {
            Table table = paragraph.ParentTable;
            if (table == null)
                return false;

            return table.IsFloating;
        }

        private static TabStop ExtractRightTabStop(Paragraph paragraph)
        {
            if (paragraph == null)
                return null;

            ParaPr paraPr = paragraph.GetExpandedParaPr(ParaPrExpandFlags.Normal);
            TabStopCollection stops = paraPr.TabStops;

            if (stops == null)
                return null;

            if (stops.Count == 0)
                return null;

            if (stops[stops.Count - 1].Alignment != TabAlignment.Right)
                return null;

            return stops[stops.Count - 1];
        }

        /// <summary>
        /// Creates and appends tab stop with the given properties to the INDEX, TOC or TOA entry paragraph's collection
        /// if a tab stop with the specified position has not been added to it yet.
        /// </summary>
        private static void AddTabStopToEntryParagraph(
            Paragraph paragraph,
            double position,
            TabAlignment alignment,
            TabLeader leader)
        {
            // All TOCN styles are auto redefined, but AW does not support this feature at the moment.
            // We will do that ourselves if needed.
            ParagraphFormat entryParagraphFormat = paragraph.ParagraphFormat;
            TabStop tabStop = entryParagraphFormat.Style.ParagraphFormat.TabStops[position];
            if (tabStop == null || tabStop.IsClear)
                entryParagraphFormat.TabStops.Add(position, alignment, leader);
        }

        /// <summary>
        /// Creates and inserts a dummy (i.e. temporary) reference node to an INDEX, TOC or TOA entry paragraph.
        /// </summary>
        internal static Node CreateAndInsertDummyRefNode(DocumentBuilder builder)
        {
            Node dummyRefNode = new Run(builder.Document);
            builder.CurrentParagraph.AppendChild(dummyRefNode);
            return dummyRefNode;
        }

        /// <summary>
        /// Gets an internal representation of the given entry type.
        /// </summary>
        private static int GetEntryType(string entryType)
        {
            if (entryType == null)
                return NullEntryType;

            // We should be able to distinguish null and empty strings as they are processed differently by MS Word.
            if (entryType.Length == 0)
                return EmptyEntryType;

            // MS Word considers only the first character of an entry type string.
            return char.ToUpperInvariant(entryType[0]);
        }

        /// <summary>
        /// Gets an internal representation of the given TOC entry type.
        /// </summary>
        internal static int GetTocEntryType(FieldCode fieldCode, string switchName)
        {
            if (!fieldCode.HasSwitch(switchName))
                return MissingEntryType;

            string entryType = fieldCode.GetSwitchArgumentAsString(switchName);

            return GetEntryType(entryType);
        }

        /// <summary>
        /// Gets an internal representation of the given INDEX entry type.
        /// </summary>
        internal static int GetIndexEntryType(string entryType)
        {
            return GetEntryType(entryType);
        }

        internal static void EnsureTocEntryTabStop(Run run, int entryLevel, bool isTableOfFigures)
        {
            Debug.Assert(run.Text == ControlChar.Tab);
            NodeRange range = new NodeRange(run.ParentParagraph.FirstChild, true, run, false);
            string text = NodeTextCollector.GetText(range, true);

            double tabStopPosition = GetTabStopPosition(run.FetchDocument(), run.Font, text, entryLevel, isTableOfFigures);

            if (!MathUtil.IsMinValue(tabStopPosition))
                AddTabStopToEntryParagraph(run.ParentParagraph, tabStopPosition, TabAlignment.Left, TabLeader.None);

            SetFontForTabStopOrParagraphBreak(run.FetchDocument(), run);
        }

        private static double GetTabStopPosition(Document document, Font font, string text, int level, bool isTableOfFigures)
        {
            const double minIndentPaddingWidth = 11d;
            const double minHangingPaddingWidth = 4d;

            DrFont drawingFont = document.FontProvider.FetchDrFont(font.Name, (float)font.Size, font.FontStyle);

            double listLabelWidth = drawingFont.GetTextWidthPoints(text);

            ParagraphFormat paragraphFormat = GetParagraphFormatForLevel(level, document, isTableOfFigures);

            double firstLineIndent = paragraphFormat.FirstLineIndent;
            double leftIndent = paragraphFormat.LeftIndent;
            double paddingPosition = leftIndent + firstLineIndent + listLabelWidth;

            bool isHanging = firstLineIndent < 0;

            double tabStopPosition = GetTabStopPositionFromTocStyle(paddingPosition, paragraphFormat);

            if (MathUtil.IsMinValue(tabStopPosition))
            {
                paddingPosition += !isHanging
                    ? minIndentPaddingWidth
                    : minHangingPaddingWidth;

                tabStopPosition = GetDefaultTabStopPosition(level, paddingPosition, document);
            }

            // WORDSNET-18715 Paragraph hanging aware
            if (isHanging && MathUtil.IsLessOrEqual(tabStopPosition, -firstLineIndent))
                return double.MinValue;

            return tabStopPosition;
        }

        private static double GetTabStopPositionFromTocStyle(double paddingPosition, ParagraphFormat paragraphFormat)
        {
            double firstLineIndent = paragraphFormat.FirstLineIndent;
            double leftIndent = paragraphFormat.LeftIndent;

            double paddingWidth2 = firstLineIndent < 0 ? leftIndent - paddingPosition : 0;

            TabStopCollection paraTabStops = paragraphFormat.TabStops;
            TabStop tabStop = paraTabStops.After(paddingPosition);

            bool considerUseTocStyleTabStop = (tabStop != null) && (tabStop.Alignment == TabAlignment.Left);

            if (considerUseTocStyleTabStop)
            {
                double paddingWidth = tabStop.Position - paddingPosition;

                if ((paddingWidth2 > 0) && (paddingWidth2 < paddingWidth))
                    paddingWidth = paddingWidth2;

                if (paddingWidth > 0)
                    return paddingPosition + paddingWidth;
            }

            return double.MinValue;
        }

        private static double GetDefaultTabStopPosition(int entryLevel, double paddingPosition, Document document)
        {
            const int maxEntryLevel = 9;

            ParagraphFormat paragraphFormat = GetParagraphFormatForLevel(entryLevel, document, false);

            double firstLineIndent = paragraphFormat.FirstLineIndent;
            double leftIndent = paragraphFormat.LeftIndent;

            double paddingWidth2 = firstLineIndent < 0 ? leftIndent - paddingPosition : 0;

            double paddingWidth = 0;
            if (paddingWidth2 > 0)
                paddingWidth = paddingWidth2;

            if (paddingWidth > 0)
                return paddingPosition + paddingWidth;

            if (entryLevel < maxEntryLevel)
            {
                ParagraphFormat nextLevelParagraphFormat = GetParagraphFormatForLevel(entryLevel + 1, document, false);
                double nextLevelIndent = nextLevelParagraphFormat.LeftIndent;
                if (nextLevelIndent - paddingPosition > 0)
                    return nextLevelIndent;

                return GetDefaultTabStopPosition(entryLevel + 1, paddingPosition, document);
            }

            return paddingPosition;
        }

        private static ParagraphFormat GetParagraphFormatForLevel(int entryLevel, Document document, bool isTableOfFigures)
        {
            const double defaultTocEntryLeftIndentStep = 11d;

            StyleIdentifier styleIdentifier = FieldToc.GetStyleIdentifierForLevel(entryLevel, isTableOfFigures);
            Style style = document.Styles.GetBySti(styleIdentifier, false);

            if (style != null)
            {
                // WORDSNET-16347 Take into account base styles.
                ParaPr paraPr = style.GetExpandedParaPr(ParaPrExpandFlags.Normal);
                // Bind lifetime of paraPr for C++ porting. Otherwise NullReferenceException will be thrown,
                // since paraPr will becode out of scope.
                return CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(new ParagraphFormat(paraPr, document.Styles), paraPr);
            }

            // Correct built-in TOC styles to make the TOC look like updated by Word 2010.
            style = document.Styles.BuiltInStyles[styleIdentifier];
            IParaAttrSource styleParaPr = style.ParaPr.Clone();
            // Bind lifetime of styleParaPr for C++ porting. Otherwise NullReferenceException will be thrown,
            // since styleParaPr will becode out of scope.
            ParagraphFormat paragraphFormat = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(new ParagraphFormat(styleParaPr, document.Styles), styleParaPr);
            paragraphFormat.LeftIndent = (entryLevel - 1) * defaultTocEntryLeftIndentStep;

            return paragraphFormat;
        }

        internal static Document OpenRefDocument(FieldRD field)
        {
            string fileName = GetRefDocFilePath(field);
            if (string.IsNullOrEmpty(fileName))
                return null;

            using (Stream stream = FieldUtil.OpenStream(fileName, field.Document.ResourceLoadingCallback))
            {
                return stream != null
                    ? new Document(stream, null, false)
                    : null;
            }
        }

        private static string GetRefDocFilePath(FieldRD field)
        {
            string fileName = field.FileName;
            if (string.IsNullOrEmpty(fileName))
                return null;

            Document document = field.FetchDocument();

            if (field.IsPathRelative && !string.IsNullOrEmpty(document.OriginalFileName))
                return Path.Combine(Path.GetDirectoryName(document.OriginalFileName), fileName);

            return fileName;
        }

        internal const int MissingEntryType = -1;
        internal const int NullEntryType = -2;
        private const int EmptyEntryType = -3;

        // Most likely these values are taken from some source like a standart TOCN style or a TOCN style from template.
        // Need to investigate further.
        private const string DefaultEntryFontName = "Calibri";
        private const string DefaultEntryFontNameBi = "Arial";
        private const int DefaultEntryFontSize = 22;

        private static readonly int[] gFrameAttrs =
        {
            ParaAttr.FrameLeft,
            ParaAttr.FrameTop,
            ParaAttr.FrameWrapType,
            ParaAttr.FrameWidth,
            ParaAttr.FrameHeight,
            ParaAttr.FrameLockAnchor,
            ParaAttr.FrameHorizontalDistanceFromText,
            ParaAttr.FrameVerticalDistanceFromText,
            ParaAttr.FrameRelativeVerticalPosition,
            ParaAttr.FrameRelativeHorizontalPosition,
            ParaAttr.FrameHorizontalAlignment,
            ParaAttr.FrameVerticalAlignment,
            ParaAttr.FrameTextOrientation,
            ParaAttr.FrameSuppressOverlap
        };
    }
}
