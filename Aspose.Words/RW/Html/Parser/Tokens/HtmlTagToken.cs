// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The token that represents a tag node of HTML markup.
    /// </summary>
    internal class HtmlTagToken : HtmlToken
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the tag node.</param>
        /// <param name="isStart">A flag that determines whether this is a start (<c>true</c>) or an end (<c>false</c>) tag.</param>
        /// <param name="isSelfClosing">Whether this tag is self-closing.</param>
        /// <param name="attributes">The collection of attributes of this tag node.</param>
        internal HtmlTagToken(
            string name,
            bool isStart,
            bool isSelfClosing,
            HtmlAttributeCollection attributes)
            : base(HtmlTokenType.Tag)
        {
            Debug.Assert(StringUtil.HasChars(name));
            Debug.Assert(attributes != null);

            Name = name;
            IsStart = isStart;
            IsSelfClosing = isSelfClosing;
            Attributes = attributes;
        }

        /// <summary>
        /// Gets the name of the tag node.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this is a start (<c>true</c>) or end (<c>false</c>) tag.
        /// </summary>
        internal bool IsStart { get; private set; }

        /// <summary>
        /// Gets the 'self-closing' flag value set by the tokenizer.
        /// </summary>
        internal bool IsSelfClosing { get; private set; }

        /// <summary>
        /// Gets the collection of attributes of this tag node.
        /// </summary>
        internal HtmlAttributeCollection Attributes { get; private set; }
    }
}
