// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2016 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Interface for visiting parts of pseudo-elements' content.
    /// </summary>
    internal interface IPseudoElementContentPartVisitor
    {
        void VisitAttr(string attributeName);
        void VisitCounter(string counterName, NumberStyle counterStyle);
        void VisitCounters(string counterName, string separator, NumberStyle counterStyle);
        void VisitText(string text);
    }
}
