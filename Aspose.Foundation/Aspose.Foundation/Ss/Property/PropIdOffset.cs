// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.IO;

namespace Aspose.Ss.Property
{
    /// <summary>
    /// Represents information about property id and its offset in a property set section.
    /// </summary>
    internal class PropIdOffset
    {
        internal PropIdOffset(int id, int offset)
        {
            Id = id;
            Offset = offset;
        }

        internal PropIdOffset(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Offset = reader.ReadInt32();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Offset);
        }

        internal int Id;
        internal int Offset;
    }
}
