// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2014 by Andrey Noskov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core
{
    internal class VmlOutline: IStroke
    {
        internal VmlOutline(Shape shape)
        {
            mShape = shape;
        }

        #region IStroke interface implementation.

        public bool On
        {
            // WORDSNET-8402 There're 2 similar attributes in VML. GeometryLineOK (strokeok in VML) attribute overrides all other
            // stroke attributes in the parent or Stroke element.
            // Actually, according to MS Word behavior, GeometryLineOK property enables or disables effect of Stroked property.
            get { return (bool)FetchAttr(ShapeAttr.LineOn) && (bool)FetchAttr(ShapeAttr.GeometryLineOK); }
            set
            {
                // Setting both attributes with the same value ensures the correct visual output.
                SetAttr(ShapeAttr.LineOn, value);
                SetAttr(ShapeAttr.GeometryLineOK, value);
            }
        }

        public double Weight
        {
            get { return ConvertUtilCore.EmuToPoint((int)FetchAttr(ShapeAttr.LineWidth)); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                SetAttr(ShapeAttr.LineWidth, ConvertUtilCore.PointToEmu(value));
            }
        }

        /// <summary>
        /// Gets or sets a color of a stroke.
        /// </summary>
        public DrColor ColorInternal
        {
            get { return (DrColor)FetchAttr(ShapeAttr.LineColor); }
            set { SetAttr(ShapeAttr.LineColor, value); }
        }

        /// <summary>
        /// Gets an unmodified base color of a stroke.
        /// </summary>
        public DrColor ColorInternalUnmodified
        {
            get { return ColorInternal; }
        }

        public DrColor Color2Internal
        {
            get { return (DrColor)FetchAttr(ShapeAttr.LineBackColor); }
            set { SetAttr(ShapeAttr.LineBackColor, value); }
        }

        public DashStyle DashStyle
        {
            get { return (DashStyle)FetchAttr(ShapeAttr.LineDashStyle); }
            set { SetAttr(ShapeAttr.LineDashStyle, value); }
        }

        public JoinStyle JoinStyle
        {
            get { return (JoinStyle)FetchAttr(ShapeAttr.LineJoinStyle); }
            set { SetAttr(ShapeAttr.LineJoinStyle, value); }
        }

        public EndCap EndCap
        {
            get { return (EndCap)FetchAttr(ShapeAttr.LineEndCapStyle); }
            set { SetAttr(ShapeAttr.LineEndCapStyle, value); }
        }

        public ShapeLineStyle LineStyle
        {
            get { return (ShapeLineStyle)FetchAttr(ShapeAttr.LineStyle); }
            set { SetAttr(ShapeAttr.LineStyle, value); }
        }

        public ArrowType StartArrowType
        {
            get { return (ArrowType)FetchAttr(ShapeAttr.LineStartArrow); }
            set { SetAttr(ShapeAttr.LineStartArrow, value); }
        }

        public ArrowType EndArrowType
        {
            get { return (ArrowType)FetchAttr(ShapeAttr.LineEndArrow); }
            set { SetAttr(ShapeAttr.LineEndArrow, value); }
        }

        public ArrowWidth StartArrowWidth
        {
            get { return (ArrowWidth)FetchAttr(ShapeAttr.LineStartArrowWidth); }
            set { SetAttr(ShapeAttr.LineStartArrowWidth, value); }
        }

        public ArrowLength StartArrowLength
        {
            get { return (ArrowLength)FetchAttr(ShapeAttr.LineStartArrowLength); }
            set { SetAttr(ShapeAttr.LineStartArrowLength, value); }
        }

        public ArrowWidth EndArrowWidth
        {
            get { return (ArrowWidth)FetchAttr(ShapeAttr.LineEndArrowWidth); }
            set { SetAttr(ShapeAttr.LineEndArrowWidth, value); }
        }

        public ArrowLength EndArrowLength
        {
            get { return (ArrowLength)FetchAttr(ShapeAttr.LineEndArrowLength); }
            set { SetAttr(ShapeAttr.LineEndArrowLength, value); }
        }

        public double Opacity
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.LineOpacity)); }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");
                SetAttr(ShapeAttr.LineOpacity, ConvertUtilCore.DoubleToFixed(value));
            }
        }

        public byte[] ImageBytes
        {
            get { return (byte[])FetchAttr(ShapeAttr.LineImageBytes); }
        }

        public LineFillType LineFillType
        {
            get { return (LineFillType)FetchAttr(ShapeAttr.LineFillType); }
            set { SetAttr(ShapeAttr.LineFillType, value); }
        }

        /// <summary>
        /// Gets or sets a <see cref="DmlFill"/> object of the stroke fill.
        /// </summary>
        DmlFill IStroke.StrokeFill
        {
            get { return null; }
            set { throw new InvalidOperationException("Object doesn't support this action."); }
        }

        #endregion

        private object FetchAttr(int key)
        {
            return mShape.FetchShapeAttrInternal(key);
        }

        private void SetAttr(int key, object value)
        {
            mShape.SetShapeAttrInternal(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Shape mShape;
    }
}
