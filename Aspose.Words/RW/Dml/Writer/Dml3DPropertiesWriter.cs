// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2014 by Andrey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Class used to write DrawingML 3D properties.
    /// </summary>
    internal static class Dml3DPropertiesWriter
    {
        internal static void WriteScene3D(DmlScene3DProperties scene3DProps, IDmlShapeWriterContext writer,
            bool isTextEffect)
        {
            string prefix = isTextEffect ? "w14" : "a";
            WriteScene3D(prefix, scene3DProps, writer, false);
        }

        internal static void WriteScene3D(string prefix, DmlScene3DProperties scene3DProps,
            IDmlShapeWriterContext writer, bool isThemeWriting)
        {
            // Check if we have something to write.
            if (scene3DProps == null)
                return;

            // Do not write properties that are inherited from theme for shapes.
            if (!isThemeWriting && scene3DProps.IsTheme)
                return;

            string tagName = string.Format("{0}:scene3d", prefix);
            
            // Child elements have 'a' prefix.
            if (prefix == "dgm")
                prefix = "a";

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(tagName);

            WriteScene3DCamera(prefix, scene3DProps.Camera, builder, writer.Compliance);
            WriteScene3DLightRig(prefix, scene3DProps.LightRig, builder);
            WriteScene3DBackdrop(prefix, scene3DProps.BackdropPlane, writer);

            DmlExtensionListWriter.Write(scene3DProps.Extensions, writer);

            builder.EndElement(tagName);
        }

        private static void WriteScene3DLightRig(string prefix, DmlLightRig lightRig, NrxXmlBuilder builder)
        {
            string tagName = string.Format("{0}:lightRig", prefix);
            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "rig"), DmlEnum.LightRigTypeDml(lightRig.LightRigType));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "dir"), DmlEnum.LightRigDirectionToDml(lightRig.Direction));
            WriteScene3DRotation(prefix, lightRig.Rotation, builder);
            builder.EndElement(tagName);
        }

        private static void WriteScene3DBackdrop(string prefix, DmlBackdropPlane backdropPlane,
            IDmlShapeWriterContext writer)
        {
            if (backdropPlane == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            string tagName = string.Format("{0}:backdrop", prefix);

            builder.StartElement(tagName);

            DmlPoint3D dmlPoint3D = backdropPlane.Anchor;
            DmlBasicsWriter.WriteXYZ(string.Format("{0}:anchor", prefix), dmlPoint3D.X, dmlPoint3D.Y, dmlPoint3D.Z, builder);

            dmlPoint3D = backdropPlane.NormalVector;
            DmlBasicsWriter.WriteDxDyDz(string.Format("{0}:norm", prefix), dmlPoint3D.X, dmlPoint3D.Y, dmlPoint3D.Z, builder);

            dmlPoint3D = backdropPlane.UpVector;
            DmlBasicsWriter.WriteDxDyDz(string.Format("{0}:up", prefix), dmlPoint3D.X, dmlPoint3D.Y, dmlPoint3D.Z, builder);

            DmlExtensionListWriter.Write(backdropPlane.Extensions, writer);

            builder.EndElement(tagName);
        }

        private static void WriteScene3DCamera(string prefix, DmlCamera dmlCamera, NrxXmlBuilder builder, 
            OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = compliance == OoxmlComplianceCore.IsoStrict;
            string tagName = string.Format("{0}:camera", prefix);

            builder.StartElement(tagName);

            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "prst"), DmlEnum.PresetCameraTypeToDml(dmlCamera.PresetCameraType));
            
            // Do not write default value.
            if (!MathUtil.AreEqual(dmlCamera.FovAngle.ValueInDegrees, 180.0d))
                builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "fov"), dmlCamera.FovAngle.Value);

            // Do not write default value.
            if (!MathUtil.AreEqual(dmlCamera.Zoom, 1.0d))
                builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "zoom"), 
                    DmlPercentageUtil.ToPercentOrDmlPercent(dmlCamera.Zoom, isIsoStrict));

            WriteScene3DRotation(prefix, dmlCamera.Rotation, builder);

            builder.EndElement(tagName);
        }

        private static void WriteScene3DRotation(string prefix, DmlRotation3D rotation, NrxXmlBuilder builder)
        {
            if (rotation == null)
                return;

            string tagName = string.Format("{0}:rot", prefix);

            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "lat"), rotation.Latitude.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "lon"), rotation.Longitude.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "rev"), rotation.Revolution.Value);
            builder.EndElement(tagName);
        }

        internal static void WriteShape3D(DmlShape3DProperties shape3DProps, IDmlShapeWriterContext writer,
            bool isTextEffect)
        {
            string prefix = isTextEffect ? "w14" : "a";
            WriteShape3D(prefix, shape3DProps, writer, false);
        }

        internal static void WriteShape3D(string prefix, DmlShape3DProperties shape3DProps,
            IDmlShapeWriterContext writer, bool isThemeWriting)
        {
            // Check if we have something to write.
            if (shape3DProps == null)
                return;

            // Do not write properties that are inherited from theme for shapes.
            if (!isThemeWriting && shape3DProps.IsTheme)
                return;

            string partTagName = (prefix == "w14") ? "props3d" : "sp3d";
            string tagName = string.Format("{0}:{1}", prefix, partTagName);

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);
            builder.WriteAttributeIfNotZero(DmlNamespaceUtil.GetAttrName(prefix, "extrusionH"), shape3DProps.ExtrusionHeight);
            builder.WriteAttributeIfNotZero(DmlNamespaceUtil.GetAttrName(prefix, "contourW"), shape3DProps.ContourWidth);
            builder.WriteAttributeIfNotDefault(DmlNamespaceUtil.GetAttrName(prefix, "prstMaterial"), 
                DmlEnum.PresetMaterialToDml(shape3DProps.PresetMaterial), "warmMatte");
            builder.WriteAttributeIfNotZero(DmlNamespaceUtil.GetAttrName(prefix, "z"), shape3DProps.Z);

            WriteBevel(prefix, "bevelT", shape3DProps.BevelTop, builder);
            WriteBevel(prefix, "bevelB", shape3DProps.BevelBottom, builder);
            WriteExtrusionColor(prefix, shape3DProps.ExtrusionColor, writer);
            WriteContourClr(prefix, shape3DProps.ContourColor, writer);

            DmlExtensionListWriter.Write("a", shape3DProps.Extensions, writer);

            builder.EndElement(tagName);
        }

        private static void WriteExtrusionColor(string prefix, DmlColor extrusionColor, IDmlShapeWriterContext writer)
        {
            if (extrusionColor == null)
                return;
            
            string tagName = string.Format("{0}:extrusionClr", prefix);

            writer.Builder.StartElement(tagName);
            DmlColorWriter.Write(prefix, extrusionColor, writer);
            writer.Builder.EndElement(tagName);
        }

        private static void WriteContourClr(string prefix, DmlColor contourColor, IDmlShapeWriterContext writer)
        {
            if (contourColor == null)
                return;

            string tagName = string.Format("{0}:contourClr", prefix);

            writer.Builder.StartElement(tagName);
            DmlColorWriter.Write(prefix,contourColor, writer);
            writer.Builder.EndElement(tagName);
        }

        private static void WriteBevel(string prefix, string elementName, DmlBevel bevel, NrxXmlBuilder builder)
        {
            if (bevel == null)
                return;
            
            string tagName = string.Format("{0}:{1}", prefix, elementName);
            
            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "w"), bevel.Width);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "h"), bevel.Height);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "prst"), DmlEnum.BevelPresetTypeToDml(bevel.BevelPresetType));
            builder.EndElement(tagName);
        }
    }
}
