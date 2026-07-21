// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2021 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS media query list.
    /// </summary>
    /// <remarks>
    /// We have implemented a very limited support for media queries. Now we only support "all" and "screen" values.
    /// See https://www.w3.org/TR/css3-mediaqueries/ and <see cref="CssParser.ParseMediaQueryList(string)"/>.
    /// </remarks>
    internal class CssMediaQueryList
    {
        internal CssMediaQueryList(bool isSupported)
        {
            IsSupported = isSupported;
        }

        internal bool IsSupported { get; }

        public override string ToString()
        {
            return IsSupported
                ? "screen"
                : "unsupported";
        }
    }
}
