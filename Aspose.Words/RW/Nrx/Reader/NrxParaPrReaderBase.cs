// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2006 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Nrx;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides methods for reading paragraph properties in DOCX and WML.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxParaPrReaderBase
    {
        protected NrxParaPrReaderBase(NrxRunPrReaderBase runPrReader,
            NrxSectPrReaderBase sectPrReader) 
        {
            Debug.Assert(runPrReader != null);
            Debug.Assert(sectPrReader != null);
            mRunPrReader = runPrReader;
            mSectPrReader = sectPrReader;
        }

        /// <summary>
        /// Reads 'w:pPr' element from the specified reader.
        /// </summary>
        /// <param name="reader">Should be positioned on the element start.</param>
        /// <param name="paraPr">Paragraph properties to read into.</param>
        /// <param name="runPr">Normally this is the run properties of the paragraph break to read into. 
        /// But I also found some documents where run properties occur inside pPr and MS Word seems to read them okay. So we do the same.</param>
        internal bool Read(NrxDocumentReaderBase reader, 
            ParaPr paraPr, 
            RunPr runPr)
        {
            if (reader.LoadOptions.SkipFormatting)
            {
                // WORDSNET-13301 Text of some headers/footers and text that appears directly in an pPr element
                // were not extracted.
                return ReadSectPrAndTextIfExist(reader, runPr);
            }

            bool isSectionBreak = false;
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            while (xmlReader.ReadChild("pPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "pStyle":
                    {
                        // WORDSNET-23667 Use style index of the actual default style by default. 
                        Style defaultStyle = reader.DefaultStyles[(int)StyleType.Paragraph];
                        int defaultStyleIndex = (defaultStyle != null) ? defaultStyle.Istd : StyleIndex.Normal;
                        
                        int istd = reader.ResolveStyleIdToIstd(xmlReader.ReadVal(), defaultStyleIndex);
                        paraPr.SetAttr(ParaAttr.Istd, istd);
                        break;
                    }
                    case "keepNext":
                        paraPr.SetAttr(ParaAttr.KeepWithNext, xmlReader.ReadBoolVal());
                        break;
                    case "keepLines":
                        paraPr.SetAttr(ParaAttr.KeepTogether, xmlReader.ReadBoolVal());
                        break;
                    case "pageBreakBefore":
                        paraPr.SetAttr(ParaAttr.PageBreakBefore, xmlReader.ReadBoolVal());
                        break;
                    case "framePr":
                        ReadFramePr(xmlReader, paraPr, complianceInfo);
                        break;
                    case "widowControl":
                        paraPr.SetAttr(ParaAttr.WidowControl, xmlReader.ReadBoolVal());
                        break;
                    case "numPr":
                    case "listPr":  // WML
                        ReadNumPrWithCollectingListId(reader, paraPr);
                        break;
                    case "suppressLineNumbers":
                    case "supressLineNumbers":  // WML
                        paraPr.SetAttr(ParaAttr.SuppressLineNumbers, xmlReader.ReadBoolVal());
                        break;
                    case "pBdr":
                        ReadBorders(xmlReader, paraPr);
                        break;
                    case "shd":
                        paraPr.SetAttr(ParaAttr.Shading, xmlReader.ReadShading());
                        break;
                    case "tabs":
                        ReadTabs(xmlReader, paraPr, complianceInfo);
                        break;
                    case "suppressAutoHyphens":
                        paraPr.SetAttr(ParaAttr.SuppressAutoHyphens, xmlReader.ReadBoolVal());
                        break;
                    case "kinsoku":
                        paraPr.SetAttr(ParaAttr.FarEastLineBreakControl, xmlReader.ReadBoolVal());
                        break;
                    case "wordWrap":
                        paraPr.SetAttr(ParaAttr.WordWrap, xmlReader.ReadBoolVal());
                        break;
                    case "overflowPunct":
                        paraPr.SetAttr(ParaAttr.HangingPunctuation, xmlReader.ReadBoolVal());
                        break;
                    case "topLinePunct":
                        paraPr.SetAttr(ParaAttr.TopLinePunctuation, xmlReader.ReadBoolVal());
                        break;
                    case "autoSpaceDE":
                        paraPr.SetAttr(ParaAttr.AddSpaceBetweenFarEastAndAlpha, xmlReader.ReadBoolVal());
                        break;
                    case "autoSpaceDN":
                        paraPr.SetAttr(ParaAttr.AddSpaceBetweenFarEastAndDigit, xmlReader.ReadBoolVal());
                        break;
                    case "bidi":
                        paraPr.SetAttr(ParaAttr.Bidi, xmlReader.ReadBoolVal());
                        break;
                    case "adjustRightInd":
                        paraPr.SetAttr(ParaAttr.AutoAdjustRightIndent, xmlReader.ReadBoolVal());
                        break;
                    case "snapToGrid":
                        paraPr.SetAttr(ParaAttr.SnapToGrid, xmlReader.ReadBoolVal());
                        break;
                    case "spacing":
                        ReadSpacing(xmlReader, paraPr, complianceInfo);
                        break;
                    case "ind":
                        ReadIndents(xmlReader, paraPr, complianceInfo);
                        break;
                    case "contextualSpacing":
                        paraPr.SetAttr(ParaAttr.NoSpaceBetweenSameStyle, xmlReader.ReadBoolVal());
                        break;
                    case "suppressOverlap":
                        paraPr.SetAttr(ParaAttr.FrameSuppressOverlap, xmlReader.ReadBoolVal());
                        break;
                    case "jc":
                        paraPr.SetAttr(ParaAttr.Alignment, NrxParaEnum.XmlToParagraphAlignment(xmlReader.ReadVal(),
                            complianceInfo));
                        break;
                    case "textDirection":
                        paraPr.SetAttr(ParaAttr.FrameTextOrientation,
                            StyleConvertUtil.XmlToTextOrientation(xmlReader.ReadVal(), complianceInfo));
                        break;
                    case "textAlignment":
                        paraPr.SetAttr(ParaAttr.BaselineAlignment, NrxParaEnum.XmlToBaselineAlignment(xmlReader.ReadVal()));
                        break;
                    case "outlineLvl":
                        paraPr.SetAttr(ParaAttr.OutlineLevel, (OutlineLevel)xmlReader.ReadIntVal());
                        break;
                    case "divId":
                        paraPr.SetAttr(ParaAttr.HtmlBlockId, xmlReader.ReadIntVal());
                        break;
                    case "rPr":
                        mRunPrReader.Read(reader, runPr);
                        break;
                    case "sectPr":
                    {
                        ReadSectPr(reader);
                        isSectionBreak = true;
                        break;
                    }
                    case "mirrorIndents":
                        paraPr.SetAttr(ParaAttr.MirrorIndents, xmlReader.ReadBoolVal());
                        break;
                    case "textboxTightWrap":
                        paraPr.SetAttr(ParaAttr.TextboxTightWrap, NrxParaEnum.XmlToTextboxTightWrap(xmlReader.ReadVal()));
                        break;
                    case "cnfStyle":
                        xmlReader.IgnoreElement();
                        break;
                    case "collapsed":
                        paraPr.SetAttr(ParaAttr.Collapsed, xmlReader.ReadBoolVal());
                        complianceInfo.IsDocxExtensions = true;
                        break;
                    default:
                        // WORDSNET-7923 If this is an unknown pPr attribute, it could be a rPr attribute and MS Word seems to handle it okay so do we.
                        ReadFormatSpecific(xmlReader.LocalName, reader, paraPr, runPr);
                            mRunPrReader.ReadChild(reader, runPr, true);
                        break;
                }
            }

            return isSectionBreak;
        }

        [JavaThrows(true)]
        protected abstract bool ReadFormatSpecific(string localName,
            NrxDocumentReaderBase reader,
            ParaPr paraPr,
            RunPr runPr);

        /// <summary>
        /// Passes through all child nodes of the 'pPr' element and reads the 'sectPr' and text elements if they exist.
        /// </summary>
        /// <param name="reader">Document reader.</param>
        /// <param name="runPr">Properties that will be used for created runs.</param>
        /// <returns>Returns true if the sectPr element has been read.</returns>
        private bool ReadSectPrAndTextIfExist(NrxDocumentReaderBase reader, RunPr runPr)
        {
            bool isSectionBreak = false;
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("pPr"))
            {
                if (xmlReader.LocalName == "sectPr")
                {
                    ReadSectPr(reader);
                    isSectionBreak = true;
                }
                else if ((xmlReader.LocalName == "t") || (xmlReader.LocalName == "r"))
                {
                    // ReadRunPrChild can read the 't' and 'r' elements.
                    mRunPrReader.ReadChild(reader, runPr, false);
                }
                else
                {
                    xmlReader.IgnoreElementNoWarn();
                }
            }
            return isSectionBreak;
        }

        /// <summary>
        /// Reads the sectPr element (Section Properties).
        /// </summary>
        /// <param name="reader">Document reader.</param>
        private void ReadSectPr(NrxDocumentReaderBase reader)
        {
            // We must be inside the last paragraph of a section.
            Paragraph para = (Paragraph)reader.CurContainer;

            if ((para != null) && (para.ParentSection != null))
            {
                reader.GlobalSectPr.ExpandTo(para.ParentSection.SectPr);
                mSectPrReader.Read(reader, para.ParentSection.SectPr);

                // Word clears accumulated section properties each time when it encounters SectPr inside ParaPr.
                // However, accumulated properties for very last section must be preserved to use it in case
                // when no new properties for last section is read.
                if (reader.GlobalSectPr.Count > 0)
                {
                    reader.LastSectPr.Clear();
                    reader.GlobalSectPr.ExpandTo(reader.LastSectPr);
                    reader.GlobalSectPr.Clear();
                }
            }
        }

        internal void ReadNumPrWithCollectingListId(NrxDocumentReaderBase reader, ParaPr paraPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            ReadNumPr(xmlReader, paraPr);
            // Collect ListId.
            if (paraPr.ListId != 0)
                reader.UsedListIds.Add(paraPr.ListId);
        }

        private static void ReadFramePr(NrxXmlReader xmlReader, ParaPr paraPr, OoxmlComplianceInfo complianceInfo)
        {
            bool heightRead = false;
            HeightRule hRule = HeightRule.AtLeast;
            int val = 0;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "dropCap":
                    case "drop-cap":    // WML
                        paraPr.SetAttr(ParaAttr.DropCapPosition, NrxParaEnum.XmlToDropCapPosition(xmlReader.Value));
                        break;
                    case "lines":
                        paraPr.SetAttr(ParaAttr.DropCapLinesToDrop, xmlReader.ValueAsInt);
                        break;
                    case "w":
                        paraPr.SetAttr(ParaAttr.FrameWidth, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "h":
                        val = xmlReader.GetValueAsTwips(complianceInfo);
                        heightRead = true;
                        break;
                    case "vSpace":
                    case "vspace":  // WML
                        paraPr.SetAttr(ParaAttr.FrameVerticalDistanceFromText, 
                            xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "hSpace":
                    case "hspace":  // WML
                        paraPr.SetAttr(ParaAttr.FrameHorizontalDistanceFromText, 
                            xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "wrap":
                        paraPr.SetAttr(ParaAttr.FrameWrapType, NrxParaEnum.XmlToTextFrameWrapType(xmlReader.Value));
                        break;
                    case "hAnchor":
                    case "hanchor": // WML
                        paraPr.SetAttr(ParaAttr.FrameRelativeHorizontalPosition, NrxParaEnum.XmlToRelativeHorizontalPosition(xmlReader.Value));
                        break;
                    case "vAnchor":
                    case "vanchor": // WML
                        paraPr.SetAttr(ParaAttr.FrameRelativeVerticalPosition, NrxParaEnum.XmlToRelativeVerticalPosition(xmlReader.Value));
                        break;
                    case "x":
                        paraPr.SetAttr(ParaAttr.FrameLeft, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "y":
                        paraPr.SetAttr(ParaAttr.FrameTop, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "xAlign":
                    case "x-align": // WML
                        paraPr.SetAttr(ParaAttr.FrameHorizontalAlignment, NrxParaEnum.XmlToHorizontalAlignment(xmlReader.Value));
                        break;
                    case "yAlign":
                    case "y-align": // WML
                        paraPr.SetAttr(ParaAttr.FrameVerticalAlignment, NrxParaEnum.XmlToVerticalAlignment(xmlReader.Value));
                        break;
                    case "hRule":
                    case "h-rule":  // WML
                        hRule = NrxParaEnum.XmlToHeightRule(xmlReader.Value);
                        heightRead = true;
                        break;
                    case "anchorLock":
                        paraPr.SetAttr(ParaAttr.FrameLockAnchor, xmlReader.ValueAsBool);
                        break;
                    case "anchor-lock": // WML
                        paraPr.SetAttr(ParaAttr.FrameLockAnchor, xmlReader.ValueAsBool);
                        break;
                    default:
                        break;
                }
            }

            if (heightRead)
                paraPr.SetAttr(ParaAttr.FrameHeight, new Height(hRule, val));
        }

        private static void ReadIndents(NrxXmlReader xmlReader, ParaPr paraPr, OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "left":
                        paraPr.SetAttr(ParaAttr.LeftIndent, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "start": // iso29500
                        {
                            complianceInfo.MarkAsIsoTransitional();
                            paraPr.SetAttr(ParaAttr.LeftIndent, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                        }
                    case "right":
                        paraPr.SetAttr(ParaAttr.RightIndent, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "end": // iso29500
                        {
                            complianceInfo.MarkAsIsoTransitional();
                            paraPr.SetAttr(ParaAttr.RightIndent, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                        }
                    //We can have either "firstLine" attributes or "hanging" attributes.
                    case "firstLine":
                    case "first-line":  // WML
                        paraPr.SetAttr(ParaAttr.FirstLineIndent, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "hanging":
                        // RK I don't think hanging is just a negative of firstLine.
                        // Hanging usually affects left indent too. Need to check in unified tests.
                        paraPr.SetAttr(ParaAttr.FirstLineIndent, -xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "leftChars":
                    case "left-chars":  // WML
                        paraPr.SetAttr(ParaAttr.LeftIndentUnits, xmlReader.ValueAsInt);
                        break;
                    case "startChars": // iso29500
                        {
                            complianceInfo.MarkAsIsoTransitional();
                        paraPr.SetAttr(ParaAttr.LeftIndentUnits, xmlReader.ValueAsInt);
                        break;
                        }
                    case "rightChars":
                    case "right-chars": // WML
                        paraPr.SetAttr(ParaAttr.RightIndentUnits, xmlReader.ValueAsInt);
                        break;
                    case "endChars": // iso29500
                        {
                            complianceInfo.MarkAsIsoTransitional();
                        paraPr.SetAttr(ParaAttr.RightIndentUnits, xmlReader.ValueAsInt);
                        break;
                        }
                    case "firstLineChars":
                    case "first-line-chars":    // WML
                        paraPr.SetAttr(ParaAttr.FirstLineIndentUnits, xmlReader.ValueAsInt);
                        break;
                    case "hangingChars":
                    case "hanging-chars":   // WML
                        paraPr.SetAttr(ParaAttr.FirstLineIndentUnits, -xmlReader.ValueAsInt);
                        break;
                    default:
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Nrx, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadSpacing(NrxXmlReader xmlReader, ParaPr paraPr, OoxmlComplianceInfo complianceInfo)
        {
            LineSpacing lineSpacing = LineSpacing.CreateDefault();
            bool hasLineSpacingValue = false;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "before":
                        SetAttrIfPositive(paraPr, ParaAttr.SpaceBefore, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "beforeLines":
                    case "before-lines":    // WML
                        paraPr.SetAttr(ParaAttr.SpaceBeforeUnits, xmlReader.ValueAsInt);
                        break;
                    case "beforeAutospacing":
                    case "before-autospacing":  // WML
                        if(NrxXmlUtil.IsOnOffValue(xmlReader.Value))
                            paraPr.SetAttr(ParaAttr.SpaceBeforeAuto, xmlReader.ValueAsBool);
                        else
                            xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx,
                                        string.Format(WarningStrings.InvalidOnOffValue, xmlReader.Value, xmlReader.LocalName));
                        break;
                    case "after":
                        SetAttrIfPositive(paraPr, ParaAttr.SpaceAfter, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "afterLines":
                    case "after-lines": // WML
                        paraPr.SetAttr(ParaAttr.SpaceAfterUnits, xmlReader.ValueAsInt);
                        break;
                    case "afterAutospacing":
                    case "after-autospacing":   // WML
                        paraPr.SetAttr(ParaAttr.SpaceAfterAuto, xmlReader.ValueAsBool);
                        break;
                    case "line":
                        // andrnosk: WORDSNET-7118 According to spec (17.18.81 ST_SignedTwipsMeasure) spacing value can be positive or negative,
                        // but MS Word says LineSpacing value must be between 0pt and 1584pt.
                        // MS Word preserves negative line spacing during open/save to DOCX, 
                        // but after saving to DOC and then back to DOCX, MS Word just converts negative value to absolute value
                        // We should do the same. 
                        lineSpacing.Value = xmlReader.GetValueAsTwips(complianceInfo);
                        hasLineSpacingValue = true;
                        break;
                    case "lineRule":
                    case "line-rule":   // WML
                        lineSpacing.Rule = NrxParaEnum.XmlToLineSpacingRule(xmlReader.Value);
                        break;
                    default:
                        break;
                }
            }

            if (hasLineSpacingValue)
            {
                Validate(lineSpacing);
                paraPr[ParaAttr.LineSpacing] = lineSpacing;
            }
        }

        /// <summary>
        /// Validates specified LineSpacing.
        /// </summary>
        private static void Validate(LineSpacing lineSpacing)
        {
            // WORDSNET-19392 Seems that value should be cast to short.
            // AM. Maybe we need to move validation to LineSpacing class.
            lineSpacing.Value = (short)lineSpacing.Value;

            // WORDSNET-10830 Negative line spacing value means that rule is Exactly.
            if (lineSpacing.Value < 0)
            {
                lineSpacing.Value = System.Math.Abs(lineSpacing.Value);
                lineSpacing.Rule = LineSpacingRule.Exactly;
            }

            // WORDSNET-13091 If line spacing rule is Exactly and value is zero, MS Word interprets the rule as At least.
            if ((lineSpacing.Rule == LineSpacingRule.Exactly) && (lineSpacing.Value == 0))
                lineSpacing.Rule = LineSpacingRule.AtLeast;
        }

        /// <summary>
        /// andrnosk: WORDSNET-5722 According to spec (22.9.2.14 ST_TwipsMeasure) spacing value cannot be negative.
        /// If negative value occurred then ignore.
        /// </summary>
        /// <remarks>
        /// MS Word preserves spacing before/after value="-1" during open/save to DOCX, 
        /// but after saving to DOC and then back to DOCX, MS Word just ignore this attributes, 
        /// and does not write anything. We should do the same. 
        /// It seems in this case "-1" means for MS Word the same as if value is not specified, 
        /// so we just do not read negative value into the model.
        /// </remarks>
        private static void SetAttrIfPositive(ParaPr paraPr, int key, int value)
        {
            if (value >= 0)
                paraPr.SetAttr(key, value);
        }

        private static void ReadTabs(NrxXmlReader xmlReader, ParaPr paraPr, OoxmlComplianceInfo complianceInfo)
        {
            TabStopCollection tabStops = new TabStopCollection();

            while (xmlReader.ReadChild("tabs"))
            {
                switch (xmlReader.LocalName)
                {
                    case "tab":
                        // WORDSNET-21091 If 'val' attr is omitted, Word treats it as Left, so default is Left.
                        TabAlignment val = TabAlignment.Left;
                        TabLeader leader = TabLeader.None;
                        int pos = 0;

                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "val":
                                    val = NrxParaEnum.XmlToTabAlignment(xmlReader.Value, complianceInfo);
                                    break;
                                case "leader":
                                    leader = NrxParaEnum.XmlToTabLeader(xmlReader.Value);
                                    break;
                                case "pos":
                                    // WORDSNET-10435 Word truncates tab positions on import to fall into the valid
                                    // range: takes only the lower 16 bits of the number and treat them as a signed 
                                    // 16-bit integer (the type short in C#).
                                    pos = xmlReader.GetValueAsTwipsAsShort(complianceInfo);
                                    break;
                                default:
                                    break;
                            }
                        }

                        tabStops.Add(new TabStop(pos, val, leader));
                        break;
                    default:
                        break;
                }
            }

            if (tabStops.Count > 0)
                paraPr.SetAttr(ParaAttr.TabStops, tabStops);
        }

        private static void ReadBorders(NrxXmlReader xmlReader, ParaPr paraPr)
        {
            while (xmlReader.ReadChild("pBdr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        paraPr.SetAttr(ParaAttr.BorderTop, xmlReader.ReadBorder());
                        break;
                    case "left":
                        paraPr.SetAttr(ParaAttr.BorderLeft, xmlReader.ReadBorder());
                        break;
                    case "bottom":
                        paraPr.SetAttr(ParaAttr.BorderBottom, xmlReader.ReadBorder());
                        break;
                    case "right":
                        paraPr.SetAttr(ParaAttr.BorderRight, xmlReader.ReadBorder());
                        break;
                    case "between":
                        paraPr.SetAttr(ParaAttr.BorderBetween, xmlReader.ReadBorder());
                        break;
                    case "bar":
                        paraPr.SetAttr(ParaAttr.BorderBar, xmlReader.ReadBorder());
                        break;
                    default:
                        break;
                }
            }
        }

        internal void ReadNumPr(NrxXmlReader xmlReader, ParaPr paraPr)
        {
            string elemName = xmlReader.LocalName;  // numPr in DOCX, listPr in WML.
            while (xmlReader.ReadChild(elemName))   
            {
                switch (xmlReader.LocalName)
                {
                    case "ilvl":
                        int intVal = xmlReader.ReadIntVal();

                        // andrnosk: WORDSNET-7650 If value is negative MS Word2007/2010 recalculate this value in the following way.
                        if (intVal < 0)
                            intVal = 256 + intVal;

                        paraPr.SetAttr(ParaAttr.ListLevel, intVal);
                        break;
                    case "numId":
                    case "ilfo":    // WML
                        paraPr.SetAttr(ParaAttr.ListId, xmlReader.ReadIntVal());
                        break;
                    case "t":  // WML
                        xmlReader.IgnoreElement(WarningType.MinorFormattingLoss, WarningSource.WordML,
                            "Import of 't' element within 'listPr' is not supported in WordML format by Aspose.Words.");
                        break;
                    case "font":  // WML
                        xmlReader.IgnoreElement(WarningType.MinorFormattingLoss, WarningSource.WordML,
                            "Import of 'font' element within 'listPr' is not supported in WordML format by Aspose.Words.");
                        break;
                    default:
                        if (!ReadFormatSpecificNumPr(xmlReader.LocalName, xmlReader, paraPr))
                            xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        protected abstract bool ReadFormatSpecificNumPr(string localName, NrxXmlReader xmlReader, ParaPr paraPr);

        private readonly NrxRunPrReaderBase mRunPrReader;
        private readonly NrxSectPrReaderBase mSectPrReader;
    }
}
