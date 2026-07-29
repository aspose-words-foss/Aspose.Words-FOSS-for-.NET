// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Provides access to the scaling options of the display units for the value axis.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// Corresponds to the dispUnits element (5.7.2.45, 21.2.2.45).
    /// </dev>
    public class AxisDisplayUnit : IDmlChartTitleHolder, IDmlExtensionListSource
    {
        /// <summary>
        /// Gets or sets the scaling value of the display units as one of the predefined values.
        /// </summary>
        /// <remarks>
        /// Default value is <see cref="AxisBuiltInUnit.None"/>. The <see cref="AxisBuiltInUnit.Custom"/> and
        /// <see cref="AxisBuiltInUnit.Percentage"/> values are not available in some chart types; see
        /// <see cref="AxisBuiltInUnit"/> for more information.
        /// </remarks>
        public AxisBuiltInUnit Unit
        {
            get { return mUnit; }
            set
            {
                if (mAxis != null && mAxis.Chart != null)
                {
                    DmlChartType chartType = mAxis.Chart.ChartType;
                    if ((value == AxisBuiltInUnit.Custom) && (chartType == DmlChartType.ChartExChart))
                        throw new ArgumentException(
                            "The AxisBuiltInUnit.Custom value is not allowed in MS Office 2016 new charts.");
                    if ((value == AxisBuiltInUnit.Percentage) && (chartType != DmlChartType.ChartExChart))
                        throw new ArgumentException(string.Format(
                            "The AxisBuiltInUnit.Percentage value is not supported by the {0} chart type.", chartType));
                }
                mUnit = value;
            }
        }

        /// <summary>
        /// Gets or sets a user-defined divisor to scale display units on the value axis.
        /// </summary>
        /// <remarks>
        /// <para>The property is not supported by MS Office 2016 new charts. Default value is 1.</para>
        /// <para>Setting this property sets the <see cref="Unit"/> property to
        /// <see cref="AxisBuiltInUnit.Custom"/>.</para>
        /// </remarks>
        public double CustomUnit
        {
            get { return mCustomUnit; }
            set
            {
                mUnit = AxisBuiltInUnit.Custom;
                mCustomUnit = value;
            }
        }

        /// <summary>
        /// Provides access to properties of the display unit label for the value axis.
        /// </summary>
        /// <dev>
        /// The property is intended for public interface in future.
        /// </dev>
        internal DmlChartTitle Label
        {
            get { return mLabel; }
        }

        /// <summary>
        /// Gets a flag indicating whether display units are defined.
        /// </summary>
        internal bool IsDefined
        {
            get
            {
                return (mUnit != AxisBuiltInUnit.None) || (mLabel != null) ||
                    ((mExtensions != null) && (mExtensions.Count > 0));
            }
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

        /// <summary>
        /// Sets axis, to which this object belongs.
        /// </summary>
        internal void SetAxis(ChartAxis axis)
        {
            mAxis = axis;
        }

        /// <summary>
        /// Sets display unit label.
        /// </summary>
        internal void SetLabel(DmlChartDisplayUnitsLabel value)
        {
            mLabel = value;
        }

        #region IDmlChartTitleHolder members

        int IDmlChartTitleHolder.GetRelativeFontSize(int chartFontSize)
        {
            return chartFontSize; // An axis display unit uses font size defined on the chart without changes.
        }

        DmlChartTitle IDmlChartTitleHolder.DCTitle
        {
            get { return Label; }
            set
            {
                // Do nothing here, DmlChartDisplayUnits actually does not have title,
                // DmlChartDisplayUnitsLabel is rendered instead of title.
            }
        }

        TitlePosition IDmlChartTitleHolder.TitlePosition
        {
            get
            {
                bool crossesMax = (mAxis.Crosses == AxisCrosses.Maximum);
                bool labelsLowAndCrossesMax = (mAxis.TickLabels.Position == AxisTickLabelPosition.Low) && crossesMax;
                bool labelsHighAndNotCrossesMax = (mAxis.TickLabels.Position == AxisTickLabelPosition.High) && !crossesMax;

                switch (mAxis.ActualAxisPosition)
                {
                    case AxisPosition.Top:
                        return labelsLowAndCrossesMax? TitlePosition.BottomRight : TitlePosition.TopRight;
                    case AxisPosition.Bottom:
                        return labelsHighAndNotCrossesMax ? TitlePosition.TopRight : TitlePosition.BottomRight;
                    case AxisPosition.Left:
                        return labelsHighAndNotCrossesMax ? TitlePosition.RightTop : TitlePosition.LeftTop;
                    case AxisPosition.Right:
                        return labelsLowAndCrossesMax ? TitlePosition.LeftTop : TitlePosition.RightTop;
                    default:
                        return TitlePosition.TopRight;
                }
            }
        }

        /// <summary>
        /// Indicates whether the title of display unit extends beyond the chart space.
        /// </summary>
        /// <remarks>
        /// WORDSNET-24266 MS Words in some cases renders display unit title outside the chart space.
        /// </remarks>
        internal bool DoesGoBeyondChart
        {
            get
            {
                TitlePosition basedOnAxisPosition;

                switch (mAxis.ActualAxisPosition)
                {
                    case AxisPosition.Top:
                        basedOnAxisPosition = TitlePosition.TopRight;
                        break;
                    case AxisPosition.Bottom:
                        basedOnAxisPosition = TitlePosition.BottomRight;
                        break;
                    case AxisPosition.Left:
                        basedOnAxisPosition = TitlePosition.LeftTop;
                        break;
                    case AxisPosition.Right:
                        basedOnAxisPosition = TitlePosition.RightTop;
                        break;
                    default:
                        basedOnAxisPosition = TitlePosition.TopRight;
                        break;
                }

                return ((IDmlChartTitleHolder)this).TitlePosition != basedOnAxisPosition;
            }
        }

        /// <summary>
        /// Returns the document containing the parent chart.
        /// </summary>
        public DocumentBase Document
        {
            get { return mAxis.Document; }
        }

        bool IDmlChartTitleHolder.TitleDeleted
        {
            get { return false;}
            set
            {
                // Do nothing, because DmlChartDisplayUnits actually does not have title.
                // DmlChartDisplayUnitsLabel is rendered instead of title.
            }
        }

        /// <summary>
        /// Indicates whether the display unit is visible.
        /// </summary>
        bool IDmlChartTitleHolder.IsVisible
        {
            get { return (Label != null); }
        }

        /// <summary>
        /// Gets the default title text.
        /// </summary>
        string IDmlChartTitleHolder.DefaultTitleText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the default font size in points.
        /// </summary>
        double IDmlChartTitleHolder.DefaultFontSize
        {
            get { return mAxis.Chart.ChartSpace.IsChartEx ? 9 : 10; }
        }

        /// <summary>
        /// Gets the title font size in points that MS Word sets for created charts.
        /// </summary>
        double IDmlChartTitleHolder.DefaultDisplayedFontSize
        {
            get { return ((IDmlChartTitleHolder)this).DefaultFontSize; }
        }

        DmlChartStyleItem IDmlChartTitleHolder.StyleItem
        {
            get { return DmlChartStyleItem.ValueAxis; }
        }

        /// <summary>
        /// Gets the parent chart space.
        /// </summary>
        DmlChartSpace IDmlChartTitleHolder.ChartSpace
        {
            get { return mAxis.Chart.ChartSpace; }
        }

        #endregion

        /// <summary>
        /// Clones this instance of display units.
        /// </summary>
        internal AxisDisplayUnit Clone()
        {
            AxisDisplayUnit lhs = (AxisDisplayUnit)MemberwiseClone();

            if (mLabel != null)
            {
                lhs.mLabel = (DmlChartDisplayUnitsLabel)mLabel.Clone();
                lhs.mLabel.SetTitleHolder(lhs);
            }

            if (mExtensions != null)
                lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            return lhs;
        }

        /// <summary>
        /// Returns text that should be rendered as <see cref="Label"/> if its text is not set explicitly.
        /// </summary>
        /// <returns></returns>
        internal string GetDisplayUnitsLabel()
        {
            switch (mUnit)
            {
                case AxisBuiltInUnit.Hundreds:
                    return "Hundreds";
                case AxisBuiltInUnit.Thousands:
                    return "Thousands";
                case AxisBuiltInUnit.Millions:
                    return "Millions";
                case AxisBuiltInUnit.Billions:
                    return "Billions";
                case AxisBuiltInUnit.Trillions:
                    return "Trillions";
                default:
                    return string.Format("x {0}", GetNumberLabel());
            }
        }

        /// <summary>
        /// Returns number text that should be rendered as <see cref="Label"/> if its text is not set explicitly.
        /// </summary>
        internal string GetNumberLabel()
        {
            double unit = GetActualUnit();

            return ((unit < 1e-9) || (unit > 9.9999999999e10))
                ? FormatterPal.DoubleToExponential(unit)
                : FormatterPal.DoubleToStr9Decimals(unit);
        }

        /// <summary>
        /// Returns double value that should be used as divider of actual value before displaying.
        /// </summary>
        internal double GetActualUnit()
        {
            switch (mUnit)
            {
                case AxisBuiltInUnit.Hundreds:
                    return 100.0d;
                case AxisBuiltInUnit.Thousands:
                    return 1000.0d;
                case AxisBuiltInUnit.TenThousands:
                    return 10000.0d;
                case AxisBuiltInUnit.HundredThousands:
                    return 100000.0d;
                case AxisBuiltInUnit.Millions:
                    return 1000000.0d;
                case AxisBuiltInUnit.TenMillions:
                    return 10000000.0d;
                case AxisBuiltInUnit.HundredMillions:
                    return 100000000.0d;
                case AxisBuiltInUnit.Billions:
                    return 1000000000.0d;
                case AxisBuiltInUnit.Trillions:
                    return 1000000000000.0d;
                case AxisBuiltInUnit.Custom:
                    return mCustomUnit;
                default:
                    return 1.0d;
            }
        }

        private AxisBuiltInUnit mUnit = AxisBuiltInUnit.None;
        private double mCustomUnit = 1;
        private DmlChartDisplayUnitsLabel mLabel;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartAxis mAxis;
        private StringToObjDictionary<DmlExtension> mExtensions;
    }
}
