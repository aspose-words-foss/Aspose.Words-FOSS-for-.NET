// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

using Aspose.Words.RW.Html.Parser.IEConditionalExpressions;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents a feature supported by HTML import.
    /// </summary>
    internal class Feature
    {
        /// <summary>
        /// Creates a new feature with no specific version.
        /// </summary>
        /// <param name="name">A feature name. Feature names are case-insensitive.</param>
        internal Feature(string name)
        {
            Debug.Assert(StringUtil.HasChars(name));

            mName = name.ToLowerInvariant();
        }

        /// <summary>
        /// Creates a new feature and initializes it with specified values.
        /// </summary>
        /// <param name="name">A feature name. Feature names are case-insensitive.</param>
        /// <param name="majorVersion">The major part of a feature version. Must be non-negative.</param>
        /// <param name="minorVersion">The minor part of a feature version. Must be non-negative.</param>
        internal Feature(string name, int majorVersion, int minorVersion)
            : this(name)
        {
            Debug.Assert(majorVersion >= 0);
            Debug.Assert(minorVersion >= 0);

            mVersion = new NumericVersionVector(majorVersion, minorVersion);
        }

        /// <summary>
        /// Gets the feature name. Names are always in lower case.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the version of the feature.
        /// </summary>
        internal NumericVersionVector Version
        {
            get { return mVersion; }
        }

        public override string ToString()
        {
            return (mVersion != null)
                ? mName + " " + mVersion
                : mName;
        }

        private readonly string mName;
        private readonly NumericVersionVector mVersion;
    }
}
