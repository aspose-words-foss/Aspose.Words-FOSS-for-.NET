// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2011 by Dmitry Matveenko

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Indicates what culture to use during field update.
    /// </summary>
    public enum FieldUpdateCultureSource
    {
        /// <summary>
        /// The culture of the current execution thread is used to update fields.
        /// </summary>
        CurrentThread,

        /// <summary>
        /// The culture specified in the field formatting properties via language setting is used.
        /// </summary>
        /// <remarks>
        /// To be exact, Aspose.Words mimics MS Word by using the language set for the first character of the field code.
        /// </remarks>
        /// <seealso cref="Field.LocaleId"/>
        FieldCode
    }
}
