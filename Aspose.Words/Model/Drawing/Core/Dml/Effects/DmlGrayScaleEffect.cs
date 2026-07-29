// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.34 grayscl (Gray Scale Effect)
    /// This element specifies a gray scale effect. 
    /// Converts all effect color values to a shade of gray, 
    /// corresponding to their luminance. Effect alpha (opacity) values are unaffected.
    /// </summary>
    internal class DmlGrayScaleEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.GrayScale; }
        }
    }
}