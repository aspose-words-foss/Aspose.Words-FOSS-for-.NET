// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Nrx;

namespace Aspose.Words.Drawing.Core.Dml.Readers
{
    /// <summary>
    /// Represents a class building DrawingML path from XML.
    /// </summary>
    internal class DmlPathReader : DmlReaderBase
    {
        private DmlPathReader(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            mReader = reader;
            mComplianceInfo = complianceInfo;
            mPath = new DmlPath();
        }

        /// <summary>
        /// Builds a path using specified XML reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="complianceInfo"></param>
        /// <returns>Null if the current node in the reader isn't a path node.</returns>
        internal static DmlPath Read(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            if (reader.LocalName != "path")
                return null;

            DmlPathReader pathReader = new DmlPathReader(reader, complianceInfo);
            pathReader.ReadPath();
            return pathReader.mPath;
        }

        /// <summary>
        /// Reads the path.
        /// </summary>
        private void ReadPath()
        {
            mPath.FillMode = ReadFillMode();

            while (mReader.MoveToNextAttribute())
            {
                switch (mReader.LocalName)
                {
                    case "h":
                        mPath.Height = mReader.ValueAsDouble;
                        break;
                    case "w":
                        mPath.Width = mReader.ValueAsDouble;
                        break;
                    case "stroke":
                        mPath.Stroke = mReader.ValueAsBool;
                        break;
                    case "extrusionOk":
                        mPath.ExtrusionOk = mReader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(mReader);
                        break;
                }
            }

            while (mReader.ReadChild("path"))
            {
                switch (mReader.LocalName)
                {
                    case "close":
                        mPath.AddPathPart(ReadClose());
                        break;
                    case "lnTo":
                        mPath.AddPathPart(ReadLineTo());
                        break;
                    case "moveTo":
                        mPath.AddPathPart(ReadMoveTo());
                        break;
                    case "arcTo":
                        mPath.AddPathPart(ReadArcTo());
                        break;
                    case "cubicBezTo":
                        mPath.AddPathPart(ReadCubicBezTo());
                        break;
                    case "quadBezTo":
                        mPath.AddPathPart(ReadQuadBezTo());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        private DmlPathFillMode ReadFillMode()
        {
            string value = mReader.ReadAttribute("fill", String.Empty);
            return DmlEnum.DmlToPathFillMode(value);
        }

        /// <summary>
        /// Reads the quadBezTo tag.
        /// </summary>
        private IDmlPathPart ReadQuadBezTo()
        {
            DmlAdjustablePoint[] points = new DmlAdjustablePoint[2];
            int index = 0;
            while (mReader.ReadChild("quadBezTo"))
            {
                switch (mReader.LocalName)
                {
                    case "pt":
                    {
                        if (index < points.Length)
                        {
                            points[index] = ReadPoint();
                            index++;
                        }
                        else
                            WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }

            return new DmlQuadraticBezierTo(points);
        }

        /// <summary>
        /// Reads the cubicBezTo tag.
        /// </summary>
        private IDmlPathPart ReadCubicBezTo()
        {
            DmlAdjustablePoint[] points = new DmlAdjustablePoint[3];
            int index = 0;
            while (mReader.ReadChild("cubicBezTo"))
            {
                switch (mReader.LocalName)
                {
                    case "pt":
                    {
                        if (index < points.Length)
                        {
                            points[index] = ReadPoint();
                            index++;
                        }
                        else
                            WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }

            return new DmlCubicBezierTo(points);
        }

        /// <summary>
        /// Reads the arcTo.
        /// </summary>
        private IDmlPathPart ReadArcTo()
        {
            DmlArcTo arcTo = new DmlArcTo();
            string hR = mReader.ReadAttribute("hR", "");
            CheckForIsoTransitional(hR);
            arcTo.HeightRadius = new DmlAdjustableCoordinate(hR);
            string wR = mReader.ReadAttribute("wR", "");
            CheckForIsoTransitional(wR);
            arcTo.WidthRadius = new DmlAdjustableCoordinate(wR);
            arcTo.StartAngle = new DmlAdjustableAngle(mReader.ReadAttribute("stAng", ""));
            arcTo.SwingAngle = new DmlAdjustableAngle(mReader.ReadAttribute("swAng", ""));
            return arcTo;
        }

        /// <summary>
        /// Checks if the input value is in format that is introduced by ISO/IEC 29500.
        /// Sets the ISO Transitional flag in this case.
        /// </summary>
        private void CheckForIsoTransitional(string value)
        {
            if ((mComplianceInfo != null) && (NrxXmlReader.IsUniversalMeasure(value)))
                mComplianceInfo.MarkAsIsoTransitional();
        }

        /// <summary>
        /// Reads the close.
        /// </summary>
        private static IDmlPathPart ReadClose()
        {
            return new DmlClose();
        }

        /// <summary>
        /// Reads the line to.
        /// </summary>
        private IDmlPathPart ReadLineTo()
        {
            DmlLineTo result = new DmlLineTo();
            while (mReader.ReadChild("lnTo"))
            {
                switch (mReader.LocalName)
                {
                    case "pt":
                        result.Point = ReadPoint();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Reads the point.
        /// </summary>
        private DmlAdjustablePoint ReadPoint()
        {
            if (mReader.LocalName != "pt")
                throw new InvalidOperationException("Cannot read a point from the '{0}' tag. The 'pt' tag is required.");
            string x = mReader.ReadAttribute("x", String.Empty);
            CheckForIsoTransitional(x);
            string y = mReader.ReadAttribute("y", String.Empty);
            CheckForIsoTransitional(y);
            return new DmlAdjustablePoint(x, y);
        }

        /// <summary>
        /// Reads the move to.
        /// </summary>
        private IDmlPathPart ReadMoveTo()
        {
            DmlMoveTo moveTo = new DmlMoveTo();
            while (mReader.ReadChild("moveTo"))
            {
                switch (mReader.LocalName)
                {
                    case "pt":
                        moveTo.Point = ReadPoint();
                        return moveTo;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
            return moveTo;
        }

        private readonly DmlPath mPath;
        private readonly NrxXmlReader mReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
