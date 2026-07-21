// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2012 by Alexey Morozov

using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.Ss;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements reading of OCXNAME stream which contains name of ActiveX control.
    /// </summary>
    internal class OcxNameStream : Ole2StreamBase
    {
        internal static OcxNameStream Read(MemoryStorage storage)
        {
            return storage.ContainsKey(OcxNameStreamName) ? new OcxNameStream(storage.GetStreamZeroPositioned(OcxNameStreamName)) : null;
        }

        internal OcxNameStream(string ocxName)
        {
            mValue = ocxName;
        }

        private OcxNameStream(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

            StringBuilder sb = new StringBuilder();
            while (StreamUtil.HasEnoughBytesToRead(reader, 2))
            {
                char ch = reader.ReadChar();

                if (ch == '\0')
                    break;

                sb.Append(ch);
            }

            mValue = sb.ToString();
        }

        protected override void Write(BinaryWriter writer)
        {
            if (StringUtil.HasChars(mValue))
            {
                for (int i = 0; i < mValue.Length; i++)
                {
                    char ch = mValue[i];
                    writer.Write((byte) ch);
                    writer.Write((byte) 0);
                }
            }
            writer.Write(0x00);
        }

        protected override string Name
        {
            get { return OcxNameStreamName; }
        }

        internal string Value
        {
            get { return mValue; }
        }

        private readonly string mValue;
    }
}
