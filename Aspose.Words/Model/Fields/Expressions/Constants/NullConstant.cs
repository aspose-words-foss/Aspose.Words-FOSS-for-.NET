// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant with no value.
    /// </summary>
    internal class NullConstant : Constant
    {
        private NullConstant()
        {
            // Hide from using outside.
        }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.Null; }
        }

        /// <summary>
        /// A singleton instance of the class.
        /// </summary>
        internal static readonly NullConstant Instance = new NullConstant();
    }
}
