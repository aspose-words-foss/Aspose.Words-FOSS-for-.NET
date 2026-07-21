// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2011 by Alexey Titov

using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// Base class for dashes.
    /// </summary>
    internal abstract class DmlDash
    {
        internal abstract DmlDash Clone();

        internal abstract DmlDashType DashType { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get; }
    }
}
