// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    internal class DmlParagraphTextElementBase
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        internal DmlParagraphTextElementBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with defining run properties.
        /// </summary>
        internal DmlParagraphTextElementBase(DmlRunProperties runProperties)
        {
            mRunProperties = runProperties;
        }

        public virtual DmlParagraphTextElementBase Clone()
        {
            DmlParagraphTextElementBase lhs = (DmlParagraphTextElementBase)MemberwiseClone();

            if (mRunProperties != null)
                lhs.mRunProperties = mRunProperties.Clone();

            return lhs;
        }

        public virtual void SetParagraph(DmlParagraph paragraph)
        {
            mParagraph = paragraph;
            RunProperties.SetParentProperties(paragraph.Properties.DefaultRunProperties);
        }

        public DmlParagraph Paragraph
        {
            get { return mParagraph; }
        }

        public DmlRunProperties RunProperties
        {
            get
            {
                if (mRunProperties == null)
                    mRunProperties = new DmlRunProperties();
                return mRunProperties;
            }
            protected set { mRunProperties = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlParagraph mParagraph;
        private DmlRunProperties mRunProperties;
    }
}
