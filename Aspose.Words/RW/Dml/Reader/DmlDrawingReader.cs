// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2008 by Roman Korchagin

using System.Drawing;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Reads a DrawingML drawing.
    /// </summary>
    internal class DmlDrawingReader
    {
        /// <summary>
        /// The main method to read a DrawingML object.
        /// Call this when the reader is at the beginning of a "w:drawing" element.
        /// </summary>
        internal ShapeBase ReadDrawing(DocxDocumentReaderBase reader)
        {
            mReader = reader;
            mXmlReader = reader.XmlReader;

            mShapePr = new ShapePr();
            mDrawingML = null;

            while (mXmlReader.ReadChild("drawing"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "anchor":
                        ReadDrawingCore(WrapType.None);
                        break;
                    case "inline":
                        ReadDrawingCore(WrapType.Inline);
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }

            if (mDrawingML != null)
            {
                // Fill DrawingML ShapePr from DmlNode.
                mShapePr.ExpandTo(mDrawingML.ShapePr, false);

                if (mShapePr.Contains(ShapeAttr.ZOrder))
                    mReader.AddToZOrderList(mDrawingML);
            }

            return mDrawingML;
        }

        private void ReadDrawingCore(WrapType wrapType)
        {
            mShapePr.SetAttr(ShapeAttr.WrapType, wrapType);

            // Specifies that this object shall be positioned using the positioning
            // information in the simplePos child element (§5.5.2.13).
            bool isSimplePos = false;

            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "allowOverlap":
                        // anchor
                        mShapePr.SetAttr(ShapeAttr.AllowOverlap, mXmlReader.ValueAsBool);
                        break;
                    case "behindDoc":
                        // anchor
                        mShapePr.SetAttr(ShapeAttr.BehindText, mXmlReader.ValueAsBool);
                        break;
                    case "distB":
                        // anchor
                        // inline
                        mShapePr.SetAttr(ShapeAttr.DistanceBottom, ParseDistance(mXmlReader.Value));
                        break;
                    case "distL":
                        // anchor
                        // inline
                        mShapePr.SetAttr(ShapeAttr.DistanceLeft, ParseDistance(mXmlReader.Value));
                        break;
                    case "distR":
                        // anchor
                        // inline
                        mShapePr.SetAttr(ShapeAttr.DistanceRight, ParseDistance(mXmlReader.Value));
                        break;
                    case "distT":
                        // anchor
                        // inline
                        mShapePr.SetAttr(ShapeAttr.DistanceTop, ParseDistance(mXmlReader.Value));
                        break;
                    case "hidden":
                        // The 'hidden' attribute is not read/written because MS Word ignores it, see the chapter
                        // 2.1.1345 of [MS-OI29500].
                        break;
                    case "layoutInCell":
                        // anchor
                        mShapePr.SetAttr(ShapeAttr.AllowInCell, mXmlReader.ValueAsBool);
                        break;
                    case "locked":
                        // anchor
                        mShapePr.SetAttr(ShapeAttr.AnchorLocked, mXmlReader.ValueAsBool);
                        break;
                    case "relativeHeight":
                    {
                        // anchor
                        int zIndex = mXmlReader.ValueAsInt;
                        // DrawingML z-index is based as in front of text.
                        mShapePr.SetAttr(ShapeAttr.ZOrder, ZOrderUtil.ZIndexToZOrder(zIndex, false));
                        break;
                    }
                    case "simplePos":
                        // anchor
                        isSimplePos = mXmlReader.ValueAsBool;
                        break;
                    case "editId":
                    case "anchorId":
                        // Word 2010. Ignore for now.
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }

            mXmlReader.MoveToElement();
            string tagName = mXmlReader.LocalName;
            while (mXmlReader.ReadChild(tagName))
                ReadDrawingCoreChild(isSimplePos);
        }

        /// <summary>
        /// Reads a child of an wp:inline or wp:anchor element.
        /// </summary>
        private void ReadDrawingCoreChild(bool isSimplePos)
        {
            switch (mXmlReader.LocalName)
            {
                case "cNvGraphicFramePr":
                    ReadCNvGraphicFramePr();
                    break;
                case "docPr":
                    ReadDocPr();
                    break;
                case "effectExtent":
                    ReadEffectExtent();
                    break;
                case "extent":
                {
                    Size extent = DmlBasicsReader.ReadCxCy(mXmlReader, mReader.ComplianceInfo);
                    mShapePr.SetAttr(ShapeAttr.Width, ConvertUtilCore.EmuToPoint(extent.Width));
                    mShapePr.SetAttr(ShapeAttr.Height, ConvertUtilCore.EmuToPoint(extent.Height));
                    break;
                }
                case "graphic":
                    mDrawingML = DmlGraphicsReader.Read(mReader);
                    if (mDocPrExtensions != null)
                        mDrawingML.DmlNode.DocPrExtensions = mDocPrExtensions;
                    break;
                case "positionH":
                    ReadPositionH();
                    break;
                case "positionV":
                    ReadPositionV();
                    break;
                case "simplePos":
                {
                    if (isSimplePos)
                        ReadSimplePos();
                    else
                        mXmlReader.IgnoreElementNoWarn();
                    break;
                }
                case "wrapNone":
                    ReadWrap(WrapType.None);
                    break;
                case "wrapSquare":
                    ReadWrap(WrapType.Square);
                    break;
                case "wrapThrough":
                    ReadWrap(WrapType.Through);
                    break;
                case "wrapTight":
                    ReadWrap(WrapType.Tight);
                    break;
                case "wrapTopAndBottom":
                    ReadWrap(WrapType.TopBottom);
                    break;
                 case "sizeRelH":
                    ReadSizeRelH();
                    break;
                 case "sizeRelV":
                    ReadSizeRelV();
                    break;
                case "AlternateContent":
                    ReadAlternateContent();
                    break;
                default:
                    Debug.Fail(mXmlReader.LocalName);
                    mXmlReader.IgnoreElement();
                    break;
            }
        }

        /// <summary>
        /// Reads M.7.3.4.2 AlternateContent (Alternate Content block).
        /// An alternate content block allows for an alternative representation of information.
        /// </summary>
        private void ReadAlternateContent()
        {
            while (mXmlReader.ReadChild("AlternateContent"))
            {
                switch (mXmlReader.LocalName)
                {
                    // Read only Choice to the model, Fallback values will be generated during validation.
                    // Please see UpdateSizeAndPositionFromRelative method.
                    case "Choice":
                        ReadAlternateContentCore();
                        break;
                    case "Fallback":
                        mXmlReader.IgnoreElement();
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads M.7.3.4.2.1 AlternateContent Syntax
        /// </summary>
        private void ReadAlternateContentCore()
        {
            string tagName = mXmlReader.LocalName;

            while (mXmlReader.ReadChild(tagName))
            {
                switch (mXmlReader.LocalName)
                {
                    case "positionV":
                        ReadPositionV();
                        break;
                    case "positionH":
                        ReadPositionH();
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 5.5.2.6 effectExtent (Object Extents Including Effects).
        /// This element specifies the additional extent which shall be added to each edge
        /// of the image (top, bottom, left, right) in order to compensate for any drawing
        /// effects applied to the DrawingML object.
        /// </summary>
        private void ReadEffectExtent()
        {
            OoxmlComplianceInfo complianceInfo = mReader.ComplianceInfo;
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "b":
                        mShapePr.SetAttr(ShapeAttr.DmlEffectExtentBottom,
                            MathUtil.DoubleToInt(mXmlReader.GetValueAsEmus(complianceInfo)));
                        break;
                    case "l":
                        mShapePr.SetAttr(ShapeAttr.DmlEffectExtentLeft,
                            MathUtil.DoubleToInt(mXmlReader.GetValueAsEmus(complianceInfo)));
                        break;
                    case "r":
                        mShapePr.SetAttr(ShapeAttr.DmlEffectExtentRight,
                            MathUtil.DoubleToInt(mXmlReader.GetValueAsEmus(complianceInfo)));
                        break;
                    case "t":
                        mShapePr.SetAttr(ShapeAttr.DmlEffectExtentTop,
                            MathUtil.DoubleToInt(mXmlReader.GetValueAsEmus(complianceInfo)));
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }
        }

        /// <summary>
        /// 5.5.2.16 wrapPolygon (Wrapping Polygon)
        /// This element specifies the wrapping polygon which shall be used to determine the extents
        /// to which text may wrap around the specified object in the document.
        /// </summary>
        private void ReadWrapPolygon()
        {
            PointList points = new PointList();

            // Has elements only.
            while (mXmlReader.ReadChild("wrapPolygon"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "start":
                    case "lineTo":
                        points.Add(DmlBasicsReader.ReadXY(mXmlReader, mReader.ComplianceInfo));
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }

            // OOXML might have last segment that runs back to the start point, we don't want it in the model.
            Point first = points[0];
            Point last = points[points.Count - 1];
            if (first == last)
                points.RemoveAt(points.Count - 1);

            // Convert to path point objects.
            PathPoint[] wrapPolygon = new PathPoint[points.Count];
            for (int i = 0; i < points.Count; i++)
                wrapPolygon[i] = new PathPoint(points[i]);

            mShapePr.SetAttr(ShapeAttr.WrapPolygonVertices, wrapPolygon);
        }

        /// <summary>
        /// 5.5.2.4 cNvGraphicFramePr (Common DrawingML Non-Visual Properties)
        /// This element specifies common non-visual DrawingML object properties for
        /// the parent DrawingML object. These properties are specified as child elements
        /// of this element.
        /// </summary>
        private void ReadCNvGraphicFramePr()
        {
            // Has elements only.
            while (mXmlReader.ReadChild("cNvGraphicFramePr"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "graphicFrameLocks":
                        DmlBasicsReader.ReadLocks(mXmlReader, mShapePr);
                        break;
                    case "extLst":
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 5.5.2.5 docPr (Drawing Object Non-Visual Properties).
        /// </summary>
        private void ReadDocPr()
        {
            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "descr":
                        mShapePr.SetAttr(ShapeAttr.ShapeDescription, mXmlReader.Value);
                        break;
                    case "hidden":
                        // WORDSNET-25714 Ignore hidden attribute for inline DML shapes.
                        if(mShapePr.WrapType != WrapType.Inline)
                            mShapePr.SetAttr(ShapeAttr.Hidden, mXmlReader.ValueAsBool);
                        break;
                    case "id":
                        mShapePr.SetAttr(ShapeAttr.ShapeId, mXmlReader.ValueAsInt);
                        break;
                    case "name":
                        mShapePr.SetAttr(ShapeAttr.ShapeName, mXmlReader.Value);
                        break;
                    case "title":
                        mShapePr.SetAttr(ShapeAttr.ShapeTitle, mXmlReader.Value);
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }

            // Read elements.
            while (mXmlReader.ReadChild("docPr"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "hlinkClick":
                    {
                        DmlHlink hlink = DmlHlinkReader.Read(mReader);

                        mShapePr.SetAttrIfNotNull(ShapeAttr.HyperlinkAddress, hlink.Id);
                        mShapePr.SetAttrIfNotNull(ShapeAttr.HyperlinkTarget, hlink.TargetFrame);
                        mShapePr.SetAttrIfNotNull(ShapeAttr.ScreenTip, hlink.Tooltip);
                        break;
                    }
                    case "extLst":
                        mDocPrExtensions = DmlExtensionListReader.Read(mReader);
                        break;
                    case "hlinkHover":
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 5.5.2.10 positionH (Horizontal Positioning)
        ///
        /// This element specifies the horizontal positioning of a floating DrawingML object
        /// within a WordprocessingML document.
        /// </summary>
        private void ReadPositionH()
        {
            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "relativeFrom":
                        mShapePr.SetAttr(ShapeAttr.RelativeHorizontalPosition, DmlEnum.DmlToRelativeHorizontalPosition(mXmlReader.Value));
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }

            // Read elements.
            while (mXmlReader.ReadChild("positionH"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "align":
                        mShapePr.SetAttr(ShapeAttr.HorizontalAlignment, DmlEnum.DmlToHorizontalAlignment(mXmlReader.ReadString()));
                        break;
                    case "posOffset":
                        mShapePr.SetAttr(ShapeAttr.Left, ConvertUtilCore.EmuToPoint(mXmlReader.ReadStringAsInt()));
                        break;
                    // MSWord 2010 An ST_Percentage element that specifies the horizontal offset.
                    case "pctPosHOffset":
                        mShapePr.SetAttr(ShapeAttr.LeftPercent, ReadPercentageAsVml());
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads value from element of percentage type and returns it converted to VML percent value.
        /// </summary>
        private int ReadPercentageAsVml()
        {
            double asFraction =
                DmlPercentageUtil.FromPercentOrDmlPercent(mXmlReader.ReadString(), mReader.ComplianceInfo);
            return DmlPercentageUtil.DmlToVmlPercent(DmlPercentageUtil.ToDmlPercent(asFraction));
        }

        /// <summary>
        /// 5.5.2.11 positionV (Vertical Positioning)
        ///
        /// This element specifies the vertical positioning of a floating DrawingML object
        /// within a WordprocessingML document.
        /// </summary>
        private void ReadPositionV()
        {
            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "relativeFrom":
                        mShapePr.SetAttr(ShapeAttr.RelativeVerticalPosition, DmlEnum.DmlToRelativeVerticalPosition(mXmlReader.Value));
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }

            // Read elements.
            while (mXmlReader.ReadChild("positionV"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "align":
                        mShapePr.SetAttr(ShapeAttr.VerticalAlignment, DmlEnum.DmlToVerticalAlignment(mXmlReader.ReadString()));
                        break;
                    case "posOffset":
                        mShapePr.SetAttr(ShapeAttr.Top, ConvertUtilCore.EmuToPoint(mXmlReader.ReadStringAsInt()));
                        break;
                    // MSWord 2010 An ST_Percentage element as specified in [ISO/IEC29500-4:2011] section 12.1.2.2 and
                    // [ISO/IEC29500-1:2011] section 20.1.10.40 that specifies the vertical offset.
                    case "pctPosVOffset":
                        mShapePr.SetAttr(ShapeAttr.TopPercent, ReadPercentageAsVml());
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// 5.5.2.13 simplePos (Simple Positioning Coordinates)
        ///
        /// This element specifies the coordinates at which a DrawingML object shall be positioned relative
        /// to the top-left edge of its page, when the simplePos attribute is specified on the anchor
        /// element (§5.5.2.3).
        /// </summary>
        private void ReadSimplePos()
        {
            mShapePr.SetAttr(ShapeAttr.RelativeHorizontalPosition, RelativeHorizontalPosition.Page);
            mShapePr.SetAttr(ShapeAttr.HorizontalAlignment, HorizontalAlignment.None);

            mShapePr.SetAttr(ShapeAttr.RelativeVerticalPosition, RelativeVerticalPosition.Page);
            mShapePr.SetAttr(ShapeAttr.VerticalAlignment, VerticalAlignment.None);

            Point pos = DmlBasicsReader.ReadXY(mXmlReader, mReader.ComplianceInfo);
            mShapePr.SetAttr(ShapeAttr.Left, ConvertUtilCore.EmuToPoint(pos.X));
            mShapePr.SetAttr(ShapeAttr.Top, ConvertUtilCore.EmuToPoint(pos.Y));
        }

        /// <summary>
        /// Reads any of the following
        /// 5.5.2.15 wrapNone (No Text Wrapping)
        /// 5.5.2.17 wrapSquare (Square Wrapping)
        /// 5.5.2.18 wrapThrough (Through Wrapping)
        /// 5.5.2.19 wrapTight (Tight Wrapping)
        /// 5.5.2.20 wrapTopAndBottom (Top and Bottom Wrapping)
        /// </summary>
        private void ReadWrap(WrapType wrapType)
        {
            // WORDSNET-19671 If 'drawing' has got an 'anchor' element, then any of the 'wrap' sub-elements is possible.
            // But if there is an 'inline' element in 'drawing', then any of the 'wrap' sub-elements indicates
            // incorrect markup. MS Word ignores such 'wrap' sub-elements. We must do the same.
            //
            if (mShapePr.WrapType == WrapType.Inline)
                return;

            mShapePr.SetAttr(ShapeAttr.WrapType, wrapType);

            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "distB":
                        mShapePr.SetAttr(ShapeAttr.DistanceBottom, mXmlReader.ValueAsInt);
                        break;
                    case "distL":
                        mShapePr.SetAttr(ShapeAttr.DistanceLeft, mXmlReader.ValueAsInt);
                        break;
                    case "distR":
                        mShapePr.SetAttr(ShapeAttr.DistanceRight, mXmlReader.ValueAsInt);
                        break;
                    case "distT":
                        mShapePr.SetAttr(ShapeAttr.DistanceTop, mXmlReader.ValueAsInt);
                        break;
                    case "wrapText":
                        mShapePr.SetAttr(ShapeAttr.WrapSide, DmlEnum.DmlToWrapSide(mXmlReader.Value));
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }

            // Read children elements of any wrap element.
            mXmlReader.MoveToElement();
            string tagName = mXmlReader.LocalName;
            while (mXmlReader.ReadChild(tagName))
            {
                switch (mXmlReader.LocalName)
                {
                    case "effectExtent":
                        ReadEffectExtent();
                        break;
                    case "wrapPolygon":
                        ReadWrapPolygon();
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// 2.19.1.3 sizeRelH
        ///
        /// If present, this element specifies that the horizontal size (width) is relative.
        /// If absent, the horizontal size is absolute.
        /// </summary>
        private void ReadSizeRelH()
        {
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "relativeFrom":
                        mShapePr.SetAttr(ShapeAttr.RelativeWidth, DmlEnum.DmlToRelativeWidth(mXmlReader.Value));
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }
            // Read elements.
            while (mXmlReader.ReadChild("sizeRelH"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "pctWidth":
                        mShapePr.SetAttr(ShapeAttr.WidthPercent, ReadPercentageAsVml());
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// 2.19.1.4 sizeRelV
        ///
        /// If present, this element specifies that the vertical size (height) is relative.
        /// If absent, the vertical size is absolute.
        /// </summary>
        private void ReadSizeRelV()
        {
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "relativeFrom":
                        mShapePr.SetAttr(ShapeAttr.RelativeHeight, DmlEnum.DmlToRelativeHeight(mXmlReader.Value));
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, mXmlReader.LocalName));
                        break;
                }
            }
            // Read elements.
            while (mXmlReader.ReadChild("sizeRelV"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "pctHeight":
                        mShapePr.SetAttr(ShapeAttr.HeightPercent, ReadPercentageAsVml());
                        break;
                    default:
                        Debug.Fail(mXmlReader.LocalName);
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Parses and validates the distance value, mimic MS Word behavior.
        /// </summary>
        /// <remarks>
        /// andrnosk: WORDSNET-7439 MS Word 2010 crashes when distance is negative.
        /// </remarks>
        private int ParseDistance(string value)
        {
            long result = FormatterPal.ParseInt64(value);

            // MS Word limits value of distance as 1584pt or 20116800 emus, all values outside the range MS Word interprets as zero.
            if (result > ConvertUtilCore.MaxSizeEmus || result < 0)
            {
                mXmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.DrawingML,
                string.Format(@"The distance value '{0}' is outside of the range and will be interpreted as zero.
                The measurement must be between 0 and 1584pt", result));
                return 0;
            }
            return (int)result;
        }

        private DocxDocumentReaderBase mReader;
        private NrxXmlReader mXmlReader;

        // We collect all shape positioning properties here during loading.
        private ShapePr mShapePr;
        private ShapeBase mDrawingML;

        private StringToObjDictionary<DmlExtension> mDocPrExtensions;
    }
}
