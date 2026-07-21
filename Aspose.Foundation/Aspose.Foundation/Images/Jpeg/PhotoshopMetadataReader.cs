// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2022 by Dmitry Burov

using System.Drawing;
using Aspose.IO;

namespace Aspose.Images.Jpeg
{
    internal static class PhotoshopMetadataReader
    {
        internal static PointF ReadResolution(BigEndianBinaryReader reader, PointF resolution)
        {
            bool bimMarkerFound = SearchFor8BimMarker(reader);
            if (!bimMarkerFound)
                return resolution;

            // Check the image resource ID
            uint imageResourceId = reader.ReadUInt16();
            if (imageResourceId != 0x03ED) //  ResolutionInfo structure. See Appendix A in Photoshop API Guide.pdf.
                return resolution;

            // Read ResolutionInfo structure.

            if (!SkipPascalString(reader))
                return resolution;

            // Read the actual size of resource data that follows.
            uint resourceDataSize = reader.ReadUInt32();

            // Read the resolution structure.

            // X resolution fixed point format.
            int xRresolutionValue = reader.ReadInt16();
            int xRresolutionFraction = reader.ReadInt16();
            float xResolution = GetFloatFromFixed(xRresolutionValue, xRresolutionFraction);

            // Skip horizontal resolution display unit.
            reader.ReadInt16();

            // Skip width display unit.
            reader.ReadInt16();

            // Y resolution fixed point format.
            int yRresolutionValue = reader.ReadInt16();
            int yRresolutionFraction = reader.ReadInt16();
            float yResolution = GetFloatFromFixed(yRresolutionValue, yRresolutionFraction);

            // Skip vertical resolution display unit.
            reader.ReadInt16();

            // Skip height display unit.
            reader.ReadInt16();

            return new PointF(xResolution, yResolution);
        }

        private static float GetFloatFromFixed(int value, int fraction)
        {
            return value + (float)fraction / 65536;
        }

        private static bool SkipPascalString(BigEndianBinaryReader reader)
        {
            // Skip the first byte of the string.
            reader.ReadByte();

            // Skip the pascal string.            
            while (!reader.IsEndOfStream)
            {
                if (reader.ReadByte() == 0)
                    return true;
            }

            // Seems like an invalid structure.
            return false;
        }

        private static bool SearchFor8BimMarker(BigEndianBinaryReader reader)
        {
            // "8BIM" marker as 32-bit integer.
            const int BimMarker = 0x3842494d;
            QuadByteQueue byteQueue = new QuadByteQueue();
            bool bimMarkerFound = false;

            while (!reader.IsEndOfStream)
            {
                byte b = reader.ReadByte();
                byteQueue.Enqueue(b);

                // Check for the end of "8BIM" marker, i.e. 'M'. first
                // and then check the full "8BIM" marker.
                if ((b == 0x4d) && (byteQueue.IntValue == BimMarker))
                {
                    bimMarkerFound = true;
                    break;
                }
            }

            return bimMarkerFound;
        }
    }
}
