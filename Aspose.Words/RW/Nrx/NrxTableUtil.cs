// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/07/2009 by Roman Korchagin

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx
{
    internal static class NrxTableUtil
    {
        /// <summary>
        /// Returns true if the chain of table styles referenced by this table properties defines the specified attribute.
        /// </summary>
        internal static bool HasAttrInTableStyles(DocumentBase doc, TablePr tablePr, int key)
        {
            TableStyle style = (TableStyle)doc.Styles.GetByIstd(tablePr.Istd, false);
            while (style != null)
            {
                if (style.RowPr.ContainsKey(key))
                    return true;

                if (style.TablePr.ContainsKey(key))
                    return true;

                style = (TableStyle)style.GetBaseStyle();
            }
            return false;
        }

        /// <summary>
        /// Inverts RowBands and ColumnBands because in the model they are inverted.
        /// </summary>
        internal static int TableStyleOptionsToNrx(TableStyleOptions options)
        {
            return (int)TablePr.InvertTableBands(options);
        }

        /// <summary>
        /// Inverts RowBands and ColumnBands because in the model they are inverted.
        /// </summary>
        internal static TableStyleOptions NrxToTableStyleOptions(int value)
        {
            return TablePr.InvertTableBands((TableStyleOptions)value);
        }

        /// <summary>
        /// Saves an AW equivalent of the given tblpX/tblpY to table properties.
        /// </summary>
        /// <remarks>
        /// MS IMPL notes say that we have to decrement tblpY and tblpX by one upon loading docx/wml.
        /// MS Word also disregards 0 values for those attributes. Do not write 0 from xml to the model.
        /// 0 in the model corresponds to 1 in xml.
        /// 0 in the model is different from no value in the model for those attributes.
        /// </remarks>
        internal static void SetPositionAttribute(TablePr tablePr, int key, int xmlValue)
        {
            Debug.Assert(key == TableAttr.FrameLeft || key == TableAttr.FrameTop);

            // Do not write 0 values from oxml to the model. MS Word disregards them and removes them on re-saving.
            if (xmlValue != 0)
                // Write the decremented value.
                tablePr[key] = --xmlValue;
        }
    }
}
