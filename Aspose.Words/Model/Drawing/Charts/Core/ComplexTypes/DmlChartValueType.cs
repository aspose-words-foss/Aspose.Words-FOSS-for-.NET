// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    internal enum DmlChartValueType
    {
        /// <summary>
        /// This is special type of values that should not be rendered. It is added for rendering Axis. 
        /// </summary>
        None,
        String,
        Numeric,
        MultiLvlString,
        /// <summary>
        /// Multi-level numeric values of dimension data. 
        /// Such data is stored in elements of the 2.24.3.57 CT_NumericDimension complex type [MS-ODRAWXML].
        /// </summary>
        MultiLvlNumeric
    }
}
