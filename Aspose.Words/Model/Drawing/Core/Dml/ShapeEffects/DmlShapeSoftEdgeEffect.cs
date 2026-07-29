// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// 20.1.8.53 softEdge (Soft Edge Effect)
    /// This element specifies a soft edge effect. The edges of the shape are blurred, while the fill is not affected.
    /// </summary>
    internal class DmlShapeSoftEdgeEffect : DmlShapeEffect
    {
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeSoftEdgeEffect value = (DmlShapeSoftEdgeEffect)obj;

            return MathUtil.AreEqual(value.Radius, Radius);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Radius.GetHashCode();
            return hash;
        }

        internal override DmlShapeEffect Clone()
        {
            return (DmlShapeSoftEdgeEffect)MemberwiseClone();
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.SoftEdges; }
        }

        /// <summary>
        /// Specifies the radius of blur to apply to the edges.
        /// </summary>
        internal double Radius
        {
            get { return mRadius; }
            set { mRadius = value; }
        }

        private double mRadius;
    }
}
