// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2016 by Alexander Zhiltsov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Transforms;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// This class is used to store default shape properties of a theme.
    /// </summary>
    /// <dev>
    /// Corresponding ISO/IEC 29500 element is 20.1.2.2.35 spPr (Shape Properties). 
    /// Implementing the following interfaces is necessary for reader/writer.
    /// </dev>
    internal class DefaultShapeProperties : IDmlShapePrSource, IDmlCommonShapePrSource
    {
        /// <summary>
        /// Clones this <see cref="DefaultShapeProperties"/> object.
        /// </summary>
        internal DefaultShapeProperties Clone()
        {
            DefaultShapeProperties lhs = (DefaultShapeProperties)MemberwiseClone();

            if (mTransform != null)
                lhs.mTransform = mTransform.Clone();
            if (mGeometry != null)
                lhs.mGeometry = mGeometry.Clone();
            if (mFill != null)
                lhs.mFill = mFill.Clone();
            if (mOutline != null)
                lhs.mOutline = mOutline.Clone();
            if (mStyle != null)
                lhs.mStyle = mStyle.Clone();
            if (mEffects != null)
                lhs.mEffects = mEffects.Clone();
            if (mScene3DProperties != null)
                lhs.mScene3DProperties = mScene3DProperties.Clone();
            if (mShape3DProperties != null)
                lhs.mShape3DProperties = mShape3DProperties.Clone();
            if (mSpPrExtensions != null)
                lhs.mSpPrExtensions = DmlExtensionListSource.CloneExtensions(mSpPrExtensions);

            return lhs;
        }

        /// <summary>
        /// Gets or sets a value that represents 2-D transforms for ordinary shapes.
        /// </summary>
        /// <dev>
        /// 20.1.7.6 xfrm (2D Transform for Individual Objects)
        /// </dev>
        public DmlTransform Transform
        {
            get { return mTransform; }
            set { mTransform = value; }
        }

        /// <summary>
        /// Gets or sets a value that allows rendering picture using only black and white coloring.
        /// </summary>
        /// <dev>
        /// 20.1.2.2.35 spPr (Shape Properties), attribute bwMode (Black and White Mode)
        /// </dev>
        public BWMode BWMode
        {
            get { return mBwMode; }
            set { mBwMode = value; }
        }

        /// <summary>
        /// Gets or sets a value that allows defining custom geometric properties.
        /// </summary>
        /// <dev>
        /// 20.1.9.8 custGeom (Custom Geometry)
        /// </dev>
        public DmlGeometry Geometry
        {
            get { return mGeometry; }
            set { mGeometry = value; }
        }

        /// <summary>
        /// Gets or sets a value that allows specifying a color fill.
        /// </summary>
        /// <dev>
        /// Elements of the EG_FillProperties choice group.
        /// </dev>
        public DmlFill Fill
        {
            get { return mFill; }
            set { mFill = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies an outline style.
        /// </summary>
        /// <dev>
        /// 20.1.2.2.24 ln (Outline)
        /// </dev>
        public DmlOutline Outline
        {
            get { return mOutline; }
            set { mOutline = value; }
        }

        /// <summary>
        /// Gets or sets a value that defines shape style.
        /// </summary>
        /// <dev>
        /// 20.1.2.2.37 style (Shape Style)
        /// </dev>
        public DmlShapeStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        /// <summary>
        /// Gets or sets a collection that defines visual effects applied to a shape.
        /// </summary>
        /// <dev>
        /// 20.1.8.26 effectLst (Effect Container)
        /// </dev>
        public DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        /// <summary>
        /// Gets or sets a value that defines optional scene-level 3D properties to apply to an object.
        /// </summary>
        /// <dev>
        /// 20.1.4.1.26 scene3d (3D Scene Properties)
        /// </dev>
        public DmlScene3DProperties Scene3DProperties
        {
            get { return mScene3DProperties; }
            set { mScene3DProperties = value; }
        }

        /// <summary>
        /// Gets or sets a value that defines the 3D properties associated with a particular shape in DrawingML.
        /// </summary>
        /// <dev>
        /// 20.1.5.12 sp3d (Apply 3D shape properties)
        /// </dev>
        public DmlShape3DProperties Shape3DProperties
        {
            get { return mShape3DProperties; }
            set { mShape3DProperties = value; }
        }

        /// <summary>
        /// Represents collection of spPr Dml extensions.
        /// </summary>
        public StringToObjDictionary<DmlExtension> SpPrExtensions
        {
            get { return mSpPrExtensions; }
            set { mSpPrExtensions = value; }
        }

        /// <summary>
        /// Gets <see cref="DmlChartSpPr"/>
        /// </summary>
        internal DmlChartSpPr GetDmlChartSpPr()
        {
            DmlChartSpPr spPr = new DmlChartSpPr();
            spPr.Outline = Outline;
            spPr.Fill = Fill;
            return spPr;
        }

        private StringToObjDictionary<DmlExtension> mSpPrExtensions;
        private DmlTransform mTransform;
        private BWMode mBwMode = (BWMode)255; // Set special value to avoid BWMode writing. 
        private DmlGeometry mGeometry;
        private DmlFill mFill;
        private DmlOutline mOutline;
        private DmlShapeStyle mStyle;
        private DmlShapeEffectsCollection mEffects;
        private DmlScene3DProperties mScene3DProperties;
        private DmlShape3DProperties mShape3DProperties;
    }
}
