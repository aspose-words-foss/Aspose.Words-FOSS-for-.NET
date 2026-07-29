// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/16/2014 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlColorModifiersWriter
    {
        internal static void Write(string prefix, IList<IDmlColorModifier> colorModifiers, IDmlShapeWriterContext writer)
        {
            foreach (IDmlColorModifier modifier in colorModifiers)
                modifier.Write(prefix, writer);
        }
    }
}
