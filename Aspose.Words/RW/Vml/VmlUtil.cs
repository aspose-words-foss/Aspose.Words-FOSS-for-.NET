// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2006 by Vladimir Averkin
using System;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Vml
{
    internal static class VmlUtil
    {
        /// <summary>
        /// Parses comma-delimited string of integers.
        /// </summary>
        internal static int[] VmlToIntArray(string value)
        {
            string[] strings = value.Split(',');
            int[] ints = new int[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                string str = strings[i];
                // Resiliency Jira 6808 - sometimes we get comma delimited string of doubles.
                ints[i] = (int)System.Math.Round(FormatterPal.ParseDouble(str));
            }

            return ints;
        }

        internal static string EmuToVmlPoints(int value)
        {
            return DistanceToVml(ConvertUtilCore.EmuToPoint(value));
        }

        internal static string EmuToVmlMillimeters(int value)
        {
            return MillimetersToVml(ConvertUtilCore.EmuToMillimeter(value));
        }

        /// <summary>
        /// Gets value for 'ShapeID' attribute for OleObjects.
        /// </summary>
        /// <remarks>
        /// WORDSNET-3646 In case shape has 'id' attribute not equal to 'spid' OleObject should refer to 'id' attribute which goes to Name.
        /// </remarks>
        /// <param name="shape"></param>
        /// <returns></returns>
        internal static string GetOleObjectShapeId(ShapeBase shape)
        {
            return StringUtil.HasChars(shape.Name) ? shape.Name : NrxXmlUtil.GetShapeId(shape);
        }

        internal static string GetInset(IntToObjDictionary<object> props)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = ShapeAttr.TextboxLeft; i <= ShapeAttr.TextboxBottom; i++)
            {
                object value = props[i];
                if (value != null)
                    sb.Append(DistanceToVml(ConvertUtilCore.EmuToPoint((int)value)));

                sb.Append(',');
            }

            return sb.ToString().TrimEnd(',');
        }

        internal static string FixedToVml(object value)
        {
            if (value == null)
                return null;

            int intValue = (int)value;  // Has to be on a separate line for Java to work.
            return FixedToVml(intValue);
        }

        internal static string FixedToVml(int value)
        {
            return FixedToVml(value, true);
        }

        /// <summary>
        /// The same like FixedToVml but with or without adding "f".
        /// We need this upon writing FillToLeft/FillToRight/FillToTop/FillToBottom attrs to mimic MS Word behavior.
        /// </summary>
        internal static string FixedToVml(int value, bool includeSuffix)
        {
            if (value == 0)
                return "0";

            if (value % 0x4000 == 0)
                return FormatterPal.DoubleToStr2Decimals((double)value / 0x10000);

            if (includeSuffix)
                return value.ToString() + "f";

            return value.ToString();
        }

        /// <summary>
        /// Converts boolean value to VML string.
        /// Return null if the value equals default value.
        /// </summary>
        internal static string BoolToVml(object value, bool defaultValue)
        {
            bool actualValue = (bool)value; // Has to be on a separate line for autoporting to Java to work correctly.

            if (actualValue != defaultValue)
                return BoolToVml(actualValue);
            else
                return null;
        }

        internal static string BoolToVml(object value)
        {
            if (value == null)
                return null;

            bool boolValue = (bool)value;   // Has to be on a separate line for autoporting to Java to work correctly.
            return BoolToVml(boolValue);
        }

        internal static string BoolToVml(bool value)
        {
            return value ? "t" : "f";
        }

        internal static string FixedDegreesToVml(int value)
        {
            double degrees = ConvertUtilCore.FixedToDouble(value);
            if (degrees == System.Math.Round(degrees))
            {
                // Rotation is a whole number, MS Word writes as a value in degrees, we do the same.
                return FormatterPal.IntToXml((int)degrees);
            }
            else
            {
                // Rotation has a fraction. MS Word writes as fixed value, we do the same.
                return FormatterPal.IntToXml(value) + "fd";
            }
        }

        internal static string ColorsToVml(GradientColor[] colors, IVmlShapeWriterContext context)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < colors.Length; i++)
            {
                int start = colors[i].Start;
                DrColor color = colors[i].Color;

                if (start == 0)
                    sb.Append(string.Format("{0} {1};", 0, context.ColorToVml(color)));
                else if (start == colors.Length - 1)
                    sb.Append(string.Format("{0} {1};", 1, context.ColorToVml(color)));
                else
                    sb.Append(string.Format("{0} {1};", FixedToVml(start), context.ColorToVml(color)));
            }

            if (sb.Length > 0)
                return sb.ToString(0, sb.Length - 1);
            else
                return "";
        }

        internal static string PointToVml(Point point)
        {
            return string.Format("{0},{1}", FormatterPal.IntToXml(point.X), FormatterPal.IntToXml(point.Y));
        }

        internal static string PointToVml(double x, double y, bool isAbsolute)
        {
            return string.Format("{0},{1}", DistanceToVml(x, isAbsolute), DistanceToVml(y, isAbsolute));
        }

        internal static string DistanceToVml(object value, bool isAbsolute)
        {
            if (value == null)
                return null;

            double doubleValue = (double)value; // Has to be on a separate line for Java to work.
            return DistanceToVml(doubleValue, isAbsolute);
        }

        internal static string DistanceToVml(double value, bool isAbsolute)
        {
            if (isAbsolute)
                return DistanceToVml(value);
            else
                return FormatterPal.IntToXml(MathUtil.DoubleToInt(value)); // Child shape coordinates are to be written as integers.
        }

        internal static string DistanceToVml(double value)
        {
            // Value may be very small but does not equal to zero. However, it will be written as zero.
            // So, need to compare with rounded value while choosing output format.
            if (System.Math.Round(value, 2) == 0)
                return "0";

            if (value % 72 == 0)
                return FormatterPal.DoubleToStr2Decimals(value / 72) + "in";

            return FormatterPal.DoubleToStr2Decimals(value) + "pt";
        }

        internal static string MillimetersToVml(double value)
        {
            if (value == 0)
                return "0";
            else
                return FormatterPal.DoubleToStr9Decimals(value) + "mm";
        }

        internal static string PercentsToVml(int value)
        {
            if (value == 0)
                return "0";
            else
                return FormatterPal.IntToXml(value) + "%";
        }

        internal static string BorderToVmlColor(Border value, IVmlShapeWriterContext context)
        {
            // TODO 3: In fact I still do not know exactly when to write image borders and when not.
            // Currently my output does not always matches MS Word in that matter.
            // But the appearance of the documents seems to match, so I am ok with it.
            if ((value != null) && value.IsVisible)
            {
                DrColor color = value.ColorInternal;
                return context.ColorToVml(color);
            }
            else
                return "";
        }

        internal static string PerspectiveToVml(int value)
        {
            double perspective = (value / PerspectiveMultiplier);
            return FormatterPal.DoubleToStr11HashesEZero(perspective);
        }

        internal static int VmlToPerspective(string value)
        {
            return MathUtil.DoubleToInt(FormatterPal.ParseDouble(value) * PerspectiveMultiplier);
        }

        /// <summary>
        /// Calculates hash code of the diagram node shape used in DiagramNodeRelation as reference.
        /// The hash is a CRC32 of the shape name stored in Escher.
        /// </summary>
        /// <param name="shapeName">Shape name from ShapeAttr.ShapeName attribute.</param>
        /// <returns>Hash code for the given name.</returns>
        internal static int CalculateDiagramHash(string shapeName)
        {
            // RK MS Word 2007 prepends # before the shape name and looks like it has to be excluded from hash.
            byte[] namebytes = Encoding.Unicode.GetBytes(shapeName.Trim('#'));

            CrcMaker crcMaker = new CrcMaker();
            return crcMaker.MakeCRC(namebytes);
        }

        /// <summary>
        /// Pack PICT image bytes into compressed PICT (*.pcz) format.
        /// </summary>
        internal static byte[] PackPictImage(byte[] imageBytes)
        {
            // PICT image bytes taken from DOC contain two blocks of data: header and body.
            // Header has length 0x22 and contains PICT image parameters,
            // such as length of uncompressed image file (Int32, goes first), width, height and some others.
            // Body is a zipped stream of image bytes.

            // Build uncompressed PICT image here.
            // It consists of the starter, which is 512 bytes long, and image data proper.
            MemoryStream imageStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(imageStream);

            // Write prefix.
            string prefix = "MSOFFICE9.0";
            for (int i = 0; i < prefix.Length; i++)
            {
                writer.Write((byte)prefix[i]);
            }

            writer.Write((uint)imageBytes.Length);

            // Write image bytes.
            ImageSizeCore imageSize = ImageUtil.GetPictSize(imageBytes);
            writer.Write(imageSize.Left);
            writer.Write(imageSize.Top);
            writer.Write(imageSize.Right);
            writer.Write(imageSize.Bottom);
            writer.Write(imageSize.WidthEmus);
            writer.Write(imageSize.HeightEmus);

            // This should be a size of compressed image bytes. Beats me why it should be stored here in uncompressed image.
            // Writing 0 seems to be ok.
            writer.Write((uint)0);

            // MS Word stores 0xfe00 here. Can be as well put to 0 as it seems nobody cares about it.
            writer.Write((ushort)0xfe00);

            // Fill the rest of the starter bytes with 0. May be this is excessive and they are 0 anyway?
            for (int i = 0; i < 467; i++)
            {
                writer.Write((byte)0);
            }

            // Write image bytes.
            writer.Write(imageBytes);

            // Pack image into compressed form.
            return PackImage(imageStream.ToArray());
        }

        internal static byte[] UnpackPictImage(byte[] packedBytes)
        {
            byte[] unpackedBytes = UnpackImage(packedBytes);

            // See PackPictImage. It seems there is a 512 bytes header. We don't need it in the model.
            byte[] imageBytes = new byte[unpackedBytes.Length - 512];
            Array.Copy(unpackedBytes, 512, imageBytes, 0, imageBytes.Length);

            return imageBytes;
        }

        /// <summary>
        /// Pack image into compressed image format (*.wmz, *.pcz, etc.).
        /// </summary>
        internal static byte[] PackImage(byte[] imageBytes)
        {
            imageBytes = ZipUtilPal.Deflate(imageBytes, ZipMethod.Deflate);

            byte[] packedImageBytes = new byte[imageBytes.Length + gPackedImageHeader.Length];
            imageBytes.CopyTo(packedImageBytes, gPackedImageHeader.Length);

            for (int i = 0; i < gPackedImageHeader.Length; i++)
            {
                packedImageBytes[i] = gPackedImageHeader[i];
            }

            return packedImageBytes;
        }

        /// <summary>
        /// Unpacks images that are compressed (*.wmz, *.pcz, etc.).
        /// Safe to call if image is not compressed, in this case returns image bytes.
        /// </summary>
        internal static byte[] UnpackImage(byte[] imageBytes)
        {
            if (IsImagePacked(imageBytes))
            {
                // Skip first 10 bytes of the header.
                MemoryStream imageStream = new MemoryStream(imageBytes, 10, imageBytes.Length - 10);
                byte[] unpackedImageBytes = ZipUtilPal.Inflate(imageStream, 0, ZipMethod.Deflate);
                return unpackedImageBytes;
            }

            return imageBytes;
        }

        /// <summary>
        /// Convert array of <see cref="PathPoint"/> to VML string.
        /// </summary>
        /// <param name="pathPointArrey">PathPoint array to convert.</param>
        /// <param name="coordDelimiter">Delimiter between X and Y coordinates.</param>
        /// <param name="pairDelimiter">Delimiter between XY pair.</param>
        /// <returns></returns>
        internal static string PathPointsToVml(PathPoint[] pathPointArrey, char coordDelimiter, char pairDelimiter)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < pathPointArrey.Length; i++)
            {
                PathPoint loc = pathPointArrey[i];
                sb.Append(PathValueToVml(loc.X));
                sb.Append(coordDelimiter);
                sb.Append(PathValueToVml(loc.Y));
                sb.Append(pairDelimiter);
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        internal static string PathValueToVml(PathValue value)
        {
            if (value.IsFormula)
                return "@" + FormatterPal.IntToXml(value.Value);
            else
                return FormatterPal.IntToXml(value.Value);
        }

        /// <summary>
        /// Calculates line vertices coordinates according to geometry data and specified
        /// shape size and positions.
        /// </summary>
        /// <param name="shape">Source shape.</param>
        /// <param name="t">Actual top position.</param>
        /// <param name="l">Actual left position.</param>
        /// <param name="w">Actual width.</param>
        /// <param name="h">Actual height.</param>
        /// <returns>Array with line vertices coordinates.</returns>
        internal static double[] GetLineVertices(ShapeBase shape, double t, double l, double w, double h)
        {
            Debug.Assert(shape != null);

            PathPoint[] vertices = (PathPoint[])shape.FetchShapeAttrInternal(ShapeAttr.GeometryVertices);

            // Expected two PathPoint to process.
            if ((vertices == null) || (vertices.Length < 2))
                return new double[0];

            return new double[] { (vertices[0].X.Value > 0) ? l + w : l,
                (vertices[0].Y.Value > 0) ? t + h : t,
                (vertices[1].X.Value > 0) ? l + w : l,
                (vertices[1].Y.Value > 0) ? t + h : t};
        }

        internal static bool IsWordArt(ShapeType shapeType, bool isGeoTextOn)
        {
            switch (shapeType)
            {
                case ShapeType.NonPrimitive:
                case ShapeType.TextSimple:
                case ShapeType.TextOctagon:
                case ShapeType.TextHexagon:
                case ShapeType.TextCurve:
                case ShapeType.TextWave:
                case ShapeType.TextRing:
                case ShapeType.TextOnCurve:
                case ShapeType.TextOnRing:
                case ShapeType.TextPlainText:
                case ShapeType.TextStop:
                case ShapeType.TextTriangle:
                case ShapeType.TextTriangleInverted:
                case ShapeType.TextChevron:
                case ShapeType.TextChevronInverted:
                case ShapeType.TextRingInside:
                case ShapeType.TextRingOutside:
                case ShapeType.TextArchUpCurve:
                case ShapeType.TextArchDownCurve:
                case ShapeType.TextCircleCurve:
                case ShapeType.TextButtonCurve:
                case ShapeType.TextArchUpPour:
                case ShapeType.TextArchDownPour:
                case ShapeType.TextCirclePour:
                case ShapeType.TextButtonPour:
                case ShapeType.TextCurveUp:
                case ShapeType.TextCurveDown:
                case ShapeType.TextCascadeUp:
                case ShapeType.TextCascadeDown:
                case ShapeType.TextWave1:
                case ShapeType.TextWave2:
                case ShapeType.TextWave3:
                case ShapeType.TextWave4:
                case ShapeType.TextInflate:
                case ShapeType.TextDeflate:
                case ShapeType.TextInflateBottom:
                case ShapeType.TextDeflateBottom:
                case ShapeType.TextInflateTop:
                case ShapeType.TextDeflateTop:
                case ShapeType.TextDeflateInflate:
                case ShapeType.TextDeflateInflateDeflate:
                case ShapeType.TextFadeRight:
                case ShapeType.TextFadeLeft:
                case ShapeType.TextFadeUp:
                case ShapeType.TextFadeDown:
                case ShapeType.TextSlantUp:
                case ShapeType.TextSlantDown:
                case ShapeType.TextCanUp:
                case ShapeType.TextCanDown:
                    return isGeoTextOn;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the modifier string value in VML.
        /// </summary>
        internal static string VmlModifier(object value)
        {
            int modifier = (int)value;
            if (modifier == 0)
                return string.Empty;

            string modifierFormat = (modifier > 0)
                ? "lighten({0})"
                : "darken({0})";
            return string.Format(modifierFormat, System.Math.Abs(modifier));
        }

        /// <summary>
        /// Returns the base color string value in VML.
        /// </summary>
        internal static string VmlBaseColor(object value)
        {
            DrColor baseColor = (DrColor)value;

            return (baseColor.IsEmpty)
                ? string.Empty
                : string.Format("rgb({0},{1},{2})", baseColor.R, baseColor.G, baseColor.B);
        }

        /// <summary>
        /// Gets adjustment key by index.
        /// </summary>
        internal static int GetAdjustKey(int index)
        {
            int adjKey = ShapeAttr.GeometryAdjust1 + index - 1;
            return adjKey;
        }

        private static bool IsImagePacked(byte[] imageBytes)
        {
            // I have seen this header as 1F 8B 08 00 00 00 00 00 02 0B
            //                   and also 1F 8B 08 00 00 00 00 00 04 0B
            // As I do not know the format of this header I will check only first four bytes.
            // I think it is safe and reliable enough, at least for now.
            const int numberOfBytesToCheck = 4;

            if ((imageBytes == null) || (imageBytes.Length < numberOfBytesToCheck))
                return false;

            for (int i = 0; i < numberOfBytesToCheck; i++)
            {
                if (imageBytes[i] != gPackedImageHeader[i])
                    return false;
            }

            return true;
        }

        private static readonly byte[] gPackedImageHeader = { 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x0b };

        private const double PerspectiveMultiplier = 16777216; // 0.000000059604644775
    }
}
