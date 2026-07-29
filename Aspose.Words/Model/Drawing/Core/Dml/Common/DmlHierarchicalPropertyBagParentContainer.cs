// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Common
{
    internal class DmlHierarchicalPropertyBagParentContainer : IDmlHierarchicalPropertyBagParentProvider
    {
        internal DmlHierarchicalPropertyBagParentContainer(IDmlHierarchicalPropertyBag parentBag)
        {
            mParentBag = parentBag;
        }

        internal DmlHierarchicalPropertyBagParentContainer()
        {
        }

        public IDmlHierarchicalPropertyBag ParentBag
        {
            get { return mParentBag; }
            set { mParentBag = value; }
        }

        public IDmlHierarchicalPropertyBagParentProvider Clone()
        {
            DmlHierarchicalPropertyBagParentContainer lhs = (DmlHierarchicalPropertyBagParentContainer)MemberwiseClone();
            if (mParentBag != null)
                lhs.mParentBag = mParentBag.Clone();

            return lhs;
        }

        private IDmlHierarchicalPropertyBag mParentBag;
    }
}
