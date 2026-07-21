// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The token that represents a DOCTYPE node.
    /// </summary>
    internal class HtmlDoctypeToken : HtmlToken
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the DOCTYPE.</param>
        /// <param name="publicId">The public identifier.</param>
        /// <param name="systemId">The system identifier.</param>
        /// <param name="forceQuirks">The 'force-quirks' flag value.</param>
        internal HtmlDoctypeToken(string name, string publicId, string systemId, bool forceQuirks)
            : base(HtmlTokenType.Doctype)
        {
            Debug.Assert(name != null);

            mName = name;
            mPublicId = publicId;
            mSystemId = systemId;
            mForceQuirks = forceQuirks;
        }

        /// <summary>
        /// Gets the name of the DOCTYPE.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the public identifier of the DOCTYPE.
        /// </summary>
        internal string PublicId
        {
            get { return mPublicId; }
        }

        /// <summary>
        /// Gets the system identifier of the DOCTYPE.
        /// </summary>
        internal string SystemId
        {
            get { return mSystemId; }
        }

        /// <summary>
        /// Gets the 'force-quirks' flag value set by the tokenizer.
        /// </summary>
        internal bool ForceQuirks
        {
            get { return mForceQuirks; }
        }

        private readonly string mName;

        private readonly string mPublicId;

        private readonly string mSystemId;

        private readonly bool mForceQuirks;
    }
}