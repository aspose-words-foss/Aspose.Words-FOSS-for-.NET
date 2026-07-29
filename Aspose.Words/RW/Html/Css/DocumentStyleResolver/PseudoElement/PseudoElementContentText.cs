// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a text string, which is a souce of generated content for pseudo-elements.
    /// </summary>
    internal class PseudoElementContentText : PseudoElementContentPart
    {
        internal PseudoElementContentText(string text)
        {
            Debug.Assert(text != null);
            mText = text;
        }

        internal override void Accept(IPseudoElementContentPartVisitor visitor)
        {
            visitor.VisitText(mText);
        }

#if DEBUG
        public override string ToString()
        {
            return "\"" + mText + "\"";
        }
#endif

        private readonly string mText;
    }
}
