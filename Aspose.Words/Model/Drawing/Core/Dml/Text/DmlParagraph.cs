// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.2.6 p (Text Paragraphs)
    /// This element specifies the presence of a paragraph of text within the containing 
    /// text body. The paragraph is the highest level text separation mechanism within a 
    /// text body. A paragraph can contain text paragraph properties associated with the 
    /// paragraph. If no properties are listed then properties specified in the defPPr 
    /// element are used.
    /// </summary>
    internal class DmlParagraph
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        internal DmlParagraph()
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with defining paragraph properties and run properties at paragraph
        /// end.
        /// </summary>
        internal DmlParagraph(DmlParagraphProperties paragraphProperties, DmlRunProperties endRunProperties)
        {
            mParagraphProperties = paragraphProperties;
            mEndParagraphRunProperties = endRunProperties;
        }

        internal DmlParagraph Clone()
        {
            DmlParagraph lhs = (DmlParagraph)MemberwiseClone();

            if (mParagraphProperties != null)
                lhs.mParagraphProperties = mParagraphProperties.Clone();

            if (mEndParagraphRunProperties != null)
                lhs.mEndParagraphRunProperties = mEndParagraphRunProperties.Clone();

            if (mElements != null)
            {
                lhs.mElements = new List<DmlParagraphTextElementBase>(mElements.Count);
                foreach (DmlParagraphTextElementBase element in mElements)
                    lhs.AddElement(element.Clone());
            }

            return lhs;
        }

        internal void AddElement(DmlParagraphTextElementBase element)
        {
            element.SetParagraph(this);
            mElements.Add(element);
        }

        internal void SetTextBody(DmlTextBody textBody)
        {
            mTextBody = textBody;
            if (textBody == null)
                return;

            Properties.SetParentProperties(textBody.TextListStyles);
            Properties.DefaultRunProperties.SetParentProperties(this, textBody.TextListStyles);
            EndParagraphRunProperties.SetParentProperties(this, textBody.TextListStyles);
        }

        /// <summary>
        /// Returns text of child runs.
        /// </summary>
        internal string GetRunText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (DmlParagraphTextElementBase element in Elements)
            {
                DmlRun run = element as DmlRun;
                if (run != null)
                    stringBuilder.Append(run.Text);
            }

            return stringBuilder.ToString();
        }

        internal IList<DmlParagraphTextElementBase> Elements
        {
            get { return mElements; }
        }

        internal DmlParagraphProperties Properties
        {
            get
            {
                if (mParagraphProperties == null)
                    mParagraphProperties = new DmlParagraphProperties();
                return mParagraphProperties;
            }
        }

        internal DmlRunProperties EndParagraphRunProperties
        {
            get
            {
                if (mEndParagraphRunProperties == null)
                    mEndParagraphRunProperties = new DmlRunProperties();
                return mEndParagraphRunProperties;
            }
            set { mEndParagraphRunProperties = value; }
        }

        internal DmlTextBody TextBody
        {
            get { return mTextBody; }
        }

        /// <summary>
        /// Gets the fist text element of this paragraph. Returns <b>null</b> if the paragraph is empty.
        /// </summary>
        internal DmlParagraphTextElementBase FirstElement
        {
            get { return (mElements.Count > 0) ? mElements[0] : null; }
        }

        private List<DmlParagraphTextElementBase> mElements = new List<DmlParagraphTextElementBase>();
        private DmlRunProperties mEndParagraphRunProperties;
        private DmlParagraphProperties mParagraphProperties;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlTextBody mTextBody;
    }
}
