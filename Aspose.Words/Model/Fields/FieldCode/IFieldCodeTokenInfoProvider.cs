// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, provides information about field code tokens encountered while field code parsing.
    /// Should be implemented by fields or field codes that parse field specific arguments and switches.
    /// </summary>
    internal interface IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Returns the type of the specified switch, determining whether the switch is valid for the field which code
        /// is being parsed and whether it has an argument.
        /// </summary>
        FieldSwitchType GetSwitchType(string switchName);
    }
}
