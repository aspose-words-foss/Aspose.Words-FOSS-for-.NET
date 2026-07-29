// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System.Drawing;
using Aspose.Words.Drawing.Core.Dml.Transforms;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 20.1.2.2.41 txSp (Text Shape)
    /// This element specifies the existence of a text shape within a parent shape. 
    /// This text shape is specifically used for displaying text as it has only text related child elements.
    /// </summary>
    internal class DmlTextShape
    {
        internal DmlTextShape Clone()
        {
            DmlTextShape lhs = (DmlTextShape)MemberwiseClone();

            if (mTransform != null)
                lhs.mTransform = mTransform.Clone();

            if (mTextBody != null)
                lhs.mTextBody = mTextBody.Clone();

            return lhs;
        }

        internal DmlTransform Transform
        {
            get
            {
                if (mTransform == null)
                    mTransform = new DmlTransform();
                return mTransform;
            }
            set { mTransform = value; }
        }

        /// <summary>
        /// 20.1.2.2.42 useSpRect (Use Shape Text Rectangle)
        /// This element specifies that the text rectangle from the parent shape should be used for 
        /// this text shape. If this attribute is specified then the text rectangle, or text bounding 
        /// box as it is also called should have the same dimensions as the text bounding box of the 
        /// parent shape within which this text shape resides.
        /// </summary>
        internal bool UseShapeTextRectangle
        {
            get { return mUseShapeTextRectangle; }
            set { mUseShapeTextRectangle = value; }
        }

        internal DmlTextBody TextBody
        {
            get
            {
                if (mTextBody == null)
                    mTextBody = new DmlTextBody();
                return mTextBody;
            }
            set { mTextBody = value; }
        }

        /// <summary>
        /// Returns the text shape bounding size without borders and effects.
        /// </summary>
        internal RectangleF Bounds
        {
            get { return mBounds; }
            set { mBounds = value; }
        }

        private RectangleF mBounds = RectangleF.Empty;
        private DmlTextBody mTextBody;
        private DmlTransform mTransform;
        private bool mUseShapeTextRectangle;
    }
}
