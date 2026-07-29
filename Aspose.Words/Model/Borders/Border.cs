// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin
using System;
using System.ComponentModel;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a border of an object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    ///    <remarks>
    ///    <p>Borders can be applied to various document elements including paragraph,
    ///    run of text inside a paragraph or a table cell.</p>
    ///    </remarks>
    /// <dev>
    /// This is a model and also presentation class that allows to see and modify border attributes.
    /// </dev>
    public class Border : InternableComplexAttr, IComplexAttr
    {
        /// <summary>
        /// Creates a non-inherited border.
        /// </summary>
        internal Border()
        {
            ClearFormatting();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="lineStyle">The style of the line.</param>
        /// <param name="rawLineWidth">Width of the line in 1/8pt point.</param>
        /// <param name="color">Color of the line.</param>
        internal Border(LineStyle lineStyle, int rawLineWidth, DrColor color)
        {
            Debug.Assert(color != null);
            mLineStyle = lineStyle;
            mRawLineWidth = rawLineWidth;
            mColor = color;
        }

        /// <summary>
        /// Creates an inherited border.
        /// </summary>
        internal Border(IBorderAttrSource parent, int key)
        {
            mParent = parent;
            mKey = key;
        }

        /// <summary>
        /// Resets border properties to default values.
        /// </summary>
        /// <remarks>
        /// When border properties are reset to default values, the border is invisible.
        /// </remarks>
        public void ClearFormatting()
        {
            BeforeChange();

            mParent = null;
            mLineStyle = LineStyle.None;
            mRawLineWidth = 0;
            mColor = DrColor.Empty;
            mRawDistanceFromText = 0;
            mShadow = false;
            mFrame = false;
            mThemeColor = null;
            mThemeShade = null;
            mThemeTint = null;
        }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        /// <remarks>
        /// <para>If you set line style to none, then line width is automatically changed to zero.</para>
        /// </remarks>
        /// <dev>
        /// Currently AW distinguish Nil and None borders.
        /// This public setter is used by customer and always creates Nil border for <see cref="LineStyle.None"/>
        /// Readers and internal code should use <see cref="LineStyleInternal" /> setter with <see cref="LineStyle.None"/> to create None.Border and
        /// <see cref="CreateNilBorder()" /> method to create Nil border.
        /// </dev>
        public LineStyle LineStyle
        {
            get { return (IsInherited) ? Inherited.LineStyle : mLineStyle; }
            set
            {
                LineStyleInternal = value;

                // andrnosk: WORDSNET-7706 Mimic MS Word behavior, assign minimal (1/4pt) width in case when border style is specified
                // and line widths equals zero.
                if ((value != LineStyle.None) && MathUtil.IsZero(LineWidth))
                    LineWidth = 0.25;

                // This setter is used by customer and always creates Nil border for LineStyle.None.
                mIsNil = (mLineStyle == LineStyle.None);
            }
        }

        /// <summary>
        /// Sets the border style.
        /// </summary>
        internal LineStyle LineStyleInternal
        {
            set
            {
                BeforeChange();
                mLineStyle = value;
                // WORDSNET-1133 Setting Border.LineStyle to LineStyle.None without explicitly setting
                // Border.LineWidth to 0 creates documents that crash MS Word 2000 and MS Word 2002.
                if (value == LineStyle.None)
                    LineWidth = 0;

                mIsNil = false;
            }
        }

        /// <summary>
        /// Gets or sets the border width in points.
        /// </summary>
        /// <remarks>
        /// <p>If you set line width greater than zero when line style is none, the line style is
        /// automatically changed to single line.</p>
        /// </remarks>
        public double LineWidth
        {
            get
            {
                if (IsInherited)
                    return Inherited.LineWidth;

                // When border is page border art, raw line width is in points already.
                // I don't like such logic, but have no better ideas at this stage.
                return (IsPageBorderArt) ? mRawLineWidth : ConvertUtilCore.EightsPointToPoint(mRawLineWidth);
            }
            set
            {
                SetLineWidthCore(value, true);

                ValidateNoneBorder();
            }
        }


        /// <summary>
        /// Validates border having line width greater than zero cannot be None border.
        /// </summary>
        /// <dev>
        /// AM. Actually combination of LineStyle.None and non zero line width is allowed in Word.
        /// I made fix for WORDSNET-1133 at the logical level while should do it at DOC export level.
        /// Unfortunately, now some code relies on this logic and that worse seems it commonly used public API behavior.
        /// That's why I decided to make only partial solution to fix WORDSNET-25597 only and
        /// try to preserve other usages.
        /// </dev>
        internal void ValidateNoneBorder()
        {
            // WORDSNET-1133 Creating a border with non zero width but line style none crashes MS Word 2000.
            if ((LineWidth > 0) && (LineStyle == LineStyle.None))
                LineStyleInternal = LineStyle.Single;
        }

        /// <summary>
        /// Gets the border line width in points.
        /// Currently needed to export to several formats.
        /// MS Words shows borders if line style is anything but None even if width is zero.
        /// </summary>
        internal double LineWidthZeroAware
        {
            get
            {
                double originalLineWidth = LineWidth;
                if ((originalLineWidth != 0) || !IsVisible)
                    return originalLineWidth;

                const double lineWidthOutputByDefault = 0.25;
                return lineWidthOutputByDefault;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this border style is page border art.
        /// The line style property should actually be cast to the PageBorderArt enum.
        /// </summary>
        internal bool IsPageBorderArt
        {
            get { return (int)LineStyle >= (int)PageBorderArt.Apples; }
        }

        /// <summary>
        /// Returns <c>true</c> if the <see cref="LineStyle"/> is not <see cref="LineStyle.None"/>.
        /// </summary>
        public bool IsVisible
        {
            get { return (LineStyle != LineStyle.None); }
        }

        /// <summary>
        /// Returns <c>true</c> if the line style is not LineStyle.None and line width
        /// is greater than zero or distance is greater that zero.
        /// </summary>
        /// <remarks>
        /// 1. Moved from AE.
        /// 2. Dependence on "distance" is included by Jira7344.
        /// </remarks>
        internal static bool GetIsVisible(LineStyle lineStyle, float lineWidth, float distance)
        {
            return ((lineStyle != LineStyle.None) && (lineWidth > 0)) || distance > 0;
        }

        /// <summary>
        /// Returns the actual width (in points) of the border.
        ///
        /// This method correctly calculates actual width of multipart borders.
        /// For example, the "line width" could be 0.5pt, but the total
        /// width of say 4 parts will be 2.5pt.
        ///
        /// Does not include shadow and distance from text.
        ///
        /// Returns zero if the line style is none.
        /// </summary>
        internal float BorderWidth
        {
            get { return GetActualWidth(LineStyle, (float)LineWidthZeroAware); }
        }

        /// <summary>
        /// Returns the total width of the border (in points) including shadow and distance from text.
        /// </summary>
        internal float TotalWidthAndDistance
        {
            get
            {
                if (IsVisible)
                {
                    float result = BorderWidth;

                    if (Shadow)
                        result *= 2;

                    result += (float)DistanceFromText;

                    return result;
                }

                return 0f;
            }
        }

        /// <summary>
        ///  Gets or sets the border color.
        /// </summary>
        public System.Drawing.Color Color
        {
            get { return ColorInternal.ToNativeColor(); }
            set
            {
                ColorInternal = DrColor.FromNativeColor(value);

                // WORDSNET-15143 Reset theme color when RGB color is set.
                mThemeColor = null;
                mThemeShade = null;
                mThemeTint = null;
            }
        }

        internal DrColor ColorInternal
        {
            get { return (IsInherited) ? Inherited.ColorInternal : mColor; }
            set
            {
                Debug.Assert(value != null);
                BeforeChange();
                mColor = value;
            }
        }

        /// <summary>
        /// Gets or sets distance of the border from text or from the page edge in points.
        /// </summary>
        /// <remarks>
        /// Has no effect and will be automatically reset to zero for borders of table cells.
        ///
        /// <seealso cref="PageSetup.BorderDistanceFrom"/>
        /// </remarks>
        public double DistanceFromText
        {
            get { return (IsInherited) ? Inherited.DistanceFromText : mRawDistanceFromText; }
            set { SetDistanceFromTextCore(value, true); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the border has a shadow.
        /// </summary>
        /// <remarks>
        /// <p>In Microsoft Word, for a border to have a shadow, the borders on all four sides
        /// (left, top, right and bottom) should be of the same type, width, color and all should have
        /// the Shadow property set to <c>true</c>.</p>
        /// </remarks>
        public bool Shadow
        {
            get { return (IsInherited) ? Inherited.Shadow : mShadow; }
            set
            {
                BeforeChange();
                mShadow = value;
            }
        }

        /// <summary>
        /// Gets or sets the theme color in the applied color scheme that is associated with this Border object.
        /// </summary>
        public ThemeColor ThemeColor
        {
            get
            {
                return ThemeColorConverter.FromString(mThemeColor);
            }
            set
            {
                mThemeColor = ThemeColorConverter.ToString(value);
                // WORDSNET-24325 Reset RGB color when theme color is set.
                mColor = DrColor.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens a color.
        /// </summary>
        /// <remarks>
        /// <para> The allowed values are in the range from -1 (the darkest) to 1 (the lightest) for this property.
        /// Zero (0) is neutral.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Throws if you attempt to set this property to a value less than -1 or more than 1.</exception>
        /// <exception cref="InvalidOperationException">Throws if setting this property for Border object with non-theme colors.</exception>
        public double TintAndShade
        {
            get
            {
                // Note, Word stores inverted values (1 - tint) and (1 + shade) in the document.
                return StringUtil.HasChars(mThemeTint)
                    ? 1.0d - ((double)FormatterPal.ParseHex(mThemeTint) / 255)
                    : StringUtil.HasChars(mThemeShade)
                        ? -1.0d - ((double)FormatterPal.ParseHex(mThemeShade) / -255)
                        : 0.0d;
            }
            set
            {
                // Despite https://docs.microsoft.com/en-us/office/vba/api/excel.font.tintandshade
                // says that "This property works for both theme colors and non-theme colors.",
                // in fact it does not work in Word VBA for non-theme colors.
                if (!StringUtil.HasChars(ThemeColorInternal))
                    throw new InvalidOperationException("TintAndShade cannot be applied to a non-theme color.");

                ArgumentUtil.CheckRangeInclusive(value, -1.0d, 1.0d, "TintAndShade");

                ThemeTint = null;
                ThemeShade = null;
                // Note, Word stores inverted value (1 - tint) in the document.
                if (value > 0)
                    ThemeTint = FormatterPal.IntToStrX2((int)((1 - value) * 255));
                // Note, Word stores inverted value (1 + shade) in the document.
                if (value < 0)
                    ThemeShade = FormatterPal.IntToStrX2((int)((-1 - value) * -255));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create a frame effect by reversing the border.
        /// </summary>
        internal bool Frame
        {
            get { return (IsInherited) ? Inherited.Frame : mFrame; }
            set
            {
                BeforeChange();
                mFrame = value;
            }
        }

        /// <summary>
        /// At the moment just plain unparsed value from DOCX.
        /// Specifies the base theme color used to generate the border color. The border color is the
        /// RGB value associated with themeColor as further transformed by themeTint or themeShade (if one is present),
        /// else the background color is the RGB value associated with themeColor.
        /// </summary>
        internal string ThemeColorInternal
        {
            get { return mThemeColor; }
            set { mThemeColor = value; }
        }

        /// <summary>
        /// At the moment just plain unparsed value from DOCX.
        /// Specifies the shade value applied to the supplied theme color (if any) for this border
        /// instance. If the themeColor attribute is not present, then this attribute shall not be used.
        /// </summary>
        internal string ThemeShade
        {
            get { return mThemeShade; }
            set { mThemeShade = value; }
        }

        /// <summary>
        /// At the moment just plain unparsed value from DOCX.
        /// </summary>
        internal string ThemeTint
        {
            get { return mThemeTint; }
            set { mThemeTint = value; }
        }

        /// <summary>
        /// Gets/sets line width in "raw" units (as stored in DOC or in WordML).
        /// For normal borders the value is in 1/8th of a point.
        /// For page border art, the value is in points.
        /// </summary>
        internal int RawLineWidth
        {
            get { return mRawLineWidth; }
            set
            {
                mRawLineWidth = value;
            }
        }

        /// <summary>
        /// Gets/sets space between border and text in points?
        /// </summary>
        internal int RawDistanceFromText
        {
            get { return mRawDistanceFromText; }
            set
            {
                mRawDistanceFromText = value;
            }
        }

        /// <summary>
        /// Returns the number of parts this border consists of.
        /// </summary>
        internal int PartsCount
        {
            get { return GetPartsCount(LineStyle); }
        }

        internal void SetLineWidthSafe(double lineWidth)
        {
            SetLineWidthCore(lineWidth, false);
        }

        /// <summary>
        /// Note this is a new design pattern for setting values.
        /// A public property should call this method with isThrow = true so it throws when user sets invalid value.
        /// But importers should call this method with isThrow = false so it sets the value safely even
        /// when the document contains an invalid value.
        /// </summary>
        private void SetLineWidthCore(double lineWidth, bool isThrow)
        {
            double lineWidthSafe = MathUtil.FitToRange(lineWidth, 0, MaxLineWidth);

            if ((lineWidthSafe != lineWidth) && isThrow)
                    throw new ArgumentOutOfRangeException("lineWidth",
                        string.Format("Must be non-negative and less than or equal to {0}.", MaxLineWidth));

            BeforeChange();

            // When border is page border art, raw line width is in points already.
            // TODO The problem here is that LineWidth property depends on LineStyle.
            // If we need to set both then we have to follow the order: first set Style then Set width.
            // But then if width > 0 and Style is None then Style gets changes to Single. Nasty!
            RawLineWidth = (IsPageBorderArt)
                ? MathUtil.DoubleToInt(lineWidthSafe)
                : ConvertUtilCore.PointToEightsPoint(lineWidthSafe);
        }

        internal void SetDistanceFromTextSafe(double distanceFromText)
        {
            SetDistanceFromTextCore(distanceFromText, false);
        }

        private void SetDistanceFromTextCore(double distanceFromText, bool isThrow)
        {
            if (distanceFromText < 0)
            {
                if (isThrow)
                    throw new ArgumentOutOfRangeException("distanceFromText");
                else
                    distanceFromText = 0;
            }
            else if (distanceFromText > MaxDistanceFromText)
            {
                if (isThrow)
                    throw new ArgumentOutOfRangeException("distanceFromText");
                else
                    distanceFromText = MaxDistanceFromText;
            }

            BeforeChange();
            RawDistanceFromText = (int)distanceFromText;
        }

        /// <summary>
        /// Determines whether the specified border is equal in value to the current border.
        /// </summary>
        [JavaConvertCheckedExceptions]
        public bool Equals(Border rhs)
        {
            // Generated by ReSharper.
            if (ReferenceEquals(null, rhs))
                return false;
            if (ReferenceEquals(this, rhs))
                return true;

            return
                (LineStyle == rhs.LineStyle) &&
                (LineWidth == rhs.LineWidth) &&
                (ColorInternal.Equals(rhs.ColorInternal)) &&
                (DistanceFromText == rhs.DistanceFromText) &&
                (Frame == rhs.Frame) &&
                (Shadow == rhs.Shadow) &&
                (ThemeColorInternal == rhs.ThemeColorInternal) &&
                (ThemeShade == rhs.ThemeShade) &&
                (ThemeTint == rhs.ThemeTint) &&
                (IsNil == rhs.IsNil);
        }

        /// <summary>
        /// Determines whether the specified object is equal in value to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Generated by ReSharper.
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(Border))
                return false;
            return Equals((Border)obj);
        }

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = (int)mLineStyle;
                result = (result * 397) ^ mRawLineWidth;
                result = (result * 397) ^ (mColor != null ? mColor.GetHashCode() : 0);
                result = (result * 397) ^ mRawDistanceFromText;
                result = (result * 397) ^ mShadow.GetHashCode();
                result = (result * 397) ^ mFrame.GetHashCode();
                result = (result * 397) ^ (mThemeColor != null ? mThemeColor.GetHashCode() : 0);
                result = (result * 397) ^ (mThemeShade != null ? mThemeShade.GetHashCode() : 0);
                result = (result * 397) ^ (mThemeTint != null ? mThemeTint.GetHashCode() : 0);
                result = (result * 397) ^ (IsNil ? 1 : 0);
                return result;
            }
        }

        /// <summary>
        /// Returns true if this border can be prolonged (drawn without a break) with the specified border.
        ///</summary>
        ///<remarks>
        /// RK In MS Word a border of the same width and pattern, but different color will be drawn without
        /// a break, but I don't know how to do that. If the borders have different color, we draw them as
        /// separate lines.
        /// </remarks>
        internal bool CanContinue(Border rhs)
        {
            return (IsVisible && Equals(rhs));
        }

        /// <summary>
        /// Detects if this border has the reverse order of parts that the specified border has.
        ///
        /// Some double borders look like mirrored versions of other double borders, for example ThinThickSmallGap and
        /// ThickThinSmallGap. These borders can be connected so we must be able to determine the mirrored borders.
        /// </summary>
        internal bool IsMirroredWith(Border rhs)
        {
            return GetIsMirrored(LineStyle, rhs.LineStyle);
        }

        /// <summary>
        /// Returns True if second line style is mirrored with first one.
        /// </summary>
        /// <remarks>Moved from AE.</remarks>
        internal static bool GetIsMirrored(LineStyle lineStyle1, LineStyle lineStyle2)
        {
            LineStyle mirroredLineStyle = GetMirroredLineStyle(lineStyle1);
            if ((mirroredLineStyle != LineStyle.None) && (mirroredLineStyle == lineStyle2))
                return true;

            mirroredLineStyle = GetMirroredLineStyle(lineStyle2);
            if ((mirroredLineStyle != LineStyle.None) && (mirroredLineStyle == lineStyle1))
                return true;

            return false;
        }

        /// <summary>
        /// Some double borders look like mirrored versions of other double borders, for example ThinThickSmallGap and
        /// ThickThinSmallGap. These borders can be connected so we must be able to determine pairs of mirrored borders.
        /// This method maps line styles of such borders.
        /// </summary>
        private static LineStyle GetMirroredLineStyle(LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.ThinThickSmallGap: return LineStyle.ThickThinSmallGap;
                case LineStyle.ThinThickMediumGap: return LineStyle.ThickThinMediumGap;
                case LineStyle.ThinThickLargeGap: return LineStyle.ThickThinLargeGap;
                default: return LineStyle.None;
            }
        }

        /// <summary>
        /// Returns true if this border can be connected (so that the joining is drawn using a special way)
        /// with the specified border.
        /// </summary>
        /// <param name="rhs"></param>
        /// <param name="isMirrored">True if the connection is mirrored.</param>
        /// <returns></returns>
        internal bool CanConnect(Border rhs, out bool isMirrored)
        {
            // Invisible borders cannot connect with other borders. Single part borders does not require connections.
            // This is not noticeable for simple borders but ornate borders like waves look corrupted.
            if ((!IsVisible) || (PartsCount == 1))
            {
                isMirrored = false;
                return false;
            }

            isMirrored = IsMirroredWith(rhs);

            return
                (LineStyle == rhs.LineStyle || isMirrored) &&
                LineWidth == rhs.LineWidth &&
                Frame == rhs.Frame;
        }

        /// <summary>
        /// Gets the border that we are inheriting from.
        /// </summary>
        private Border Inherited
        {
            get { return (Border)mParent.FetchInheritedBorderAttr(mKey); }
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return IsInherited; }
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return Clone();
        }

        internal bool IsInherited
        {
            get { return (mParent != null); }
        }

        internal Border Clone()
        {
            // Not expected to create copies of inherited borders because inherited borders
            // are essentially presentation objects only and created on demand.
            if (IsInherited)
                throw new InvalidOperationException("Cannot clone an inherited attribute.");
            return (Border)MemberwiseClone();
        }

        internal void CopyFrom(Border src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            mKey = src.mKey;
            mLineStyle = src.LineStyle;
            mRawLineWidth = src.RawLineWidth;
            mColor = src.ColorInternal;
            mRawDistanceFromText = src.RawDistanceFromText;
            mShadow = src.Shadow;
            mFrame = src.Frame;
            mThemeColor = src.ThemeColorInternal;
            mThemeShade = src.ThemeShade;
            mThemeTint = src.ThemeTint;
            mIsNil = src.mIsNil;
            mParent = src.mParent;
        }

        /// <summary>
        /// Returns an array of values that represents length of on and off runs of a dashed line.
        /// 1 unit equals line width. If the line is not dashed, returns null.
        /// </summary>
        internal static float[] GetDashPattern(LineStyle lineStyle, float lineWidth)
        {
            float[] constDashLengths = gDashPatternMap[(int)lineStyle];

            if (constDashLengths == null)
                return null;

            // Clone so we don't modify the original.
            float[] dashLengths = new float[constDashLengths.Length];
            constDashLengths.CopyTo(dashLengths, 0);

            for (int i = 0; i < dashLengths.Length; i++)
                dashLengths[i] *= lineWidth;

            return dashLengths;
        }

        /// <summary>
        /// Gets the array of widths of individual lines and gaps between them.
        ///
        /// The result is scaled to the specified "line width".
        /// </summary>
        internal static float[] GetPartWidths(LineStyle lineStyle, float lineWidth)
        {
            float[] constPartWidths = gPartWidthMap[(int)lineStyle];

            // Single part border.
            if (constPartWidths == null)
                return new float[] {lineWidth};

            // Clone so we don't modify the original.
            float[] partWidths = new float[constPartWidths.Length];
            constPartWidths.CopyTo(partWidths, 0);

            for (int i = 0; i < partWidths.Length; i++)
            {
                if (partWidths[i] >= 0)
                    partWidths[i] *= lineWidth;
                else
                    partWidths[i] = System.Math.Abs(partWidths[i]);
            }

            return partWidths;
        }

        /// <summary>
        /// Returns the number of parts for the specified line style.
        /// </summary>
        /// <remarks>Moved from AE.</remarks>
        internal static int GetPartsCount(LineStyle lineStyle)
        {
            float[] partWidths = gPartWidthMap[(int)lineStyle];
            return (partWidths == null) ? 1 : partWidths.Length;
        }

        /// <summary>
        /// Returns actual width of the border for the specified "line width".
        ///
        /// This method correctly calculates actual width of multipart borders.
        /// For example, the "line width" could be 0.5pt, but the total
        /// width of say 4 parts will be 2.5pt.
        ///
        /// Returns zero if the line style is none.
        /// </summary>
        internal static float GetActualWidth(LineStyle lineStyle, float lineWidth)
        {
            switch (lineStyle)
            {
                case LineStyle.None:
                    return 0;

                case LineStyle.Single:
                case LineStyle.Dot:
                case LineStyle.DotDash:
                case LineStyle.DotDotDash:
                case LineStyle.DashLargeGap:
                case LineStyle.DashSmallGap:
                    return lineWidth;

                case LineStyle.Double:
                case LineStyle.Triple:
                case LineStyle.ThinThickSmallGap:
                case LineStyle.ThickThinSmallGap:
                case LineStyle.ThinThickThinSmallGap:
                case LineStyle.ThinThickMediumGap:
                case LineStyle.ThickThinMediumGap:
                case LineStyle.ThinThickThinMediumGap:
                case LineStyle.ThinThickLargeGap:
                case LineStyle.ThickThinLargeGap:
                case LineStyle.ThinThickThinLargeGap:
                case LineStyle.Emboss3D:
                case LineStyle.Engrave3D:
                {
                    float[] partWidths = GetPartWidths(lineStyle, lineWidth);

                    float totalWidth = 0;
                    foreach (float partWidth in partWidths)
                        totalWidth += partWidth;

                    return totalWidth;
                }
                case LineStyle.Thick:
                case LineStyle.Hairline:
                    return 0.75f;
                case LineStyle.Wave:
                    return GetWaveWidth(lineWidth);
                case LineStyle.DoubleWave:
                    return GetDoubleWaveWidth(lineWidth);
                case LineStyle.DashDotStroker:
                case LineStyle.Outset:
                    return lineWidth;

                default:
                    // This seems to suit other cases (page border art).
                    return lineWidth;
            }
        }

        private static float GetWaveWidth(float lineWidth)
        {
            return (lineWidth < 1) ? 3f : 3.75f;
        }

        private static float GetDoubleWaveWidth(float lineWidth)
        {
            return 6.75f * lineWidth;
        }

        /// <summary>
        /// Gets border number for resolving border conflicts
        /// according to Normative Variations 2.1.169 Part 4 Section 2.4.63, tcBorders (Table Cell Borders).
        /// </summary>
        internal int BorderNumber
        {
            get { return GetBorderNumber(LineStyle); }
        }

        /// <summary>
        /// Returns border number for resolving border conflicts according to ECMA $17.4.67.
        /// </summary>
        private static int GetBorderNumber(LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.Single: return 1;
                case LineStyle.Thick: return 2;
                case LineStyle.Double: return 3;
                case LineStyle.Dot: return 4;
                case LineStyle.DashLargeGap: return 5;
                case LineStyle.DotDash: return 8;
                case LineStyle.DotDotDash: return 9;
                case LineStyle.Triple: return 10;
                case LineStyle.ThinThickSmallGap: return 11;
                case LineStyle.ThickThinSmallGap: return 12;
                case LineStyle.ThinThickThinSmallGap: return 13;
                case LineStyle.ThinThickMediumGap: return 14;
                case LineStyle.ThickThinMediumGap: return 15;
                case LineStyle.ThinThickThinMediumGap: return 16;
                case LineStyle.ThinThickLargeGap: return 17;
                case LineStyle.ThickThinLargeGap: return 18;
                case LineStyle.ThinThickThinLargeGap: return 19;
                case LineStyle.Wave: return 20;
                case LineStyle.DoubleWave: return 21;
                case LineStyle.DashSmallGap: return 22;
                case LineStyle.DashDotStroker: return 23;
                case LineStyle.Emboss3D: return 24;
                case LineStyle.Engrave3D: return 25;
                case LineStyle.Outset: return 26;
                case LineStyle.Inset: return 27;
                // not in ECMA $17.4.67. Hairline made equivalent to Single,
                // conflicts are resolved by LineWidth.
                case LineStyle.None: return 0;
                case LineStyle.Hairline: return 1;
                default: return 0;
            }
        }

        /// <summary>
        /// Gets border weight for resolving border conflicts
        /// according to Normative Variations 2.1.169 Part 4 Section 2.4.63, tcBorders (Table Cell Borders).
        /// </summary>
        internal int Weight
        {
            get
            {
                // The borders with dotted and dashed styles shall be assigned the weight 1 regardless of the border width and number.
                return ((LineStyle != LineStyle.Dot) && (LineStyle != LineStyle.DashLargeGap))
                    ? BorderNumber * RawLineWidth
                    : 1;
            }
        }

        /// <summary>
        /// Creates instance of Nil border.
        /// </summary>
        /// <returns></returns>
        internal static Border CreateNilBorder()
        {
            Border border = new Border();
            border.mIsNil = true;
            return border;
        }

        /// <summary>
        /// Makes given border to be Nil border and returns it.
        /// </summary>
        internal static Border CreateNilBorder(Border border)
        {
            border.ClearFormatting();
            border.mIsNil = true;
            return border;
        }

        /// <summary>
        /// Indicates that the border defines explicitly no border.
        /// </summary>
        internal bool IsNil
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mIsNil; }
            set { mIsNil = value; }
        }

        /// <summary>
        /// Returns the winning border in case of border conflict, according to ECMA $17.4.67 with
        /// Normative Variations 2.1.169 Part 4 Section 2.4.63, tcBorders (Table Cell Borders).
        /// See http://msdn.microsoft.com/en-us/library/ff529927%28v=office.12%29.aspx.
        /// </summary>
        /// <remarks>
        /// If the borders are identical, first border wins. According to ECMA this must be "the first border in the reading order".
        /// So the order of parameters is significant, for identical style and color.
        /// </remarks>
        internal static Border GetWinningBorder(Border a, Border b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            if (a.Weight != b.Weight)
                return (a.Weight > b.Weight) ? a : b;

            int aBorderNumber = a.BorderNumber;
            int bBorderNumber = b.BorderNumber;
            if (aBorderNumber != bBorderNumber)
                return (aBorderNumber > bBorderNumber) ? a : b;

            // Styles are identical. Consider brightness. Lesser brightness wins.
            int aBrightness = a.Brightness0;
            int bBrightness = b.Brightness0;
            if (aBrightness != bBrightness)
                return (aBrightness < bBrightness) ? a : b;

            aBrightness = a.Brightness1;
            bBrightness = b.Brightness1;
            if (aBrightness != bBrightness)
                return (aBrightness < bBrightness) ? a : b;

            // For identical brightness, first border wins.
            return (a.Brightness2 <= b.Brightness2) ? a : b;
        }

        /// <summary>
        /// Brightness according to step 3 of ECMA $17.4.67.
        /// </summary>
        private int Brightness0
        {
            get { return Color.R + Color.B + 2 * Color.G; }
        }

        /// <summary>
        /// Brightness according to step 4 of ECMA $17.4.67.
        /// </summary>
        private int Brightness1
        {
            get { return Color.B + 2 * Color.G; }
        }

        /// <summary>
        /// Brightness according to step 5 of ECMA $17.4.67.
        /// </summary>
        private int Brightness2
        {
            get { return Color.G; }
        }

        static Border()
        {
            gDashPatternMap = new IntToObjDictionary<float[]>();
            gDashPatternMap.Add((int)LineStyle.Dot, new float[] {1f, 1f});
            gDashPatternMap.Add((int)LineStyle.DashSmallGap, new float[] {4f, 1f});
            gDashPatternMap.Add((int)LineStyle.DashLargeGap, new float[] {4f, 4f});
            gDashPatternMap.Add((int)LineStyle.DotDash, new float[] {7f, 3f, 3f, 3f});
            gDashPatternMap.Add((int)LineStyle.DotDotDash, new float[] {6f, 2f, 2f, 2f, 2f, 2f});

            gPartWidthMap = new IntToObjDictionary<float[]>();
            gPartWidthMap.Add((int)LineStyle.Double, new float[] {1f, 1f, 1f});
            gPartWidthMap.Add((int)LineStyle.Triple, new float[] {1f, 1f, 1f, 1f, 1f});
            gPartWidthMap.Add((int)LineStyle.ThinThickSmallGap, new float[] {1f, -0.75f, -0.75f});
            gPartWidthMap.Add((int)LineStyle.ThickThinSmallGap, new float[] {-0.75f, -0.75f, 1f});
            gPartWidthMap.Add((int)LineStyle.ThinThickMediumGap, new float[] {1f, 0.5f, 0.5f});
            gPartWidthMap.Add((int)LineStyle.ThickThinMediumGap, new float[] {0.5f, 0.5f, 1f});
            gPartWidthMap.Add((int)LineStyle.ThinThickLargeGap, new float[] {-1.5f, 1f, -0.75f});
            gPartWidthMap.Add((int)LineStyle.ThickThinLargeGap, new float[] {-0.75f, 1f, -1.5f});
            gPartWidthMap.Add((int)LineStyle.ThinThickThinSmallGap, new float[] {-0.75f, -0.75f, 1f, -0.75f, -0.75f});
            gPartWidthMap.Add((int)LineStyle.ThinThickThinMediumGap, new float[] {0.5f, 0.5f, 1f, 0.5f, 0.5f});
            gPartWidthMap.Add((int)LineStyle.ThinThickThinLargeGap, new float[] {-0.75f, 1f, -1.5f, 1f, -0.75f});
            // This is the trick used to draw 3D borders. Basically, these are the only borders that stand out for
            // the way of rendering. I have made them multipart and their parts have different colors (pens). Although
            // this is a rough approach, it works. Maybe I need to make it a bit sharper.
            gPartWidthMap.Add((int)LineStyle.Emboss3D, new float[] {0.25f, 0f, 1f, 0f, 0.25f});
            gPartWidthMap.Add((int)LineStyle.Engrave3D, new float[] {0.25f, 0f, 1f, 0f, 0.25f});
        }

        /// <summary>
        /// Checks if LineStyle is of a valid value.
        /// </summary>
        /// <returns></returns>
        internal bool HasValidLineStyle()
        {
            return (LineStyle >= LineStyle.None && LineStyle <= LineStyle.Inset);
        }

        /// <summary>
        /// Sets parent.
        /// </summary>
        internal void SetParent(IBorderAttrSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Copies inherited border values into this border and stops inheriting.
        /// </summary>
        private void BeforeChange()
        {
            NotifyChanging();

            if (IsInherited)
            {
                //Copy all attrs from the inherited border.
                CopyFrom(Inherited);
                mParent = null;
            }
        }

        /// <summary>
        /// Only set for an inherited border.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private IBorderAttrSource mParent;
        /// <summary>
        /// The key used to retrieve the parent border object we are inheriting from.
        /// Only used for an inherited border.
        /// </summary>
        private int mKey;
        private LineStyle mLineStyle;
        /// <summary>
        /// Gets/sets line width in "raw" units (as stored in DOC or in WordML).
        /// For normal borders the value is in 1/8th of a point.
        /// For page border art, the value is in points.
        ///
        /// It is quite ugly from the model point of view that I store line width and space
        /// in non-standard for the model (twips) measurements. Maybe can rework later,
        /// but watch out for the performance since there could be many borders in big documents with tables
        /// and there will be a lot of unneeded conversions taking place both on load and on save.
        /// </summary>
        private int mRawLineWidth;
        private DrColor mColor = DrColor.Empty;
        /// <summary>
        /// Spacing between border and the text in points?
        /// </summary>
        private int mRawDistanceFromText;
        private bool mShadow;
        private bool mFrame;

        private string mThemeColor;
        private string mThemeShade;
        private string mThemeTint;

        private bool mIsNil;

        /// <summary>
        /// Map of dashed line styles to on/off widths. 1 unit equals line width.
        /// </summary>
        private static readonly IntToObjDictionary<float[]> gDashPatternMap;
        /// <summary>
        /// Map of LineStyle to float[] that indicate widths of individual lines and gaps
        /// of a border that consists of several lines.
        /// Positive value indicates width that is scaled according to the line width.
        /// Negative value indicates a value that is not scaled, just use the absolute value.
        /// </summary>
        private static readonly IntToObjDictionary<float[]> gPartWidthMap;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxLineWidth = 31;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxDistanceFromText = 31;

        /// <summary>
        /// A valid border that is not visible. You can use this instead of using null borders.
        /// </summary>
        internal static readonly Border Empty = new Border();
    }
}
