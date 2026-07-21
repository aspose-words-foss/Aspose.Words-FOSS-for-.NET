// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2010 by Dmitry Burov

namespace Aspose.Words.Drawing.Core
{
    internal class HandlePoint
    {
        /// <summary>
        /// Describes the positioning of adjust handle.
        /// </summary>
        internal HandlePoint(HandlePositionValue x, HandlePositionValue y)
        {
            mX = x;
            mY = y;
        }

        /// <summary>
        /// The X value of the adjust handle position.
        /// </summary>
        internal HandlePositionValue X
        {
            set { mX = value; }
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mX; }
        }

        /// <summary>
        /// The Y value of the adjust handle position.
        /// </summary>
        internal HandlePositionValue Y
        {
            set { mY = value; }
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mY; }
        }

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}", X.ToString(), Y.ToString());
        }

        private HandlePositionValue mY;
        private HandlePositionValue mX;
    }
}
