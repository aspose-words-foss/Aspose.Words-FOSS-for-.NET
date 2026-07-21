// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2010 by Denis Darkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx
{
    /// <summary>
    /// Knows how to read/write new ISO29500 style <see cref="TableStyleOptions"/> element.
    /// </summary>
    internal class DocxTableStyleLookReader
    {
        /// <summary>
        /// Reads w:tblLook elements and returns <see cref="TableStyleOptions"/>.
        /// </summary>
        internal static TableStyleOptions ReadTableStyleLook(NrxXmlReader xmlReader, 
            OoxmlComplianceInfo complianceInfo)
        {
            TableStyleOptions result = TableStyleOptions.Default;
            string initialVal = xmlReader.ReadVal();
            if (StringUtil.HasChars(initialVal))
                result = NrxTableUtil.NrxToTableStyleOptions(NrxXmlUtil.HexToInt(initialVal));

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "firstRow":
                        complianceInfo.MarkAsIsoTransitional();
                        result = (TableStyleOptions)BitUtil.SetBit((int)result, (int)TableStyleOptions.FirstRow, xmlReader.ValueAsBool);
                        break;
                    case "firstColumn":
                        complianceInfo.MarkAsIsoTransitional();
                        result = (TableStyleOptions)BitUtil.SetBit((int)result, (int)TableStyleOptions.FirstColumn, xmlReader.ValueAsBool);
                        break;
                    case "lastColumn":
                        complianceInfo.MarkAsIsoTransitional();
                        result = (TableStyleOptions)BitUtil.SetBit((int)result, (int)TableStyleOptions.LastColumn, xmlReader.ValueAsBool);
                        break;
                    case "lastRow":
                        complianceInfo.MarkAsIsoTransitional();
                        result = (TableStyleOptions)BitUtil.SetBit((int)result, (int)TableStyleOptions.LastRow, xmlReader.ValueAsBool);
                        break;
                    case "noHBand":
                        complianceInfo.MarkAsIsoTransitional();
                        // In the model this is inverted.
                        result = (TableStyleOptions)BitUtil.SetBit((int)result, (int)TableStyleOptions.RowBands, !xmlReader.ValueAsBool);
                        break;
                    case "noVBand":
                        complianceInfo.MarkAsIsoTransitional();
                        // In the model this is inverted.
                        result = (TableStyleOptions)BitUtil.SetBit((int)result, (int)TableStyleOptions.ColumnBands, !xmlReader.ValueAsBool);
                        break;
                    default:
                        continue;
                }
            }
            
            return result;
        }
    }
}
