// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2007 by Roman Korchagin
using System;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.Styles;
using Aspose.Words.Tables;
using Aspose.Xml;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// There seems to be a lot of similarities between how MS Word writes to HTML and WordML.
    /// This class is common code for writing to DOCX, WordML and HTML.
    /// </summary>
    internal class NrxXmlBuilder : AnyXmlBuilder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <param name="isPrettyFormat">True to use pretty formatting for the output XML.
        /// Note that when this is true, spaces could appear between spans, so use this for testing only when write Html.</param>
        /// <param name="useOnOff">If true, then "on/off" are written for boolean values, otherwise "true/false" are written.</param>
        public NrxXmlBuilder(Stream stream, Encoding encoding, bool isPrettyFormat, bool useOnOff)
            : base(stream, encoding, isPrettyFormat)
        {
            mUseOnOff = useOnOff;
        }

        /// <summary>
        /// Writes an element with value. Valid object types are bool, int, string, color.
        /// If value is null or not of valid type - the element is not written.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">Value object. Must be bool, int, string or color to be written.</param>
        internal void WriteVal(string elementName, object value)
        {
            // Do not write element if the value not set.
            if (value == null)
                return;

            if (value is bool)
            {
                // This MUST be on a separate line for boolean and integer for correct autoporting to Java.
                // Other cases are done in the same way just for consistency.
                bool boolValue = (bool)value;
                WriteVal(elementName, boolValue);
            }
            else if (value is string)
            {
                string stringValue = (string)value;
                WriteVal(elementName, stringValue);
            }
            else if (value is int)
            {
                int intValue = (int)value;
                WriteVal(elementName, intValue);
            }
            else if (value is DrColor)
            {
                DrColor colorValue = (DrColor)value;
                WriteVal(elementName, colorValue);
            }
            else
            {
                throw new InvalidOperationException("Unknown object type in the WriteVal method.");
            }
        }

        /// <summary>
        /// Writes element with the specified name and a 'val' attribute if provided value string is not null or empty.
        /// Prefix for 'val' is taken from element name.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">String value to write to element's 'val' attribute. Element is written only if the provided string value is not null or empty.</param>
        internal void WriteVal(string elementName, [CodePorting.Translator.Cs2Cpp.CppForceStringParam] string value)
        {
            if (StringUtil.HasChars(value))
            {
                StartElement(elementName);
                WriteAttributeString(GetPrefix(elementName) + "val", value);
                EndElement();
            }
        }

        /// <summary>
        /// Writes element with the specified name and a 'val' attribute if provided value string is not null. Prefix for 'val' is taken from element name. Empty strings are written.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">String value to write to element's 'val' attribute. Element is written only if the provided string value is not null. Empty strings are written.</param>
        internal void WriteValEvenIfEmpty(string elementName, string value)
        {
            if (value != null)
            {
                StartElement(elementName);
                WriteAttributeString(GetPrefix(elementName) + "val", value);
                EndElement();
            }
        }

        internal void WriteValRequired(string elementName, string value)
        {
            if (!StringUtil.HasChars(value))
                throw new InvalidOperationException("Attempted to write a null or empty value, but it is required according to the schema.");

            WriteVal(elementName, value);
        }

        /// <summary>
        /// Writes an element with the specified name and a color value written to 'val' attribute. Prefix for 'val' is taken from element name.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">Color value to write to element's 'val' attribute. Specified color is converted to WordML color string.</param>
        internal void WriteVal(string elementName, DrColor value)
        {
            WriteVal(elementName, NrxXmlUtil.ColorToXml(value));
        }

        /// <summary>
        /// Writes an element with the specified name and an integer value written to 'val' attribute. Prefix for 'val' is taken from element name.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">Integer value to write to element's 'val' attribute.</param>
        internal void WriteVal(string elementName, int value)
        {
            WriteVal(elementName, FormatterPal.IntToXml(value));
        }

        /// <summary>
        /// Writes an element with a 'val' attribute encoded as base64 if the value is not null or an empty array.
        /// </summary>
        internal void WriteVal(string elementName, byte[] value)
        {
            if ((value != null) && (value.Length > 0))
                WriteVal(elementName,  NrxXmlUtil.BytesToBase64Attribute(value));
        }

        /// <summary>
        /// Writes an element with the specified name and an integer value written to 'val' attribute. Element is written only if the provided value is greater than zero.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">Integer value to write to element's 'val' attribute. Element is written only if the provided value is greater than zero.</param>
        internal void WriteValIfPositive(string elementName, int value)
        {
            if (value > 0)
                WriteVal(elementName, FormatterPal.IntToXml(value));
        }

        /// <summary>
        /// Writes element with the specified name and "w:val" attribute with "off" value if specified boolean value is false
        /// and "on" value if specified boolean value is true.
        /// </summary>
        internal virtual void WriteBoolValExplicit(string elementName, bool value)
        {
            WriteVal(elementName, value ? "on" : "off");
        }

        /// <summary>
        /// Writes an element with the specified name and a boolean value written to 'val' attribute. Prefix for 'val' is taken from element name.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">Boolean value to write to element's 'val' attribute. Writes "off" if false. If true, element is written without the attribute.</param>
        internal virtual void WriteVal(string elementName, bool value)
        {
            if (value)
                WriteEmptyElement(elementName); // do not write "on", "on" is default
            else
                WriteVal(elementName, "off");
        }

        /// <summary>
        /// Writes an element with the specified name if the provided boolean value is true.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="value">If true, empty element is written. If false, nothing is written.</param>
        internal void WriteValIfTrue(string elementName, bool value)
        {
            if (value)
                WriteEmptyElement(elementName); // do not write "on", "on" is default
        }

        /// <summary>
        /// Writes an element with the "val" attribute if the value is not equal to the specified default value.
        /// Returns true if the value was written.
        /// </summary>
        internal void WriteValIfNotDefault(string elementName, int value, int defaultValue)
        {
            if (value != defaultValue)
                WriteVal(elementName, value);
        }

        /// <summary>
        /// Writes element with a specified attribute name/value list.
        /// At least one of the attributes should be non-empty to write the element.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="attrNamesValues">Name/value attribute list</param>
        internal void WriteElementWithAttributes(string elementName, params object[] attrNamesValues)
        {
            if (WriteElementWithAttributesStart(elementName, attrNamesValues))
                EndElement();
        }

        /// <summary>
        /// Writes element with a specified attribute name/value list.
        /// At least one of the attributes should be non-empty to write the element.
        /// </summary>
        /// <param name="elementName">The local name of the element.</param>
        /// <param name="attrNamesValues">Name/value attribute list</param>
        /// <returns>True, if element start was in fact written.</returns>
        internal bool WriteElementWithAttributesStart(string elementName, params object[] attrNamesValues)
        {
            int length = attrNamesValues.Length / 2;

            // check if non-empty attribute values present
            bool elementHasAttributes = false;

            for (int i = 0; i < length; i++)
            {
                object attrValue = attrNamesValues[i * 2 + 1];
                if (attrValue != null)
                {
                    if ( !( (attrValue is string) && ((string)attrValue == "") ) )
                    {
                        elementHasAttributes = true;
                        break;
                    }
                }
            }

            // return if no non-empty attributes found
            if (!elementHasAttributes)
                return false;

            StartElement(elementName);
            for (int i = 0; i < length; i++)
            {
                string attrName = (string)attrNamesValues[i * 2];
                object attrValue = attrNamesValues[i * 2 + 1];
                WriteAttribute(attrName, attrValue);
            }

            return true;
        }

        /// <summary>
        /// Writes attribute with object value. Object is converted to WordML string.
        /// Value object should have string, int, double, bool, Color or DateTime underlying type.
        ///
        /// RK I don't like this. It will be better to have separate methods for this.
        /// </summary>
        /// <remarks>
        /// NOTE: uint is int on Java. So we can't use writeAttribute() overload for uint
        /// since it conflicts with int on Java. Please use writeAttributeUInt() instead.
        /// </remarks>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">Object value for the attribute.</param>
        internal void WriteAttribute(string attributeName, object value)
        {
            // Do not write element if the value not set.
            if (value == null)
                return;

            if (value is string)
            {
                // These has to be on separate lines for autoporting to work correctly.
                string stringValue = (string)value;
                WriteAttribute(attributeName, stringValue);
            }
            else if (value is int)
            {
                int intValue = (int)value;
                WriteAttribute(attributeName, intValue);
            }
            else if (value is long)
            {
                long longValue = (long)value;
                WriteAttribute(attributeName, longValue);
            }
            else if (value is double)
            {
                double doubleValue = (double)value;
                WriteAttribute(attributeName, doubleValue);
            }
            else if (value is bool)
            {
                bool boolValue = (bool)value;
                WriteAttribute(attributeName, boolValue);
            }
            else if (value is BooleanConstant)
            {
                bool boolValue = ((BooleanConstant)value).ValueBoolean;
                WriteAttribute(attributeName, boolValue);
            }
            else if (value is DrColor)
            {
                DrColor colorValue = (DrColor)value;
                WriteAttribute(attributeName, NrxXmlUtil.ColorToXml(colorValue));
            }
            else if (value is DateTime)
            {
                DateTime dateValue = (DateTime)value;
                WriteAttribute(attributeName, dateValue);
            }
            else
            {
                throw new InvalidOperationException("Unknown object type in WriteAttribute method.");
            }
        }

        /// <summary>
        /// Writes attribute with unsigned integer value.
        /// </summary>
        /// <remarks>
        /// NOTE: uint is int on Java. So we can't use writeAttribute() overload for uint
        /// since it conflicted with int on Java.
        /// </remarks>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Unsigned integer value for the attribute.</param>
        internal void WriteAttributeUInt(string attributeName, object value)
        {
            // Do not write element if the value not set.
            if (value == null)
                return;

            uint uintValue = (uint)value;
            WriteAttributeUInt(attributeName, uintValue);
        }

        internal void WriteAttributeUInt(string attributeName, object value, object defaultValue)
        {
            // Do not write element if the value not set.
            if (value == null)
                value = defaultValue;

            WriteAttributeUInt(attributeName, value);
        }

        /// <summary>
        /// Writes attribute. If the value is null, writes the specified default value. Object is converted to WordML string.
        /// Value object should have string, int, double, bool or Color underlying type.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">Value for the attribute.</param>
        /// <param name="defaultValue">Default value for the attribute.</param>
        internal void WriteAttribute(string attributeName, object value, object defaultValue)
        {
            if (value == null)
                value = defaultValue;

            WriteAttribute(attributeName, value);
        }

        /// <summary>
        /// Writes attribute with string value.
        /// Attribute is not written if provided string value is null or empty.
        ///
        /// RK I don't like this. It is confusing with WriteAttributeString.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">String value for the attribute.</param>
        internal void WriteAttribute(string attributeName, [CodePorting.Translator.Cs2Cpp.CppForceStringParam] string value)
        {
            if (StringUtil.HasChars(value))
                WriteAttributeString(attributeName, value);
        }

        /// <summary>
        /// Writes attribute with string value. Attribute is not written if provided string value is null, empty or equals specified default value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">String value for the attribute.</param>
        /// <param name="defaultValue">Default value for the attribute.</param>
        internal void WriteAttributeIfNotDefault(string attributeName, string value, string defaultValue)
        {
            if (value != defaultValue)
                WriteAttribute(attributeName, value);
        }

        /// <summary>
        /// Writes attribute with color value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Color value for the attribute.</param>
        internal void WriteAttribute(string attributeName, DrColor value)
        {
            WriteAttributeString(attributeName, NrxXmlUtil.ColorToXml(value));
        }

        /// <summary>
        /// Writes attribute with integer value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Integer value for the attribute.</param>
        internal void WriteAttribute(string attributeName, int value)
        {
            WriteAttributeString(attributeName, FormatterPal.IntToXml(value));
        }

        /// <summary>
        /// Writes attribute with unsigned integer value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Unsigned integer value for the attribute.</param>
        internal void WriteAttributeUInt(string attributeName, uint value)
        {
            WriteAttributeString(attributeName, FormatterPal.UIntToXml(value));
        }

        /// <summary>
        /// Writes attribute with long value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Long value for the attribute.</param>
        internal void WriteAttribute(string attributeName, long value)
        {
            WriteAttributeString(attributeName, FormatterPal.LongToXml(value));
        }

        /// <summary>
        /// Writes an attribute as base64 encoded data using "&#xA;" as a separator (the way MS Word does).
        /// </summary>
        internal void WriteAttribute(string attributeName, byte[] value)
        {
            WriteAttributeString(attributeName, NrxXmlUtil.BytesToBase64Attribute(value));
        }

        /// <summary>
        /// Writes attribute with double value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Double value for the attribute.</param>
        internal void WriteAttribute(string attributeName, double value)
        {
            WriteAttributeString(attributeName, FormatterPal.DoubleToStr2Decimals(value));
        }

        /// <summary>
        /// Writes attribute with Guid value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Guid value for the attribute.</param>
        internal void WriteAttribute(string attributeName, Guid value)
        {
            // WORDSNET-10971 Format string "B" matches to the regular expression pattern
            // \{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\}
            // and corresponds to ST_Guid type of 5.1.12.27, ECMA-376, part 4.
            WriteAttributeString(attributeName, value.ToString("B"));
        }

        /// <summary>
        /// Writes attribute with DateTime value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">DateTime value for the attribute.</param>
        /// <remarks>
        /// If the value has <see cref="DateTimeKind.Local"/> kind, the value is converted to UTC before writing.
        /// If it has <see cref="DateTimeKind.Unspecified"/> kind, it is written as is but with the Z UTC postfix.
        /// MS Word uses different behavior for different datetime properties: some are written with conversion to UTC,
        /// others without conversion, but with the Z UTC postfix. The behavior of this method is determined by actual
        /// kind of the datetime value.
        /// </remarks>
        internal void WriteAttribute(string attributeName, DateTime value)
        {
            //WORDSJAVA-2406: if data-time is Local - convert it to UTC.
            if (value.Kind == DateTimeKind.Local)
                value = value.ToUniversalTime();

            WriteAttribute(attributeName, FormatterPal.DateTimeToXmlUtc(value));
        }

        /// <summary>
        /// Writes attribute with DateTime value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">DateTime value for the attribute.</param>
        /// <param name="targetKind">Behaviour of the method depends on kind of the datetime value. This parameter
        /// allows to redefine behavior without actual changing datetime value. The datetime value is treated as having
        /// targetKind.</param>
        /// <remarks>
        /// MS Word uses different behavior for different datetime properties: some are written with conversion to
        /// UTC, others without conversion, but with the Z UTC postfix.
        /// This method allows to define the desired behaviour without changing <see cref="DateTime.Kind"/> value.
        /// See description of the method <see cref="WriteAttribute(string,DateTime)"/> for more information.
        /// </remarks>
        internal void WriteAttribute(string attributeName, DateTime value, DateTimeKind targetKind)
        {
            WriteAttribute(attributeName, DateTime.SpecifyKind(value, targetKind));
        }

        /// <summary>
        /// Writes attribute with double value is it is no equals zero.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Double value for the attribute.</param>
        internal void WriteAttributeIfNotZero(string attributeName, double value)
        {
            if (!MathUtil.IsZero(value))
                WriteAttribute(attributeName, value);
        }

        /// <summary>
        /// Writes attribute with int value is it is no equals zero.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Int value for the attribute.</param>
        internal void WriteAttributeIfNotZero(string attributeName, int value)
        {
            if (value != 0)
                WriteAttribute(attributeName, value);
        }

        /// <summary>
        /// Writes attribute with boolean value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Boolean value for the attribute. Writes "on" if true, "off" if false.</param>
        internal virtual void WriteAttribute(string attributeName, bool value)
        {
            if (mUseOnOff)
                WriteAttributeString(attributeName, value ? "on" : "off");
            else
                WriteAttributeString(attributeName, value ? "true" : "false");
        }

        /// <summary>
        /// Writes attribute with boolean value.
        /// </summary>
        /// <param name="attributeName">The local name of the attribute.</param>
        /// <param name="value">Boolean value for the attribute. Writes "on" if true. If false, the attribute is not written.</param>
        internal virtual void WriteAttributeIfTrue(string attributeName, bool value)
        {
            if (value)
                WriteAttributeString(attributeName, (mUseOnOff) ? "on" : "true");
        }

        /// <summary>
        /// Substitutes 'false' boolean value with null.
        /// </summary>
        protected static object DontWriteOff(bool value)
        {
            if (value)
                return value;

            return null;
        }

        /// <summary>
        /// Gets namespace prefix from element name. Used to automatically extract prefixes for
        /// 'val' attributes from element names, because elements with 'val' attributes are written
        /// with either 'w' or 'wx' prefixes.
        /// </summary>
        /// <param name="name">Name to get prefix from.</param>
        /// <returns>Returns prefix if found, terminated with colon. Otherwise, returns an empty string.</returns>
        internal static string GetPrefix(string name)
        {
            int separatorPos = name.IndexOf(':');

            if (separatorPos > 0)
                return name.Substring(0, separatorPos + 1);
            else
                return "";
        }

        /// <summary>
        /// Writes element with several Border child elements.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="borderNameValuePairs">Sequence of Name/Border pairs.</param>
        internal void WriteBorders(string elementName, params object[] borderNameValuePairs)
        {
            int length = borderNameValuePairs.Length / 2;

            // check if non-empty attribute values present
            bool elementHasBorders = false;

            for (int i = 0; i < length; i++)
            {
                Border border = (Border)borderNameValuePairs[i * 2 + 1];

                if (border != null && !border.IsInherited)
                {
                    elementHasBorders = true;
                    break;
                }
            }

            // return if no non-empty attributes found
            if (!elementHasBorders)
                return;

            StartElement(elementName);

            for (int i = 0; i < length; i++)
                WriteBorder((string)borderNameValuePairs[i * 2], (Border)borderNameValuePairs[i * 2 + 1]);

            EndElement();
        }

        /// <summary>
        /// Writes border element.
        /// </summary>
        /// <param name="elementName">Name of the border element.</param>
        /// <param name="border">Border value.</param>
        internal void WriteBorder(string elementName, Border border)
        {
            if (border == null || border.IsInherited)
                return;

            // WORDSNET-11214 MS Word writes 'DistanceFromText' even when LineStyle is 'None'.
            if (border.IsNil)
                WriteElementWithAttributes(elementName, "w:val", "nil");
            else
                WriteBorderCore(elementName, border);
        }

        /// <summary>
        /// Writes border element.
        /// </summary>
        /// <param name="elementName">Name of the border element.</param>
        /// <param name="border">Border value.</param>
        [JavaThrows(true)]  // IO Exceptions
        protected virtual void WriteBorderCore(string elementName, Border border)
        {
            // Do nothing by default.
        }

        /// <summary>
        /// Writes w:shd element.
        /// </summary>
        /// <param name="shading">Shading value.</param>
        [JavaThrows(true)]  // IO Exceptions
        internal virtual void WriteShd(Shading shading)
        {
            // Do nothing by default.
        }

        /// <summary>
        /// Writes length patterned element in WordML.
        /// These elements can have different names but similar set of attributes:
        /// 'w:w' attribute holds value for length expressed in units that are specified in 'w:type' attribute.
        /// If length type is not specified - "dxa" (twips) is written by default.
        /// </summary>
        /// <param name="elementName">Element name (with namespace prefix included).</param>
        /// <param name="lengthInTwips">Value, holding length in twips.</param>
        /// <param name="isIsoStrict">Flag indicates that destination format is Docx ISO Strict.</param>
        internal void WriteLength(string elementName, int lengthInTwips, bool isIsoStrict)
        {
            WriteLength(elementName, lengthInTwips, PreferredWidthType.Points, isIsoStrict);
        }

        /// <summary>
        /// Writes length patterned element in WordML.
        /// These elements can have different names but similar set of attributes:
        /// 'w:w' attribute holds value for length expressed in units that are specified in 'w:type' attribute.
        /// Value of the 'w:w' attribute is universal measure in ISO Strict format and has units too.
        /// </summary>
        /// <param name="elementName">Element name (with namespace prefix included).</param>
        /// <param name="value">Length value in units that are specified by the <paramref name="type"/> argument:
        /// in twips or 50ths of percent.</param>
        /// <param name="type">Type of <paramref name="value"/>.</param>
        /// <param name="isIsoStrict">Flag indicates that destination format is Docx ISO Strict.</param>
        protected void WriteLength(string elementName, int value, PreferredWidthType type, bool isIsoStrict)
        {
            string strValue = (isIsoStrict)
                ? ((type == PreferredWidthType.Percent)
                    ? PercentToXml((double)value / PreferredWidth.PercentFactor)
                    : ToUniversalMeasure(value, NrxUnit.Twips, NrxUnit.Point))
                : FormatterPal.IntToXml(value);

            WriteElementWithAttributes(
                elementName,
                "w:w", strValue,
                "w:type", StyleConvertUtil.LengthTypeToXml(type));
        }

        /// <summary>
        /// Writes length patterned element to DOCX or WordML.
        /// These elements can have different names but similar set of attributes:
        /// 'w:w' attribute holds value for length expressed in units that are specified in 'w:type' attribute.
        /// Value of the 'w:w' attribute is universal measure in ISO Strict format and has units too.
        /// </summary>
        /// <param name="elementName">Element name (with namespace prefix included).</param>
        /// <param name="length">Length object, holding the length value and type in its properties.</param>
        /// <param name="isIsoStrict">Flag indicates that destination format is Docx ISO Strict.</param>
        internal void WriteLength(string elementName, PreferredWidth length, bool isIsoStrict)
        {
            // Do not write this element if the value is not set.
            if (length == null)
                return;

            WriteLength(elementName, length.ValueRaw, length.Type, isIsoStrict);
        }

        internal void WriteMargins(string elementName, PreferredWidth marginTop, PreferredWidth marginLeft,
            PreferredWidth marginBottom, PreferredWidth marginRight, bool isDocxIsoStrict)
        {
            if ((marginTop != null) || (marginLeft != null) ||
                (marginBottom != null) || (marginRight != null))
            {
                StartElement(elementName);
                WriteLength("w:top", marginTop, isDocxIsoStrict);
                WriteLength((isDocxIsoStrict) ? "w:start" : "w:left", marginLeft, isDocxIsoStrict);
                WriteLength("w:bottom", marginBottom, isDocxIsoStrict);
                WriteLength((isDocxIsoStrict) ? "w:end" : "w:right", marginRight, isDocxIsoStrict);
                EndElement(); //w:tcMar
            }
        }

        /// <summary>
        /// Writes comma delimited set of string values.
        /// </summary>
        internal void WriteSet(string attributeName, string x, string y)
        {
            WriteAttribute(attributeName, string.Format("{0},{1}", x, y).TrimEnd(','));
        }

        /// <summary>
        /// Writes comma delimited set of string values.
        /// </summary>
        internal void WriteSet(string attributeName, string x, string y, string z)
        {
            WriteAttribute(attributeName, string.Format("{0},{1},{2}", x, y, z).TrimEnd(','));
        }

        /// <summary>
        /// Converts numeric percent value to format defined by ST_Percentage simple type of ISO/IEC 29500-1:2012.
        /// </summary>
        /// <param name="value">Input value in percentage points.</param>
        /// <returns>String value with percent sign.</returns>
        internal static string PercentToXml(double value)
        {
            return FormatterPal.DoubleToStrNDecimals(value, 3) + "%";
        }

        /// <summary>
        /// Writes format revision element start.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevisionStart(FormatRevision revision, string elementName, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes edit revision element start.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevisionStart(EditRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes move revision element start.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevisionStart(MoveRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes revision element end. Provided for symmetry with StartRevision.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevisionEnd()
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes numbering revision.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevision(ParagraphNumberRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes field numbering revision.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        internal virtual void WriteRevision(FieldNumberRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Write a simple revision mark. As far as I know it is written only inside p -> pPr -> rPr nesting
        /// to indicate an inserted or deleted paragraph.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevision(EditRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes move revision.
        /// Does nothing. Concrete classes to implement if they support revisions.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteRevision(MoveRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Write a simple cell's revision mark.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteCellRevision(EditRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes SDT's edit revision range start mark.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteSdtRevisionStart(EditRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes SDT's move revision range start mark.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteSdtRevisionStart(MoveRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes SDT's edit revision range end mark.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteSdtRevisionEnd(EditRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// Writes SDT's move revision range end mark.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void WriteSdtRevisionEnd(MoveRevision revision, int id)
        {
            throw new NotImplementedException("Concrete classes should implement.");
        }

        /// <summary>
        /// DOCX only. Returns true if this builder is writing the styles XML document.
        /// </summary>
        internal virtual bool IsStylesBuilder
        {
            get { return false; }
        }

        internal void StartMath()
        {
            mMathNestedLevel++;
        }

        internal void EndMath()
        {
            mMathNestedLevel--;
        }

        /// <summary>
        /// Use this counter to track if writer is inside an Office Math object. In this case we will decorate
        /// "r" and "t" tags with "m" prefix e.g. "m:r" and "m:t.
        /// </summary>
        internal bool IsInMath
        {
            get { return mMathNestedLevel > 0; }
        }

        /// <summary>
        /// Converts the input value to a form difined by the ST_UniversalMeasure simple type of ISO/IEC 29500-1:2012.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="sourceUnit">Measure units of the input value.</param>
        /// <param name="targetUnit">Target measure units. The ST_UniversalMeasure simple type supports only
        /// millimeters, centimeters, inches, points and picas.</param>
        /// <returns>Value formatted as universal measure with <paramref name="targetUnit"/>.</returns>
        internal static string ToUniversalMeasure(double value, NrxUnit sourceUnit, NrxUnit targetUnit)
        {
            string units = UnitToString(targetUnit);
            double targetValue = NrxXmlUtil.ConvertMeasure(value, sourceUnit, targetUnit);

            return FormatterPal.DoubleToStr9Decimals(targetValue) + units;
        }

        /// <summary>
        /// Gets string representation of the specified universal measure unit.
        /// </summary>
        private static string UnitToString(NrxUnit unit)
        {
            switch (unit)
            {
                case NrxUnit.Millimeter:
                    return "mm";
                case NrxUnit.Centimeter:
                    return "cm";
                case NrxUnit.Inch:
                    return "in";
                case NrxUnit.Point:
                    return "pt";
                case NrxUnit.Pica:
                    return "pc";
                default:
                    // ISO 29500 does not define other units for universal measure, so throw
                    throw new InvalidOperationException("Unknown universal measure.");
            }
        }

        /// <summary>
        /// If set to false, allows to enforce ISO29500 specific rule:
        /// The content model of ST_OnOff (ISO29500, Part1, §22.9.2.7) was changed to an xsd:boolean,
        /// removing the values on and off.
        /// </summary>
        private readonly bool mUseOnOff;

        private int mMathNestedLevel;
    }
}
