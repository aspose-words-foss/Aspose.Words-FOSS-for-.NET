// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2012 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// Implements [MS-DOC] 2.5.6 FibRgFcLcb97 structure.
    /// </summary>
    internal class RgFcLcb
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Fc/Lcb of STSH.
        /// </summary>
        /// <remarks>
        /// During fast save Word will attempt to reuse this allocation if STSH is small enough to fit.
        /// This value is undefined and MUST be ignored.
        /// </remarks>
        internal FcLcb StshfOrig = new FcLcb();     // 0x9a

        /// <summary>
        /// Fc/Lcb of STSH.
        /// </summary>
        internal FcLcb Stshf = new FcLcb();         // 0xa2

        /// <summary>
        /// Fc/Lcb of footnote reference PLCF.
        /// </summary>
        internal FcLcb PlcffndRef = new FcLcb();    // 0xaa

        /// <summary>
        /// Fc/Lcb of footnote text PLCF.
        /// </summary>
        internal FcLcb PlcffndTxt = new FcLcb();    // 0xb2

        /// <summary>
        /// Fc/Lcb of annotation reference PLCF.
        /// </summary>
        internal FcLcb PlcfAndRef = new FcLcb();    // 0xba

        /// <summary>
        /// Fc/Lcb of annotation text PLCF.
        /// </summary>
        internal FcLcb PlcfAndTxt = new FcLcb();    // 0xc2

        /// <summary>
        /// Fc/Lcb of PlcfSed in the Table Stream which specifies the locations of property lists for each section in the Main Document.
        /// </summary>
        internal FcLcb PlcfSed = new FcLcb();         // 0xca

        /// <summary>
        /// Fc/Lcb of paragraph descriptor PLCF.
        /// SPEC: This value is undefined and MUST be ignored.
        /// </summary>
        internal FcLcb PlcPad = new FcLcb();         // 0xd2

        /// <summary>
        /// Fc/Lcb of PLCF which specifies version-specific information about paragraph height.
        /// SPEC: This Plc SHOULD NOT be emitted and SHOULD be ignored.
        /// </summary>
        /// <remarks>
        /// Word 97, Word 2000, and Word 2002 emit this information when performing an incremental save. 
        /// Office Word 2003, Office Word 2007,and Word 2010 do not emit this information.
        /// Word 97 reads this information if FibBase.nFib is 193. Word 2000 reads this information if FibRgCswNew.nFibNew is 217. 
        /// Word 2002 reads this information if FibRgCswNew.nFibNew is 257. 
        /// 
        /// Office Word 2003, Office Word 2007, and Word 2010 do not read this information.
        /// </remarks>
        internal FcLcb PlcfPhe = new FcLcb();          // 0xda

        /// <summary>
        /// Fc/Lcb of glossary string table.
        /// If FibBase.fGlsy of the Fib that contains this FibRgFcLcb97 is zero, this value MUST be zero.
        /// </summary>
        internal FcLcb SttbfGlsy = new FcLcb();        // 0xe2

        /// <summary>
        /// Fc/Lcb of glossary PLCF.
        /// If FibBase.fGlsy of the Fib that contains this FibRgFcLcb97 is zero, this value MUST be zero.
        /// </summary>
        internal FcLcb PlcfGlsy = new FcLcb();         // 0xea

        /// <summary>
        /// Fc/Lcb of header PLCF.
        /// SPEC: A PlcfHdd MUST exist if FibRgLw97.ccpHdd indicates that there are characters in the Header Document 
        /// (that is, if FibRgLw97.ccpHdd is greater than 0). Otherwise, the Plcfhdd MUST NOT exist.
        /// </summary>
        internal FcLcb PlcfHdd = new FcLcb();       // 0xf2

        /// <summary>
        /// Fc/Lcb of character property bin table PLCF.
        /// </summary>
        internal FcLcb PlcfBteChpx = new FcLcb();   // 0xfa

        /// <summary>
        /// Fc/Lcb of paragraph property bin table PLCF.
        /// </summary>
        internal FcLcb PlcfBtePapx = new FcLcb();   // 0x102

        /// <summary>
        /// Fc/Lcb of PLCF reserved for private use. The SEA is 6 bytes long.
        /// SPEC: This value is undefined and MUST be ignored.
        /// </summary>
        internal FcLcb PlcfSea = new FcLcb();       // 0x10a

        /// <summary>
        /// Fc/Lcb of font information STTBF. See the FFN file structure definition.
        /// </summary>
        internal FcLcb SttbfFfn = new FcLcb();      // 0x112

        /// <summary>
        /// Fc/Lcb of the PLCF of field positions in the main document.
        /// </summary>
        internal FcLcb PlcfFldMom = new FcLcb();    // 0x11a

        /// <summary>
        /// Fc/Lcb of the PLCF of field positions in the header subdocument.
        /// </summary>
        internal FcLcb PlcfFldHdr = new FcLcb();    // 0x122

        /// <summary>
        /// Fc/Lcb of the PLCF of field positions in the footnote subdocument.
        /// </summary>
        internal FcLcb PlcfFldFtn = new FcLcb();    // 0x12a

        /// <summary>
        /// Fc/Lcb of the PLCF of field positions in the annotation subdocument.
        /// </summary>
        internal FcLcb PlcfFldAtn = new FcLcb();    // 0x132

        /// <summary>
        /// Fc/Lcb of the PLCF of field positions in the macro subdocument.
        /// SPEC: This value is undefined and MUST be ignored.
        /// </summary>
        internal FcLcb PlcfFldMcr = new FcLcb();     // 0x13a

        /// <summary>
        /// Fc/Lcb in the Table Stream of the STTBF that records bookmark names in the main document.
        /// </summary>
        internal FcLcb SttbfBkmk = new FcLcb();     // 0x142 

        /// <summary>
        /// Fc/Lcb in the Table Stream of the PLCF that records the beginning CP offsets of bookmarks in the main document.
        /// </summary>
        internal FcLcb PlcfBkf = new FcLcb();       // 0x14a

        /// <summary>
        /// Fc/Lcb in the Table Stream of the PLCF that records the ending CP offsets of bookmarks recorded in the main document. 
        /// </summary>
        internal FcLcb PlcfBkl = new FcLcb();       // 0x152

        /// <summary>
        /// Fc/Lcb in the Table Stream of a Tcg that specifies command-related customizations. 
        /// </summary>
        internal FcLcb Cmds = new FcLcb();          // 0x15a

        /// <summary>
        /// SPEC: fcUnused1 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        private readonly FcLcb PlcfMcr = new FcLcb();        // 0x162

        /// <summary>
        /// SPEC: fcSttbfMcr (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        internal FcLcb SttbfMcr = new FcLcb();       // 0x16a

        /// <summary>
        /// Fc/Lcb in the Table Stream of the printer driver information (names of drivers, port etc...).
        /// </summary>
        internal FcLcb PrDrvr = new FcLcb();         // 0x172

        /// <summary>
        /// Fc/Lcb in the Table Stream of the print environment in portrait mode.
        /// </summary>
        internal FcLcb PrEnvPort = new FcLcb();      // 0x17a

        /// <summary>
        /// Fc/Lcb in the Table Stream of the print environment in landscape mode.
        /// </summary>
        internal FcLcb PrEnvLand = new FcLcb();      // 0x182

        /// <summary>
        /// Fc/Lcb in the Table Stream of Selsf which specifies the last selection that was made in the Main Document.
        /// </summary>
        internal FcLcb Wss = new FcLcb();            // 0x18a 

        /// <summary>
        /// Fc/Lcb of document property data structure.
        /// </summary>
        /// <remarks>
        /// Lcb is 84 when nFib less than 103.
        /// </remarks>
        internal FcLcb Dop = new FcLcb();           // 0x192

        /// <summary>
        /// Fc/Lcb of STTBF of associated strings. See STTBFASSOC.
        /// The value of lcbSttbfAssoc MUST NOT be zero.
        /// </summary>
        internal FcLcb SttbfAssoc = new FcLcb();    // 0x19a

        /// <summary>
        /// Fc/Lcb of beginning of information for complex files.
        /// SPEC: The value of lcbClx MUST be greater than zero.
        /// </summary>
        internal FcLcb Clx = new FcLcb();           // 0x1a2

        /// <summary>
        /// Fc/Lcb of page descriptor PLCF for footnote subdocument.
        /// </summary>
        /// <remarks>
        /// fcPlcfPgdFtn (4 bytes): This value is undefined and MUST be ignored.
        /// </remarks>
        internal FcLcb PlcfPgdFtn = new FcLcb();     // 0x1aa

        // 35

        /// <summary>
        /// Fc/Lcb of the name of the original file.
        /// SPEC: fcAutosaveSource (4 bytes): This value is undefined and MUST be ignored.
        /// SPEC: lcbAutosaveSource (4 bytes): This value MUST be zero and MUST be ignored.
        /// </summary>
        internal FcLcb AutosaveSource = new FcLcb();     // 0x1b2

        /// <summary>
        /// Fc/Lcb of strings array recording the names of the owners of annotations.
        /// </summary>
        internal FcLcb GrpXstAtnOwners = new FcLcb();   // 0x1ba

        /// <summary>
        /// Fc/Lcb of Sttbf in Table Stream that records names of bookmarks in the annotation subdocument.
        /// </summary>
        internal FcLcb SttbfAtnBkmk = new FcLcb();      // 0x1c2

        /// <summary>
        /// Fc/Lcb of the FDOA (drawn object) PLCF for main document. The length of the FDOA is 6 bytes.
        /// SPEC: fcUnused2 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        internal FcLcb PlcfdoaMom = new FcLcb();         // 0x1ca

        /// <summary>
        /// Fc/Lcb of the FDOA (drawn object) PLCF for the header document. The length of the FDOA is 6 bytes.
        /// SPEC: fcUnused3 (4 bytes): This value is undefined and MUST be ignored.
        /// </summary>
        internal FcLcb PlcfdoaHdr = new FcLcb();         // 0x1d2

        // 40

        /// <summary>
        /// Fc/Lcb of the FSPA PLCF for main document.
        /// If there are no shapes in the Main Document, lcbPlcfSpaMom and fcPlcfSpaMom MUST be zero and MUST be ignored.
        /// </summary>
        internal FcLcb PlcfSpaMom = new FcLcb();        // 0x1da

        /// <summary>
        /// Fc/Lcb of the FSPA PLCF for header document.
        /// If there are no shapes in the Header Document, lcbPlcSpaHdr and fcPlcSpaHdr MUST both be zero and MUST be ignored.
        /// </summary>
        internal FcLcb PlcfSpaHdr = new FcLcb();      // 0x1e2

        /// <summary>
        /// BKF (bookmark first) PLCF of the annotation subdocument.
        /// </summary>
        internal FcLcb PlcfAtnBkf = new FcLcb();      // 0x1ea

        /// <summary>
        /// BKL (bookmark last) PLCF of the annotation subdocument.
        /// </summary>
        internal FcLcb PlcfAtnBkl = new FcLcb();      // 0x1f2

        /// <summary>
        /// PMS (Print Merge State) information block.
        /// </summary>
        internal FcLcb Pms = new FcLcb();           // 0x1fa

        // 45

        /// <summary>
        /// Fc/Lcb of form field Sttbf which contains strings used in form field dropdown controls.
        /// </summary>
        /// <remarks>
        /// SPEC: fcFormFldSttbs (4 bytes): This value is undefined and MUST be ignored.
        /// SPEC: lcbFormFldSttbs (4 bytes): This value MUST be zero, and MUST be ignored.
        /// </remarks>
        internal FcLcb FormFldSttbs = new FcLcb();    // 0x202

        /// <summary>
        /// Fc/Lcb of PLCFendRef which points to endnote references in the main document stream.
        /// </summary>
        internal FcLcb PlcfendRef = new FcLcb();      // 0x20a

        /// <summary>
        /// Fc/Lcb of PLCFendRef which points to endnote text in the endnote document stream which corresponds with the PLCFendRef.
        /// </summary>
        /// <remarks>
        /// lcbPlcfendTxt MUST be zero if FibRgLw97.ccpEdn is zero, and MUST be nonzero if FibRgLw97.ccpEdn is nonzero.
        /// </remarks>
        internal FcLcb PlcfendTxt = new FcLcb();      // 0x212

        /// <summary>
        /// Fc/Lcb to PLCF of field positions in the endnote subdocument.
        /// </summary>
        internal FcLcb PlcfFldEdn = new FcLcb();      // 0x21a

        /// <summary>
        /// Fc/Lcb to PLCF of page boundaries in the endnote subdocument.
        /// </summary>
        /// <remarks>
        /// [MS-DOC] fcUnused4 (4 bytes): This value is undefined and MUST be ignored.
        /// </remarks>
        internal FcLcb PlcfpgdEdn = new FcLcb();       // 0x222

        // 50

        /// <summary>
        /// Fc/Lcb in the Table Stream of the office art object table data.
        /// </summary>
        internal FcLcb DggInfo = new FcLcb();         // 0x22a

        /// <summary>
        /// Fc/Lcb to STTBF that contains the names of authors who have added revision marks or comments to the document.
        /// </summary>
        internal FcLcb SttbfRMark = new FcLcb();      // 0x232

        /// <summary>
        /// Fc/Lcb of the SttbfCaption that contains information about the captions.
        /// </summary>
        /// <remarks>
        /// If this document is not the Normal template, this value MUST be ignored.
        /// 
        /// If FibBase.fDot of the Fib that contains this FibRgFcLcb97 is zero, Lcb value MUST be zero.
        /// 
        /// Each string in this STTB structure is the label of a caption, and MUST have less than or equal to 40 characters. 
        /// The extra data appended to each string is a CAPI structure that specifies addition information about the caption.
        /// </remarks>
        internal FcLcb SttbfCaption = new FcLcb();     // 0x23a

        /// <summary>
        /// Fc/Lcb of the SttbfAutoCaption that contains information about the AutoCaption strings.
        /// </summary>
        /// <remarks>
        /// If base.fDot of the Fib that contains this FibRgFcLcb97 is zero, Lcb value MUST be zero.
        /// 
        /// Each string in this STTB is the ProgID of an OLE object that, when inserted into the document, 
        /// automatically has a caption inserted with it. 
        /// The extra data which is appended to each string is an unsigned 16-bit integer that specifies a zero-based index into SttbfCaption.
        /// </remarks>
        internal FcLcb SttbfAutoCaption = new FcLcb(); // 0x242

        /// <summary>
        /// Fc/Lcb of PLCF that contains information about all master documents and subdocuments. See <see cref="PlcfWkb"/>.
        /// </summary>
        internal FcLcb PlcfWkb = new FcLcb();         // 0x24a

        // 55

        /// <summary>
        /// Fc/Lcb in Table Stream of PLCF (of SPLS structures) that records spell check state.
        /// </summary>
        internal FcLcb PlcfSpl = new FcLcb();          // 0x252

        /// <summary>
        /// Fc/Lcb in the Table Stream of PlcftxbxTxt which specifies which ranges of text are contained in which textboxes.
        /// lcbPlcftxbxTxt MUST be zero if FibRgLw97.ccpTxbx is zero, and MUST be nonzero if FibRgLw97.ccpTxbx is nonzero.
        /// </summary>
        internal FcLcb PlcftxbxTxt = new FcLcb();     // 0x25a

        /// <summary>
        /// Fc/Lcb of PLCF of field boundaries recorded in the textbox subdocument.
        /// </summary>
        internal FcLcb PlcfFldTxbx = new FcLcb();     // 0x262

        /// <summary>
        /// Fc/Lcb of PLCF of beginning CP in the header text box subdocument.
        /// </summary>
        internal FcLcb PlcfHdrtxbxTxt = new FcLcb();  // 0x26a

        /// <summary>
        /// Fc/Lcb of PLCF of field boundaries recorded in the header textbox subdocument.
        /// </summary>
        internal FcLcb PlcffldHdrTxbx = new FcLcb();  // 0x272

        // 60

        /// <summary>
        /// Fc/Lcb in the Table Stream of StwUser that specifies the user-defined variables and VBA digital signature (2).
        /// </summary>
        internal FcLcb StwUser = new FcLcb();         // 0x27a

        /// <summary>
        /// Fc/Lcb in table stream of embedded true type font data.
        /// </summary>
        internal FcLcb SttbTtmbd = new FcLcb();       // 0x282

        /// <summary>
        /// Fc/Lcb of array of Cdb structures which are grammar checker cookies.
        /// </summary>
        /// <remarks>
        /// fcCookieData MAY be ignored. Office Word 2007 and Word 2010 ignore this data.
        /// </remarks>
        internal FcLcb CookieData = new FcLcb();       // 0x28a

        /// <summary>
        /// Deprecated document page layout cache. 
        /// </summary>
        /// <remarks>
        /// Information SHOULD NOT be emitted at this offset and SHOULD be ignored.
        /// Word 97 emits information at offset fcPgdMotherOldOld.
        /// Word 97 reads this information. Word 2000, Word 2002, Office Word 2003, Office Word 2007, and Word 2010 ignore this information.
        /// </remarks>
        internal FcLcb PgdMotherOldOld = new FcLcb();  // 0x292

        /// <summary>
        /// Fc/Lcb in the Table Stream of deprecated document text flow break cache.
        /// </summary>
        internal FcLcb BkdMotherOldOld = new FcLcb();  // 0x29a

        // 65

        /// <summary>
        /// Fc/Lcb in the Table Stream of deprecated footnote layout cache.
        /// </summary>
        internal FcLcb PgdFtnOldOld = new FcLcb();     // 0x2a2

        /// <summary>
        /// Fc/Lcb in the Table Stream of deprecated footnote text flow break cache begins at this offset.
        /// </summary>
        internal FcLcb BkdFtnOldOld = new FcLcb();     // 0x2aa

        /// <summary>
        /// Fc/Lcb in the Table Stream of deprecated endnote layout cache begins at this offset.
        /// </summary>
        internal FcLcb PgdEdnOldOld = new FcLcb();     // 0x2b2

        /// <summary>
        /// Fc/Lcb in the Table Stream of deprecated endnote text flow break cache.
        /// </summary>
        internal FcLcb BkdEdnOldOld = new FcLcb();     // 0x2ba

        /// <summary>
        /// Fc/Lcb of the STTBF containing field keywords. This is only used in a small number of the international versions of word.
        /// </summary>
        /// <remarks>
        /// This field is no longer written to the file for nFib >= 167.
        /// fcSttbfIntlFld (4 bytes): This value is undefined and MUST be ignored.
        /// </remarks>
        internal FcLcb SttbfIntlFld = new FcLcb();     // 0x2c2

        // 70

        /// <summary>
        /// Fc/Lcb of the RouteSlip that specifies the route slip for this document.
        /// </summary>
        internal FcLcb RouteSlip = new FcLcb();       // 0x2ca

        /// <summary>
        /// Fc/Lcb of STTBF recording the names of the users who have saved this document alternating with the save locations.
        /// </summary>
        /// <remarks>
        /// SttbSavedBy is only saved and read by Word 97 and Word 2000.
        /// </remarks>
        internal FcLcb SttbSavedBy = new FcLcb();      // 0x2d2

        /// <summary>
        /// Fc/Lcb in the Table Stream of SttbFnm that contains information about the external files that are referenced by this document.
        /// </summary>
        internal FcLcb SttbFnm = new FcLcb();         // 0x2da

        /// <summary>
        /// Fc/Lcb in the Table Stream of PlfLst that contains list formatting information.
        /// </summary>
        internal FcLcb PlcfLst = new FcLcb();         // 0x2e2

        /// <summary>
        /// Fc/Lcb in the Table Stream of PlfLfo that contains list formatting override information.
        /// </summary>
        internal FcLcb PlfLfo = new FcLcb();          // 0x2ea

        // 75

        /// <summary>
        /// Fc/Lcb in the Table Stream of PlcftxbxBkd that specifies which ranges of text go inside which textboxes.
        /// </summary>
        internal FcLcb PlcfTxbxBkd = new FcLcb();     // 0x2f2

        /// <summary>
        /// Fc/Lcb in the Table Stream of PlcfTxbxHdrBkd that specifies which ranges of text are contained inside which header textboxes.
        /// lcbPlcfTxbxHdrBkd MUST be zero if FibRgLw97.ccpHdrTxbx is zero, and MUST be nonzero if FibRgLw97.ccpHdrTxbx is nonzero.
        /// </summary>
        internal FcLcb PlcfTxbxHdrBkd = new FcLcb();  // 0x2fa

        /// <summary>
        /// Fc/Lcb in the WordDocument Stream of version-specific undo information.
        /// </summary>
        /// <remarks>
        /// This information SHOULD NOT be emitted and SHOULD be ignored.
        /// Word 97 and Word 2000 write this information when the user chooses to save versions in the document.
        /// Word 97, Word 2000, Word 2002, and Office Word 2003 read this information. Office Word 2007 and Word 2010 ignore it.
        /// </remarks>
        internal FcLcb DocUndoWord9 = new FcLcb();     // 0x302

        /// <summary>
        /// Fc/Lcb in the WordDocument Stream of version-specific undo information.
        /// </summary>
        internal FcLcb Rgbuse = new FcLcb();           // 0x30a

        /// <summary>
        /// Fc/Lcb in the WordDocument Stream of version-specific undo information.
        /// </summary>
        internal FcLcb Usp = new FcLcb();              // 0x312

        // 80 //

        /// <summary>
        /// Fc/Lcb in the Table Stream of version-specific undo information.
        /// </summary>
        internal FcLcb Uskf = new FcLcb();             // 0x31a

        /// <summary>
        /// Fc/Lcb in the Table Stream of Plc that contains version-specific undo information.
        /// </summary>
        internal FcLcb PlcupcRgbUse = new FcLcb();     // 0x322

        /// <summary>
        /// Fc/Lcb in the Table Stream of the Plc that contains version-specific undo information.
        /// </summary>
        internal FcLcb PlcupcUsp = new FcLcb();        // 0x32a

        /// <summary>
        /// Fc/Lcb in the Table Stream of string table of style names for glossary entries.
        /// </summary>
        /// <remarks>
        /// If base.fGlsy of the Fib that contains this FibRgFcLcb97 is zero, this value MUST be zero.
        /// </remarks>
        internal FcLcb SttbGlsyStyle = new FcLcb();    // 0x332

        /// <summary>
        /// Fc/Lcb in the Table Stream of the PlfGosl that contains option set for a grammar checker.
        /// </summary>
        internal FcLcb Plgosl = new FcLcb();           // 0x33a

        // 85

        /// <summary>
        /// Fc/Lcb of the RgxOcxInfo that specifies information about the OLE controls.
        /// </summary>
        /// <remarks>
        /// If there are any OLE controls in the document, fcPlcocx MUST point to a valid RgxOcxInfo.
        ///
        /// The data that is contained in OcxInfo structures SHOULD be ignored.
        /// 
        /// Office Word 2003, Office Word 2007, and Word 2010 ignore the values in the OcxInfo structure but, for backward compatibility, 
        /// emit values based on the OLE controls in the document. The values are populated by finding all the control FLDs in the document and 
        /// saving the values for the corresponding OLE controls. 
        /// 
        /// Previous versions of Word expect that the values in OcxInfo structures and the values of the controls all match. 
        /// The description of OcxInfo fields specifies the values that are written.
        /// </remarks>
        internal FcLcb Plcocx = new FcLcb();           // 0x342

        /// <summary>
        /// Fc/Lcb in the Table Stream of deprecated numbering field cache.
        /// </summary>
        internal FcLcb PlcfBteLvc = new FcLcb();       // 0x034a

        /// <summary>
        /// The low-order part of a FILETIME structure, as specified by [MS-DTYP], that specifies when the document was last saved.
        /// </summary>
        private int dwLowDateTime;      // 0x352
        /// <summary>
        /// The high-order part of a FILETIME structure, as specified by [MS-DTYP], that specifies when the document was last saved.
        /// </summary>
        private int dwHighDateTime;     // 0x356

        /// <summary>
        ///  Fc/Lcb in the Table Stream of the deprecated list level cache.
        /// </summary>
        internal FcLcb PlcfLvcPre10 = new FcLcb();     // 0x35a

        /// <summary>
        /// Fc/Lcb of PlcfAsumy whose data elements contain information about priority of a text range for AutoSummary.
        /// </summary>
        internal FcLcb PlcfAsumy = new FcLcb();        // 0x362

        // 90

        /// <summary>
        /// Fc/Lcb of a Plcfgram, which specifies the state of the grammar checker for each text range.
        /// </summary>
        internal FcLcb PlcfGram = new FcLcb();         // 0x36a

        /// <summary>
        /// Fc/Lcb in the Table Stream of SttbListNames, which specifies the LISTNUM field names of the lists.
        /// </summary>
        internal FcLcb SttbListNames = new FcLcb();   // 0x0372

        /// <summary>
        /// Fc/Lcb of the deprecated, version-specific undo information.
        /// </summary>
        /// <remarks>
        /// Some sort of a string table that contains version comments mangled with timestamps probably.
        /// </remarks>
        internal FcLcb SttbfUssr = new FcLcb();       // 0x37a

        // ReSharper enable InconsistentNaming

        internal bool HasAutoCaptions
        {
            get { return (SttbfCaption.Lcb > 0) || (SttbfAutoCaption.Lcb > 0); }
        }

        internal bool HasVersioning
        {
            get 
            { 
                return (DocUndoWord9.Lcb > 0) || (Rgbuse.Lcb > 0) || (Usp.Lcb > 0) || (Uskf.Lcb > 0) || 
                (PlcupcRgbUse.Lcb > 0) || (PlcupcUsp.Lcb > 0) || (SttbfUssr.Lcb > 0); 
            }
        }

        internal bool HasPrinterDriverData
        {
            get { return (PrDrvr.Lcb > 0) || (PrEnvPort.Lcb > 0) || (PrEnvLand.Lcb > 0); }
        }
        
        internal bool HasGrammarCheckerData
        {
            get { return (CookieData.Lcb > 0) || (Plgosl.Lcb > 0) || (PlcfGram.Lcb > 0); }
        }
        
        internal bool HasLastSelectionData
        {
            get { return Wss.Lcb > 0; }
        }

        internal bool HasSpellCheckerData
        {
            get { return PlcfSpl.Lcb > 0; }
        }

        internal bool HasRouteSlip
        {
            get { return RouteSlip.Lcb > 0; }
        }

        internal bool HasAutoSummaryPriorities
        {
            get { return PlcfAsumy.Lcb > 0; }
        }

        internal bool HasDeprecatedOcxData
        {
            get { return Plcocx.Lcb > 0; }
        }

        internal bool HasSaveHistory
        {
            get { return SttbSavedBy.Lcb > 0; }
        }

        internal void Read(BinaryReader reader)
        {
            // 0
            StshfOrig.Read(reader);
            Stshf.Read(reader);
            PlcffndRef.Read(reader);
            PlcffndTxt.Read(reader);
            PlcfAndRef.Read(reader);
            PlcfAndTxt.Read(reader);
            PlcfSed.Read(reader);
            PlcPad.Read(reader);
            PlcfPhe.Read(reader);
            SttbfGlsy.Read(reader);
            // 10
            PlcfGlsy.Read(reader);
            PlcfHdd.Read(reader);
            PlcfBteChpx.Read(reader);
            PlcfBtePapx.Read(reader);
            PlcfSea.Read(reader);
            SttbfFfn.Read(reader);
            PlcfFldMom.Read(reader);
            PlcfFldHdr.Read(reader);
            PlcfFldFtn.Read(reader);
            PlcfFldAtn.Read(reader);
            // 20
            PlcfFldMcr.Read(reader);
            SttbfBkmk.Read(reader);
            PlcfBkf.Read(reader);
            PlcfBkl.Read(reader);
            Cmds.Read(reader);
            PlcfMcr.Read(reader);
            SttbfMcr.Read(reader);
            PrDrvr.Read(reader);
            PrEnvPort.Read(reader);
            PrEnvLand.Read(reader);
            // 30
            Wss.Read(reader);
            Dop.Read(reader);
            SttbfAssoc.Read(reader);
            Clx.Read(reader);
            PlcfPgdFtn.Read(reader);
            AutosaveSource.Read(reader);
            GrpXstAtnOwners.Read(reader);
            SttbfAtnBkmk.Read(reader);
            PlcfdoaMom.Read(reader);
            PlcfdoaHdr.Read(reader);
            // 40
            PlcfSpaMom.Read(reader);
            PlcfSpaHdr.Read(reader);
            PlcfAtnBkf.Read(reader);
            PlcfAtnBkl.Read(reader);
            Pms.Read(reader);
            FormFldSttbs.Read(reader);
            PlcfendRef.Read(reader);
            PlcfendTxt.Read(reader); 
            PlcfFldEdn.Read(reader);
            PlcfpgdEdn.Read(reader);
            // 50
            DggInfo.Read(reader);
            SttbfRMark.Read(reader);
            SttbfCaption.Read(reader);
            SttbfAutoCaption.Read(reader);
            PlcfWkb.Read(reader);
            // 55
            PlcfSpl.Read(reader);
            PlcftxbxTxt.Read(reader);
            PlcfFldTxbx.Read(reader);
            PlcfHdrtxbxTxt.Read(reader);
            PlcffldHdrTxbx.Read(reader);
            // 60
            StwUser.Read(reader);
            SttbTtmbd.Read(reader);
            CookieData.Read(reader);
            PgdMotherOldOld.Read(reader);
            BkdMotherOldOld.Read(reader);
            // 65
            PgdFtnOldOld.Read(reader);
            BkdFtnOldOld.Read(reader);
            PgdEdnOldOld.Read(reader);
            BkdEdnOldOld.Read(reader);
            SttbfIntlFld.Read(reader);
            // 70
            RouteSlip.Read(reader);
            SttbSavedBy.Read(reader);
            SttbFnm.Read(reader);
            PlcfLst.Read(reader);
            PlfLfo.Read(reader);
            // 75
            PlcfTxbxBkd.Read(reader);
            PlcfTxbxHdrBkd.Read(reader);
            DocUndoWord9.Read(reader);
            Rgbuse.Read(reader);
            Usp.Read(reader);
            // 80
            Uskf.Read(reader);
            PlcupcRgbUse.Read(reader);
            PlcupcUsp.Read(reader);
            SttbGlsyStyle.Read(reader);
            Plgosl.Read(reader);
            // 85
            Plcocx.Read(reader);
            PlcfBteLvc.Read(reader);
            dwLowDateTime = reader.ReadInt32();
            dwHighDateTime = reader.ReadInt32();
            PlcfLvcPre10.Read(reader);
            PlcfAsumy.Read(reader);
            // 90
            PlcfGram.Read(reader);
            SttbListNames.Read(reader);
            SttbfUssr.Read(reader);
        }

        internal void Write(BinaryWriter writer)
        {
            StshfOrig.Write(writer);
            Stshf.Write(writer);
            PlcffndRef.Write(writer);
            PlcffndTxt.Write(writer);
            PlcfAndRef.Write(writer);
            // 5
            PlcfAndTxt.Write(writer);
            PlcfSed.Write(writer);
            PlcPad.Write(writer);
            PlcfPhe.Write(writer);
            SttbfGlsy.Write(writer);
            // 10
            PlcfGlsy.Write(writer);
            PlcfHdd.Write(writer);
            PlcfBteChpx.Write(writer);
            PlcfBtePapx.Write(writer);
            PlcfSea.Write(writer);
            // 15
            SttbfFfn.Write(writer);
            PlcfFldMom.Write(writer);
            PlcfFldHdr.Write(writer);
            PlcfFldFtn.Write(writer);
            PlcfFldAtn.Write(writer);
            // 20
            PlcfFldMcr.Write(writer);
            SttbfBkmk.Write(writer);
            PlcfBkf.Write(writer);
            PlcfBkl.Write(writer);
            Cmds.Write(writer);
            // 25
            PlcfMcr.Write(writer);
            SttbfMcr.Write(writer);
            PrDrvr.Write(writer);
            PrEnvPort.Write(writer);
            PrEnvLand.Write(writer);
            // 30
            Wss.Write(writer);
            Dop.Write(writer);
            SttbfAssoc.Write(writer);
            Clx.Write(writer);
            PlcfPgdFtn.Write(writer);
            // 35
            AutosaveSource.Write(writer);
            GrpXstAtnOwners.Write(writer);
            SttbfAtnBkmk.Write(writer);
            PlcfdoaMom.Write(writer);
            PlcfdoaHdr.Write(writer);
            // 40
            PlcfSpaMom.Write(writer);
            PlcfSpaHdr.Write(writer);
            PlcfAtnBkf.Write(writer);
            PlcfAtnBkl.Write(writer);
            Pms.Write(writer);
            // 45
            FormFldSttbs.Write(writer);
            PlcfendRef.Write(writer);
            PlcfendTxt.Write(writer);
            PlcfFldEdn.Write(writer);
            PlcfpgdEdn.Write(writer);
            // 50
            DggInfo.Write(writer);
            SttbfRMark.Write(writer);
            SttbfCaption.Write(writer);
            SttbfAutoCaption.Write(writer);
            PlcfWkb.Write(writer);
            // 55
            PlcfSpl.Write(writer);
            PlcftxbxTxt.Write(writer);
            PlcfFldTxbx.Write(writer);
            PlcfHdrtxbxTxt.Write(writer);
            PlcffldHdrTxbx.Write(writer);
            // 60
            StwUser.Write(writer);
            SttbTtmbd.Write(writer);
            CookieData.Write(writer);
            PgdMotherOldOld.Write(writer);
            BkdMotherOldOld.Write(writer);
            // 65
            PgdFtnOldOld.Write(writer);
            BkdFtnOldOld.Write(writer);
            PgdEdnOldOld.Write(writer);
            BkdEdnOldOld.Write(writer);
            SttbfIntlFld.Write(writer);
            // 70
            RouteSlip.Write(writer);
            SttbSavedBy.Write(writer);
            SttbFnm.Write(writer);
            PlcfLst.Write(writer);
            PlfLfo.Write(writer);
            // 75
            PlcfTxbxBkd.Write(writer);
            PlcfTxbxHdrBkd.Write(writer);
            DocUndoWord9.Write(writer);
            Rgbuse.Write(writer);
            Usp.Write(writer);
            // 80
            Uskf.Write(writer);
            PlcupcRgbUse.Write(writer);
            PlcupcUsp.Write(writer);
            SttbGlsyStyle.Write(writer);
            Plgosl.Write(writer);
            // 85
            Plcocx.Write(writer);
            PlcfBteLvc.Write(writer);
            writer.Write(dwLowDateTime);
            writer.Write(dwHighDateTime);
            PlcfLvcPre10.Write(writer);
            PlcfAsumy.Write(writer);
            // 90
            PlcfGram.Write(writer);
            SttbListNames.Write(writer);
            SttbfUssr.Write(writer);
        }

        internal void ReadWord60Part1(BinaryReader reader)
        {
            StshfOrig.Read(reader);
            Stshf.Read(reader);
            PlcffndRef.Read(reader);
            PlcffndTxt.Read(reader);
            PlcfAndRef.Read(reader);
            PlcfAndTxt.Read(reader);
            PlcfSed.Read(reader);
            PlcPad.Read(reader);
            PlcfPhe.Read(reader);
            SttbfGlsy.Read(reader);
            // 10
            PlcfGlsy.Read(reader);
            PlcfHdd.Read(reader);
            PlcfBteChpx.Read(reader);
            PlcfBtePapx.Read(reader);
            PlcfSea.Read(reader);
            SttbfFfn.Read(reader);
            PlcfFldMom.Read(reader);
            PlcfFldHdr.Read(reader);
            PlcfFldFtn.Read(reader);
            PlcfFldAtn.Read(reader);
            // 20
            PlcfFldMcr.Read(reader);
            SttbfBkmk.Read(reader);
            PlcfBkf.Read(reader);
            PlcfBkl.Read(reader);
            Cmds.Read(reader);
            PlcfMcr.Read(reader);
            SttbfMcr.Read(reader);
            PrDrvr.Read(reader);
            PrEnvPort.Read(reader);
            PrEnvLand.Read(reader);
            // 30
            Wss.Read(reader);
            Dop.Read(reader);
            SttbfAssoc.Read(reader);
            Clx.Read(reader);
            PlcfPgdFtn.Read(reader);
            AutosaveSource.Read(reader);
            GrpXstAtnOwners.Read(reader);
            SttbfAtnBkmk.Read(reader);
        }

        internal void ReadWord60Part2(BinaryReader reader)
        {
            PlcfdoaMom.Read(reader);
            PlcfdoaHdr.Read(reader);
            // 40
            new FcLcb().Read(reader);       // unused1
            new FcLcb().Read(reader);       // unused2

            PlcfAtnBkf.Read(reader);
            PlcfAtnBkl.Read(reader);
            Pms.Read(reader);
            FormFldSttbs.Read(reader);
            PlcfendRef.Read(reader);
            PlcfendTxt.Read(reader);
            PlcfFldEdn.Read(reader);
            PlcfpgdEdn.Read(reader);
            // 50
            new FcLcb().Read(reader);       // unused3
            SttbfRMark.Read(reader);
            SttbfCaption.Read(reader);
            SttbfAutoCaption.Read(reader);
            PlcfWkb.Read(reader);
            // 55
            new FcLcb().Read(reader);       // unused4
            PlcftxbxTxt.Read(reader);
            PlcfFldTxbx.Read(reader);
            PlcfHdrtxbxTxt.Read(reader);
            PlcffldHdrTxbx.Read(reader);
            // 60
            StwUser.Read(reader);
            SttbTtmbd.Read(reader);
            new FcLcb().Read(reader);       // unused5
            PgdMotherOldOld.Read(reader);
            BkdMotherOldOld.Read(reader);
            // 65
            PgdFtnOldOld.Read(reader);
            BkdFtnOldOld.Read(reader);
            PgdEdnOldOld.Read(reader);
            BkdEdnOldOld.Read(reader);
            SttbfIntlFld.Read(reader);
            // 70
            RouteSlip.Read(reader);
            SttbSavedBy.Read(reader);
            SttbFnm.Read(reader);
        }

        /// <summary>
        /// Writes dummy Fc for few Fc/Lcb pairs although these structures are undefined.
        /// </summary>
        internal void SetDummyFc(int position)
        {
            PlcfPhe.Fc = position;
            PlcfSea.Fc = position;
            SttbfGlsy.Fc = position;
            PlcfGlsy.Fc = position;
        }
    }
}
