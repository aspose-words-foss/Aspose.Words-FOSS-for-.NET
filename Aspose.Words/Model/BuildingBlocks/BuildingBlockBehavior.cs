// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/02/2009 by Roman Korchagin

namespace Aspose.Words.BuildingBlocks
{
    /// <summary>
    /// Specifies the behavior that shall be applied to the contents of the building block
    /// when it is inserted into the main document.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>Corresponds to the <b>ST_DocPartBehavior</b> type in OOXML.</para>
    /// 
    /// <seealso cref="BuildingBlock.Behavior"/>
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum BuildingBlockBehavior
    {
        /// <summary>
        /// Specifies that the building block shall be inserted as inline content.
        /// </summary>
        Content,
        /// <summary>
        /// Specifies that the building block shall be inserted into its own paragraph.
        /// </summary>
        Paragraph,
        /// <summary>
        /// Specifies that the building block shall be added into its own page.
        /// </summary>
        Page,

        /// <summary>
        /// Same as <see cref="Content"/>.
        /// </summary>
        Default = Content
    }
}
