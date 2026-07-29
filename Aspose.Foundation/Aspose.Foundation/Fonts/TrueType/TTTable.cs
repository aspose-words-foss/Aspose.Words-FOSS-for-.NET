// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2009 by Roman Korchagin

using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts.TrueType
{
    public abstract class TTTable
    {
        /// <summary>
        /// All concrete classes need to implement this.
        /// </summary>
        [JavaThrows(true)]
        internal abstract void Write(BigEndianBinaryWriter writer);

        /// <summary>
        /// Writes this table to a new memory stream.
        /// </summary>
        internal MemoryStream ToMemoryStream()
        {
            MemoryStream stream = new MemoryStream();
            BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);
            Write(writer);
            return stream;
        }

        /// <summary>
        /// Writes this table to a byte array.
        /// </summary>
        internal byte[] ToByteArray()
        {
            using (MemoryStream stream = ToMemoryStream())
            {
                return stream.ToArray();
            }
        }
    }
}
