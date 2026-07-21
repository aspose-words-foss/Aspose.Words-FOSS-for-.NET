// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// Represents a base class for colors.
    /// </summary>
    internal abstract class DmlColor
    {
        /// <summary>
        /// Creates DmlColor from ARGB.
        /// </summary>
        /// <remarks>
        /// Use this overload to avoid scaling "opacity" using "A" part of the "DrColor"
        /// because "opacity" uses 2 bytes to hold value unlike the alpha part of DrColor uses only one byte.
        /// </remarks>
        /// <param name="alpha">Opacity, valid range is from 0 to 1.</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        public static DmlColor CreateFromArgb(double alpha, int r, int g, int b)
        {
            DmlHexRgbColor color = new DmlHexRgbColor();

            // Exclude drColor.A, it will be added as color modifier.
            DrColor drColorWithoutA = new DrColor(0, r, g, b);
            color.Value = FormatterPal.IntToStrX6(drColorWithoutA.ToArgb());

            // Add modifier for alpha.
            if (!MathUtil.AreEqual(alpha, 1))
                AddAlphaModifier(color, alpha);

            return color;
        }

        /// <summary>
        /// Creates DmlColor from DrColor.
        /// </summary>
        public static DmlColor CreateFromDrColor(DrColor drColor)
        {
            return CreateFromArgb((double)drColor.A/255, drColor.R, drColor.G, drColor.B);
        }

        /// <summary>
        /// Appends "Alpha" modifier to color.
        /// </summary>
        /// <param name="color">Color to add modifier.</param>
        /// <param name="value">Modifier value.</param>
        internal static void AddAlphaModifier(DmlColor color, double value)
        {
            DmlAlpha alpha = new DmlAlpha();
            alpha.Value = value;
            color.ColorModifiers.Add(alpha);
        }

        public DrColor CreateDrColor(IThemeProvider themeProvider, IDmlColorModifier additionalModifier)
        {
            DrColor color = CreateUnmodifiedDrColor(themeProvider);
            color = ApplyColorModifiersTo(color, additionalModifier);
            return color;
        }

        public abstract DmlColor Clone();

        public virtual void ApplyStyleColor(DmlColor styleColor)
        {
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlColor value = (DmlColor)obj;

            return (ColorType == value.ColorType) && ListUtil.CheckAreEquals(value.ColorModifiers, ColorModifiers);
        }

        public override int GetHashCode()
        {
            int hash = ColorType.GetHashCode();
            hash ^= GetModifiersHashCode();
            return hash;
        }

        /// <summary>
        /// Gets the base color without modifiers.
        /// </summary>
        internal DrColor CreateUnmodifiedDrColor(IThemeProvider themeProvider)
        {
            return CreateUnmodifiedColor(themeProvider);
        }

        /// <summary>
        /// Gets or sets an alpha modifier.
        /// </summary>
        internal DmlAlpha Alpha
        {
            get
            {
                int index = IndexOf(DmlColorModifierType.Alpha);
                if (index == -1)
                    return null;

                return (DmlAlpha)ColorModifiers[index];
            }
            set
            {
                int index = IndexOf(DmlColorModifierType.Alpha);
                if (index != -1)
                {
                    if (value != null)
                        ColorModifiers[index] = value;
                    else
                        ColorModifiers.RemoveAt(index);
                }
                else
                {
                    if (value != null)
                        ColorModifiers.Add(value);
                }
            }
        }

        /// <summary>
        /// Updates an alpha modifier with a specified value.
        /// </summary>
        /// <remarks>If there is no alpha modifier, then creates a new one.</remarks>
        internal DmlColor UpdateAlpha(double value)
        {
            DmlAlpha alpha = Alpha;
            if (alpha == null)
            {
                alpha = new DmlAlpha();
                ColorModifiers.Add(alpha);
            }
            alpha.Value = value;

            return this;
        }

        /// <summary>
        /// Returns color modifier of the specified type or <b>null</b> if not found.
        /// </summary>
        internal IDmlColorModifier GetColorModifier(DmlColorModifierType type)
        {
            int index = IndexOf(type);
            return (index >= 0) ? ColorModifiers[index] : null;
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        protected int GetModifiersHashCode()
        {
            int hash = 0;
            foreach (IDmlColorModifier modifier in ColorModifiers)
                hash ^= modifier.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Returns color modifier index of the specified type or -1 if not found.
        /// </summary>
        private int IndexOf(DmlColorModifierType type)
        {
            for (int i = 0; i < ColorModifiers.Count; i++)
            {
                if (ColorModifiers[i].ModifierType == type)
                    return i;
            }

            return -1;
        }

        private DrColor ApplyColorModifiersTo(DrColor color, IDmlColorModifier additionalModifier)
        {
            foreach (IDmlColorModifier colorModificator in ColorModifiers)
                color = colorModificator.Modify(color);

            if (additionalModifier != null)
                color = additionalModifier.Modify(color);

            return color;
        }

        protected abstract DrColor CreateUnmodifiedColor(IThemeProvider themeProvider);

        protected void CopyColorModifiersTo(DmlColor color)
        {
            List<IDmlColorModifier> newModifiers = new List<IDmlColorModifier>(ColorModifiers.Count);
            foreach (IDmlColorModifier colorModifier in ColorModifiers)
                newModifiers.Add(colorModifier.Clone());

            color.ColorModifiers = newModifiers;
        }

        static DmlColor()
        {
            Empty = DmlHexRgbColor.FromDrColor(DrColor.Empty);
        }

        public List<IDmlColorModifier> ColorModifiers
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mColorModifiers == null)
                    mColorModifiers = new List<IDmlColorModifier>();
                return mColorModifiers;
            }
            set { mColorModifiers = value; }
        }

        /// <summary>
        /// Returns true if this color is empty.
        /// </summary>
        internal virtual bool IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// Gets Empty color. This is full transparency black color of <see cref="DmlHexRgbColor"/> type.
        ///</summary>
        internal static readonly DmlColor Empty;

        public abstract DmlColorType ColorType { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get; }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private List<IDmlColorModifier> mColorModifiers;
    }
}
