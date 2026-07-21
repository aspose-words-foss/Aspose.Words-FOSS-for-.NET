// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2014 by Andrey Noskov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Instance class for writing DrawingML shape effects.
    /// </summary>
    internal static class DmlShapeEffectsWriter
    {
        internal static void Write(DmlShapeEffectsCollection shapeEffectsCollection, IDmlShapeWriterContext writer,
            bool isThemeWriting)
        {
            if (!isThemeWriting)
            {
                // For theme effectLst is required element.
                if (shapeEffectsCollection == null)
                    return;

                // Do not write effects that are inherited from theme for shapes. 
                if (shapeEffectsCollection.IsTheme)
                    return;
            }

            NrxXmlBuilder builder = writer.Builder;

            string tagName = "a:effectLst";
            if ((shapeEffectsCollection != null) && shapeEffectsCollection.IsEffectDag)
                tagName = "a:effectDag";

            builder.StartElement(tagName);

            if ((shapeEffectsCollection != null) && shapeEffectsCollection.IsEffectDag)
            {
                builder.WriteAttribute("name", shapeEffectsCollection.DagName);
                builder.WriteAttributeIfNotDefault("type", DmlEnum.EffectDagTypeToDml(shapeEffectsCollection.DagType), "sib");
            }

            if (shapeEffectsCollection != null) 
                foreach (DmlShapeEffect dmlShapeEffect in shapeEffectsCollection)
                    WriteEffect(dmlShapeEffect, writer);

            builder.EndElement(tagName);
        }

        private static void WriteEffect(DmlShapeEffect dmlShapeEffect, IDmlShapeWriterContext writer)
        {
            switch (dmlShapeEffect.EffectType)
            {
                case DmlShapeEffectType.Blur:
                    WriteBlur((DmlShapeBlurEffect)dmlShapeEffect, writer);
                    break;
                case DmlShapeEffectType.FillOverlay:
                    WriteFillOverlay((DmlShapeFillOverlayEffect)dmlShapeEffect, writer);
                    break;
                case DmlShapeEffectType.Glow:
                    WriteGlow((DmlShapeGlowEffect)dmlShapeEffect, writer, false);
                    break;
                case DmlShapeEffectType.InnerShadow:
                    WriteInnerShadow((DmlShapeInnerShadowEffect)dmlShapeEffect, writer);
                    break;
                case DmlShapeEffectType.OuterShadow:
                    WriteOuterShadow((DmlShapeOuterShadowEffect)dmlShapeEffect, writer, false);
                    break;
                case DmlShapeEffectType.PresetShadow:
                    WritePresetShadow((DmlShapePresetShadowEffect)dmlShapeEffect, writer);
                    break;
                case DmlShapeEffectType.Reflection:
                    WriteReflection((DmlShapeReflectionEffect)dmlShapeEffect, writer, false);
                    break;
                case DmlShapeEffectType.SoftEdges:
                    WriteSoftEdges((DmlShapeSoftEdgeEffect)dmlShapeEffect, writer);
                    break;
                default:
                   throw new ArgumentException("Unexpected Dml shape effect type.");
            }
        }

        private static void WriteSoftEdges(DmlShapeSoftEdgeEffect shapeSoftEdge, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.WriteElementWithAttributes("a:softEdge", "rad", shapeSoftEdge.Radius);
        }

        internal static void WriteReflection(DmlShapeReflectionEffect shapeReflection, IDmlShapeWriterContext writer,
            bool isTextEffect)
        {
            string prefix = isTextEffect ? "w14" : "a";
            string tagName = string.Format("{0}:reflection", prefix);
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            // Note: optional attributes are written because 
            // MS Word does not process text effects with omitted default attributes properly.
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "blurRad"), 
                shapeReflection.BlurRadius);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "stA"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeReflection.StartAlpha, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "stPos"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeReflection.StartPosition, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "endA"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeReflection.EndAlpha, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "endPos"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeReflection.EndPosition, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "dist"), 
                shapeReflection.Distance);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "dir"), 
                shapeReflection.Direction.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "fadeDir"), 
                shapeReflection.FadeDirection.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "sx"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeReflection.HorizontalScale, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "sy"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeReflection.VerticalScale, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "kx"), 
                shapeReflection.HorizontalSkew.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "ky"), 
                shapeReflection.VerticalSkew.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "algn"), 
                DmlEnum.RectangleAlignmentToDml(shapeReflection.Alignment));
            if (!shapeReflection.RotateWithShape)
                builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "rotWithShape"), shapeReflection.RotateWithShape);
            builder.EndElement(tagName);
        }

        private static void WritePresetShadow(DmlShapePresetShadowEffect shapePresetShadow, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:prstShdw");
            builder.WriteAttribute("prst", DmlEnum.PresetShadowToDml(shapePresetShadow.PresetShadow));
            builder.WriteAttribute("dist", shapePresetShadow.Distance);
            builder.WriteAttribute("dir", System.Math.Round(shapePresetShadow.Direction.Value, 0));
            DmlColorWriter.Write(shapePresetShadow.Color, writer);
            builder.EndElement("a:prstShdw");
        }

        internal static void WriteOuterShadow(DmlShapeOuterShadowEffect shapeOuterShadow, IDmlShapeWriterContext writer,
                                              bool isTextEffect)
        {
            string prefix = isTextEffect ? "w14" : "a";
            string tagName = isTextEffect ? "w14:shadow" : "a:outerShdw";
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            // Note: optional attributes are written because 
            // MS Word does not process text effects with omitted default attributes properly.
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "blurRad"), 
                shapeOuterShadow.BlurRadius);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "dist"), 
                shapeOuterShadow.Distance);

            // Direction value have to be written as integer.
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "dir"),
                System.Math.Round(shapeOuterShadow.Direction.Value, 0));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "sx"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeOuterShadow.HorizontalScale, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "sy"), 
                DmlPercentageUtil.ToPercentOrDmlPercent(shapeOuterShadow.VerticalScale, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "kx"), 
                System.Math.Round(shapeOuterShadow.HorizontalSkew.Value, 0));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "ky"),
                 System.Math.Round(shapeOuterShadow.VerticalSkew.Value, 0));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "algn"),
                DmlEnum.RectangleAlignmentToDml(shapeOuterShadow.Alignment));
            if (!shapeOuterShadow.RotateWithShape)
                builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "rotWithShape"), shapeOuterShadow.RotateWithShape);

            DmlColorWriter.Write(prefix, shapeOuterShadow.Color, writer);
            builder.EndElement(tagName);
        }

        private static void WriteFillOverlay(DmlShapeFillOverlayEffect shapeFillOverlay, IDmlShapeWriterContext writer)
        {
            DmlEffectsWriter.WriteFillOverlay(shapeFillOverlay.FillOverlay, writer);
        }

        private static void WriteInnerShadow(DmlShapeInnerShadowEffect shapeInnerShadow, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:innerShdw");
            builder.WriteAttribute("blurRad", shapeInnerShadow.BlurRadius);
            builder.WriteAttribute("dist", shapeInnerShadow.Distance);
            builder.WriteAttribute("dir", shapeInnerShadow.Direction.Value);
            DmlColorWriter.Write(shapeInnerShadow.Color, writer);
            builder.EndElement("a:innerShdw");
        }

        internal static void WriteGlow(DmlShapeGlowEffect shapeGlow, IDmlShapeWriterContext writer, bool isTextEffect)
        {
            string prefix = isTextEffect ? "w14" : "a";
            string tagName = string.Format("{0}:glow", prefix);

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "rad"), shapeGlow.Radius);
            DmlColorWriter.Write(prefix, shapeGlow.Color, writer);
            builder.EndElement(tagName);
        }

        private static void WriteBlur(DmlShapeBlurEffect shapeBlur, IDmlShapeWriterContext writer)
        {
            DmlEffectsWriter.WriteBlur(shapeBlur.Blur, writer);
        }
    }
}
