// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/02/2017 by Alexey Butalov

using Aspose.JavaAttributes;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// This interface is used to break the direct dependency Aspose.Words from RW.Docx.
    /// We use a stub implementation of IDefaultThemeProvider in C++ branches until RW.Docx isn't ported to C++. 
    /// </summary>
    internal interface IDefaultThemeProvider
    {
        /// <summary>
        /// Reads default theme.
        /// </summary>
        [JavaThrows(true)]
        Theme GetDefaultTheme();
    }
}
