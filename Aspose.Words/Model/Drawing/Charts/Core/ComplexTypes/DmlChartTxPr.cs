// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using System.Collections.Generic;
using System.Text;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Class represents txPr element (5.7.2.217).
    /// </summary>
    internal class DmlChartTxPr
    {
        static DmlChartTxPr()
        {
            // Looks like chart paragraphs has different defaults than shape ones.
            IDmlHierarchicalPropertyBag defaults = DmlParagraphPropertiesDefaults.Instance.Clone();
            defaults.SetProperty((int)DmlParagraphPropertiesIds.Alignment, ParagraphAlignment.Center);
            gDefaultParentBagProvider = new DmlHierarchicalPropertyBagParentContainer(defaults);
        }

        internal DmlParagraph AddParagraph()
        {
            DmlParagraph para = new DmlParagraph();
            para.Properties.SetParentProperties(gDefaultParentBagProvider);
            mParagraphs.Add(para);
            return para;
        }

        internal DmlChartTxPr Clone()
        {
            DmlChartTxPr lhs = (DmlChartTxPr)MemberwiseClone();
            if (mBodyPr != null)
                lhs.mBodyPr = mBodyPr.Clone();

            lhs.mEmptyBodyPr = mEmptyBodyPr.Clone();

            if (mLstStyle != null)
                lhs.mLstStyle = mLstStyle.Clone();

            lhs.mParagraphs = new List<DmlParagraph>();
            foreach (DmlParagraph paragraph in mParagraphs)
                lhs.mParagraphs.Add(paragraph.Clone());

            // If mParagraphs is not empty, mFirstParagraph is the first item of it and is already cloned.
            lhs.mFirstParagraph = (mParagraphs.Count == 0) && (mFirstParagraph != null)
                ? mFirstParagraph.Clone()
                : null;

            return lhs;
        }

        /// <summary>
        /// If <see cref="Paragraphs"/> is empty, adds a paragraph to it.
        /// </summary>
        internal void EnsureParagraphExists()
        {
            if (Paragraphs.Count == 0)
                Paragraphs.Add(FirstParagraph);
        }

        /// <summary>
        /// If <see cref="BodyPr"/> is empty, creates a new one.
        /// </summary>
        internal void EnsureBodyPrExists()
        {
            if (mBodyPr == null)
            {
                mBodyPr = mEmptyBodyPr;
                EnsureParagraphExists();
                // Without this, data labels are displayed with wrong font size.
                FirstParagraph.Properties.HasDefaultRunProperties = true;
            }
        }

        /// <summary>
        /// Returns text of the runs of the child paragraphs.
        /// </summary>
        internal string GetRunText()
        {
            if (mParagraphs.Count <= 1)
                return FirstParagraph.GetRunText();

            StringBuilder stringBuilder = new StringBuilder();
            bool isFirst = true;

            foreach (DmlParagraph paragraph in mParagraphs)
            {
                if (!isFirst)
                    stringBuilder.Append("\r");

                stringBuilder.Append(paragraph.GetRunText());

                isFirst = false;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Clears the properties so that default formatting is used.
        /// </summary>
        internal void Clear()
        {
            mBodyPr = null;
            mFirstParagraph = (mParagraphs.Count > 0) ? mParagraphs[0] : null;

            foreach (DmlParagraph paragraph in mParagraphs)
            {
                paragraph.Properties.Clear();
                paragraph.SetTextBody(null);
                paragraph.EndParagraphRunProperties = null;
            }
        }

        internal DmlTextBodyProperties BodyPr
        {
            get { return (mBodyPr == null) ? mEmptyBodyPr : mBodyPr; }
            set { mBodyPr = value; }
        }

        internal DmlTextListStyles LstStyle
        {
            get
            {
                if (mLstStyle == null)
                    mLstStyle = new DmlTextListStyles();

                return mLstStyle;
            }
        }

        internal DmlRunProperties RunPr
        {
            get { return FirstParagraph.Properties.DefaultRunProperties; }
        }

        /// <summary>
        /// Shortcut to the first paragraph in the collection.
        /// If the collection is empty, generated one paragraph without adding to the collection to not write it to
        /// the document.
        /// </summary>
        internal DmlParagraph FirstParagraph
        {
            get
            {
                // I am not sure why we need paragraphs in Pr element at all, but it seems MS Word uses only properties
                // of the first paragraph.
                // For data label from Waterfall.docx, MS Word takes pPr properties from the first paragraph, but rPr
                // properties from the last one: ignored now since all paragraphs have same formatting (WORDSNET-14824).
                if (mFirstParagraph == null)
                {
                    if (mParagraphs.Count > 0)
                    {
                        mFirstParagraph = mParagraphs[0];
                    }
                    else
                    {
                        mFirstParagraph = new DmlParagraph();
                        mFirstParagraph.Properties.SetParentProperties(gDefaultParentBagProvider);
                    }
                }

                return mFirstParagraph;
            }
        }

        /// <summary>
        /// Gets a list of paragraphs in this text properties.
        /// </summary>
        internal IList<DmlParagraph> Paragraphs
        {
            get { return mParagraphs; }
        }

        internal bool IsEmpty
        {
            get
            {
                if ((mBodyPr != null) || (mParagraphs.Count > 0))
                    return false;

                if (mFirstParagraph == null)
                    return true;

                return
                    mFirstParagraph.Properties.IsEmpty &&
                    ((mFirstParagraph.TextBody == null) || mFirstParagraph.TextBody.Properties.IsEmpty) &&
                    mFirstParagraph.EndParagraphRunProperties.IsEmpty;
            }
        }

        private DmlTextBodyProperties mBodyPr;
        private DmlTextListStyles mLstStyle;
        private DmlTextBodyProperties mEmptyBodyPr = new DmlTextBodyProperties();

        /// <summary>
        /// According to specification txPr must contain at least one paragraph, but can contain unlimited number
        ///  of paragraphs.
        /// I am not sure why we need paragraphs in Pr element at all, but it seems MS Word uses only properties
        /// of the first paragraph.
        /// Anyway lets read all the paragraphs into this list.
        /// </summary>
        private List<DmlParagraph> mParagraphs = new List<DmlParagraph>();

        private DmlParagraph mFirstParagraph;

        private static readonly IDmlHierarchicalPropertyBagParentProvider gDefaultParentBagProvider;
    }
}
