// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2019 by Andrey Noskov

using Aspose.Images;
using Aspose.Words.Drawing;

namespace Aspose.Words.Drawing
{
    internal class ExifOrientationUtils
    {
        internal static double GetRotationAngle(ExifOrientation exifOrientation)
        {
            double rotationAng = 0;
            switch (exifOrientation)
            {
                case ExifOrientation.Rotate270:
                // Word saves this orientation as mirrored vertical and turned to 270 degrees.
                case ExifOrientation.MirrorHorizontalAndRotate90:
                    rotationAng = 270;
                    break;
                case ExifOrientation.Rotate90:
                // Word saves this orientation as mirrored vertical and turned to 90 degrees.
                case ExifOrientation.MirrorHorizontalAndRotate270:
                    rotationAng = 90;
                    break;
                case ExifOrientation.Rotate:
                    rotationAng = 180;
                    break;
            }

            return rotationAng;
        }

        internal static FlipOrientation GetFlipOrientation(ExifOrientation exifOrientation)
        {
            FlipOrientation flip = FlipOrientation.None;
            switch (exifOrientation)
            {
                case ExifOrientation.Mirror:
                case ExifOrientation.MirrorHorizontalAndRotate90:
                case ExifOrientation.MirrorHorizontalAndRotate270:
                    flip = FlipOrientation.Vertical;
                    break;
                case ExifOrientation.MirrorHorizontal:
                    flip = FlipOrientation.Horizontal;
                    break;
            }

            return flip;
        }
    }
}
