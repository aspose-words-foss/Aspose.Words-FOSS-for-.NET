// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Maps prefixes of declared CSS namespaces to corresponding namespace selectors.
    /// </summary>
    internal class CssNamespacePrefixes
    {
        /// <summary>
        /// Gets a namespace selector for the specified prefix.
        /// </summary>
        /// <param name="prefix">A namespace prefix. Must not be <c>null</c> or empty.</param>
        /// <returns>
        /// The CSS namespace selector that selects the namespace with the specified prefix.
        /// If the namespace prefix is not declared, the method returns <c>null</c>.
        /// </returns>
        internal CssNamespace GetNamespace(string prefix)
        {
            Debug.Assert(StringUtil.HasChars(prefix));

            return mPrefixToNamespaceMap.GetValueOrNull(prefix);
        }

        /// <summary>
        /// Gets the current default namespace for elements that have no namespace declaration.
        /// </summary>
        /// <returns>
        /// The default namespace, if it has been set. Otherwise, <see cref="CssNamespace.Any"/> is returned.
        /// </returns>
        internal CssNamespace DefaultNamespaceForElements
        {
            get
            {
                CssNamespace result;
                if (mPrefixToNamespaceMap.TryGetValue("", out result))
                {
                    return result;
                }
                return CssNamespace.Any;
            }
        }

        /// <summary>
        /// Remembers a CSS namespace declaration.
        /// </summary>
        /// <param name="prefix">
        /// The prefix of the declared CSS namespace. Must not be <c>null</c>. If the prefix is empty, the default namespace
        /// will be set. This prefix is local to rules of the stylesheet that is being parsed.
        /// </param>
        /// <param name="ns">A specific namespace that corresponds to the prefix. The prefix stored in the namespace
        /// is shared among rules of all stylesheets that are parsed by the same instance of the CSS parser.</param>
        internal void SetNamespace(string prefix, CssNamespace ns)
        {
            Debug.Assert(prefix != null);
            Debug.Assert(ns != null);
            Debug.Assert(ns.IsSpecific);

            mPrefixToNamespaceMap[prefix] = ns;
        }

        /// <summary>
        /// Maps declared CSS namespace prefixes to namespace names. Keys are not empty.
        /// </summary>
        private readonly Dictionary<string, CssNamespace> mPrefixToNamespaceMap = new Dictionary<string, CssNamespace>();
    }
}
