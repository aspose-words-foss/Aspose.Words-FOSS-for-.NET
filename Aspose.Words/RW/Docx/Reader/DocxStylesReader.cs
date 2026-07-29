// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2007 by Vladimir Averkin

using System;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Settings;
using Aspose.Words.Styles;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides methods for reading "Styles" package part
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxStylesReader
    {
        internal DocxStylesReader(DocxParaPrReader paraPrReader,
            DocxRunPrReader runPrReader)
        {
            Debug.Assert(paraPrReader != null);
            Debug.Assert(runPrReader != null);
            mParaPrReader = paraPrReader;
            mRunPrReader = runPrReader;
        }

        /// <summary>
        /// Reads "Styles" part for the specified document.
        /// </summary>
        internal void Read(DocxDocumentReaderBase reader)
        {
            if (reader.LoadOptions.SkipFormatting)
                return;

            bool defaultsRead = false;
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.Styles);
            if (xmlReader != null)
            {
                NrxUnresolvedStylePartCollection unresolvedParts = new NrxUnresolvedStylePartCollection();

                while (xmlReader.ReadChild("styles"))
                {
                    switch (xmlReader.LocalName)
                    {
                        case "docDefaults":
                            defaultsRead = true;
                            ReadDocDefaults(reader);
                            break;
                        case "latentStyles":
                            ReadLatentStyles(reader);
                            break;
                        case "style":
                            ReadStyle(reader, unresolvedParts);
                            break;
                        default:
                            xmlReader.IgnoreElement();
                            break;
                    }
                }

                unresolvedParts.ResolveStyleLinks(reader);
                reader.RestorePartReader();
            }

            // When either rPrDefault or pPrDefault tags are missing,
            // Word loads some predefined document defaults depending on Word application version.
            if (!defaultsRead)
            {
                InitDefaultParaPr(reader);
                InitDefaultRunPr(reader);
            }

            // WORDSNET-23715 Fix base styles before we started to resolve toggle issues.
            reader.Document.Styles.FixUpBasedOnStyles();
            // WORDSNET-26869 Fixed style name case issue.
            reader.Document.Styles.FixUpCaseIssue();
        }

        private void ReadDocDefaults(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            bool defaultRunPrRead = false;
            bool defaultParaPrRead = false;

            while (xmlReader.ReadChild("docDefaults"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rPrDefault":
                        defaultRunPrRead = true;
                        ReadRPrDefault(reader);
                        break;
                    case "pPrDefault":
                        defaultParaPrRead = true;
                        ReadPPrDefault(reader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            // When either rPrDefault or pPrDefault tags are missing,
            // Word loads some predefined document defaults depending on Word application version.
            if (!defaultRunPrRead)
                InitDefaultRunPr(reader);
            if (!defaultParaPrRead)
                InitDefaultParaPr(reader);
        }

        private void ReadRPrDefault(DocxDocumentReaderBase reader)
        {
            StyleCollection styles = reader.Document.Styles;

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("rPrDefault"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rPr":
                    {
                        RunPr runPr = styles.DefaultRunPr;
                        mRunPrReader.Read(reader, runPr);
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private void ReadPPrDefault(DocxDocumentReaderBase reader)
        {
            StyleCollection styles = reader.Document.Styles;

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("pPrDefault"))
            {
                switch (xmlReader.LocalName)
                {
                    case "pPr":
                    {
                        ParaPr paraPr = styles.DefaultParaPr;
                        RunPr runPr = styles.DefaultRunPr;
                        mParaPrReader.Read(reader, paraPr, runPr);
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            // andrnosk: WORDSNET-10126 Mimic MSWord behavior, ignore framePr inside ParaPrDefault.
            styles.DefaultParaPr.RemoveFloatingAttrs();
        }

        private static void ReadLatentStyles(DocxDocumentReaderBase reader)
        {
            LatentStyles latentStyles = reader.Document.Styles.LatentStyles;
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "count":
                        latentStyles.KnownStylesCount = xmlReader.ValueAsInt;
                        break;
                    case "defLockedState":
                        latentStyles.DefaultLockedState = xmlReader.ValueAsBool;
                        break;
                    case "defQFormat":
                        latentStyles.DefaultQuickFormat = xmlReader.ValueAsBool;
                        break;
                    case "defSemiHidden":
                        latentStyles.DefaultSemiHidden = xmlReader.ValueAsBool;
                        break;
                    case "defUIPriority":
                        latentStyles.DefaultUIPriority = xmlReader.ValueAsInt;
                        break;
                    case "defUnhideWhenUsed":
                        latentStyles.DefaultUnhideWhenUsed = xmlReader.ValueAsBool;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // Read elements.
            while (xmlReader.ReadChild("latentStyles"))
            {
                switch (xmlReader.LocalName)
                {
                    case "lsdException":
                        ReadLsdException(reader);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        private static void ReadLsdException(DocxDocumentReaderBase reader)
        {
            LatentStyles latentStyles = reader.Document.Styles.LatentStyles;

            bool locked = latentStyles.DefaultLockedState;
            string name = "";
            bool qFormat = latentStyles.DefaultQuickFormat;
            bool semiHidden = latentStyles.DefaultSemiHidden;
            int uiPriority = latentStyles.DefaultUIPriority;
            bool unhideWhenUsed = latentStyles.DefaultUnhideWhenUsed;

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "locked":
                        locked = xmlReader.ValueAsBool;
                        break;
                    case "name":
                        name = xmlReader.Value;
                        break;
                    case "qFormat":
                        qFormat = xmlReader.ValueAsBool;
                        break;
                    case "semiHidden":
                        semiHidden = xmlReader.ValueAsBool;
                        break;
                    case "uiPriority":
                        uiPriority = xmlReader.ValueAsInt;
                        break;
                    case "unhideWhenUsed":
                        unhideWhenUsed = xmlReader.ValueAsBool;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            StyleIdentifier styleIdentifier = StyleConvertUtil.XmlToStyleIdentifier(name);
            if (styleIdentifier != StyleIdentifier.User)
            {
                latentStyles.Add(new LatentStyle(
                    styleIdentifier, locked, qFormat, semiHidden, uiPriority, unhideWhenUsed));
            }
        }

        /// <summary>
        /// Reads 'w:style' element from 'w:styles' children.
        /// </summary>
        private void ReadStyle(DocxDocumentReaderBase reader, NrxUnresolvedStylePartCollection unresolvedParts)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            StyleCollection styles = reader.Document.Styles;

            // Read attributes into temporary local variables.
            bool isCustomStyle = false;
            string styleId = null;
            StyleType styleType = StyleType.Paragraph;

            bool isDefault = false;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "customStyle":
                        isCustomStyle = xmlReader.ValueAsBool;
                        break;
                    case "default":
                        // WORDSNET-23886 Read as boolean, instead of just as 'true' value.
                        isDefault = xmlReader.ValueAsBool;
                        break;
                    case "styleId":
                        // This string identifies style in DOCX, we convert it to istd below for the model use.
                        styleId = xmlReader.Value;
                        break;
                    case "type":
                        styleType = DocxEnum.DocxToStyleType(xmlReader.Value);
                        break;
                    default:
                        {
                            string message = String.Format(WarningStrings.UnexpectedAttribute, xmlReader.LocalName);
                            reader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, message);
                            break;
                        }
                }
            }

            // This is the style we are building.
            Style style = Style.Create(styleType);
            NrxUnresolvedStylePart unresolvedStylePart = new NrxUnresolvedStylePart(style);

            string name = null;
            string aliases = null;

            while (xmlReader.ReadChild("style"))
            {
                switch (xmlReader.LocalName)
                {
                    case "name":
                    {
                        // We have to do this as soon as we read the name because this is a primary
                        // non-localized style name and we use it to recognize built-in styles.
                        // Further code that reads formatting properties relies on istd properly set.
                        name = xmlReader.ReadVal();
                        break;
                    }
                    case "aliases":
                        aliases = xmlReader.ReadVal();
                        break;
                    case "basedOn":
                        unresolvedStylePart.BasedOn = xmlReader.ReadVal();
                        break;
                    case "next":
                        unresolvedStylePart.Next = xmlReader.ReadVal();
                        break;
                    case "link":
                        unresolvedStylePart.Link = xmlReader.ReadVal();
                        break;
                    case "autoRedefine":
                        style.AutomaticallyUpdate = xmlReader.ReadBoolVal();
                        break;
                    case "hidden":
                        style.Hidden = xmlReader.ReadBoolVal();
                        break;
                    case "uiPriority":
                        style.Priority = xmlReader.ReadIntVal();
                        break;
                    case "unhideWhenUsed":
                        style.UnhideWhenUsed = xmlReader.ReadBoolVal();
                        break;
                    case "qFormat":
                        style.IsQuickStyle = xmlReader.ReadBoolVal();
                        break;
                    case "semiHidden":
                        style.SemiHidden = xmlReader.ReadBoolVal();
                        break;
                    case "locked":
                        style.Locked = xmlReader.ReadBoolVal();
                        break;
                    case "personal":
                        style.Personal = xmlReader.ReadBoolVal();
                        break;
                    case "personalCompose":
                        style.PersonalCompose = xmlReader.ReadBoolVal();
                        break;
                    case "personalReply":
                        style.PersonalReply = xmlReader.ReadBoolVal();
                        break;
                    case "rsid":
                    {
                        // WORDSNET-5028, xmlReader.ReadVal() can be not hex value, so we try parse and on error return int.MinValue
                        int rsid = NrxXmlUtil.TryHexToInt(xmlReader.ReadVal());
                        if (rsid != int.MinValue)
                            style.Rsid = rsid;
                        break;
                    }
                    case "pPr":
                        mParaPrReader.Read(reader, style.ParaPr, style.RunPr);
                        break;
                    case "rPr":
                        mRunPrReader.Read(reader, style.RunPr);
                        break;
                    default:
                        // All other elements are related to table styles.
                        ReadTableStyleElement(reader, style);
                        break;
                }
            }

            // RESILIENCY WORDSNET-22563 The problem occurred because the table style does not have name.
            // We ignored such styles, however MS Word read them normally.
            // It seems styleId and StyleName are interchangeable. So if id is not specified we use name of the style instead id,
            // and if name is not specified we use id instead of name.
            if (!StringUtil.HasChars(name) && StringUtil.HasChars(styleId))
                name = styleId;

            // WORDSNET-25354 Do not read styles exceeding maximum allowed count.
            if (!reader.Document.Styles.HasFreeIstd())
                return;

            if (StringUtil.HasChars(name))
            {
                NrxStyleUtil.SetStyleNameAndIdentifiers(name, styles, isCustomStyle, !isCustomStyle, true, style);

                // WORDSNET-15791 There is no style id. MS Word reads it okay, so do we now.
                if (!StringUtil.HasChars(styleId))
                    styleId = name;

                reader.AddStyleIdToIstdMapping(styleId, style.Istd);
            }

            string[] aliasesArray = null;
            if (aliases != null)
                aliasesArray = aliases.Split(',');

            // WORDSNET-5506 RK If a style has no name, it will not be properly initialized by the above code,
            // it is better to ignore such style completely.
            if (StringUtil.HasChars(style.Name))
            {
                styles.AddForLoad(style, aliasesArray);

                unresolvedParts.Add(unresolvedStylePart);
            }

            if (isDefault)
                reader.DefaultStyles[(int)style.Type] = style;
        }

        private void ReadTableStyleElement(DocxDocumentReaderBase reader, Style style)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // RK Safe cast and check for null just in case table style properties occur for a non table style.
            TableStyle tableStyle = style as TableStyle;
            if (tableStyle == null)
            {
                xmlReader.IgnoreElement();
                return;
            }

            switch (xmlReader.LocalName)
            {
                case "tblPr":
                    DocxTablePrReader.ReadStyleTblPr(reader, tableStyle.TablePr);
                    break;
                case "trPr":
                    DocxRowPrReader.Read(reader, tableStyle.RowPr);
                    break;
                case "tcPr":
                {
                    DocxReaderFactory.CellPrReader.Read(reader, tableStyle.CellPr);
                    // WORDSNET-25389 Seems Word ignores TextOrientation property in table style.
                    tableStyle.CellPr.Remove(CellAttr.Orientation);
                    break;
                }
                case "tblStylePr":
                    tableStyle.AddConditionalStyle(ReadConditionalStyle(reader));
                    break;
                default:
                    xmlReader.IgnoreElement();
                    break;
            }
        }

        /// <summary>
        /// Reads 2.7.5.6 tblStylePr (Style Conditional Table Formatting Properties).
        /// </summary>
        private ConditionalStyle ReadConditionalStyle(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            TableStyleOverrideType type = TableStyleOverrideType.None;

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "type":
                        type = StyleConvertUtil.XmlToTableStyleOverrideType(xmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            ConditionalStyle conditionalStyle = new ConditionalStyle(type);

            // Read elements.
            while (xmlReader.ReadChild("tblStylePr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "pPr":
                        conditionalStyle.ParaPr = new ParaPr();
                        mParaPrReader.Read(reader, conditionalStyle.ParaPr, conditionalStyle.RunPr);
                        break;
                    case "rPr":
                        conditionalStyle.RunPr = new RunPr();
                        mRunPrReader.Read(reader, conditionalStyle.RunPr);
                        break;
                    case "tblPr":
                        conditionalStyle.TablePr = new TablePr();
                        DocxTablePrReader.ReadStyleTblPr(reader, conditionalStyle.TablePr);
                        break;
                    case "trPr":
                        conditionalStyle.RowPr = new TablePr();
                        DocxRowPrReader.Read(reader, conditionalStyle.RowPr);
                        break;
                    case "tcPr":
                        conditionalStyle.CellPr = new CellPr();
                        DocxReaderFactory.CellPrReader.Read(reader, conditionalStyle.CellPr);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
            return conditionalStyle;
        }

        /// <summary>
        /// Inits document defaults depending on specified version.
        /// </summary>
        private static void InitDefaultParaPr(DocxDocumentReaderBase reader)
        {
            // WORDSNET-19598 In case of missing the 'pPrDefault' element inside the 'docDefaults' (styles.xml)
            // Chinese preferred language doesn't need any 'spacing' attributes.
            //
            int defaultEditingLanguage = (int)reader.LoadOptions.LanguagePreferences.DefaultEditingLanguage;
            if (LocaleClassifier.IsChineseOrJapanese(defaultEditingLanguage))
                return;

            ParaPr paraPr = reader.Document.Styles.DefaultParaPr;

            switch (reader.LoadOptions.MswVersion)
            {
                case MsWordVersion.Word2007:
                case MsWordVersion.Word2010:
                    Set2007DefaultParaPr(paraPr);
                    break;
                case MsWordVersion.Word2013:
                case MsWordVersion.Word2016:
                case MsWordVersion.Word2019:
                default:
                    Set2013DefaultParaPr(paraPr);
                    break;
            }
        }

        /// <summary>
        /// Sets paraPr which were the DefaultParaPr in the MSW2007.
        /// </summary>
        private static void Set2007DefaultParaPr(ParaPr paraPr)
        {
            paraPr.SpaceAfter = 200;
            paraPr.LineSpacing = 276;
            paraPr.LineSpacingRule = LineSpacingRule.Multiple;
        }

        /// <summary>
        /// Sets paraPr which were the DefaultParaPr in the MSW2013.
        /// </summary>
        private static void Set2013DefaultParaPr(ParaPr paraPr)
        {
            paraPr.SpaceAfter = 160;
            paraPr.LineSpacing = 259;
            paraPr.LineSpacingRule = LineSpacingRule.Multiple;
        }

        /// <summary>
        /// Inits document defaults depending on specified version.
        /// </summary>
        private static void InitDefaultRunPr(DocxDocumentReaderBase reader)
        {
            RunPr runPr = reader.Document.Styles.DefaultRunPr;

            switch (reader.LoadOptions.MswVersion)
            {
                case MsWordVersion.Word2007:
                case MsWordVersion.Word2010:
                case MsWordVersion.Word2013:
                case MsWordVersion.Word2016:
                case MsWordVersion.Word2019:
                default:
                    Set2007DefaultRunPr(runPr);
                    break;
            }
        }

        /// <summary>
        /// Sets RunPr which were the DefaultRunPr in the MSW2007
        /// </summary>
        private static void Set2007DefaultRunPr(RunPr runPr)
        {
            runPr.ComplexNameAscii = ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi);
            runPr.ComplexNameOther = ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi);
            runPr.ComplexNameFarEast = ComplexFontName.FromTheme(ThemeFontCore.MinorEastAsia);
            runPr.ComplexNameBi = ComplexFontName.FromTheme(ThemeFontCore.MinorBidi);

            runPr.Size = 22;
            runPr.SizeBi = 22;
        }


        private readonly DocxParaPrReader mParaPrReader;
        private readonly DocxRunPrReader mRunPrReader;
    }
}
