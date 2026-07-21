// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2013 by Ivan Lyagin

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies which cleanup actions should be applied to a field after its update.
    /// </summary>
    [Flags]
    internal enum FieldUpdateCleanupActions
    {
        /// <summary>
        /// Specifies that no cleanup should be performed after a field update.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that a field's code should be removed after its update, i.e. the field should be
        /// replaced with its result.
        /// </summary>
        RemoveFieldCode = 1,
        /// <summary>
        /// Specifies that a field's result should be removed after its update.
        /// </summary>
        RemoveFieldResult = 2,
        /// <summary>
        /// Specifies that <see cref="RemoveFieldCode"/> should be applied to all ancestor fields of
        /// a particular field being updated.
        /// </summary>
        RemoveContainingFieldCode = 4,
        /// <summary>
        /// Specifies that a paragraph containing a particular field being updated should be removed
        /// after the field is updated if it will become empty.
        /// </summary>
        RemoveContainingParagraphIfEmpty = 8,
        /// <summary>
        /// Specifies that a paragraph containing a particular field being updated should be removed
        /// after the field is updated if it will contain punctuation marks.
        /// </summary>
        RemoveContainingParagraphWithPunctuationMarks = 16
    }
}
