// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Drawing.Core.Dml.Path;

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal class DmlGeometry
    {
        /// <summary>
        /// Adds the path.
        /// </summary>
        /// <param name="path">The path. If the path is null then method do nothing.</param>
        internal void AddPath(DmlPath path)
        {
            if (path == null)
                return;
            mPaths.Add(path);
        }

        internal DmlGeometry Clone()
        {
            DmlGeometry lhs = (DmlGeometry)MemberwiseClone();

            lhs.mGuides = mGuides.Clone();

            if (lhs.mPaths != null)
            {
                lhs.mPaths = new List<DmlPath>();
                foreach (DmlPath path in mPaths)
                    lhs.mPaths.Add(path.Clone());
            }

            if (mConnectionSites != null)
            {
                lhs.mConnectionSites = new List<DmlConnectionSite>();
                foreach (DmlConnectionSite site in mConnectionSites)
                    lhs.mConnectionSites.Add(site.Clone());
            }

            if (mAdjustHandles != null)
            {
                lhs.mAdjustHandles = new List<DmlAdjustHandle>();
                foreach (DmlAdjustHandle adjustHandle in mAdjustHandles)
                    lhs.mAdjustHandles.Add(adjustHandle.Clone());
            }

            if (mTxbxRect != null)
                lhs.mTxbxRect = mTxbxRect.Clone();

            return lhs;
        }

        /// <summary>
        /// Gets the guides.
        /// </summary>
        /// <remarks>
        /// Contains adjustable values and guides for the specified shape.
        /// </remarks>
        internal DmlGuides Guides
        {
            get { return mGuides; }
        }

        internal string PresetName
        {
            get
            {
                if (mName == null)
                    return "";
                return mName;
            }
            set { mName = value; }
        }

        internal IList<DmlPath> Paths
        {
            get { return mPaths; }
        }

        internal IList<DmlAdjustHandle> AdjustHandles
        {
            get { return mAdjustHandles; }
        }

        /// <summary>
        /// Returns rectangular bounding box for text within a shape.
        /// Note: Guides must be calculated.
        /// </summary>
        internal RectangleF TextboxRect
        {
            get
            {
                if (mTxbxRect == null)
                    return RectangleF.Empty;
                return mTxbxRect.GetRectangle(mGuides,
                    string.Equals(PresetName, "uparrow", System.StringComparison.OrdinalIgnoreCase));
            }
        }

        internal DmlTextBoxRect DmlTextboxRect
        {
            get { return mTxbxRect; }
            set { mTxbxRect = value; }
        }

        /// <summary>
        /// Collection of the shape connection sites.
        /// </summary>
        internal IList<DmlConnectionSite> ConnectionSites
        {
            get { return mConnectionSites; }
            set { mConnectionSites = value; }
        }
        
        private List<DmlPath> mPaths = new List<DmlPath>();
        private DmlGuides mGuides = new DmlGuides();
        private IList<DmlConnectionSite> mConnectionSites = new List<DmlConnectionSite>();

        private string mName;
        private DmlTextBoxRect mTxbxRect;        
        private List<DmlAdjustHandle> mAdjustHandles = new List<DmlAdjustHandle>();
    }
}
