// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2014 by Alexey Morozov

using System;
using System.IO;

namespace Aspose.Words.RW.Ole.Moniker
{
    /// <summary>
    /// Implements [MS-OLEDS] 2.3.3.1 MONIKERSTREAM structure. http://msdn.microsoft.com/en-us/library/dd942007.aspx
    /// </summary>
    internal class MonikerStream
    {
        internal static MonikerBase Read(BinaryReader reader, int length)
        {
            if (length == 0)
                return null;

            int startPos = (int)reader.BaseStream.Position;

            Guid clsId = new Guid(reader.ReadBytes(16));
            MonikerBase moniker = MonikerBase.Create(clsId);
            moniker.Read(reader);

            // Adjust length because it occurred that moniker can have some extra undocumented data, FileMoniker for example.
            reader.BaseStream.Position = startPos + length - 4;

            return moniker;
        }

        internal static void Write(BinaryWriter writer, MonikerBase moniker)
        {
            MemoryStream tempStream = new MemoryStream();
            BinaryWriter tempWriter = new BinaryWriter(tempStream);

            moniker.Write(tempWriter);

            int monikerLength = (int)tempStream.Length;

            // MonikerSize
            writer.Write(monikerLength + 16 /* ClsId */+ 4 /* monikerLength size */);
            byte[] monikerBytes = tempStream.ToArray();
            writer.Write(moniker.ClsId.ToByteArray());
            writer.Write(monikerBytes, 0, monikerBytes.Length);
        }
    }
}
