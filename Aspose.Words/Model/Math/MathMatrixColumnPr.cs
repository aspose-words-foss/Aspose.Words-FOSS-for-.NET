// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/03/2011 by Denis Darkin

using Aspose.Words.Drawing;

namespace Aspose.Words.Math
{
    internal class MathMatrixColumnPr
    {
        internal MathMatrixColumnPr Clone()
        {
            return (MathMatrixColumnPr)MemberwiseClone();
        }
        
        /// <summary>
        /// Specifies <see cref="Drawing.HorizontalAlignment"/> of a matrix column within <see cref="MathObjectMatrix"/>.
        /// This property does not allow whole range of <see cref="Drawing.HorizontalAlignment"/> enum values.
        /// Instead it is limited to:
        /// <see cref="Drawing.HorizontalAlignment.Left"/>
        /// <see cref="Drawing.HorizontalAlignment.Right"/>
        /// <see cref="Drawing.HorizontalAlignment.Center"/>, being default value.
        /// </summary>
        internal HorizontalAlignment HorizontalAlignment
        {
            get { return mXAlign; }
    
            set
            {
                switch (value)
                {
                    case HorizontalAlignment.Left:
                    case HorizontalAlignment.Right:
                    case HorizontalAlignment.Center:
                        mXAlign = value;
                        break;
                    default:
                        mXAlign = HorizontalAlignment.Center;
                        break;
                }
            }
        }
        
        private HorizontalAlignment mXAlign = HorizontalAlignment.Center;
    }
}
