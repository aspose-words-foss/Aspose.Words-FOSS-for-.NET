// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2007 by Vladimir Averkin

using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.RW.Vml;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides static method for building "Numbering" package part
    /// </summary>
    internal static class DocxNumberingWriter
    {
        /// <summary>
        /// Writes "Numbering" document part for the specified document.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer)
        {
            DocumentBase doc = writer.Document;

            if ((doc.Lists.ListDefCount == 0) && (doc.Lists.Count == 0))
                return;

            DocxBuilder builder = writer.CreateChildPartAndBuilder("numbering.xml", DocxContentType.Numbering, writer.RelTypes.Numbering);
            writer.PushBuilder(builder);

            builder.StartDocumentWithStandardNamespaces("w:numbering");

            WritePictureBullets(doc, writer);
            WriteListDefs(doc, writer);
            WriteListInstances(doc, writer);
            WriteNumIdMacAtCleanup(doc, writer);

            builder.EndDocument(); //w:numbering
            writer.PopBuilder();
        }

        private static void WriteNumIdMacAtCleanup(DocumentBase doc, DocxDocumentWriterBase writer)
        {
            object numIdMac = doc.Lists.NumIdMacAtCleanup;
            if (numIdMac != null)
                writer.CurrentBuilder.WriteVal("w:numIdMacAtCleanup", (int)numIdMac);
        }

        private static void WritePictureBullets(DocumentBase doc, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            for (int i = 0; i < doc.Lists.PictureBulletCount; i++)
            {
                Shape shape = doc.Lists.GetPictureBullet(i);
                builder.StartElement("w:numPicBullet");

                builder.WriteAttribute("w:numPicBulletId", i);

                if (shape.FallbackShape != null)
                    writer.StartAlternateContent(shape);

                if (shape.MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    builder.StartElement("w:pict");
                    VmlShapeWriter.Write(shape, builder, writer);
                    builder.EndElement(); // RK 2 VA Very ugly! Writing end tag should be automatic.
                    builder.EndElement(); // pict
                }
                else
                {
                    DmlDrawingWriter.WriteStart(shape, writer);
                    DmlDrawingWriter.WriteEnd(shape, writer);
                }

                if (shape.FallbackShape != null)
                    writer.EndAlternateContent(shape);

                builder.EndElement();  // numPicBullet
            }
        }

        private static void WriteListDefs(DocumentBase doc, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            Document document = doc as Document;
            bool isWord2013Up =
                (document != null) &&
                (document.ComplianceInfo != null) &&
                (document.ComplianceInfo.MsWordExtensionsVersion >= MsWordVersionCore.Word2013);

            for (int i = 0; i < doc.Lists.ListDefCount; i++)
            {
                ListDef listDef = doc.Lists.GetListDefByIndex(i);

                builder.StartElement("w:abstractNum");

                builder.WriteAttribute("w:abstractNumId", i);
                
                if (isWord2013Up)
                    builder.WriteAttribute("w15:restartNumberingAfterBreak", listDef.IsRestartAtEachSection);

                builder.WriteVal("w:nsid", NrxXmlUtil.IntToHex(listDef.ListDefId));
                builder.WriteVal("w:multiLevelType", DocxNumberingEnum.ListTypeToDocx(listDef.ListType));
                builder.WriteVal("w:tmpl", NrxXmlUtil.IntToHex(listDef.TemplateCode));
                builder.WriteVal("w:name", listDef.Name);

                if (listDef.IsListStyleDefinition)
                    builder.WriteVal("w:styleLink", writer.GetStyleId(listDef.ListStyleIstd));

                if (listDef.IsListStyleReference)
                {
                    builder.WriteVal("w:numStyleLink", writer.GetStyleId(listDef.ListStyleIstd));
                    // Do not write list levels for style reference.
                }
                else
                {
                    for (int listLeveNumber = 0; listLeveNumber < listDef.Levels.Count; listLeveNumber++)
                        WriteListLevel(listDef.Levels[listLeveNumber], true, writer);
                }

                builder.EndElement(); //w:abstractNum
            }
        }

        private static void WriteListInstances(DocumentBase doc, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (List list in doc.Lists)
            {
                int abstractNumId = doc.Lists.GetIndexOfListDefByListDefId(list.ListDefId);
                // RK Let's not write a list when we cannot find its list definition.
                // What else can we do? We cannot create and write a dummy list definition
                // because we have already written list definitions.
                if (abstractNumId >= 0)
                {
                    builder.StartElement("w:num");
                    builder.WriteAttribute("w:numId", list.ListId);

                    if ((list.DurableId != 0) && (writer.MsWordExtensionsVersion >= MsWordVersionCore.Word2016))
                        builder.WriteAttribute("w16cid:durableId", list.DurableId);

                    builder.WriteVal("w:abstractNumId", abstractNumId);

                    foreach (ListLevelOverride levelOverride in list.Overrides)
                    {
                        builder.StartElement("w:lvlOverride");
                        builder.WriteAttribute("w:ilvl", levelOverride.ListLevel.LevelNumber);

                        if (levelOverride.IsStartAt)
                            builder.WriteValIfNotDefault("w:startOverride", levelOverride.StartAtRaw, 0);

                        if (levelOverride.IsFormatting)
                            WriteListLevel(levelOverride.ListLevel, levelOverride.WriteStartAt, writer);

                        builder.EndElement();    //w:lvlOverride
                    }

                    builder.EndElement();    //w:num
                }
            }
        }

        private static void WriteListLevel(ListLevel level, bool writeStartAt, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("w:lvl");
            builder.WriteAttribute("w:ilvl", level.LevelNumber);

            // TODO 3 w:tplc attribute - not found in DOC

            builder.WriteAttribute("w:tentative", level.IsTentative ? "1" : "");

            if (writeStartAt)
                builder.WriteVal("w:start", level.StartAt);

            WriteNumberStyle(level, writer);

            // Add one since it is 1-based in WordML.
            if (level.IsRestartAfterLevelCustom)
                builder.WriteVal("w:lvlRestart", level.RestartAfterLevel + 1);

            // RK Here was a compare for zero, but I think the compare should be for StyleIndex.Nul
            // because zero istd is the Normal style.
            if (level.ParaStyleIstd != StyleIndex.Nil)
                builder.WriteVal("w:pStyle", writer.GetStyleId(level.ParaStyleIstd));

            builder.WriteValIfTrue("w:isLgl", level.IsLegal);

            if (level.TrailingCharacter != ListTrailingCharacter.Tab)
                builder.WriteVal("w:suff", DocxNumberingEnum.ListTrailingCharacterToDocx(level.TrailingCharacter));

            if (StringUtil.HasChars(level.NumberFormat))
            {
                builder.StartElement("w:lvlText");
                builder.WriteAttributeString("w:val", NrxXmlUtil.NumberFormatToXml(level.NumberFormat));
                // TODO 3 'w:null' not seen in the current tests. Skip writing it, check later.
                builder.EndElement(); //w:lvlText
            }

            if (level.HasPictureBullet)
                builder.WriteVal("w:lvlPicBulletId", level.PictureBulletId);

            if (level.Legacy)
            {
                builder.WriteElementWithAttributes(
                    "w:legacy",
                    "w:legacy", level.Legacy,
                    "w:legacySpace", level.LegacySpace,
                    "w:legacyIndent", level.LegacyIndent);
            }

            builder.WriteVal("w:lvlJc", DocxNumberingEnum.ListLevelAlignmentToDocx(level.Alignment, 
                writer.Compliance == OoxmlComplianceCore.IsoStrict));

            NrxParaPrWriter.Write(level.ParaPr, writer);

            DocxRunPrWriter.WriteForStyles(level.RunPr, level.RunPr, writer);

            builder.EndElement(); //w:lvl
        }

        /// <summary>
        /// Writes the 17.9.17 numFmt (Numbering Format) element.
        /// </summary>
        private static void WriteNumberStyle(ListLevel level, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            if (level.NumberStyle == NumberStyle.Custom)
            {
                builder.StartElement("mc:AlternateContent");
                builder.StartElement("mc:Choice");
                builder.WriteAttribute("Requires", "w14");

                builder.WriteElementWithAttributes("w:numFmt", 
                    "w:val", DocxEnum.NumberStyleToDocx(level.NumberStyle), 
                    "w:format", StringUtil.HasChars(level.CustomNumberStyle) ? level.CustomNumberStyle : null);

                builder.EndElement("mc:Choice");
                builder.StartElement("mc:Fallback");

                builder.WriteVal("w:numFmt", DocxEnum.NumberStyleToDocx(NumberStyle.Arabic));

                builder.EndElement("mc:Fallback");
                builder.EndElement("mc:AlternateContent");
            }
            else
            {
                builder.WriteVal("w:numFmt", DocxEnum.NumberStyleToDocx(level.NumberStyle));
            }
        }
    }
}
