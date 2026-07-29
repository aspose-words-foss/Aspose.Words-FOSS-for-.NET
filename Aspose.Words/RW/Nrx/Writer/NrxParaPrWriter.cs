// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2010 by Roman Korchagin

using Aspose.Words.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Writer
{
    internal static class NrxParaPrWriter
    {
        internal static void Write(ParaPr paraPr, INrxWriterContext writer)
        {
             if (WriteStart(paraPr, writer))
                writer.Builder.EndElement();    //w:pPr
        }

        /// <summary>
        /// Writes paragraph formatting properties. NOTE does not close the pPr element!
        /// If return value is true, you need to close the pPr element.
        /// </summary>
        /// <returns>True if at least one paragraph property was written.</returns>
        internal static bool WriteStart(ParaPr paraPr, INrxWriterContext writer)
        {
            if ((paraPr == null) || (paraPr.Count == 0))
                return false;

            NrxXmlBuilder builder = writer.Builder;

            if (paraPr.FormatRevision != null)
            {
                // "w:pPr" is written because format revision is present in paragraph properties.
                builder.StartElement("w:pPr");

                // Calculate and write the AfterChanges attribute collection.
                WordAttrCollection afterChanges = paraPr.Clone();
                afterChanges.AcceptFormatRevision();
                WriteProps(afterChanges, false, writer, false);

                // Write the BeforeChanges attribute collection.
                builder.WriteRevisionStart(paraPr.FormatRevision, "w:pPrChange", writer.GetNextAnnotationId());
                builder.StartElement("w:pPr");
                WriteProps(paraPr, false, writer, true);
                builder.EndElement("w:pPr");
                builder.WriteRevisionEnd(); //w:pPrChange

                // If format revision is present in paragraph properties then for sure "w:pPr" was written.
                return true;
            }
            else
            {
                // This is normal course, no revisions.
                return WriteProps(paraPr, true, writer, false);
            }
        }

        private static bool WriteProps(WordAttrCollection attrs, bool doWriteParaPrStart, INrxWriterContext writer, bool isFormattingRevision)
        {
            // WORDSNET-27683 The customer asked not to write a framePr element, if it can be omitted, i.e. if a frame is
            // not applied.
            ClearFramePropertiesIfNotFramed(attrs);

            bool isDocx = writer.IsDocx;
            bool isIsoStrict = isDocx && (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            NrxXmlBuilder builder = writer.Builder;

            // Use counter to determine if there are any properties that need to be written in w:pPr declaration.
            // That is used to avoid writing empty w:pPr declarations.
            // Checking paraPr.Count does not help here because there can be attributes that should not be written in WordML.
            int writableAttributesCounter = 0;

            string styleId = null;
            object keepNext = null;
            object keepLines = null;
            object pageBreakBefore = null;

            string frameDropCap = null;
            object frameLines = null;
            object frameW = null;
            object frameH = null;
            object frameVSpace = null;
            object frameHSpace = null;
            string frameWrap = null;
            string frameHAnchor = null;
            string frameVAnchor = null;
            object frameX = null;
            string frameXAlign = null;
            object frameY = null;
            string frameYAlign = null;
            string frameHRule = null;
            object frameAnchorLock = null;

            object widowControl = null;

            object listId = null;
            object listLevel = null;

            object supressLineNumbers = null;

            Border bdrTop = null;
            Border bdrLeft = null;
            Border bdrBottom = null;
            Border bdrRight = null;
            Border bdrBetween = null;
            Border bdrBar = null;

            Shading shading = null;

            TabStopCollection tabStops = null;

            object suppressAutoHyphens = null;
            object kinsoku = null;
            object wordWrap = null;
            object overflowPunct = null;
            object topLinePunct = null;
            object autoSpaceDE = null;
            object autoSpaceDN = null;
            object bidi = null;
            object adjustRightInd = null;
            object snapToGrid = null;

            object spacingBefore = null;
            object spacingBeforeLines = null;
            object spacingBeforeAutospacing = null;
            object spacingAfter = null;
            object spacingAfterLines = null;
            object spacingAfterAutospacing = null;
            object spacingLine = null;
            string spacingLineRule = null;

            object mirrorIndents = null;
            object indLeft = null;
            object indRight = null;
            object indHanging = null;
            object indFirstLine = null;

            object indLeftChars = null;
            object indRightChars = null;
            object indHangingChars = null;
            object indFirstLineChars = null;

            object contextualSpacing = null;
            object suppressOverlap = null;
            string jc = null;
            string textDirection = null;
            string textAlignment = null;
            object outlineLvl = null;

            object divId = null;
            object tightWrap = null;
            object collapsed = null;

            ParagraphNumberRevision numberRevision = null;

            // This is the main loop to collect the properties.
            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);
                writableAttributesCounter++;

                switch (key)
                {
                    case ParaAttr.Istd:
                    {
                        int istd = (int)value;
                        //pStyle
                        if ((istd != 0) && WordUtil.IsValidIstd(istd))
                        {
                            styleId = writer.GetStyleId(istd);
                        }
                        else
                        {
                            // We need to set it explicitly here to combine paraPr revisions correctly.
                            styleId = null;
                            writableAttributesCounter--;
                        }

                        if (styleId != null)
                        {
                            Style style = writer.Document.Styles.GetByIstd(istd, false);

                            // List style references are not written.
                            // Also style references are not written inside numbering and style definitions.
                            if ((style.Type == StyleType.List) || builder.IsStylesBuilder)
                            {
                                styleId = null;
                                writableAttributesCounter--;
                            }
                        }
                        break;
                    }
                    case ParaAttr.TextboxTightWrap:
                        tightWrap = NrxParaEnum.TextboxTightWrapToXml((TextboxTightWrap) value);
                        break;
                    case ParaAttr.HtmlBlockId:
                        divId = value;
                        break;
                    case ParaAttr.KeepWithNext:
                        keepNext = value;
                        break;
                    case ParaAttr.KeepTogether:
                        keepLines = value;
                        break;
                    case ParaAttr.PageBreakBefore:
                        pageBreakBefore = value;
                        break;
                    case ParaAttr.DropCapPosition:
                        frameDropCap = NrxParaEnum.DropCapPositionToXml((DropCapPosition)value);
                        break;
                    case ParaAttr.DropCapLinesToDrop:
                        frameLines = value;
                        break;
                    case ParaAttr.FrameWidth:
                        frameW = value;
                        break;
                    case ParaAttr.FrameHorizontalDistanceFromText:
                        frameHSpace = value;
                        break;
                    case ParaAttr.FrameVerticalDistanceFromText:
                        frameVSpace = value;
                        break;
                    case ParaAttr.FrameWrapType:
                        frameWrap = NrxParaEnum.TextFrameWrapTypeToXml((WrapType)value, isDocx);
                        break;
                    case ParaAttr.FrameRelativeHorizontalPosition:
                        frameHAnchor = NrxParaEnum.RelativeHorizontalPositionToXml((RelativeHorizontalPosition)value);
                        break;
                    case ParaAttr.FrameRelativeVerticalPosition:
                        frameVAnchor = NrxParaEnum.RelativeVerticalPositionToXml((RelativeVerticalPosition)value);
                        break;
                    case ParaAttr.FrameLeft:
                        frameX = ((value != null) && ((int)value == 0)) ? null : value;
                        break;
                    case ParaAttr.FrameHorizontalAlignment:
                        frameXAlign = NrxParaEnum.HorizontalAlignmentToXml((HorizontalAlignment)value);
                        break;
                    case ParaAttr.FrameTop:
                        frameY = ((value != null) && ((int)value == 0)) ? null : value;
                        break;
                    case ParaAttr.FrameVerticalAlignment:
                        frameYAlign = NrxParaEnum.VerticalAlignmentToXml((VerticalAlignment)value);
                        break;
                    case ParaAttr.FrameHeight:
                        Height height = (Height) value;
                        frameHRule = NrxParaEnum.HeightRuleToXml(height.Rule, isDocx);
                        frameH = height.Value;
                        break;
                    case ParaAttr.FrameLockAnchor:
                        frameAnchorLock = value;
                        break;
                    case ParaAttr.WidowControl:
                        widowControl = value;
                        break;
                    case ParaAttr.ListId:
                        listId = value;
                        break;
                    case ParaAttr.ListLevel:
                        listLevel = value;
                        break;
                    case ParaAttr.SuppressLineNumbers:
                        supressLineNumbers = value;
                        break;
                    case ParaAttr.BorderBar:
                        bdrBar = (Border)value;
                        if (bdrBar.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.BorderBetween:
                        bdrBetween = (Border)value;
                        if (bdrBetween.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.BorderBottom:
                        bdrBottom = (Border)value;
                        if (bdrBottom.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.BorderLeft:
                        bdrLeft = (Border)value;
                        if (bdrLeft.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.BorderRight:
                        bdrRight = (Border)value;
                        if (bdrRight.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.BorderTop:
                        bdrTop = (Border)value;
                        if (bdrTop.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.Shading:
                        shading = (Shading)value;
                        if (shading.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case ParaAttr.TabStops:
                        tabStops = (TabStopCollection)value;
                        break;
                    case ParaAttr.SuppressAutoHyphens:
                        suppressAutoHyphens = value;
                        break;
                    case ParaAttr.FarEastLineBreakControl:
                        kinsoku = value;
                        break;
                    case ParaAttr.WordWrap:
                        wordWrap = value;
                        break;
                    case ParaAttr.HangingPunctuation:
                        overflowPunct = value;
                        break;
                    case ParaAttr.TopLinePunctuation:
                        topLinePunct = value;
                        break;
                    case ParaAttr.AddSpaceBetweenFarEastAndAlpha:
                        autoSpaceDE = value;
                        break;
                    case ParaAttr.AddSpaceBetweenFarEastAndDigit:
                        autoSpaceDN = value;
                        break;
                    case ParaAttr.Bidi:
                        bidi = value;
                        break;
                    case ParaAttr.AutoAdjustRightIndent:
                        adjustRightInd = value;
                        break;
                    case ParaAttr.SnapToGrid:
                        snapToGrid = value;
                        break;
                    case ParaAttr.SpaceAfter:
                        spacingAfter = value;
                        break;
                    case ParaAttr.SpaceAfterUnits:
                        spacingAfterLines = value;
                        break;
                    case ParaAttr.SpaceAfterAuto:
                        spacingAfterAutospacing = value;
                        break;
                    case ParaAttr.SpaceBefore:
                        spacingBefore = value;
                        break;
                    case ParaAttr.SpaceBeforeUnits:
                        spacingBeforeLines = value;
                        break;
                    case ParaAttr.SpaceBeforeAuto:
                        spacingBeforeAutospacing = value;
                        break;
                    case ParaAttr.LineSpacing:
                    {
                        LineSpacing lineSpacing = (LineSpacing)value;

                        spacingLine = lineSpacing.Value;
                        spacingLineRule = NrxParaEnum.LineSpacingRuleToXml(lineSpacing.Rule, isDocx);
                        break;
                    }
                    case ParaAttr.MirrorIndents:
                        mirrorIndents = value;
                        break;
                    case ParaAttr.FirstLineIndent:
                        // if this attribute is negative then it shall be written as "w:hanging",
                        // otherwise as "w:first-line"
                        if ((int)value < 0)
                            indHanging = -(int)value;
                        else
                            indFirstLine = value;
                        break;
                    case ParaAttr.LeftIndent:
                        indLeft = value;
                        break;
                    case ParaAttr.RightIndent:
                        indRight = value;
                        break;
                    case ParaAttr.FirstLineIndentUnits:
                        // if this attribute is negative then it shall be written as "w:hanging",
                        // otherwise as "w:first-line"
                        if ((int)value < 0)
                            indHangingChars = -(int)value;
                        else
                            indFirstLineChars = value;
                        break;
                    case ParaAttr.LeftIndentUnits:
                        indLeftChars = value;
                        break;
                    case ParaAttr.RightIndentUnits:
                        indRightChars = value;
                        break;
                    case ParaAttr.NoSpaceBetweenSameStyle:
                        contextualSpacing = value;
                        break;
                    case ParaAttr.FrameSuppressOverlap:
                        suppressOverlap = value;
                        break;
                    case ParaAttr.Alignment:
                        jc = NrxParaEnum.ParagraphAlignmentToXml((ParagraphAlignment)value, isDocx, writer.Compliance);
                        break;
                    case ParaAttr.FrameTextOrientation:
                        textDirection = StyleConvertUtil.TextOrientationToXml((TextOrientation)value, isDocx, 
                            writer.Compliance);
                        break;
                    case ParaAttr.BaselineAlignment:
                        textAlignment = NrxParaEnum.BaselineAlignmentToXml((BaselineAlignment)value);
                        break;
                    case ParaAttr.OutlineLevel:
                        outlineLvl = (int)value;
                        break;
                    case ParaAttr.Collapsed:
                        collapsed = (bool)value;
                        break;
                    case RevisionAttr.NumberRevision:
                    {
                        // Write number revision only if we are not writing a formatting revision right now.
                        if (!isFormattingRevision)
                        {
                            numberRevision = (ParagraphNumberRevision)value;
                            if (!numberRevision.IsActive)
                            {
                                numberRevision = null;
                                writableAttributesCounter--;
                            }
                        }
                        break;
                    }
                    case RevisionAttr.FormatRevision:
                        // This is written elsewhere.
                        writableAttributesCounter--;
                        break;
                    default:
                        writableAttributesCounter--;
                        break;
                }
            }

            if (writableAttributesCounter == 0)
                return false;

            if (doWriteParaPrStart)
                builder.StartElement("w:pPr");

            builder.WriteVal("w:pStyle", styleId);
            builder.WriteVal("w:textboxTightWrap", tightWrap);
            builder.WriteVal("w:divId", divId);
            builder.WriteVal("w:keepNext", keepNext);
            builder.WriteVal("w:keepLines", keepLines);
            builder.WriteVal("w:pageBreakBefore", pageBreakBefore);

            builder.WriteElementWithAttributes(
                "w:framePr",
                isDocx ? "w:dropCap" : "w:drop-cap", frameDropCap,
                "w:lines", frameLines,
                "w:w", frameW,
                "w:h", frameH,
                isDocx ? "w:hRule" : "w:h-rule", frameHRule,
                isDocx ? "w:hSpace" : "w:hspace", frameHSpace,
                isDocx ? "w:vSpace" : "w:vspace", frameVSpace,
                "w:wrap", frameWrap,
                isDocx ? "w:vAnchor" : "w:vanchor", frameVAnchor,
                isDocx ? "w:hAnchor" : "w:hanchor", frameHAnchor,
                "w:x", frameX,
                isDocx ? "w:xAlign" : "w:x-align", frameXAlign,
                "w:y", frameY,
                isDocx ? "w:yAlign" : "w:y-align", frameYAlign,
                isDocx ? "w:anchorLock" : "w:anchor-lock", frameAnchorLock);

            builder.WriteVal("w:widowControl", widowControl);

            bool hasListLevel = (listLevel != null);
            bool hasListId = (listId != null);
            // AS ListLabelString and ListLabelFontName are not used for DOCX as list properties.
            string listLabelString = isDocx ? null : writer.CurrentParagraphListLabelString;
            string listLabelFontName = isDocx ? null : writer.CurrentParagraphListLabelFontName;
            bool hasNumberRevision = (numberRevision != null) && (numberRevision.IsActive);

            // We write w:listPr element if we are going to write at least one of these.
            // AS The case is possible when (hasListId == false) but (listLabelString != null) or (listLabelFontName != null)
            // if the paragraph inherits list item property.
            if (hasListLevel || hasListId || hasNumberRevision || (listLabelString != null) || (listLabelFontName != null))
            {
                builder.StartElement(isDocx ? "w:numPr" : "w:listPr");

                if (hasListLevel)
                    builder.WriteVal("w:ilvl", listLevel);

                if (hasListId)
                    builder.WriteVal(isDocx ? "w:numId" : "w:ilfo", listId);

                if (listLabelString != null)
                {
                    builder.WriteVal("wx:t", listLabelString);
                }

                if (listLabelFontName != null)
                {
                    builder.WriteVal("wx:font", listLabelFontName);
                }

                if (hasNumberRevision)
                    builder.WriteRevision(numberRevision, writer.GetNextAnnotationId());

                builder.EndElement(); //w:listPr
            }

            // Funny that in WordML it was called "supressLineNumbers" (with one p).
            // "Suppress" is actually the right spelling for this word.
            builder.WriteVal(isDocx ? "w:suppressLineNumbers" : "w:supressLineNumbers", supressLineNumbers);

            builder.WriteBorders(
                "w:pBdr",
                "w:top", bdrTop,
                "w:left", bdrLeft,
                "w:bottom", bdrBottom,
                "w:right", bdrRight,
                "w:between", bdrBetween,
                "w:bar", bdrBar);

            builder.WriteShd(shading);

            WriteTabs(tabStops, writer);

            builder.WriteVal("w:suppressAutoHyphens", suppressAutoHyphens);
            builder.WriteVal("w:kinsoku", kinsoku);
            builder.WriteVal("w:wordWrap", wordWrap);
            builder.WriteVal("w:overflowPunct", overflowPunct);
            builder.WriteVal("w:topLinePunct", topLinePunct);
            builder.WriteVal("w:autoSpaceDE", autoSpaceDE);
            builder.WriteVal("w:autoSpaceDN", autoSpaceDN);
            builder.WriteVal("w:bidi", bidi);
            builder.WriteVal("w:adjustRightInd", adjustRightInd);
            builder.WriteVal("w:snapToGrid", snapToGrid);

            // Word writes either "w:before", "w:after" or "w:before-lines", "w:after-lines" attributes.
            // I don't know how to decide which one should be written, so I will always write "w:before", "w:after".
            builder.WriteElementWithAttributes(
                "w:spacing",
                "w:before", spacingBefore,
                isDocx ? "w:beforeLines" : "w:before-lines", spacingBeforeLines,
                isDocx ? "w:beforeAutospacing" : "w:before-autospacing", spacingBeforeAutospacing,
                "w:after", spacingAfter,
                isDocx ? "w:afterLines" : "w:after-lines", spacingAfterLines,
                isDocx ? "w:afterAutospacing" : "w:after-autospacing", spacingAfterAutospacing,
                "w:line", spacingLine,
                isDocx ? "w:lineRule" : "w:line-rule", spacingLineRule);

            //Word writes either "w:left", "w:right", "w:hanging", "w:first-line" or "w:left-chars", "w:right-chars", "w:hanging-chars", "w:first-line-chars" attributes.
            //I don't know how to decide which one should be written, so I will always write "w:left", "w:right", "w:hanging", "w:first-line".
            builder.WriteElementWithAttributes(
                "w:ind",
                isIsoStrict ? "w:start" : "w:left", indLeft,
                isIsoStrict ? "w:end" : "w:right", indRight,
                "w:hanging", indHanging,
                isDocx ? "w:firstLine" : "w:first-line", indFirstLine,
                // line/char
                isDocx ? "w:leftChars" : "w:left-chars", indLeftChars,
                isDocx ? "w:rightChars" : "w:right-chars", indRightChars,
                isDocx ? "w:hangingChars" : "w:hanging-chars", indHangingChars,
                isDocx ? "w:firstLineChars" : "w:first-line-chars", indFirstLineChars);

            builder.WriteVal("w:contextualSpacing", contextualSpacing);
            builder.WriteVal("w:suppressOverlap", suppressOverlap);
            builder.WriteVal("w:jc", jc);
            builder.WriteVal("w:textDirection", textDirection);
            builder.WriteVal("w:textAlignment", textAlignment);
            builder.WriteVal("w:outlineLvl", outlineLvl);

            // andrnosk: MirrorIndents is not supported in WordML. 
            if (isDocx)
                builder.WriteVal("w:mirrorIndents", mirrorIndents);
            else if (mirrorIndents != null)
                writer.Warn(WarningType.MinorFormattingLoss, "MirrorIndents is not supported in WordML.");

            // Collapsed is not supported in WordML. 
            if ((collapsed != null) && (bool)collapsed && 
                isDocx && (writer.Compliance != OoxmlComplianceCore.Ecma376))
                builder.WriteEmptyElement("w15:collapsed");

            return true;
        }

        /// <summary>
        /// Clears the frame attributes, if a frame is not applied, to avoid writing a meaningless 'framePr' element.
        /// </summary>
        private static void ClearFramePropertiesIfNotFramed(WordAttrCollection attrs)
        {
            // If the ParaAttr.FrameWrapType attribute has the 'auto' value and the other frame attributes are null,
            // the frame is not applied and the ParaAttr.FrameWrapType attribute can be cleared.

            if ((attrs[ParaAttr.FrameWrapType] == null) || ((WrapType)attrs[ParaAttr.FrameWrapType] != WrapType.Inline))
                return;

            foreach (int key in gFramePrAttributesExceptWrap)
            {
                if (attrs[key] != null)
                    return;
            }

            attrs.Remove(ParaAttr.FrameWrapType);
        }

        /// <summary>
        /// Writes w:tabs element using specified TabStops.
        /// </summary>
        private static void WriteTabs(TabStopCollection tabs, INrxWriterContext writer)
        {
            if ((tabs == null) || (tabs.Count == 0))
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("w:tabs");

            for (int i = 0; i < tabs.Count; i++)
            {
                TabStop tabStop = tabs[i];
                string leader = NrxParaEnum.TabLeaderToXml(tabStop.Leader, writer.IsDocx);
                int pos = System.Math.Max(tabStop.PositionTwips, MinTab);

                builder.WriteElementWithAttributes(
                    "w:tab",
                    "w:val", NrxParaEnum.TabAlignmentToXml(tabStop.Alignment, writer.IsDocx,
                        writer.Compliance == OoxmlComplianceCore.IsoStrict),
                    // do not write w:leader if its value is 'none'
                    "w:leader", (leader != "none") ? leader : null,
                    "w:pos", pos);
            }

            builder.EndElement(); //w:tabs
        }

        private static int[] gFramePrAttributesExceptWrap = new int[]
        {
            ParaAttr.DropCapPosition, ParaAttr.DropCapLinesToDrop, ParaAttr.FrameWidth, ParaAttr.FrameHeight,
            ParaAttr.FrameHeight, ParaAttr.FrameHorizontalDistanceFromText, ParaAttr.FrameVerticalDistanceFromText,
            ParaAttr.FrameRelativeVerticalPosition, ParaAttr.FrameRelativeHorizontalPosition, ParaAttr.FrameLeft,
            ParaAttr.FrameHorizontalAlignment, ParaAttr.FrameTop, ParaAttr.FrameVerticalAlignment, ParaAttr.FrameLockAnchor
        };

        private const int MinTab = -31680;
    }
}
