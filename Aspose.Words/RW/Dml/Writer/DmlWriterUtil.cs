// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2025 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Contains various methods used when writing DML shapes.
    /// </summary>
    internal static class DmlWriterUtil
    {
        /// <summary>
        /// Writes DML adjustable point.
        /// </summary>
        internal static void WriteDmlAdjustablePoint(DmlAdjustablePoint point, NrxXmlBuilder builder)
        {
            builder.WriteElementWithAttributes("a:pt", "x", point.X.String, "y", point.Y.String);
        }

        /// <summary>
        /// Writes percentage offset rectangle.
        /// </summary>
        internal static void WritePercentageOffsetRectangle(DmlPercentageOffsetRectangle rect, string prefix,
            string tagName, bool writeEmpty, NrxXmlBuilder builder, bool isIsoStrict)
        {
            if (rect.IsEmpty && !writeEmpty)
                return;

            string tagNameWithPrefix = string.Format("{0}:{1}", prefix, tagName);

            builder.StartElement(tagNameWithPrefix);
            if (!rect.IsEmpty)
            {
                WritePercentAttributeIfNotZero(prefix, "l", rect.LeftOffset, builder, isIsoStrict);
                WritePercentAttributeIfNotZero(prefix, "t", rect.TopOffset, builder, isIsoStrict);
                WritePercentAttributeIfNotZero(prefix, "r", rect.RightOffset, builder, isIsoStrict);
                WritePercentAttributeIfNotZero(prefix, "b", rect.BottomOffset, builder, isIsoStrict);
            }
            builder.EndElement(tagNameWithPrefix);
        }

        /// <summary>
        /// Writes a percentage type attribute if the value is not zero.
        /// </summary>
        private static void WritePercentAttributeIfNotZero(string prefix, string attributeName,
            double value, NrxXmlBuilder builder, bool isIsoStrict)
        {
            if (MathUtil.DoubleToInt(DmlPercentageUtil.ToDmlPercent(value)) != 0)
            {
                builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, attributeName),
                    DmlPercentageUtil.ToPercentOrDmlPercent(value, isIsoStrict));
            }
        }
    }
}
