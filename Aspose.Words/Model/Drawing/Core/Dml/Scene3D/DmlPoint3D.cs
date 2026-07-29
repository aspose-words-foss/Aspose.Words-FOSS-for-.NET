// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.5.1 anchor (Anchor Point)
    /// This element specifies a point in 3D space.
    /// </summary>
    [CppAllowBoxing]
    [CppAddStructDefaultMethods]
    internal struct DmlPoint3D
    {
        /// <summary>
        /// X-Coordinate in 3D space.
        /// </summary>
        internal double X
        {
            get { return mX; }
            set { mX = value; }
        }

        /// <summary>
        /// Y-Coordinate in 3D space.
        /// </summary>
        internal double Y
        {
            get { return mY; }
            set { mY = value; }
        }

        /// <summary>
        /// Z-Coordinate in 3D space.
        /// </summary>
        internal double Z
        {
            get { return mZ; }
            set { mZ = value; }
        }

        private double mX;
        private double mY;
        private double mZ;
    }
}
