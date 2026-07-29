// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using System.Text;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents ether tx (Chart Text) (5.7.2.215) or tx (Series Text) (5.7.2.216) element.
    /// </summary>
    internal class DmlChartTx : DmlExtensionListSource
    {
        internal DmlChartTx Clone()
        {
            DmlChartTx lhs = (DmlChartTx)MemberwiseClone();
            if (mStrRef != null)
                lhs.mStrRef = mStrRef.Clone();

            if (mRichText != null)
                lhs.mRichText = mRichText.Clone();

            if (mFormula != null)
                lhs.mFormula = mFormula.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Returns text representation of the Tx.
        /// </summary>
        internal string GetText()
        {
            if (RichText != null)
            {
                StringBuilder reachStrBuilder = new StringBuilder();

                foreach (DmlParagraph paragraph in RichText.Paragraphs)
                    reachStrBuilder.Append(paragraph.GetRunText());

                return reachStrBuilder.ToString();
            }
            else
            {
                return PlainText;
            }
        }

        /// <summary>
        /// Returns the font size in EMU of the first paragraph.
        /// </summary>
        /// <returns>Font size in EMU</returns>
        internal float GetFirstParagraphFontSizeInEmu()
        {
            float fontSizeInEmu = float.NaN;

            if ((FirstParagraph != null) && FirstParagraph.Properties.HasDefaultRunProperties)
                fontSizeInEmu = (float)FirstParagraph.Properties.DefaultRunProperties.FontSize.ValueInEmus;

            return fontSizeInEmu;
        }
       
        internal DmlChartTxType TxType
        {
            get { return mTxType; }
        }

        internal DmlChartValueRef StrRef
        {
            get { return mStrRef; }
            set { mStrRef = value; }
        }

        internal string PlainText
        {
            get
            {
                if (!StringUtil.HasChars(mPlainText) && mStrRef != null)
                {
                    // It seems when there is string ref, plain text must return concatenated string of all points,
                    // instead of the first one only.
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < mStrRef.Values.ValueCount; i++)
                    {
                        DmlChartValue value = mStrRef.Values[i];
                        if (value != null)
                        {
                            builder.Append(value.StringValue);
                            builder.Append(" ");
                        }
                    }

                    // Trim redundant whitespace at the end.
                    mPlainText = builder.ToString().Trim();
                }

                return mPlainText;
            }
            set
            {
                // If plain text property is set consider tx as series text.
                mTxType = DmlChartTxType.SeriesText;
                mSourcePlainText = value;
                mPlainText = value;
                mStrRef = null;
            }
        }

        /// <summary>
        /// Returns value of plain text read from the document.
        /// Used for writing correct value from the model.
        /// </summary>
        internal string SourcePlainText
        {
            get { return mSourcePlainText; }
        }

        internal DmlTextBody RichText
        {
            get { return mRichText; }
            set
            {
                // If reach text property is set consider tx as chart text.
                mTxType = DmlChartTxType.ChartText;
                mRichText = value;
            }
        }

        /// <summary>
        /// Gets or sets a formula that specifies text data reference.
        /// </summary>
        /// <remarks>
        /// This is the f element of the 2.24.3.79 CT_TextData complex type [MS-ODRAWXML]
        /// </remarks>
        internal DmlChartFormula Formula
        {
            get { return mFormula; }
            set
            {
                mFormula = value;
                // If formula is set consider tx as text data reference.
                if (mFormula != null)
                    mTxType = DmlChartTxType.TextData;
            }
        }

        /// <summary>
        /// Indicates whether the default properties are specified for the first paragraph.
        /// </summary>
        internal bool HasDefaultRunPr
        {
           get { return (FirstParagraph != null) && FirstParagraph.Properties.HasDefaultRunProperties; }
        }

        /// <summary>
        /// Shortcut to the first paragraph in the reach text collection.
        /// If the collection is empty, return null.
        /// </summary>
        internal DmlParagraph FirstParagraph
        {
            get
            {
                // WORDSNET-21691, WORDSNET-21478 Check the number of paragraphs before taking the first one.
                if ((RichText != null)  && (RichText.Paragraphs.Count > 0) && (RichText.Paragraphs[0] != null))
                    return RichText.Paragraphs[0];

                return null;
            }
        }

        /// <summary>
        /// Lets consider tx as series text by default.
        /// </summary>
        private DmlChartTxType mTxType = DmlChartTxType.SeriesText;
        private DmlChartValueRef mStrRef;
        private string mPlainText;
        private string mSourcePlainText;
        private DmlTextBody mRichText;
        private DmlChartFormula mFormula;
    }

    internal enum DmlChartTxType
    {
        /// <summary>
        /// Text is series text (the 21.2.2.215 tx element). Text may be defined as plain text only.
        /// </summary>
        SeriesText,
        /// <summary>
        /// Text is chart text (the 21.2.2.214 tx element). Text includes rich text formatting.
        /// </summary>
        ChartText,
        // In chartEx the elements are defined in another way, but meaning is the same. So, same values are used.
        /// <summary>
        /// The text is a text data element (2.24.3.79 CT_TextData [MS-ODRAWXML]).
        /// </summary>
        TextData = SeriesText,
        /// <summary>
        /// The text is a rich text element (CT_TextBody [ISO/IEC29500-1:2012] section A.4.1).
        /// </summary>
        RichText = ChartText
    }
}
