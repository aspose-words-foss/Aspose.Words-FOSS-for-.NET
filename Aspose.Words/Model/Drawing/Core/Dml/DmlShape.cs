// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Represents a custom shape.
    /// </summary>
    /// <remarks>
    /// 20.1.2.2.33 sp (Shape)
    /// This element specifies the existence of a single shape. A shape can either be a preset 
    /// or a custom geometry, defined using the DrawingML framework. In addition to a geometry 
    /// each shape can have both visual and non-visual properties attached. Text and corresponding 
    /// styling information can also be attached to a shape. This shape is specified along with all 
    /// other shapes within either the shape tree or group shape elements.
    /// </remarks>
    internal class DmlShape : DmlShapeBase
    {
        internal DmlShape(DmlNodeType shapeType)
        {
            if (shapeType != DmlNodeType.Shape &&
                shapeType != DmlNodeType.WordprocessingShape &&
                shapeType != DmlNodeType.ConnectorShape)
                throw new ArgumentException("DmlShape cannot be created with the specified DmlNodeType");

            mShapeType = shapeType;
        }

        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            DmlShape lhs = (DmlShape)base.Clone(isCloneChildren, cloningListener);

            if (mTextShape != null)
                lhs.mTextShape = mTextShape.Clone();

            return lhs;
        }

        internal override DmlNodeType DmlNodeType
        {
            get { return mShapeType; }
        }

        internal override bool ShouldBuildUsingSqueezedSize()
        {
            // Originally this returned true only for 'trapezoid' preset geometry (see TestJira8595).
            // In WORDSNET-13127  the same problem occurs with 'homePlate'.
            // Further testing show that size must be recalculated almost for all geometries (see TestDmlShapeSqueezedSize).
            // So return true for all geometries here, except custom drawings (preset geometry is not set).
            // WORDSNET-15182 Also return true if shape is textbox, even if it has custom geometry.
            return StringUtil.HasChars(Geometry.PresetName) || Dml.HasTextbox;
        }

        internal DmlTextShape TextShape
        {
            get { return mTextShape; }
            set { mTextShape = value; }
        }

        /// <summary>
        /// Shortcut to body properties of <see cref="TextShape"/>.
        /// If there is no <see cref="TextShape"/> returns new instance of <see cref="DmlTextBodyProperties"/>.
        /// </summary>
        internal DmlTextBodyProperties TextBodyPr
        {
            get { return (TextShape != null) ? TextShape.TextBody.Properties : new DmlTextBodyProperties(); }
        }


        internal override bool IsUpdateDimensionsFromRelative
        {
            get
            {
                // Always update dimensions from relative if shape does not have textbox.
                if (!Dml.HasTextbox)
                    return true;

                double angle = Transform.Rotation.ValueInDegrees;
                angle = MathUtil.NormalizeAngle(angle);

                // WORDSNET-12642 MS Word seems do not use updated size when textbox shape is 45, 90 or 270 degrees rotated.
                // Have no idea why these angles only, but do the same.
                bool isExceptionAngle = (MathUtil.AreEqual(angle, 45.0d) ||
                                         MathUtil.AreEqual(angle, 90.0d) ||
                                         MathUtil.AreEqual(angle, 270.0d));

                // When text is upright (i.e. id not rotated with shape) size is always updated from percent.
                return !isExceptionAngle || TextBodyPr.IsTextUpright;
            }
        }

        /// <summary>
        /// Returns true if shape must grow horizontally to fit the text box content.
        /// </summary>
        internal bool AutoWidth
        {
            get { return (TextBodyPr.TextWrappingType == TextBoxWrapMode.None); }
        }

        /// <summary>
        /// Returns true if shape must grow vertically to fit the text box content.
        /// </summary>
        internal bool AutoHeight
        {
            // todo alexnosk: Consider adding Type property to AutoFitMode to avoid using 'is' operator where possible.
            get { return (TextBodyPr.AutoFitMode is DmlShapeAutoFitMode); }
        }

        private DmlTextShape mTextShape;
        private readonly DmlNodeType mShapeType;
    }
}
