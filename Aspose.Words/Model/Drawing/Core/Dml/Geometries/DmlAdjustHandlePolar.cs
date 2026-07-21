// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal class DmlAdjustHandlePolar : DmlAdjustHandle
    {
        public override DmlAdjustType GetAdjustType(string adjustName)
        {
            if (CompareAdjustNames(adjustName, GdRefAng))
                return DmlAdjustType.Angle;

            if (CompareAdjustNames(adjustName, GdRefR))
                return DmlAdjustType.Coordinate;

            return DmlAdjustType.Unknown;
        }

        /// <summary>
        /// Clones this instance of adjust handle.
        /// </summary>
        internal override DmlAdjustHandle Clone()
        {
            return (DmlAdjustHandle)MemberwiseClone();
        }

        public string GdRefAng
        {
            get { return mGdRefAng; }
            set { mGdRefAng = value; }
        }

        public string GdRefR
        {
            get { return mGdRefR; }
            set { mGdRefR = value; }
        }

        private string mGdRefAng;
        private string mGdRefR;
    }
}
