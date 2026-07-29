// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/16/2014 by Andrey Noskov

using System;
using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlNamespaceUtil
    {
        /// <summary>
        /// Returns namespace of the specified Dml node type.
        /// </summary>
        internal static string GetNamespace(DmlNodeType dmlType, bool isIsoStrict)
        {
            switch (dmlType)
            {
                case DmlNodeType.LockedCanvas:
                case DmlNodeType.Picture:
                case DmlNodeType.Chart:
                case DmlNodeType.ChartEx:
                case DmlNodeType.Diagram:
                case DmlNodeType.WordprocessingShape:
                case DmlNodeType.WordprocessingGroupShape:
                case DmlNodeType.WordprocessingCanvas:
                case DmlNodeType.ContentPart:
                    return isIsoStrict
                        ? gGraphicDataNamespacesStrict[(int)dmlType]
                        : gGraphicDataNamespacesTransitional[(int)dmlType];
                default:
                    throw new ArgumentException("Unexpected DrawingML type.");
            }
        }

        /// <summary>
        /// Adds prefix to attribute if prefix is 'w14'.
        /// This is required for writing text effects.
        /// </summary>
        internal static string GetAttrName(string prefix, string attr)
        {
            if (prefix != "w14")
                return attr;

            return string.Format("{0}:{1}", prefix, attr);
        }

        static DmlNamespaceUtil()
        {
            // Init graphicData namespaces dictionary.
            gGraphicDataNamespacesTransitional = new IntToObjDictionary<string>();
            InitNamespacesTransitional();
            gGraphicDataNamespacesStrict = new IntToObjDictionary<string>();
            InitNamespacesStrict();
        }

        private static void InitNamespacesTransitional()
        {
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.Chart] = EcmaDmlPrefix + "chart";
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.ChartEx] = ChartEx;
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.Diagram] = EcmaDmlPrefix + "diagram";
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.LockedCanvas] = EcmaDmlPrefix + "lockedCanvas";
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.Picture] = EcmaDmlPrefix + "picture";
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.WordprocessingCanvas] = 
                WordPrefix + "wordprocessingCanvas";
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.WordprocessingGroupShape] = 
                WordPrefix + "wordprocessingGroup";
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.WordprocessingShape] = 
                WordPrefix + "wordprocessingShape";
            // Theoretically here might be other than Ink types, but for now I did not see such documents.
            // So write wordprocessingInk for ContentPart.
            gGraphicDataNamespacesTransitional[(int)DmlNodeType.ContentPart] = WordPrefix + "wordprocessingInk";

            // There is no namespaces for DmlNodeType.GroupShape and DmlNodeType.Shape, 
            // because they seem cannot be direct children 
            // of graphicData, they are always children of lockedCanvas.
            // For the same reason GraphicFrame DML node type is missing here (Frame can be child element
            // of the group or canvas according to XML scheme described into attachment 
            // “A.4.4 DrawingML - WordprocessingML Drawing” of the spec).
        }

        private static void InitNamespacesStrict()
        {
            gGraphicDataNamespacesStrict[(int)DmlNodeType.Chart] = IsoDmlPrefix + "chart";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.ChartEx] = ChartEx;
            gGraphicDataNamespacesStrict[(int)DmlNodeType.Diagram] = IsoDmlPrefix + "diagram";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.LockedCanvas] = IsoDmlPrefix + "lockedCanvas";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.Picture] = IsoDmlPrefix + "picture";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.WordprocessingCanvas] = WordPrefix + "wordprocessingCanvas";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.WordprocessingGroupShape] = 
                WordPrefix + "wordprocessingGroup";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.WordprocessingShape] = WordPrefix + "wordprocessingShape";
            gGraphicDataNamespacesStrict[(int)DmlNodeType.ContentPart] = WordPrefix + "wordprocessingInk";
        }

        private static readonly IntToObjDictionary<string> gGraphicDataNamespacesTransitional;
        private static readonly IntToObjDictionary<string> gGraphicDataNamespacesStrict;

        private const string IsoDmlPrefix = "http://purl.oclc.org/ooxml/drawingml/";
        private const string EcmaDmlPrefix = "http://schemas.openxmlformats.org/drawingml/2006/";
        private const string WordPrefix = "http://schemas.microsoft.com/office/word/2010/";
        private const string ChartEx = "http://schemas.microsoft.com/office/drawing/2014/chartex";
    }
}
