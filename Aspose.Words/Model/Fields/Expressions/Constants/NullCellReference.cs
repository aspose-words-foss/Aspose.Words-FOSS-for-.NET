// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/11/2014 by Edward Voronov

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a reference to cell, which does not exist.
    /// </summary>
    internal class NullCellReference : Constant
    {
        private NullCellReference()
        {
        }

        /// <summary>
        /// Gets the type of the constant.
        /// </summary>
        internal override ConstantType ConstantType
        {
            get { return ConstantType.NullCellReference; }
        }

        internal static readonly NullCellReference Instance = new NullCellReference();
    }
}
