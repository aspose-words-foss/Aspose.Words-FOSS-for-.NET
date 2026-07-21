// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is a result of an erroneous evaluation.
    /// </summary>
    internal class ErrorConstant : Constant
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="errorMessage"></param>
        internal ErrorConstant(string errorMessage)
        {
            ValueString = errorMessage;
        }

        internal static ErrorConstant CreateSyntaxError()
        {
            return new ErrorConstant("!Syntax Error");
        }

        internal static ErrorConstant CreateBookmarkError(string bookmarkName)
        {
            return new ErrorConstant(string.Format("!Undefined Bookmark, {0}", bookmarkName.ToUpper()));
        }

        internal override string ValueString { get; }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.Error; }
        }
    }
}
