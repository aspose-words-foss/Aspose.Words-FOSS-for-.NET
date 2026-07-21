// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2015 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Represents piece of Ruby.
    /// </summary>
    internal class RubyChunk
    {
        public override int GetHashCode()
        {
            int hashCode = (Text != null) ? Text.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ ((RunPr != null) ? RunPr.GetHashCode() : 0);
            return hashCode;
        }

        internal string Text;
        internal RunPr RunPr = new RunPr();
    }
}
