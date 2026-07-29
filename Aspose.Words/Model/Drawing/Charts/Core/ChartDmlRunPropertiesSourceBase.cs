// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2022 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Drawing;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.Defaults;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents a base for <see cref="IRunAttrSource"/> implementations to provide <see cref="Font"/> property in
    /// chart objects.
    /// </summary>
    internal abstract class ChartDmlRunPropertiesSourceBase : IRunAttrSource
    {
        /// <summary>
        /// Ctor to create an instance of this class.
        /// </summary>
        internal ChartDmlRunPropertiesSourceBase(DmlChartSpace chartSpace)
        {
            mChartSpace = chartSpace;
        }

        /// <summary>
        /// Gets the property value specified directly on the source or <b>null</b> by the key.
        /// </summary>
        public object GetDirectRunAttr(int key)
        {
            return GetDirectRunAttr(key, RevisionsView.Original);
        }

        /// <summary>
        /// Gets the property value specified directly on the source or <b>null</b> by the key.
        /// </summary>
        public object GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            if ((GeneralRunProperties != null) && RunPrConverter.IsPropertySpecified(GeneralRunProperties, key))
                return GetPropertyValue(key, GeneralRunProperties);

            // A Font object doesn't try to get inherited Fill attribute: do it here.
            if (key == FontAttr.EffectFill)
                return FetchInheritedRunPropertyValue(key);

            return null;
        }

        /// <summary>
        /// Gets the property value from one of the parents or from defaults.
        /// </summary>
        public object FetchInheritedRunAttr(int key)
        {
            object value = FetchInheritedRunPropertyValue(key);
            if (value != null)
                return value;

            return FetchDefaultRunPropertyValue(key);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        public void SetRunAttr(int key, object value)
        {
            if (NeedThrowExceptionWhenSettingRunAttr(key))
                throw new InvalidOperationException("The property is not supported on a chart object.");

            EnsureGeneralRunPropertiesExist(true);

            // Set the property for all runs/fields and paragraphs of the text.
            foreach (DmlRunProperties runProperties in GetRunPropertiesEnumerable())
                RunPrConverter.SetProperty(runProperties, key, value, Theme);

            // Set the flag indicating that run properties of the paragraphs are not empty.
            foreach (DmlParagraph paragraph in GetParagraphEnumerable())
                paragraph.Properties.HasDefaultRunProperties = true;
        }

        /// <summary>
        /// Removes the direct property value if it exists.
        /// </summary>
        public void RemoveRunAttr(int key)
        {
            // Clear the property in the all runs/fields and paragraphs of the text.
            foreach (DmlRunProperties runProperties in GetRunPropertiesEnumerable())
                RunPrConverter.ClearProperty(runProperties, key);
        }

        /// <summary>
        /// Resets properties of a chart object so that properties of the parent object will be used.
        /// </summary>
        public void ClearRunAttrs()
        {
            ClearAllRunProperties();
        }

        /// <summary>
        /// Clears all properties of all instances of <see cref="DmlRunProperties"/> related to the current chart object.
        /// </summary>
        protected virtual void ClearAllRunProperties()
        {
            foreach (DmlRunProperties runProperties in GetRunPropertiesEnumerable())
                runProperties.RemoveAll();
        }

        /// <summary>
        /// Fetches the specified property from the parent paragraph, collection or chart space. Run defaults are not
        /// returned.
        /// </summary>
        protected abstract object FetchInheritedRunPropertyValue(int key);

        /// <summary>
        /// Returns the default property value of a chart run.
        /// </summary>
        protected virtual object FetchDefaultRunPropertyValue(int key)
        {
            // See DmlChartFontDefaults.GetDefaultRunPr for more information.
            switch (key)
            {
                case FontAttr.NameAscii:
                {
                    string fontName = (Theme != null)
                        ? ((IThemeProvider)Theme).GetFontName(ThemeFontCore.MinorAscii)
                        : null;

                    return ComplexFontName.FromName(
                        (fontName != null)
                            ? fontName
                            : DmlChartFontDefaults.DefaultFontName);
                }
                case FontAttr.Size:
                    return ConvertUtilCore.PointToHalfPoint(
                        IsChartEx ? 9 : DmlChartFontDefaults.DefaultFontSizeInPoints);
                case FontAttr.Kerning:
                    return IsChartEx ? 0 : 24; // half points
                case FontAttr.Color:
                    return GetDefaultTextColor();
                default:
                    return RunPr.FetchDefaultAttr(key);
            }
        }

        /// <summary>
        /// Gets an enumerable over all instances of <see cref="DmlRunProperties"/> related to the current chart object.
        /// </summary>
        protected abstract IEnumerable<DmlRunProperties> GetRunPropertiesEnumerable();

        /// <summary>
        /// Gets an enumerable over all instances of <see cref="DmlParagraph"/> related to the current chart object.
        /// </summary>
        protected abstract IEnumerable<DmlParagraph> GetParagraphEnumerable();

        /// <summary>
        /// Generates <see cref="GeneralRunProperties"/> if it is <b>null</b>.
        /// </summary>
        protected abstract void EnsureGeneralRunPropertiesExist(bool toSetProperty);

        /// <summary>
        /// Gets a value from the specified parent <see cref="DmlRunProperties"/> and, if necessary, clones it and puts
        /// to <see cref="GeneralRunProperties"/>.
        /// </summary>
        /// <remarks>
        /// If The property value is an object instance, state of which can be modified through API, we need to make
        /// a clone and place to the object's run properties so that the parent will not be affected.
        /// </remarks>
        protected object GetValueFromParentRunProperties(int key, DmlRunProperties parentRunProperties)
        {
            object value = GetPropertyValue(key, parentRunProperties);
            if ((value != null) && NeedCloneInheritedRunPropertyValue(key))
            {
                // The 'value' is a mutable object instance: we need to make a clone and put to GeneralRunProperties.
                value = CloneRunPropertyValue(value);
                EnsureGeneralRunPropertiesExist(false);
                RunPrConverter.SetProperty(GeneralRunProperties, key, value, Theme);
            }

            return value;
        }

        /// <summary>
        /// Gets a property value specified by the font attribute key.
        /// </summary>
        protected object GetPropertyValue(int key, DmlRunProperties runProperties)
        {
            Theme theme = Theme;
            // Let's resolve theme font names like "+mn-cs".
            if ((theme == null) && IsFontNameKey(key))
                theme = Theme.BuiltInTheme;

            return RunPrConverter.GetProperty(runProperties, key, theme, true);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified font attribute key is a font name key.
        /// </summary>
        private static bool IsFontNameKey(int key)
        {
            switch (key)
            {
                case FontAttr.NameAscii:
                case FontAttr.NameBi:
                case FontAttr.NameFarEast:
                case FontAttr.NameOther:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the specified run attribute is not supported, and if an exception should
        /// be generated when trying to set it.
        /// </summary>
        private static bool NeedThrowExceptionWhenSettingRunAttr(int key)
        {
            switch (key)
            {
                // These Font properties are currently not supported by chart objects. Font.Border.get and
                // Font.Shading.get have a behavior that if the property value is null, a new instance is
                // created and set to IRunAttrSource: do not generate the exception at this case.
                case FontAttr.Border:
                case FontAttr.Shading:
                    return false;
                default:
                    return !RunPrConverter.IsPropertySupported(key);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the specified run property value need to be cloned when inherited value
        /// is used. This method should return <b>true</b> for all non-immutable objects.
        /// </summary>
        private static bool NeedCloneInheritedRunPropertyValue(int key)
        {
            switch (key)
            {
                case FontAttr.EffectFill:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Clones the specified run property value if required.
        /// </summary>
        private static object CloneRunPropertyValue(object value)
        {
            if (value is DmlFill)
                return ((DmlFill)value).Clone();

            return value;
        }

        /// <summary>
        /// Gets a color that is used by default to display the text.
        /// </summary>
        private DrColor GetDefaultTextColor()
        {
            if (!IsChartEx)
                return DrColor.Empty;

            HSLColor hslColor = new HSLColor(DrColor.FromNativeColor(Theme.Colors.Dark1));
            // It seems MS Word calculates the color in this way.
            hslColor.Lum = 1 - (1 - hslColor.Lum) * 0.65;
            return hslColor.ToDrColor();
        }

        /// <summary>
        /// Gets text properties of the parent chart space.
        /// </summary>
        protected DmlChartTxPr ChartSpaceTxPr
        {
            get { return mChartSpace.TxPr; }
        }

        /// <summary>
        /// Gets a theme of the document.
        /// </summary>
        protected Theme Theme
        {
            get { return mChartSpace.Dml.Document.GetThemeInternal(); }
        }

        /// <summary>
        /// Gets a flag indicating whether the current chart is a Word 2016 chart.
        /// </summary>
        protected bool IsChartEx
        {
            get { return mChartSpace.IsChartEx; }
        }

        /// <summary>
        /// Gets DML run properties that is used to get direct property value.
        /// </summary>
        internal abstract DmlRunProperties GeneralRunProperties { get; }

        private readonly DmlChartSpace mChartSpace;
    }
}
