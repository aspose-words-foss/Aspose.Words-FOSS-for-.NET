// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/10/2013 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Saving;

namespace Aspose.Words.Validation.DmlToVml
{
    /// <summary>
    /// FOSS reduction: only the pure-model helpers survive. The DML-to-VML conversion
    /// helpers (ConvertDmlToShape, ConvertDmlToShapeNoReplace, ReplaceShape) and their
    /// geometry/fill helpers are gone with the rendering subsystem.
    /// </summary>
    internal static class DmlUtil
    {
        /// <summary>
        /// Returns true if the specified drawingML needs fallback shape.
        /// MS Word always writes fallback shape for Wordprocessing Shape, Wordprocessing Group, Wordprocessing Canvas and
        /// ContentPart - so we do the same.
        /// </summary>
        internal static bool NeedFallback(ShapeBase dml, OoxmlComplianceCore compliance)
        {
            if (!dml.IsTopLevel)
                return false;

            DmlNodeType dmlType = dml.DmlNode.DmlNodeType;

            if (dmlType == DmlNodeType.ChartEx) // A chartEx shape needs DML picture fallback.
                return true;

            if (compliance == OoxmlComplianceCore.IsoStrict)  // ISO Strict does not support VML shapes.
                return false;

            // Graphic frame can not be top level, so missed here.
            return (dmlType == DmlNodeType.WordprocessingShape)
                || (dmlType == DmlNodeType.WordprocessingGroupShape)
                || (dmlType == DmlNodeType.WordprocessingCanvas)
                || (dmlType == DmlNodeType.ContentPart);
        }

        /// <summary>
        /// Returns true if the specified DrawingML shape needs DrawingML fallback shape.
        /// (A chartEx shape needs DML picture fallback.)
        /// </summary>
        internal static bool NeedDmlFallback(ShapeBase dml)
        {
            return dml.IsTopLevel && (dml.DmlNode.DmlNodeType == DmlNodeType.ChartEx);
        }

        /// <summary>
        /// Replaces DrawingML node with alternate (FallBack) nodes.
        /// We expect shape inside the fallback but according to spec there can be any node.
        /// Currently this method works just in case when FallBack contains inline nodes.
        /// </summary>
        internal static bool ReplaceDmlWithFallBack(ShapeBase dml, DocumentVisitor visitor)
        {
            ShapeBase fallbackShape = dml.FallbackShape;

            if (fallbackShape == null)
                return false;

            // WORDSNET-6729 We need to preserve run properties that were on the original DrawingML node, assign them onto the fallback VML shape.
            fallbackShape.RunPr = dml.RunPr;

            // andrnosk: WORDSNET-9723 Mimic MS Word behavior and set drawingML z-order to fallback shape.
            fallbackShape.ZOrder = dml.ZOrder;

            // WORDSNET-12083 Copy hyperlink from DML when it is replaced by Fallback.
            if (dml.HasHyperlink && !fallbackShape.HasHyperlink)
                fallbackShape.HRef = dml.HRef;

            NodeUtil.InsertShapeAtCompatibleTreeLevel(fallbackShape, dml);

            fallbackShape.Accept(visitor);
            dml.Remove();

            return true;
        }
    }
}
