// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2012 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// RgFcLcb2003 part of FIB which contains Word2003 specific Fc/Lcb pairs.
    /// </summary>
    internal class RgFcLcb2003
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Fc/Lcb of the Hplxsdr that contains specifies information about XML schema definition references.
        /// </summary>
        internal FcLcb Hplxsdr = new FcLcb();

        /// <summary>
        /// Fc/Lcb of SttbfBkmkSdt that contains information about the structured document tag bookmarks.
        /// </summary>
        internal FcLcb SttbfBkmkSdt = new FcLcb();

        /// <summary>
        /// Fc/Lcb of the PlcBkfd that contains information about the structured document tag bookmarks.
        /// </summary>
        internal FcLcb PlcfBkfSdt = new FcLcb();

        /// <summary>
        /// Fc/Lcb of the PlcBkld that contains information about the structured document tag bookmarks.
        /// </summary>
        internal FcLcb PlcfBklSdt = new FcLcb();

        /// <summary>
        /// Fc/Lcb of array of 16-bit Unicode characters, which specifies the full path and file name of the XML Stylesheet 
        /// to apply when saving this document in XML format.
        /// </summary>
        /// <remarks>
        /// lcbCustomXForm MUST be less than or equal to 4168 and MUST be evenly divisible by two.
        /// </remarks>
        internal readonly FcLcb CustomXForm = new FcLcb();

        /// <summary>
        /// Fc/Lcb of the SttbfBkmkProt that contains information about range-level protection bookmarks.
        /// </summary>
        internal readonly FcLcb SttbfBkmkProt = new FcLcb();

        /// <summary>
        /// Fc/Lcb of the PlcBkf that contains information about range-level protection bookmarks.
        /// </summary>
        internal readonly FcLcb PlcfBkfProt = new FcLcb();

        /// <summary>
        /// Fc/Lcb in the Table Stream of the PlcBkl that contains information about range-level protection bookmarks.
        /// </summary>
        internal readonly FcLcb PlcfBklProt = new FcLcb();

        /// <summary>
        /// Fc/Lcb in the Table Stream of the SttbProtUser that specifies the usernames that are used for range-level protection.
        /// </summary>
        internal readonly FcLcb SttbProtUser = new FcLcb();

        /// <summary>
        /// Fc value MUST be zero, and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        /// <remarks>
        /// Made internal for unit testing.
        /// </remarks>
        internal readonly FcLcb Unused = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated paragraph mark information cache used by Word 2003. 
        /// </summary>
        internal readonly FcLcb PlcfpmiOld = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated paragraph mark information cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcfpmiOldInline = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated paragraph mark information cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcfpmiNew = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated paragraph mark information cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcfpmiNewInline = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated listnum field cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcflvcOld = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated listnum field cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcflvcOldInline = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated listnum field cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcflvcNew = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated listnum field cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PlcflvcNewInline = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated document page layout cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PgdMother = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated document text flow break cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb BkdMother = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated document author filter cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb AfdMother = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated footnote layout cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PgdFtn = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated footnote text flow break cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb BkdFtn = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated footnote author filter cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb AfdFtn = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated endnote layout cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb PgdEdn = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated endnote text flow break cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb BkdEdn = new FcLcb();

        /// <summary>
        /// Fc/Lcb of deprecated endnote author filter cache used by Word 2003 only.
        /// </summary>
        internal readonly FcLcb AfdEdn = new FcLcb();

        /// <summary>
        /// Deprecated AFD structure that specifies whose revisions and comments were being hidden 
        /// when this document was last saved used by Word2003 only.
        /// </summary>
        /// <remarks>
        /// See [MS-DOC] 2.9.2 Afd.
        /// </remarks>
        internal readonly FcLcb Afd = new FcLcb();

        // ReSharper enable InconsistentNaming
        internal bool HasRevisionsAuthorSelection
        {
            get { return (Afd.Lcb > 0); }
        }

        internal void Read(BinaryReader reader)
        {
            Hplxsdr.Read(reader);
            SttbfBkmkSdt.Read(reader);
            PlcfBkfSdt.Read(reader);
            PlcfBklSdt.Read(reader);
            CustomXForm.Read(reader);
            SttbfBkmkProt.Read(reader);
            PlcfBkfProt.Read(reader);
            PlcfBklProt.Read(reader);
            SttbProtUser.Read(reader);

            // Below fields are ignored and ain't written but read them for test reason.
            Unused.Read(reader);
            PlcfpmiOld.Read(reader);
            PlcfpmiOldInline.Read(reader);
            PlcfpmiNew.Read(reader);
            PlcfpmiNewInline.Read(reader);
            PlcflvcOld.Read(reader);
            PlcflvcOldInline.Read(reader);
            PlcflvcNew.Read(reader);
            PlcflvcNewInline.Read(reader);
            PgdMother.Read(reader);
            BkdMother.Read(reader);
            AfdMother.Read(reader);
            PgdFtn.Read(reader);
            BkdFtn.Read(reader);
            AfdFtn.Read(reader);
            PgdEdn.Read(reader);
            BkdEdn.Read(reader);
            AfdEdn.Read(reader);
            Afd.Read(reader);
        }

        /// <summary>
        /// Writes RgFcLcb2003 structure to given writer.
        /// </summary>
        /// <remarks>
        /// AM. Previous implementation wrote some dummy value to each Fc we ignore. 
        /// Trying to do the same to avoid any possible problems.
        /// </remarks>
        internal void Write(BinaryWriter writer, int dummyFc)
        {
            Hplxsdr.Write(writer);
            SttbfBkmkSdt.Write(writer);
            PlcfBkfSdt.Write(writer);
            PlcfBklSdt.Write(writer);
            CustomXForm.Write(writer);

            SttbfBkmkProt.Write(writer);
            PlcfBkfProt.Write(writer);
            PlcfBklProt.Write(writer);
            SttbProtUser.Write(writer);
            
            // This MUST be zero per spec. Otherwise BffValidator fails.
            Unused.Fc = 0;
            Unused.Lcb = 0;

            PlcfpmiOld.Fc = dummyFc;
            PlcfpmiOldInline.Fc = dummyFc;
            PlcfpmiNew.Fc = dummyFc;
            PlcfpmiNewInline.Fc = dummyFc;
            PlcflvcOld.Fc = dummyFc;
            PlcflvcOldInline.Fc = dummyFc;
            PlcflvcNew.Fc = dummyFc;
            PlcflvcNewInline.Fc = dummyFc;
            PgdMother.Fc = dummyFc;
            BkdMother.Fc = dummyFc;
            AfdMother.Fc = dummyFc;
            PgdFtn.Fc = dummyFc;
            BkdFtn.Fc = dummyFc;
            AfdFtn.Fc = dummyFc;
            PgdEdn.Fc = dummyFc;
            BkdEdn.Fc = dummyFc;
            AfdEdn.Fc = dummyFc;
            Afd.Fc = dummyFc;

            Unused.Write(writer);
            PlcfpmiOld.Write(writer);
            PlcfpmiOldInline.Write(writer);
            PlcfpmiNew.Write(writer);
            PlcfpmiNewInline.Write(writer);
            PlcflvcOld.Write(writer);
            PlcflvcOldInline.Write(writer);
            PlcflvcNew.Write(writer);
            PlcflvcNewInline.Write(writer);
            PgdMother.Write(writer);
            BkdMother.Write(writer);
            AfdMother.Write(writer);
            PgdFtn.Write(writer);
            BkdFtn.Write(writer);
            AfdFtn.Write(writer);
            PgdEdn.Write(writer);
            BkdEdn.Write(writer);
            AfdEdn.Write(writer);
            Afd.Write(writer);
        }
    }
}
