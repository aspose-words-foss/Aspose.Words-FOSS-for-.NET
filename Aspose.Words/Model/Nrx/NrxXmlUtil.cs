// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2007 by Roman Korchagin

using System;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Revisions;
using Aspose.Xml;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Utility methods for WordML and DOCX XML processing.
    /// Converting base types to/from XML and so on.
    ///
    /// If we find generally useful methods here (non-MS Word specific), should move out
    /// so they could be reused by other XML exports (to ODF and so on).
    /// </summary>
    internal static class NrxXmlUtil
    {
        /// <summary>
        /// Number format can contain chars 0x00 - 0x08 which designate placeholders for level numbers.
        /// They should be converted to %1 - %9 for WordML and DOCX.
        /// </summary>
        internal static string NumberFormatToXml(string numberFormat)
        {
            for (int i = ListLevel.MinLevel; i < ListLevel.MaxLevels; i++)
                numberFormat = numberFormat.Replace(((char)i).ToString(), ListLevelToXml(i));

            return numberFormat;
        }

        /// <summary>
        /// Described in ECMA TC 45: 2.13.5.30 numberingChange (Previous Paragraph Numbering Properties).
        /// Example: "%1:3:0:-%2:1:0:%3:1:4:.".
        /// </summary>
        internal static string GetOriginal(ParagraphNumberRevision numberRevision)
        {
            string numFormat = numberRevision.NumberFormat;
            int[] numValues = numberRevision.NumberValues;
            NumberStyle[] numStyles = numberRevision.NumberStyles;

            // RK Creating string build each time is not a very good idea.
            // To avoid this, should be an instance method on revision writer or something like that.
            StringBuilder strBuilder = new StringBuilder();

            int numFormatIndex = 0;
            int valueIndex = 0;

            while (numFormatIndex < numFormat.Length)
            {
                strBuilder.Append(ListLevelToXml(numFormat[numFormatIndex++]));
                strBuilder.Append(":");
                strBuilder.Append(FormatterPal.IntToXml(numValues[valueIndex]));
                strBuilder.Append(":");
                strBuilder.Append(FormatterPal.IntToXml((int)numStyles[valueIndex]));
                strBuilder.Append(":");

                if (numFormatIndex < numFormat.Length)
                {
                    char separator = numFormat[numFormatIndex];

                    if (separator > '\x0008')
                    {
                        strBuilder.Append(separator);
                        numFormatIndex++;
                    }
                }

                valueIndex++;
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Converts list level, which is int (0-8) in the model, to WordML representation ("%1"-"%9").
        /// </summary>
        internal static string ListLevelToXml(int value)
        {
            return String.Format("%{0}", value + 1);
        }

        /// <summary>
        /// Converts WordML representation ("%1"-"%9") to list level, which is int (0-8) in the model.
        /// </summary>
        internal static string XmlToListLevel(string numberFormat)
        {
            // Number format contains chars 0x00 - 0x08 which designate placeholders for level numbers.
            // They are represented as %1 - %9 in WordML.
            for (int i = 0; i < 10; i++)
                numberFormat = numberFormat.Replace("%" + (i + 1), ((char)i).ToString());

            return numberFormat;
        }

        /// <summary>
        /// Converts array of integers to WordML comma delimited string.
        /// </summary>
        /// <param name="values">Array of integers.</param>
        /// <returns>WordML comm delimited string representation of array.</returns>
        internal static string IntArrayToXml(int[] values)
        {
            // RK Creating string build each time is not a very good idea.
            // Used only by VmlDiagramWriter, so should be moved there and made an instance method.
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                sb.Append(FormatterPal.IntToXml(values[i]));
                sb.Append(',');
            }

            // remove last comma
            if (sb.Length > 0)
                sb.Length--;

            return sb.ToString();
        }

        /// <summary>
        /// Converts integer stored in byte array (4 bytes, from less significant to most) to string, e.g. integer value 0x01020304,
        /// stored as byte[] {0x04, 0x03, 0x02, 0x01}, is convereted to "01020304".
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <param name="startIndex">The index of the first byte of the converted integer value.</param>
        /// <returns>String representation of the integer stored in byte array.</returns>
        internal static string IntToHex(byte[] bytes, int startIndex)
        {
            return StringUtil.BytesToHex(bytes, startIndex, 4, true);
        }

        /// <summary>
        /// Converts Int32 to WordMl hex string, e.g. integer value 0x01020304 is convereted to "01020304".
        /// </summary>
        /// <param name="value">Int32 to convert.</param>
        /// <returns>WordML hex string representing integer value.</returns>
        internal static string IntToHex(int value)
        {
            return FormatterPal.IntToStrX8(value);
        }

        /// <summary>
        /// Converts Int32 to WordMl hex string, e.g. integer value 0x01020304 is converted to "01020304".
        /// </summary>
        /// <param name="value">Object to convert. Must be Int32. If not Int32 or null, returns null.</param>
        /// <returns>WordML hex string representing integer value. If not Int32 or null, returns null.</returns>
        internal static string IntToHex(object value)
        {
            if (value is int)
                return FormatterPal.IntToStrX8((int)value);
            else
                return null;
        }

        /// <summary>
        /// Try parses strings containing integers in hexadecimal representation.
        /// on error return int.MinValue
        /// </summary>
        internal static int TryHexToInt(string value)
        {
            return FormatterPal.TryParseHex(value);
        }

        /// <summary>
        /// Parses strings containing integers in hexadecimal representation.
        /// </summary>
        internal static int HexToInt(string value)
        {
            return FormatterPal.ParseHex(value);
        }

        /// <summary>
        /// Parses strings containing integers in hexadecimal representation,
        /// taking into account only 8 characters from right and discarding all others.
        /// </summary>
        internal static int Hex8ToInt(string value)
        {
            // ISO/IEC 29500-1, 17.18.50  ST_LongHexNumber.
            // This simple type's contents have a length of exactly 8 hexadecimal digit(s).
            const int maxHexLength = 8;

            if (value.Length > maxHexLength)
                value = value.Substring(value.Length - maxHexLength);

            return HexToInt(value);
        }

        /// <summary>
        /// Converts hex string into the byte array.
        /// Resulting array length will be equal to the specified desired length.
        ///
        /// RK I think name does not match the name of the corresponding opposite method.
        /// Should be XmlToBytes probably.
        /// </summary>
        internal static byte[] HexToBytes(string hex, int desiredLength)
        {
            byte[] array = StringUtil.HexToBytes(hex);
            if (array.Length == desiredLength)
                return array;

            byte[] newArray = new byte[desiredLength];
            Array.Copy(array, newArray, System.Math.Min(desiredLength, array.Length));

            return newArray;
        }

        /// <summary>
        /// RK I think name does not match the name of the corresponding opposite method.
        /// Should be XmlToBytes probably.
        ///
        /// RK BytesToHex accepts a boolean flag IsReversed but HexToBytes is split into two methods,
        /// making it inconsistent. Either use a boolean flag here and there or split into two methods.
        /// </summary>
        internal static void HexToBytesReversed(string hex, byte[] bytes, int start)
        {
            // RK Bad practice. Start was meaning one thing, now it means another thing.
            // Should be a different local variable with an apprpriate name.
            start = start + hex.Length / 2 - 1;
            for (int i = 0; i < hex.Length; i += 2)
                bytes[start--] = (byte)FormatterPal.ParseHex(hex.Substring(i, 2));
        }

        /// <summary>
        /// Converts color to WordML color string ("auto", "FF00C0", etc.).
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <returns>WordML string representing color.</returns>
        internal static string ColorToXml(DrColor color)
        {
            if (color.IsEmpty)
                return "auto";
            else
                return FormatterPal.IntToStrX8(color.ToArgb()).Substring(2);    // RK I don't like this hack of removing "0x" at the beginning.
        }

        /// <summary>
        /// Parses WordML color strings, like "auto", "FF00C0", etc.
        /// </summary>
        internal static DrColor XmlToColor(string value)
        {
            value = value.ToLower();

            if (value == "auto")
                return DrColor.Empty;

            // RESILIENCY WORDSNET-21462 The problem occurred because there was '#' at the beginning of color code string.
            // Color code parser did not recognize such code as valid.
            // Fixed by trimming '#' character from the color code string.
            value = value.Trim(gColorTrimChars);

            if (IsHexColorString(value))
                return HexColorStringToColor(value);

            return DrKnownColors.FromName(value);
        }

        /// <summary>
        /// Converts color string, like "A90045", to color.
        /// </summary>
        private static DrColor HexColorStringToColor(string value)
        {
            int xmlColor = HexToInt(value);
            int r = (xmlColor & 0x00ff0000) >> 16;
            int g = (xmlColor & 0x0000ff00) >> 8;
            int b = (xmlColor & 0x000000ff);
            return new DrColor(r, g, b);
        }

        /// <summary>
        /// Checks if hex digit is a color string, like "A90045"
        /// </summary>
        private static bool IsHexColorString(string value)
        {
            if (value.Length != 6)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!StringUtil.IsHexDigit(value[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Converts numeric value of version of MS Word application to XML string representation.
        /// </summary>
        internal static string VersionNumberToXml(int versionNumber)
        {
            // Generate version number string in X.YYYY format.
            // High 16 bit value contains version number X,
            // low 16 bit value contains build number YYYY.
            int versionHigh = versionNumber >> 16;
            int versionLow = versionNumber % 65536;

            string version = FormatterPal.IntToXml(versionHigh);

            version = String.Format("{0}.{1}", version, FormatterPal.IntToStrD4(versionLow));

            return version;
        }

        /// <summary>
        /// Converts XML string representation of version to a numeric value.
        /// </summary>
        internal static int XmlToVersionNumber(string version)
        {
            int versionNumber = 0;

            if (StringUtil.HasChars(version))
            {
                // This comes as something like "11.5604" which must be translated to high and low 16 bit values.
                // It can also be "12" with no dot separator and sub-version digits, so be careful.

                string[] versionParts = version.Split('.');
                int high = FormatterPal.XmlToInt(versionParts[0]);
                int low = 0;

                if (versionParts.Length > 1)
                    low = FormatterPal.XmlToInt(versionParts[1]);

                versionNumber = ((high << 16) | low);
            }

            return versionNumber;
        }

        internal static string BytesToBase64Attribute(byte[] value)
        {
            return Base64Splitter.Convert(value, "\x000a");
        }

        /// <summary>
        /// Tries to extract an ordinal from strings like "_x0000_s1096" and "#_x0000_t75".
        ///
        /// Collects all digits from the end of the string until encounters a non digit character and
        /// returns that as a number.
        /// </summary>
        internal static int TryParseId(string id)
        {
            if (!StringUtil.HasChars(id))
                return Int32.MinValue;

            // WORDSNET-12060 Id string has value with decimal point.
            int startIndex = id.Length - 1;
            while ((startIndex >= 0) && (((id[startIndex] >= '0') && (id[startIndex] <= '9')) || id[startIndex] == '.'))
                startIndex--;

            return FormatterPal.TryParseIntPortion(id.Substring(startIndex + 1));
        }

        /// <summary>
        /// Sets font name to RunPr if given name has chars.
        /// </summary>
        /// <param name="runPr"></param>
        /// <param name="key"></param>
        /// <param name="name">Either simple font name or theme font name.</param>
        /// <param name="isTheme">The meaning of name. If true name is simple font name otherwise name is theme font name.</param>
        internal static void SetFontNameIfHasChars(RunPr runPr, int key, string name, bool isTheme)
        {
            if (StringUtil.HasChars(name))
            {
                ComplexFontName fontName = isTheme
                                               ? ComplexFontName.FromTheme(NrxRunEnum.XmlToThemeFont(name))
                                               : ComplexFontName.FromName(name);

                // WORDSNET-22517 If we set theme font, but it is actually not a theme font (for example, theme
                // font name is invalid), then we should ignore this name and take it from the global defaults.
                if (isTheme && !fontName.IsThemeFont)
                {
                    // WORDSNET-28435 Do not override font that already set.
                    if (!runPr.Contains(key))
                    {
                        fontName = (ComplexFontName)runPr.FetchInheritedAttr(key);
                        runPr.SetAttr(key, fontName);
                    }
                }
                else
                    runPr.SetAttr(key, fontName);
            }
        }

        /// <summary>
        /// Checks that value is valid OnOff value.
        /// </summary>
        internal static bool IsOnOffValue(string value)
        {
            return ((value == "on") || (value == "1") || (value == "true") || (value == "t") ||
                    (value == "off") || (value == "0") || (value == "false") || (value == "f"));
        }

        /// <summary>
        /// Converts XML border name to CellAttr key.
        /// </summary>
        internal static int BorderNameToCellAttr(string border)
        {
            switch (border)
            {
                case "top":
                    return CellAttr.BorderTop;

                case "bottom":
                    return CellAttr.BorderBottom;

                case "left":
                case "start":
                    return CellAttr.BorderLeft;

                case "right":
                case "end":
                    return CellAttr.BorderRight;

                case "tl2br":
                    return CellAttr.BorderDiagonalDown;

                case "tr2bl":
                    return CellAttr.BorderDiagonalUp;

                case "insideH":
                    return CellAttr.BorderHorizontal;

                case "insideV":
                    return CellAttr.BorderVertical;
                default:
                    break;
            }

            throw new InvalidOperationException("Invalid border name.");
        }

        /// <summary>
        /// Converts source units into points, and then convert points to target units.
        /// </summary>
        internal static double ConvertMeasure(double sourceValue, NrxUnit sourceUnit, NrxUnit targetUnit)
        {
            double result = sourceValue;
            if (sourceUnit != targetUnit) // only convert to points if values are not in desired target units
            {
                double inPoints = sourceValue / NrxUnitToMultiplier(sourceUnit);
                result = inPoints * NrxUnitToMultiplier(targetUnit);
            }
            return result;
        }

        /// <summary>
        /// Converts desired unit into multiplier that is required to obtain this unit from point.
        /// </summary>
        private static double NrxUnitToMultiplier(NrxUnit unit)
        {
            switch (unit)
            {
                case NrxUnit.Inch:
                    return 1 / ConvertUtilCore.PointsPerInch;
                case NrxUnit.Millimeter:
                    return 1 / ConvertUtilCore.PointsPerMm;
                case NrxUnit.Centimeter:
                    return 1 / ConvertUtilCore.PointsPerCm;
                case NrxUnit.Pica:
                    return 1d / 12d; // 1 pica = 12 points.
                case NrxUnit.Point:
                    return 1;
                case NrxUnit.HalfPoints:
                    return 2;
                case NrxUnit.Twips:
                    return 20;
                case NrxUnit.Emus:
                    return ConvertUtilCore.EmusPerPoint;
                case NrxUnit.HundredthsOfPoint:
                    return 100;
                default:
                    throw new InvalidOperationException("Unsupported units.");
            }
        }

        internal static string GetShapeId(ShapeBase shape)
        {
            return String.Format("_x0000_{0}{1}",
                (shape.IsInline) ? "i" : "s",
                FormatterPal.IntToStrD4(shape.Id));
        }

        private static readonly char[] gColorTrimChars = { '#' };
    }
}
