// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2006 by Roman Korchagin

using System;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// maxp - Maximum Profile 
    /// 
    /// This table establishes the memory requirements for this font. 
    /// </summary>
    internal class MaximumProfile : TTTable
    {
        /// <summary>
        /// 0x00005000 for version 0.5 
        /// 0x00010000 for version 1.0
        /// </summary>
        internal uint Version;
        /// <summary>
        /// The number of glyphs in the font.
        /// </summary>
        internal ushort NumGlyphs;
        /// <summary>
        /// Maximum points in a non-composite glyph.
        /// </summary>
        internal ushort MaxPoints;
        /// <summary>
        /// Maximum contours in a non-composite glyph.
        /// </summary>
        internal ushort MaxContours;
        /// <summary>
        /// Maximum points in a composite glyph.
        /// </summary>
        internal ushort MaxCompositePoints;
        /// <summary>
        /// Maximum contours in a composite glyph.
        /// </summary>
        internal ushort MaxCompositeContours;
        /// <summary>
        /// 1 if instructions do not use the twilight zone (Z0), or 2 if instructions do use Z0; 
        /// should be set to 2 in most cases.
        /// </summary>
        internal ushort MaxZones;
        /// <summary>
        /// Maximum points used in Z0.
        /// </summary>
        internal ushort MaxTwilightPoints;
        /// <summary>
        /// Number of Storage Area locations. 
        /// </summary>
        internal ushort MaxStorage;
        /// <summary>
        /// Number of FDEFs.
        /// </summary>
        internal ushort MaxFunctionDefs;
        /// <summary>
        /// Number of IDEFs.
        /// </summary>
        internal ushort MaxInstructionDefs;
        /// <summary>
        /// Maximum stack depth.
        /// </summary>
        internal ushort MaxStackElements;
        /// <summary>
        /// Maximum byte count for glyph instructions.
        /// </summary>
        internal ushort MaxSizeOfInstructions;
        /// <summary>
        /// Maximum number of components referenced at "top level" for any composite glyph.
        /// </summary>
        internal ushort MaxComponentElements;
        /// <summary>
        /// Maximum levels of recursion; 1 for simple components.
        /// </summary>
        internal ushort MaxComponentDepth;

        /// <summary>
        /// Ctor. Reads the table.
        /// </summary>
        internal MaximumProfile(BigEndianBinaryReader reader)
        {
            Version = reader.ReadUInt32();
            NumGlyphs = reader.ReadUInt16();
            
            if (Version == 0x00005000)
                return;

            if (Version != 0x00010000)
                throw new NotSupportedException ("Unsupported maximum profile version.");

            MaxPoints = reader.ReadUInt16();
            MaxContours = reader.ReadUInt16();
            MaxCompositePoints = reader.ReadUInt16();
            MaxCompositeContours = reader.ReadUInt16();
            
            MaxZones = reader.ReadUInt16();
            MaxTwilightPoints = reader.ReadUInt16();
            MaxStorage = reader.ReadUInt16();
            MaxFunctionDefs = reader.ReadUInt16();
            
            MaxInstructionDefs = reader.ReadUInt16();
            MaxStackElements = reader.ReadUInt16();
            MaxSizeOfInstructions = reader.ReadUInt16();
            MaxComponentElements = reader.ReadUInt16();
            MaxComponentDepth = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the table.
        /// </summary>
        internal override void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt32(Version);    
            writer.WriteInt16(NumGlyphs);

            if (Version == 0x00005000)
                return;

            writer.WriteInt16(MaxPoints);
            writer.WriteInt16(MaxContours);
            writer.WriteInt16(MaxCompositePoints);
            writer.WriteInt16(MaxCompositeContours);
            
            writer.WriteInt16(MaxZones);
            writer.WriteInt16(MaxTwilightPoints);
            writer.WriteInt16(MaxStorage);
            writer.WriteInt16(MaxFunctionDefs);

            writer.WriteInt16(MaxInstructionDefs);
            writer.WriteInt16(MaxStackElements);
            writer.WriteInt16(MaxSizeOfInstructions);
            writer.WriteInt16(MaxComponentElements);
            writer.WriteInt16(MaxComponentDepth);
        }
    }
}
