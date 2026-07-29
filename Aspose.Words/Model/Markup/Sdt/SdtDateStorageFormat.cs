// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2011 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies how the date for a date SDT is stored/retrieved when the SDT is bound to an XML node in the document's data store.
    /// </summary>
    public enum SdtDateStorageFormat
    {
        /// <summary>
        /// The date value for a date SDT is stored as a date in the standard XML Schema Date format.
        /// </summary>
        Date,

        /// <summary>
        /// The date value for a date SDT is stored as a date in the standard XML Schema DateTime format.
        /// </summary>
        DateTime, 
        
        /// <summary>
        ///  The date value for a date SDT is stored as text. 
        /// </summary>
        Text,

        /// <summary>
        /// Defaults to <see cref="DateTime"/>
        /// </summary>
        Default = DateTime
    }
}
