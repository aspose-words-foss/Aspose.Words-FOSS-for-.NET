// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2003 by Roman Korchagin
using System;
using System.IO;
using System.Text;
using Aspose.Words.Nrx;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// Responsible for reading and wring Word FIB structure.
    /// The FIB starts at the beginning of the file.
    /// </summary>
    internal class Fib
    {
        // ReSharper disable InconsistentNaming

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.2 FibBase
        //////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Specifies that this is a Word Binary File. This value MUST be 0xA5EC.
        /// </summary>
        private UInt16 wIdent;              // 0x00

        /// <summary>
        /// Specifies the version number of the file format used. Superseded by FibRgCswNew.nFibNew if it is present.
        /// </summary>
        /// <remarks>
        /// This value SHOULD be 0x00C1.
        ///
        /// Special empty document is installed with Word 97, Word 2000, Word 2002, and Office Word 2003 to allow "Create New Word Document"
        /// from the operating system. This document has an nFib of 0x00C0. In addition the BiDi build of Word 97 differentiates its documents
        /// by saving 0x00C2 as the nFib. In both cases treat them as if they were 0x00C1.
        /// </remarks>
        internal NFibValue nFib;            // 0x02

        /// <summary>
        /// Product version.
        /// SPEC unused (2 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private UInt16 nProduct;            // 0x04

        /// <summary>
        /// Specifies the install language of the application that is producing the document.
        /// </summary>
        /// <remarks>
        /// If nFib is 0x00D9 or greater, then any East Asian install lid or any install lid with a base language of Spanish,
        /// German or French MUST be recorded as lidAmerican. If the nFib is 0x0101 or greater,
        /// then any install lid with a base language of Vietnamese, Thai, or Hindi MUST be recorded as lidAmerican.
        /// </remarks>
        internal Int16 lid;                  // 0x06

        /// <summary>
        /// Specifies the offset in the WordDocument stream of the FIB for the document which contains all the AutoText items.
        /// </summary>
        /// <remarks>
        /// If this value is 0, there are no AutoText items attached. Otherwise the FIB is found at file location pnNext×512.
        /// If fGlsy is 1 or fDot is 0, this value MUST be 0. If pnNext is not 0, each FIB MUST share the same values for FibRgFcLcb97.fcPlcBteChpx,
        /// FibRgFcLcb97.lcbPlcBteChpx, FibRgFcLcb97.fcPlcBtePapx, FibRgFcLcb97.lcbPlcBtePapx, and FibRgLw97.cbMac.
        /// </remarks>
        internal Int16 pnNext;               // 0x08

        /// <summary>
        /// Specifies whether this is a document template.
        /// </summary>
        internal bool fDot;                 // 0x0a.0001

        /// <summary>
        /// Specifies whether this is a document that contains only AutoText items.
        /// </summary>
        /// <remarks>
        /// See FibRgFcLcb97.fcSttbfGlsy, FibRgFcLcb97.fcPlcfGlsy and FibRgFcLcb97.fcSttbGlsyStyle
        /// </remarks>
        internal bool fGlsy;                 // 0x0a.0002

        /// <summary>
        /// Specifies that file is in complex, fast-saved format.
        /// SPEC: Specifies that the last save operation that was performed on this document was an incremental save operation.
        /// </summary>
        internal bool fComplex;             // 0x0a.0004

        /// <summary>
        /// If set file contains 1 or more pictures.
        /// </summary>
        internal bool fHasPic;              // 0x0a.0008

        /// <summary>
        /// 4bit count of times file was quick saved.
        /// </summary>
        /// <remarks>
        /// If nFib is less than 0x00D9, then cQuickSaves specifies the number of consecutive times this document was incrementally saved.
        /// If nFib is 0x00D9 or greater, then cQuickSaves MUST be 0xF.
        /// </remarks>
        internal int cQuickSaves;           // 0x0a.00f0

        /// <summary>
        /// Specifies whether the document is encrypted or obfuscated.
        /// </summary>
        internal bool fEncrypted;           // 0x0a.0100

        /// <summary>
        /// When 0, this fib refers to the table stream named "0Table", when 1, this fib refers to the table stream named "1Table".
        /// Normally, a file will have only one table stream, but under unusual circumstances a file may have table streams with both names.
        /// In that case, this flag must be used to decide which table stream is valid.
        /// </summary>
        internal bool fWhichTblStm;         // 0x0a.0200

        /// <summary>
        /// Specifies whether the document author recommended that the document be opened in read-only mode.
        /// </summary>
        internal bool fReadOnlyRecommended; // 0x0a.0400

        /// <summary>
        /// Specifies whether the document has a write-reservation password.
        /// </summary>
        internal bool fWriteReservation;    // 0x0a.0800

        /// <summary>
        /// If true use extended character set in file. This value MUST be 1.
        /// </summary>
        private bool fExtChar;              // 0x0a.1000

        /// <summary>
        /// Specifies whether to override the language information and font that are specified in the paragraph style at istd 0
        /// (the normal style) with the defaults that are appropriate for the installation language of the application.
        /// </summary>
        private bool fLoadOverride;         // 0x0a.2000

        /// <summary>
        /// Specifies whether the installation language of the application that created the document was an East Asian language.
        /// </summary>
        private bool fFarEast;              // 0x0a.4000

        /// <summary>
        /// If fEncrypted is 1, specifies whether the document is obfuscated using XOR obfuscation.
        /// otherwise this bit MUST be ignored.
        /// </summary>
        internal bool fObfuscation;         // 0x0a.8000

        /// <summary>
        /// This value SHOULD be 0x00BF. This value MUST be 0x00BF or 0x00C1.
        /// </summary>
        internal UInt16 nFibBack;           // 0x0c

        /// <summary>
        /// If fEncryption is 1 and fObfuscation is 1, specifies the XOR obfuscation password verifier.
        /// If fEncryption is 1 and fObfuscation is 0, specifies the size of the EncryptionHeader stored
        /// at the start of the Table stream as described in Encryption and Obfuscation.
        /// Otherwise, MUST be 0.
        /// </summary>
        internal Int32 lKey;                // 0x0e

        /// <summary>
        /// Byte environment in which file was created: 0 created by Win Word / 1 created by Mac Word.
        /// SPEC: This value MUST be 0, and MUST be ignored.
        /// </summary>
        private Byte envr;                  // 0x12

        /// <summary>
        /// If 1 file was last saved in the Mac environment.
        /// SPEC: This value MUST be 0, and MUST be ignored.
        /// </summary>
        private bool fMac;                  // 0x13

        /// <summary>
        /// This value SHOULD be 0 and SHOULD be ignored.
        /// </summary>
        /// <remarks>
        /// Word 97, Word 2000, Word 2002, and Office Word 2003 install a minimal .doc file for use with the New-Microsoft Word Document of the shell.
        /// This minimal .doc file has fEmptySpecial set to 1.
        /// Word uses this flag to identify a document that was created by using the New – Microsoft Word Document of the operating system shell.
        /// </remarks>
        private bool fEmptySpecial;

        /// <summary>
        /// Specifies whether to override the section properties for page size, orientation, and margins
        /// with the defaults that are appropriate for the installation language of the application.
        /// </summary>
        private bool fLoadOverridePage;

        /// <summary>
        /// This value is undefined and MUST be ignored.
        /// </summary>
        private bool fFutureSavedUndo;

        /// <summary>
        /// This value is undefined and MUST be ignored.
        /// </summary>
        private bool fWord97Saved;

        /// <summary>
        /// Default extended character set id for text in document stream. (overridden by chp.chse). 0 = ANSI  / 256 Macintosh character set.
        /// SPEC: reserved3 (2 bytes): This value MUST be 0 and MUST be ignored.
        /// </summary>
        private UInt16 chse;                // 0x14

        /// <summary>
        /// Default extended character set id for text: 0 = ANSI, 256 = Macintosh.
        /// SPEC: reserved4 (2 bytes): This value MUST be 0 and MUST be ignored.
        /// </summary>
        private UInt16 chseTables;          // 0x16

        /// <summary>
        /// File offset of first character of text.
        /// SPEC: reserved5 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        internal Int32 fcMin;               // 0x18

        /// <summary>
        /// File offset of last character of text + 1
        /// SPEC: reserved6 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        internal Int32 fcMac;               // 0x1c

        /// <summary>
        /// Specifies the count of 16-bit values corresponding to fibRgW that follow. MUST be 0x000E.
        /// </summary>
        private UInt16 csw;

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.3 FibRgW97.
        //////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Unique number Identifying the File's creator. 0x6A62 is the creator ID for Word and is reserved.
        /// Other creators should choose a different value.
        /// </summary>
        private UInt16 wMagicCreated;

        /// <summary>
        /// Identifies the File's last modifier.
        /// </summary>
        private UInt16 wMagicRevised;

        /// <summary>
        /// 0x26. Private data.
        /// </summary>
        private UInt16 wMagicCreatedPrivate;

        /// <summary>
        /// 0x28. Private data.
        /// </summary>
        private UInt16 wMagicRevisedPrivate;

        /*
        INT16  pnFbpChpFirst_W6;// not used
        INT16  pnChpFirst_W6;   // not used
        INT16  cpnBteChp_W6;    // not used
        INT16  pnFbpPapFirst_W6;// not used
        INT16  pnPapFirst_W6;   // not used
        INT16  cpnBtePap_W6;    // not used
        INT16  pnFbpLvcFirst_W6;// not used
        INT16  pnLvcFirst_W6;   // not used
        INT16  cpnBteLvc_W6;    // not used
        */

        /// <summary>
        /// A LID whose meaning depends on the nFib value.
        /// </summary>
        /// <remarks>
        /// 0x00C1 - If FibBase.fFarEast is "true", this is the LID of the stored style names. Otherwise it MUST be ignored.
        /// 0x00D9, 0x0101, 0x010C, 0x0112 - The LID of the stored style names (STD.xstzName).
        /// </remarks>
        private Int16  lidFE;

        /// <summary>
        /// Specifies the count of 32-bit values corresponding to fibRgLw that follow. MUST be 0x0016.
        /// </summary>
        private UInt16 cslw;            // 0x3e

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.4 FibRgLw97.
        //////////////////////////////////////////////////////////////////////////////////////

        internal Int32 cbMac;           // 0x40 file offset of last byte written to file + 1.

        /// <summary>
        /// The build date of the creator. 10695 indicates the creator program was compiled on Jan 6, 1995
        /// SPEC: reserved1 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32 lProductCreated;  // 0x44

        /// <summary>
        /// The build date of the File's last modifier.
        /// SPEC: reserved2 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32 lProductRevised;  // 0x48

        /// <summary>
        /// Length of main document text stream.
        /// </summary>
        internal Int32 ccpText;         // 0x4c

        /// <summary>
        /// Length of footnote subdocument text stream.
        /// </summary>
        internal Int32 ccpFtn;          // 0x50

        /// <summary>
        /// length of header subdocument text stream.
        /// </summary>
        internal Int32 ccpHdr;          // 0x54

        /// <summary>
        /// Length of macro subdocument text stream.
        /// SPEC: reserved3 (4 bytes): This value MUST be zero and MUST be ignored.
        /// </summary>
        private Int32 ccpMcr;           // 0x58

        /// <summary>
        /// Length of annotation subdocument text stream.
        /// </summary>
        internal Int32 ccpAtn;          // 0x5c

        /// <summary>
        /// Length of endnote subdocument text stream.
        /// </summary>
        internal Int32 ccpEdn;          // 0x60

        /// <summary>
        /// Length of textbox subdocument text stream.
        /// </summary>
        internal Int32 ccpTxbx;         // 0x64

        /// <summary>
        /// Length of header textbox subdocument text stream.
        /// </summary>
        internal Int32 ccpHdrTxbx;      // 0x68

        /// <summary>
        /// When there was insufficient memory for Word to expand the PLCFbte at save time,
        /// the PLCFbte is written to the file in a linked list of 512-byte pieces starting with this pn.
        /// SPEC: reserved4 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32  pnFbpChpFirst;   // 0x6c

        /// <summary>
        /// The page number of the lowest numbered page in the 0x74 document that records CHPX FKP information.
        /// reserved5 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32  pnChpFirst;      // 0x70

        /// <summary>
        /// Count of CHPX FKPs recorded in file. In non-complex files if the number of entries in the PLCFbteChpx
        /// is less than this, the PLCFbteChpx is incomplete.
        /// </summary>
        /// <remarks>
        /// SPEC: This value MUST be equal or less than the number of data elements in PlcBteChpx, as specified
        /// by FibRgFcLcb97.fcPlcfBteChpx and FibRgFcLcb97.lcbPlcfBteChpx. This value MUST be ignored.
        /// </remarks>
        private Int32  cpnBteChp;       // 0x74

        /// <summary>
        /// When there was insufficient memory for Word to expand the PLCFbte at save time, the PLCFbte is written to
        /// the file in a linked list of 512-byte pieces starting with this pn.
        /// SPEC: reserved7 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32  pnFbpPapFirst;   // 0x78

        /// <summary>
        /// The page number of the lowest numbered page in the document that records PAPX FKP information.
        /// SPEC. reserved8 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32  pnPapFirst;      // 0x7c

        /// <summary>
        /// Count of PAPX FKPs recorded in file. In non-complex files if the number of entries in the PLCFbtePapx is
        /// less than this, the PLCFbtePapx is incomplete.
        /// </summary>
        /// <remarks>
        /// SPEC: reserved9 (4 bytes): This value MUST be less than or equal to the number of data elements
        /// in PlcBtePapx, as specified by FibRgFcLcb97.fcPlcfBtePapx and FibRgFcLcb97.lcbPlcfBtePapx. This value MUST be ignored.
        /// </remarks>
        private Int32  cpnBtePap;       // 0x80

        /// <summary>
        /// When there was insufficient memory for Word to expand the PLCFbte at save time, the PLCFbte is written to
        /// the file in a linked list of 512-byte pieces starting with this pn.
        /// SPEC: reserved10 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32  pnFbpLvcFirst;   // 0x84

        /// <summary>
        /// The page number of the lowest numbered page in the document that records LVC FKP information.
        /// SPEC: reserved11 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private Int32  pnLvcFirst;      // 0x88

        /// <summary>
        /// Count of LVC FKPs recorded in file. In non-complex files if the number of entries in the PLCFbtePapx is
        /// less than this, the PLCFbtePapx is incomplete.
        /// </summary>
        /// <remarks>
        /// SPEC: reserved12 (4 bytes): This value SHOULD be zero, and MUST be ignored. Word 97, Word 2000, Word 2002,
        /// and Office Word 2003 write a nonzero value here when saving a document template with changes
        /// that require the saving of an AutoText document.
        /// </remarks>
        private Int32 cpnBteLvc;        // 0x8C

        /// <summary>
        /// SPEC: reserved13 (4 bytes): This value MUST be zero and MUST be ignored.
        /// </summary>
        private Int32 fcIslandFirst;   // 0x90

        /// <summary>
        /// SPEC: reserved14 (4 bytes): This value MUST be zero and MUST be ignored.
        /// </summary>
        private Int32 fcIslandLim;     // 0x94

        /// <summary>
        /// Specifies the count of 64-bit values corresponding to fibRgFcLcb that follow.
        /// </summary>
        /// <remarks>
        /// This MUST be one of the following values, depending on the value of nFib.
        /// 0x00C1 - 0x005D
        /// 0x00D9 - 0x006C
        /// 0x0101 - 0x0088
        /// 0x010C - 0x00A4
        /// 0x0112 - 0x00B7
        /// </remarks>
        private UInt16 cbRgFcLcb;       // 0x98

        // JAVA: all former structs below are intitialized for java.
        // See https://auckland.dynabic.com/wiki/display/org/Struct about using stucts in AW.

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.5 FibRgFcLcb
        //////////////////////////////////////////////////////////////////////////////////////
        internal RgFcLcb RgFcLcb = new RgFcLcb();           // 93 plcf pairs.

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.7 FibRgFcLcb2000
        //////////////////////////////////////////////////////////////////////////////////////
        internal RgFcLcb2000 RgFcLcb2000 = new RgFcLcb2000();   // 108 plcf pairs.

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.8 FibRgFcLcb2002
        //////////////////////////////////////////////////////////////////////////////////////
        internal RgFcLcb2002 RgFcLcb2002 = new RgFcLcb2002();   // 136 plcf pairs.

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.9 FibRgFcLcb2003
        //////////////////////////////////////////////////////////////////////////////////////
        internal RgFcLcb2003 RgFcLcb2003 = new RgFcLcb2003();   // 164 plcf pairs.

        //////////////////////////////////////////////////////////////////////////////////////
        // 2.5.10 FibRgFcLcb2007
        //////////////////////////////////////////////////////////////////////////////////////
        internal RgFcLcb2007 RgFcLcb2007 = new RgFcLcb2007();   // 183 plcf pairs.

        /// <summary>
        /// I found that in an encrypted document, some bytes at the start are not encrypted.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int NotEncryptedHeaderLength = 68;

        /// <summary>
        /// Word stores various plcfs and tables with the stream named either "0Table" or "1Table". Ordinarily a file will contain
        /// only one table stream. However, in some unusual circumstances (e.g. crash during file save) a file might have two table
        /// streams. In that case the bit field fWhichTblStm in the FIB should be used to determine which table stream to read.
        /// If fWhichTblStm is 0, then the FIB refers to the stream named "0Table", and if fWhichTblStm is 1, then the FIB refers
        /// to the stream name "1Table".
        /// </summary>
        internal string TableStreamName
        {
            get { return (fWhichTblStm) ? "1Table" : "0Table"; }
        }

        /// <summary>
        /// Initializes a FIB.
        /// </summary>
        internal Fib(NFibValue targetNFib)
        {
            wIdent = 0xa5ec;

            // RK If modifying this, check that TestDefect4237, TestTableNested and TestTableNested2000
            // still look correct in MS Word.
            if (targetNFib == NFibValue.Word2007)
            {
                nFib = NFibValue.Word2007;
                cbRgFcLcb = FcLcbCountWord2007;
            }
            else
            {
                nFib = NFibValue.Word2003;
                cbRgFcLcb = FcLcbCountWord2003;
            }

            // RK I'm not aware of this field having any effects.
            // 0x4035 is what Word 2002 writes.
            // 0x6021 is what Word 2003 writes.
            // 0x6071 is written by Word 2003 build 11.5604.5606
            nProduct = 0x6021;
            lid = 1033;
            fComplex = true;
            fExtChar = true;
            nFibBack = 191;
            fWord97Saved = true;
            csw = 14;
            // 0x6A62 is what Word 2003 writes.
            // MS Recommends a unique value for the creator.
            // AW - means Aspose.Words
            wMagicCreated = 0x5741;
            // N  - means .NET
            // For Java this should be 'J' - 0x4a
            // For C++ this should be 'C' - 0x43
#if CPLUSPLUS
            wMagicRevised = 0x0043;
#else
            wMagicRevised = 0x004e;
#endif

            // 0x32CF is written by Word 2002.
            // 0xAD5C is what Word 2003 writes.
            // 0x5071 is written by Word 2003 build 11.5604.5606
            // Seems can be any value. Lets write unique.
            // We write version here. It can contain 4 symbols minimum (e.g. "17.4" min and 17.12.9 max)
            wMagicCreatedPrivate = (ushort)(AssemblyConstants.Version[0] | (AssemblyConstants.Version[1] << 8));
            wMagicRevisedPrivate = (ushort)(AssemblyConstants.Version[2] | (AssemblyConstants.Version[3] << 8));

            lidFE = 1033;
            cslw = 22;
            pnFbpChpFirst = pnFbpPapFirst = pnFbpLvcFirst = 0x000fffff;

            // 0x58AD is written by Word 2002.
            // 0xC73E is what Word 2003 writes.
            // 0x13A13 is written by Word 2003 build 11.5604.5606
            // Seems can be any value. Lets write unique.
            lProductCreated = 0xC73E;
            lProductRevised = 0xC73E;
        }

        /// <summary>
        /// NOTE this reads only the small part of the FIB that is guaranteed to be not encrypted.
        /// </summary>
        internal Fib(BinaryReader reader, IWarningCallback warningCallback, int offset)
        {
            mWarningCallback = warningCallback;

            ReadPart1(reader, offset);
        }

        internal bool IsPreWord60
        {
            get { return IsPreWord60Value(nFib); }
        }

        internal bool IsWord60
        {
            get { return IsWord60Value(nFib); }
        }

        /// <summary>
        /// Reads the first 68 bytes of the FIB that are never encrypted even in an encrypted document.
        /// </summary>
        private void ReadPart1(BinaryReader reader, int offset)
        {
            reader.BaseStream.Position = offset;

            // 2.5.2 FibBase
            wIdent = reader.ReadUInt16();
            nFib = (NFibValue)reader.ReadUInt16();

            nProduct = reader.ReadUInt16();
            lid = reader.ReadInt16();
            pnNext = reader.ReadInt16();

            int flags = reader.ReadByte();
            fDot = (flags & 0x01) != 0;
            fGlsy = (flags & 0x02) != 0;

            fComplex = (flags & 0x04) != 0;
            fHasPic = (flags & 0x08) != 0;
            cQuickSaves = (flags & 0xf0) >> 4;

            flags = reader.ReadByte();
            fEncrypted  = (flags & 0x01) != 0;
            fWhichTblStm= (flags & 0x02) != 0;
            fReadOnlyRecommended = (flags & 0x04) != 0;
            fWriteReservation = (flags & 0x08) != 0;
            fExtChar = (flags & 0x10) != 0;
            fLoadOverride = (flags & 0x20) != 0;
            fFarEast = (flags & 0x40) != 0;
            fObfuscation = (flags & 0x80) != 0;

            nFibBack = reader.ReadUInt16();
            lKey = reader.ReadInt32();
            envr = (byte)reader.ReadByte();//JAVA: casting needed since ReadByte() returns short in java.

            flags = reader.ReadByte();
            fMac = (flags & 0x01) != 0;
            fEmptySpecial = (flags & 0x02) != 0;
            fLoadOverridePage = (flags & 0x04) != 0;
            fFutureSavedUndo = (flags & 0x08) != 0;
            fWord97Saved = (flags & 0x10) != 0;

            chse = reader.ReadUInt16();
            chseTables = reader.ReadUInt16();
            fcMin = reader.ReadInt32();
            fcMac = reader.ReadInt32();

            if (IsWord60)
            {
                cbMac = reader.ReadInt32();

                // Ignore spare0.
                reader.ReadInt32();
                // Ignore spare1.
                reader.ReadInt32();
                // Ignore spare2.
                reader.ReadInt32();
                // Ignore spare3.
                reader.ReadInt32();

                ccpText = reader.ReadInt32();
                ccpFtn = reader.ReadInt32();
                ccpHdr = reader.ReadInt32();
                ccpMcr = reader.ReadInt32();
                ccpAtn = reader.ReadInt32();
                ccpEdn = reader.ReadInt32();
                ccpTxbx = reader.ReadInt32();
                ccpHdrTxbx = reader.ReadInt32();

                reader.ReadInt32(); // ccpSpare2
            }
            else
            {
                // 2.5.3 FibRgW97
                csw = reader.ReadUInt16();

                wMagicCreated = reader.ReadUInt16();
                wMagicRevised = reader.ReadUInt16();
                wMagicCreatedPrivate = reader.ReadUInt16();
                wMagicRevisedPrivate = reader.ReadUInt16();
                //Skip 9 * Int16 not used data.
                reader.BaseStream.Seek(9*2, SeekOrigin.Current);
                lidFE = reader.ReadInt16();

                // 2.5.4 FibRgLw97
                cslw = reader.ReadUInt16();

                cbMac = reader.ReadInt32();
            }
        }

        /// <summary>
        /// Reads the remaining data of the FIB (after the initial 68 bytes).
        /// To read successfully, the data must already be decrypted.
        /// </summary>
        internal void ReadPart2(BinaryReader reader, int offset)
        {
            if (IsWord60)
            {
                reader.BaseStream.Position = offset + 88;

                RgFcLcb.ReadWord60Part1(reader);

                reader.ReadInt16(); // wSpare4Fib

                pnChpFirst = reader.ReadInt16();
                pnPapFirst = reader.ReadInt16();
                cpnBteChp = reader.ReadInt16();
                cpnBtePap = reader.ReadInt16();

                RgFcLcb.ReadWord60Part2(reader);

                // Warnings.
                Warnings();
                return;
            }

            reader.BaseStream.Position = offset + NotEncryptedHeaderLength;

            lProductCreated = reader.ReadInt32();
            lProductRevised = reader.ReadInt32();

            ccpText = reader.ReadInt32();
            ccpFtn = reader.ReadInt32();
            ccpHdr = reader.ReadInt32();
            ccpMcr = reader.ReadInt32();
            ccpAtn = reader.ReadInt32();
            ccpEdn = reader.ReadInt32();
            ccpTxbx = reader.ReadInt32();
            ccpHdrTxbx = reader.ReadInt32();

            pnFbpChpFirst = reader.ReadInt32();
            pnChpFirst = reader.ReadInt32();
            cpnBteChp = reader.ReadInt32();
            pnFbpPapFirst = reader.ReadInt32();
            pnPapFirst = reader.ReadInt32();
            cpnBtePap = reader.ReadInt32();
            pnFbpLvcFirst = reader.ReadInt32();
            pnLvcFirst = reader.ReadInt32();
            cpnBteLvc = reader.ReadInt32();
            fcIslandFirst = reader.ReadInt32();
            fcIslandLim = reader.ReadInt32();

            // This is the count of pairs of longs.
            // The offset at this point is 154 (0x9a).
            cbRgFcLcb = reader.ReadUInt16();
            int fcLcbPos = (int)reader.BaseStream.Position;

            // Beginning of array of FC/LCB pairs.
            RgFcLcb.Read(reader);

            // End of table as documented per Word 97 specification.
            // Offset here is 0x382, meaning only 93 Fc/Lcb pairs have been read so far.
            // All tables from here are for versions greater than Word 97.
            Debug.Assert(reader.BaseStream.Position == offset + FibLengthWord97);

            // This block of data is for Word 2000 and later. 108 Fc/Lcb pairs.
            if (cbRgFcLcb >= FcLcbCountWord2000)
                RgFcLcb2000.Read(reader);

            // This block of data is for Word 2002 and later. 136 Fc/Lcb pairs.
            if (cbRgFcLcb >= FcLcbCountWord2002)
                RgFcLcb2002.Read(reader);

            // This block of data is for Word 2003 and later. 164 Fc/Lcb pairs.
            if (cbRgFcLcb >= FcLcbCountWord2003)
                RgFcLcb2003.Read(reader);

            // This block of data is for Word 2007 and later. 183 Fc/Lcb pairs.
            if (cbRgFcLcb >= FcLcbCountWord2007)
                RgFcLcb2007.Read(reader);

            // This is one more block of properties. I think it was added in Word 2000.
            // We detect it by looking at the number of cfclcb pairs.
            if (cbRgFcLcb >= FcLcbCountWord2000)
            {
                reader.BaseStream.Position = fcLcbPos + cbRgFcLcb * 8;

                int cswNew = reader.ReadUInt16();

                // These are entries I know so far.
                if (cswNew >= 2)
                {
                    RgCswNew = new FcLcb((int)reader.BaseStream.Position, cswNew * 2);

                    // New FIB value is stored here.
                    nFib = (NFibValue)reader.ReadUInt16();
                    // Force NFib to be at least NFibValue.Word2000.
                    if (nFib < NFibValue.Word2000)
                        nFib = NFibValue.Word2000;
                    cQuickSaves = reader.ReadUInt16();
                }
            }

            // WORDSNET-11471 Ensure that ccpText is valid.
            int totalCcp = ccpText + ccpFtn + ccpHdr + ccpMcr + ccpAtn + ccpEdn + ccpTxbx + ccpHdrTxbx + 1;
            if (fExtChar)
                totalCcp *= 2;

            int minCcp = fcMac - 2048;
            if ((nFib == NFibValue.Word97) && (totalCcp < minCcp))
            {
                // Adjust main subdoc character count and zero all other subdocs.
                ccpText = minCcp;
                ccpFtn = 0;
                ccpHdr = 0;
                ccpMcr = 0;
                ccpAtn = 0;
                ccpEdn = 0;
                ccpTxbx = 0;
                ccpHdrTxbx = 0;
            }

            // Warnings.
            Warnings();
        }

        /// <summary>
        /// Saves FIB in the Word 2003 format to the stream object.
        /// </summary>
        internal void Write(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);

            writer.Write(wIdent);
            // RK This is what Word 2003 writes. It is an "old" value superseded by nFib at the end of the FIB.
            writer.Write((UInt16)NFibValue.Word97);
            writer.Write(nProduct);
            writer.Write(lid);
            writer.Write(pnNext);

            UInt16 nBits16 = 0;
            if (fDot)
                nBits16 |= 0x0001;
            if (fGlsy)
                nBits16 |= 0x0002;
            if (fComplex)
                nBits16 |= 0x0004;
            if (fHasPic)
                nBits16 |= 0x0008;
            nBits16 |= (ushort)(0xf0 & (cQuickSaves << 4));
            if (fEncrypted)
                nBits16 |= 0x0100;
            if (fWhichTblStm)
                nBits16 |= 0x0200;
            if (fReadOnlyRecommended)
                nBits16 |= 0x0400;
            if (fWriteReservation)
                nBits16 |= 0x0800;
            if (fExtChar)
                nBits16 |= 0x1000;
            writer.Write(nBits16);

            writer.Write(nFibBack);
            writer.Write(lKey);
            writer.Write(envr);

            byte nBits8 = 0;
            if (fMac)
                nBits8 |= 0x0001;
            if (fEmptySpecial)
                nBits8 |= 0x0002;
            if (fLoadOverridePage)
                nBits8 |= 0x0004;
            if (fFutureSavedUndo)
                nBits8 |= 0x0008;
            if (fWord97Saved)
                nBits8 |= 0x0010;
            writer.Write(nBits8);

            writer.Write(chse);
            writer.Write(chseTables);
            writer.Write(fcMin);
            writer.Write(fcMac);

            //Beginning of the array of shorts "rgsw".
            writer.Write(csw);
            writer.Write(wMagicCreated);
            writer.Write(wMagicRevised);
            writer.Write(wMagicCreatedPrivate);
            writer.Write(wMagicRevisedPrivate);
            //Skip 9 * Int16 unused data.
            writer.Seek(9 * 2, SeekOrigin.Current);
            writer.Write(lidFE);
            writer.Write(cslw);

            //Beginning of the array of longs "rclw".
            writer.Write(cbMac);

            writer.Write(lProductCreated);
            writer.Write(lProductRevised);

            writer.Write(ccpText);
            writer.Write(ccpFtn);
            writer.Write(ccpHdr);
            writer.Write(ccpMcr);
            writer.Write(ccpAtn);
            writer.Write(ccpEdn);
            writer.Write(ccpTxbx);
            writer.Write(ccpHdrTxbx);

            writer.Write(pnFbpChpFirst);
            writer.Write(pnChpFirst);
            writer.Write(cpnBteChp);
            writer.Write(pnFbpPapFirst);
            writer.Write(pnPapFirst);
            writer.Write(cpnBtePap);
            writer.Write(pnFbpLvcFirst);
            writer.Write(pnLvcFirst);
            writer.Write(cpnBteLvc);
            writer.Write(fcIslandFirst);
            writer.Write(fcIslandLim);
            writer.Write(cbRgFcLcb);

            // Beginning of array of FC/LCB pairs "rgfclcb".
            int fcLcbStartPos = (int)writer.BaseStream.Position;
            // 0
            RgFcLcb.Write(writer);
            // *** End of Word 97 PLCF pairs.

            RgFcLcb2000.Write(writer);
            // 0x3fa. End of Word 2000 data. 108 Fc/Lcb pairs.
            Debug.Assert(writer.BaseStream.Position == 0x3fa);

            // Word 2002 PLCF pairs start here.
            RgFcLcb2002.Write(writer);
            // 0x4da. End of Word 2002 data. 136 plcf pairs.
            Debug.Assert(writer.BaseStream.Position == 0x4da);

            // Some of the Word 2003 PLCF pairs here.
            RgFcLcb2003.Write(writer, RgFcLcb.Clx.Fc);

            // 0x5ba. End of Word 2003 data here. 164 plcf pairs.
            Debug.Assert(writer.BaseStream.Position == 0x5ba);

            if (nFib == NFibValue.Word2007)
                RgFcLcb2007.Write(writer);

            // Verify that all FcLcb pairs are written.
            int fcLcbPairsWritten = ((int)writer.BaseStream.Position - fcLcbStartPos) / 8; // 8 bytes each pair.
            int dummyPlcfPairsToWrite = cbRgFcLcb - fcLcbPairsWritten;
            Debug.Assert(dummyPlcfPairsToWrite == 0);

            // New array of shorts since Word 2000.
            // Number of the following entries.
            writer.Write((UInt16)2);
            // The actual FIB version.
            writer.Write((UInt16)nFib);
            // New quick saves value.
            writer.Write((UInt16)0);
        }

        // public for java.
        [CppSkipDefinition] // TODO C++ doesn't support object reflection
        public new string ToString()
        {
            return StringUtil.ObjectToLogString(this, "FIB");
        }

        /// <summary>
        /// Converts document CP into CP relative to the subdocument where this CP belongs.
        /// Copied from Werner's code. It's a but rough, I'd prefer something more elegant.
        /// </summary>
        /// <param name="globalCP"></param>
        /// <returns></returns>
        internal int ToLocalCP(int globalCP)
        {
            int localCP = globalCP;

            if (localCP < ccpText)
                return localCP;
            localCP -= ccpText;

            if (localCP < ccpFtn)
                return localCP;
            localCP -= ccpFtn;

            if (localCP < ccpHdr)
                return localCP;
            localCP -= ccpHdr;

            if (localCP < ccpMcr)
                return localCP;
            localCP -= ccpMcr;

            if (localCP < ccpAtn)
                return localCP;
            localCP -= ccpAtn;

            if (localCP < ccpEdn)
                return localCP;
            localCP -= ccpEdn;

            if (localCP < ccpTxbx)
                return localCP;
            localCP -= ccpTxbx;

            if (localCP < ccpHdrTxbx)
                return localCP;

            throw new InvalidOperationException(string.Format(
                "Attempted to aim {0} characters past the end of the text. ",
                localCP));
        }

        /// <summary>
        /// Converts a local CP of the specified subdocument into a global CP.
        /// </summary>
        internal int ToGlobalCP(SubDocType subDocType, int localCP)
        {
            int globalCP = localCP;

            if (subDocType == SubDocType.Main)
                return globalCP;
            globalCP += ccpText;

            if (subDocType == SubDocType.Footnote)
                return globalCP;
            globalCP += ccpFtn;

            if (subDocType == SubDocType.Header)
                return globalCP;
            globalCP += ccpHdr;

            globalCP += ccpMcr;

            if (subDocType == SubDocType.Comment)
                return globalCP;
            globalCP += ccpAtn;

            if (subDocType == SubDocType.Endnote)
                return globalCP;
            globalCP += ccpEdn;

            if (subDocType == SubDocType.Textbox)
                return globalCP;
            globalCP += ccpTxbx;

            if (subDocType == SubDocType.HeaderTextBox)
                return globalCP;

            throw new InvalidOperationException("Unknown subdocument type specified.");
        }

        /// <summary>
        /// Indicates that document uses extended Word60 character encoding.
        /// </summary>
        internal bool IsWord60ExtChar
        {
            get
            {
                return IsWord60 && fExtChar;
            }
        }

        internal static bool IsWord60Value(NFibValue nFib)
        {
            return (NFibValue.Word60ForWin <= nFib) && (nFib < NFibValue.Value106);
        }

        /// <summary>
        /// Indicates that Fib is valid.
        /// </summary>
        /// <remarks>
        /// Used to detect document embedded into WordDocument stream, see WORDSNET-26415.
        /// </remarks>
        internal bool IsValid
        {
            get
            {
                if (cbMac < 0)
                    return false;

                switch (nFib)
                {
                    case NFibValue.Word60ForWin:
                    case NFibValue.Word60ForMac:
                    case NFibValue.Value106:
                    case NFibValue.Value191:
                    case NFibValue.Word95:
                    case NFibValue.Word97:
                    case NFibValue.Word97Empty:
                    case NFibValue.Word97BiDi:
                    case NFibValue.Word2000:
                    case NFibValue.Word2002:
                    case NFibValue.Word2003:
                    case NFibValue.Word2007:
                        return true;

                    default:
                        return false;
                }
            }
        }

        private static bool IsPreWord60Value(NFibValue nFib)
        {
            return nFib < NFibValue.Word60ForWin;
        }

        internal FcLcb RgCswNew = new FcLcb();

        /// <summary>
        /// This is the number of FC/LCB pairs written by Word 97.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FcLcbCountWord97 = 93;

        /// <summary>
        /// RK 108 is my number of FC/LCB pairs written by Word 2000.
        /// But according to the spec, this is 110.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FcLcbCountWord2000 = 108;

        /// <summary>
        /// This is the number of FC/LCB pairs written by Word 2002.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FcLcbCountWord2002 = 136;

        /// <summary>
        /// This is the number of FC/LCB pairs written by Word 2003.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FcLcbCountWord2003 = 164;

        /// <summary>
        /// This is the number of FC/LCB pairs written by Word 2007.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int FcLcbCountWord2007 = 183;

        /// <summary>
        /// The length of the Fib in Word97.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int FibLengthWord97 = 0x382;

        private readonly IWarningCallback mWarningCallback;

        internal static bool logSwitch = false;

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Issues warnings based on FIB information.
        /// </summary>
        private void Warnings()
        {
            // General Fib specific warnings.
            WarnIf(fLoadOverride, WarningStrings.LoadOverride);
            WarnIf(fFarEast, WarningStrings.FarEastInstallationLid);
            WarnIf(IsWord60, WarningStrings.Word60Document);

            // RgFcLcb specific warnings.
            WarnIf(RgFcLcb.HasPrinterDriverData, WarningStrings.PrinterDriverData);
            WarnIf(RgFcLcb.HasLastSelectionData, WarningStrings.LastSelectionData);
            WarnIf((fDot && RgFcLcb.HasAutoCaptions), WarningStrings.AutoCaptions);
            WarnIf(RgFcLcb.HasSpellCheckerData, WarningStrings.SpellChecker);
            WarnIf(RgFcLcb.HasGrammarCheckerData, WarningStrings.GrammarChecker);
            WarnIf(RgFcLcb.HasRouteSlip, WarningStrings.RouteSlip);
            WarnIf(RgFcLcb.HasSaveHistory, WarningStrings.SaveHistory);
            WarnIf(RgFcLcb.HasVersioning, WarningStrings.DocumentVersioning);
            WarnIf(RgFcLcb.HasDeprecatedOcxData, WarningStrings.DeprecatedOcxData);
            WarnIf(RgFcLcb.HasAutoSummaryPriorities, WarningStrings.AutoSummaryPriorities);

            // RgLcb2000 specific warnings.
            WarnIf(RgFcLcb2000.HasThreadingData, WarningStrings.ThreadingData);
            WarnIf(RgFcLcb2000.HasHybridListFormats, WarningStrings.HybridListsFormats);
            WarnIf(RgFcLcb2000.HasEnvelopeData, WarningStrings.MsoEnvelope);
            WarnIf(RgFcLcb2000.HasLanguageAutoDetectData, WarningStrings.LanguageAutoDetectData);
            WarnIf(RgFcLcb2000.HasGrammarCheckerOptions, WarningStrings.GrammarCheckerOptions);

            // RgLcb2002 specific warnings.
            WarnIf(RgFcLcb2002.HasGrammarCheckerCookies, WarningStrings.GrammarCheckerCookies);
            WarnIf(RgFcLcb2002.HasTextFrameworkData, WarningStrings.TextFrameworkData);
            WarnIf(RgFcLcb2002.HasSmartTagRecognizerData, WarningStrings.SmartTagRecognizerData);
            WarnIf(RgFcLcb2002.HasRepairBookmarks, WarningStrings.RepairBookmarks);
            WarnIf(RgFcLcb2002.HasConsistencyCheckerBookmarks, WarningStrings.ConsistencyCheckerBookmarks);

            // RgLcb2003 specific warnings.
            WarnIf(RgFcLcb2003.HasRevisionsAuthorSelection, WarningStrings.RevisionsAuthorSelection);
        }

        /// <summary>
        /// Conditionally logs a warning to the user-provided warning callback.
        /// </summary>
        private void WarnIf(bool condition, string description)
        {
            if (condition)
                WarningUtil.WarnUnexpected(mWarningCallback, WarningSource.Doc, description);
        }
    }
}
