// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Scene3D;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams
{
    /// <summary>
    /// 21.4.5.7 styleDef (Style Definition)
    /// </summary>
    internal class DmlDiagramStyleDefinition : DmlExtensionListSource
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

        internal DmlScene3DProperties Scene3D
        {
            get { return mScene3D; }
            set { mScene3D = value; }
        }

        internal DmlStyleLabel[] StyleLabels
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

        public DmlStyleLabel GetStyleLabel(string labelName)
        {
            if (!StringUtil.HasChars(labelName))
                return null;

            foreach (DmlStyleLabel label in mStyleLabels)
                if (label.Name == labelName)
                    return label;

            return null;
        }

        private DmlDiagramString[] mTitles;
        private DmlDiagramString[] mDescriptions;
        private DmlColorTransformCategory[] mCategories;
        private string mUniqueId;
        private string mMinVersion;
        private DmlScene3DProperties mScene3D;
        private DmlStyleLabel[] mStyleLabels;
    }
}
