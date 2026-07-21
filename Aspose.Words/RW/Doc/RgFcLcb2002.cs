// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2012 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// Implements [MS-DOC] 2.5.8 FibRgFcLcb2002 structure.
    /// </summary>
    internal class RgFcLcb2002
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Fc/Lcb to some PLCF that specifies how nested tables are structured. 
        /// SPEC: fcUnused1 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private readonly FcLcb Unused2 = new FcLcb();       // 0x3fa

        /// <summary>
        /// Fc/Lcb in the Table Stream of PGPArray which describes the border and margin properties 
        /// that can be applied to a contiguous range of paragraphs.
        /// </summary>
        internal FcLcb PlcfPgp = new FcLcb();       // 0x402

        /// <summary>
        /// Fc/Lcb in the Table Stream of the Plcfuim which contains data that was provided 
        /// by the Microsoft Windows Text Services Framework, a service provided by Microsoft Windows that 
        /// enables the application to receive input from different input sources, such as handwriting.
        /// </summary>
        internal readonly FcLcb Plcfuim = new FcLcb();          // 0x40a

        /// <summary>
        /// Fc/Lcb in the Table Stream of the PlfguidUim.
        /// </summary>
        internal readonly FcLcb PlfguidUim = new FcLcb();       // 0x412

        /// <summary>
        /// Fc/Lcb in the Table Stream of ATRDPost10 structures.
        /// </summary>
        internal FcLcb AtrdExtra = new FcLcb();       // 0x41a

        /// <summary>
        /// Fc/Lcb in the Table Stream of a PLRSID which is array of revision-save identifiers.
        /// </summary>
        internal FcLcb Plrsid = new FcLcb();           // 0x422

        /// <summary>
        /// Fc/Lcb of smart tag bookmark STTB.
        /// The SttbfBkmkFactoid is parallel to the PlcfBkfd at offset fcPlcfBkfFactoid.
        /// </summary>
        internal FcLcb SttbfBkmkFactoid = new FcLcb(); // 0x42a

        /// <summary>
        /// Fc/Lcb of smart tag bookmark Plc of cpFirsts. PLCF of smart tag starts.
        /// </summary>
        internal FcLcb PlcfBkfFactoid = new FcLcb();   // 0x432

        /// <summary>
        /// Fc/Lcb of a PlcfcookieOffset which contains information about a grammar checker cookie. 
        /// The grammar checker cookie itself is contained within the data that corresponds to the fcCookieData member of FibRgFcLcb97.
        /// </summary>
        internal readonly FcLcb Plcfcookie = new FcLcb();           // 0x43a

        /// <summary>
        /// Fc/Lcb of smart tag bookmark Plc of cpLims. PLCF of smart tag ends.
        /// </summary>
        internal FcLcb PlcfBklFactoid = new FcLcb();      // 0x442

        /// <summary>
        /// Fc/Lcb of smart tag data.
        /// </summary>
        internal FcLcb FactoidData = new FcLcb();         // 0x44a

        /// <summary>
        /// Fc/Lcb in the WordDocument Stream of version-specific undo information.
        /// </summary>
        internal readonly FcLcb DocUndo = new FcLcb();          // 0x452

        /// <summary>
        /// Fc/Lcb of SttbfBkmkFcc that contains information about the format consistency-checker bookmarks.
        /// </summary>
        internal readonly FcLcb SttbfBkmkFcc = new FcLcb();      // 0x45a

        /// <summary>
        /// Fc/Lcb of PlcfBkfFcc that contains information about the format consistency-checker bookmarks.
        /// </summary>
        internal readonly FcLcb PlcfBkfFcc = new FcLcb();        // 0x462

        /// <summary>
        /// Fc/Lcb of PlcfBklFcc that contains information about the format consistency-checker bookmarks.
        /// </summary>
        private readonly FcLcb PlcfBklFcc = new FcLcb();        // 0x46a

        /// <summary>
        /// Fc/Lcb of an SttbfBkmkBPRepairs that contains information about the repair bookmarks.
        /// </summary>
        internal readonly FcLcb SttbfbkmkBPRepairs = new FcLcb();   // 0x472

        /// <summary>
        /// Fc/Lcb of an PlcfBkf that contains information about the repair bookmarks.
        /// </summary>
        internal readonly FcLcb PlcfbkfBPRepairs = new FcLcb();     // 0x47a

        /// <summary>
        /// Fc/Lcb of an PlcfBkl that contains information about the repair bookmarks.
        /// </summary>
        internal readonly FcLcb PlcfbklBPRepairs = new FcLcb();  // 0x482

        /// <summary>
        /// Fc/Lcb of new Pms, which contains the current state of a print merge operation.
        /// </summary>
        internal FcLcb PmsNew = new FcLcb();            // 0x48a

        /// <summary>
        /// Fc/Lcb of Office Data Source Object (ODSO) data used to perform mail merge begins at this offset.
        /// </summary>
        internal FcLcb ODSO = new FcLcb();              // 0x492

        /// <summary>
        /// Fc/Lcb of the deprecated paragraph mark information cache used by Word2002.
        /// Information SHOULD NOT be emitted at this offset and SHOULD be ignored.
        /// </summary>
        internal readonly FcLcb PlcfpmiOldXP = new FcLcb();      // 0x49a

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated paragraph mark information cache used by Word 2002.
        /// Information SHOULD NOT be emitted at this offset and SHOULD be ignored.
        /// </summary>
        internal readonly FcLcb PlcfpmiNewXP = new FcLcb();     // 0x4a2

        /// <summary>
        /// Fc/Lcb in the Table Stream of the deprecated paragraph mark information cache used by Word 2002.
        /// Information SHOULD NOT be emitted at this offset and SHOULD be ignored.
        /// </summary>
        internal readonly FcLcb PlcfpmiMixedXP = new FcLcb();   // 0x4aa

        /// <summary>
        /// Fc/Lcb in Table Stream of encryption properties.
        /// SPEC: fcUnused2 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        internal readonly FcLcb EncryptedProps = new FcLcb();    // 0x4b2

        /// <summary>
        /// Fc/Lcb in the Table Stream of a Plcffactoid, which specifies the smart tag recognizer state of each text range.
        /// </summary>
        internal readonly FcLcb Plcffactoid = new FcLcb();       // 0x4ba

        /// <summary>
        /// Fc/Lcb in Table Stream of LVC PLC (Old View) for Word 2002.
        /// This is an internal information cache used by Word.
        /// </summary>
        internal readonly FcLcb PlcflvcOldXP = new FcLcb();      // 0x4c2

        /// <summary>
        /// Fc/Lcb in Table Stream of LVC PLC (New View) for Word 2002.
        /// This is an internal information cache used by Word.
        /// </summary>
        internal readonly FcLcb PlcflvcNewXP = new FcLcb();      // 0x4ca

        /// <summary>
        /// Fc/Lcb in Table Stream of LVC PLC (Mixed View) for Word 2002.
        /// This is an internal information cache used by Word.
        /// </summary>
        internal readonly FcLcb PlcflvcMixedXP = new FcLcb();    // 0x4d2

        // ReSharper enable InconsistentNaming

        /// <summary>
        /// Indicates that document has grammar checker cookies.
        /// </summary>
        internal bool HasGrammarCheckerCookies
        {
            get { return Plcfcookie.Lcb > 0; }
        }

        /// <summary>
        /// Indicates that document has Microsoft Windows Text Services Framework data.
        /// </summary>
        internal bool HasTextFrameworkData
        {
            get { return (PlfguidUim.Lcb > 0) || (Plcfuim.Lcb > 0); }
        }

        /// <summary>
        /// Indicates that document has smart tag recognizer state data.
        /// </summary>
        internal bool HasSmartTagRecognizerData
        {
            get { return Plcffactoid.Lcb > 0; }
        }

        /// <summary>
        /// Indicates that document has repair bookmarks.
        /// </summary>
        internal bool HasRepairBookmarks
        {
            get { return (SttbfbkmkBPRepairs.Lcb > 0) || (PlcfbkfBPRepairs.Lcb > 0) || (PlcfbklBPRepairs.Lcb > 0); }
        }

        internal bool HasConsistencyCheckerBookmarks
        {
            get { return (SttbfBkmkFcc.Lcb > 0) || (PlcfBkfFcc.Lcb > 0) || (PlcfBklFcc.Lcb > 0); }
        }

        internal void Read(BinaryReader reader)
        {
            // 108
            Unused2.Read(reader);           // 0x3fa
            PlcfPgp.Read(reader);           // 0x402

            // RK According to the spec, end of Word 2000 data is here.

            Plcfuim.Read(reader);           // 0x40a
            // 110
            PlfguidUim.Read(reader);        // 0x412
            AtrdExtra.Read(reader);         // 0x41a
            Plrsid.Read(reader);            // 0x422
            SttbfBkmkFactoid.Read(reader);  // 0x42a
            PlcfBkfFactoid.Read(reader);    // 0x432
            // 115
            Plcfcookie.Read(reader);        // 0x43a
            PlcfBklFactoid.Read(reader);    // 0x442
            FactoidData.Read(reader);       // 0x44a
            DocUndo.Read(reader);           // 0x452
            SttbfBkmkFcc.Read(reader);      // 0x45a
            // 120
            PlcfBkfFcc.Read(reader);        // 0x462
            PlcfBklFcc.Read(reader);        // 0x46a
            SttbfbkmkBPRepairs.Read(reader);// 0x472
            PlcfbkfBPRepairs.Read(reader);  // 0x47a
            PlcfbklBPRepairs.Read(reader);  // 0x482
            // 125
            PmsNew.Read(reader);            // 0x48a
            ODSO.Read(reader);              // 0x492
            PlcfpmiOldXP.Read(reader);      // 0x49a
            PlcfpmiNewXP.Read(reader);      // 0x4a2
            PlcfpmiMixedXP.Read(reader);    // 0x4aa
            // 130
            EncryptedProps.Read(reader);    // 0x4b2
            Plcffactoid.Read(reader);       // 0x4ba
            PlcflvcOldXP.Read(reader);      // 0x4c2
            PlcflvcNewXP.Read(reader);      // 0x4ca
            PlcflvcMixedXP.Read(reader);    // 0x4d2

        }

        internal void Write(BinaryWriter writer)
        {
            // 108
            Unused2.Write(writer);     // 0x3fa
            PlcfPgp.Write(writer);     // 0x402

            // RK According to the spec, end of Word 2000 data is here.

            Plcfuim.Write(writer);       // 0x40a
            // 110
            PlfguidUim.Write(writer);    // 0x412
            AtrdExtra.Write(writer);     // 0x41a
            Plrsid.Write(writer);        // 0x422
            SttbfBkmkFactoid.Write(writer);  // 0x42a
            PlcfBkfFactoid.Write(writer);    // 0x432
            // 115
            Plcfcookie.Write(writer);        // 0x43a
            PlcfBklFactoid.Write(writer);    // 0x442
            FactoidData.Write(writer);       // 0x44a
            DocUndo.Write(writer);          // 0x452
            SttbfBkmkFcc.Write(writer);      // 0x45a
            // 120
            PlcfBkfFcc.Write(writer);        // 0x462
            PlcfBklFcc.Write(writer);        // 0x46a
            SttbfbkmkBPRepairs.Write(writer);// 0x472
            PlcfbkfBPRepairs.Write(writer);  // 0x47a
            PlcfbklBPRepairs.Write(writer);  // 0x482
            // 125
            PmsNew.Write(writer);            // 0x48a
            ODSO.Write(writer);              // 0x492
            PlcfpmiOldXP.Write(writer);      // 0x49a
            PlcfpmiNewXP.Write(writer);      // 0x4a2
            PlcfpmiMixedXP.Write(writer);    // 0x4aa
            // 130
            EncryptedProps.Write(writer);    // 0x4b2
            Plcffactoid.Write(writer);       // 0x4ba
            PlcflvcOldXP.Write(writer);      // 0x4c2
            PlcflvcNewXP.Write(writer);      // 0x4ca
            PlcflvcMixedXP.Write(writer);    // 0x4d2
        }
    }
}
