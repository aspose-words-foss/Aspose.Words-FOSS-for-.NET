// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies the possible set of locking behaviors which can be applied to the contents of the 
    /// Structured Document Tag when the contents of this documents are edited by an application (whether 
    /// through a user interface or directly).
    /// </summary>
    internal enum SdtLockType
    {

        /// <summary>
        /// No locking.
        /// </summary>
        Unlocked,

        /// <summary>
        /// Contents cannot be edited at runtime.
        /// </summary>
        ContentLocked,

        /// <summary>
        /// Contents cannot be edited at runtime and SDT cannot be deleted.
        /// </summary>
        SdtAndContentLocked,

        /// <summary>
        /// SDT cannot be deleted.
        /// </summary>
        SdtLocked
    }
}
