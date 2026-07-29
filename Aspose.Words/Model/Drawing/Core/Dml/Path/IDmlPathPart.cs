// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    internal interface IDmlPathPart
    {
        DmlPathPartType PathPartType { get; }

        IDmlPathPart Clone();

        void Write(NrxXmlBuilder builder);
    }
}
