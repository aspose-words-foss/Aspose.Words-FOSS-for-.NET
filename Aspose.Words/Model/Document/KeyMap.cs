// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2010 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Describes an individual key mapping.  Attributes collectively specify a sequence of key combinations that, 
    /// when pressed, will cause the execution of an action such as a fixed command or macro in MS Word.
    /// 
    /// The key combination for the mapping may be specified using KCM- (key code with modifiers) and CHM- (character with modifiers) based attributes.
    /// </summary>
    internal class KeyMap
    {
        /// <summary>
        /// Specifies the first CHM key combination in the sequence.
        /// </summary>
        internal int ChmPrimary;
        /// <summary>
        /// Specifies the second CHM key combination in the sequence.
        /// </summary>
        internal int ChmSecondary;
        /// <summary>
        /// Specifies the first key combination in the sequence.
        /// </summary>
        internal int KcmPrimary;
        /// <summary>
        /// Specifies the second key combination in the sequence.
        /// </summary>
        internal int KcmSecondary;

        /// <summary>
        /// Specifies the type of the keymap. Affects how to interpret data stored in the fields below.
        /// </summary>
        internal KeyMapType KeymapType;

        /// <summary>
        /// Zero based index into the <see cref="Document.AllocatedCommands"/> collection that specifies the command.
        /// Valid when <see cref="KeyMapType.AllocatedCommand"/>
        /// </summary>
        internal int AllocatedCommandIndex;

        /// <summary>
        /// Valid when <see cref="KeyMapType.FixedCommand"/>.
        /// </summary>
        internal FixedCommandIdentifier FixedCommandIdentifier;

        /// <summary>
        /// Valid when <see cref="KeyMapType.FixedCommand"/>.
        /// </summary>
        internal int FixedCommandArgument;

        /// <summary>
        /// Valid when <see cref="KeyMapType.Macro"/> or <see cref="KeyMapType.LegacyMacro"/>.
        /// </summary>
        internal string MacroName;

        /// <summary>
        /// Valid when <see cref="KeyMapType.InsertCharacter"/>.
        /// </summary>
        internal int CharacterCode;
    }
}
