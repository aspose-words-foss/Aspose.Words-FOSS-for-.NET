// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/02/2011 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Represents a class building DmlShapeStyle objects from xml.
    /// </summary>
    internal class DmlShapeStyleReader : DmlReaderBase
    {
        private DmlShapeStyleReader(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            mReader = reader;
            mComplianceInfo = complianceInfo;
        }

        internal static DmlShapeStyle Read(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            if (reader.LocalName != "style")
                return null;

            DmlShapeStyleReader shapeStyleReader = new DmlShapeStyleReader(reader, complianceInfo);

            DmlShapeStyle style = new DmlShapeStyle();
            while (reader.ReadChild("style"))
            {
                switch (reader.LocalName)
                {
                    case "effectRef":
                        style.EffectReference = shapeStyleReader.ReadEffectReference();
                        break;
                    case "fillRef":
                        style.FillReference = shapeStyleReader.ReadFillReference();
                        break;
                    case "fontRef":
                        style.FontReference = shapeStyleReader.ReadFontReference();
                        break;
                    case "lnRef":
                        style.LineReference = shapeStyleReader.ReaLineReference();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
            return style;
        }

        private DmlLineReference ReaLineReference()
        {
            DmlLineReference lineReference = new DmlLineReference();
            lineReference.StyleMatrixIndex = mReader.ReadIntAttribute("idx", 0);
            ReadReferenceContent(lineReference);
            return lineReference;
        }


        private DmlFontReference ReadFontReference()
        {
            DmlFontReference fontReference = new DmlFontReference();
            string idxValue = mReader.ReadAttribute("idx", String.Empty);
            fontReference.FontCollectionIndex = DmlEnum.DmlToFontCollectionIndex(idxValue);
            ReadReferenceContent(fontReference);
            return fontReference;
        }

        private DmlFillReference ReadFillReference()
        {
            DmlFillReference fillReference = new DmlFillReference();
            fillReference.StyleMatrixIndex = mReader.ReadIntAttribute("idx", 0);
            ReadReferenceContent(fillReference);
            return fillReference;
        }

        private DmlEffectReference ReadEffectReference()
        {
            DmlEffectReference effectReference = new DmlEffectReference();
            effectReference.StyleMatrixIndex = mReader.ReadIntAttribute("idx", 0);
            ReadReferenceContent(effectReference);
            return effectReference;
        }

        private void ReadReferenceContent(DmlStyleReferenceBase reference)
        {
            string currentTagName = mReader.LocalName;
            while (mReader.ReadChild(currentTagName))
            {
                DmlColor newColor = DmlColorReader.Read(mReader, mComplianceInfo);
                // Reader returns null if it founds unknown tag.
                // We filter null value to disable overwriting of
                // already initialized Color property by null values.
                if (newColor != null)
                    reference.Color = newColor;
            }
        }

        private readonly NrxXmlReader mReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
