// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.5 alphaMod (Alpha Modulate Effect)
    /// This element represents an alpha modulate effect.
    /// Effect alpha (opacity) values are multiplied by a fixed percentage. 
    /// The effect container specifies an effect containing alpha values to modulate.
    /// </summary>
    internal class DmlAlphaModulateEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.AlphaModulate; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlAlphaModulateEffect value = (DmlAlphaModulateEffect)obj;

            return ListUtil.CheckAreEquals(value.EffectsContainer, EffectsContainer);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (DmlEffect effect in EffectsContainer)
                hash ^= effect.GetHashCode();
            return hash;
        }

        internal IList<DmlEffect> EffectsContainer
        {
            get { return mEffectsContainer; }
        }

        private readonly List<DmlEffect> mEffectsContainer = new List<DmlEffect>();
    }
}