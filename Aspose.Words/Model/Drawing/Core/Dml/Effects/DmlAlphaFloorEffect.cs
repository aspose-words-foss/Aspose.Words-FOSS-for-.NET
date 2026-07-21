// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.3 alphaFloor (Alpha Floor Effect)
    /// This element represents an alpha floor effect.
    /// Alpha (opacity) values less than 100% are changed to zero. 
    /// In other words, anything partially transparent becomes fully transparent.
    /// </summary>
    internal class DmlAlphaFloorEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.AlphaFloor; }
        }
    }
}