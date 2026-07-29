// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS URI value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// URI stands for Uniform Resource Identifiers. See [RFC3986], which includes URLs, URNs, etc.
    /// </para>
    /// <para>
    /// The format of a URI value is 'url(' followed by optional white space followed by an optional single
    /// quote (') or double quote (") character followed by the URI itself, followed by an optional single
    /// quote (') or double quote (") character followed by optional white space followed by ')'.
    /// The two quote characters must be the same.
    /// </para>
    /// </remarks>
    internal class CssUriValue : CssValue
    {
        internal CssUriValue(string uriValue)
            : base(CssValueType.Uri, uriValue)
        {
            Value = uriValue;
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append("url('");
            sb.Append(Value);
            sb.Append("')");
        }

        internal new string Value { get; }
    }
}
