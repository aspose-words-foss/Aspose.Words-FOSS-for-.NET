// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2015 by Andrey Noskov

using System;
using Aspose.Common;
using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Vml
{
    internal class VmlShapeReaderBase
    {
        /// <summary>
        /// Sets fixed attribute only if the specified string has a valid value.
        /// </summary>
        internal static void SetFixedAttribute(ShapeBase shape, int shapeAttr, string value)
        {
            SetFixedAttribute(shape, shapeAttr, new VmlQuantity(value));
        }

        /// <summary>
        /// Sets fixed attribute only if the specified quantity qualifies as a valid fixed value.
        /// </summary>
        internal static void SetFixedAttribute(ShapeBase shape, int shapeAttr, VmlQuantity quantity)
        {
            if (quantity.IsFixed)
                shape.SetShapeAttrInternal(shapeAttr, quantity.ToFixed());
        }

        /// <summary>
        /// Sets fixed degrees attribute.
        /// </summary>
        /// <remarks>
        /// If units are 'fd' or 'f' then the attr value set as is
        /// If units are not specified then the attr value set multiplied by 0x10000.
        /// If units are not recognized or no valid numeric value is found then the attr value set as 0.
        /// </remarks>
        internal static void SetFixedDegreesAttribute(ShapeBase shape, int shapeAttr, VmlQuantity quantity)
        {
            if (quantity.IsDegrees)
                shape.SetShapeAttrInternal(shapeAttr, quantity.DegreesToFixed());
        }

        /// <summary>
        /// Sets percent attribute only if the specified quantity qualifies as a valid percent value.
        /// </summary>
        internal static void SetPercentAttribute(ShapeBase shape, int shapeAttr, string value)
        {
            VmlQuantity q = new VmlQuantity(value);

            if (q.IsPercent)
                shape.SetShapeAttrInternal(shapeAttr, q.PercentToInt());
        }

        internal static void Set2DFixed(ShapeBase shape, int attrX, int attrY, string value)
        {
            VmlQuantity[] attrValues = VmlToQuantityArray(value);

            SetFixedAttribute(shape, attrX, attrValues[0]);

            if (attrValues.Length > 1)
                SetFixedAttribute(shape, attrY, attrValues[1]);
        }

        /// <summary>
        /// Sets boolean attribute only if the specified string has "t", "true" or "f", "false" value.
        /// </summary>
        internal static void SetBoolAttribute(ShapeBase shape, int shapeAttr, string value)
        {
            switch (value)
            {
                // andrnosk: WORDSNET-8346 (conversion part) According to specification DOCX-ECMA 376 (part4) 6.1.2.14.
                // The possible values for 'strokeok' attribute are defined by the ST_TrueFalse simple type,
                // and for this type: 'true', 'false', 'f', 't' are possible values.
                // WORDSNET-20845 Added resilience for 1 as true and 0 as false.
                case "t":
                case "true":
                case "1":
                    shape.SetShapeAttrInternal(shapeAttr, true);
                    break;
                case "f":
                case "false":
                case "0":
                    shape.SetShapeAttrInternal(shapeAttr, false);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected ST_TrueFalse value.");
            }
        }

        /// <summary>
        /// Sets the boolean attribute even if the value doesn't match the correct values.
        /// </summary>
        internal static void SetBoolAttributeForce(ShapeBase shape, int shapeAttr, string value)
        {
            SetBoolAttribute(shape, shapeAttr, value, value != "0");
        }

        /// <summary>
        /// Sets boolean attribute only if the specified string has "t", "true" or "f", "false" value
        /// in all other invalid cases set defaultValue.
        /// </summary>
        internal static void SetBoolAttribute(ShapeBase shape, int shapeAttr, string value, bool defaultValue)
        {
            try
            {
                SetBoolAttribute(shape, shapeAttr, value);
            }
            catch (InvalidOperationException)
            {
                shape.SetShapeAttrInternal(shapeAttr, defaultValue);
            }
        }

        /// <summary>
        /// Sets boolean attribute only if the specified string has one of the specified true or false string values.
        /// </summary>
        internal static void SetBoolAttribute(ShapeBase shape, int shapeAttr, string trueValue, string falseValue, string value)
        {
            if (value == trueValue)
                shape.SetShapeAttrInternal(shapeAttr, true);
            else if (value == falseValue)
                shape.SetShapeAttrInternal(shapeAttr, false);
        }

        /// <summary>
        /// Sets EMUs attribute only if the specified string has a valid value.
        /// </summary>
        internal static void SetEmusAttribute(ShapeBase shape, int shapeAttr, string value)
        {
            SetEmusAttribute(shape, shapeAttr, new VmlQuantity(value));
        }

        /// <summary>
        /// Sets EMUs attribute only if the specified string has a valid value.
        /// </summary>
        internal static void SetEmusAttribute(ShapeBase shape, int shapeAttr, VmlQuantity quantity)
        {
            if (quantity.IsDistance)
                shape.SetShapeAttrInternal(shapeAttr, quantity.DistanceToEmus());
        }

        /// <summary>
        /// Sets int attribute if the string is not empty.
        /// </summary>
        internal static void SetIntAttribute(ShapeBase shape, int shapeAttr, string value)
        {
            if (StringUtil.HasChars(value))
                shape.SetShapeAttrInternal(shapeAttr, FormatterPal.ParseInt(value));
        }

        internal static VmlQuantity[] VmlToQuantityArray(string value)
        {
            return VmlToQuantityArray(value, "");
        }

        /// <summary>
        /// Parses comma-delimited string of quantities (floating values with dimension units).
        /// </summary>
        internal static VmlQuantity[] VmlToQuantityArray(string value, string defaultUnits)
        {
            string[] strings = value.Split(',');
            VmlQuantity[] quantities = new VmlQuantity[strings.Length];

            for (int i = 0; i < strings.Length; i++)
                quantities[i] = new VmlQuantity(strings[i], defaultUnits);

            return quantities;
        }

        internal const string WarningMessageFormat = "Import of element '{0}' is not supported upon VML import by Aspose.Words.";
    }
}
