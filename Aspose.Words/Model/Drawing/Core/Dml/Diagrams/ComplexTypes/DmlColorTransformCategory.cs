// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.4 cat (Category)
    /// This element specifies a category in the user interface where this layout definition displays to the user.
    /// </summary>
    internal class DmlColorTransformCategory
    {
        internal string CategoryType
        {
            get { return mCategoryType; }
            set { mCategoryType = value; }
        }

        internal int Priority
        {
            get { return mPriority; }
            set { mPriority = value; }
        }

        private string mCategoryType;
        private int mPriority;
    }
}
