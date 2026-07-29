// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents composite record in glyf table in OpenType file.
    /// </summary>
    internal class CompositeGlyfTableRecord : GlyfTableRecord
    {
        public CompositeGlyfTableRecord()
        {
            Components = new List<ComponentGlyphRecord>();
        }

        /// <summary>
        /// Reads composite record from binary reader.
        /// </summary>
        public static CompositeGlyfTableRecord ReadCompositeRecord(BigEndianBinaryReader reader)
        {
            CompositeGlyfTableRecord result = new CompositeGlyfTableRecord();
            result.ReadHeader(reader);
            result.ReadComponents(reader);

            if (result.WeHaveInstructions)
            {
                ushort numInstructions = reader.ReadUInt16();
                result.Instructions = reader.ReadBytes(numInstructions);
            }

            return result;
        }

        public void ReadComponents(BigEndianBinaryReader reader)
        {
            while (true)
            {
                ComponentGlyphRecord component = ComponentGlyphRecord.Read(reader);
                Components.Add(component);
                if (!component.MoreComponents)
                    break;
            }
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            WriteNumContoursAndBoundingBox(writer);

            foreach (ComponentGlyphRecord component in Components)
                component.Write(writer);

            if (WeHaveInstructions)
            {
                writer.WriteUInt16(Instructions.Length);
                writer.WriteBytes(Instructions, 0, Instructions.Length);
            }
        }

        public byte[] BuildComponentsData()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);
                foreach (ComponentGlyphRecord component in Components)
                    component.Write(writer);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// TrueType instructions.
        /// </summary>
        public byte[] Instructions { get; set; }

        /// <summary>
        /// True when WeHaveInstructions flag is set.
        /// </summary>
        public bool WeHaveInstructions
        {
            get { return Components.Count > 0 && Components[Components.Count - 1].WeHaveInstructions; }
        }

        public List<ComponentGlyphRecord> Components { get; }
    }
}
