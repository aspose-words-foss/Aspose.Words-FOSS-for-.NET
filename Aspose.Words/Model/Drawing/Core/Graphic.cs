// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/10/2014 by Andrey Noskov

using System.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Base class to define all common public properties for Vml and Dml shapes which have different implementation.
    /// </summary>
    internal abstract class Graphic
    {
        internal void SetCoordSizeSafe(Size value)
        {
            SetCoordSizeWidthSafe(value.Width);
            SetCoordSizeHeightSafe(value.Height);
        }

        internal abstract void SetCoordSizeWidthSafe(int width);
        internal abstract void SetCoordSizeHeightSafe(int height);

        internal abstract void SetWidthCore(double value, bool isThrow);
        internal abstract void SetHeightCore(double value, bool isThrow);

        /// <summary>
        /// Returns true if this shape is a horizontal rule.
        /// </summary>
        public abstract bool IsHorizontalRule { get; }

        /// <summary>
        /// Returns true if this shape is a WordArt object.
        /// </summary>
        public abstract bool IsWordArt { get; }

        public abstract ShapeType ShapeType { get; }

        /// <summary>
        /// Provides access to the OLE data of a shape. For a shape that is not an OLE object or ActiveX control, returns <c>null</c>.
        /// </summary>
        public abstract OleFormat OleFormat { get; }

        /// <summary>
        /// Switches the orientation of a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.FlipOrientation.None"/>.</p>
        /// </remarks>
        public abstract FlipOrientation FlipOrientation { get; set; }

        /// <summary>
        /// Defines the angle (in degrees) that a shape is rotated.
        /// Positive value corresponds to clockwise rotation angle.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public abstract double Rotation { get; set; }

        /// <summary>
        /// Provides access to adjustment raw values of a shape.
        /// For a shape that is not contains any adjustment raw values, returns empty collection.
        /// </summary>
        public abstract AdjustmentCollection Adjustments { get; }

        /// <summary>
        /// Defines the text of the text path (of a WordArt object).
        /// </summary>
        public abstract TextPath TextPath { get; }

        /// <summary>
        /// Returns true if an extrusion effect is enabled.
        /// </summary>
        public abstract bool ExtrusionEnabled { get; }

        /// <summary>
        /// Gets or sets the optional shape name.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Defines alternative text (description) to be displayed instead of a graphic.
        /// </summary>
        public abstract string AlternativeText { get; set; }

        /// <summary>
        /// Gets or sets the flag that specifies whether the shape is decorative in the document or not.
        /// </summary>
        public abstract bool Decorative { get; set; }

        /// <summary>
        /// Defines the title (caption) of the current shape object.
        /// </summary>
        public abstract string Title { get; set; }

        /// <summary>
        /// Gets or sets the flag the specifies whether the shape is visible in the document or not.
        /// </summary>
        internal abstract bool Hidden { get; set; }

        /// <summary>
        /// Returns true if a shadow effect is enabled.
        /// </summary>
        public abstract bool ShadowEnabled { get; }

        internal abstract int CoordSizeWidth { get; }

        internal abstract int CoordSizeHeight { get; }

        internal abstract int CoordOriginX { get; set; }

        internal abstract int CoordOriginY { get; set; }

        internal abstract bool AspectRatioLocked { get; set; }

        /// <summary>
        /// Provides access to the properties of the horizontal rule shape.
        /// </summary>
        internal abstract HorizontalRule HorizontalRule { get; }

        // ImageData common attributes.
        public abstract double Brightness { get; set; }
        public abstract double Contrast { get; set; }
        public abstract bool BiLevel { get; set; }
        public abstract bool GrayScale { get; set; }

        public abstract double CropTop { get; set; }
        public abstract double CropBottom { get; set; }
        public abstract double CropLeft { get; set; }
        public abstract double CropRight { get; set; }

        // TextBox common attributes.
        public abstract double InternalMarginLeft { get; set; }
        public abstract double InternalMarginRight { get; set; }
        public abstract double InternalMarginTop { get; set; }
        public abstract double InternalMarginBottom { get; set; }

        public abstract bool FitShapeToText { get; set; }
        public abstract LayoutFlow LayoutFlow { get; set; }
        public abstract TextBoxWrapMode TextBoxWrapMode { get; set; }
        internal abstract TextBoxAnchor TextBoxAnchor { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating either text of the TextBox should not rotate when the shape is rotated.
        /// </summary>
        internal abstract bool TextBoxNoTextRotation { get; set; }

        internal virtual void SetHRef(string href)
        {
            if (StringUtil.HasChars(href))
                mShape.SetShapeAttrInternal(ShapeAttr.HyperlinkAddress, href);
            else
                mShape.RemoveShapeAttrInternal(ShapeAttr.HyperlinkAddress);
        }

        /// <summary>
        /// Gets or sets <see cref="Aspose.Words.Drawing.ShadowType"/> object for a shape.
        /// </summary>
        internal abstract ShadowType ShadowType { get; set; }

        /// <summary>
        /// Removes shadow effect from a shape.
        /// </summary>
        internal abstract void RemoveShadow();

        /// <summary>
        /// Gets or sets color of a shadow effect.
        /// </summary>
        internal abstract Color ShadowColor { get; set; }

        /// <summary>
        /// Gets or sets transparency of a shadow effect.
        /// </summary>
        internal abstract double ShadowTransparency { get; set; }


        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        protected ShapeBase mShape;
    }
}
