// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/04/2023 by Alexander Zhiltsov

using Aspose.Collections;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Transforms;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents CT_ShapeProperties complex type for charts.
    /// </summary>
    internal class DmlChartShapeProperties : IDmlShapePrSource, IDmlCommonShapePrSource
    {
        public DmlTransform Transform
        {
            get { return mTransform; }
            set { mTransform = value; }
        }

        /// <summary>
        /// Gets or sets preset geometric shape properties.
        /// </summary>
        public DmlGeometry Geometry
        {
            get { return mGeometry; }
            set { mGeometry = value; }
        }

        public DmlFill Fill
        {
            get { return mFill; }
            set { mFill = value; }
        }

        public DmlOutline Outline
        {
            get { return mOutline; }
            set { mOutline = value; }
        }

        public DmlShapeStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        public BWMode BWMode
        {
            get { return mBwMode; }
            set { mBwMode = value; }
        }

        /// <summary>
        /// Gets or sets scene-level 3D properties to apply to a shape.
        /// </summary>
        public DmlScene3DProperties Scene3DProperties
        {
            get { return mScene3DProperties; }
            set { mScene3DProperties = value; }
        }

        /// <summary>
        /// Gets or sets the 3D properties associated with a particular shape.
        /// </summary>
        public DmlShape3DProperties Shape3DProperties
        {
            get { return mShape3DProperties; }
            set { mShape3DProperties = value; }
        }

        public StringToObjDictionary<DmlExtension> SpPrExtensions
        {
            get { return mExtLst; }
            set { mExtLst = value; }
        }

        public DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        private BWMode mBwMode = (BWMode)255; // Set special value to avoid BWMode writing. 
        private DmlTransform mTransform;
        private DmlGeometry mGeometry;
        private DmlFill mFill;
        private DmlOutline mOutline;
        private DmlShapeStyle mStyle;
        private DmlShapeEffectsCollection mEffects;
        private DmlScene3DProperties mScene3DProperties;
        private DmlShape3DProperties mShape3DProperties;
        private StringToObjDictionary<DmlExtension> mExtLst;
    }
}
