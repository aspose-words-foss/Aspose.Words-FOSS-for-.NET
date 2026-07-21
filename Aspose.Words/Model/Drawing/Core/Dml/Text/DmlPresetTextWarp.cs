// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Drawing.Core.Dml.Path;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 20.1.9.19 prstTxWarp (Preset Text Warp)
    /// This element specifies when a preset geometric shape should be used to transform 
    /// a piece of text. This operation is known formally as a text warp. The generating 
    /// application should be able to render all preset geometries enumerated in the ST_TextShapeType list.
    /// </summary>
    internal class DmlPresetTextWarp
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

        internal DmlPresetTextWarp Clone()
        {
            DmlPresetTextWarp lhs = (DmlPresetTextWarp)MemberwiseClone();

            lhs.mGuides = mGuides.Clone();

            if (lhs.mPaths != null)
            {
                lhs.mPaths = new List<DmlPath>();
                foreach (DmlPath path in mPaths)
                    lhs.mPaths.Add(path.Clone());
            }

            return lhs;
        }

        internal DmlTextShapeType TextShapeType
        {
            get { return mTextShapeType; }
            set { mTextShapeType = value; }
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

        internal IList<DmlPath> Paths
        {
            get { return mPaths; }
        }

        private List<DmlPath> mPaths = new List<DmlPath>();
        private DmlTextShapeType mTextShapeType = DmlTextShapeType.None;
        private DmlGuides mGuides = new DmlGuides();
    }
}