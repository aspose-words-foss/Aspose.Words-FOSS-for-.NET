// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents an unsupported version vector (a version number) of <see cref="Feature"/>. We currently support only a subset
    /// of all possible version vector variants (see <see cref="NumericVersionVector"/>. But we have to parse unsupported 
    /// version vectors too, because if we failed to do so, we could fail to parse otherwise valid conditional expressions.
    /// </summary>
    internal class UnsupportedVersionVector : VersionVector
    {
        internal UnsupportedVersionVector(string text)
        {
            Debug.Assert(text != null);
            mText = text;
        }

        internal override bool CompareTo(VersionVector other, ComparisonOperation operation)
        {
            return false;
        }

        public override string ToString()
        {
            return mText;
        }

        private readonly string mText;
    }
}
