// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2013 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A lightweight structure to pass field code arguments' validation results at once between methods.
    /// </summary>
    /// <dev>
    /// It is immutable and is not intended to be checked on equality, so it should be autoported without any problems.
    /// </dev>
    internal struct FieldIndexValidateResult
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldIndexValidateResult(string errorMessage, FieldCodeIndex fieldCode, Bookmark bookmark)
        {
            ErrorMessage = errorMessage;
            FieldCode = fieldCode;
            Bookmark = bookmark;
        }

        /// <summary>
        /// Gets a value indicating if an INDEX field's code contains any invalid argument.
        /// </summary>
        internal bool IsError
        {
            get { return (ErrorMessage != null); }
        }

        /// <summary>
        /// Gets an error message.
        /// </summary>
        internal string ErrorMessage { get; }

        /// <summary>
        /// Gets a corresponding <see cref="FieldCodeIndex"/> object.
        /// </summary>
        internal FieldCodeIndex FieldCode { get; }

        /// <summary>
        /// Gets a corresponding bookmark.
        /// </summary>
        internal Bookmark Bookmark { get; }
    }
}
