// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2012 by Andrey Noskov

namespace Aspose.Words 
{
    /// <summary>
    /// Container to store FallBack of AlternateContent and Requires attribute. 
    /// In case when we need FallBack, Choice is in the model.  
    /// </summary>
    internal class AlternateContent : IComplexAttr
    {
        public override int GetHashCode()
        {
            // Nodes do not support GetHashCode now: take from type.
            int hashCode = (mFallBack != null) ? mFallBack.GetType().GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ ((mRequires != null) ? mRequires.GetHashCode() : 0);
            return hashCode;
        }

        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        public IComplexAttr DeepCloneComplexAttr()
        {
            AlternateContent altc = (AlternateContent)MemberwiseClone();
            altc.mFallBack = (mFallBack != null) ? (CompositeNode)mFallBack.Clone(true) : null;
            return altc;
        }

        /// <summary>
        /// Fallback element allows clients that do not support Requires namespace to see an appropriate alternative
        /// representation of AlternateContent.
        /// </summary>
        /// <remarks>
        /// When we clone whole document, parent is changed for each node, but FallBack in this case it is alternate node outside of the main tree, 
        /// that is why FallBack still belongs to the original document, and that is why we need to set parent directly to FallBack shape.
        /// Please see <see cref="Drawing.ShapeBase.UpdateFallBackParent"/> method.
        /// </remarks>
        internal CompositeNode FallBack
        {
            get { return mFallBack; }
            set { mFallBack = (value == null) ? value :  CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(value, value.Document); }
        }

        internal string Requires
        {
            get { return mRequires; }
            set { mRequires = value; }
        }

        private CompositeNode mFallBack;
        private string mRequires;
    }
}
