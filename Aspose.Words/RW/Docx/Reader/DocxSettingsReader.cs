// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Saving;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading "Settings" package part
    /// </summary>
    internal static class DocxSettingsReader
    {
        /// <summary>
        /// Reads "Settings" part for the specified document.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            DocxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.Settings);
            if (xmlReader == null)
                return;

            DocumentBase doc = reader.Document;

            DocPr docPr = doc.DocPr;
            ViewOptions viewOptions = docPr.ViewOptions;
            CompatibilityOptions compat = docPr.CompatibilityOptions;
            HyphenationOptions hyphenationOptions = docPr.HyphenationOptions;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            // Need to set some defaults here.
            docPr.PunctuationKerning = true;
            viewOptions.ViewType = ViewType.PageLayout;

            while (xmlReader.ReadChild("settings")) // w:settings
            {
                switch (xmlReader.LocalName)
                {
                    case "doNotIncludeSubdocsInStats":
                        docPr.DoNotIncludeSubDocsInStats = xmlReader.ReadBoolVal();
                        break;
                    case "writeProtection":
                        ReadWriteProtection(xmlReader, docPr, reader.ComplianceInfo);
                        break;
                    case "alwaysMergeEmptyNamespace":
                        docPr.AlwaysMergeEmptyNamespace = xmlReader.ReadBoolVal();
                        break;
                    case "alwaysShowPlaceholderText":
                        docPr.AlwaysShowPlaceholderText = xmlReader.ReadBoolVal();
                        break;
                    case "autoFormatOverride":
                        docPr.DocumentProtection.AutoFormatOverride = xmlReader.ReadBoolVal();
                        break;
                    case "bookFoldPrinting":
                    case "bookFoldRevPrinting":
                    case "mirrorMargins":
                    case "printTwoOnOne":
                        NrxSettingsReader.ReadMultiplePagesType(reader, docPr);
                        break;
                    case "bookFoldPrintingSheets":
                        docPr.BookFoldPrintingSheets = xmlReader.ReadIntVal();
                        break;
                    case "embedSystemFonts":
                        docPr.EmbedSystemFonts = xmlReader.ReadBoolVal();
                        break;
                    case "printFractionalCharacterWidth":
                        docPr.PrintFractionalCharacterWidth = xmlReader.ReadBoolVal();
                        break;
                    case "printPostScriptOverText":
                        docPr.PrintPostScriptOverText = xmlReader.ReadBoolVal();
                        break;
                    case "removePersonalInformation":
                        docPr.RemovePersonalInformation = xmlReader.ReadBoolVal();
                        break;
                    case "removeDateAndTime":
                        docPr.RemoveDateAndTime = xmlReader.ReadBoolVal();
                        break;
                    case "showEnvelope":
                        docPr.ShowEnvelope = xmlReader.ReadBoolVal();
                        break;
                    case "doNotValidateAgainstSchema":
                        docPr.ValidateAgainstSchema = !xmlReader.ReadBoolVal();
                        break;
                    case "saveInvalidXml":
                        // Note DOCX name difference.
                        docPr.SaveInvalidXml = xmlReader.ReadBoolVal();
                        break;
                    case "saveXmlDataOnly":
                        docPr.SaveXmlDataOnly = xmlReader.ReadBoolVal();
                        break;
                    case "showXMLTags":
                        docPr.ShowXmlTags = xmlReader.ReadBoolVal();
                        break;
                    case "ignoreMixedContent":
                        docPr.IgnoreMixedContent = xmlReader.ReadBoolVal();
                        break;
                    case "useXSLTWhenSaving":
                        docPr.UseXsltWhenSaving = xmlReader.ReadBoolVal();
                        break;
                    case "saveThroughXslt":
                        // Note DOCX name difference.
                        docPr.SaveThroughXslt = reader.GetRelationshipTarget(xmlReader.ReadId());
                        break;
                    case "view":
                        viewOptions.ViewType = DocxDopEnum.DocxToViewType(xmlReader.ReadVal());
                        break;
                    case "zoom":
                        ReadZoom(xmlReader, viewOptions);
                        break;
                    case "doNotDisplayPageBoundaries":
                        viewOptions.DoNotDisplayPageBoundaries = xmlReader.ReadBoolVal();
                        break;
                    case "displayBackgroundShape":
                        viewOptions.DisplayBackgroundShape = xmlReader.ReadBoolVal();
                        break;
                    case "printFormsData":
                        docPr.PrintFormsData = xmlReader.ReadBoolVal();
                        break;
                    case "embedTrueTypeFonts":
                        docPr.EmbedTrueTypeFonts = xmlReader.ReadBoolVal();
                        break;
                    case "saveSubsetFonts":
                        docPr.SaveSubsetFonts = xmlReader.ReadBoolVal();
                        break;
                    case "saveFormsData":
                        docPr.SaveFormsData = xmlReader.ReadBoolVal();
                        break;
                    case "alignBordersAndEdges":
                        docPr.AlignBordersAndEdges = xmlReader.ReadBoolVal();
                        break;
                    case "bordersDoNotSurroundHeader":
                        docPr.BordersDoNotSurroundHeader = xmlReader.ReadBoolVal();
                        break;
                    case "bordersDoNotSurroundFooter":
                        docPr.BordersDoNotSurroundFooter = xmlReader.ReadBoolVal();
                        break;
                    case "doNotDemarcateInvalidXml":
                        docPr.DoNotUnderlineInvalidXml = xmlReader.ReadBoolVal();
                        break;
                    case "doNotEmbedSmartTags":
                        docPr.DoNotEmbedSmartTags = xmlReader.ReadBoolVal();
                        break;
                    case "gutterAtTop":
                        docPr.GutterAtTop = xmlReader.ReadBoolVal();
                        break;
                    case "proofState":
                        ReadProofState(xmlReader, docPr);
                        break;
                    case "formsDesign":
                        viewOptions.FormsDesign = xmlReader.ReadBoolVal();
                        break;
                    case "attachedTemplate":
                        docPr.AttachedTemplate = reader.GetRelationshipTarget(xmlReader.ReadId());
                        break;
                    case "linkStyles":
                        docPr.LinkStyles = xmlReader.ReadBoolVal();
                        break;
                    case "documentType":
                        docPr.DocumentType = DocxDopEnum.DocxToAutoFormatDocumentType(xmlReader.ReadVal());
                        break;
                    case "mailMerge":
                        DocxReaderFactory.MailMergePrReader.Read(reader, docPr.MailMergeSettings);
                        break;
                    case "mathPr":
                        ReadMathProperties(xmlReader, docPr.MathProperties, complianceInfo);
                        break;
                    case "styleLockQFSet":
                        docPr.StyleLockQuickFormatSet = xmlReader.ReadBoolVal();
                        break;
                    case "styleLockTheme":
                        docPr.StyleLockTheme = xmlReader.ReadBoolVal();
                        break;
                    case "stylePaneFormatFilter":
                    {
                        string elValue = xmlReader.ReadVal();
                        if (StringUtil.HasChars(elValue))
                            docPr.StylePaneFormatFilterSettings.Data = NrxXmlUtil.HexToInt(elValue);

                        docPr.StylePaneFormatFilterSettings = 
                            new StylePaneFormatFilterSettings(docPr.StylePaneFormatFilterSettings.Data);
                        ReadStylePaneFormatFilterSettings(xmlReader, docPr.StylePaneFormatFilterSettings,
                            xmlReader.ComplianceInfo);
                    }
                        break;
                    case "stylePaneSortMethod":
                        docPr.StylePaneSortMethod = DocxDopEnum.DocxToStylePaneSortMethod(xmlReader.ReadVal(),
                            reader.ComplianceInfo);
                        break;
                    case "trackRevisions":
                        docPr.TrackRevisions = xmlReader.ReadBoolVal();
                        break;
                    case "doNotTrackMoves":
                        docPr.DoNotTrackMoves = xmlReader.ReadBoolVal();
                        break;
                    case "doNotTrackFormatting":
                        docPr.DoNotTrackFormatting = xmlReader.ReadBoolVal();
                        break;
                    case "documentProtection":
                        ReadDocumentProtection(xmlReader, docPr.DocumentProtection, reader.ComplianceInfo);
                        break;
                    case "defaultTabStop":
                        docPr.DefaultTabStop = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "autoHyphenation":
                        hyphenationOptions.AutoHyphenation = xmlReader.ReadBoolVal();
                        break;
                    case "consecutiveHyphenLimit":
                        hyphenationOptions.SetConsecutiveHyphenLimitSafe(xmlReader.ReadIntVal());
                        break;
                    case "hyphenationZone":
                        hyphenationOptions.SetHyphenationZoneSafe(xmlReader.ReadValAsTwips(complianceInfo));
                        break;
                    case "doNotHyphenateCaps":
                        hyphenationOptions.HyphenateCaps = !xmlReader.ReadBoolVal();
                        break;
                    case "evenAndOddHeaders":
                        docPr.EvenAndOddHeaders = xmlReader.ReadBoolVal();
                        break;
                    case "drawingGridHorizontalSpacing":
                        docPr.DrawingGridHorizontalSpacing = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "drawingGridVerticalSpacing":
                        docPr.DrawingGridVerticalSpacing = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "displayHorizontalDrawingGridEvery":
                        docPr.DisplayHorizontalDrawingGridEvery = xmlReader.ReadIntVal();
                        break;
                    case "displayVerticalDrawingGridEvery":
                        docPr.DisplayVerticalDrawingGridEvery = xmlReader.ReadIntVal();
                        break;
                    case "doNotUseMarginsForDrawingGridOrigin":
                        docPr.UseMarginsForDrawingGridOrigin = !xmlReader.ReadBoolVal();
                        break;
                    case "drawingGridHorizontalOrigin":
                        docPr.DrawingGridHorizontalOrigin = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "drawingGridVerticalOrigin":
                        docPr.DrawingGridVerticalOrigin = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "doNotShadeFormData":
                        docPr.DoNotShadeFormData = xmlReader.ReadBoolVal();
                        break;
                    case "noPunctuationKerning":
                        docPr.PunctuationKerning = !xmlReader.ReadBoolVal();
                        break;
                    case "characterSpacingControl":
                        docPr.CharacterSpacingType = DocxDopEnum.DocxToKinsokuJustification(xmlReader.ReadVal());
                        break;
                    case "strictFirstAndLastChars":
                        docPr.StrictFirstAndLastChars = xmlReader.ReadBoolVal();
                        break;
                    case "noLineBreaksAfter":
                        docPr.NoLineBreaksAfter = ReadNoLineBreaks(xmlReader, docPr);
                        break;
                    case "noLineBreaksBefore":
                        docPr.NoLineBreaksBefore = ReadNoLineBreaks(xmlReader, docPr);
                        break;
                    case "footnotePr":
                    case "endnotePr":
                        ReadFootnotePr(reader, xmlReader.LocalName, docPr.FootnotePr);
                        break;
                    case "compat":
                        ReadCompat(xmlReader, compat, reader.ComplianceInfo);
                        break;
                    case "docVars":
                        ReadVars(xmlReader, doc.Variables);
                        break;
                    case "uiCompat97To2003":
                        compat.UICompat97To2003 = xmlReader.ReadBoolVal();
                        break;
                    case "revisionView":
                        ReadRevisionView(xmlReader, docPr);
                        break;
                    case "themeFontLang":
                        ReadThemeFontLang(xmlReader, docPr.ThemeFontLanguages);
                        break;
                    case "clrSchemeMapping":
                        // andrnosk: We do not read this element but write ClrSchemeMapping with default values to Settings (see WORDSNET-4961).
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    case "attachedSchema":
                        docPr.XmlSchemaReferences.Add(xmlReader.ReadVal(), null);
                        break;
                    case "schemaLibrary":
                        ReadAttachedSchemaLibrary(xmlReader, docPr);
                        break;
                    case "hideSpellingErrors":
                        docPr.HideSpellingErrors = xmlReader.ReadBoolVal();
                        break;
                    case "hideGrammaticalErrors":
                        docPr.HideGrammaticalErrors = xmlReader.ReadBoolVal();
                        break;
                    case "shapeDefaults":
                    case "hdrShapeDefaults":
                        NrxShapeDefaultsReader.Read(xmlReader, reader.ConnectorRules);
                        break;
                    case "decimalSymbol":
                    case "listSeparator":
                    {
                        // Word ignores these elements and always reads the system locale settings
                        // to get the values of the radix point symbol and the list item separator.
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    }
                    case "savePreviewPicture":
                    {
                        // Looks like MS Word just removes this option (and a thumbnail), so ignore it.
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    }
                    case "clickAndTypeStyle":
                        reader.ClickAndTypeStyleId = xmlReader.ReadVal();
                        break;
                    case "defaultTableStyle":
                        reader.DefaultTableStyleId = xmlReader.ReadVal();
                        break;
                    case "doNotAutoCompressPictures":
                        docPr.DoNotAutoCompressPictures = xmlReader.ReadBoolVal();
                        break;
                    case "updateFields":
                        docPr.UpdateFields = xmlReader.ReadBoolVal();
                        break;
                    case "forceUpgrade":
                        docPr.ForceUpgrade = xmlReader.ReadBoolVal();
                        break;
                    case "chartTrackingRefBased": // w15:chartTrackingRefBased
                    {
                        complianceInfo.IsDocxExtensions = true;
                        docPr.ChartTrackingRefBased = xmlReader.ReadBoolVal();
                        break;
                    }
                    case "docId":
                    {
                        if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.W15Markup,
                                complianceInfo.Compliance == OoxmlComplianceCore.IsoStrict))
                        {
                            complianceInfo.IsDocxExtensions = true;
                            docPr.DocumentSetId = xmlReader.ReadVal();
                        }
                        else if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.W14Markup,
                                complianceInfo.Compliance == OoxmlComplianceCore.IsoStrict))
                        {
                            complianceInfo.IsDocxExtensions = true;
                            docPr.DocId = xmlReader.ReadVal();
                        }
                        else
                        {
                            xmlReader.IgnoreElement();
                        }
                        break;
                    }
                    case "discardImageEditingData": // w14:discardImageEditingData 
                    {
                        complianceInfo.IsDocxExtensions = true;
                        docPr.DiscardImageEditingData = xmlReader.ReadBoolVal();
                        break;
                    }
                    case "defaultImageDpi": // w14:defaultImageDpi
                    {
                        complianceInfo.IsDocxExtensions = true;
                        docPr.DefaultImageDpi = xmlReader.ReadIntVal();
                        break;
                    }

                    case "rsids":
                        NrxSettingsReader.ReadRsids(xmlReader, docPr);
                        break;

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Read attributes of stylePaneFormatFilter 
        /// (Suggested Filtering for List of Document Styles)
        /// </summary>
        private static void ReadStylePaneFormatFilterSettings(
            NrxXmlReader xmlReader,
            StylePaneFormatFilterSettings settings,
            OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                bool isUnknownAttribute = false;

                switch (xmlReader.LocalName)
                {
                    case "allStyles":
                        settings.AllStyles = xmlReader.ValueAsBool;
                        break;
                    case "alternateStyleNames":
                        settings.AlternateStyleNames = xmlReader.ValueAsBool;
                        break;
                    case "clearFormatting":
                        settings.ClearFormatting = xmlReader.ValueAsBool;
                        break;
                    case "customStyles":
                        settings.CustomStyles = xmlReader.ValueAsBool;
                        break;
                    case "directFormattingOnNumbering":
                        settings.DirectFormattingOnNumbering = xmlReader.ValueAsBool;
                        break;
                    case "directFormattingOnParagraphs":
                        settings.DirectFormattingOnParagraphs = xmlReader.ValueAsBool;
                        break;
                    case "directFormattingOnRuns":
                        settings.DirectFormattingOnRuns = xmlReader.ValueAsBool;
                        break;
                    case "directFormattingOnTables":
                        settings.DirectFormattingOnTables = xmlReader.ValueAsBool;
                        break;
                    case "headingStyles":
                        settings.HeadingStyles = xmlReader.ValueAsBool;
                        break;
                    case "latentStyles":
                        settings.LatentStyles = xmlReader.ValueAsBool;
                        break;
                    case "numberingStyles":
                        settings.NumberingStyles = xmlReader.ValueAsBool;
                        break;
                    case "stylesInUse":
                        settings.StylesInUse = xmlReader.ValueAsBool;
                        break;
                    case "tableStyles":
                        settings.TableStyles = xmlReader.ValueAsBool;
                        break;
                    case "top3HeadingStyles":
                        settings.Top3HeadingStyles = xmlReader.ValueAsBool;
                        break;
                    case "visibleStyles":
                        settings.VisibleStyles = xmlReader.ValueAsBool;
                        break;
                    default:
                        // We don't know this attribute. Skip it.
                        isUnknownAttribute = true;
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.Value);
                        break;
                }

                if (!isUnknownAttribute)
                    complianceInfo.MarkAsIsoTransitional();
            }
        }

        private static string ReadNoLineBreaks(NrxXmlReader xmlReader, DocPr docPr)
        {
            string value = string.Empty;
            while (xmlReader.MoveToNextAttribute())
                switch (xmlReader.LocalName)
                {
                    case "val":
                        value = xmlReader.Value;
                        break;
                    case "lang":
                        docPr.NoLineBreaksLanguage = (Language)LocaleConverter.DocxTagToLocale(xmlReader.Value);
                        break;
                    default:
                        // We don't know this attribute. Skip it.
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.Value);
                        break;
                }
            return value;
        }

        private static void ReadAttachedSchemaLibrary(NrxXmlReader xmlReader, DocPr docPr)
        {
            string uri = null;
            string manifestLocation = null;
            string schemaLanguage = null;
            string schemaLocation = null;

            while (xmlReader.ReadChild("schemaLibrary"))
            {
                switch (xmlReader.LocalName)
                {
                    case "schema":
                    {
                        while (xmlReader.MoveToNextAttribute())
                            switch (xmlReader.LocalName)
                            {
                                case "uri":
                                    uri = xmlReader.Value;
                                    break;
                                case "manifestLocation":
                                    manifestLocation = xmlReader.Value;
                                    break;
                                case "schemaLanguage":
                                    schemaLanguage = xmlReader.Value;
                                    break;
                                case "schemaLocation":
                                    schemaLocation = xmlReader.Value;
                                    break;
                                default:
                                    xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.Value);
                                    break;
                            }
                        XmlNamespace xmlNamespace = new XmlNamespace(uri, manifestLocation, schemaLanguage, schemaLocation);
                        docPr.XmlNamespaces.Add(xmlNamespace);
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadMathProperties(NrxXmlReader xmlReader, MathProperties mathProperties,
            OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.ReadChild("mathPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "brkBin":
                        mathProperties.BreakOnBinary = DocxDopEnum.DocxToMathBreakOnBinary(xmlReader.ReadVal());
                        break;
                    case "brkBinSub":
                        mathProperties.BreakOnBinarySubtraction =
                            DocxDopEnum.DocxToMathBreakOnBinarySubtraction(xmlReader.ReadVal());
                        break;
                    case "defJc":
                        mathProperties.DefaultJustification = DocxEnum.DocxToMathJustification(xmlReader.ReadVal());
                        break;
                    case "dispDef":
                        mathProperties.UseDisplayMathDefaults = xmlReader.ReadBoolVal();
                        break;
                    case "interSp":
                        mathProperties.InterEquationSpacing = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "intLim":
                        mathProperties.IntegralLimitLocation = DocxEnum.DocxToMathLimitLocation(xmlReader.ReadVal());
                        break;
                    case "intraSp":
                        mathProperties.IntraEquationSpacing = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "lMargin":
                        mathProperties.LeftMargin = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "mathFont":
                        mathProperties.DefaultFont = xmlReader.ReadVal();
                        break;
                    case "naryLim":
                        mathProperties.NaryLimitLocation = DocxEnum.DocxToMathLimitLocation(xmlReader.ReadVal());
                        break;
                    case "postSp":
                        mathProperties.PostParagraphSpacing = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "preSp":
                        mathProperties.PreParagraphSpacing = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "rMargin":
                        mathProperties.RightMargin = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "smallFrac":
                        mathProperties.IsSmallFraction = xmlReader.ReadBoolVal();
                        break;
                    case "wrapIndent":
                        mathProperties.WrapIndent = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    case "wrapRight":
                        mathProperties.WrapRight = xmlReader.ReadBoolVal();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }


        /// <summary>
        /// Reads 'w:zoom' element.
        /// </summary>
        private static void ReadZoom(DocxXmlReader xmlReader, ViewOptions viewOptions)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "val":
                        viewOptions.ZoomType = DocxDopEnum.DocxToZoomType(xmlReader.Value);
                        break;
                    case "percent":
                        viewOptions.ZoomPercent = NrxXmlReader.XmlToPercent(xmlReader.Value, xmlReader.ComplianceInfo);
                        break;
                    default:
                        // We don't know this attribute. Skip it.
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'w:proofState' element.
        /// </summary>
        private static void ReadProofState(NrxXmlReader xmlReader, DocPr docPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "spelling":
                        docPr.ProofStateSpelling = DocxDopEnum.DocxToProofState(xmlReader.Value);
                        break;
                    case "grammar":
                        docPr.ProofStateGrammar = DocxDopEnum.DocxToProofState(xmlReader.Value);
                        break;
                    default:
                        // We don't know this attribute. Skip it.
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadWriteProtection(NrxXmlReader xmlReader, DocPr docPr, OoxmlComplianceInfo ciInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "recommended":
                        docPr.WriteProtection.ReadOnlyRecommended = xmlReader.ValueAsBool;
                        break;
                    default:
                        ReadPasswordHashAttr(xmlReader, docPr.WriteProtection.PasswordHash, ciInfo);
                        break;
                }
            }
        }

        private static void ReadDocumentProtection(NrxXmlReader xmlReader, DocumentProtection docProt, OoxmlComplianceInfo cInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "edit":
                        docProt.Edit = DocxDopEnum.DocxToProtectionType(xmlReader.Value);
                        break;
                    case "formatting":
                        docProt.Formatting = xmlReader.ValueAsBool;
                        break;
                    case "enforcement":
                        docProt.Enforcement = xmlReader.ValueAsBool;
                        break;
                    default:
                        ReadPasswordHashAttr(xmlReader, docProt.PasswordHash, cInfo);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a document protection password or a write password. 
        /// The reader must be positioned on the protection element.
        /// </summary>
        private static void ReadPasswordHashAttr(NrxXmlReader xmlReader, PasswordHash passwordHash, OoxmlComplianceInfo cInfo)
        {
            switch (xmlReader.LocalName)
            {
                case "cryptProviderType":
                    // RK This can be "rsaAES" or "rsaFull" according to the spec.
                    // I don't understand what this can be used for, lets ignore.
                    break;
                case "cryptAlgorithmClass":
                    // RK In current OOXML spec this can only be "hash". Lets ignore.
                    break;
                case "cryptAlgorithmType":
                    // RK In the current OOXML spec this can only be "typeAny". Lets ignore.
                    break;
                case "cryptAlgorithmSid":
                    passwordHash.CryptAlgorithmSid = xmlReader.ValueAsInt;
                    break;
                case "algorithmName":
                {
                    cInfo.MarkAsIsoTransitional();
                    passwordHash.CryptAlgorithmSid =
                        PasswordHash.CryptAlgorithmSidFromAlgorithmName(xmlReader.Value);
                    break;
                }
                case "cryptSpinCount":
                    passwordHash.CryptSpinCount = xmlReader.ValueAsInt;
                    break;
                case "spinCount": // ISO 29500 defined attribute.
                {
                    cInfo.MarkAsIsoTransitional();
                    passwordHash.CryptSpinCount = xmlReader.ValueAsInt;
                    break;
                }
                case "hash":
                    passwordHash.Hash = TryConvertFromBase64(xmlReader);
                    break;
                case "hashValue": // ISO 29500 defined attribute.
                {
                    cInfo.MarkAsIsoTransitional();
                    passwordHash.Hash = TryConvertFromBase64(xmlReader);
                    break;
                }
                case "salt":
                    passwordHash.Salt = TryConvertFromBase64(xmlReader);
                    break;
                case "saltValue": // ISO 29500 defined attribute.
                {
                    cInfo.MarkAsIsoTransitional();
                    passwordHash.Salt = TryConvertFromBase64(xmlReader);
                    break;
                }
                default:
                    xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.LocalName);
                    // Do nothing.
                    break;
            }
        }

        private static void ReadCompat(NrxXmlReader xmlReader, CompatibilityOptions compat, OoxmlComplianceInfo cInfo)
        {
            while (xmlReader.ReadChild("compat"))
            {
                switch (xmlReader.LocalName)
                {
                    case "adjustLineHeightInTable":
                        compat.AdjustLineHeightInTable = xmlReader.ReadBoolVal();
                        break;
                    case "alignTablesRowByRow":
                        compat.AlignTablesRowByRow = xmlReader.ReadBoolVal();
                        break;
                    case "allowSpaceOfSameStyleInTable":
                        compat.AllowSpaceOfSameStyleInTable = xmlReader.ReadBoolVal();
                        break;
                    case "applyBreakingRules":
                        compat.ApplyBreakingRules = xmlReader.ReadBoolVal();
                        break;
                    case "autofitToFirstFixedWidthCell":
                        compat.AutofitToFirstFixedWidthCell = xmlReader.ReadBoolVal();
                        break;
                    case "autoSpaceLikeWord95":
                        compat.AutoSpaceLikeWord95 = xmlReader.ReadBoolVal();
                        break;
                    case "balanceSingleByteDoubleByteWidth":
                        compat.BalanceSingleByteDoubleByteWidth = xmlReader.ReadBoolVal();
                        break;
                    case "cachedColBalance":
                        compat.CachedColBalance = xmlReader.ReadBoolVal();
                        break;
                    case "convMailMergeEsc":
                        compat.ConvMailMergeEsc = xmlReader.ReadBoolVal();
                        break;
                    case "displayHangulFixedWidth":
                        compat.DisplayHangulFixedWidth = xmlReader.ReadBoolVal();
                        break;
                    case "doNotAutofitConstrainedTables":
                        compat.DoNotAutofitConstrainedTables = xmlReader.ReadBoolVal();
                        break;
                    case "doNotBreakConstrainedForcedTable":
                        compat.DoNotBreakConstrainedForcedTable = xmlReader.ReadBoolVal();
                        break;
                    case "doNotBreakWrappedTables":
                        compat.DoNotBreakWrappedTables = xmlReader.ReadBoolVal();
                        break;
                    case "doNotExpandShiftReturn":
                        compat.DoNotExpandShiftReturn = xmlReader.ReadBoolVal();
                        break;
                    case "doNotLeaveBackslashAlone":
                        compat.DoNotLeaveBackslashAlone = xmlReader.ReadBoolVal();
                        break;
                    case "doNotSnapToGridInCell":
                        compat.DoNotSnapToGridInCell = xmlReader.ReadBoolVal();
                        break;
                    case "doNotSuppressIndentation":
                        compat.DoNotSuppressIndentation = xmlReader.ReadBoolVal();
                        break;
                    case "doNotSuppressParagraphBorders":
                        compat.DoNotSuppressParagraphBorders = xmlReader.ReadBoolVal();
                        break;
                    case "doNotUseEastAsianBreakRules":
                        compat.DoNotUseEastAsianBreakRules = xmlReader.ReadBoolVal();
                        break;
                    case "doNotUseHTMLParagraphAutoSpacing":
                        compat.DoNotUseHTMLParagraphAutoSpacing = xmlReader.ReadBoolVal();
                        break;
                    case "doNotUseIndentAsNumberingTabStop":
                        compat.DoNotUseIndentAsNumberingTabStop = xmlReader.ReadBoolVal();
                        break;
                    case "doNotVertAlignCellWithSp":
                        compat.DoNotVertAlignCellWithSp = xmlReader.ReadBoolVal();
                        break;
                    case "doNotVertAlignInTxbx":
                        compat.DoNotVertAlignInTxbx = xmlReader.ReadBoolVal();
                        break;
                    case "doNotWrapTextWithPunct":
                        compat.DoNotWrapTextWithPunct = xmlReader.ReadBoolVal();
                        break;
                    case "footnoteLayoutLikeWW8":
                        compat.FootnoteLayoutLikeWW8 = xmlReader.ReadBoolVal();
                        break;
                    case "forgetLastTabAlignment":
                        compat.ForgetLastTabAlignment = xmlReader.ReadBoolVal();
                        break;
                    case "growAutofit":
                        compat.GrowAutofit = xmlReader.ReadBoolVal();
                        break;
                    case "layoutRawTableWidth":
                        compat.LayoutRawTableWidth = xmlReader.ReadBoolVal();
                        break;
                    case "layoutTableRowsApart":
                        compat.LayoutTableRowsApart = xmlReader.ReadBoolVal();
                        break;
                    case "lineWrapLikeWord6":
                        compat.LineWrapLikeWord6 = xmlReader.ReadBoolVal();
                        break;
                    case "mwSmallCaps":
                        compat.MWSmallCaps = xmlReader.ReadBoolVal();
                        break;
                    case "noColumnBalance":
                        compat.NoColumnBalance = xmlReader.ReadBoolVal();
                        break;
                    case "noExtraLineSpacing":
                        compat.NoExtraLineSpacing = xmlReader.ReadBoolVal();
                        break;
                    case "noLeading":
                        compat.NoLeading = xmlReader.ReadBoolVal();
                        break;
                    case "noSpaceRaiseLower":
                        compat.NoSpaceRaiseLower = xmlReader.ReadBoolVal();
                        break;
                    case "noTabHangInd":
                        compat.NoTabHangInd = xmlReader.ReadBoolVal();
                        break;
                    case "printBodyTextBeforeHeader":
                        compat.PrintBodyTextBeforeHeader = xmlReader.ReadBoolVal();
                        break;
                    case "printColBlack":
                        compat.PrintColBlack = xmlReader.ReadBoolVal();
                        break;
                    case "selectFldWithFirstOrLastChar":
                        compat.SelectFldWithFirstOrLastChar = xmlReader.ReadBoolVal();
                        break;
                    case "shapeLayoutLikeWW8":
                        compat.ShapeLayoutLikeWW8 = xmlReader.ReadBoolVal();
                        break;
                    case "showBreaksInFrames":
                        compat.ShowBreaksInFrames = xmlReader.ReadBoolVal();
                        break;
                    case "spaceForUL":
                        compat.SpaceForUL = xmlReader.ReadBoolVal();
                        break;
                    case "spacingInWholePoints":
                        compat.SpacingInWholePoints = xmlReader.ReadBoolVal();
                        break;
                    case "splitPgBreakAndParaMark":
                        compat.SplitPgBreakAndParaMark = xmlReader.ReadBoolVal();
                        break;
                    case "subFontBySize":
                        compat.SubFontBySize = xmlReader.ReadBoolVal();
                        break;
                    case "suppressBottomSpacing":
                        compat.SuppressBottomSpacing = xmlReader.ReadBoolVal();
                        break;
                    case "suppressSpacingAtTopOfPage":
                        compat.SuppressSpacingAtTopOfPage = xmlReader.ReadBoolVal();
                        break;
                    case "suppressSpBfAfterPgBrk":
                        compat.SuppressSpBfAfterPgBrk = xmlReader.ReadBoolVal();
                        break;
                    case "suppressTopSpacing":
                        compat.SuppressTopSpacing = xmlReader.ReadBoolVal();
                        break;
                    case "suppressTopSpacingWP":
                        compat.SuppressTopSpacingWP = xmlReader.ReadBoolVal();
                        break;
                    case "swapBordersFacingPages":
                        compat.SwapBordersFacingPgs = xmlReader.ReadBoolVal();
                        break;
                    case "truncateFontHeightsLikeWP6":
                        compat.TruncateFontHeightsLikeWP6 = xmlReader.ReadBoolVal();
                        break;
                    case "ulTrailSpace":
                        compat.UlTrailSpace = xmlReader.ReadBoolVal();
                        break;
                    case "underlineTabInNumList":
                        compat.UnderlineTabInNumList = xmlReader.ReadBoolVal();
                        break;
                    case "useAltKinsokuLineBreakRules":
                        compat.UseAltKinsokuLineBreakRules = xmlReader.ReadBoolVal();
                        break;
                    case "useAnsiKerningPairs":
                        compat.UseAnsiKerningPairs = xmlReader.ReadBoolVal();
                        break;
                    case "useFELayout":
                        compat.UseFELayout = xmlReader.ReadBoolVal();
                        break;
                    case "useNormalStyleForList":
                        compat.UseNormalStyleForList = xmlReader.ReadBoolVal();
                        break;
                    case "usePrinterMetrics":
                        compat.UsePrinterMetrics = xmlReader.ReadBoolVal();
                        break;
                    case "useSingleBorderforContiguousCells":
                        compat.UseSingleBorderforContiguousCells = xmlReader.ReadBoolVal();
                        break;
                    case "useWord2002TableStyleRules":
                        compat.UseWord2002TableStyleRules = xmlReader.ReadBoolVal();
                        break;
                    case "useWord97LineBreakRules":
                        compat.UseWord97LineBreakRules = xmlReader.ReadBoolVal();
                        break;
                    case "wpJustification":
                        compat.WPJustification = xmlReader.ReadBoolVal();
                        break;
                    case "wpSpaceWidth":
                        compat.WPSpaceWidth = xmlReader.ReadBoolVal();
                        break;
                    case "wrapTrailSpaces":
                        compat.WrapTrailSpaces = xmlReader.ReadBoolVal();
                        break;
                    case "compatSetting": // ISO 29500 specific.
                        ReadCustomCompatibilitySetting(compat, xmlReader, cInfo);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadCustomCompatibilitySetting(CompatibilityOptions compat, NrxXmlReader xmlReader, OoxmlComplianceInfo cInfo)
        {
            string name = "";
            string uri = "";
            string val = "";

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "name":
                        name = xmlReader.Value;
                        break;
                    case "uri":
                        uri = xmlReader.Value;
                        break;
                    case "val":
                        val = xmlReader.Value;
                        break;
                    default:
                        // We don't know this attribute. Skip it.
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
            cInfo.MarkAsIsoTransitional();
            if (name == "compatibilityMode")
                compat.MswVersion = OoxmlComplianceInfo.MsWordVersionFromCompatibilityMode(val);

            compat.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting(name, uri, val));
        }

        private static void ReadVars(NrxXmlReader xmlReader, VariableCollection vars)
        {
            while (xmlReader.ReadChild("docVars"))
            {
                if (xmlReader.LocalName == "docVar")
                {
                    string name = null;
                    string val = null;

                    while (xmlReader.MoveToNextAttribute())
                    {
                        switch (xmlReader.LocalName)
                        {
                            case "name":
                                name = xmlReader.Value;
                                break;
                            case "val":
                                // WORDSNET-9275/9830/11735
                                val = xmlReader.Value.Replace("_x000d_", "\r").Replace("_x000D_", "\r").Replace("_x000b_", "\v").Replace("_x000B_", "\v");
                                break;
                            default:
                                // We don't know this attribute. Skip it.
                                xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                                break;
                        }
                    }

                    if (name != null)
                        vars.Add(name, val);
                }
                else if (xmlReader.LocalName == "docVars")
                {
                    // WORDSNET-6248 Nested 'docVars' elements. 
                    // MS Word loads file successfully and has access to variables so we have to read it.
                    ReadVars(xmlReader, vars);
                }
                else
                {
                    xmlReader.IgnoreElement();
                }
            }
        }

        private static void ReadRevisionView(NrxXmlReader xmlReader, DocPr docPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "comments":
                        docPr.ShowAnnotations = xmlReader.ValueAsBool;
                        break;
                    case "formatting":
                        docPr.ShowFormatting = xmlReader.ValueAsBool;
                        break;
                    case "inkAnnotations":
                        docPr.ShowInkAnnotations = xmlReader.ValueAsBool;
                        break;
                    case "insDel":
                        docPr.ShowInsertionsDeletions = xmlReader.ValueAsBool;
                        break;
                    case "markup":
                        docPr.ShowMarkup = xmlReader.ValueAsBool;
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
        }

        internal static void ReadFootnotePr(DocxDocumentReaderBase reader, string elemName, AttrCollection attrs)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            bool isEndnote = (elemName == "endnotePr");
            int keyDelta = (isEndnote) ? SectAttr.EndnoteKeyDelta : 0;

            while (xmlReader.ReadChild(elemName))
            {
                switch (xmlReader.LocalName)
                {
                    case "pos":
                    {
                        attrs.SetAttr(SectAttr.FootnoteLocation + keyDelta, isEndnote
                            ? (object)NrxSectEnum.XmlToEndnotePosition(xmlReader.ReadVal())
                            : (object)NrxSectEnum.XmlToFootnotePosition(xmlReader.ReadVal()));
                        break;
                    }
                    case "numFmt":
                        attrs.SetAttr(SectAttr.FootnoteNumberStyle + keyDelta, DocxEnum.DocxToNumberStyle(xmlReader.ReadVal()));
                        break;
                    case "numStart":
                        attrs.SetAttr(SectAttr.FootnoteStartNumber + keyDelta, xmlReader.ReadIntVal());
                        break;
                    case "numRestart":
                        attrs.SetAttr(SectAttr.FootnoteNumberingRule + keyDelta, NrxSectEnum.XmlToNoteNumberingRule(xmlReader.ReadVal()));
                        break;
                    case "footnote":
                    case "endnote":
                    {
                        // WORDSNET-16479 Notify customer that footnote/endnote part is missing.
                        string relType = (xmlReader.LocalName == "footnote")
                            ? reader.RelTypes.Footnotes
                            : reader.RelTypes.Endnotes;

                        if(reader.GetPartByRelationshipType(reader.DocumentPart, relType) == null)
                            reader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, WarningStrings.FootnotePartMissing);

                        // These occur in document-wide element and are references to footnote separators.
                        // It is not ideal, but we read separators straight away without following this reference.
                        // andrnosk: I think we do not need to warn user in this case.
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadThemeFontLang(NrxXmlReader xmlReader, ThemeFontLanguages languages)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "bidi":
                        languages.Bidi = (Language)LocaleConverter.DocxTagToLocale(xmlReader.Value);
                        break;
                    case "eastAsia":
                        languages.EastAsia = (Language)LocaleConverter.DocxTagToLocale(xmlReader.Value);
                        break;
                    case "val":
                        languages.Latin = (Language)LocaleConverter.DocxTagToLocale(xmlReader.Value);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
        }

        /// <summary>
        /// Returns a byte array that is equivalent to the given base64 string or null if the conversion failed.
        /// </summary>
        private static byte[] TryConvertFromBase64(NrxXmlReader xmlReader)
        {
            string base64String = xmlReader.Value;
            byte[] bytes = StringUtil.ConvertFromBase64Safe(base64String);

            if (bytes.Length == 0)
            {
                // WORDSNET-27395 We have to inform the user that the password to protect the document is corrupted.
                xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, WarningStrings.UnexpectedBase64);

                return null;
            }

            return bytes;
        }
    }
}
