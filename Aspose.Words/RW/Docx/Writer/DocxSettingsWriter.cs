// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2007 by Vladimir Averkin

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Math;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides static method for building "Settings" package part
    /// </summary>
    internal static class DocxSettingsWriter
    {
        /// <summary>
        /// Writes "Settings" document part for the specified document.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer)
        {
            DocumentBase doc = writer.Document;
            DocPr docPr = doc.DocPr;
            ViewOptions viewOptions = docPr.ViewOptions;
            OoxmlComplianceCore compliance = writer.Compliance;
            bool isIso = compliance != OoxmlComplianceCore.Ecma376;

            DocxBuilder builder = writer.CreateChildPartAndBuilder("settings.xml", DocxContentType.Settings, writer.RelTypes.Settings);
            writer.PushBuilder(builder);

            builder.StartSettingsDocumentPart();

            builder.WriteValIfTrue("w:hideSpellingErrors", docPr.HideSpellingErrors);
            builder.WriteValIfTrue("w:hideGrammaticalErrors", docPr.HideGrammaticalErrors);

            WriteWriteProtection(docPr.WriteProtection, builder);

            builder.WriteVal("w:view", DocxDopEnum.ViewTypeToDocx(viewOptions.ViewType));
            WriteZoom(viewOptions, builder, compliance);
            builder.WriteValIfTrue("w:removePersonalInformation", docPr.RemovePersonalInformation);
            builder.WriteValIfTrue("w:removeDateAndTime", docPr.RemoveDateAndTime);
            builder.WriteValIfTrue("w:doNotDisplayPageBoundaries", viewOptions.DoNotDisplayPageBoundaries);
            builder.WriteValIfTrue("w:displayBackgroundShape", viewOptions.DisplayBackgroundShape);
            builder.WriteValIfTrue("w:printPostScriptOverText", docPr.PrintPostScriptOverText);
            builder.WriteValIfTrue("w:printFractionalCharacterWidth", docPr.PrintFractionalCharacterWidth);
            builder.WriteValIfTrue("w:printFormsData", docPr.PrintFormsData);
            WriteFontEmbeddingOptions(writer);
            builder.WriteValIfTrue("w:saveFormsData", docPr.SaveFormsData);
            builder.WriteValIfTrue("w:mirrorMargins", (docPr.MultiplePages == MultiplePagesType.MirrorMargins));
            builder.WriteValIfTrue("w:alignBordersAndEdges", docPr.AlignBordersAndEdges);
            builder.WriteValIfTrue("w:bordersDoNotSurroundHeader", docPr.BordersDoNotSurroundHeader);
            builder.WriteValIfTrue("w:bordersDoNotSurroundFooter", docPr.BordersDoNotSurroundFooter);
            builder.WriteValIfTrue("w:gutterAtTop", docPr.GutterAtTop);
            WriteProofState(docPr, builder);
            builder.WriteValIfTrue("w:formsDesign", viewOptions.FormsDesign);
            WriteLink(builder, "w:attachedTemplate", docPr.AttachedTemplate, writer.RelTypes.AttachedTemplate);
            builder.WriteValIfTrue("w:linkStyles", docPr.LinkStyles);

            if (docPr.StylePaneFormatFilterSettings.Data != StylePaneFormatFilterSettings.StylePaneFormatFilterDefault)
            {
                builder.StartElement("w:stylePaneFormatFilter");
                if (compliance < OoxmlComplianceCore.IsoStrict)
                {
                    builder.WriteAttributeString(
                        Val, FormatterPal.IntToStrX4(docPr.StylePaneFormatFilterSettings.Data));
                }

                if (isIso) // only for iso29500 and up
                    WriteStylePaneFormatFilterSettings(builder, docPr.StylePaneFormatFilterSettings);
                
                builder.EndElement();
            }

            if (docPr.StylePaneSortMethod != StylePaneSortMethod.Default)
                builder.WriteVal("w:stylePaneSortMethod", !isIso ?
                    FormatterPal.IntToStrX4(StyleConvertUtil.StylePaneSortMethodToInt(docPr.StylePaneSortMethod)) :
                    DocxDopEnum.StylePaneSortMethodToDocx(docPr.StylePaneSortMethod)); 
            
            builder.WriteVal("w:documentType", DocxDopEnum.AutoFormatDocumentTypeToDocx(docPr.DocumentType));
            
            DocxMailMergePrWriter mailMergePrWriter = new DocxMailMergePrWriter();
            mailMergePrWriter.Write(docPr.MailMergeSettings, writer);

            if (IsAnyRevisionViewDisabled(docPr))
            {
                builder.StartElement("w:revisionView");
                builder.WriteAttribute("w:comments", docPr.ShowAnnotations);
                builder.WriteAttribute("w:formatting", docPr.ShowFormatting);
                builder.WriteAttribute("w:inkAnnotations", docPr.ShowInkAnnotations);
                builder.WriteAttribute("w:insDel", docPr.ShowInsertionsDeletions);
                builder.WriteAttribute("w:markup", docPr.ShowMarkup);
                builder.EndElement();
            }

            builder.WriteValIfTrue("w:trackRevisions", docPr.TrackRevisions);

            // WORDSNET-9070 To avoid accepting lot of golds we have to always write doNotTrackMoves in test mode.
            bool doNotTrackMoves = true;
            if (writer.SaveOptions.WriteDoNotTrackMovesCorrectly)
                doNotTrackMoves = docPr.DoNotTrackMoves;

            builder.WriteValIfTrue("w:doNotTrackMoves", doNotTrackMoves);
            builder.WriteValIfTrue("w:doNotTrackFormatting", docPr.DoNotTrackFormatting);
            
            WriteDocumentProtection(docPr.DocumentProtection, builder);
            builder.WriteValIfTrue("w:autoFormatOverride", docPr.DocumentProtection.AutoFormatOverride);
            builder.WriteValIfTrue("w:styleLockTheme", docPr.StyleLockTheme);
            builder.WriteValIfTrue("w:styleLockQFSet", docPr.StyleLockQuickFormatSet);
            builder.WriteVal("w:defaultTabStop", docPr.DefaultTabStop);

            int istd = docPr.ClickTypeParaStyleIstd;
            if ((istd != DocPr.ClickTypeParaStyleIstdDefault) && (doc.Styles.GetByIstd(istd, false) != null))
                builder.WriteVal("w:clickAndTypeStyle", writer.GetStyleId(istd));
            istd = docPr.DefaultTableStyleIstd;
            if ((istd != DocPr.DefaultTableStyleIstdDefault) && (doc.Styles.GetByIstd(istd, false) != null))
                builder.WriteVal("w:defaultTableStyle", writer.GetStyleId(istd));

            builder.WriteValIfTrue("w:updateFields", docPr.UpdateFields);
            builder.WriteValIfTrue("w:forceUpgrade", docPr.ForceUpgrade);

            HyphenationOptions hyphenationOptions = docPr.HyphenationOptions;
            builder.WriteValIfTrue("w:autoHyphenation", hyphenationOptions.AutoHyphenation);
            builder.WriteValIfPositive("w:consecutiveHyphenLimit", hyphenationOptions.ConsecutiveHyphenLimit);
            builder.WriteValIfNotDefault("w:hyphenationZone", hyphenationOptions.HyphenationZone, 
                HyphenationOptions.HyphenationZoneDefault);
            builder.WriteValIfTrue("w:doNotHyphenateCaps", !hyphenationOptions.HyphenateCaps);

            builder.WriteValIfTrue("w:showEnvelope", docPr.ShowEnvelope);
            builder.WriteValIfTrue("w:evenAndOddHeaders", docPr.EvenAndOddHeaders);

            bool bookFoldPrinting = (docPr.MultiplePages == MultiplePagesType.BookFoldPrinting);
            bool bookFoldPrintingReverse = (docPr.MultiplePages == MultiplePagesType.BookFoldPrintingReverse);

            builder.WriteValIfTrue("w:bookFoldRevPrinting", bookFoldPrintingReverse);
            builder.WriteValIfTrue("w:bookFoldPrinting", bookFoldPrinting);
            if (bookFoldPrinting || bookFoldPrintingReverse)
                builder.WriteVal("w:bookFoldPrintingSheets", docPr.BookFoldPrintingSheets);

            builder.WriteValIfNotDefault("w:drawingGridHorizontalSpacing", docPr.DrawingGridHorizontalSpacing, DocPr.DrawingGridHorizontalSpacingDefault);
            builder.WriteValIfNotDefault("w:drawingGridVerticalSpacing", docPr.DrawingGridVerticalSpacing, DocPr.DrawingGridVerticalSpacingDefault);
            builder.WriteValIfNotDefault("w:displayHorizontalDrawingGridEvery", docPr.DisplayHorizontalDrawingGridEvery, DocPr.DisplayHorizontalDrawingGridEveryDefault);
            builder.WriteValIfNotDefault("w:displayVerticalDrawingGridEvery", docPr.DisplayVerticalDrawingGridEvery, DocPr.DisplayVerticalDrawingGridEveryDefault);
            // RK Note difference in DOCX name.
            builder.WriteValIfTrue("w:doNotUseMarginsForDrawingGridOrigin", !docPr.UseMarginsForDrawingGridOrigin);
            if (!docPr.UseMarginsForDrawingGridOrigin)
            {
                builder.WriteValIfNotDefault("w:drawingGridHorizontalOrigin", docPr.DrawingGridHorizontalOrigin, DocPr.DrawingGridHorizontalOriginDefault);
                builder.WriteValIfNotDefault("w:drawingGridVerticalOrigin", docPr.DrawingGridVerticalOrigin, DocPr.DrawingGridVerticalOriginDefault);
            }

            builder.WriteValIfTrue("w:doNotShadeFormData", docPr.DoNotShadeFormData);
            builder.WriteValIfTrue("w:noPunctuationKerning", !docPr.PunctuationKerning);
            builder.WriteVal("w:characterSpacingControl", DocxDopEnum.KinsokuJustificationToDocx(docPr.CharacterSpacingType));
            builder.WriteValIfTrue("w:printTwoOnOne", (docPr.MultiplePages == MultiplePagesType.TwoPagesPerSheet));
            builder.WriteValIfTrue("w:strictFirstAndLastChars", docPr.StrictFirstAndLastChars);
            
            NrxDocPropertiesWriter.WriteNoLineBreaks(docPr, builder, docPr.NoLineBreaksLanguage == Language.LanguageNotSet
                              ? "en-US"
                              : LocaleConverter.LocaleToDocxTag((int) docPr.NoLineBreaksLanguage));

            builder.WriteValIfTrue("w:doNotEmbedSmartTags", docPr.DoNotEmbedSmartTags);
            builder.WriteValIfTrue("w:showEnvelope", docPr.ShowEnvelope);
            builder.WriteValIfTrue("w:doNotValidateAgainstSchema", !docPr.ValidateAgainstSchema);
            builder.WriteValIfTrue("w:saveInvalidXml", docPr.SaveInvalidXml);
            builder.WriteValIfTrue("w:ignoreMixedContent", docPr.IgnoreMixedContent);
            builder.WriteValIfTrue("w:alwaysShowPlaceholderText", docPr.AlwaysShowPlaceholderText);
            builder.WriteValIfTrue("w:doNotDemarcateInvalidXml", docPr.DoNotUnderlineInvalidXml);
            builder.WriteValIfTrue("w:saveXmlDataOnly", docPr.SaveXmlDataOnly);
            builder.WriteValIfTrue("w:useXSLTWhenSaving", docPr.UseXsltWhenSaving);
            WriteLink(builder, "w:saveThroughXslt", docPr.SaveThroughXslt, writer.RelTypes.SaveThroughXslt);
            builder.WriteValIfTrue("w:showXMLTags", docPr.ShowXmlTags);
            builder.WriteValIfTrue("w:alwaysMergeEmptyNamespace", docPr.AlwaysMergeEmptyNamespace);
            writer.WriteFootnotePr(docPr.FootnotePr, true);
            WriteCompat(docPr.CompatibilityOptions, builder, compliance);

            if (writer.SaveOptions.WriteRsidTable)
                NrxSettingsWriter.WriteRsids(docPr, builder, SaveFormat.Docx);

            WriteDocVars(doc.Variables, builder);
            
            if (docPr.MathProperties.IsNonDefault) // at least one property is non default.
                WriteMathProperties(docPr.MathProperties, builder);  // need to create mathPr section in the document.

            if (compliance == OoxmlComplianceCore.Ecma376) // this element is removed from ISO29500 onward.
                builder.WriteValIfTrue("w:uiCompat97To2003", docPr.CompatibilityOptions.UICompat97To2003);

            WriteThemeFontLang(docPr.ThemeFontLanguages, builder);

            // WORDSNET-4961 andrnosk: Add this section to make the document editable in MS Word Web App.
            if (writer.SaveOptions.WriteClrSchemeMapping)
                WriteClrSchemeMapping(builder);

            if(writer.SaveOptions.WriteWordCountOption)
                builder.WriteValIfTrue("w:doNotIncludeSubdocsInStats", docPr.DoNotIncludeSubDocsInStats);

            builder.WriteValIfTrue("w:doNotEmbedSmartTags", docPr.DoNotEmbedSmartTags);
            builder.WriteValIfTrue("w:doNotAutoCompressPictures", docPr.DoNotAutoCompressPictures);

            if (compliance < OoxmlComplianceCore.IsoStrict)
            {
                NrxShapeDefaultsWriter.Write(builder, writer.SaveInfo, false);
                NrxShapeDefaultsWriter.Write(builder, writer.SaveInfo, true);
            }

            if (isIso)
            {
                builder.WriteValIfTrue("w14:discardImageEditingData", docPr.DiscardImageEditingData);
                if (docPr.DefaultImageDpi > 0)
                    builder.WriteElementWithAttributes("w14:defaultImageDpi", "w14:val", docPr.DefaultImageDpi);
                if (writer.SaveOptions.WriteW14DocId)
                    builder.WriteElementWithAttributes("w14:docId", "w14:val", docPr.DocId);
                builder.WriteValIfTrue("w15:chartTrackingRefBased", docPr.ChartTrackingRefBased);
                builder.WriteElementWithAttributes("w15:docId", "w15:val", docPr.DocumentSetId);
            }

            builder.EndDocument(); //w:settings
            writer.PopBuilder();
        }

        private static void WriteLink(DocxBuilder builder, string elementName, string value, string relType)
        {
            if (StringUtil.HasChars(value))
            {
                // RK This value is escaped when adding to relationships.
                string relId = builder.Part.Rels.Add(relType, value, true);
                builder.StartElement(elementName);
                builder.WriteAttributeString("r:id", relId);
                builder.EndElement();
            }
        }

        /// <summary>
        /// Write ISO 29500 specific attributes.
        /// </summary>
        private static void WriteStylePaneFormatFilterSettings(
            DocxBuilder builder,
            StylePaneFormatFilterSettings settings)
        {
            builder.WriteAttribute("w:allStyles", settings.AllStyles);
            builder.WriteAttribute("w:alternateStyleNames", settings.AlternateStyleNames);
            builder.WriteAttribute("w:clearFormatting", settings.ClearFormatting);
            builder.WriteAttribute("w:customStyles", settings.CustomStyles);
            builder.WriteAttribute("w:directFormattingOnNumbering", settings.DirectFormattingOnNumbering);
            builder.WriteAttribute("w:directFormattingOnParagraphs", settings.DirectFormattingOnParagraphs);
            builder.WriteAttribute("w:directFormattingOnRuns", settings.DirectFormattingOnRuns);
            builder.WriteAttribute("w:directFormattingOnTables", settings.DirectFormattingOnTables);
            builder.WriteAttribute("w:headingStyles", settings.HeadingStyles);
            builder.WriteAttribute("w:latentStyles", settings.LatentStyles);
            builder.WriteAttribute("w:numberingStyles", settings.NumberingStyles);
            builder.WriteAttribute("w:stylesInUse", settings.StylesInUse);
            builder.WriteAttribute("w:tableStyles", settings.TableStyles);
            builder.WriteAttribute("w:top3HeadingStyles", settings.Top3HeadingStyles);
            builder.WriteAttribute("w:visibleStyles", settings.VisibleStyles);
        }

        private static bool IsAnyRevisionViewDisabled(DocPr docPr)
        {
            return !docPr.ShowAnnotations || 
                   !docPr.ShowFormatting ||
                   !docPr.ShowInkAnnotations || 
                   !docPr.ShowInsertionsDeletions || 
                   !docPr.ShowMarkup;
        }

        /// <summary>
        /// RK This seems to be DOCX only setting, not available in WordML.
        /// </summary>
        private static void WriteWriteProtection(WriteProtection writeProt, DocxBuilder builder)
        {
            if (writeProt.ReadOnlyRecommended || writeProt.IsWriteProtected)
            {
                builder.StartElement("w:writeProtection");
                
                builder.WriteAttributeIfTrue("w:recommended", writeProt.ReadOnlyRecommended);
                
                writeProt.UpdateDocxHash();
                WritePasswordHashAttrs(writeProt.PasswordHash, builder);

                builder.EndElement(); //w:writeProtection
            }
        }

        /// <summary>
        /// Without this section in Settings MS Word Web App reports 
        /// that the document (generated by AW) cannot be opened for editing. 
        /// </summary>
        private static void WriteClrSchemeMapping(DocxBuilder builder)
        {
            // Write the same default values as MS Word does. 
            // Valid values of attributes are listed in 2.18.12 section in DOCX spec. 
            builder.StartElement("w:clrSchemeMapping");

            builder.WriteAttribute("w:bg1", "light1");
            builder.WriteAttribute("w:t1", "dark1");
            builder.WriteAttribute("w:bg2", "light2");
            builder.WriteAttribute("w:t2", "dark2");
            builder.WriteAttribute("w:accent1", "accent1");
            builder.WriteAttribute("w:accent2", "accent2");
            builder.WriteAttribute("w:accent3", "accent3");
            builder.WriteAttribute("w:accent4", "accent4");
            builder.WriteAttribute("w:accent5", "accent5");
            builder.WriteAttribute("w:accent6", "accent6");
            builder.WriteAttribute("w:hyperlink", "hyperlink");
            builder.WriteAttribute("w:followedHyperlink", "followedHyperlink");

            builder.EndElement();
        }

        private static void WriteZoom(ViewOptions viewOptions, NrxXmlBuilder builder, OoxmlComplianceCore compliance)
        {
            builder.StartElement("w:zoom");

            if (viewOptions.ZoomType != ZoomType.None)
                builder.WriteAttribute(Val, DocxDopEnum.ZoomTypeToDocx(viewOptions.ZoomType));

            string percent = FormatterPal.IntToXml(viewOptions.ZoomPercent);
            if (compliance == OoxmlComplianceCore.IsoStrict)
                percent += "%";
            builder.WriteAttribute("w:percent", percent);

            builder.EndElement(); //w:zoom
        }

        private static void WriteProofState(DocPr docPr, NrxXmlBuilder builder)
        {
            builder.WriteElementWithAttributes(
                "w:proofState",
                "w:spelling", DocxDopEnum.ProofStateToDocx(docPr.ProofStateSpelling),
                "w:grammar", DocxDopEnum.ProofStateToDocx(docPr.ProofStateGrammar));
        }

        private static void WriteDocumentProtection(DocumentProtection prot, DocxBuilder builder)
        {
            if ((prot.Edit == ProtectionType.NoProtection) && (!prot.Formatting))
                return;

            builder.StartElement("w:documentProtection");

            if (prot.Edit != ProtectionType.NoProtection)
                builder.WriteAttribute("w:edit", DocxDopEnum.ProtectionTypeToDocx(prot.Edit));

            builder.WriteAttributeIfTrue("w:formatting", prot.Formatting);
            // WORDSNET-4700 Write enforcement attribute always in case of omitted attribute Word enforces protection in spite of OOXML spec.
            builder.WriteAttribute("w:enforcement", prot.Enforcement);

            prot.UpdateDocxHash();
            WritePasswordHashAttrs(prot.PasswordHash, builder);

            builder.EndElement(); //w:documentProtection
        }


        private static void WritePasswordHashAttrs(PasswordHash hash, DocxBuilder builder)
        {
            if (hash.IsEmpty)
                return;

            bool isIsoStrict = builder.OoxmlCompliance == OoxmlComplianceCore.IsoStrict;

            if (isIsoStrict)
            {
                builder.WriteAttribute("w:algorithmName", PasswordHash.CryptAlgorithmNameFromSid(hash.CryptAlgorithmSid));
            }
            else
            {
                string cryptProviderType =
                    (
                        (hash.CryptAlgorithmSid == PasswordHash.CryptAlgorithmSidFromAlgorithmName("SHA-512")) ||
                        // WORDSNET-21134 SHA-256 is also used for AES encryption.
                        (hash.CryptAlgorithmSid == PasswordHash.CryptAlgorithmSidFromAlgorithmName("SHA-256"))
                    )
                        ? "rsaAES"
                        : "rsaFull";

                builder.WriteAttribute("w:cryptProviderType", cryptProviderType);
                builder.WriteAttribute("w:cryptAlgorithmClass", "hash");
                builder.WriteAttribute("w:cryptAlgorithmType", "typeAny");
                builder.WriteAttribute("w:cryptAlgorithmSid", hash.CryptAlgorithmSid);
            }

            builder.WriteAttribute(isIsoStrict ? "w:spinCount" : "w:cryptSpinCount", hash.CryptSpinCount);
            builder.WriteAttribute(isIsoStrict ? "w:hashValue" : "w:hash", hash.Hash);
            builder.WriteAttribute(isIsoStrict ? "w:saltValue" : "w:salt", hash.Salt);
        }

        private static void WriteCompat(CompatibilityOptions co,
            NrxXmlBuilder builder,
            OoxmlComplianceCore compliance)
        {
            builder.StartElement("w:compat");

            if (compliance == OoxmlComplianceCore.IsoStrict)
            {
                builder.WriteValIfTrue("w:spaceForUL", co.SpaceForUL);
                builder.WriteValIfTrue("w:balanceSingleByteDoubleByteWidth", co.BalanceSingleByteDoubleByteWidth);
                builder.WriteValIfTrue("w:doNotLeaveBackslashAlone", co.DoNotLeaveBackslashAlone);
                builder.WriteValIfTrue("w:ulTrailSpace", co.UlTrailSpace);
                builder.WriteValIfTrue("w:doNotExpandShiftReturn", co.DoNotExpandShiftReturn);
                builder.WriteValIfTrue("w:adjustLineHeightInTable", co.AdjustLineHeightInTable);
                builder.WriteValIfTrue("w:applyBreakingRules", co.ApplyBreakingRules);
            }
            else
            {
                builder.WriteValIfTrue("w:useSingleBorderforContiguousCells", co.UseSingleBorderforContiguousCells);
                builder.WriteValIfTrue("w:wpJustification", co.WPJustification);
                builder.WriteValIfTrue("w:noTabHangInd", co.NoTabHangInd);
                builder.WriteValIfTrue("w:noLeading", co.NoLeading);
                builder.WriteValIfTrue("w:spaceForUL", co.SpaceForUL);
                builder.WriteValIfTrue("w:noColumnBalance", co.NoColumnBalance);
                builder.WriteValIfTrue("w:balanceSingleByteDoubleByteWidth", co.BalanceSingleByteDoubleByteWidth);
                builder.WriteValIfTrue("w:noExtraLineSpacing", co.NoExtraLineSpacing);
                builder.WriteValIfTrue("w:doNotLeaveBackslashAlone", co.DoNotLeaveBackslashAlone);
                builder.WriteValIfTrue("w:ulTrailSpace", co.UlTrailSpace);
                builder.WriteValIfTrue("w:doNotExpandShiftReturn", co.DoNotExpandShiftReturn);
                builder.WriteValIfTrue("w:spacingInWholePoints", co.SpacingInWholePoints);
                builder.WriteValIfTrue("w:lineWrapLikeWord6", co.LineWrapLikeWord6);
                builder.WriteValIfTrue("w:printBodyTextBeforeHeader", co.PrintBodyTextBeforeHeader);
                builder.WriteValIfTrue("w:printColBlack", co.PrintColBlack);
                builder.WriteValIfTrue("w:showBreaksInFrames", co.ShowBreaksInFrames);
                builder.WriteValIfTrue("w:subFontBySize", co.SubFontBySize);
                builder.WriteValIfTrue("w:suppressBottomSpacing", co.SuppressBottomSpacing);
                builder.WriteValIfTrue("w:suppressTopSpacing", co.SuppressTopSpacing);
                builder.WriteValIfTrue("w:suppressSpacingAtTopOfPage", co.SuppressSpacingAtTopOfPage);
                builder.WriteValIfTrue("w:suppressTopSpacingWP", co.SuppressTopSpacingWP);
                builder.WriteValIfTrue("w:suppressSpBfAfterPgBrk", co.SuppressSpBfAfterPgBrk);
                builder.WriteValIfTrue("w:swapBordersFacingPages", co.SwapBordersFacingPgs);
                builder.WriteValIfTrue("w:convMailMergeEsc", co.ConvMailMergeEsc);
                builder.WriteValIfTrue("w:truncateFontHeightsLikeWP6", co.TruncateFontHeightsLikeWP6);
                builder.WriteValIfTrue("w:mwSmallCaps", co.MWSmallCaps);
                builder.WriteValIfTrue("w:usePrinterMetrics", co.UsePrinterMetrics);
                builder.WriteValIfTrue("w:doNotSuppressParagraphBorders", co.DoNotSuppressParagraphBorders);
                builder.WriteValIfTrue("w:wrapTrailSpaces", co.WrapTrailSpaces);
                builder.WriteValIfTrue("w:footnoteLayoutLikeWW8", co.FootnoteLayoutLikeWW8);
                builder.WriteValIfTrue("w:shapeLayoutLikeWW8", co.ShapeLayoutLikeWW8);
                builder.WriteValIfTrue("w:alignTablesRowByRow", co.AlignTablesRowByRow);
                builder.WriteValIfTrue("w:forgetLastTabAlignment", co.ForgetLastTabAlignment);
                builder.WriteValIfTrue("w:adjustLineHeightInTable", co.AdjustLineHeightInTable);
                builder.WriteValIfTrue("w:autoSpaceLikeWord95", co.AutoSpaceLikeWord95);
                builder.WriteValIfTrue("w:noSpaceRaiseLower", co.NoSpaceRaiseLower);
                builder.WriteValIfTrue("w:doNotUseHTMLParagraphAutoSpacing", co.DoNotUseHTMLParagraphAutoSpacing);
                builder.WriteValIfTrue("w:layoutRawTableWidth", co.LayoutRawTableWidth);
                builder.WriteValIfTrue("w:layoutTableRowsApart", co.LayoutTableRowsApart);
                builder.WriteValIfTrue("w:useWord97LineBreakRules", co.UseWord97LineBreakRules);
                builder.WriteValIfTrue("w:doNotBreakWrappedTables", co.DoNotBreakWrappedTables);
                builder.WriteValIfTrue("w:doNotSnapToGridInCell", co.DoNotSnapToGridInCell);
                builder.WriteValIfTrue("w:selectFldWithFirstOrLastChar", co.SelectFldWithFirstOrLastChar);
                builder.WriteValIfTrue("w:applyBreakingRules", co.ApplyBreakingRules);
                builder.WriteValIfTrue("w:doNotWrapTextWithPunct", co.DoNotWrapTextWithPunct);
                builder.WriteValIfTrue("w:doNotUseEastAsianBreakRules", co.DoNotUseEastAsianBreakRules);
                builder.WriteValIfTrue("w:useWord2002TableStyleRules", co.UseWord2002TableStyleRules);
                builder.WriteValIfTrue("w:growAutofit", co.GrowAutofit);
                builder.WriteValIfTrue("w:useFELayout", co.UseFELayout);
                builder.WriteValIfTrue("w:useNormalStyleForList", co.UseNormalStyleForList);
                builder.WriteValIfTrue("w:doNotUseIndentAsNumberingTabStop", co.DoNotUseIndentAsNumberingTabStop);
                builder.WriteValIfTrue("w:useAltKinsokuLineBreakRules", co.UseAltKinsokuLineBreakRules);
                builder.WriteValIfTrue("w:allowSpaceOfSameStyleInTable", co.AllowSpaceOfSameStyleInTable);
                builder.WriteValIfTrue("w:doNotSuppressIndentation", co.DoNotSuppressIndentation);
                builder.WriteValIfTrue("w:doNotAutofitConstrainedTables", co.DoNotAutofitConstrainedTables);
                builder.WriteValIfTrue("w:autofitToFirstFixedWidthCell", co.AutofitToFirstFixedWidthCell);
                builder.WriteValIfTrue("w:underlineTabInNumList", co.UnderlineTabInNumList);
                builder.WriteValIfTrue("w:displayHangulFixedWidth", co.DisplayHangulFixedWidth);
                builder.WriteValIfTrue("w:splitPgBreakAndParaMark", co.SplitPgBreakAndParaMark);
                builder.WriteValIfTrue("w:doNotVertAlignCellWithSp", co.DoNotVertAlignCellWithSp);
                builder.WriteValIfTrue("w:doNotBreakConstrainedForcedTable", co.DoNotBreakConstrainedForcedTable);
                builder.WriteValIfTrue("w:doNotVertAlignInTxbx", co.DoNotVertAlignInTxbx);
                builder.WriteValIfTrue("w:useAnsiKerningPairs", co.UseAnsiKerningPairs);
                builder.WriteValIfTrue("w:cachedColBalance", co.CachedColBalance);
                builder.WriteValIfTrue("w:wpSpaceWidth", co.WPSpaceWidth);
            }

            WriteCustomCompatSettings(co, compliance, builder);

            builder.EndElement(); //w:compat
        }
        
        /// <summary>
        /// If compliance is other than older ECMA376, then write custom compat settings into docx.
        /// </summary>
        private static void WriteCustomCompatSettings(
            CompatibilityOptions co, 
            OoxmlComplianceCore compliance,
            NrxXmlBuilder builder)
        {
            if ((co.CustomCompatibilitySettings.Count != 0) && // if we have something to write 
                (compliance != OoxmlComplianceCore.Ecma376))
            {
                CustomCompatibilitySettingCollection settings = co.CustomCompatibilitySettings;
                for (int iSetting = 0; iSetting < settings.Count; iSetting++)
                {
                    // WORDSNET-4777 It's essential to write "w" namespace prefix. Otherwise Word2010 shows "Compatibility mode" warning.
                    builder.StartElement("w:compatSetting"); // w:compatSetting
                    builder.WriteAttribute("w:name", settings[iSetting].Name);
                    builder.WriteAttribute("w:uri", settings[iSetting].Uri);
                    builder.WriteAttribute(Val, settings[iSetting].Value);
                    builder.EndElement();
                }
            }
        }

        private static void WriteDocVars(VariableCollection variables, NrxXmlBuilder builder)
        {
            if (variables.Count > 0)
            {
                builder.StartElement("w:docVars");

                foreach (KeyValuePair<string, string> var in variables)
                {
                    // WORDSNET-22135 The correct writing of the Line Break character for the 'w:docVar' element.
                    // AM. Someday we need to move all these transformation into NrxXmlBuilder class.
                    string value = (var.Value != null) ? var.Value.Replace("\v", "_x000b_") : null;

                    builder.WriteElementWithAttributes(
                        "w:docVar",
                        "w:name", var.Key,
                        Val, value);
                }

                builder.EndElement(); //w:docVars
            }
        }

        private static void WriteThemeFontLang(ThemeFontLanguages languages, NrxXmlBuilder builder)
        {
            string bidi = LocaleConverter.LocaleToDocxTag((int)languages.Bidi);
            string eastAsia = LocaleConverter.LocaleToDocxTag((int)languages.EastAsia);
            string other = LocaleConverter.LocaleToDocxTag((int)languages.Latin);

            builder.WriteElementWithAttributes(
                "w:themeFontLang",
                Val, other,
                "w:eastAsia", eastAsia,
                "w:bidi", bidi);
        }
        
        private static void WriteMathProperties(MathProperties mathPr, NrxXmlBuilder builder)
        {
            builder.StartElement("m:mathPr");
            

            builder.WriteVal("m:mathFont", mathPr.DefaultFont);

            if (mathPr.BreakOnBinary != MathBreakOnBinary.Default)
                builder.WriteVal("m:brkBin", DocxDopEnum.MathBreakOnBynaryToDocx(mathPr.BreakOnBinary));
            
            if (mathPr.BreakOnBinarySubtraction != MathBreakOnBinarySubtraction.Default)
                builder.WriteVal("m:brkBinSub", DocxDopEnum.MathBreakOnBinarySubtractionToDocx(mathPr.BreakOnBinarySubtraction));
            
            builder.WriteValIfTrue("m:smallFrac", mathPr.IsSmallFraction);
        
            if (!mathPr.UseDisplayMathDefaults)
                builder.WriteVal("m:dispDef", mathPr.UseDisplayMathDefaults);
            
            builder.WriteValIfNotDefault("m:lMargin", mathPr.LeftMargin, 0);
            builder.WriteValIfNotDefault("m:rMargin", mathPr.RightMargin, 0);
            
            if (mathPr.DefaultJustification != OfficeMathJustification.Default)
                builder.WriteVal("m:defJc", DocxEnum.MathJustificationToDocx(mathPr.DefaultJustification));

            builder.WriteValIfNotDefault("m:preSp", mathPr.PreParagraphSpacing, 0);
            builder.WriteValIfNotDefault("m:postSp", mathPr.PostParagraphSpacing, 0);
            builder.WriteValIfNotDefault("m:interSp", mathPr.InterEquationSpacing, 0);
            builder.WriteValIfNotDefault("m:intraSp", mathPr.IntraEquationSpacing, 0);
            builder.WriteValIfNotDefault("m:wrapIndent", mathPr.WrapIndent, MathProperties.DefaultWrapIndent);
            builder.WriteValIfTrue("m:wrapRight", mathPr.WrapRight);            
            
            if (mathPr.IntegralLimitLocation !=  MathLimitLocation.SubscriptSuperscript)
                builder.WriteVal("m:intLim", DocxEnum.MathLimitLocationToDocx(mathPr.IntegralLimitLocation));

            if (mathPr.NaryLimitLocation != MathLimitLocation.UnderOver)
                builder.WriteVal("m:naryLim", DocxEnum.MathLimitLocationToDocx(mathPr.NaryLimitLocation));

            builder.EndElement();
        }

        /// <summary>
        /// Writes font embedding options.
        /// </summary>
        private static void WriteFontEmbeddingOptions(DocxDocumentWriterBase writer)
        {
            DocumentBase doc = writer.Document;
            DocxBuilder builder = writer.CurrentBuilder;
            DocPr docPr = doc.DocPr;

            builder.WriteValIfTrue("w:embedTrueTypeFonts", docPr.EmbedTrueTypeFonts);
            builder.WriteValIfTrue("w:embedSystemFonts", docPr.EmbedSystemFonts);
            builder.WriteValIfTrue("w:saveSubsetFonts", docPr.SaveSubsetFonts);
        }

        private const string Val = "w:val";
    }
}
