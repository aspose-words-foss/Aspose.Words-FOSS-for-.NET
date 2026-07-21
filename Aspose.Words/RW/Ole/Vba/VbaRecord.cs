// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/10/2019 by Alexander Sevidov

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Represents a list of constants, which are used in a VbaProject import/export.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class VbaRecord
    {
        #region VbaModule
        internal const ushort ModuleName = 0x19;
        internal const ushort ModuleNameUnicode = 0x47;
        internal const ushort StreamName = 0x1A;
        internal const ushort StreamNameUnicode = 0x32;
        internal const ushort DocString = 0x1C;
        internal const ushort DocStringUnicode = 0x48;

        internal const ushort Offset = 0x31;
        internal const ushort ModuleHelpContext = 0x1e;

        internal const ushort ModuleCookieRecordId = 0x2c;

        internal const ushort ProceduralModule = 0x0021;
        internal const ushort NonProceduralModule = 0x0022;

        internal const ushort Terminator = 0x2b;
        #endregion

        #region VbaProject
        internal const ushort SysKind = 0x01;
        internal const ushort Lcid = 0x02;
        internal const ushort LcidInvoke = 0x14;
        internal const ushort CodePage = 0x03;
        
        internal const ushort Name = 0x04;
        internal const ushort Description = 0x05;
        internal const ushort DescriptionUnicode = 0x40;
        internal const ushort HelpFilePath1 = 0x06;

        internal const ushort HelpFilePath2 = 0x3d;
        internal const ushort HelpFilePath2Obsolete = 0x49;
        
        internal const ushort ProjectHelpContext = 0x07;
        internal const ushort LibFlags = 0x08;
        internal const ushort ProjectVersion = 0x09;

        internal const ushort Constants = 0x0c;
        internal const ushort ConstantsUnicode = 0x3c;

        internal const ushort ModulesCount = 0x0f;

        internal const ushort ProjectCookieRecordId = 0x13;
        internal const ushort DirTerminator = 0x0010;
        #endregion

        #region VbaReference
        internal const ushort ReferenceName = 0x16;
        internal const ushort ReferenceNameUnicode = 0x3e;
        internal const ushort ReferenceControlId = 0x2f;
        internal const ushort ReferenceReserved = 0x0030; 
        #endregion

        internal const ushort CookieRecord = 0xFFFF;

        #region String constants
        internal const string ProjectStreamName = "PROJECT";
        internal const string DirStreamName = "dir";
        internal const string CacheStreamName = "_VBA_PROJECT";
        internal const string VbaStorageName = "VBA";

        internal const string ProtectionState = "CMG";
        internal const string Password = "DPB";
        internal const string VisibilityState = "GC";
        internal const string Id = "ID";

        #endregion
    }
}
