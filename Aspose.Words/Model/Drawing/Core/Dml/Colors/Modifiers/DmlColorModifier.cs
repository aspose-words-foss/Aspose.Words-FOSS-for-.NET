// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// Base class for color modifiers.
    /// </summary>
    internal abstract class DmlColorModifier : IDmlColorModifier
    {
        /// <summary>
        /// Apply the modifier to the color.
        /// </summary>
        /// <returns>The modified color.</returns>
        public virtual DrColor Modify(DrColor color)
        {
            return color; // return the original color.
        }

        public abstract IDmlColorModifier Clone();

        public abstract void Write(string prefix, IDmlShapeWriterContext writer);

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hash code does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlColorModifier value = (DmlColorModifier)obj;

            return (value.ModifierType == ModifierType);
        }

        public override int GetHashCode()
        {
            return ModifierType.GetHashCode();
        }

        public abstract DmlColorModifierType ModifierType { get; }
    }
}
