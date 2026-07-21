// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is aggregate (a list of other constants).
    /// </summary>
    internal class AggregateConstant : Constant
    {
        internal AggregateConstant()
        {
            Values = new ConstantCollection();
        }

        /// <summary>
        /// Gets a list of contained values.
        /// </summary>
        internal ConstantCollection Values { get; }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.Aggregate; }
        }
   }
}
