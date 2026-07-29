// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2006 by Vladimir Averkin
using System;
using System.Text;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes geometry-related attributes and subelements of the shape to WordML (path, v:path, v:formulas, v:handles).
    /// </summary>
    internal class VmlGeoWriter
    {
        internal VmlGeoWriter(ShapeBase shape, NrxXmlBuilder builder)
        {
            mBuilder = builder;
            mVmlBuilder = new VmlBuilder(builder);
            mShape = shape;
        }

        /// <summary>
        /// Add geometry-related related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            switch (key)
            {
                case ShapeAttr.LineArrowHeadsOK:
                    mArrowOk = (bool)value;
                    break;
                case ShapeAttr.GeometryFillOK:
                    mFillOk = (bool)value;
                    break;
                case ShapeAttr.GeometryLineOK:
                    mStrokeOk = (bool)value;
                    break;
                case ShapeAttr.GeometryShadowOK:
                    mShadowOk = (bool)value;
                    break;
                case ShapeAttr.GeometryThreeDOK:
                    mExtrusionOk = (bool)value;
                    break;
                case ShapeAttr.GeometryFillShadeShapeOK:
                    mGradientShapeOk = (bool)value;
                    break;
                case ShapeAttr.GeometryGTextOK:
                    mTextpathOk = (bool)value;
                    break;
                case ShapeAttr.GeometryXLimo:
                    mXLimo = (int)value;
                    break;
                case ShapeAttr.GeometryYLimo:
                    mYLimo = (int)value;
                    break;
                case ShapeAttr.GeometryConnectionSiteType:
                    mConnectType = VmlEnum.ConnectionSiteTypeToVml((ConnectionSiteType)value);
                    break;
                case ShapeAttr.GeometryConnectLocs:
                    mConnectLocs = (PathPoint[])value;
                    break;
                case ShapeAttr.GeometryConnectAngles:
                    mConnectAngles = (int[])value;
                    break;
                case ShapeAttr.GeometryPathTextBoxRects:
                    mTextboxRects = (PathRectangle[])value;
                    break;
                case ShapeAttr.GeometrySegmentInfo:
                    mPathInfos = (PathInfo[])value;
                    break;
                case ShapeAttr.GeometryVertices:
                    mPoints = (PathPoint[])value;
                    break;
                case ShapeAttr.GeometryFormulas:
                    mFormulas = (Formula[])value;
                    break;
                case ShapeAttr.GeometryHandles:
                    mHandles = (Handle[])value;
                    break;
                case ShapeAttr.GeometryAdjust1:
                case ShapeAttr.GeometryAdjust2:
                case ShapeAttr.GeometryAdjust3:
                case ShapeAttr.GeometryAdjust4:
                case ShapeAttr.GeometryAdjust5:
                case ShapeAttr.GeometryAdjust6:
                case ShapeAttr.GeometryAdjust7:
                case ShapeAttr.GeometryAdjust8:
                case ShapeAttr.GeometryAdjust9:
                case ShapeAttr.GeometryAdjust10:
                    mGeometryAdjust[key - ShapeAttr.GeometryAdjust1] = FormatterPal.IntToXml((int)value);
                    break;
                default:
                    // Ignore other attributes.
                    break;
            }
        }

        /// <summary>
        /// Write 'adj' attribute of 'w:shape' element.
        /// </summary>
        internal void WriteAdjAttribute()
        {
            // Roundrect has adj1, but to VML it is written as arcsize.
            if (mShape.ShapeType == ShapeType.RoundRectangle)
                return;
            
             if (mShape.ShapeType == ShapeType.CustomShape)
             {
                 if (mGeometryAdjust[0] == null)
                     mGeometryAdjust[0] = "-11796480";
 
                 if (mGeometryAdjust[2] == null)
                     mGeometryAdjust[2] = "5400";
             }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= 9; i++)
            {
                if (mGeometryAdjust[i] != null)
                    sb.Append(mGeometryAdjust[i]);

                sb.Append(',');
            }

            mVmlBuilder.WriteVmlAttribute("adj", sb.ToString().TrimEnd(','));
        }

        /// <summary>
        /// Write 'path' attribute of 'w:shape' element.
        /// </summary>
        internal void WritePathAttribute()
        {
            if ((mPoints == null) || (mPoints.Length == 0))
                return;

            if ((mPathInfos == null) || (mPathInfos.Length == 0))
                return;

            StringBuilder sb = new StringBuilder();
            // write path using commands defined in ShapeAttr.GeometrySegmentInfo
            int pointsProcessed = 0; 

            for (int pathInfoIdx = 0; pathInfoIdx < mPathInfos.Length; pathInfoIdx++)
            {
                PathInfo pathInfo = mPathInfos[pathInfoIdx];

                sb.Append(VmlEnum.PathTypeToVml(pathInfo.PathType));

                int pointCount = pathInfo.GetPointCount();
                for (int i = 0; i < pointCount; i++)
                {
                    if (pointsProcessed >= mPoints.Length)
                        throw new InvalidOperationException("Not enough parameters in geometry path.");

                    PathPoint pp = mPoints[pointsProcessed];
                    sb.Append(PathValueToVml(pp.X));
                    sb.Append(',');
                    sb.Append(PathValueToVml(pp.Y));
                    sb.Append(',');

                    pointsProcessed++;
                }

                if (pointCount > 0)
                    sb.Remove(sb.Length - 1, 1);
            }

            mBuilder.WriteAttribute("path", sb.ToString());
        }

        private static string PathValueToVml(PathValue pathValue)
        {
            if (pathValue.IsFormula)
                return "@" + pathValue.Value;
            else
                return FormatterPal.IntToStrNoZero(pathValue.Value);
        }

        /// <summary>
        /// Write 'v:formulas' element.
        /// </summary>
        internal void WriteFormulas()
        {
            if (mFormulas != null)
            {
                mBuilder.StartElement("v:formulas");

                for (int i = 0; i < mFormulas.Length; i++)
                {
                    Formula f = mFormulas[i];
                    mBuilder.StartElement("v:f");
                    mBuilder.WriteAttributeString("eqn", GetFormulaEqn(f));
                    mBuilder.EndElement(); //v:f
                }

                mBuilder.EndElement(); //v:formulas
            }
        }

        /// <summary>
        /// Write 'v:path' element.
        /// </summary>
        internal void WritePath()
        {
            if (HasNotDefaultPathAttrs)
            {
                mBuilder.StartElement("v:path");
                mVmlBuilder.WriteVmlAttributeIfNotDefault("arrowok", mArrowOk, false);
                mVmlBuilder.WriteVmlAttributeIfNotDefault("fillok", mFillOk, true);
                mVmlBuilder.WriteVmlAttributeIfNotDefault("strokeok", mStrokeOk, true);
                mVmlBuilder.WriteVmlAttributeIfNotDefault("shadowok", mShadowOk, true);
                mVmlBuilder.WriteVmlAttributeIfNotDefault("o:extrusionok", mExtrusionOk, true);
                mVmlBuilder.WriteVmlAttributeIfNotDefault("gradientshapeok", mGradientShapeOk, false);
                mVmlBuilder.WriteVmlAttributeIfNotDefault("textpathok", mTextpathOk, false);

                if (mXLimo != 0 || mYLimo != 0)
                    mBuilder.WriteAttribute("limo", string.Format("{0},{1}", mXLimo, mYLimo));

                if (mConnectLocs != null && mConnectLocs.Length > 0 && mConnectType == null)
                    mBuilder.WriteAttribute("o:connecttype", "custom");
                else
                    mBuilder.WriteAttributeIfNotDefault("o:connecttype", mConnectType, "none");

                WriteConnectLocs();
                WriteConnectAngles();
                WriteTextboxRects();
                mBuilder.EndElement(); //v:path
            }
        }

        private bool HasNotDefaultPathAttrs
        {
            get
            {
                return (mArrowOk || !mFillOk || !mStrokeOk || !mShadowOk || !mExtrusionOk || mGradientShapeOk ||
                        mTextpathOk || IsNotDefaultConnectType || HasLocs || HasAngles || HasTextboxRects);
            }
        }

        private bool IsNotDefaultConnectType
        {
            get { return (mConnectType != null && mConnectType != "none"); }
        }

        private bool HasLocs
        {
            get { return (mConnectLocs != null && mConnectLocs.Length > 0); }
        }

        private bool HasAngles
        {
            get { return (mConnectAngles != null && mConnectAngles.Length > 0); }
        }

        private bool HasTextboxRects
        {
            get { return (mTextboxRects != null); }
        }

        private void WriteConnectLocs()
        {
            if (mConnectLocs != null && mConnectLocs.Length > 0)
                mBuilder.WriteAttribute("o:connectlocs", VmlUtil.PathPointsToVml(mConnectLocs, ',', ';'));
        }

        private void WriteConnectAngles()
        {
            if (mConnectAngles != null && mConnectAngles.Length > 0)
            {
                StringBuilder sb = new StringBuilder(32);

                for (int i = 0; i < mConnectAngles.Length; i++)
                {
                    int angle = mConnectAngles[i];
                    sb.Append(VmlUtil.FixedDegreesToVml(angle));
                    sb.Append(',');
                }

                sb.Remove(sb.Length - 1, 1);
                mBuilder.WriteAttribute("o:connectangles", sb.ToString());
            }
        }

        private void WriteTextboxRects()
        {
            if (mTextboxRects != null)
            {
                string rects = GetTextBoxRects(mTextboxRects);
                mBuilder.WriteAttribute("textboxrect", rects);
            }
            else if (mShape.HasChildNodes && (mShape.ShapeType == ShapeType.TextBox))
            {
                // andrnosk: WORDSNET-7053 Text box rectangle is not specified so return the bounding rectangle of shape.
                string rects = GetTextBoxRects(((Shape)mShape).TextBoxRects);
                mBuilder.WriteAttribute("textboxrect", rects);
            }
        }

        /// <summary>
        /// Create Text box rectangle string value.
        /// </summary>
        private static string GetTextBoxRects(PathRectangle[] pathRectangle)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pathRectangle.Length; i++)
            {
                PathRectangle rect = pathRectangle[i];
                sb.Append(VmlUtil.PathValueToVml(rect.Left));
                sb.Append(',');
                sb.Append(VmlUtil.PathValueToVml(rect.Top));
                sb.Append(',');
                sb.Append(VmlUtil.PathValueToVml(rect.Right));
                sb.Append(',');
                sb.Append(VmlUtil.PathValueToVml(rect.Bottom));

                if (i < pathRectangle.Length - 1)
                    sb.Append(";");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Write 'v:handles' element.
        /// </summary>
        internal void WriteHandles()
        {
            if (mHandles != null && mHandles.Length > 0)
            {
                mBuilder.StartElement("v:handles");

                for (int i = 0; i < mHandles.Length; i++)
                {
                    Handle h = mHandles[i];

                    mBuilder.StartElement("v:h");

                    // NOTE: It still is not clear for me if all other handle params like Polar, Map and so on
                    //       should use another class for their value. So I decided to leave the procedure intact for now.
                    //       Later it should be rewritten using proper types for Handle properties, maybe.
                    WriteHandlePoint("position",
                        new PathValue(h.PositionX.PackedValue, true),
                        new PathValue(h.PositionY.PackedValue, true));

                    if (h.HasSwitch)
                        mBuilder.WriteAttributeString("switch", "");

                    if (h.HasPolar)
                        WriteHandlePoint("polar", h.PolarX, h.PolarY);

                    if (h.HasMap)
                        WriteHandlePoint("map", h.PolarX, h.PolarY);

                    if (h.HasRadiusRange)
                        WriteHandlePoint("radiusrange", h.XMin, h.XMax);

                    if (h.HasXYRange)
                    {
                        if (!((h.XMin.Value == int.MinValue) && (h.XMax.Value == int.MaxValue)))
                            WriteHandlePoint("xrange", h.XMin, h.XMax);

                        if (!((h.YMin.Value == int.MinValue) && (h.YMax.Value == int.MaxValue)))
                            WriteHandlePoint("yrange", h.YMin, h.YMax);
                    }

                    mBuilder.EndElement(); //v:h
                }

                mBuilder.EndElement(); //v:handles
            }
        }

        private void WriteHandlePoint(string attribName, PathValue x, PathValue y)
        {
            mBuilder.WriteAttributeString(attribName, BuildPoint(GetHandleValue(x), GetHandleValue(y)));
        }

        private static string BuildPoint(string x, string y)
        {
            return string.Format("{0},{1}", x, y);
        }

        private static string GetHandleValue(PathValue value)
        {
            if (value.IsFormula)
            {
                switch (value.Value)
                {
                    case 0:
                        return "topLeft";
                    case 1:
                        return "bottomRight";
                    case 2:
                        return "center";
                    default:
                        if (value.Value < 256)
                            return "@" + (value.Value - 3);
                        else
                            return "#" + (value.Value - 256);
                }
            }
            else
                return value.Value.ToString();
        }

        private static string GetFormulaEqn(Formula formula)
        {
            if (formula.Operation == Operation.Sum && formula.Param2 == 0 && formula.Param3 == 0)
                return BuildEqnString("val", GetOperand(formula.Param1, formula.IsParam1Calculated));

            string operation = VmlEnum.OperationToVml(formula.Operation);

            switch (formula.Operation)
            {
                case Operation.Abs:
                case Operation.Sqrt:
                    return BuildEqnString(
                        operation, 
                        GetOperand(formula.Param1, formula.IsParam1Calculated));
                case Operation.Mid:
                case Operation.Min:
                case Operation.Max:
                case Operation.Atan2:
                case Operation.Sin:
                case Operation.Cos:
                    return BuildEqnString(
                        operation,
                        GetOperand(formula.Param1, formula.IsParam1Calculated),
                        GetOperand(formula.Param2, formula.IsParam2Calculated));
                default:
                    return BuildEqnString(
                        operation,
                        GetOperand(formula.Param1, formula.IsParam1Calculated),
                        GetOperand(formula.Param2, formula.IsParam2Calculated),
                        GetOperand(formula.Param3, formula.IsParam3Calculated));
            }
        }

        private static string BuildEqnString(params string[] args)
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                sb.Append(' ');
                sb.Append(args[i]);
            }

            return sb.ToString();
        }

        private static string GetOperand(int value, bool isRef)
        {
            if (isRef)
            {
                switch (value)
                {
                    case 0x140:
                        return "xcenter"; // The x ordinate of the center of coordorigin, coordsize (x+w/2).
                    case 0x141:
                        return "ycenter"; // The y ordinate of the center of coordorigin, coordsize (y+h/2).
                    case 0x142:
                        return "width";    // The width defined by the coordsize attribute. 
                    case 0x143:
                        return "height"; // The height defined by the coordsize attribute.
                    case 0x153:
                        return "xlimo"; // The x value of the limo attribute.
                    case 0x154:
                        return "ylimo"; // The y value of the limo attribute.
                    case 0x1fc:
                        return "lineDrawn";
                    case 0x4f7:
                        // The line width in output device pixels. This is used to outset lines from the edge of a rectangle on the assumption that 
                        // the implementation draws to lower right pixel in preference to the upper left pixel when a line is on a pixel boundary.
                        return "pixelLineWidth"; 
                    case 0x4f8:
                        return "pixelWidth"; // The width of the shape in device pixels (i.e. the coordsize width transformed into device space).
                    case 0x4f9:
                        return "pixelHeight"; // The height of the coordsize in device pixels.
                    case 0x4fc:
                        return "emuWidth"; // The width of the coordsize in EMUs.
                    case 0x4fd:
                        return "emuHeight"; // The height of the coordsize in EMUs.
                    case 0x4fe:
                        return "emuWidth2"; // Half the width of the coordsize in EMUs.
                    case 0x4ff:
                        return "emuHeight2"; // Half the height of the coordsize in EMUs.
                    default:
                        if (value >= CalculatedParam.FormulaReferenceMin)
                        {
                            // formula reference
                            return "@" + (value - CalculatedParam.FormulaReferenceMin);
                        }
                        else if ((value >= CalculatedParam.Adjust1) && (value <= CalculatedParam.Adjust8))
                        {
                            // adjustment reference
                            return "#" + (value - CalculatedParam.Adjust1);
                        }
                        else
                        {
                            throw new InvalidOperationException("Unrecognized operand value in the formula.");
                        }
                }
            }
            else
            {
                return FormatterPal.IntToXml(value);
            }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly VmlBuilder mVmlBuilder;
        private readonly ShapeBase mShape;

        private bool mArrowOk;
        private bool mFillOk = true;
        private bool mStrokeOk = true;
        private bool mShadowOk = true;
        private bool mExtrusionOk = true;
        private bool mGradientShapeOk;
        private bool mTextpathOk;
        private int mXLimo;
        private int mYLimo;
        private string mConnectType;
        private PathRectangle[] mTextboxRects;
        private PathPoint[] mConnectLocs;
        private int[] mConnectAngles;
        private PathPoint[] mPoints;
        private PathInfo[] mPathInfos;
        private readonly string[] mGeometryAdjust = new string[10];
        private Formula[] mFormulas;
        private Handle[] mHandles;
    }

}
