// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2015 by Nikolay Eryomin

using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// 20.1.4.1.11 effectStyle (Effect Style)
    /// This element defines a set of effects and 3D properties that can be applied to an object.
    /// </summary>
    internal class EffectStyle
    {
        /// <summary>
        /// Defines optional scene-level 3D properties to apply to an object.
        /// </summary>
        internal DmlScene3DProperties Scene3DProperties
        {
            get { return mScene3DProperties; }
            set { mScene3DProperties = value; }
        }

        /// <summary>
        /// Defines the 3D properties to apply to an object.
        /// </summary>
        internal DmlShape3DProperties Shape3DProperties
        {
            get { return mShape3DProperties; }
            set { mShape3DProperties = value; }
        }

        /// <summary>
        /// Collection of effects applied to an object.
        /// </summary>
        internal DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        internal virtual EffectStyle Clone()
        {
            EffectStyle newEffectStyle = new EffectStyle();

            if (mScene3DProperties != null)
                newEffectStyle.mScene3DProperties = mScene3DProperties.Clone();

            if (mShape3DProperties != null)
                newEffectStyle.mShape3DProperties = mShape3DProperties.Clone();

            if (mEffects != null)
                newEffectStyle.mEffects = mEffects.Clone();

            return newEffectStyle;
        }
        
        private DmlScene3DProperties mScene3DProperties;
        private DmlShape3DProperties mShape3DProperties;
        private DmlShapeEffectsCollection mEffects;
    }
}
