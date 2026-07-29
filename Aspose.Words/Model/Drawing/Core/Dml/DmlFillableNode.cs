// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2011 by Alexey Titov

using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;

namespace Aspose.Words.Drawing.Core.Dml
{
    internal abstract class DmlFillableNode : DmlNode, IDmlFillProvider
    {
        /// <summary>
        /// Clones this instance of <see cref="DmlFillableNode"/>.
        /// </summary>
        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            DmlFillableNode lhs = (DmlFillableNode)base.Clone(isCloneChildren, cloningListener);

            if (mFill != null)
                lhs.mFill = mFill.Clone();

            if (mEffects != null)
                lhs.mEffects = mEffects.Clone();

            return lhs;
        }

        public DmlFill Fill
        {
            get
            {
                // If fill isn't specified then use the fill provided by the shape's style.
                if (mFill == null)
                    mFill = new DmlStyleFill();
                return mFill;
            }
            set { mFill = value; }
        }
        
        public virtual DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        /// <summary>
        /// Returns true if DmlNode has transparency. Special shadow processing is required in this case.
        /// </summary>
        [JavaThrows(true)]
        public virtual bool HasTransparency
        {
            get
            {
                // If fill has transparency we also should build shadow in a special way.
                if ((Fill == null) || (Fill.DmlFillType == DmlFillType.NoFill))
                    return false;

                return ((Fill.Opacity < 1) || (Fill.Opacity2 < 1));
            }
        }

        /// <summary>
        /// Finds the first fill in parent hierarchy that can be drawn.
        /// </summary>
        /// <returns></returns>
        DmlFill IDmlFillProvider.FindDrawableFillInParents()
        {
            if (!(Fill is DmlGroupFill))
                return Fill;

            ShapeBase dml = Parent as ShapeBase;
            if (dml != null && dml.DmlNode is IDmlFillProvider)
                return ((IDmlFillProvider)dml.DmlNode).FindDrawableFillInParents();
            return null;
        }

        private DmlFill mFill;
        protected DmlShapeEffectsCollection mEffects;
    }
}
