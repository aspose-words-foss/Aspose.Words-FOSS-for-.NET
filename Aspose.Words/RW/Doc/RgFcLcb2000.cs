// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2012 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// Represents 2.5.7 FibRgFcLcb2000 structure.
    /// </summary>
    internal class RgFcLcb2000
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// F/Lcb in the Table Stream of a PlcfTch which specifies a cache of table characters.
        /// </summary>
        internal readonly FcLcb PlcfTch = new FcLcb();           // 0x382

        /// <summary>
        /// Fc/Lcb in the Table Stream of RmdThreading that specifies the data concerning the e-mail messages and their authors.
        /// </summary>
        internal readonly FcLcb RmdfThreading = new FcLcb();    // 0x38a

        // 95

        /// <summary>
        /// Fc/Lcb in the Table Stream of a double-byte character Unicode string that specifies the message identifier.
        /// This value MUST be ignored.
        /// </summary>
        internal readonly FcLcb Mid = new FcLcb();               // 0x392

        /// <summary>
        /// Fc/Lcb in the Table Stream of a SttbRgtplc that specifies the bullet/numbering formats for a hybrid bulleted/numbered multi-level list.
        /// </summary>
        internal readonly FcLcb SttbRgtplc = new FcLcb();        // 0x39a

        /// <summary>
        /// Fc/Lcb in the Table Stream of an MsoEnvelopeCLSID, which specifies the envelope data as specified by [MS-OSHARED] section 2.3.8.1.
        /// </summary>
        internal readonly FcLcb MsoEnvelope = new FcLcb();       // 0x3a2

        /// <summary>
        /// Fc/Lcb in the Table Stream of a Plcflad that specifies the language auto-detect state of each text range.
        /// </summary>
        internal readonly FcLcb PlcfLad = new FcLcb();           // 0x3aa

        /// <summary>
        /// Fc/Lcb in the Table Stream of a variable-length array with elements of type Dofrh which are records that support the frame set and list style features.
        /// </summary>
        internal FcLcb RgDofr = new FcLcb();                    // 0x3b2

        /// <summary>
        /// Fc/Lcb in the Table Stream of a PlfCosl which specifies the option set to use for a grammar checker.
        /// </summary>
        internal readonly FcLcb Plcosl = new FcLcb();            // 0x3ba

        /// <summary>
        /// Fc/Lcb in the Table Stream of a PlcfcookieOld.
        /// </summary>
        /// <remarks>
        /// Word 2002, Office Word 2003, Office Word 2007, Word 2010, and Word 15 Technical Preview ignore this value.
        /// </remarks>
        internal readonly FcLcb PlcfCookieOld = new FcLcb();     // 0x3c2

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated document page layout cache.
        /// </summary>
        internal readonly FcLcb PgdMotherOld = new FcLcb();      // 0x3ca

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated document text flow break cache.
        /// </summary>
        internal readonly FcLcb BkdMotherOld = new FcLcb();      // 0x3d2

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated footnote layout cache.
        /// </summary>
        internal readonly FcLcb PgdFtnOld = new FcLcb();         // 0x3da

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated footnote text flow break cache.
        /// </summary>
        internal readonly FcLcb BkdFtnOld = new FcLcb();         // 0x3e2

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated endnote layout cache.
        /// </summary>
        internal readonly FcLcb PgdEdnOld = new FcLcb();         // 0x3ea

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated endnote text flow break cache.
        /// </summary>
        internal readonly FcLcb BkdEdnOld = new FcLcb();         // 0x3f2

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Indicates that document has information concerning the e-mail messages and their authors.
        /// </summary>
        internal bool HasThreadingData
        {
            get { return RmdfThreading.Lcb > 0; }
        }

        /// <summary>
        /// Indicates that document has bullet/numbering formats for a hybrid bulleted/numbered multi-level list
        /// </summary>
        internal bool HasHybridListFormats
        {
            get { return SttbRgtplc.Lcb > 0; }
        }

        /// <summary>
        /// Indicates that document has MSO Envelope information.
        /// </summary>
        internal bool HasEnvelopeData
        {
            get { return MsoEnvelope.Lcb > 0; }
        }

        internal bool HasLanguageAutoDetectData
        {
            get { return PlcfLad.Lcb > 0; }
        }

        internal bool HasGrammarCheckerOptions
        {
            get { return Plcosl.Lcb > 0; }
        }

        internal void Read(BinaryReader reader)
        {
            PlcfTch.Read(reader);
            RmdfThreading.Read(reader);
            // 95
            Mid.Read(reader);
            SttbRgtplc.Read(reader);
            MsoEnvelope.Read(reader);
            PlcfLad.Read(reader);
            RgDofr.Read(reader);
            // 100
            Plcosl.Read(reader);
            PlcfCookieOld.Read(reader);
            PgdMotherOld.Read(reader);
            BkdMotherOld.Read(reader);
            PgdFtnOld.Read(reader);
            // 105
            BkdFtnOld.Read(reader);
            PgdEdnOld.Read(reader);
            BkdEdnOld.Read(reader);
        }

        internal void Write(BinaryWriter writer)
        {
            PlcfTch.Write(writer);
            RmdfThreading.Write(writer);
            // 95
            Mid.Write(writer);
            SttbRgtplc.Write(writer);
            MsoEnvelope.Write(writer);
            PlcfLad.Write(writer);
            RgDofr.Write(writer);
            // 100
            Plcosl.Write(writer);
            PlcfCookieOld.Write(writer);
            PgdMotherOld.Write(writer);
            BkdMotherOld.Write(writer);
            PgdFtnOld.Write(writer);
            // 105
            BkdFtnOld.Write(writer);
            PgdEdnOld.Write(writer);
            BkdEdnOld.Write(writer);
        }

        /// <summary>
        /// Sets given Fc for certain RgFcLcb fields.
        /// </summary>
        internal void SetDummyFc(int fc)
        {
            PlcfTch.Fc = fc; 
        }
    }
}
