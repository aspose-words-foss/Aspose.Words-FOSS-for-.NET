// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// CT_LayoutVariablePropertySet
    /// </summary>
    internal class DmlLayoutVariablePropertySet : DmlDiagramLayoutNodeContentItem
    {
        internal DmlLayoutVariablePropertySet()
        {
            mPrBag.ParentBagProvider = new DmlLayoutVariablePropertySetParentBagProvider(gDefaultPr);
        }

        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitLayoutVariablePropertySet(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.LayoutVariablePropertySet; }
        }

        internal DmlLayoutVariablePropertySet Clone()
        {
            DmlLayoutVariablePropertySet lhs = (DmlLayoutVariablePropertySet)MemberwiseClone();
            lhs.mPrBag = mPrBag.Clone();

            if (HasExtensions)
                lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal override DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            return Clone();
        }

        internal bool OrgChart
        {
            get { return (bool)GetProperty(DmlLayoutVariableAttr.OrgChart); }
            set { SetProperty(DmlLayoutVariableAttr.OrgChart, value); }
        }

        internal DmlDiagramNodeCount ChildMax
        {
            get { return (DmlDiagramNodeCount)GetProperty(DmlLayoutVariableAttr.ChildMax); }
            set { SetProperty(DmlLayoutVariableAttr.ChildMax, value); }
        }

        internal DmlDiagramNodeCount ChildPref
        {
            get { return (DmlDiagramNodeCount)GetProperty(DmlLayoutVariableAttr.ChildPref); }
            set { SetProperty(DmlLayoutVariableAttr.ChildPref, value); }
        }

        internal bool BulletEnabled
        {
            get { return (bool)GetProperty(DmlLayoutVariableAttr.BulletEnabled); }
            set { SetProperty(DmlLayoutVariableAttr.BulletEnabled, value); }
        }

        internal DmlDiagramDirection Direction
        {
            get { return (DmlDiagramDirection)GetProperty(DmlLayoutVariableAttr.Direction); }
            set { SetProperty(DmlLayoutVariableAttr.Direction, value); }
        }

        internal DmlHierBranchStyle HierBranch
        {
            get { return (DmlHierBranchStyle)GetProperty(DmlLayoutVariableAttr.HierBranchStyle); }
            set { SetProperty(DmlLayoutVariableAttr.HierBranchStyle, value); }
        }

        internal DmlAnimOne AnimOne
        {
            get { return (DmlAnimOne)GetProperty(DmlLayoutVariableAttr.AnimOne); }
            set { SetProperty(DmlLayoutVariableAttr.AnimOne, value); }
        }

        internal DmlAnimLevel AnimLevel
        {
            get { return (DmlAnimLevel)GetProperty(DmlLayoutVariableAttr.AnimLevel); }
            set { SetProperty(DmlLayoutVariableAttr.AnimLevel, value); }
        }

        internal DmlResizeHandles ResizeHandles
        {
            get { return (DmlResizeHandles)GetProperty(DmlLayoutVariableAttr.ResizeHandles); }
            set { SetProperty(DmlLayoutVariableAttr.ResizeHandles, value); }
        }

        internal object GetProperty(DmlLayoutVariableAttr attr)
        {
            return mPrBag.GetProperty((int)attr);
        }

        internal void SetProperty(DmlLayoutVariableAttr attr, object value)
        {
            mPrBag.SetProperty((int)attr, value);
        }

        /// <summary>
        /// Returns null if property is not set explicitly
        /// </summary>
        internal object GetDirectProperty(DmlLayoutVariableAttr attr)
        {
            return mPrBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlLayoutVariableAttr attr)
        {
            return mPrBag.IsPropertySpecified((int)attr);
        }

        internal void SetParent(DmlLayoutVariablePropertySet parent)
        {
            mPrBag.ParentBagProvider = new DmlLayoutVariablePropertySetParentBagProvider(parent);
        }

        private IDmlHierarchicalPropertyBag mPrBag = new DmlHierarchicalPropertyBag();

        static DmlLayoutVariablePropertySet()
        {
            gDefaultPr = new DmlLayoutVariablePropertySet();
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.OrgChart, false);
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.ChildMax, new DmlDiagramNodeCount(-1));
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.ChildPref, new DmlDiagramNodeCount(-1));
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.BulletEnabled, false);
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.Direction, DmlDiagramDirection.Normal);
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.HierBranchStyle, DmlHierBranchStyle.Standard);
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.AnimOne, DmlAnimOne.One);
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.AnimLevel, DmlAnimLevel.None);
            gDefaultPr.SetProperty(DmlLayoutVariableAttr.ResizeHandles, DmlResizeHandles.Relative);
        }

        private static readonly DmlLayoutVariablePropertySet gDefaultPr;

        private class DmlLayoutVariablePropertySetParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            public DmlLayoutVariablePropertySetParentBagProvider(DmlLayoutVariablePropertySet parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPrBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlLayoutVariablePropertySetParentBagProvider lhs =
                    (DmlLayoutVariablePropertySetParentBagProvider)MemberwiseClone();

                if (mParentPr != null)
                    lhs.mParentPr = mParentPr.Clone();

                return lhs;
            }

            private DmlLayoutVariablePropertySet mParentPr;
        }
    }
}
