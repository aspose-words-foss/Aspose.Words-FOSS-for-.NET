// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/02/2009 by Roman Korchagin

namespace Aspose.Words.BuildingBlocks
{
    /// <summary>
    /// Specifies a building block type. The type might affect the visibility and behavior of the building block
    /// in Microsoft Word.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>Corresponds to the <b>ST_DocPartType</b> type in OOXML.</para>
    /// 
    /// <seealso cref="BuildingBlock.Type"/>
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum BuildingBlockType
    {
        /// <summary>
        /// No type information is specified for the building block.
        /// </summary>
        None,
        /// <summary>
        /// Allows the building block to be automatically inserted into the document whenever 
        /// its name is entered into an application.
        /// </summary>
        AutomaticallyReplaceNameWithContent,
        /// <summary>
        /// The building block is a structured document tag placeholder text.
        /// </summary>
        StructuredDocumentTagPlaceholderText,
        /// <summary>
        /// The building block is a form field help text.
        /// </summary>
        FormFieldHelpText,
        /// <summary>
        /// The building block is a normal (i.e. regular) glossary document entry.
        /// </summary>
        Normal,
        /// <summary>
        /// The building block is associated with the spelling and grammar tools.
        /// </summary>
        AutoCorrect,
        /// <summary>
        /// The building block is an AutoText entry.
        /// </summary>
        AutoText,
        /// <summary>
        /// The building block is associated with all types.
        /// </summary>
        All,

        /// <summary>
        /// Save as <see cref="None"/>.
        /// </summary>
        Default = None
    }
}
