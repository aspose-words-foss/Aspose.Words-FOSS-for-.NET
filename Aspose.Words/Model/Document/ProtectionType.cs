// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2006 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Protection type for a document.
    /// </summary>
    /// <dev>Keep in the Aspose.Words namespace (not in Aspose.Words.Settings) because many clients are using
    /// this enum and it will be a big and unnecessary breaking change.</dev>
    public enum ProtectionType
    {
        /// <summary>
        /// User can only modify comments in the document.
        /// </summary>
        AllowOnlyComments = 1,
        /// <summary>
        /// User can only enter data in the form fields in the document.
        /// </summary>
        AllowOnlyFormFields = 2,
        /// <summary>
        /// User can only add revision marks to the document.
        /// </summary>
        AllowOnlyRevisions = 0,
        /// <summary>
        /// No changes are allowed to the document. Available since Microsoft Word 2003.
        /// </summary>
        ReadOnly = 3,
        /// <summary>
        /// The document is not protected.
        /// </summary>
        NoProtection = -1
    }
}
