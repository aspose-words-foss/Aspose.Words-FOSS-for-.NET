// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// Base class for blip fill effects.
    /// </summary>
    internal abstract class DmlEffect
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlEffect value = (DmlEffect)obj;

            return (value.Type == Type);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal abstract DmlEffectType Type { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get; }

        internal virtual DmlEffect Clone()
        {
            return (DmlEffect)MemberwiseClone();
        }
    }
}
