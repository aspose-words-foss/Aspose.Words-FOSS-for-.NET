// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies the type of the OLE link.
    /// </summary>
    internal enum OleLinkType
    {
        /// <summary>
        /// Inserts the linked object as a picture. Default.
        /// </summary>
        Picture = 0,
        /// <summary>
        /// Inserts the linked object as a bitmap.
        /// </summary>
        Bitmap,
        /// <summary>
        /// Inserts the linked object as HTML text.
        /// </summary>
        Html,
        /// <summary>
        /// Inserts the linked object in RTF format.
        /// </summary>
        Rtf,
        /// <summary>
        /// Inserts the linked object in text-only format.
        /// </summary>
        Text,
        /// <summary>
        /// Inserts the linked object as Unicode text.
        /// </summary>
        Unicode,
        //JAVA: moved here to exclude illegal forward reference.
        /// <summary>
        /// Default value is <see cref="Picture"/>.
        /// </summary>
        Default = Picture
    }
}
