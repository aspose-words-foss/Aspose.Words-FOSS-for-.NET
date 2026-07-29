// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using System;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Nrx;

namespace Aspose.Words.Drawing.Core.Dml.Readers
{
    /// <summary>
    /// Represents a class building DrawingML shape by xml
    /// </summary>
    internal class DmlGeometryReader : DmlReaderBase
    {
        private DmlGeometryReader(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            mReader = reader;
            mComplianceInfo = complianceInfo;
        }

        internal static DmlGeometry Read(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            DmlGeometryReader geometryReader = new DmlGeometryReader(reader, complianceInfo);
            return geometryReader.ReadCore();
        }

        private DmlGeometry ReadCore()
        {
            switch (mReader.LocalName)
            {
                case "custGeom":
                    DmlGeometry geometry = new DmlGeometry();
                    ReadGeometry("custGeom", false, geometry);
                    return geometry;
                case "prstGeom":
                    string presetName = mReader.ReadAttribute("prst", "");
                    DmlGeometry presetGeometry = GetPresetGeometry(presetName);

                    if (presetGeometry == null)
                        return new DmlGeometry();

                    ReadGeometry("prstGeom", false, presetGeometry);
                    return presetGeometry;
                default:
                    WarnUnexpectedAndIgnoreElement(mReader);
                    return new DmlGeometry();
            }
        }

        internal static DmlGeometry GetPresetGeometry(string presetName)
        {
            IPresetGeometryXmlRepository repository = new PresetGeometryXmlRepository();
            string presetXml = repository.GetPresetGeometryXml(presetName);
            if (!StringUtil.HasChars(presetXml))
                return null;

            string actualPresetName = presetName;
            if (presetName == "rect")
            {
                // We have to rewrite xml because XmlReader
                // doesn't handle properly following xml:
                // <rect>... <rect/>...</rect>
                presetXml = "<rect1" + presetXml.Substring(5, presetXml.Length - 10) + "rect1>";
                actualPresetName = "rect1";
            }

            NrxXmlReader xmlReader = new NrxXmlReader(presetXml, null);
            DmlGeometryReader geometryReader = new DmlGeometryReader(xmlReader, null);

            DmlGeometry geometry = new DmlGeometry();
            geometry.PresetName = presetName;
            geometryReader.ReadGeometry(actualPresetName, true, geometry);

            return geometry;
        }

        /// <summary>
        /// Reads the geometry.
        /// </summary>
        /// <remarks>
        /// 20.1.9.8 custGeom (Custom Geometry)
        /// This element specifies the existence of a custom geometric shape. This shape
        /// consists of a series of lines and curves described within a creation path.
        /// In addition to this there can also be adjust values, guides, adjust handles,
        /// connection sites and an inscribed rectangle specified for this custom geometric shape.
        /// </remarks>
        private void ReadGeometry(string tag, bool isPreset, DmlGeometry geometry)
        {
            while (mReader.ReadChild(tag))
            {
                switch (mReader.LocalName)
                {
                    case "pathLst":
                        ReadPathList(geometry);
                        break;
                    case "avLst":
                        ReadAdjustableValues(geometry.Guides, isPreset, mReader, mGuidesNames);
                        break;
                    case "gdLst":
                        ReadGuides(geometry, isPreset);
                        break;
                    case "rect":
                        geometry.DmlTextboxRect = ReadTextboxRect();
                        break;
                    case "ahLst":
                        ReadAdjustHandleList(geometry);
                        break;
                    case "cxnLst":
                        ReadConnectionSites(geometry);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }

            ValidateConnectionSites(geometry);
        }

        /// <summary>
        /// Reads list of shape connection sites.
        /// </summary>
        /// <remarks>
        /// 20.1.9.10 cxnLst (List of Shape Connection Sites)
        /// This element specifies all the connection sites that are used for this shape. A connection site is
        /// specified by defining a point within the shape bounding box that can have a cxnSp element attached
        /// to it. These connection sites are specified using the shape coordinate system that is specified
        /// within the ext transform element.
        /// </remarks>
        private void ReadConnectionSites(DmlGeometry geometry)
        {
            Debug.Assert(geometry != null);

            while (mReader.ReadChild("cxnLst"))
            {
                switch (mReader.LocalName)
                {
                    case "cxn":
                        ReadConnectionSite(geometry);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        /// <summary>
        ///  Reads shape connection site element.
        /// </summary>
        /// <param name="geometry">Object with geometry properties.</param>
        private void ReadConnectionSite(DmlGeometry geometry)
        {
            Debug.Assert(geometry != null);

            string angle = mReader.ReadAttribute("ang", "");

            // Document can not be opened by MSW, when attribute "angle" are missing or empty.
            // For resilience purposes just skip current connection site.
            if (!CheckAttribute(angle, true))
                return;

            DmlConnectionSite site = new DmlConnectionSite(angle);

            // Read "pos" child element.
            while (mReader.ReadChild("cxn"))
            {
                switch (mReader.LocalName)
                {
                    case "pos":
                        ReadShapePosition(site);
                        break;
                    default:
                        // Skip any another elements.
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }

            // Document can not be opened by MSW, when "pos" element is missing.
            // For resilience purposes just skip current connection site.
            if (site.Coordinates == null)
                WarningUtil.Warn(mReader.WarningCallback, WarningType.UnexpectedContent,
                  WarningSource.DrawingML, WarningStrings.UnacceptableMarkup);
            else
                geometry.ConnectionSites.Add(site);
        }

        /// <summary>
        /// Reads a position coordinate within the shape bounding box.
        /// </summary>
        /// <param name="site">Connection site which holds position element.</param>
        private void ReadShapePosition(DmlConnectionSite site)
        {
            Debug.Assert(site != null);

            string xCoord = "";
            string yCoord = "";

            while (mReader.MoveToNextAttribute())
            {
                switch (mReader.LocalName)
                {
                    case "x":
                        xCoord = mReader.Value;
                        break;
                    case "y":
                        yCoord = mReader.Value;
                        break;
                    default:
                        break;
                }
            }

            // Document can not be opened by MSW, when coordinates attributes are missing or empty.
            // For resilience purposes just skip current connection site.
            if (CheckAttribute(xCoord, false) && CheckAttribute(yCoord, false))
                site.Coordinates = new DmlAdjustablePoint(xCoord, yCoord);
        }

        /// <summary>
        /// Checks that connection site attributes have valid values and
        /// if values are not correct then skip such sites.
        /// </summary>
        /// <param name="geometry">Object with geometry properties.</param>
        private void ValidateConnectionSites(DmlGeometry geometry)
        {
            Debug.Assert(geometry != null);

            if (geometry.ConnectionSites.Count == 0)
                return;

            // This method was added for resilience purposes.
            List<DmlConnectionSite> validSites = new List<DmlConnectionSite>();

            foreach(DmlConnectionSite site in geometry.ConnectionSites)
            {
                string ang = site.Angle.String;

                DmlAdjustablePoint coord = site.Coordinates;
                string xCoord = coord.X.String;
                string yCoord = coord.Y.String;

                double dValX= FormatterPal.TryParseDoubleInvariant(xCoord);
                double dValY= FormatterPal.TryParseDoubleInvariant(yCoord);
                double dValAng = FormatterPal.TryParseDoubleInvariant(ang);

                // Position of the connection site can store as an absolute coordinate position
                // or a reference to a geometry guide. If reference is invalid then skip this site.
                // MSW does not open such documents.
                if ((Double.IsNaN(dValX) && !mGuidesNames.Contains(xCoord)) ||
                    (Double.IsNaN(dValY) && !mGuidesNames.Contains(yCoord)) ||
                    (Double.IsNaN(dValAng) && !mGuidesNames.Contains(ang)))
                    WarningUtil.Warn(mReader.WarningCallback, WarningType.UnexpectedContent,
                         WarningSource.DrawingML, WarningStrings.UnacceptableMarkup);
                else
                    validSites.Add(site);
            }

            // Replace collection with valid connection sites.
            geometry.ConnectionSites = validSites;
        }

        /// <summary>
        /// Checks that attribute value is not empty, otherwise register warning
        /// and skip current element if it is requested.
        /// </summary>
        /// <param name="val">Attribute value for check.</param>
        /// <param name="registerWarn">When true and value is empty, then register warning and skip element.</param>
        /// <returns>True when attribute is not empty.</returns>
        private bool CheckAttribute(string val, bool registerWarn)
        {
            bool hasChars = StringUtil.HasChars(val);

            if (!hasChars && registerWarn)
            {
                WarningUtil.Warn(mReader.WarningCallback, WarningType.UnexpectedContent,
                    WarningSource.DrawingML, WarningStrings.UnacceptableMarkup);
                mReader.IgnoreElementNoWarn();
            }

            return hasChars;
        }

        private DmlTextBoxRect ReadTextboxRect()
        {
            string l = "";
            string t = "";
            string r = "";
            string b = "";

            // Read attributes.
            while (mReader.MoveToNextAttribute())
            {
                switch (mReader.LocalName)
                {
                    case "l":
                        l = mReader.Value;
                        CheckForIsoTransitional(l);
                        break;
                    case "t":
                        t = mReader.Value;
                        CheckForIsoTransitional(t);
                        break;
                    case "r":
                        r = mReader.Value;
                        CheckForIsoTransitional(r);
                        break;
                    case "b":
                        b = mReader.Value;
                        CheckForIsoTransitional(b);
                        break;
                    default:
                        break;
                }
            }

            // There must be no children.
            while (mReader.ReadChild("rect"))
                WarnUnexpectedAndIgnoreElement(mReader);

            return new DmlTextBoxRect(l, t, r, b);
        }

        /// <summary>
        /// Checks if the input value is in format that is introduced by ISO/IEC 29500.
        /// Sets the ISO Transitional flag in this case.
        /// </summary>
        private void CheckForIsoTransitional(string value)
        {
            if ((mComplianceInfo != null) && (NrxXmlReader.IsUniversalMeasure(value)))
                mComplianceInfo.MarkAsIsoTransitional();
        }

        /// <summary>
        /// Reads guides
        /// </summary>
        /// <remarks>
        /// 20.1.9.12 gdLst (List of Shape Guides)
        /// This element specifies all the guides that are used for this shape.
        /// A guide is specified by the gd element and defines a calculated value
        /// that can be used for the construction of the corresponding shape.
        /// </remarks>
        private void ReadGuides(DmlGeometry geometry, bool isPreset)
        {
            while (mReader.ReadChild("gdLst"))
            {
                switch (mReader.LocalName)
                {
                    case "gd":
                        ReadGuide(false, geometry.Guides, isPreset, mReader, mGuidesNames);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads guide and collects guide name when collection to store is specified.
        /// </summary>
        /// <param name="isAdjustableValue">True when adjustable value is used.</param>
        /// <param name="guides">Guide collection.</param>
        /// <param name="isPreset">True when preset geometric shape is used.</param>
        /// <param name="reader">Input data reader.</param>
        /// <param name="guidesNames">Dictionary to collect guides names.</param>
        private static void ReadGuide(bool isAdjustableValue, DmlGuides guides, bool isPreset, NrxXmlReader reader,
            HashSetGeneric<string> guidesNames)
        {
            string guideName = reader.ReadAttribute("name", String.Empty);
            string guideFormula = reader.ReadAttribute("fmla", String.Empty);
            if ((guideFormula == String.Empty) || (guideName == String.Empty))
                return;

            if (isAdjustableValue)
                guides.AddAdjustableValue(guideName, guideFormula, isPreset);
            else
                guides.AddGuide(guideName, guideFormula, isPreset);

            if (guidesNames != null)
                guidesNames.Add(guideName);
        }

        internal static void ReadGuide(bool isAdjustableValue, DmlGuides guides, bool isPreset, NrxXmlReader reader)
        {
            ReadGuide(isAdjustableValue, guides, isPreset, reader, null);
        }

        /// <summary>
        /// Reads adjustable values
        /// </summary>
        /// <remarks>
        /// 20.1.9.5 avLst (List of Shape Adjust Values)
        /// This element specifies
        /// the adjust values that are applied to the specified shape.
        /// An adjust value is simply a guide that has a value based formula
        /// specified. That is, no calculation takes place for an adjust value guide.
        /// Instead, this guide specifies a parameter value that is used for calculations within the shape guides.
        /// </remarks>
        internal static void ReadAdjustableValues(DmlGuides guides, bool isPreset, NrxXmlReader reader)
        {
            ReadAdjustableValues(guides, isPreset, reader, null);
        }

        /// <summary>
        /// Reads adjustable value and collects guide name when collection to store is specified.
        /// </summary>
        /// <param name="guides">Guide collection.</param>
        /// <param name="isPreset">True when preset geometric shape is used.</param>
        /// <param name="reader">Input data reader.</param>
        /// <param name="guidesNames">Dictionary to collect guides names.</param>
        private static void ReadAdjustableValues(DmlGuides guides, bool isPreset, NrxXmlReader reader,
            HashSetGeneric<string> guidesNames)
        {
              while (reader.ReadChild("avLst"))
            {
                switch (reader.LocalName)
                {
                    case "gd":
                        ReadGuide(true, guides, isPreset, reader, guidesNames);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the path list.
        /// </summary>
        /// <remarks>
        /// 20.1.9.16 pathLst (List of Shape Paths)
        /// This element specifies the entire path that is to make up a single geometric shape. The pathLst can consist
        /// of many individual paths within it.
        /// </remarks>
        private void ReadPathList(DmlGeometry geometry)
        {
            while (mReader.ReadChild("pathLst"))
            {
                switch (mReader.LocalName)
                {
                    case "path":
                        DmlPath path = DmlPathReader.Read(mReader, mComplianceInfo);
                        geometry.AddPath(path);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        private void ReadAdjustHandleList(DmlGeometry geometry)
        {
            // Currently adjust handles are only required to determine adjust value type for diagrams.
            // So read only guide reference attributes.
            while (mReader.ReadChild("ahLst"))
            {
                switch (mReader.LocalName)
                {
                    case "ahPolar":
                        DmlAdjustHandlePolar polar = new DmlAdjustHandlePolar();
                        polar.GdRefAng = mReader.ReadAttribute("gdRefAng", "");
                        polar.GdRefR = mReader.ReadAttribute("gdRefR", "");
                        geometry.AdjustHandles.Add(polar);
                        break;
                    case "ahXY":
                        DmlAdjustHandleXY xy = new DmlAdjustHandleXY();
                        xy.GdRefX = mReader.ReadAttribute("gdRefX", "");
                        xy.GdRefY = mReader.ReadAttribute("gdRefY", "");
                        geometry.AdjustHandles.Add(xy);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        private readonly NrxXmlReader mReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;

        /// <summary>
        /// Dictionary which holds names of the guides.
        /// </summary>
        private readonly HashSetGeneric<string> mGuidesNames = new HashSetGeneric<string>();
    }
}
