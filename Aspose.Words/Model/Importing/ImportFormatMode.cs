// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies how formatting is merged when importing content from another document.
    /// </summary>
    /// <remarks>
    /// <p>When you copy nodes from one document to another, this option specifies how formatting
    /// is resolved when both documents have a style with the same name, but different formatting.</p>
    ///
    /// <p>The formatting is resolved as follows:</p>
    /// <list type="number">
    /// <item>Built-in styles are matched using their locale independent style identifier.
    /// User defined styles are matched using case-sensitive style name.</item>
    /// <item>If a matching style is not found in the destination document, the style
    /// (and all styles referenced by it) are copied into the destination document
    /// and the imported nodes are updated to reference the new style.</item>
    /// <item>If a matching style already exists in the destination document, what happens
    /// depends on the <c>importFormatMode</c> parameter passed to
    /// <see cref="DocumentBase.ImportNode(Node, bool, ImportFormatMode)"/>
    /// as described below.</item>
    /// </list>
    ///
    /// <p>When using the <see cref="UseDestinationStyles"/> option, if a matching style already exists
    /// in the destination document, the style is not copied and the imported nodes are updated
    /// to reference the existing style.</p>
    ///
    /// <p>The drawback of using <see cref="UseDestinationStyles"/> is that the imported text might
    /// look different in the destination document comparing to the source document.
    /// For example, the "Heading 1" style in the source document uses Arial 16pt font and
    /// the "Heading 1" style in the destination document uses Times New Roman 14pt font.
    /// When importing text of "Heading 1" style with no other direct formatting, it will
    /// appear as Times New Roman 14pt font in the destination document.</p>
    ///
    /// <p><see cref="KeepSourceFormatting"/> option allows to make sure the imported content looks the same 
    /// in the destination document like it looks in the source document.
    /// If a matching style already exists in the destination document, the source style formatting is expanded
    /// into direct Node attributes and the style is changed to Normal.
    /// If the style does not exist in the destination document, then the source style is imported
    /// into the destination document and applied to the imported node.
    /// Note, that it is not always possible to preserve the source style even if it does not exist in the destination document.
    /// In this case formatting of such style will be expanded into direct Node attributes in favor of preserving original Node formatting.</p>
    /// 
    /// <p>The drawback of using <see cref="KeepSourceFormatting"/> is that if you perform several imports,
    /// you could end up with many styles in the destination document and that could make using
    /// consistent style formatting in Microsoft Word difficult for this document.</p>
    /// 
    /// <p>Using <see cref="KeepDifferentStyles"/> option allows to reuse destination styles
    /// if the formatting they provide is identical to the styles in the source document.
    /// If the style in destination document is different from the source then it is imported.</p>
    /// 
    /// <seealso cref="DocumentBase.ImportNode(Node, bool, ImportFormatMode)"/>
    /// </remarks>
    public enum ImportFormatMode
    {
        /// <summary>
        /// Use the destination document styles and copy new styles. This is the default option.
        /// </summary>
        UseDestinationStyles,

        /// <summary>
        /// Copy all required styles to the destination document, generate unique style names if needed.
        /// </summary>
        KeepSourceFormatting,

        /// <summary>
        /// Only copy styles that are different from those in the source document.
        /// </summary>
        KeepDifferentStyles
    }
}
