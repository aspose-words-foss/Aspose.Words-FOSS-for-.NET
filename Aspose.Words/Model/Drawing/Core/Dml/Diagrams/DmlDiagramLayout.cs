// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams
{
    /// <summary>
    /// 21.4.6 Layout Definition
    /// </summary>
    internal class DmlDiagramLayout : DmlExtensionListSource
    {
        internal DmlDiagramLayout(string relId)
        {
            mRelId = relId;
        }

        internal string RelId
        {
            get { return mRelId; }
        }

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

        internal string DefaultStyle
        {
            get { return mDefaultStyle; }
            set { mDefaultStyle = value; }
        }

        internal DmlDiagramLayoutNode LayoutNode
        {
            get { return mLayoutNode; }
            set { mLayoutNode = value; }
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

        internal DmlDiagramDataModel SampleData
        {
            get { return mSampleData; }
            set { mSampleData = value; }
        }

        internal DmlDiagramDataModel ColorTransformSampleData
        {
            get { return mColorTransformSampleData; }
            set { mColorTransformSampleData = value; }
        }

        internal DmlDiagramDataModel StyleData
        {
            get { return mStyleData; }
            set { mStyleData = value; }
        }

        private DmlDiagramDataModel mSampleData;
        private DmlDiagramDataModel mColorTransformSampleData;
        private DmlDiagramDataModel mStyleData;
        private DmlDiagramString[] mTitles;
        private DmlDiagramString[] mDescriptions;
        private DmlColorTransformCategory[] mCategories;
        private string mUniqueId;
        private string mMinVersion;
        private string mDefaultStyle;
        private DmlDiagramLayoutNode mLayoutNode;
        private readonly string mRelId;
    }
}
