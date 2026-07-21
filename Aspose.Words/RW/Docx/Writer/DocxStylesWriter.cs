// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides methods for building the "Styles" package part.
    /// </summary>
    internal static class DocxStylesWriter
    {
        /// <summary>
        /// Writes the "Styles" document part for the specified document.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CreateChildPartAndBuilder("styles.xml", DocxContentType.Styles, writer.RelTypes.Styles);
            writer.PushBuilder(builder);

            builder.StartStylesDocumentPart();

            WriteDocDefaults(writer);

            if(writer.SaveOptions.WriteLatentStyles)
                WriteLatentStyles(writer);

            WriteStyles(writer);

            builder.EndDocument(); //w:styles
            writer.PopBuilder();
        }

        private static void WriteDocDefaults(DocxDocumentWriterBase writer)
        {
            StyleCollection styles = writer.Document.Styles;
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("w:docDefaults");

            builder.StartElement("w:rPrDefault");

            RunPr defaultRunPr = styles.DefaultRunPr;

            RemoveInvariantLocale(defaultRunPr, FontAttr.LocaleIdFarEast);
            RemoveInvariantLocale(defaultRunPr, FontAttr.LocaleIdBi);

            DocxRunPrWriter.WriteForStyles(defaultRunPr, defaultRunPr, writer);
            builder.EndElement(); // w:rPrDefault

            builder.StartElement("w:pPrDefault");
            NrxParaPrWriter.Write(styles.DefaultParaPr, writer);
            builder.EndElement(); // w:pPrDefault

            builder.EndElement(); // w:docDefaults
        }

        /// <summary>
        /// Removes InvariantCulture locale.
        /// </summary>
        /// <remarks>
        /// AM. This is workaround for WORDSNET-23785 to reduce amount of affected golds.
        /// </remarks>
        private static void RemoveInvariantLocale(RunPr runPr, int key)
        {
            if(runPr.ContainsKey(key) &&
               ((int)runPr[key] == (int)Language.InvariantCulture))
                runPr.Remove(key);
        }

        private static void WriteLatentStyles(DocxDocumentWriterBase writer)
        {
            DocumentBase doc = writer.Document;
            LatentStyles latentStyles = doc.Styles.LatentStyles;
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("w:latentStyles");

            builder.WriteAttribute("w:defLockedState", latentStyles.DefaultLockedState);
            builder.WriteAttribute("w:defUIPriority", latentStyles.DefaultUIPriority);
            builder.WriteAttribute("w:defSemiHidden", latentStyles.DefaultSemiHidden);
            builder.WriteAttribute("w:defUnhideWhenUsed", latentStyles.DefaultUnhideWhenUsed);
            builder.WriteAttribute("w:defQFormat", latentStyles.DefaultQuickFormat);
            builder.WriteAttribute("w:count", latentStyles.KnownStylesCount);

            // andrnosk: WORDSNET-5562 If latent styles latentStyle are missing, let's build defaults.
            if (latentStyles.Count <=0)
                latentStyles.InitToWord2007Default();

            for (int i = 0; i < latentStyles.Count; i++)
            {
                LatentStyle latentStyle = latentStyles[i];

                string name = StyleConvertUtil.StyleIdentifierToXml(latentStyle.StyleIdentifier, "");
                if (!StringUtil.HasChars(name))
                    continue;

                // WORDSNET-19842 Used to avoid accepting lot of golds.
                if (!writer.SaveOptions.Write2019LatentStyle &&
                    ((latentStyle.StyleIdentifier == StyleIdentifier.Mention) ||
                     (latentStyle.StyleIdentifier == StyleIdentifier.SmartHyperlink) ||
                     (latentStyle.StyleIdentifier == StyleIdentifier.Hashtag) ||
                     (latentStyle.StyleIdentifier == StyleIdentifier.UnresolvedMention)))
                    continue;

                builder.StartElement("w:lsdException");

                builder.WriteAttributeString("w:name", name);

                if (latentStyle.Locked != latentStyles.DefaultLockedState)
                    builder.WriteAttribute("w:locked", latentStyle.Locked);

                if (latentStyle.SemiHidden != latentStyles.DefaultSemiHidden)
                    builder.WriteAttribute("w:semiHidden", latentStyle.SemiHidden);

                if (latentStyle.UIPriority != latentStyles.DefaultUIPriority)
                    builder.WriteAttribute("w:uiPriority", latentStyle.UIPriority);

                if (latentStyle.UnhideWhenUsed != latentStyles.DefaultUnhideWhenUsed)
                    builder.WriteAttribute("w:unhideWhenUsed", latentStyle.UnhideWhenUsed);

                if (latentStyle.QuickStyle != latentStyles.DefaultQuickFormat)
                    builder.WriteAttribute("w:qFormat", latentStyle.QuickStyle);

                builder.EndElement(); // w:lsdException
            }

            builder.EndElement(); // w:latentStyles
        }

        private static void WriteStyles(DocxDocumentWriterBase writer)
        {
            StyleCollection styles = writer.Document.Styles;
            for (int i = 0; i < styles.Count; i++)
                WriteStyle(writer, styles[i]);
        }

        private static void WriteStyle(DocxDocumentWriterBase writer, Style style)
        {
            if(!WordUtil.CheckValidAndWarn(style, writer.Document.WarningCallback, WarningSource.Docx))
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("w:style");

            builder.WriteAttributeString("w:type", DocxEnum.StyleTypeToDocx(style.Type));

            switch (style.StyleIdentifier)
            {
                case StyleIdentifier.Normal:
                case StyleIdentifier.DefaultParagraphFont:
                case StyleIdentifier.NoList:
                case StyleIdentifier.TableNormal:
                    // Don't know if the default styles could be redefined and where to get info for it.
                    // I will always write these styles as default for the time being.
                    builder.WriteAttributeString("w:default", "1");
                    break;
                case StyleIdentifier.User:
                    // These are all custom styles.
                    builder.WriteAttributeString("w:customStyle", "1");
                    break;
                default:
                    // All others are built-in styles. Do nothing.
                    break;
            }

            builder.WriteAttributeString("w:styleId", writer.GetStyleId(style.Istd));

            builder.WriteVal("w:name", StyleConvertUtil.StyleIdentifierToXml(style.StyleIdentifier, style.Name));
            builder.WriteVal("w:aliases", style.Styles.GetAliases(style, false));

            if(WordUtil.IsValidIstd(style.BasedOnIstd))
                builder.WriteVal("w:basedOn", writer.GetStyleId(style.BasedOnIstd));

            if ((style.NextIstd != style.Istd) && (WordUtil.IsValidIstd(style.NextIstd)))
                builder.WriteVal("w:next", writer.GetStyleId(style.NextIstd));

            if(WordUtil.IsValidIstd(style.LinkedIstd))
                builder.WriteVal("w:link", writer.GetStyleId(style.LinkedIstd));

            builder.WriteValIfTrue("w:autoRedefine", style.AutomaticallyUpdate);
            builder.WriteValIfTrue("w:hidden", style.Hidden);

            // DOCX only attribute.
            builder.WriteValIfPositive("w:uiPriority", style.Priority);

            builder.WriteValIfTrue("w:semiHidden", style.SemiHidden);

            // DOCX only attributes.
            builder.WriteValIfTrue("w:unhideWhenUsed", style.UnhideWhenUsed);
            builder.WriteValIfTrue("w:qFormat", style.IsQuickStyle);

            builder.WriteValIfTrue("w:locked", style.Locked);

            builder.WriteValIfTrue("w:personal", style.Personal);
            builder.WriteValIfTrue("w:personalCompose", style.PersonalCompose);
            builder.WriteValIfTrue("w:personalReply", style.PersonalReply);

            if (style.Rsid != 0)
                builder.WriteVal("w:rsid", NrxXmlUtil.IntToHex(style.Rsid));

            if ((style.ParaPr.TabStops != null) && (style.ParaPr.TabStops.Count <= 0))
                style.ParaPr.Remove(ParaAttr.TabStops);

            if (style.Type != StyleType.Character)
                NrxParaPrWriter.Write(style.ParaPr, writer);

            DocxRunPrWriter.WriteForStyles(style.RunPr, style, writer);

            if (style.Type == StyleType.Table)
            {
                TableStyle tableStyle = (TableStyle)style;
                NrxRowPrWriter.Write(tableStyle.TablePr, true, true, writer);
                NrxRowPrWriter.Write(tableStyle.RowPr, false, true, writer);
                NrxCellPrWriter.Write(tableStyle.CellPr, writer);

                foreach (ConditionalStyle conditionalStyle in tableStyle.ConditionalStyles.DefinedStyles)
                    WriteConditionalStyle(conditionalStyle, writer);
            }

            builder.EndElement(); //w:style
        }

        private static void WriteConditionalStyle(ConditionalStyle conditionalStyle, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            builder.StartElement("w:tblStylePr");
            builder.WriteAttribute("w:type", StyleConvertUtil.TableStyleOverrideTypeToXml(conditionalStyle.OverrideType));

            if (conditionalStyle.HasParagraphFormatting)
                NrxParaPrWriter.Write(conditionalStyle.ParaPr, writer);
            // RK Passing conditionalStyle.RunPr as an attr source for resolving BoolEx values is probably
            // not correct here, but I do not have proper support for resolving values for table styles yet.
            if (conditionalStyle.HasRunFormatting)
                DocxRunPrWriter.WriteForStyles(conditionalStyle.RunPr, conditionalStyle.RunPr, writer);
            if (conditionalStyle.HasTableFormatting || conditionalStyle.HasCellFormatting) // mimic MS Word
                NrxRowPrWriter.Write(conditionalStyle.TablePr, true, true, writer);
            if (conditionalStyle.HasRowFormatting)
                NrxRowPrWriter.Write(conditionalStyle.RowPr, false, true, writer);
            if (conditionalStyle.HasCellFormatting)
                NrxCellPrWriter.Write(conditionalStyle.CellPr, writer);

            builder.EndElement(); //w:tblStylePr
        }
    }
}
