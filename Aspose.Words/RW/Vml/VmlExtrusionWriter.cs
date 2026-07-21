// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/08/2007 by Vladimir Averkin

using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'v:extrusion' sub-element of v:shape to WordML.
    /// </summary>
    internal class VmlExtrusionWriter
    {
        internal VmlExtrusionWriter(NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            mBuilder = builder;
            mContext = context;
        }

        /// <summary>
        /// Add 'v:extrusion' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            // Determine if the added attribute is written inside 'v:extrusion' element.
            // This is done to avoid writing of the empty 'v:extrusion' declaration.
            if (value is bool)
                switch (key)
                {
                    // these values are true by default
                    case ShapeAttr.TDParallel:
                    case ShapeAttr.TDLightFace:
                    case ShapeAttr.TDConstrainRotation:
                    case ShapeAttr.TDKeyHarsh:
                        if (!(bool)value)
                            mAttrCount++;
                        break;
                    // other possible values are false by default
                    default:
                        if ((bool)value)
                            mAttrCount++;
                        break;
                }
            else
                mAttrCount++;

            switch (key)
            {
                case ShapeAttr.TDAmbientIntensity:
                    mTDAmbientIntensity = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDConstrainRotation:
                    mTDConstrainRotation = VmlUtil.BoolToVml(value, true);
                    break;
                case ShapeAttr.TDDiffuseAmount:
                    mTDDiffuseAmount = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDEdgeThickness:
                    mTDEdgeThickness = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.TDExtrudeBackward:
                    mTDExtrudeBackward = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.TDExtrudeForward:
                    mTDExtrudeForward = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.TDExtrudePlane:
                    mTDExtrudePlane = VmlEnum.PlaneTypeToVml((PlaneType)value);
                    break;
                case ShapeAttr.TDExtrusionColor:
                    mTDExtrusionColor = mContext.ColorToVml((DrColor)value);
                    break;
                case ShapeAttr.TDExtrusionColorExt:
                    // RK Why nothing?
                    break;
                case ShapeAttr.TDExtrusionColorExtCMY:
                    // RK Why nothing?
                    break;
                case ShapeAttr.TDExtrusionColorExtK:
                    // RK Why nothing?
                    break;
                case ShapeAttr.TDExtrusionColorExtMod:
                    // RK Why nothing?
                    break;
                case ShapeAttr.TDFillHarsh:
                    mTDFillHarsh = VmlUtil.BoolToVml(value, false);
                    break;
                case ShapeAttr.TDFillIntensity:
                    mTDFillIntensity = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDFillX:
                    mTDFillX = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDFillY:
                    mTDFillY = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDFillZ:
                    mTDFillZ = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDKeyHarsh:
                    mTDKeyHarsh = VmlUtil.BoolToVml(value, true);
                    break;
                case ShapeAttr.TDKeyIntensity:
                    mTDKeyIntensity = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDKeyX:
                    mTDKeyX = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDKeyY:
                    mTDKeyY = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDKeyZ:
                    mTDKeyZ = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDLightFace:
                    mTDLightFace = VmlUtil.BoolToVml(value, true);
                    break;
                case ShapeAttr.TDMetallic:
                    mTDMetallic = VmlUtil.BoolToVml(value, false);
                    break;
                case ShapeAttr.TDOn:
                    mTDOn = VmlUtil.BoolToVml(value, false);
                    break;
                case ShapeAttr.TDOriginX:
                    mTDOriginX = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDOriginY:
                    mTDOriginY = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDParallel:
                    if (!(bool)value)
                        mTDParallel = "perspective";
                    break;
                case ShapeAttr.TDRenderMode:
                    mTDRenderMode = VmlEnum.ThreeDRenderModeToVml((ThreeDRenderMode)value);
                    break;
                case ShapeAttr.TDRotationAngle:
                    mTDRotationAngle = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDRotationAngleX:
                    mTDRotationAngleX = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDRotationAngleY:
                    mTDRotationAngleY = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDRotationAxisX:
                    mTDRotationAxisX = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDRotationAxisY:
                    mTDRotationAxisY = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDRotationAxisZ:
                    mTDRotationAxisZ = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDRotationCenterAuto:
                    mTDRotationCenterAuto = VmlUtil.BoolToVml(value, false);
                    break;
                case ShapeAttr.TDRotationCenterX:
                    mTDRotationCenterX = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDRotationCenterY:
                    mTDRotationCenterY = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDRotationCenterZ:
                    mTDRotationCenterZ = VmlUtil.EmuToVmlMillimeters((int)value);
                    break;
                case ShapeAttr.TDShininess:
                    mTDShininess = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.TDSkewAmount:
                    mTDSkewAmount = VmlUtil.PercentsToVml((int)value);
                    break;
                case ShapeAttr.TDSkewAngle:
                    mTDSkewAngle = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDSpecularAmount:
                    mTDSpecularAmount = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDTolerance:
                    mTDTolerance = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.TDUseExtrusionColor:
                    // Ignored.
                    break;
                case ShapeAttr.TDViewpointX:
                    mTDViewpointX = VmlUtil.EmuToVmlMillimeters((int)value);
                    break;
                case ShapeAttr.TDViewpointY:
                    mTDViewpointY = VmlUtil.EmuToVmlMillimeters((int)value);
                    break;
                case ShapeAttr.TDViewpointZ:
                    mTDViewpointZ = VmlUtil.EmuToVmlMillimeters((int)value);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'v:extrusion' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount <= 0)
                return;

            mBuilder.StartElement("o:extrusion");

            mBuilder.WriteAttribute("v:ext", "view");
            mBuilder.WriteAttribute("specularity", mTDSpecularAmount);
            mBuilder.WriteAttribute("diffusity", mTDDiffuseAmount);
            mBuilder.WriteAttribute("shininess", mTDShininess);
            mBuilder.WriteAttribute("edge", mTDEdgeThickness);
            mBuilder.WriteAttribute("foredepth", mTDExtrudeForward);
            mBuilder.WriteAttribute("backdepth", mTDExtrudeBackward);
            mBuilder.WriteAttribute("plane", mTDExtrudePlane);
            mBuilder.WriteAttribute("color", mTDExtrusionColor);
            mBuilder.WriteAttribute("on", mTDOn);
            mBuilder.WriteAttribute("metal", mTDMetallic);
            mBuilder.WriteAttribute("lightface", mTDLightFace);
            mBuilder.WriteSet("rotationangle", mTDRotationAngleX, mTDRotationAngleY);
            mBuilder.WriteSet("orientation", mTDRotationAxisX, mTDRotationAxisY, mTDRotationAxisZ);
            mBuilder.WriteAttribute("orientationangle", mTDRotationAngle);
            mBuilder.WriteSet("rotationcenter", mTDRotationCenterX, mTDRotationCenterY, mTDRotationCenterZ);
            mBuilder.WriteAttribute("render", mTDRenderMode);
            mBuilder.WriteAttribute("facet", mTDTolerance);
            mBuilder.WriteSet("viewpoint", mTDViewpointX, mTDViewpointY, mTDViewpointZ);
            mBuilder.WriteSet("viewpointorigin", mTDOriginX, mTDOriginY);
            mBuilder.WriteAttribute("skewangle", mTDSkewAngle);
            mBuilder.WriteAttribute("skewamt", mTDSkewAmount);
            mBuilder.WriteAttribute("brightness", mTDAmbientIntensity);
            mBuilder.WriteSet("lightposition", mTDKeyX, mTDKeyY, mTDKeyZ);
            mBuilder.WriteAttribute("lightlevel", mTDKeyIntensity);
            mBuilder.WriteSet("lightposition2", mTDFillX, mTDFillY, mTDFillZ);
            mBuilder.WriteAttribute("lightlevel2", mTDFillIntensity);
            // colormode - ignored in WordML
            mBuilder.WriteAttribute("lockrotationcenter", mTDConstrainRotation);
            mBuilder.WriteAttribute("autorotationcenter", mTDRotationCenterAuto);
            mBuilder.WriteAttribute("type", mTDParallel);
            mBuilder.WriteAttribute("lightharsh", mTDKeyHarsh);
            mBuilder.WriteAttribute("lightharsh2", mTDFillHarsh);

            mBuilder.EndElement(); //o:extrusion
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly IVmlShapeWriterContext mContext;

        private int mAttrCount = 0;

        private string mTDSpecularAmount = null;
        private string mTDDiffuseAmount = null;
        private string mTDShininess = null;
        private string mTDEdgeThickness = null;
        private string mTDExtrudeForward = null;
        private string mTDExtrudeBackward = null;
        private string mTDExtrudePlane = null;
        private string mTDExtrusionColor = null;
        private string mTDOn = null;
        private string mTDMetallic = null;
        private string mTDLightFace = null;
        private string mTDRotationAngleY = null;
        private string mTDRotationAngleX = null;
        private string mTDRotationAxisX = null;
        private string mTDRotationAxisY = null;
        private string mTDRotationAxisZ = null;
        private string mTDRotationAngle = null;
        private string mTDRotationCenterX = null;
        private string mTDRotationCenterY = null;
        private string mTDRotationCenterZ = null;
        private string mTDRenderMode = null;
        private string mTDTolerance = null;
        private string mTDViewpointX = null;
        private string mTDViewpointY = null;
        private string mTDViewpointZ = null;
        private string mTDOriginX = null;
        private string mTDOriginY = null;
        private string mTDSkewAngle = null;
        private string mTDSkewAmount = null;
        private string mTDAmbientIntensity = null;
        private string mTDKeyX = null;
        private string mTDKeyY = null;
        private string mTDKeyZ = null;
        private string mTDKeyIntensity = null;
        private string mTDFillX = null;
        private string mTDFillY = null;
        private string mTDFillZ = null;
        private string mTDFillIntensity = null;
        private string mTDConstrainRotation = null;
        private string mTDRotationCenterAuto = null;
        private string mTDParallel = null;
        private string mTDKeyHarsh = null;
        private string mTDFillHarsh = null;

        //v:ext    Yes    
        //specularity    Yes    
        //diffusity    Yes    
        //shininess    Yes    
        //edge    Yes    
        //foredepth    Yes    
        //backdepth    Yes    
        //plane    Yes    
        //color    Yes    
        //on    Yes    
        //metal    Yes    
        //lightface    Yes    
        //rotationangle    Yes    
        //orientation    Yes    
        //orientationangle    Yes    
        //rotationcenter    Yes    
        //render    Yes    
        //facet    Yes    
        //viewpoint    Yes    
        //viewpointorigin    Yes    
        //skewangle    Yes    
        //skewamt    Yes    
        //brightness    Yes    
        //lightposition    Yes    
        //lightlevel    Yes    
        //lightposition2    Yes    
        //lightlevel2    Yes    
        //colormode    No need    Ignored in WordML.
        //lockrotationcenter    Yes    
        //autorotationcenter    Yes    
        //type    Yes    
        //lightharsh    Yes    
        //lightharsh2    Yes    
    }
}
