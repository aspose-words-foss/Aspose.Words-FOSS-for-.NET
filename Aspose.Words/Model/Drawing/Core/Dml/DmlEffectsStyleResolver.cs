// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/30/2015 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Class simplifies access to effects applied through theme reference.
    /// </summary>
    internal class DmlEffectsStyleResolver
    {
        internal DmlEffectsStyleResolver(DmlShapeStyle style, IThemeProvider theme)
        {
            mStyle = style;
            mTheme = theme;
        }

        private EffectStyle EffectStyle
        {
            get
            {
                if ((mEffectStyle == null) && (mStyle != null))
                    mEffectStyle = mStyle.GetEffectStyle(mTheme);

                return mEffectStyle;
            }
        }

        public DmlScene3DProperties Scene3DProperties
        {
            get { return (EffectStyle == null) ? null : EffectStyle.Scene3DProperties; }
        }

        public DmlShape3DProperties Shape3DProperties
        {
            get { return (EffectStyle == null) ? null : EffectStyle.Shape3DProperties; }
        }

        public DmlShapeEffectsCollection Effects
        {
            get { return (EffectStyle == null) ? null : EffectStyle.Effects; }
        }

        private EffectStyle mEffectStyle;
        private readonly DmlShapeStyle mStyle;
        private readonly IThemeProvider mTheme;
    }
}
