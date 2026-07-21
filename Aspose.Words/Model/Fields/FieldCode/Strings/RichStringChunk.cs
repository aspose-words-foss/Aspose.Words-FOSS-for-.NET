// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2020 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>Represents a collection of characters with same formatting.</summary>
    internal class RichStringChunk
    {
        public RichStringChunk(string value, RunPr runPr)
        {
            Value = value;
            RunPr = runPr;
        }

        internal string Value { get; }
        internal RunPr RunPr { get; }
    }
}
