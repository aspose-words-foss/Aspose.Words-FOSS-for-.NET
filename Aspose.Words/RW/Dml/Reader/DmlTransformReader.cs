// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Dml.Reader
{
    internal class DmlTransformReader : DmlReaderBase
    {
        private DmlTransformReader(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            mReader = reader;
            mComplianceInfo = complianceInfo;
        }

        /// <summary>
        /// 20.1.7.5 xfrm (2D Transform for Grouped Objects)
        /// This element is nearly identical to the representation of 2-D transforms for ordinary shapes (20.1.7.6).
        /// The only addition is a member to represent the Child offset and the Child extents.
        /// </summary>
        internal static DmlGroupTransform ReadGroupTransform(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            if (reader.LocalName != "xfrm")
                return null;

            DmlTransformReader transformReader = new DmlTransformReader(reader, complianceInfo);
            DmlGroupTransform transform = new DmlGroupTransform();
            transformReader.Read2DTransformForGroupedObjects(transform);
            return transform;
        }

        /// <summary>
        /// 20.1.7.6 xfrm (2D Transform for Individual Objects)
        /// This element represents 2-D transforms for ordinary shapes.
        /// </summary>
        internal static DmlTransform ReadTransform(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            if (reader.LocalName != "xfrm" && reader.LocalName != "txXfrm")
                return null;

            DmlTransformReader transformReader = new DmlTransformReader(reader, complianceInfo);

            DmlTransform transform = new DmlTransform();
            transformReader.Read2DTransformForIndividualObjects(transform);
            return transform;
        }

        private void Read2DTransformForGroupedObjects(DmlGroupTransform transform)
        {
            ReadAttributes(transform);
            while (mReader.ReadChild("xfrm"))
            {
                switch (mReader.LocalName)
                {
                    case "ext":
                        ReadExtents(transform);
                        break;
                    case "off":
                        ReadOffset(transform);
                        break;
                    case "chExt":
                        transform.ChildWidth = mReader.ReadDoubleAttribute("cx", 0.0);
                        transform.ChildHeight = mReader.ReadDoubleAttribute("cy", 0.0);
                        break;
                    case "chOff":
                        transform.ChildX = mReader.ReadAttributeAsEmus("x", 0.0, mComplianceInfo);
                        transform.ChildY = mReader.ReadAttributeAsEmus("y", 0.0, mComplianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        private void Read2DTransformForIndividualObjects(DmlTransform transform)
        {
            string elementName = mReader.LocalName;
            ReadAttributes(transform);
            while (mReader.ReadChild(elementName))
            {
                switch (mReader.LocalName)
                {
                    case "ext":
                        ReadExtents(transform);
                        break;
                    case "off":
                        ReadOffset(transform);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mReader);
                        break;
                }
            }
        }

        private void ReadAttributes(DmlTransform transform)
        {
            bool flipH = mReader.ReadBoolAttribute("flipH", false);
            bool flipV = mReader.ReadBoolAttribute("flipV", false);

            if (flipH)
                transform.FlipOrientation = FlipOrientation.Horizontal;

            if (flipV)
                transform.FlipOrientation = FlipOrientation.Vertical;

            if (flipH && flipV)
                transform.FlipOrientation = FlipOrientation.Both;

            transform.Rotation = DmlAngle.CreateWithNormalization(mReader.ReadDoubleAttribute("rot", 0.0));
        }

        private void ReadOffset(DmlTransform transform)
        {
            transform.X = mReader.ReadAttributeAsEmus("x", 0.0, mComplianceInfo);
            transform.Y = mReader.ReadAttributeAsEmus("y", 0.0, mComplianceInfo);
        }

        private void ReadExtents(DmlTransform transform)
        {
            transform.Width = mReader.ReadAttributeAsEmus("cx", 0.0, mComplianceInfo);
            transform.Height = mReader.ReadAttributeAsEmus("cy", 0.0, mComplianceInfo);
        }

        private readonly NrxXmlReader mReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
