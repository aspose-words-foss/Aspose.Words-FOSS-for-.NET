// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    internal class DmlAdjustHandleXY : DmlAdjustHandle
    {
        public override DmlAdjustType GetAdjustType(string adjustName)
        {
            if (CompareAdjustNames(adjustName, GdRefX))
                return DmlAdjustType.Coordinate;

            if (CompareAdjustNames(adjustName, GdRefY))
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

        public string GdRefX
        {
            get { return mGdRefX; }
            set { mGdRefX = value; }
        }

        public string GdRefY
        {
            get { return mGdRefY; }
            set { mGdRefY = value; }
        }

        private string mGdRefX;
        private string mGdRefY;
    }
}
