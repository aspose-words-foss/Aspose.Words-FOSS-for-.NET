// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2010 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the type of a keymap customization.
    /// </summary>
    internal enum KeyMapType
    {
        /// <summary>
        /// No command specified.
        /// </summary>
        None,
        /// <summary>
        /// Masks out the built-in command.
        /// </summary>
        Mask,
        /// <summary>
        /// Invokes one of the allocated commands.
        /// </summary>
        AllocatedCommand,
        /// <summary>
        /// Invokes one of the fixed commands.
        /// </summary>
        FixedCommand,
        /// <summary>
        /// Invokes a macro by name.
        /// </summary>
        Macro,
        /// <summary>
        /// Inserts a character.
        /// </summary>
        InsertCharacter,
        /// <summary>
        /// Invokes a legace Word macro by name.
        /// </summary>
        LegacyMacro
    }
}
