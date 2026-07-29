// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/12/2014 by Dmitry Matveenko

namespace Aspose.Words.Tables
{
    internal enum GridState
    {
        /// <summary>
        /// Original state before calculation is attempted.
        /// </summary>
        NotCalculated,

        /// <summary>
        /// Set when an unsupported or unexpected situation during grid calculation is encountered.
        /// </summary>
        CalculationFailed,

        /// <summary>
        /// The grid is calculated successfully. 
        /// Table cell and row attributes should be updated accordingly.
        /// </summary>
        Applied,
    }
}
