// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2012 by Denis Darkin

using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Styles;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Class container to collect table/row properties.
    /// </summary>
    /// <remarks>>Extracted from <see cref="NrxRowPrWriter"/> during refactoring. Main goal to reduce complexity of static Write method,
    /// which was a mega method several screens long.</remarks>
    internal class NrxRowPrAttrContainer
    {
        internal NrxRowPrAttrContainer(TablePr tablePr, bool isStyle, INrxWriterContext context)
        {
            IsDocx = context.IsDocx;
            Compliance = context.Compliance;

            // Collect existing attributes.
            for (int i = 0; i < tablePr.Count; i++)
            {
                int key = tablePr.GetKey(i);
                object value = tablePr.GetByIndex(i);
                switch (key)
                {
                    case TableAttr.Istd:
                        if ((int)value != 0)
                            styleId = context.GetStyleId((int)value);
                        break;
                    case TableAttr.HtmlBlockId:
                        htmlBlockId = FormatterPal.IntToXml((int)value);
                        TrPrCount++;
                        break;
                    case TableAttr.StyleRowBandSize:
                        styleRowBandSize = FormatterPal.IntToXml((int)value);
                        break;
                    case TableAttr.StyleColBandSize:
                        styleColBandSize = FormatterPal.IntToXml((int)value);
                        break;
                    case TableAttr.FrameDistanceFromTop:
                        wDistanceFromTop = FormatterPal.IntToXml((int)value);
                        break;
                    case TableAttr.FrameDistanceFromLeft:
                        wDistanceFromLeft = FormatterPal.IntToXml((int)value);
                        break;
                    case TableAttr.FrameDistanceFromBottom:
                        wDistanceFromBottom = FormatterPal.IntToXml((int)value);
                        break;
                    case TableAttr.FrameDistanceFromRight:
                        wDistanceFromRight = FormatterPal.IntToXml((int)value);
                        break;
                    case TableAttr.RelativeHorizontalPosition:
                    {
                        RelativeHorizontalPosition relativeHorizontalPosition = (RelativeHorizontalPosition)value;
                        if (relativeHorizontalPosition != RelativeHorizontalPosition.Default)
                            wHorzAnchor = StyleConvertUtil.RelativeHorizontalPositionToXml(relativeHorizontalPosition);
                        break;
                    }
                    case TableAttr.RelativeVerticalPosition:
                    {
                        RelativeVerticalPosition relativeVerticalPosition = (RelativeVerticalPosition)value;
                        if (relativeVerticalPosition != RelativeVerticalPosition.TableDefault)
                            wVertAnchor = StyleConvertUtil.RelativeVerticalPositionToXml(relativeVerticalPosition);
                        break;
                    }
                    case TableAttr.HorizontalAlignment:
                    {
                        // WORDSNET-6139 See "part 1 reference c059575_ISO_IEC_29500-1_2011 22.9.2.18 ST_XAlign (Horizontal Alignment Location)".
                        // Acceptable values: center, inside, left, outside, right
                        HorizontalAlignment ha = (HorizontalAlignment)value;
                        if (ha != HorizontalAlignment.None)
                            wXSpec = StyleConvertUtil.HorizontalAlignmentToXml(ha);
                        break;
                    }
                    case TableAttr.VerticalAlignment:
                    {
                        // WORDSNET-6139 See "part 1 reference c059575_ISO_IEC_29500-1_2011 22.9.2.20 ST_YAlign (Vertical Alignment Location)".
                        // Acceptable values: bottom, center, inline, inside, outside, top.
                        VerticalAlignment va = (VerticalAlignment)value;
                        if (va != VerticalAlignment.None)
                            wYSpec = StyleConvertUtil.VerticalAlignmentToXml(va);
                        break;
                    }
                    case TableAttr.FrameLeft:
                        // WORDSNET-3623 See test remarks.
                        wX = FormatterPal.IntToXml((int)value + 1);
                        break;
                    case TableAttr.FrameTop:
                        // WORDSNET-3623 See test remarks.
                        wY = FormatterPal.IntToXml((int)value + 1);
                        break;
                    case TableAttr.AllowOverlap:
                        wAllowOverlap = (bool)value;
                        break;
                    case TableAttr.Bidi:
                        wBidi = (bool)value;
                        break;
                    case TableAttr.PreferredWidth:
                        wPrefferedWidth = (PreferredWidth)value;
                        if (IsDocx) // todo 0 rk check if this should be applied to WML too.
                            TblPrExCount++;
                        break;
                    case TableAttr.Alignment:
                        wAlignment = NrxTableEnum.TableAlignmentToXml((TableAlignment)value, IsDocx,
                            context.Compliance);
                        TrPrCount++;
                        TblPrExCount++;
                        break;
                    case TableAttr.CellSpacing:
                        wCellSpacing = (PreferredWidth)value;
                        TrPrCount++;
                        TblPrExCount++;
                        break;
                    case TableAttr.LeftIndent:
                        wLeftIndent = PreferredWidth.FromTwipsSafe((int)value);
                        TblPrExCount++;
                        break;
                    case TableAttr.BorderTop:
                        wBorderTop = (Border)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.BorderLeft:
                        wBorderLeft = (Border)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.BorderBottom:
                        wBorderBottom = (Border)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.BorderRight:
                        wBorderRight = (Border)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.BorderHorizontal:
                        wBorderHorizontal = (Border)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.BorderVertical:
                        wBorderVertical = (Border)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.AllowAutoFit:
                        // We do not write this in styles.
                        if (!isStyle)
                        {
                            AllowAutoFit = value;
                            TblPrExCount++;
                        }
                        break;
                    case TableAttr.TopPadding:
                        marginTop = PreferredWidth.FromTwipsSafe((int)value);
                        TblPrExCount++;
                        break;
                    case TableAttr.LeftPadding:
                        marginLeft = PreferredWidth.FromTwipsSafe((int)value);
                        TblPrExCount++;
                        break;
                    case TableAttr.BottomPadding:
                        marginBottom = PreferredWidth.FromTwipsSafe((int)value);
                        TblPrExCount++;
                        break;
                    case TableAttr.RightPadding:
                        marginRight = PreferredWidth.FromTwipsSafe((int)value);
                        TblPrExCount++;
                        break;
                    case TableAttr.Shading:
                        wShading = (Shading)value;
                        TblPrExCount++;
                        break;
                    case TableAttr.StyleOptions:
                        wStyleLook = FormatterPal.IntToStrX4(NrxTableUtil.TableStyleOptionsToNrx((TableStyleOptions)value));
                        TblPrExCount++;
                        break;
                    case TableAttr.RowHeight:
                    {
                        TrPrCount++;
                        wRowHeight = FormatterPal.IntToXml(tablePr.Height);
                        HeightRule heightRule = tablePr.HeightRule;
                        if (heightRule != HeightRule.AtLeast)
                            wRowHeightRule = NrxTableEnum.HeightRuleToXml(heightRule, IsDocx);
                        break;
                    }
                    case TableAttr.HeadingFormat:
                    {
                        wHeader = (bool)value;
                        TrPrCount++;
                        break;
                    }
                    case TableAttr.AllowBreakAcrossPages:
                    {
                        AllowBreakAcrossPages = (bool)value;
                        TrPrCount++;
                        break;
                    }
                    case TableAttr.RsidTr:
                        rsidTr = NrxXmlUtil.IntToHex((int)value);
                        break;
                    case TableAttr.Title:
                        wTblCaption = (string)value;
                        break;
                    case TableAttr.Description:
                        wTblDescription = (string)value;
                        break;
                    case TableAttr.Hidden:
                    {
                        // WordML doesn't support this attribute and Word fails on open.
                        if (context.IsDocx)
                        {
                            bool isHidden = (bool)value;
                            Hidden = isHidden;
                            // Do not write 'w:trPr' if the value is false.
                            if (isHidden)
                                TrPrCount++;
                        }
                        break;
                    }
                    case TableAttr.Sys_GridBefore:
                        GridBefore = (int) value;
                        if (GridBefore > 0)
                            TrPrCount++;
                        break;
                    case TableAttr.Sys_GridAfter:
                        GridAfter = (int)value;
                        if (GridAfter > 0)
                            TrPrCount++;
                        break;
                    default:
                        break;
                }
            }

            WidthBefore = (PreferredWidth)tablePr.GetDirectAttr(TableAttr.WidthBefore);
            WidthAfter = (PreferredWidth)tablePr.GetDirectAttr(TableAttr.WidthAfter);

            // Use the original width before and width after value if the new table grid was not calculated successfully.
            // Fixes scenarios like Test22709 (NrxReader uses some naive technique to assign incorrect widthBefore).
            if (!TableGridAttr.IsApplied(tablePr.CalculatedGrid))
            {
                PreferredWidth originalValue = (PreferredWidth)tablePr.GetDirectAttr(TableAttr.WidthBeforeOriginal);
                if (PreferredWidth.HasPositiveValue(originalValue))
                    WidthBefore = originalValue;

                // Test19759 shows a conversion scenario where new table layout logic is not invoked.
                // The original width after in percent units is lost without this line.
                originalValue = (PreferredWidth)tablePr.GetDirectAttr(TableAttr.WidthAfterOriginal);
                if (PreferredWidth.HasPositiveValue(originalValue))
                    WidthAfter = originalValue;
            }

            if (PreferredWidth.HasPositiveValue(WidthBefore))
                TrPrCount++;
            if (PreferredWidth.HasPositiveValue(WidthAfter))
                TrPrCount++;

            // RK This is handling of an attribute whose default is different between model and DOCX/WML.
            // In the model we have one default value, but DOCX/WML has a different default value.
            // Hence if the attribute is missing - we have to write the default.
            if (!isStyle)
            {
                // If the attribute was not encountered in the loop above, this code below gets the default value from the model.
                 if (AllowAutoFit == null)
                {
                     AllowAutoFit = tablePr.AllowAutoFit;
                    TblPrExCount++;
                }

                // But we do not want to write the value if it is true.
                 if ((bool)AllowAutoFit)
                    TblPrExCount--;
            }
        }

        internal readonly string styleId;
        internal readonly string htmlBlockId;

        internal readonly string styleRowBandSize;
        internal readonly string styleColBandSize;

        internal readonly string wDistanceFromTop;
        internal readonly string wDistanceFromLeft;
        internal readonly string wDistanceFromBottom;
        internal readonly string wDistanceFromRight;

        internal readonly string wVertAnchor;
        internal readonly string wHorzAnchor;
        internal readonly string wXSpec;
        internal readonly string wX;
        internal readonly string wYSpec;
        internal readonly string wY;
        internal readonly bool wAllowOverlap = true;

        internal readonly bool wBidi;
        internal readonly PreferredWidth wPrefferedWidth;
        internal readonly string wAlignment;
        internal readonly PreferredWidth wCellSpacing;
        internal readonly PreferredWidth wLeftIndent;

        internal readonly Border wBorderBottom;
        internal readonly Border wBorderHorizontal;
        internal readonly Border wBorderLeft;
        internal readonly Border wBorderRight;
        internal readonly Border wBorderTop;
        internal readonly Border wBorderVertical;
        internal readonly Shading wShading;

        internal readonly PreferredWidth marginTop;
        internal readonly PreferredWidth marginLeft;
        internal readonly PreferredWidth marginBottom;
        internal readonly PreferredWidth marginRight;

        internal readonly string wStyleLook;
        internal readonly string wRowHeight;
        internal readonly string wRowHeightRule;
        internal readonly object wHeader;
        internal readonly object AllowBreakAcrossPages;
        internal readonly object AllowAutoFit;

        internal readonly bool Hidden;

        internal readonly string wTblCaption;
        internal readonly string wTblDescription;

        internal readonly string rsidTr;
        internal readonly string rsidR = null;

        internal readonly int TrPrCount;
        internal readonly int TblPrExCount;

        internal readonly int GridBefore;
        internal readonly int GridAfter;

        internal readonly PreferredWidth WidthBefore;
        internal readonly PreferredWidth WidthAfter;

        internal readonly bool IsDocx;
        internal readonly OoxmlComplianceCore Compliance;
    }
}
