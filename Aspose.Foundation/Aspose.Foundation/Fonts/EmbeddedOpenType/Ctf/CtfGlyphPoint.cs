// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Represents glyph point in CTF Glyph Table.
    /// </summary>
    internal class CtfGlyphPoint
    {
        /// <summary>
        /// Reads next glyph point from binary reader utilizing previously read flag value.
        /// </summary>
        public static CtfGlyphPoint Read(BigEndianBinaryReader reader, byte flag)
        {
            CtfGlyphPoint point = new CtfGlyphPoint();

            TripletEncoding.ReadTriplet(reader, flag, point); 
  
            return point;
        }

        public short X;
        public short Y;
        public bool IsOnCurve;
    }
}
