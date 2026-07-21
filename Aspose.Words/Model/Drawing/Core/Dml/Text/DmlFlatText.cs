// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 20.1.5.8 flatTx (No text in 3D scene)
    /// Keep text out of 3D scene entirely.
    /// </summary>
    internal class DmlFlatText
    {
        internal DmlFlatText Clone()
        {
            return (DmlFlatText)MemberwiseClone();
        }

        /// <summary>
        /// Specifies the Z coordinate to be used in positioning the flat text within the 3D scene.
        /// </summary>
        internal double ZCoordinate
        {
            get { return mZCoordinate; }
            set { mZCoordinate = value; }
        }
        
        private double mZCoordinate;
    }
}