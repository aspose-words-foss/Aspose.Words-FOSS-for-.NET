// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Ss
{
    /// <summary>
    /// Special codes for FAT entries.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class FatEntryType
    {
        /// <summary>
        /// denotes a DIFAT sector in a FAT
        /// </summary>
        internal const uint DifSect = 0xFFFFFFFC;
        /// <summary>
        /// denotes a FAT sector in a FAT
        /// </summary>
        internal const uint FatSect = 0xFFFFFFFD;
        /// <summary>
        /// end of a virtual stream chain
        /// </summary>
        internal const uint EndOfChain = 0xFFFFFFFE;
        /// <summary>
        /// unallocated sector
        /// </summary>
        internal const uint FreeSect = 0xFFFFFFFF;
    }

    /// <summary>
    /// Directory entry type.
    /// </summary>
    internal enum DirEntryType
    {
        /// <summary>
        /// unknown storage type
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// element is a storage object
        /// </summary>
        Storage = 1,
        /// <summary>
        /// element is a stream object
        /// </summary>
        Stream = 2,
        /// <summary>
        /// element is an ILockBytes object
        /// </summary>
        LockBytes = 3,
        /// <summary>
        /// element is an IPropertyStorage object
        /// </summary>
        Property = 4,
        /// <summary>
        /// element is a root storage
        /// </summary>
        Root = 5
    }

    /// <summary>
    /// Directory entry color for binary tree stuff. I don't really use it.
    /// </summary>
    internal enum DirEntryColor
    {
        Red = 0,
        Black = 1
    }
}
