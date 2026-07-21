// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Styles
{
    /// <summary>
    /// 20.1.2.2.37 style (Shape Style)
    /// This element specifies the style information for a shape.
    /// </summary>
    internal class DmlShapeStyle
    {
        /// <summary>
        /// Creates the effects style.
        /// </summary>
        internal EffectStyle GetEffectStyle(IThemeProvider theme)
        {
            return EffectReference.GetEffectStyle(theme);
        }

        internal DmlShapeStyle Clone()
        {
            if (IsEmpty)
                return new DmlShapeStyle();

            DmlShapeStyle lhs = (DmlShapeStyle)MemberwiseClone();
            if (mEffectReference != null)
                lhs.EffectReference = (DmlEffectReference)mEffectReference.Clone();
            if (mFillReference != null)
                lhs.FillReference = (DmlFillReference)mFillReference.Clone();
            if (mFontReference != null)
                lhs.mFontReference = (DmlFontReference)mFontReference.Clone();
            if (mLineReference != null)
                lhs.LineReference = (DmlLineReference)mLineReference.Clone();
            return lhs;
        }

        internal DmlEffectReference EffectReference
        {
            get
            {
                if (mEffectReference == null)
                    mEffectReference = new DmlEffectReference();
                return mEffectReference;
            }
            set { mEffectReference = value; }
        }

        internal DmlFillReference FillReference
        {
            get
            {
                if (mFillReference == null)
                    mFillReference = new DmlFillReference();
                return mFillReference;
            }
            set { mFillReference = value; }
        }

        internal DmlFontReference FontReference
        {
            get
            {
                if (mFontReference == null)
                    mFontReference = new DmlFontReference();
                return mFontReference;
            }
            set { mFontReference = value; }
        }

        internal DmlLineReference LineReference
        {
            get
            {
                if (mLineReference == null)
                    mLineReference = new DmlLineReference();
                return mLineReference;
            }
            set { mLineReference = value; }
        }

        internal bool IsEmpty
        {
            get { return (((mEffectReference == null) || mEffectReference.IsEmpty) &&
                    ((mFillReference == null) || mFillReference.IsEmpty) &&
                    ((mFontReference == null) || mFontReference.IsEmpty) &&
                    ((mLineReference == null) || mLineReference.IsEmpty));
            }
        }

        private DmlEffectReference mEffectReference;
        private DmlFillReference mFillReference;
        private DmlFontReference mFontReference;
        private DmlLineReference mLineReference;
    }
}
