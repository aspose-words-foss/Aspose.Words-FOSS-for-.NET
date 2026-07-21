// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/10/2011 by Alexey Titov

using System;
using Aspose.Words.Nrx;

namespace Aspose.Words.Drawing.Core.Dml.Readers
{
    /// <summary>
    /// Base class for reading DML shapes.
    /// </summary>
    internal class DmlReaderBase
    {
        /// <summary>
        /// Warns current reader element is not supported and ignores it.
        /// </summary>
        protected static void WarnNotSupportedAndIgnoreElement(NrxXmlReader reader)
        {
            WarnNotSupported(reader);
            reader.IgnoreElement();
        }

        /// <summary>
        /// Warns current reader element is unexpected and ignores it.
        /// </summary>
        protected static void WarnUnexpectedAndIgnoreElement(NrxXmlReader reader)
        {
            WarnUnexpected(reader);
            reader.IgnoreElement();
        }

        /// <summary>
        /// Warns current reader element is not supported.
        /// </summary>
        private static void WarnNotSupported(NrxXmlReader reader)
        {
            reader.Warn(
                WarningType.DataLoss,
                WarningSource.DrawingML,
                string.Format(WarningStrings.NotSupportedTag, reader.LocalName));
        }

        /// <summary>
        /// Warns current reader element is unexpected.
        /// </summary>
        protected static void WarnUnexpected(NrxXmlReader reader)
        {
            reader.Warn(
                WarningType.UnexpectedContent,
                WarningSource.DrawingML,
                string.Format(WarningStrings.UnexpectedTagOrAttribute, reader.LocalName));
        }

        /// <summary>
        /// Warns shape is ignored.
        /// </summary>
        protected static void WarnShapeIgnored(NrxXmlReader reader, string details)
        {
            reader.Warn(
                WarningType.UnexpectedContent,
                WarningSource.DrawingML,
                string.Format(WarningStrings.ShapeIgnored, details));
        }

        /// <summary>
        /// Returns shape's type by <paramref name="tagName"/>.
        /// </summary>
        protected static DmlNodeType GetShapeType(string tagName)
        {
            switch (tagName)
            {
                case "sp":
                    return DmlNodeType.Shape;
                case "wsp":
                    return DmlNodeType.WordprocessingShape;
                case "cxnSp":
                    return DmlNodeType.ConnectorShape;
                case "lockedCanvas":
                    return DmlNodeType.LockedCanvas;
                case "wpc":
                    return DmlNodeType.WordprocessingCanvas;
                case "grpSp":
                    return DmlNodeType.GroupShape;
                case "wgp":
                    return DmlNodeType.WordprocessingGroupShape;
                case "spTree":
                    return DmlNodeType.SpTree;
                case "chart":
                    return DmlNodeType.Chart;
                case "relIds":
                    return DmlNodeType.Diagram;
                case "pic":
                    return DmlNodeType.Picture;
                case "graphicFrame":
                    return DmlNodeType.GraphicFrame;
                default:
                    throw new ArgumentException("Unexpected tag name");
            }
        }
    }
}
