// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2012 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Doc
{
    /// <summary>
    /// Implements [MS-DOC] 2.5.10 FibRgFcLcb2007 structure.
    /// </summary>
    internal class RgFcLcb2007
    {

        // ReSharper disable InconsistentNaming

        // Currently unused, hide in Release.

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Plcfmthd = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        /// <remarks>
        /// Seems to be reserved for new kind of revisions "Move revision" introduced in DOCX.
        /// But it also seems that MS decided to reject all new features in DOC. At least Move revisions are not
        /// supported in DOC - Word converts them into Insert/Delete pair. Maybe it's need to be investigated.
        /// </remarks>
        internal readonly FcLcb SttbfBkmkMoveFrom = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb PlcfBkfMoveFrom = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb PlcfBklMoveFrom = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb SttbfBkmkMoveTo = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb PlcfBkfMoveTo = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb PlcfBklMoveTo = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Unused1 = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Unused2 = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Unused3 = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb SttbfBkmkArto = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb PlcfBkfArto = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb PlcfBklArto = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb ArtoData = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Unused4 = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Unused5 = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value MUST be zero, and MUST be ignored.
        /// </summary>
        internal readonly FcLcb Unused6 = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value SHOULD be zero, and MUST be ignored.
        /// </summary>
        /// <remarks>
        /// Neither Office Word 2007, Word 2010, nor Word 15 Technical Preview write 0 here, 
        /// but all three ignore this value when loading files.
        /// 
        /// Undocumented Table, Slot:181, Start:2F22, Length:C09
        /// This is a ZIP (an OPC Package) file that contains the theme for the document.
        /// 50 4B 03 04 14 00 06 00 08 00 00 00 21 00 82 8A BC 13 FA ...
        /// </remarks>
        internal FcLcb OssTheme = new FcLcb();

        /// <summary>
        /// Fc value is undefined and MUST be ignored.
        /// Lcb value SHOULD be zero, and MUST be ignored.
        /// </summary>
        /// <remarks>
        /// Neither Office Word 2007, Word 2010, nor Word 15 Technical Preview write 0 here, 
        /// but all three ignore this value when loading files.
        /// </remarks>
        internal FcLcb ColorSchemeMapping = new FcLcb();

        // ReSharper enable InconsistentNaming

        internal void Read(BinaryReader reader)
        {
            // Read this only for testing purposes.
            Plcfmthd.Read(reader);
            SttbfBkmkMoveFrom.Read(reader);
            PlcfBkfMoveFrom.Read(reader);
            PlcfBklMoveFrom.Read(reader);
            SttbfBkmkMoveTo.Read(reader);
            PlcfBkfMoveTo.Read(reader);
            PlcfBklMoveTo.Read(reader);
            Unused1.Read(reader);
            Unused2.Read(reader);
            Unused3.Read(reader);
            SttbfBkmkArto.Read(reader);
            PlcfBkfArto.Read(reader);
            PlcfBklArto.Read(reader);
            ArtoData.Read(reader);
            Unused4.Read(reader);
            Unused5.Read(reader);
            Unused6.Read(reader);
            OssTheme.Read(reader);
            ColorSchemeMapping.Read(reader);
        }

        internal void Write(BinaryWriter writer)
        {
            Plcfmthd.Write(writer);
            SttbfBkmkMoveFrom.Write(writer);
            PlcfBkfMoveFrom.Write(writer);
            PlcfBklMoveFrom.Write(writer);
            SttbfBkmkMoveTo.Write(writer);
            PlcfBkfMoveTo.Write(writer);
            PlcfBklMoveTo.Write(writer);
            Unused1.Write(writer);
            Unused2.Write(writer);
            Unused3.Write(writer);
            SttbfBkmkArto.Write(writer);
            PlcfBkfArto.Write(writer);
            PlcfBklArto.Write(writer);
            ArtoData.Write(writer);
            Unused4.Write(writer);
            Unused5.Write(writer);
            Unused6.Write(writer);
            OssTheme.Write(writer);
            ColorSchemeMapping.Write(writer);
        }
    }
}
