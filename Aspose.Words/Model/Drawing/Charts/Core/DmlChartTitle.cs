// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Provides access to the chart title properties.
    /// </summary>
    /// <dev>
    /// 5.7.2.211 title (Title) element.
    /// </dev>
    internal class DmlChartTitle : IDmlExtensionListSource, IChartItemTextProperties, IChartFormatSource
    {
        internal DmlChartTitle(IDmlChartTitleHolder chartTitleHolder)
        {
            mTitleHolder = chartTitleHolder;
        }

        internal virtual DmlChartTitle Clone()
        {
            DmlChartTitle lhs = (DmlChartTitle)MemberwiseClone();

            CopyComplexPropertiesTo(lhs);

            return lhs;
        }

        /// <summary>
        /// Sets the holder of this title.
        /// </summary>
        internal void SetTitleHolder(IDmlChartTitleHolder value)
        {
            mTitleHolder = value;
        }

        /// <summary>
        /// Copies complex properties to the specified chart title instance.
        /// </summary>
        protected void CopyComplexPropertiesTo(DmlChartTitle destination)
        {
            if (mSpPr != null)
                destination.mSpPr = mSpPr.Clone();

            if (mTx != null)
                destination.mTx = mTx.Clone();

            if (mTxPr != null)
                destination.mTxPr = mTxPr.Clone();

            if (mLayout != null)
                destination.mLayout = mLayout.Clone();

            if (mExtensions != null)
                destination.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);
        }

        /// <summary>
        /// Ensures that the title format is initialized.
        /// </summary>
        internal void EnsureInitialized()
        {
            if ((mTx != null) || (TxPr.Paragraphs.Count > 0))
                return;

            TxPr.EnsureParagraphExists();
            TxPr.FirstParagraph.Properties.DefaultRunProperties = GetDefaultRunProperties();
            TxPr.FirstParagraph.Properties.HasDefaultRunProperties = true;
            mTitleHolder.ChartSpace.ApplyChartStyle(TxPr, mTitleHolder.StyleItem);
        }

        /// <summary>
        /// Creates and assigns <see cref="Tx"/> and its <see cref="DmlChartTx.RichText"/> property if they are not
        /// defined yet.
        /// </summary>
        private void EnsureTxMinimum()
        {
            if (mTx == null)
                mTx = new DmlChartTx();

            if (mTx.RichText == null)
                mTx.RichText = CreateEmptyReachTx();
        }

        /// <summary>
        /// Creates empty reach text with one paragraph and one run.
        /// Applies default run properties.
        /// </summary>
        private DmlTextBody CreateEmptyReachTx()
        {
            // Create new empty chart text.
            DmlTextBody textBody = new DmlTextBody();

            // Assign existing TxPr to preserve text formatting.
            if ((mTxPr != null) && !mTxPr.BodyPr.IsEmpty)
                textBody.Properties = mTxPr.BodyPr.Clone();

            // There is an issue with Word 2016 charts if these properties are not set.
            if (IsChartEx)
                SetTitleAnchor(textBody.Properties);

            DmlRunProperties runProperties = FontRunPropertiesSource.GeneralRunProperties;
            if ((runProperties == null) || runProperties.IsEmpty)
                runProperties = !TxPr.IsEmpty ? TxPr.RunPr : GetDefaultRunProperties();

            DmlParagraph paragraph = new DmlParagraph(null, null);

            paragraph.Properties.HasDefaultRunProperties = true;
            paragraph.Properties.DefaultRunProperties = runProperties.Clone();

            DmlRun run = new DmlRun();
            paragraph.AddElement(run);
            textBody.AddParagraph(paragraph);

            return textBody;
        }

        /// <summary>
        /// Sets the necessary text body anchor properties, which is required for Word 2016 charts.
        /// </summary>
        private static void SetTitleAnchor(DmlTextBodyProperties textBodyProperties)
        {
            textBodyProperties.Anchor = DmlTextAnchoringType.Center;
            textBodyProperties.AnchorCenter = true;
        }

        /// <summary>
        /// Gets default run properties for the title.
        /// </summary>
        private DmlRunProperties GetDefaultRunProperties()
        {
            if (mTitleHolder.ChartSpace.DmlChartStyle == null)
            {
                DmlRunProperties runProperties = new DmlRunProperties();
                runProperties.FontSize = DmlTextPoints.FromPoints(mTitleHolder.DefaultDisplayedFontSize);
                runProperties.Bold = false;
                runProperties.Italics = false;
                runProperties.Underline = Underline.None;
                runProperties.Strikethrough = DmlTextStrike.No;
                runProperties.Kerning = new DmlTextPoints(IsChartEx ? 0 : 1200);
                runProperties.Spacing = new DmlTextPoints(0);
                runProperties.Baseline = 0;

                DmlSchemeColor schemeColor = new DmlSchemeColor(ThemeColor.Text1);
                DmlLuminanceModulation lMod = new DmlLuminanceModulation();
                lMod.Value = 0.65;
                DmlLuminanceOffset lOff = new DmlLuminanceOffset();
                lOff.Value = 0.35;
                schemeColor.ColorModifiers.Add(lMod);
                schemeColor.ColorModifiers.Add(lOff);
                runProperties.Fill = new DmlSolidFill(schemeColor);

                return runProperties;
            }
            else
            {
                DmlChartTxPr txPr = new DmlChartTxPr();
                mTitleHolder.ChartSpace.ApplyChartStyle(txPr, mTitleHolder.StyleItem);
                return txPr.RunPr;
            }
        }

        /// <summary>
        /// Gets properties of the first element of the specified text body, if the element exists, or run properties
        /// of the first paragraph if it doesn't. These are default run properties of the title.
        /// </summary>
        private static DmlRunProperties GetFirstElementProperties(DmlTextBody textBody)
        {
            foreach (DmlParagraph paragraph in textBody.Paragraphs)
            {
                foreach (DmlParagraphTextElementBase element in paragraph.Elements)
                    return element.RunProperties;
            }

            return textBody.Paragraphs[0].EndParagraphRunProperties;
        }

        #region IChartItemTextProperties members

        string IChartItemTextProperties.GenerateItemText()
        {
            return mTitleHolder.DefaultTitleText;
        }

        object IChartItemTextProperties.FetchSpecialDefaultRunPropertyValue(int key)
        {
            if (key == FontAttr.Size)
                return ConvertUtilCore.PointToHalfPoint(mTitleHolder.DefaultFontSize);

            if ((key == FontAttr.Bold) && !IsChartEx)
                return AttrBoolEx.True;

            return null;
        }

        object IChartItemTextProperties.GetRelativePropertyValue(int key, object value)
        {
            if ((value != null) && ((key == FontAttr.Size) || (key == FontAttr.SizeBi)))
                return mTitleHolder.GetRelativeFontSize((int)value);
            else
                return value;
        }

        DmlChartTx IChartItemTextProperties.ItemTx
        {
            get { return Tx; }
        }

        DmlChartTxPr IChartItemTextProperties.ItemTxPr
        {
            get { return TxPr; }
            set { mTxPr = value; }
        }

        DmlChartSpPr IChartItemTextProperties.ItemSpPr
        {
            get { return SpPr; }
        }

        DmlChartTxPr IChartItemTextProperties.CollectionTxPr
        {
            get { return null; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the text of the chart title.
        /// If <c>null</c> or empty value is specified, auto generated title will be shown.
        /// </summary>
        internal string Text
        {
            get
            {
                return (Tx == null) ? mTitleHolder.DefaultTitleText : Tx.GetText();
            }
            set
            {
                // If no text is specified, the auto generated title will be shown.
                if (!StringUtil.HasChars(value))
                {
                    if ((mTx == null) && ((mTxPr == null) || mTxPr.IsEmpty))
                        return;

                    // Keep the existing text format: just set the default title text.
                    value = mTitleHolder.DefaultTitleText;
                }

                EnsureTxMinimum();

                // Get properties of the first run or of paragraph end if there are no runs.
                DmlRunProperties runProperties = GetFirstElementProperties(Tx.RichText);

                // Remove all dml paragraphs except first one, to preserve formatting.
                List<DmlParagraph> paragraphs = Tx.RichText.Paragraphs;
                while (paragraphs.Count != 1)
                    paragraphs.RemoveAt(paragraphs.Count - 1);

                DmlParagraph firstPara = paragraphs[0];
                firstPara.Elements.Clear();

                DmlRun run = new DmlRun(runProperties.Clone());
                firstPara.AddElement(run);

                Tx.StrRef = null;
                run.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the title text.
        /// </summary>
        public ShapeTextOrientation Orientation
        {
            get { return BodyPr.TextOrientation; }
            set { BodyPrForUpdate.TextOrientation = value; }
        }

        /// <summary>
        /// Gets or sets the rotation of the title in degrees.
        /// </summary>
        /// <remarks>
        /// The range of acceptable values is from -180 to 180 inclusive.
        /// </remarks>
        public int Rotation
        {
            get { return (int)System.Math.Round(BodyPr.Rotation.ValueInDegrees); }
            set { BodyPrForUpdate.Rotation = DmlAngle.FromDegrees(value); }
        }

        /// <summary>
        /// Gets or sets the side position of the chart title.
        /// </summary>
        /// <remarks>
        /// Default side position is top.
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </remarks>
        internal SidePosition SidePosition
        {
            get { return mSidePosition; }
            set { mSidePosition = value; }
        }

        /// <summary>
        /// Gets or sets the alignment along the side position of the chart title.
        /// </summary>
        /// <remarks>
        /// Default side position is center.
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </remarks>
        internal PositionAlignment PositionAlignment
        {
            get { return mPositionAlignment; }
            set { mPositionAlignment = value; }
        }

        /// <summary>
        /// Determines whether other chart elements shall be allowed to overlap title.
        /// By default overlay is <c>false</c>.
        /// </summary>
        internal bool Overlay
        {
            get { return mOverlay; }
            set { mOverlay = value; }
        }

        /// <summary>
        /// Determines whether the title shall be shown for this chart.
        /// </summary>
        internal bool Show
        {
            get { return !mTitleHolder.TitleDeleted; }
        }

        /// <summary>
        /// Returns an object that represents current font formatting properties.
        /// </summary>
        internal Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(FontRunPropertiesSource, Document);

                return mFont;
            }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the title.
        /// </summary>
        internal ChartFormat Format
        {
            get
            {
                if (mFormat == null)
                    mFormat = new ChartFormat(this);

                return mFormat;
            }
        }

        internal DmlChartManualLayout Layout
        {
            get { return mLayout; }
            set { mLayout = value; }
        }

        internal DmlChartSpPr SpPr
        {
            get
            {
                if (mSpPr == null)
                    mSpPr = new DmlChartSpPr();

                return mSpPr;
            }
            set { mSpPr = value; }
        }

        internal DmlChartTx Tx
        {
            get { return mTx; }
            set { mTx = value; }
        }

        internal DmlChartTxPr TxPr
        {
            get
            {
                if (mTxPr == null)
                    mTxPr = new DmlChartTxPr();

                return mTxPr;
            }
        }

        /// <summary>
        /// Gets a <see cref="DmlTextBodyProperties"/> instance to get text properties of the title.
        /// </summary>
        private DmlTextBodyProperties BodyPr
        {
            get
            {
                if ((Tx != null) && (Tx.RichText != null))
                    return Tx.RichText.Properties;

                return TxPr.BodyPr;
            }
        }

        /// <summary>
        /// Gets a <see cref="DmlTextBodyProperties"/> instance to set text properties of the title.
        /// </summary>
        private DmlTextBodyProperties BodyPrForUpdate
        {
            get
            {
                if ((Tx != null) && (Tx.RichText != null))
                    return Tx.RichText.Properties;

                TxPr.EnsureBodyPrExists();
                return TxPr.BodyPr;
            }
        }

        /// <summary>
        /// Gets a <see cref="ChartItemDmlRunPropertiesSource"/> instance that is the source of the title font properties.
        /// </summary>
        private ChartItemDmlRunPropertiesSource FontRunPropertiesSource
        {
            get
            {
                if (mFontRunPropertiesSource == null)
                    mFontRunPropertiesSource = new ChartItemDmlRunPropertiesSource(this, mTitleHolder.ChartSpace);

                return mFontRunPropertiesSource;
            }
        }

        /// <summary>
        /// Gets the parent document.
        /// </summary>
        private DocumentBase Document
        {
            get { return mTitleHolder.ChartSpace.Dml.Document; }
        }

        /// <summary>
        /// Gets a flag indicating whether the parent chart is a Word 2016 chart.
        /// </summary>
        private bool IsChartEx
        {
            get { return mTitleHolder.ChartSpace.IsChartEx; }
        }

        /// <summary>
        /// Represents extLst: a CT_OfficeArtExtensionList ([ISO/IEC29500-1:2012] section A.4.1) element that specifies
        /// the extension list in which all future extensions of element type ext is defined.
        ///  </summary>
        /// <remarks>
        /// Explicit implementation hides this from public.
        /// </remarks>
        StringToObjDictionary<DmlExtension> IDmlExtensionListSource.Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // Do nothing for a title.
        }

        bool IChartFormatSource.IsFillSupported
        {
            get { return true; }
        }

        DmlFill IChartFormatSource.Fill
        {
            get { return SpPr.Fill; }
            set { SpPr.Fill = value; }
        }

        DmlOutline IChartFormatSource.Outline
        {
            get { return SpPr.Outline; }
            set { SpPr.Outline = value; }
        }

        ChartShapeType IChartFormatSource.ShapeType
        {
            get
            {
                // Not supported by a title.
                return ChartShapeType.Default;
            }
            set
            {
                // Not supported by a title.
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return !SpPr.IsEmpty; }
        }

        #endregion

        private StringToObjDictionary<DmlExtension> mExtensions;
        private DmlChartManualLayout mLayout;
        private bool mOverlay;
        private DmlChartSpPr mSpPr;
        private DmlChartTx mTx;
        private DmlChartTxPr mTxPr;
        private Font mFont;
        private ChartItemDmlRunPropertiesSource mFontRunPropertiesSource;
        private ChartFormat mFormat;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private IDmlChartTitleHolder mTitleHolder;
        private SidePosition mSidePosition = SidePosition.Top;
        private PositionAlignment mPositionAlignment = PositionAlignment.Center;
    }
}
