// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2011 by Alexey Titov

using System;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// Base class for DrawingML fills.
    /// </summary>
    internal abstract class DmlFill : IDmlColorable, IFill, IComplexAttr
    {
        /// <summary>
        /// Creates <see cref="DmlFill"/> of a specified type.
        /// </summary>
        internal static DmlFill Create(DmlFillType type)
        {
            switch (type)
            {
                case DmlFillType.BlipFill:
                    return new DmlBlipFill();
                case DmlFillType.GradientFill:
                    return new DmlGradientFill();
                case DmlFillType.GroupFill:
                    return new DmlGroupFill();
                case DmlFillType.NoFill:
                    return new DmlNoFill();
                case DmlFillType.PatternFill:
                    return new DmlPatternFill();
                case DmlFillType.SolidFill:
                    return new DmlSolidFill();
                case DmlFillType.StyleFill:
                    return new DmlStyleFill();
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlFill value = (DmlFill)obj;

            return (value.DmlFillType == DmlFillType);
        }

        public override int GetHashCode()
        {
            int hash = DmlFillType.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Set style color to any color placeholders used in the fill.
        /// </summary>
        public virtual void ApplyStyleColor(DmlColor styleColor)
        {
            // By default does nothing.
        }

        internal abstract DmlFill Clone();

        /// <summary>
        /// Resolves theme colors to a concrete RGB colors using a specified theme provider.
        /// </summary>
        internal virtual void ResolveThemeColors(IThemeProvider themeProvider)
        {
            // Resolve ForeColor.
            DmlSchemeColor schemeColor = DmlColorInternal as DmlSchemeColor;
            if (schemeColor != null)
                DmlColorInternal = schemeColor.Resolve(themeProvider);

            // Resolve BackColor.
            schemeColor = DmlColor2Internal as DmlSchemeColor;
            if (schemeColor != null)
                DmlColor2Internal = schemeColor.Resolve(themeProvider);
        }

        /// <summary>
        /// Gets type of the Dml Fill.
        /// </summary>
        internal abstract DmlFillType DmlFillType { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get; }

        #region IFill interface implementation.

        public void SetImageBytes(byte[] imageBytes)
        {
            if (DmlFillType == DmlFillType.BlipFill)
                ((DmlBlipFill)this).Blip.EmbedImage = imageBytes;
        }

        /// <summary>
        /// Gets or sets a color.
        /// </summary>
        public DrColor ColorInternal
        {
            get { return (DmlColorInternal != null) ? DmlColorInternal.CreateDrColor(ThemeProvider, null) : DrColor.Empty; }
            set { DmlColorInternal = DmlHexRgbColor.FromDrColor(value); }
        }


        /// <summary>
        /// Gets a base color without modifiers.
        /// </summary>
        public DrColor ColorInternalUnmodified
        {
            get { return (DmlColorInternal != null) ? DmlColorInternal.CreateUnmodifiedDrColor(ThemeProvider) : DrColor.Empty; }
        }

        public DrColor Color2Internal
        {
            get { return (DmlColor2Internal != null) ? DmlColor2Internal.CreateDrColor(ThemeProvider, null) : DrColor.Empty; }
            set { DmlColor2Internal = DmlHexRgbColor.FromDrColor(value); }
        }

        /// <summary>Gets the array of custom gradient colors.</summary>
        /// <remarks>
        /// <p>The default value is null.</p>
        /// </remarks>
        public virtual GradientColor[] GradientColors
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets fill color opacity.
        /// </summary>
        public virtual double Opacity
        {
            get
            {
                if (DmlColorInternal != null)
                {
                    DmlAlpha alpha = DmlColorInternal.Alpha;
                    if (alpha != null)
                        return alpha.Value;
                }

                return 1.0;
            }
            set
            {
                if (DmlColorInternal != null)
                    DmlColorInternal.UpdateAlpha(value);
            }
        }

        /// <summary>
        /// Gets or sets fill opacity of the second fill color.
        /// </summary>
        public virtual double Opacity2
        {
            get
            {
                if (DmlColor2Internal != null)
                {
                    DmlAlpha alpha = DmlColor2Internal.Alpha;
                    if (alpha != null)
                        return alpha.Value;
                }

                return 1.0;
            }
            set
            {
                if (DmlColor2Internal != null)
                    DmlColor2Internal.UpdateAlpha(value);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a parent object is filled.
        /// </summary>
        public virtual bool On
        {
            get
            {
                return true;
            }
            set
            {
                // Check if nothing to do.
                if (value)
                    return;

                // IN. Actually, Word also saves an existing fill to the 'a14:hiddenFill' extension.
                Parent.SetFill(new DmlNoFill());
            }
        }

        /// <summary>
        /// <p>Determines whether the fill rotates with the specified shape.</p>
        /// <p>The default value is <c>false</c>.</p>
        /// </summary>
        /// <dev>
        /// IN. Word VBA allows to get this property even for fills that cannot have this property by returning <c>false</c>.
        /// But setting this property for such fills throws an exception.
        /// </dev>
        public virtual bool RotateWithShape
        {
            get { return false; }
            set { throw new InvalidOperationException(MsgValueOutOfRange); }
        }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is null.</p>
        /// </remarks>
        [JavaThrows(true)]
        public virtual byte[] ImageBytes
        {
            get { return null; }
        }

        public virtual double Angle
        {
            get { return 0; }
            set
            {
                // Ignore.
            }
        }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientstyle
        /// </summary>
        public virtual GradientStyle GradientStyle
        {
            get { return GradientStyle.None; }
        }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills or 0 if not defined.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientvariant
        /// </summary>
        public virtual GradientVariant GradientVariant
        {
            get { return GradientVariant.None; }
        }

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        public IFillable Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        public bool LockAspectRatio
        {
            get
            {
                // Return false for Dml shape fill. Cannot find this attribute for Dml blip fill.
                return false;
            }
            set
            {
                // Do nothing for Dml shape fill.
            }
        }

        public double FocusLeft
        {
            get
            {
                // Return zero for Dml shape fill. Cannot find suitable attribute for Dml fill.
                return 0;
            }
            set
            {
                // Do nothing for Dml shape fill.
            }
        }

        public double FocusTop
        {
            get
            {
                // Return zero for Dml shape fill. Cannot find suitable attribute for Dml fill.
                return 0;
            }
            set
            {
                // Do nothing for Dml shape fill.
            }
        }

        public int Focus
        {
            get
            {
                // Return zero for Dml shape fill. Cannot find suitable attribute for Dml fill.
                return 0;
            }
            set
            {
                // Do nothing for Dml shape fill.
            }
        }

        /// <summary>
        /// Gets or sets the type of fill.
        /// </summary>
        public virtual FillTypeCore FillType
        {
            get { return FillTypeCore.Solid; }
            set
            {
                // Do nothing for DML shape fill.
                Debug.Assert(false, "FillType сan not be changed at DML fill.");
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets foreground color of the fill.
        /// </summary>
        internal virtual DmlColor DmlColorInternal
        {
            get { return null; }
            set { Debug.Assert(value != null); }
        }

        /// <summary>
        /// Gets or sets background color of the fill.
        /// </summary>
        internal virtual DmlColor DmlColor2Internal
        {
            get { return null; }
            set { Debug.Assert(value != null); }
        }

        #region IComplexAttr implementation

        /// <summary>
        /// Returns true if the attribute inherits the value from some parent.
        /// </summary>
        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        /// <summary>
        /// Called to create a deep clone of the attribute. Will be called only for non inherited attribute values.
        /// </summary>
        public IComplexAttr DeepCloneComplexAttr()
        {
            return Clone();
        }

        #endregion

        /// <summary>
        /// Gets IThemeProvider object.
        /// </summary>
        internal IThemeProvider ThemeProvider
        {
            get { return mThemeProvider == null ? ((mParent != null) ? mParent.FillableThemeProvider : null) : mThemeProvider; }
            set { mThemeProvider = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private IFillable mParent;
        private IThemeProvider mThemeProvider;
        // These constant messages for exceptions are borrowed from VBA.
        internal const string MsgInvalidAction = "Object doesn't support this action.";
        internal const string MsgValueOutOfRange = "The specified value is out of range.";
    }
}
