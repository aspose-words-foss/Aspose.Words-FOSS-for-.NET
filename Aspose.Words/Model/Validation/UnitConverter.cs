// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2012 by Alexey Morozov

using System;
using System.Reflection;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Updates <see cref="ParaPr.SpaceBefore" />, <see cref="ParaPr.SpaceAfter" /> and
    /// <see cref="ParaPr.LeftIndent" />, <see cref="ParaPr.RightIndent" />, <see cref="ParaPr.FirstLineIndent" />
    /// from corresponding unit values if specified.
    /// </summary>
    /// <remarks>
    /// There are several reasons for this code:
    ///  - line/character units are not supported by layout.
    ///  - users have no public API support for line/character units.
    ///  - there are few files in TestData (TestJira4880 for example)
    ///    which have invalid "normal" indents but valid character unit indents.
    ///
    /// So in summary the following behavior is implemented:
    ///   We read/write line/characters units during roundtrip. This allows Word to display indent/spacing as in original file.
    ///   We update "normal" spacing/indents upon load and before save (Normal style can be changed).
    ///   We remove line/character units once user sets "normal" spacing/indents.
    /// </remarks>
    internal class UnitConverter : DocumentVisitor
    {
        /// <summary>
        /// Called when enumeration of the document has started.
        /// </summary>
        public override VisitorAction VisitDocumentStart(Document doc)
        {
            // WORDSNET-28431 Update indents from characters units in styles.
            foreach (Style style in doc.Styles)
                UpdateTwipsFromUnits(style);

            mTwipsPerCharacterUnit = GetTwipsPerCharacterUnit(doc);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Returns twips per character unit of the document.
        /// This is used to compute paragraph left/right indents in twips when they are expressed in character units.
        /// The trick is that first line indent cannot be converted using it because it depends on font size of the first run of the paragraph instead.
        /// </summary>
        private static int GetTwipsPerCharacterUnit(Document doc)
        {
            // Character unit is Normal style font size.
            // Do not modify style collection to avoid side effects.
            Style styleNormal = doc.Styles.GetBySti(StyleIdentifier.Normal, false);

            int fontSize = (styleNormal != null)
                ? (int)styleNormal.GetFontAttr(FontAttr.Size, true)
                : (int)RunPr.FetchDefaultAttr(FontAttr.Size);

            // Font size is half points, hence divide by 2.
            return ConvertUtilCore.PointToTwip(fontSize) / 2;
        }

        public override VisitorAction VisitBodyStart(Body body)
        {
            SectionLayoutMode gridType = body.ParentSection.SectPr.GridType;

            // If document uses "document grid" feature line unit size is specified by LinePitch section property.
            // Otherwise, line unit size is 12 pt.
            mTwipsPerLineUnit = (gridType == SectionLayoutMode.Default)
                ? ConvertUtilCore.PointToTwip(DefaultTwipsPerLine)
                : body.ParentSection.SectPr.LinePitch;

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            // WORDSNET-26551 Word always uses default value for header/footer content.
            mTwipsPerLineUnit = ConvertUtilCore.PointToTwip(DefaultTwipsPerLine);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            UpdateTwipsFromUnits(para, ParaAttr.SpaceBeforeUnits);
            UpdateTwipsFromUnits(para, ParaAttr.SpaceAfterUnits);
            UpdateTwipsFromUnits(para, ParaAttr.RightIndentUnits);

            bool leftUpdated = UpdateTwipsFromUnits(para, ParaAttr.LeftIndentUnits);
            bool firstLineUpdated = UpdateTwipsFromUnits(para, ParaAttr.FirstLineIndentUnits);

            // WORDSNET-19909 Update left indent using hanging even if "FirstLineIndentUnits" is not specified.
            if (firstLineUpdated || leftUpdated)
                ApplyHangingToLeftIndent(para.ParaPr, (int)((IParaAttrSource)para).FetchParaAttr(ParaAttr.FirstLineIndent));

            return VisitorAction.Continue;
        }

        private void UpdateTwipsFromUnits(Style style)
        {
            foreach (int key in gUnitKeys)
            {
                int twipKey = GetTwipsKey(key);

                if (style.ParaPr.ContainsKey(twipKey))
                    continue;

                UpdateTwipsFromUnits(style, style.ParaPr, null, key);

                // AM. Found strange MS Word logic.
                // It removes inherited value for styles after indent calculation, at for FirstLineIndent.
                // Will try to test it deeper during next issues analysis.
                if (key == ParaAttr.FirstLineIndentUnits)
                {
                    object inhValue = GetInheritedValueSafe(style, ParaAttr.FirstLineIndent);

                    if(object.Equals(inhValue, style.ParaPr[ParaAttr.FirstLineIndent]))
                        style.ParaPr.Remove(ParaAttr.FirstLineIndent);
                }
            }
        }

        private bool UpdateTwipsFromUnits(Paragraph para, int unitsKey)
        {
            return UpdateTwipsFromUnits(para, para.ParaPr, para.GetChildNodes(NodeType.Any, false), unitsKey);
        }

        private static int GetTwipsKey(int unitsKey)
        {
            switch (unitsKey)
            {
                case ParaAttr.SpaceBeforeUnits: return ParaAttr.SpaceBefore;
                case ParaAttr.SpaceAfterUnits: return ParaAttr.SpaceAfter;
                case ParaAttr.RightIndentUnits: return ParaAttr.RightIndent;
                case ParaAttr.LeftIndentUnits: return ParaAttr.LeftIndent;
                case ParaAttr.FirstLineIndentUnits: return ParaAttr.FirstLineIndent;

                default:
                    throw new InvalidOperationException("Unexpected key value.");
            }
        }

        /// <summary>
        /// Updates single property in ParaPr.
        /// </summary>
        private bool UpdateTwipsFromUnits(IRunAttrSource runAttrSource, ParaPr paraPr, NodeCollection childNodes, int unitsKey)
        {
            int twipsKey = GetTwipsKey(unitsKey);

            object val = paraPr[unitsKey];

            // WORDSNET-27416 Spacing attributes are not updated from units because of inherited spacing presence.
            // Actually we need to look if any inheritance presents but lets simplify solution for a while.
            if ((paraPr.LineSpacingRule == LineSpacingRule.Exactly) &&
                (((unitsKey == ParaAttr.SpaceBeforeUnits) && (paraPr.SpaceBefore > 0)) ||
                 ((unitsKey == ParaAttr.SpaceAfterUnits) && (paraPr.SpaceAfter > 0)))
               )
                return false;

            if (val == null)
                return false;

            int units = (int) val;

            // It seems that Word ignores 0 value for paragraphs.
            if (units == 0 && runAttrSource is Paragraph)
                return false;

            // twips per unit.
            int tpu = 0;

            switch (unitsKey)
            {
                case ParaAttr.SpaceBeforeUnits:
                case ParaAttr.SpaceAfterUnits:
                    tpu = mTwipsPerLineUnit;
                    break;

                case ParaAttr.RightIndentUnits:
                case ParaAttr.LeftIndentUnits:
                    tpu = mTwipsPerCharacterUnit;
                    break;

                case ParaAttr.FirstLineIndentUnits:
                {
                    // Start with paragraph break size for empty paragraph case.
                    tpu = (int)InlineHelper.FetchAttr(runAttrSource, FontAttr.Size) * 10;

                    if (childNodes == null)
                        break;

                    foreach (Node node in childNodes)
                    {
                        IInline inline = node as IInline;

                        if (inline != null)
                        {
                            // This is not quite correct as Scale/Spacing must be taken into account here. See WORDSNET-19367 for details.
                            // Thus, FirstLineIndent and/or LeftIndent attributes can be calculated incorrectly (Layout code will not use attributes FirstLineIndent/LeftIndent calculated here).
                            tpu = (int)InlineHelper.FetchAttr(inline, FontAttr.Size) * 10;
                            break;
                        }
                    }

                    break;
                }

                default:
                    break;
            }

            int twips = (int)System.Math.Round(((double)units) / 100 * tpu);
            paraPr.SetAttr(twipsKey, twips);

            return true;
        }

        /// <summary>
        /// Applies hanging to current left indent value.
        /// </summary>
        private static void ApplyHangingToLeftIndent(ParaPr paraPr, int firstLineIndent)
        {
            // Note that LeftIndent must be updated before FirstLineIndent update.
            if (firstLineIndent < 0)
            {
                // It seems that Word ignores 0 value.
                bool hasLeftIndentUnits = (int)paraPr.FetchAttr(ParaAttr.LeftIndentUnits) != 0;

                // If there is no left indent units then left indent twips was not updated.
                int leftIndent = hasLeftIndentUnits ? paraPr.LeftIndent : 0;
                paraPr.SetAttr(ParaAttr.LeftIndent, leftIndent + (-firstLineIndent));
            }
        }

        /// <summary>
        /// Safely gets inherited value by the key.
        /// </summary>
        /// <remarks>
        /// At the moment when we need inherited value we might have invalid style hierarchy.
        /// </remarks>
        private static object GetInheritedValueSafe(IParaAttrSource paraAttrSource, int key)
        {
            object val = null;
            try
            {
                val = paraAttrSource.FetchInheritedParaAttr(key);
            }
            catch
            {
                // Do nothing.
            }

            return val;
        }

        /// <summary>
        /// TODO this code never used.
        /// Twips per line unit in the Section being processed.
        /// </summary>
        private int mTwipsPerLineUnit = ConvertUtilCore.PointToTwip(DefaultTwipsPerLine);

        /// <summary>
        /// Twips per character unit in the Document.
        /// </summary>
        /// <remarks>
        /// It looks that styles use this constant value.
        /// After styles are calculated this value is updated from document parts properties.
        /// </remarks>
        private int mTwipsPerCharacterUnit = 100;

        /// <summary>
        /// Twips per line unit if "document grid" is not used.
        /// </summary>
        private const int DefaultTwipsPerLine = 12;

        /// <summary>
        /// All keys for character units indents.
        /// </summary>
        private static readonly int[] gUnitKeys = new[]
        {
            ParaAttr.SpaceBeforeUnits, ParaAttr.SpaceAfterUnits, ParaAttr.RightIndentUnits,
            ParaAttr.LeftIndentUnits, ParaAttr.FirstLineIndentUnits
        };

    }
}
