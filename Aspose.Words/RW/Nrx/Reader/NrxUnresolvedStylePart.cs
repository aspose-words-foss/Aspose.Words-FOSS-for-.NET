// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2008 by Roman Korchagin

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// VA I cannot resolve some attributes of the style when reading styles because I need istds
    /// and all istds are known only after all styles are loaded.
    /// This class temporarily stores the attributes I cannot resolve until all styles are read.
    /// </summary>
    internal class NrxUnresolvedStylePart
    {
        internal NrxUnresolvedStylePart(Style style)
        {
            Debug.Assert(style != null);
            Style = style;
        }

        /// <summary>
        /// The style that needs resolution.
        /// </summary>
        internal Style Style;
        /// <summary>
        /// Unresolved "basedOn" attribute. Can be null.
        /// The type of the value is up to the importer. 
        /// This is string for DOCX and WordML, but int for RTF.
        /// </summary>
        internal object BasedOn;
        /// <summary>
        /// Unresolved "next" attribute. Can be null.
        /// This is string for DOCX and WordML, but int for RTF.
        /// </summary>
        internal object Next;
        /// <summary>
        /// Unresolved "link" attribute. Can be null.
        /// This is string for DOCX and WordML, but int for RTF.
        /// </summary>
        internal object Link;
    }
}
