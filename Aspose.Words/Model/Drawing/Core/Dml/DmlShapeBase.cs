// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2011 by Alexey Titov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Represents a base class for all nodes having shape behavior.
    /// </summary>
    internal abstract class DmlShapeBase : DmlFillableNode, IDmlShapePrSource, IDmlCommonShapePrSource
    {
        /// <summary>
        /// Shows if corrected size should be used for building geometry of the shape.
        /// Default is <c>false</c>.
        /// </summary>
        internal virtual bool ShouldBuildUsingSqueezedSize()
        {
            return false;
        }

        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            DmlShapeBase lhs = (DmlShapeBase)base.Clone(isCloneChildren, cloningListener);

            if (mTransform != null)
                lhs.mTransform = mTransform.Clone();

            if (mGeometry != null)
                lhs.Geometry = mGeometry.Clone();

            if (mOutline != null)
                lhs.Outline = mOutline.Clone();

            if (mStyle != null)
                lhs.Style = mStyle.Clone();

            if (mScene3DProperties != null)
                lhs.mScene3DProperties = mScene3DProperties.Clone();

            if (mShape3DProperties != null)
                lhs.mShape3DProperties = mShape3DProperties.Clone();

            lhs.mEffectsStyleResolver = null;

            if (mSpPrExtensions != null)
                lhs.mSpPrExtensions = DmlExtensionListSource.CloneExtensions(mSpPrExtensions);

            return lhs;
        }

        public DmlGeometry Geometry
        {
            get
            {
                if (mGeometry == null)
                    mGeometry = new DmlGeometry();
                return mGeometry;
            }
            set { mGeometry = value; }
        }

        public DmlOutline Outline
        {
            get
            {
                // If outline isn't specified then we will use the outline provided by the shape's style.
                // Please use Outline.DirectPropertiesCount to find out outline isn't specified directly. 
                if (mOutline == null)
                    mOutline = new DmlOutline();

                return mOutline;
            }
            set { mOutline = value; }
        }

        public BWMode BWMode
        {
            get { return mDmlBWMode; }
            set { mDmlBWMode = value; }
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

        public override DmlShapeEffectsCollection Effects
        {
            get
            {
                if ((mEffects == null) && (EffectsStyleResolver.Effects != null))
                    mEffects = EffectsStyleResolver.Effects;

                return mEffects;
            }
            set { mEffects = value; }
        }

        public DmlShapeStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        public StringToObjDictionary<DmlExtension> SpPrExtensions
        {
            get { return mSpPrExtensions; }
            set { mSpPrExtensions = value; }
        }

        private DmlEffectsStyleResolver EffectsStyleResolver
        {
            get
            {
                if (mEffectsStyleResolver == null)
                    mEffectsStyleResolver = new DmlEffectsStyleResolver(Style, Dml.DocumentTheme);

                return mEffectsStyleResolver;
            }
        }

        private StringToObjDictionary<DmlExtension> mSpPrExtensions;
        private DmlGeometry mGeometry;
        private DmlOutline mOutline;
        private DmlShapeStyle mStyle;
        private BWMode mDmlBWMode = (BWMode)255; // Set special value to avoid BWMode writing. 
        private DmlScene3DProperties mScene3DProperties;
        private DmlShape3DProperties mShape3DProperties;
        private DmlEffectsStyleResolver mEffectsStyleResolver;
    }
}
