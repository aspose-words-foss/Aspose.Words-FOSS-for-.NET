// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    internal class DmlScene3DReader : DmlReaderBase
    {
        private DmlScene3DReader(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            mDocumentReader = reader;
            mXmlReader = reader.XmlReader;
            mComplianceInfo = complianceInfo;
        }

        internal static DmlScene3DProperties ReadScene3DProperties(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            return ReadScene3DProperties(reader, false, complianceInfo);
        }

        internal static DmlScene3DProperties ReadScene3DProperties(DocxDocumentReaderBase reader, bool isTheme,
            OoxmlComplianceInfo complianceInfo)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlScene3DReader scene3DReader = new DmlScene3DReader(reader, complianceInfo);

            DmlScene3DProperties scene3D = new DmlScene3DProperties(isTheme);

            while (xmlReader.ReadChild("scene3d"))
            {
                switch (xmlReader.LocalName)
                {
                    case "backdrop":
                        scene3D.BackdropPlane = scene3DReader.ReadBackdropPlane();
                        break;
                    case "camera":
                        scene3DReader.ReadCameraProperties(scene3D.Camera);
                        break;
                    case "lightRig":
                        scene3DReader.ReadLightRigProperties(scene3D.LightRig);
                        break;
                    case "extLst":
                        scene3D.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return scene3D;
        }

        internal static DmlShape3DProperties ReadShape3DProperties(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            return ReadShape3DProperties(reader, false, complianceInfo);
        }

        internal static DmlShape3DProperties ReadShape3DProperties(DocxDocumentReaderBase reader, bool isTheme,
            OoxmlComplianceInfo complianceInfo)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlScene3DReader shape3DReader = new DmlScene3DReader(reader, complianceInfo);
            string tagName = xmlReader.LocalName;

            DmlShape3DProperties shape3DProperties = new DmlShape3DProperties(isTheme);

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "z":
                        shape3DProperties.Z = xmlReader.GetValueAsEmus(complianceInfo);
                        break;
                    case "extrusionH":
                        shape3DProperties.ExtrusionHeight = xmlReader.ValueAsDouble;
                        break;
                    case "contourW":
                        shape3DProperties.ContourWidth = xmlReader.ValueAsDouble;
                        break;
                    case "prstMaterial":
                        shape3DProperties.PresetMaterial = DmlEnum.DmlToPresetMaterial(xmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "bevelT":
                        shape3DProperties.BevelTop = shape3DReader.ReadBevel();
                        break;
                    case "bevelB":
                        shape3DProperties.BevelBottom = shape3DReader.ReadBevel();
                        break;
                    case "extrusionClr":
                    {
                        while (xmlReader.ReadChild("extrusionClr"))
                        {
                            DmlColor color = DmlColorReader.Read(xmlReader, complianceInfo);
                            if ((shape3DProperties.ExtrusionColor == null) && (color != null))
                                shape3DProperties.ExtrusionColor = color;
                        }
                        break;
                    }
                    case "contourClr":
                    {
                        while (xmlReader.ReadChild("contourClr"))
                        {
                            DmlColor color = DmlColorReader.Read(xmlReader, complianceInfo);
                            if ((shape3DProperties.ContourColor == null) && (color != null))
                                shape3DProperties.ContourColor = color;
                        }
                        break;
                    }
                    case "extLst":
                        shape3DProperties.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return shape3DProperties;
        }

        private void ReadLightRigProperties(DmlLightRig lightRig)
        {
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "dir":
                        lightRig.Direction = DmlEnum.DmlToLightRigDirection(mXmlReader.Value);
                        break;
                    case "rig":
                        lightRig.LightRigType = DmlEnum.DmlToLightRigType(mXmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(mXmlReader);
                        break;
                }
            }

            mXmlReader.MoveToElement();
            while (mXmlReader.ReadChild("lightRig"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "rot":
                        lightRig.Rotation = ReadRotation();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }
        }

        private void ReadCameraProperties(DmlCamera camera)
        {
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "fov":
                        camera.FovAngle = new DmlAngle(mXmlReader.ValueAsDouble);
                        break;
                    case "prst":
                        camera.PresetCameraType = DmlEnum.DmlToPresetCameraType(mXmlReader.Value);
                        break;
                    case "zoom":
                        camera.Zoom = DmlPercentageUtil.FromPercentOrDmlPercent(mXmlReader.Value, mComplianceInfo);
                        break;
                    default:
                        WarnUnexpected(mXmlReader);
                        break;
                }
            }

            mXmlReader.MoveToElement();
            while (mXmlReader.ReadChild("camera"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "rot":
                        camera.Rotation = ReadRotation();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }
        }

        private DmlBackdropPlane ReadBackdropPlane()
        {
            DmlBackdropPlane backdrop = new DmlBackdropPlane();

            while (mXmlReader.ReadChild("backdrop"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "anchor":
                        backdrop.Anchor = ReadPoint3D();
                        break;
                    case "norm":
                        backdrop.NormalVector = ReadVector3D();
                        break;
                    case "up":
                        backdrop.UpVector = ReadVector3D();
                        break;
                    case "extLst":
                        backdrop.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return backdrop;
        }

        private DmlBevel ReadBevel()
        {
            DmlBevel bevel = new DmlBevel();

            bevel.Width = mXmlReader.ReadDoubleAttribute("w", 76200d);
            bevel.Height = mXmlReader.ReadDoubleAttribute("h", 76200d);
            bevel.BevelPresetType = DmlEnum.DmlToBevelPresetType(mXmlReader.ReadAttribute("prst", ""));

            return bevel;
        }

        private DmlRotation3D ReadRotation()
        {
            DmlRotation3D rot = new DmlRotation3D();
            rot.Latitude = new DmlAngle(mXmlReader.ReadDoubleAttribute("lat", 0.0d));
            rot.Longitude = new DmlAngle(mXmlReader.ReadDoubleAttribute("lon", 0.0d));
            rot.Revolution = new DmlAngle(mXmlReader.ReadDoubleAttribute("rev", 0.0d));
            return rot;
        }

        private DmlPoint3D ReadPoint3D()
        {
            DmlPoint3D point = new DmlPoint3D();
            point.X = mXmlReader.ReadAttributeAsEmus("x", 0.0d, mComplianceInfo);
            point.Y = mXmlReader.ReadAttributeAsEmus("y", 0.0d, mComplianceInfo);
            point.Z = mXmlReader.ReadAttributeAsEmus("z", 0.0d, mComplianceInfo);
            return point;
        }

        private DmlPoint3D ReadVector3D()
        {
            DmlPoint3D point = new DmlPoint3D();
            point.X = mXmlReader.ReadAttributeAsEmus("dx", 0.0d, mComplianceInfo);
            point.Y = mXmlReader.ReadAttributeAsEmus("dy", 0.0d, mComplianceInfo);
            point.Z = mXmlReader.ReadAttributeAsEmus("dz", 0.0d, mComplianceInfo);
            return point;
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
        private readonly NrxXmlReader mXmlReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
