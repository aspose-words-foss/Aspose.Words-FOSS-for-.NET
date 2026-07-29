// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 20.1.2.2.40 txBody (Shape Text Body)
    /// This element specifies the existence of text to be contained within the corresponding shape. 
    /// All visible text and visible text related properties are contained within this element. 
    /// There can be multiple paragraphs and within paragraphs multiple runs of text.
    /// </summary>
    internal class DmlTextBody
    {
        internal DmlTextBody Clone()
        {
            DmlTextBody lhs = (DmlTextBody)MemberwiseClone();

            if (mTextListStyles != null)
                lhs.mTextListStyles = mTextListStyles.Clone();

            if (mTextBodyProperties != null)
                lhs.mTextBodyProperties = mTextBodyProperties.Clone();

            if (mParagraphs != null)
            {
                lhs.mParagraphs = new List<DmlParagraph>(mParagraphs.Count);
                foreach (DmlParagraph para in mParagraphs)
                    lhs.AddParagraph(para.Clone());
            }

            return lhs;
        }

        internal void AddParagraph(DmlParagraph paragraph)
        {
            paragraph.SetTextBody(this);
            Paragraphs.Add(paragraph);
        }

        internal DmlTextListStyles TextListStyles
        {
            get
            {
                if (mTextListStyles == null)
                    mTextListStyles = new DmlTextListStyles();
                return mTextListStyles;
            }
            set { mTextListStyles = value; }
        }

        internal DmlTextBodyProperties Properties
        {
            get
            {
                if (mTextBodyProperties == null)
                    mTextBodyProperties = new DmlTextBodyProperties();
                return mTextBodyProperties;
            }
            set { mTextBodyProperties = value; }
        }

        internal List<DmlParagraph> Paragraphs
        {
            get
            {
                if (mParagraphs == null)
                    mParagraphs = new List<DmlParagraph>();
                return mParagraphs;
            }
            set { mParagraphs = value; }
        }

        private List<DmlParagraph> mParagraphs;
        private DmlTextBodyProperties mTextBodyProperties;
        private DmlTextListStyles mTextListStyles;
    }
}