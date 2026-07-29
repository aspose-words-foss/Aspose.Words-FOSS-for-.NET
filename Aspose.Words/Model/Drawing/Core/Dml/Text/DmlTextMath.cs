// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Words.Math;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// A CT_TextMath element that specifies a math content inside of a text paragraph. 
    /// Such a math zone can be either inline math zone or math paragraph.
    /// </summary>
    internal class DmlTextMath : DmlParagraphTextElementBase
    {
        public DmlTextMath(MathObject mathObject)
            : this(mathObject, new DmlRunProperties())
        {
        }

        public DmlTextMath(MathObject mathObject, DmlRunProperties runPr)
        {
            RunProperties = runPr;
            mMathObject = mathObject;
        }

        public override DmlParagraphTextElementBase Clone()
        {
            DmlTextMath lhs = (DmlTextMath) base.Clone();

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
            mElements.Add(element);
        }

        internal IList<DmlParagraphTextElementBase> Elements
        {
            get { return mElements; }
        }

        internal MathObject MathObject
        {
            get { return mMathObject; }
        }

        /// <summary>
        /// If true indicates that Math is root math element and must be wrapped into 'a14:m' tag.
        /// </summary>
        public bool IsRootElement
        {
            get { return mIsRootElement; }
            set { mIsRootElement = value; }
        }

        public override void SetParagraph(DmlParagraph paragraph)
        {
            base.SetParagraph(paragraph);
            foreach (DmlParagraphTextElementBase element in mElements)
            {
                element.SetParagraph(paragraph);
            }
        }

        private List<DmlParagraphTextElementBase> mElements = new List<DmlParagraphTextElementBase>();
        private readonly MathObject mMathObject;
        private bool mIsRootElement;
    }
}