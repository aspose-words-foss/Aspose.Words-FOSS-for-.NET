// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies the type of a structured document tag (SDT) node.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum SdtType
    {
        /// <summary>
        /// No type is assigned to the SDT.
        /// </summary>
        None,

        /// <summary>
        /// The SDT represents a bibliography entry. 
        /// </summary>
        Bibliography,

        /// <summary>
        /// The SDT represents a citation.
        /// </summary>
        Citation,

        /// <summary>
        /// The SDT represents an equation.
        /// </summary>
        Equation,

        /// <summary>
        /// The SDT represents a drop down list when displayed in the document.
        /// </summary>
        DropDownList,

        /// <summary>
        /// The SDT represents a combo box when displayed in the document.
        /// </summary>
        ComboBox,

        /// <summary>
        /// The SDT represents a date picker when displayed in the document.
        /// </summary>
        Date,

        /// <summary>
        /// The SDT represents a building block gallery type.
        /// </summary>
        BuildingBlockGallery,

        /// <summary>
        /// The SDT represents a document part type.
        /// </summary>
        DocPartObj,

        /// <summary>
        /// The SDT represents a restricted grouping when displayed in the document.
        /// </summary>
        Group,

        /// <summary>
        /// The SDT represents a picture when displayed in the document.
        /// </summary>
        Picture,

        /// <summary>
        /// The SDT represents a rich text box when displayed in the document.
        /// </summary>
        RichText,

        /// <summary>
        /// The SDT represents a plain text box when displayed in the document.
        /// </summary>
        PlainText,

        /// <summary>
        /// The SDT represents a checkbox when displayed in the document.
        /// </summary>
        /// <remarks>
        /// This is MS-specific feature available since Office 2010 and not supported by the ISO/IEC 29500 OOXML standard.
        /// </remarks>
        Checkbox,

        /// <summary>
        /// The SDT represents repeating section type when displayed in the document.
        /// </summary>
        /// <remarks>
        /// This is MS-specific feature available since Office 2013 and not supported by the ISO/IEC 29500 OOXML standard.
        /// </remarks>
        RepeatingSection,

        /// <summary>
        /// The SDT represents repeating section item.
        /// </summary>
        /// <remarks>
        /// This is MS-specific feature available since Office 2013 and not supported by the ISO/IEC 29500 OOXML standard.
        /// </remarks>
        RepeatingSectionItem,

        /// <summary>
        /// The SDT represents an entity picker that allows the user to select an instance of an external content type.
        /// </summary>
        /// <remarks>
        /// This is MS-specific feature available since Office 2010 and not supported by the ISO/IEC 29500 OOXML standard.
        /// </remarks>
        /// <dev>
        /// §2.6.1.15 entityPicker [MS-DOCX].
        /// Some additional information about entity pickers can be taken from the chapter USING EXTERNAL DATA IN WORD
        /// of Scot Hillier, Brad Stevenson. Professional Business Connectivity Services in SharePoint 2010.
        /// </dev>
        EntityPicker
    }
}
