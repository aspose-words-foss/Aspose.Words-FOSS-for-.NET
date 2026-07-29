// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

namespace Aspose.Common
{
    /// <summary>Represents a character.</summary>
    public interface IChar
    {
        /// <summary>Converts the value of this instance to its equivalent char representation.</summary>
        char ToSystemChar();

        /// <summary>Converts the value of this instance to its uppercase equivalent.</summary>
        IChar ToUpper();

        /// <summary>Converts the value of this instance to its lowercase equivalent.</summary>
        IChar ToLower();
    }
}
