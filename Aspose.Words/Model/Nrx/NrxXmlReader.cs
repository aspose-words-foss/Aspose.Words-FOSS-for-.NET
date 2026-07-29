// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2007 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Styles;
using Aspose.Words.Tables;
using Aspose.Xml;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Provides methods useful for reading MS Word XML files (DOCX and WordML).
    /// Put only MS Word specific methods here.
    /// </summary>
    internal class NrxXmlReader : AnyXmlReader
    {
        /// <summary>
        /// Ctor to read from a stream.
        /// </summary>
        internal NrxXmlReader(Stream stream)
            : this(stream, null, WarningSource.Nrx)
        {
        }

        /// <summary>
        /// Ctor to read from a stream and specifies WarningCallback.
        /// </summary>
        internal NrxXmlReader(Stream stream, IWarningCallback warningCallback, WarningSource warningSource)
            : base(stream)
        {
            mWarningCallback = warningCallback;
            mWarningSource = warningSource;
        }

        /// <summary>
        /// Ctor to read from a string, possibly a fragment.
        /// </summary>
        /// <param name="xml">This can be an XML document or fragment.</param>
        /// <param name="namespaces">Key is prefix, value is namespace uri. Needed to properly load
        /// fragments that contain elements with prefixes. Optional, can be null.</param>
        internal NrxXmlReader(string xml, IDictionary<string, string> namespaces)
            : this(xml, namespaces, null, WarningSource.Nrx)
        {
        }

        /// <summary>
        /// Ctor to read from a string, possibly a fragment and register warnings.
        /// </summary>
        /// <param name="xml">This can be an XML document or fragment.</param>
        /// <param name="namespaces">Key is prefix, value is namespace uri.</param>
        /// <param name="warningCallback">"Callback" for register warning.</param>
        internal NrxXmlReader(string xml, Dictionary<string, string> namespaces, IWarningCallback warningCallback)
            : this(xml, namespaces, warningCallback, WarningSource.Nrx)
        {

        }

        /// <summary>
        /// Ctor to read from a string, possibly a fragment and specifies WarningCallback.
        /// </summary>
        private NrxXmlReader(string xml, IDictionary<string, string> namespaces, IWarningCallback warningCallback, WarningSource warningSource)
            : base(xml, namespaces)
        {
            mWarningCallback = warningCallback;
            mWarningSource = warningSource;
        }

        /// <summary>
        /// Skips the current element content and positions reader to the element's end.
        /// Reader should already be positioned on the specified element
        /// (or on an attribute of the element).
        ///
        /// This method is different from XmlTextReader.Skip in the way it positions the cursor.
        /// Skip positions to start of next element, but this method positions to the end of the skipped element.
        /// This is needed because we read elements using ReadChild and it will read next node when called.
        ///
        /// Warning will be produced to the user-provided warning callback.
        /// </summary>
        [JavaThrows(false)]
        public override void IgnoreElement()
        {
            // Create warning message.
            string message = mWarningCallback != null
                ? String.Format("Import of element '{0}' is not supported in {1} format by Aspose.Words.", LocalName,
                    WarningInfo.WarningSourceToString(mWarningSource))
                : null;
            IgnoreElement(WarningType.MinorFormattingLoss, mWarningSource, message);
        }

        /// <summary>
        /// Ignore element and warn user with specified warning information.
        /// </summary>
        /// <param name="warningType">Type of a warning that is issued by Aspose.Words during document loading.</param>
        /// <param name="warningSource">Module that produces a warning during document loading.</param>
        /// <param name="description">Warning description.</param>
        [JavaThrows(false)]
        internal void IgnoreElement(WarningType warningType, WarningSource warningSource, string description)
        {
            base.IgnoreElement();
            Warn(warningType, warningSource, description);
        }

        internal void IgnoreElementUnexpected(bool isDocx)
        {
            base.IgnoreElement();
            Warn(WarningType.UnexpectedContent, isDocx ? WarningSource.Docx : WarningSource.WordML,
                mWarningCallback != null ? string.Format(WarningStrings.UnexpectedElement, LocalName) : null);
        }

        /// <summary>
        /// Ignore element and do no generate warning.
        /// </summary>
        internal void IgnoreElementNoWarn()
        {
            base.IgnoreElement();
        }

        /// <summary>
        /// Logs a warning of unexpected attribute to the user-provided warning callback.
        /// </summary>
        private void WarnOfUnexpectedAttribute()
        {
            Warn(WarningType.UnexpectedContent, WarningSource.Nrx,
                string.Format(WarningStrings.UnexpectedAttribute, LocalName));
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        internal void Warn(WarningType warningType, WarningSource warningSource, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(warningType, warningSource, description));
        }

        /// <summary>
        /// Indicates that currently begin read element namespace is well known.
        /// </summary>
        /// <remarks>
        /// Used to distinguish between Microsoft document markup and custom xml markup.
        /// </remarks>
        internal virtual bool IsWordNamespace
        {
            get { throw new InvalidOperationException("Abstract implementation is called."); }
        }

        /// <summary>
        /// Reads the 'id' attribute of the current element.
        /// When the attribute is missing returns null.
        /// </summary>
        internal string ReadId()
        {
            return ReadAttribute("id", null);
        }

        /// <summary>
        /// Reads the 'val' attribute of the current element.
        /// When the attribute is missing returns null.
        /// </summary>
        internal string ReadVal()
        {
            return ReadAttribute("val", null);
        }

        /// <summary>
        /// Reads a 'val' attribute of the integer type.
        /// When the 'val' attribute is missing, returns 0.
        /// </summary>
        internal int ReadIntVal()
        {
            string val = ReadVal();

            if (StringUtil.HasChars(val))
                return FormatterPal.XmlToInt(val);
            else
                return 0;
        }

        /// <summary>
        /// Reads a 'val' attribute of the onOffType.
        /// When the 'val' attribute is missing, returns true.
        /// </summary>
        internal bool ReadBoolVal()
        {
            string val = ReadVal();

            if (StringUtil.HasChars(val))
                return XmlToBool(val);
            else
                return true;
        }

        /// <summary>
        /// Reads the 'val' attribute of the current element.
        /// If the attribute is not found returns the default value.
        /// </summary>
        internal string ReadAttribute(string defaultValue)
        {
            return ReadAttribute("val", defaultValue);
        }

        /// <summary>
        /// Reads a 'val' attribute of the integer type.
        /// If the attribute is not found returns the default value.
        /// </summary>
        internal int ReadIntAttribute(int defaultValue)
        {
            return ReadIntAttribute("val", defaultValue);
        }

        /// <summary>
        /// Reads a 'val' attribute of the double type.
        /// If the attribute is not found returns the default value.
        /// </summary>
        internal double ReadDoubleAttribute(double defaultValue)
        {
            return ReadDoubleAttribute("val", defaultValue);
        }

        /// <summary>
        /// Reads a 'val' attribute of the onOffType.
        /// If the attribute is not found returns the default value.
        /// </summary>
        internal bool ReadBoolAttribute(bool defaultValue)
        {
            return ReadBoolAttribute("val", defaultValue);
        }

        /// <summary>
        /// Reads a 'val' attribute of the onOffType for an attribute that can be on/off or toggle.
        /// When the 'val' attribute is missing, return true.
        /// </summary>
        internal AttrBoolEx ReadBoolExVal()
        {
            string val = ReadVal();

            if (StringUtil.HasChars(val))
                return AttrBoolEx.FromBool(XmlToBool(val));
            else
                return AttrBoolEx.True;
        }

        /// <summary>
        /// Reads contents of an element as a boolean "true", "t" or "false", "f".
        /// For other values the default value is returned.
        /// </summary>
        internal bool ReadBoolString(bool defaultValue)
        {
            switch (ReadString())
            {
                case "true":
                case "t":
                    return true;
                case "false":
                case "f":
                    return false;
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// Reads a w:sym element. Returns the character read and also sets SymbolFonts in the specified runPr.
        /// </summary>
        internal char ReadSymbol(RunPr runPr)
        {
            // MS Word considers document to be invalid if 'w:font' or 'w:char' attributes not set.
            // We are resilient and will just import default values.
            char sym = '\uf0ff'; // 'empty square' sign
            string font = "Symbol";

            while (MoveToNextAttribute())
            {
                switch (LocalName)
                {
                    case "font":
                        font = Value;
                        break;
                    case "char":
                        sym = (char)NrxXmlUtil.HexToInt(Value);
                        break;
                    default:
                        WarnOfUnexpectedAttribute();
                        break;
                }
            }

            runPr.SetSymbolFonts(font);

            return sym;
        }

        internal Border ReadBorder()
        {
            Border border = new Border();
            bool isAuto = true;

            while (MoveToNextAttribute())
            {
                switch (LocalName)
                {
                    case "val":
                        // This allows us to distinguish "nil" from "none" string because in
                        // the line style we only have None. I don't want Nil in the line style.
                        if (Value == "nil")
                            return Border.CreateNilBorder();

                        border.LineStyleInternal = ReadLineStyle();
                        break;
                    case "sz":
                        border.RawLineWidth = ValueAsInt;
                        break;
                    case "space":
                        border.RawDistanceFromText = ValueAsInt;
                        break;
                    case "color":
                        isAuto = (Value == "auto");
                        border.ColorInternal = NrxXmlUtil.XmlToColor(Value);
                        break;
                    case "shadow":
                        border.Shadow = ValueAsBool;
                        break;
                    case "frame":
                        border.Frame = ValueAsBool;
                        break;
                    case "themeColor":
                        border.ThemeColorInternal = Value;
                        break;
                    case "themeShade":
                        border.ThemeShade = Value;
                        break;
                    case "themeTint":
                        border.ThemeTint = Value;
                        break;
                    case "bdrwidth": // Not supported attribute of WML 2003 format
                        Warn(WarningType.MinorFormattingLoss, WarningSource.Nrx,
                            string.Format(WarningStrings.NotSupportedAttribute, LocalName));
                        break;
                    default:
                        WarnOfUnexpectedAttribute();
                        break;
                }
            }

            if (border.LineStyle == LineStyle.None && !isAuto)
            {
                // Cell inherits table border only if color is 'auto' and both space and sz is 0. See TestJira8023A for example.
                border.IsNil = true;
            }

            return border;
        }

        internal Shading ReadShading()
        {
            Shading shading = new Shading();

            while (MoveToNextAttribute())
            {
                switch (LocalName)
                {
                    case "val":
                        shading.Texture = StyleConvertUtil.XmlToTextureIndex(Value);
                        break;
                    case "color":
                        shading.ForegroundPatternColorInternal = NrxXmlUtil.XmlToColor(Value);
                        break;
                    case "fill":
                        shading.BackgroundPatternColorInternal = NrxXmlUtil.XmlToColor(Value);
                        break;
                    case "themeColor":
                        shading.ThemeColor = Value;
                        break;
                    case "themeShade":
                        shading.ThemeShade = Value;
                        break;
                    case "themeTint":
                        shading.ThemeTint = Value;
                        break;
                    case "themeFill":
                        shading.ThemeFill = Value;
                        break;
                    case "themeFillShade":
                        shading.ThemeFillShade = Value;
                        break;
                    case "themeFillTint":
                        shading.ThemeFillTint = Value;
                        break;
                    default:
                        WarnOfUnexpectedAttribute();
                        break;
                }
            }

            return shading;
        }

        /// <summary>
        /// Reads length value in twips.
        /// </summary>
        internal int ReadLengthInTwips(OoxmlComplianceInfo cInfo)
        {
            return ReadLength(cInfo).ValueRaw;
        }

        /// <summary>
        /// Reads length element.
        /// </summary>
        internal PreferredWidth ReadLength(OoxmlComplianceInfo cInfo)
        {
            // If w:w attribute is not specified MS Word defaults it to "0".
            int w = 0;

            // If w:type attribute is not specified and if w:w attribute has no unit MS Word defaults it to "auto".
            PreferredWidthType type = PreferredWidthType.Auto;

            bool isPercentage = false;
            bool isUniversalMeasure = false;
            bool hasType = false;

            while (MoveToNextAttribute())
            {
                switch (LocalName)
                {
                    case "w":
                        {
                            isPercentage = IsPercentage(Value);
                            isUniversalMeasure = !isPercentage && IsUniversalMeasure(Value);
                            if (isUniversalMeasure)
                            {
                                // Conversion to twips
                                w = GetValueAsTwips(cInfo);
                                if (cInfo != null)
                                    cInfo.MarkAsIsoTransitional();
                            }
                            else if (isPercentage)
                            {
                                // Conversion to 50ths of percent
                                string num = Value.Substring(0, Value.Length - 1);
                                w = MathUtil.DoubleToInt(System.Math.Floor(FormatterPal.ParseDouble(num)) * PreferredWidth.PercentFactor);
                                if (cInfo != null)
                                    cInfo.MarkAsIsoTransitional();
                            }
                            else
                            {
                                w = FormatterPal.XmlToShortInt(Value);
                            }
                            break;
                        }
                    case "type":
                        type = StyleConvertUtil.XmlToLengthType(Value);
                        hasType = true;
                        break;
                    default:
                        WarnOfUnexpectedAttribute();
                        break;
                }
            }

            // If value of the 'type' attribute and actual measurement specified by the 'w' attribute are
            // contradictory, the type specified by the 'type' attribute shall be ignored.
            // MS Word uses the 'pt' unit in values of the 'w' attribute when the type attribute is 'auto'.
            // So, correct the type if needed.
            // WORDSNET-13442 In case when length value contains universal measure unit and type of measure unit is "nil"
            // then length can not be ignored, so type "nil"(0) updated to points (after re-savings MSW store length in points).
            // WORDSNET-19279 If the type is missing, then the type is a points.
            if (isPercentage)
                type = PreferredWidthType.Percent;
            else if (!hasType || (isUniversalMeasure && ((type == PreferredWidthType.Percent) || (type == 0))))
                type = PreferredWidthType.Points;

            return PreferredWidth.FromRawSafe(type, w);
        }

        /// <summary>
        /// Reads value as universal measure from XML and converts it into Half-points.
        /// </summary>
        internal int ReadValAsHalfPoints(OoxmlComplianceInfo complianceInfo)
        {
            return MathUtil.DoubleToInt(ConvertUniversalMeasure(ReadVal(), NrxUnit.HalfPoints, complianceInfo));
        }

        /// <summary>
        /// Reads value as universal measure from XML and converts it into twips.
        /// </summary>
        internal int ReadValAsTwips(OoxmlComplianceInfo complianceInfo)
        {
            return MathUtil.DoubleToInt(ConvertUniversalMeasure(ReadVal(), NrxUnit.Twips, complianceInfo));
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name as universal measure.
        /// Converts value to English Metric Units.
        /// Returns the default value, if the attribute is not found.
        /// </summary>
        public double ReadAttributeAsEmus(string localName, double defaultValue, OoxmlComplianceInfo complianceInfo)
        {
            string strValue = ReadAttribute(localName, null);
            double result = TryConvertUniversalMeasureToEmus(strValue, complianceInfo);
            return (!Double.IsNaN(result)) ? result : defaultValue;
        }

        /// <summary>
        /// Exposed mainly for testing. Not recommended for use.
        /// Instead use <see cref="ReadAttributeAsEmus"/>, <see cref="ReadValAsTwips"/> and <see cref="ReadValAsHalfPoints"/>
        /// to read measures from OOXML data.
        /// </summary>
        /// <remarks>WORDSNET-10435 Returns double instead of float, as it can be a very large values.</remarks>
        internal static double ConvertUniversalMeasure(string value, NrxUnit targetUnit,
            OoxmlComplianceInfo complianceInfo)
        {
            double result = TryConvertUniversalMeasure(value, targetUnit, complianceInfo);
            return (!Double.IsNaN(result)) ? result : 0;
        }

        internal static double ConvertUniversalMeasure(string p, NrxUnit nrxUnit)
        {
            return ConvertUniversalMeasure(p, nrxUnit, null);
        }

        /// <summary>
        /// Tries to convert input string that may contain number or ST_UniversalMeasure to target units.
        /// If units are not specified in the input string, <paramref name="targetUnit"/> are expected.
        /// If the input string contains wrong value, Double.Nan is returned.
        /// </summary>
        /// <param name="value">Source string value.</param>
        /// <param name="targetUnit">Unit to which result value will be converted.</param>
        /// <param name="complianceInfo">Allows setting compliance info about the document.</param>
        /// <returns>Result value in <paramref name="targetUnit"/> or Double.NaN if the conversion has failed.</returns>
        private static double TryConvertUniversalMeasure(string value, NrxUnit targetUnit,
            OoxmlComplianceInfo complianceInfo)
        {
            double result = Double.NaN;
            if (StringUtil.HasChars(value))
            {
                // The index of a character where number part ends and units part starts,
                // that is index of the last character of the unit (which is before digit).
                int splitIndex;
                for (splitIndex = value.Length - 1; splitIndex >= 0; splitIndex--)
                {
                    if (value[splitIndex] >= '0' && value[splitIndex] <= '9')
                        break;
                }

                string numericPart = value.Substring(0, splitIndex + 1);
                if (!StringUtil.HasChars(numericPart))
                    return Double.NaN;

                bool valueHasUnitsPart = (splitIndex < value.Length - 1);

                double numericVal = FormatterPal.TryParseDoubleInvariant(numericPart);
                if (Double.IsNaN(numericVal))
                    return Double.NaN;

                // WORDSNET-11357 Should truncate values that have not units part.
                if (!valueHasUnitsPart)
                    numericVal = MathUtil.CastDoubleToInt(numericVal);

                NrxUnit sourceUnit = UnitFromString(
                    value.Substring(splitIndex + 1, value.Length - splitIndex - 1), targetUnit);

                // WORDSNET-25829 Read zero value in case of invalid measurement.
                if (sourceUnit == NrxUnit.None)
                    result = 0;
                else
                {
                    result = NrxXmlUtil.ConvertMeasure(numericVal, sourceUnit, targetUnit);

                    if (valueHasUnitsPart && complianceInfo != null)
                        complianceInfo.MarkAsIsoTransitional();
                }
            }
            return result;
        }

        /// <summary>
        /// Tries to convert input string that may contain number or ST_UniversalMeasure to English Metric Units.
        /// If units are not specified in the input string, English Metric Units are expected. If the input string
        /// contains wrong value, Double.Nan is returned.
        /// </summary>
        /// <param name="value">Source string value.</param>
        /// <param name="complianceInfo">Allows setting compliance info about the document.</param>
        /// <returns>Result value in EMUs or Double.NaN if the conversion has failed.</returns>
        internal static double TryConvertUniversalMeasureToEmus(string value, OoxmlComplianceInfo complianceInfo)
        {
            return TryConvertUniversalMeasure(value, NrxUnit.Emus, complianceInfo);
        }

        /// <summary>
        /// Handles both decimal and percentage data types.
        /// This function returns int or rounded percent value as int.
        /// </summary>
        /// <remarks>
        /// Iso29500 introduces several types where a value can be percent value:
        /// ST_DecimalNumberOrPercent (this includes decimal number or percent),
        /// ST_Percentage, ST_FixedPercentage, ST_PositiveFixedPercentage.
        /// Note that these types stores values in 1000ths of percent if the percent sign is not specified. This method
        /// cannot be used for the types.
        /// The format, which is supported by this method, is like in <see cref="FormatterPal.ParseDouble"/> but
        /// followed by "%" sign.
        /// In AW model XmlToInt was used for ECMA-376 for lots of attributes into Model.
        /// Because of that AW DOM contains lots of int-valued attributes, whereas spec now allows two
        /// digits after floating point.
        /// We will still return rounded int, thus bringing small round-off error into the numeric data.
        /// </remarks>
        internal static int XmlToPercent(string val, OoxmlComplianceInfo cInfo)
        {
            if (IsPercentage(val))
            {
                if (cInfo != null)
                    cInfo.MarkAsIsoTransitional();

                return MathUtil.DoubleToInt(FormatterPal.ParseDouble(val.Substring(0, val.Length - 1)));
            }
            else
            {
                return FormatterPal.XmlToInt(val);
            }
        }

        /// <summary>
        /// Reads cell spacing. Returns 'null' if unit type is 'nil' or missed.
        /// </summary>
        internal PreferredWidth ReadCellSpacing(OoxmlComplianceInfo cInfo)
        {
            return ReadAttribute("type", "nil") != "nil" ? ReadLength(cInfo) : null;
        }

        protected virtual LineStyle ReadLineStyle()
        {
            return LineStyle.None;
        }

        private static bool IsPercentage(string val)
        {
            return (val.Length >= 1) && (val[val.Length - 1] == '%');
        }

        /// <summary>
        /// Returns true if the input value meets the demands of the ST_UniversalMeasure simple type.
        /// </summary>
        internal static bool IsUniversalMeasure(string value)
        {
            if (!StringUtil.HasChars(value) || StringUtil.IsDigit(value[value.Length - 1]))
                return false;
            if (gUniversalMeasureRegex == null)
                gUniversalMeasureRegex = new Regex(@"-?[0-9]+(\.[0-9]+)?(mm|cm|in|pt|pc|pi)", RegexOptions.Compiled);
            return gUniversalMeasureRegex.IsMatch(value);
        }

        /// <summary>
        /// Convert source string units into enum <see cref="NrxUnit"/>
        /// </summary>
        private static NrxUnit UnitFromString(string unit, NrxUnit targetUnit)
        {
            NrxUnit result;
            switch (unit)
            {
                case "in":
                    result = NrxUnit.Inch;
                    break;
                case "pt":
                    result = NrxUnit.Point;
                    break;
                case "mm":
                    result = NrxUnit.Millimeter;
                    break;
                case "cm":
                    result = NrxUnit.Centimeter;
                    break;
                case "pc":
                case "pi":
                    result = NrxUnit.Pica;
                    break;
                case "":
                    // unitless values have target units.
                    result = targetUnit;
                    break;
                case "UL":
                case "\n":
                    result = NrxUnit.None;
                    break;
                default:
                    // we don't expect other units, so throw
                    throw new ArgumentOutOfRangeException(String.Format("Unknown universal measure {0}", unit));
            }

            return result;
        }

        /// <summary>
        /// This helps to properly build run nodes. In DOCX text of one run can consist from
        /// several elements such as "t", "tab" and so on. These all are joined here when possible.
        ///
        /// RK Used by DocxRunReader, should be moved there.
        /// </summary>
        internal StringBuilder RunTextBuilder
        {
            get { return mRunTextBuilder; }
        }


        /// <summary>
        /// RK Used in DocxParaChildReader, maybe should be moved there.
        /// Remembers created field start and separator nodes. Pops on field end.
        /// </summary>
        internal Stack<FieldChar> FieldNodesStack
        {
            get { return mFieldNodesStack; }
        }

        /// <summary>
        /// Used for DOCX only.
        /// RK Used only by DocxParaReader and DocxParaChildReader, so better be moved there.
        /// Should probably be refactored to stack to work correctly as paragraphs can be nested.
        /// But since RSIDs implementation in the model is incomplete anyway - I leave it as is for now.
        /// </summary>
        internal int RsidRDefault
        {
            get { return mRsidRDefault; }
            set { mRsidRDefault = value; }
        }

        /// <summary>
        /// Used for DOCX only.
        /// </summary>
        internal FormFieldPr CurrentFormFieldPr
        {
            get { return mFormFieldPrStack.Top(); }
        }

        /// <summary>
        /// Used for DOCX only.
        /// </summary>
        internal void PushFormFieldPr(FormFieldPr value)
        {
            mFormFieldPrStack.Push(value);
        }

        /// <summary>
        /// Used for DOCX only.
        /// </summary>
        internal void PopFormFieldPr()
        {
            mFormFieldPrStack.Pop();
        }

        /// <summary>
        /// Returns current attribute value as universal measure from XML and converts it into twips.
        /// </summary>
        internal int GetValueAsTwips(OoxmlComplianceInfo complianceInfo)
        {
            return MathUtil.DoubleToInt(ConvertUniversalMeasure(Value, NrxUnit.Twips, complianceInfo));
        }

        /// <summary>
        /// Returns current attribute value as universal measure from XML and converts it into twips.
        /// Result is adjusted to fit in a short type range.
        /// </summary>
        internal int GetValueAsTwipsAsShort(OoxmlComplianceInfo complianceInfo)
        {
            return MathUtil.DoubleToShort(ConvertUniversalMeasure(Value, NrxUnit.Twips, complianceInfo));
        }

        /// <summary>
        /// Returns current attribute value as universal measure from XML and converts it into hundredths of a point.
        /// </summary>
        internal int GetValueAs100thsOfPoint(OoxmlComplianceInfo complianceInfo)
        {
            return MathUtil.DoubleToInt(ConvertUniversalMeasure(Value, NrxUnit.HundredthsOfPoint, complianceInfo));
        }

        /// <summary>
        /// Returns current attribute value as universal measure from XML and converts it into EMUs.
        /// </summary>
        internal double GetValueAsEmus(OoxmlComplianceInfo complianceInfo)
        {
            return ConvertUniversalMeasure(Value, NrxUnit.Emus, complianceInfo);
        }

        public override bool ValueAsBool
        {
            get { return XmlToBool(Value); }
        }

        /// <summary>
        /// Last read paragraph or block level cross-structure annotation. Used to bind block-level custom xml.
        /// </summary>
        internal Node PossibleSdtEnd
        {
            get { return mPossibleSdtEnd; }
            set { mPossibleSdtEnd = value; }
        }

        /// <summary>
        /// Converts a value of an onOffType attribute into a boolean value.
        /// </summary>
        internal static bool XmlToBool(string value)
        {
            if ((value == "on") || (value == "1") || (value == "true") || (value == "t"))
                return true;

            if ((value == "off") || (value == "0") || (value == "false") || (value == "f"))
                return false;

            // WORDSNET-28064 Invalid bool attribute value.
            if ((value == "00"))
                return false;

            throw new InvalidOperationException(string.Format("Unknown value '{0}' for an 'onOffType' attribute.", value));
        }

        /// <summary>
        /// Warning callback.
        /// </summary>
        internal IWarningCallback WarningCallback
        {
            get { return mWarningCallback; }
        }

        /// <summary>
        /// The [Shape, FillSource] pairs for deferred binding.
        /// </summary>
        internal Dictionary<ShapeBase, string> FillSourceMap
        {
            get { return mFillSourceMap; }
        }

        /// <summary>
        /// Used for WML only.
        /// Memorizes fieldData encountered when parsing field definitions.
        /// When type of the field becomes known this field data is used to set various field properties.
        /// Right now it is actually used only when reading form fields. In all other cases it is simply discarded.
        /// </summary>
        internal string FieldData;

        private readonly StringBuilder mRunTextBuilder = new StringBuilder();
        private readonly Stack<FieldChar> mFieldNodesStack = new Stack<FieldChar>();
        private int mRsidRDefault;

        private Node mPossibleSdtEnd;
        private readonly IWarningCallback mWarningCallback;
        private readonly WarningSource mWarningSource;
        private readonly Stack<FormFieldPr> mFormFieldPrStack = new Stack<FormFieldPr>();
        private readonly Dictionary<ShapeBase, string> mFillSourceMap = new Dictionary<ShapeBase, string>();

        private static Regex gUniversalMeasureRegex;
    }
}
