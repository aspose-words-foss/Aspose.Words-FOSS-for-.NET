// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2012 by Denis Darkin

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// The type of revision mark. Note that MS Word VBA defines many different revision types that
    /// are not really documented and I don't know if they are used or not, for example wdRevisionDisplayField.
    ///
    /// Revision types are much simpler in WordML so this enumeration is more similar to WordML than to MS Word VBA.
    /// </summary>
    internal enum EditRevisionType
    {
        Insertion,
        Deletion
    }
}
