// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2021 by Alexander Zhiltsov

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Represents types of revisions that are disabled to track.
    /// </summary>
    [CppOverrideAccessModifier(AccessModifiers.Public)]
    internal enum SuspendedRevisionTypes
    {
        /// <summary>
        /// Indicates that tracking revisions of all types is disabled.
        /// </summary>
        All,
        /// <summary>
        /// Indicates that tracking only move revisions is disabled. Move revisions are represented as
        /// <see cref="RevisionType.Deletion"/> and <see cref="RevisionType.Insertion"/> in this case.
        /// </summary>
        Move
    }
}
