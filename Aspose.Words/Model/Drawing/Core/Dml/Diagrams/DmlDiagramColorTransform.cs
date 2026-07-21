// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams
{
    /// <summary>
    /// 21.4.4.3 colorsDef (Color Transform Definitions)
    /// </summary>
    internal class DmlDiagramColorTransform : DmlExtensionListSource
    {
        internal string UniqueId
        {
            get { return mUniqueId; }
            set { mUniqueId = value; }
        }

        internal string MinVersion
        {
            get { return mMinVersion; }
            set { mMinVersion = value; }
        }

        internal DmlColorTransformationStyleLabel[] StyleLabels
        {
            get { return mStyleLabels; }
            set { mStyleLabels = value; }
        }

        internal DmlColorTransformCategory[] Categories
        {
            get { return mCategories; }
            set { mCategories = value; }
        }

        internal DmlDiagramString[] Titles
        {
            get { return mTitles; }
            set { mTitles = value; }
        }

        internal DmlDiagramString[] Descriptions
        {
            get { return mDescriptions; }
            set { mDescriptions = value; }
        }

        public DmlColorTransformationStyleLabel GetStyleLabel(string labelName)
        {
            if (!StringUtil.HasChars(labelName))
                return null;

            foreach (DmlColorTransformationStyleLabel label in mStyleLabels)
                if (label.Name == labelName)
                    return label;

            return null;
        }

        private DmlDiagramString[] mTitles;
        private DmlDiagramString[] mDescriptions;
        private string mUniqueId;
        private string mMinVersion;
        private DmlColorTransformationStyleLabel[] mStyleLabels;
        private DmlColorTransformCategory[] mCategories;
    }
}
