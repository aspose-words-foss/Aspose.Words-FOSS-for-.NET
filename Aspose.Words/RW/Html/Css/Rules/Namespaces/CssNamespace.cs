// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// CSS namespace selector that selects CSS elements and attributes by their namespaces.
    /// </summary>
    internal class CssNamespace
    {
        internal CssNamespace(string prefix, string namespaceName)
        {
            Prefix = prefix;
            Name = namespaceName;
        }

        internal bool Matches(string namespaceName)
        {
            // Any namespace.
            if (Name == null)
            {
                return true;
            }

            // Use ordinal comparison (case-sensitive). This also covers the empty namespace case.
            return Name == namespaceName;
        }

        internal string GetPrefixedName(string localName, string defaultNamespaceName)
        {
            if (Name == defaultNamespaceName)
            {
                return localName;
            }

            if (Name == null)
            {
                return "*|" + localName;
            }

            if (Name.Length == 0)
            {
                return "|" + localName;
            }

            return Prefix + "|" + localName;
        }

        internal string Prefix { get; }

        internal string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this namespace selector matches a specific namespace (not any namespace).
        /// </summary>
        internal bool IsSpecific
        {
            get
            {
                // Note that the empty namespace, whose name is an empty string, is considered specific too.
                return Name != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this namespace selector matches no namespace.
        /// </summary>
        internal bool IsEmpty
        {
            get { return Name == ""; }
        }

        internal static readonly CssNamespace Any = new CssNamespace(null, null);

        internal static readonly CssNamespace Empty = new CssNamespace(null, "");
    }
}
