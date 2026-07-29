// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/05/2012 by Alexey Morozov

using System;
using System.IO;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// FC/LCB pair value.
    /// </summary>
    internal class FcLcb
    {
        internal FcLcb()
        {
            Fc = 0;
            Lcb = 0;
        }

        internal FcLcb(int fc, int lcb)
        {
            Fc = fc;
            Lcb = lcb;
        }

        internal void Read(BinaryReader reader)
        {
            Fc = reader.ReadInt32();
            Lcb = reader.ReadInt32();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(Fc);
            writer.Write(Lcb);
        }

        public override string ToString()
        {
            return (((Fc == 0) && (Lcb == 0)) ? "-" : string.Format("{0}, {1}", Fc, Lcb));
        }

        internal Int32 Fc;
        internal Int32 Lcb;
    }

}
