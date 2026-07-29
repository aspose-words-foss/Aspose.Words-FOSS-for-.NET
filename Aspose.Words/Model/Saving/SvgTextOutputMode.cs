// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/01/2011 by Alexey Noskov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Allows to specify how text inside a document should be rendered
    /// when saving in SVG format.
    /// </summary>
    public enum SvgTextOutputMode
    {
        /// <summary>
        /// SVG fonts are used to render text. Note, not all browsers support SVG fonts.
        /// </summary>
        UseSvgFonts = 0,
        /// <summary>
        /// Fonts installed on the target machine are used to render text. 
        /// Note, if some of fonts used in the document are not available on the target machine, document can look differently.
        /// </summary>
        UseTargetMachineFonts = 1,
        /// <summary>
        /// Text is rendered using curves. Note, text selection will not work if you use this option.
        /// </summary>
        UsePlacedGlyphs = 2
    }
}
