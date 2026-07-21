// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Transforms;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// Represents CT_ShapeProperties complex type.
    /// </summary>
    internal class DmlShapeProperties : IDmlShapePrSource, IDmlCommonShapePrSource
    {
        internal DmlShapeProperties(DocumentBase doc)
        {
            mDoc = doc;
        }

        public DmlTransform Transform
        {
            get { return mTransform; }
            set { mTransform = value; }
        }
        
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
        /// Defines optional scene-level 3D properties to apply to an object.
        /// </summary>
        public DmlScene3DProperties Scene3DProperties
        {
            get { return (mScene3DProperties != null) ? mScene3DProperties : EffectsStyleResolver.Scene3DProperties; }
            set { mScene3DProperties = value; }
        }

        /// <summary>
        /// Defines the 3D properties associated with a particular shape in DrawingML.
        /// </summary>
        public DmlShape3DProperties Shape3DProperties
        {
            get { return (mShape3DProperties != null) ? mShape3DProperties : EffectsStyleResolver.Shape3DProperties; }
            set { mShape3DProperties = value; }
        }

        public StringToObjDictionary<DmlExtension> SpPrExtensions
        {
            get { return mExtLst; }
            set { mExtLst = value; }
        }

        public DmlShapeEffectsCollection Effects
        {
            get
            {
                if ((mEffects == null) && (EffectsStyleResolver.Effects != null))
                    mEffects = EffectsStyleResolver.Effects;

                return mEffects;
            }
            set { mEffects = value; }
        }

        private DmlEffectsStyleResolver EffectsStyleResolver
        {
            get
            {
                if (mEffectsStyleResolver == null)
                    mEffectsStyleResolver = new DmlEffectsStyleResolver(Style, mDoc.GetThemeInternal());

                return mEffectsStyleResolver;
            }
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
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentBase mDoc;
        private DmlEffectsStyleResolver mEffectsStyleResolver;
        private StringToObjDictionary<DmlExtension> mExtLst;
    }
}
