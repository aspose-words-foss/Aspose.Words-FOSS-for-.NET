// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2019 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Parses CSS: stylesheets, declarations, or values.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/css-syntax-3/
    /// </remarks>
    internal class CssParser
    {
        private CssParser(
            CssDocumentMode documentMode,
            string stylesheetUri,
            CssNamespacePrefixResolver multipleStylesheetNamespacePrefixResolver)
        {
            if (stylesheetUri != null)
            {
                string stylesheetBaseUri = UriUtil.GetDirectoryHref(stylesheetUri);
                if (StringUtil.HasChars(stylesheetBaseUri) && (stylesheetBaseUri != "."))
                {
                    mStylesheetBaseUri = stylesheetBaseUri;
                }
            }

            mIsInQuirksMode = documentMode == CssDocumentMode.Quirks;
            mMultipleStylesheetNamespacePrefixResover = multipleStylesheetNamespacePrefixResolver;
        }

        /// <summary>
        /// Parses a CSS stylesheet.
        /// </summary>
        internal static IList<CssRule> ParseStandaloneStylesheet(
            string css,
            CssDocumentMode documentMode)
        {
            CssParser parser = new CssParser(
                documentMode,
                "",
                new CssNamespacePrefixResolver());
            return parser.ParseRuleList(new CssTokenizerTokenSource(css));
        }

        /// <summary>
        /// Parses a CSS stylesheet. Makes sure namespaces of this stylesheet don't re-use prefixes of namespaces imported by
        /// other stylesheets.
        /// </summary>
        internal static IList<CssRule> ParseStylesheet(
            string css,
            CssDocumentMode documentMode,
            string stylesheetUri,
            CssNamespacePrefixResolver multipleStylesheetNamespacePrefixResolver)
        {
            CssParser parser = new CssParser(
                documentMode,
                stylesheetUri,
                multipleStylesheetNamespacePrefixResolver);
            return parser.ParseRuleList(new CssTokenizerTokenSource(css));
        }

        /// <summary>
        /// Parses a CSS declaration list. For example, content of a HTML "style" attribute.
        /// </summary>
        internal static CssDeclarationCollection ParseDeclarations(string css, CssDocumentMode documentMode)
        {
            // CSS namespaces are used in CSS selectors only. Since CSS declarations cannot contain selectors, it is okay that
            // here we don't pass any CSS namespace prefix collector to the CSS parser.
            CssParser parser = new CssParser(
                documentMode,
                "",
                null);

            // Note that if at-rules are expected, they are allowed to end with {}-blocks and have no semicolon character
            // at the end. Most modern browsers, however, don't follow the CSS specification and require all components
            // of a declaration list to be separated by semicolon characters.
            // According to the CSS specification, the following declaration list:
            //   "@media screen { color: blue } color: red"
            // must be equivalent to:
            //   "color: red"
            // However, modern browsers don't follow the specification and consider such a declaration list totally invalid.
            // We mimic the browsers' behavior.
            return parser.ConsumeDeclarationList(new CssTokenizerTokenSource(css), false);
        }

        /// <summary>
        /// Parses an individual CSS value.
        /// </summary>
        internal static CssValue ParseValue(string css)
        {
            CssTokenSource tokenSource = new CssTokenizerTokenSource(css);
            tokenSource.ConsumeOptionalWhitespace();
            CssValue value = ParseValue(tokenSource, "");

            if ((value == null) ||
                (value.ValueType == CssValueType.Comma) ||
                (value.ValueType == CssValueType.Solidus))
            {
                return null;
            }

            return tokenSource.ConsumeWhitespaceAndToken().IsEof()
                ? value
                : null;
        }

        /// <summary>
        /// Parses a space-separated list of CSS values.
        /// </summary>
        internal static CssValueList ParseValueList(string css)
        {
            CssTokenizerTokenSource tokenSource = new CssTokenizerTokenSource(css);
            CssValueList result = ParseSpaceSeparatedValueList(tokenSource, "");
            return result;
        }

        /// <summary>
        /// Parses a media query string.
        /// </summary>
        internal static CssMediaQueryList ParseMediaQueryList(string mediaQuery)
        {
            return ConsumeMediaQueryList(new CssTokenizerTokenSource(mediaQuery));
        }

        /// <summary>
        /// Parses a stylesheet (list of rules).
        /// </summary>
        private List<CssRule> ParseRuleList(CssTokenSource tokenSource)
        {
            while (tokenSource.HasMoreTokens())
            {
                CssToken token = tokenSource.Consume();

                if (mRuleLists.IsTopLevel && ((token.Type == CssTokenType.Cdc) || (token.Type == CssTokenType.Cdo)))
                {
                    // HTML comment tokens '<--' and '-->' are ignored at the top level of stylesheets.
                    continue;
                }

                if (!mRuleLists.IsTopLevel && (token.Type == CssTokenType.BlockBracketRight))
                {
                    // Closing block brackets end nested rule lists if they appear at a nested level (not at the top level).
                    EndNestedMediaRule();
                    continue;
                }

                switch (token.Type)
                {
                    case CssTokenType.Whitespace:
                    {
                        // Skip whitespace.
                        break;
                    }
                    case CssTokenType.AtKeyword:
                    {
                        // at-rule names are ASCII case-insensitive.
                        string name = ((CssTextToken)token).Text;
                        name = StringUtil.AsciiLowerCase(name);

                        // WORDSNET-24021 @media rules are processed in a special way, because they contain nested rule
                        // lists. In order to avoid stack overflow exceptions we cannot parse those nested rule lists
                        // recursively and we have to use an iterative approach.
                        if (name == "media")
                        {
                            List<CssToken> prelude = ConsumeAtRulePrelude(tokenSource);
                            CssToken blockStartToken = tokenSource.Consume(); // ';', '{', or EOF.

                            // Semicolon and EOF tokens end a @media rule right away. Such a rule is malformed since it has
                            // no block, so we skip it. We process only @media rules that do have a block.
                            if (blockStartToken.Type == CssTokenType.BlockBracketLeft)
                            {
                                BeginNestedMediaRule(prelude);
                            }
                        }
                        else
                        {
                            tokenSource.Reconsume();
                            CssRule atRule = ConsumeAtRule(tokenSource, mRuleLists.IsTopLevel);
                            AddRuleToCurrentList(atRule);
                        }
                        break;
                    }
                    default:
                    {
                        tokenSource.Reconsume();
                        CssStyleRule styleRule = ConsumeStyleRule(tokenSource);
                        AddRuleToCurrentList(styleRule);
                        break;
                    }
                }
            }

            // At EOF, CSS parsers silently close all nested rule lists that are not closed correctly and don't have closing
            // '}' brackets.
            while (!mRuleLists.IsTopLevel)
            {
                EndNestedMediaRule();
            }

            return mRuleLists.CurrentRuleList;
        }

        private void BeginNestedMediaRule(List<CssToken> prelude)
        {
            CssMediaQueryList mediaQueryList = ConsumeMediaQueryList(new CssListTokenSource(prelude));
            mRuleLists.BeginNestedMediaRule(mediaQueryList);

            // @import and @namespace rules must precede any well-formed @media rules.
            mCanProcessMoreImportRules = false;
            mCanProcessMoreNamespaceRules = false;
        }

        private void EndNestedMediaRule()
        {
            CssMediaRule mediaRule = BuildMediaRule(mRuleLists.CurrentMediaQueryList, mRuleLists.CurrentRuleList);
            mRuleLists.EndNestedMediaRule();
            AddRuleToCurrentList(mediaRule);
        }

        private void AddRuleToCurrentList(CssRule rule)
        {
            if (rule == null)
            {
                return;
            }

            mRuleLists.CurrentRuleList.Add(rule);

            // Note that we don't check for @charset here, because our parser doesn't support @charset rules
            // and always ignores them.
            if (mRuleLists.IsTopLevel && !(rule is CssImportRule))
            {
                // @import rules must precede other well-formed rules except @charset and other @import rules.
                mCanProcessMoreImportRules = false;
                // @namespace rules must precede other well-formed rules except @charset, @import, and other @namespace rules.
                mCanProcessMoreNamespaceRules = false;
            }
        }

        private CssStyleRule ConsumeStyleRule(CssTokenSource tokenSource)
        {
            List<CssToken> selectorTokens = new List<CssToken>();
            bool reachedEof = false;
            do
            {
                CssToken token = tokenSource.Consume();
                switch (token.Type)
                {
                    case CssTokenType.Eof:
                        tokenSource.Reconsume();
                        reachedEof = true;
                        break;
                    case CssTokenType.BlockBracketLeft:
                        List<CssToken> declarationTokens = new List<CssToken>();
                        ConsumeBlock(tokenSource, declarationTokens);
                        return BuildStyleRule(selectorTokens, declarationTokens);
                    default:
                        tokenSource.Reconsume();
                        ConsumeComponent(tokenSource, selectorTokens);
                        break;
                }
            } while (!reachedEof);

            // Reached end of file.
            return null;
        }

        private CssRule ConsumeAtRule(CssTokenSource tokenSource, bool isTopLevel)
        {
            // at-rule names are ASCII case-insensitive.
            string name = ((CssTextToken)tokenSource.Consume()).Text;
            name = StringUtil.AsciiLowerCase(name);

            List<CssToken> prelude = ConsumeAtRulePrelude(tokenSource);
            CssToken blockStartToken = tokenSource.Consume(); // ';', '}', or EOF.

            // Consume the block if there is any.
            List<CssToken> block = null;
            if (blockStartToken.Type == CssTokenType.BlockBracketLeft)
            {
                block = new List<CssToken>();
                ConsumeBlock(tokenSource, block);
            }

            CssRule atRule = ParseAtRule(name, prelude, block, isTopLevel);
            return atRule;
        }

        private static List<CssToken> ConsumeAtRulePrelude(CssTokenSource tokenSource)
        {
            List<CssToken> prelude = new List<CssToken>();
            CssToken token = tokenSource.Consume();
            while ((token.Type != CssTokenType.Eof) &&
                (token.Type != CssTokenType.Semicolon) &&
                (token.Type != CssTokenType.BlockBracketLeft))
            {
                tokenSource.Reconsume();
                ConsumeComponent(tokenSource, prelude);
                token = tokenSource.Consume();
            }
            tokenSource.Reconsume();
            return prelude;
        }

        private static void SkipAtRule(CssTokenSource tokenSource)
        {
            // Any at-rule starts with an at-token followed by a possibly empty prelude (sequence of components) and is ended
            // with a semicolon, EOF, or a '{'-block.
            bool atRuleEndReached = false;
            while (!atRuleEndReached)
            {
                CssToken token = tokenSource.Consume();
                switch (token.Type)
                {
                    case CssTokenType.Semicolon:
                    case CssTokenType.Eof:
                        // This at-rule ends with a semicolon or EOF.
                        atRuleEndReached = true;
                        break;
                    case CssTokenType.BlockBracketLeft:
                        // This at-rule ends with a block. Skip it too.
                        ConsumeBlock(tokenSource, null);
                        atRuleEndReached = true;
                        break;
                    default:
                        // Skip prelude components.
                        tokenSource.Reconsume();
                        ConsumeComponent(tokenSource, null);
                        break;
                }
            }
        }

        private CssDeclarationCollection ConsumeDeclarationList(CssTokenSource tokenSource, bool expectNestedAtRules)
        {
            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();
            while (true)
            {
                CssToken token = tokenSource.ConsumeWhitespaceAndToken();
                if (token.IsEof())
                {
                    break;
                }

                if (expectNestedAtRules && (token.Type == CssTokenType.AtKeyword))
                {
                    // This might be, for example, a margin at-rule (like @top-left) nested in a @page rule.
                    // We don't support at-rules in declaration lists so we skip the rule.
                    SkipAtRule(tokenSource);
                }
                else
                {
                    switch (token.Type)
                    {
                        case CssTokenType.Semicolon:
                        {
                            // Empty declaration. Skip it.
                            break;
                        }
                        case CssTokenType.Ident:
                        {
                            CssDeclarationCollection declarations = ConsumeAndParseDeclaration(
                                tokenSource,
                                ((CssTextToken)token).Text);
                            if (declarations != null)
                            {
                                result.AddOrReplaceIfMoreOrEquallyImportant(declarations);
                            }
                            break;
                        }
                        default:
                        {
                            tokenSource.Reconsume();
                            while (true)
                            {
                                CssToken nextToken = tokenSource.Consume();
                                if ((nextToken.Type == CssTokenType.Semicolon) || nextToken.IsEof())
                                {
                                    break;
                                }
                                tokenSource.Reconsume();
                                ConsumeComponent(tokenSource, null);
                            }
                            break;
                        }
                    }
                }
            }

            return result.GetDeclarations();
        }

        private CssDeclarationCollection ConsumeAndParseDeclaration(CssTokenSource tokenSource, string name)
        {
            Debug.Assert(StringUtil.HasChars(name));

            // A colon character between the name and the value of a property.
            CssToken token = tokenSource.ConsumeWhitespaceAndToken();
            if (token.Type != CssTokenType.Colon)
            {
                if ((token.Type != CssTokenType.Semicolon) && !token.IsEof())
                {
                    tokenSource.Reconsume();
                }
                return null;
            }

            // Tokens that constitute the value of the property.
            List<CssToken> value = new List<CssToken>();
            ConsumeComponentSequence(tokenSource, CssTokenType.Semicolon, value);
            tokenSource.Consume(); // ';' or EOF

            // Check if the declaration is important. Look for trailing "!important" tokens with optional whitespace and remove
            // them if found.
            bool isImportant = ParseAndRemoveImportantKeyword(value);

            return ParseDeclaration(name, value, isImportant);
        }

        private static bool ParseAndRemoveImportantKeyword(List<CssToken> valueTokens)
        {
            // Search backward for the "important" keyword.
            int i = valueTokens.Count - 1;
            for (; i >= 0; i--)
            {
                CssToken valueToken = valueTokens[i];
                if (valueToken.Type == CssTokenType.Ident)
                {
                    string asciiCaseInsensitiveText = StringUtil.AsciiLowerCase(((CssTextToken)valueToken).Text);
                    if (asciiCaseInsensitiveText == "important")
                    {
                        break;
                    }
                }
                if (!valueToken.IsWhitespace())
                {
                    // Stop parsing if we meet anything but "important" or whitespace.
                    return false;
                }
            }

            // Return if the value contains only the "important" identifier followed by whitespace.
            i--;
            if (i < 0)
            {
                return false;
            }

            // Search backward for the "!" character.
            for (; i >= 0; i--)
            {
                CssToken valueToken = valueTokens[i];
                if (valueToken.IsDelim('!'))
                {
                    // Remove the "!important" part from the value.
                    valueTokens.RemoveRange(i, valueTokens.Count - i);
                    return true;
                }
                if (!valueToken.IsWhitespace())
                {
                    // Stop parsing if we meet anything but "!" or whitespace.
                    return false;
                }
            }

            return false;
        }

        private CssRule ParseAtRule(
            string name,
            List<CssToken> preludeTokens,
            List<CssToken> blockTokens,
            bool isTopLevel)
        {
            // at-rule names are ASCII case-insensitive.
            Debug.Assert(StringUtil.IsAsciiLowerCase(name));

            switch (name)
            {
                case "import":
                {
                    if (mCanProcessMoreImportRules)
                    {
                        return ParseImportRule(preludeTokens, blockTokens, mStylesheetBaseUri);
                    }
                    // @import rules are ignored if they come after other rules.
                    return null;
                }
                case "namespace":
                {
                    // Note that @namespace rules are processed internally by the parser in order to make declared
                    // namespaces available to parsing of style rules.
                    // @namespace rules cannot be nested inside other at-rules and thus they are ignored if we're parsing
                    // a nested (not top-level) stylesheet.
                    if (mCanProcessMoreNamespaceRules && isTopLevel)
                    {
                        ParseAndProcessNamespaceRule(preludeTokens, blockTokens);
                    }
                    // Note that we always return "null" here even if we successfully process the @namespace rule. That's because
                    // @namespace rules are fully processed inside the parser and don't become visible to the calling code.
                    return null;
                }
                case "page":
                {
                    return ParsePageRule(preludeTokens, blockTokens);
                }
                case "font-face":
                {
                    return ParseFontFaceRule(blockTokens);
                }
                case "list":
                {
                    return ParseListRule(preludeTokens, blockTokens);
                }
                default:
                {
                    // Unsupported at-rule.
                    return null;
                }
            }
        }

        private void ParseAndProcessNamespaceRule(List<CssToken> preludeTokens, List<CssToken> blockTokens)
        {
            // @namespace rules cannot have a block.
            if (blockTokens != null)
            {
                return;
            }

            CssTokenSource stream = new CssListTokenSource(preludeTokens);
            CssToken token = stream.ConsumeWhitespaceAndToken();

            // Optional prefix.
            string prefix = string.Empty;
            if (token.Type == CssTokenType.Ident)
            {
                // Namespace prefixes are case-sensitive.
                prefix = ((CssTextToken)token).Text;

                token = stream.ConsumeWhitespaceAndToken();
            }

            // Namespace name.
            if ((token.Type != CssTokenType.String) && (token.Type != CssTokenType.Url))
            {
                return;
            }
            string name = ((CssTextToken)token).Text;

            token = stream.ConsumeWhitespaceAndToken();
            if (!token.IsEof())
            {
                return;
            }

            // Get a prefix that will be unique among all parsed stylesheets.
            // Individual stylesheets may use same namespace prefixes for different namespaces. If we're going to combine
            // rules imported from different stylesheets, we need to make sure all namespace names use unique prefixes.
            string uniquePrefix;
            CssNamespace redefinedNamespace = StringUtil.HasChars(prefix)
                ? mDeclaredNamespaces.GetNamespace(prefix)
                : mDeclaredNamespaces.DefaultNamespaceForElements;
            if ((redefinedNamespace != null) && redefinedNamespace.IsSpecific)
            {
                // If we have already processed a namespace with the same prefix in this stylesheet, re-use the unique prefix
                // that we've computed for that existing namespace.
                uniquePrefix = (mMultipleStylesheetNamespacePrefixResover != null)
                    ? mMultipleStylesheetNamespacePrefixResover.RedefinePrefix(redefinedNamespace.Name, name)
                    : redefinedNamespace.Prefix;
            }
            else
            {
                // We haven't yet processed a namespace with this prefix in this stylesheet but we might have met the same
                // prefix in other stylesheets. Make sure different namespaces don't have the same prefix in parsed rules.
                uniquePrefix = (mMultipleStylesheetNamespacePrefixResover != null)
                    ? mMultipleStylesheetNamespacePrefixResover.GetPrefix(name, prefix)
                    : prefix;
            }

            // If the namespace rule is well-formed, remember the namespace.
            // Note that "prefix" is unique to this stylesheet only and "uniquePrefix" is unique among all stylesheets processed
            // by this instance of the CSS parser.
            mDeclaredNamespaces.SetNamespace(prefix, new CssNamespace(uniquePrefix, name));

            // @import rules must precede other well-formed rules except @charset and other @import rules.
            // We've just parsed and processed a well-formed @namespace rule, so we cannot accept @import rules any more.
            mCanProcessMoreImportRules = false;
        }

        private static CssImportRule ParseImportRule(
            List<CssToken> preludeTokens,
            List<CssToken> blockTokens,
            string baseUri)
        {
            // @import rules cannot have a block.
            if (blockTokens != null)
            {
                return null;
            }

            CssTokenSource tokenSource = new CssListTokenSource(preludeTokens);

            // URL of the stylesheet to import.
            CssToken token = tokenSource.ConsumeWhitespaceAndToken();
            if ((token.Type != CssTokenType.String) && (token.Type != CssTokenType.Url))
            {
                return null;
            }
            string url = ResolveRelativeUri(baseUri, ((CssTextToken)token).Text);

            // If we don't support the media query, the @import rule is still considered well-formed and it must not be ignored.
            CssMediaQueryList mediaQueryList = ConsumeMediaQueryList(tokenSource);
            return new CssImportRule(url, mediaQueryList);
        }

        private CssPageRule ParsePageRule(List<CssToken> preludeTokens, List<CssToken> blockTokens)
        {
            CssTokenSource preludeTokenSource = new CssListTokenSource(preludeTokens);
            CssPageSelector selector = ParsePageSelector(preludeTokenSource);
            if (selector == null)
            {
                // Parsing error.
                return null;
            }

            // Declarations of @page rules may contain nested margin at-rules. We don't support them but we must parse them.
            CssDeclarationCollection declarations = ConsumeDeclarationList(new CssListTokenSource(blockTokens), true);

            return new CssPageRule(selector, declarations);
        }

        private CssFontFaceRule ParseFontFaceRule(List<CssToken> blockTokens)
        {
            CssDeclarationCollection declarations = ConsumeDeclarationList(new CssListTokenSource(blockTokens), false);
            return new CssFontFaceRule(declarations);
        }

        private CssListRule ParseListRule(List<CssToken> preludeTokens, List<CssToken> blockTokens)
        {
            CssTokenSource preludeTokenSource = new CssListTokenSource(preludeTokens);

            CssToken token = preludeTokenSource.ConsumeWhitespaceAndToken();
            if ((token.Type != CssTokenType.Ident) || (token.Type == CssTokenType.Eof))
            {
                return null;
            }

            Debug.Assert(token.Type == CssTokenType.Ident);
            string listDefName = ((CssTextToken)token).Text;

            string listLevelName = string.Empty;
            token = preludeTokenSource.ConsumeWhitespaceAndToken();
            if (token.Type == CssTokenType.Colon)
            {
                token = preludeTokenSource.ConsumeWhitespaceAndToken();
                listLevelName = (token.Type == CssTokenType.Ident)
                    ? ((CssTextToken)token).Text
                    : string.Empty;

                token = preludeTokenSource.ConsumeWhitespaceAndToken();
            }

            string lfoName = (token.Type == CssTokenType.Ident)
                ? ((CssTextToken)token).Text
                : string.Empty;

            token = preludeTokenSource.ConsumeWhitespaceAndToken();
            if (token.Type != CssTokenType.Eof)
            {
                return null;
            }

            CssDeclarationCollection declarations = ConsumeDeclarationList(new CssListTokenSource(blockTokens), false);

            return new CssListRule(listDefName, listLevelName, lfoName, declarations);
        }

        private static CssPageSelector ParsePageSelector(CssTokenSource tokenSource)
        {
            CssToken token = tokenSource.ConsumeWhitespaceAndToken();
            if (token.IsEof())
            {
                // Empty selector list. The rule is supported and it matches any page.
                return new CssPageSelector(null);
            }
            tokenSource.Reconsume();

            List<string> pageNames = new List<string>();
            string currentPageName = null;
            while (true)
            {
                token = tokenSource.ConsumeWhitespaceAndToken();

                switch (token.Type)
                {
                    case CssTokenType.Comma:
                    case CssTokenType.Eof:
                    {
                        // A list item has ended. If it's a supported page name, add it to the result.
                        if (currentPageName == null)
                        {
                            // A list item has not been recognized. The list is malformed.
                            return null;
                        }

                        // A page name will be empty if it is recognized but not supported.
                        if (currentPageName != "")
                        {
                            pageNames.Add(currentPageName);
                        }
                        currentPageName = null;

                        // If the all list has been parsed and it is well-formed, return it. Note that the list of pages may
                        // be empty if we recognize but don't support any of page selectors.
                        if (token.IsEof())
                        {
                            return new CssPageSelector(pageNames.ToArray());
                        }
                        break;
                    }
                    case CssTokenType.Ident:
                    case CssTokenType.Colon:
                    {
                        // Try to parse a page name.
                        if (currentPageName == null)
                        {
                            tokenSource.Reconsume();
                            currentPageName = ParsePageName(tokenSource);
                        }
                        else
                        {
                            // Parsing error. We have already parsed this list item as a page name. The list is malformed.
                            return null;
                        }
                        break;
                    }
                    default:
                    {
                        // Unexpected token. Malformed list.
                        return null;
                    }
                }
            }
        }

        private static string ParsePageName(CssTokenSource tokenSource)
        {
            CssToken token = tokenSource.Consume();
            string pageName = null;

            if (token.Type == CssTokenType.Ident)
            {
                // Page names are case sensitive.
                pageName = ((CssTextToken)token).Text;

                // According to the specification, the "auto" keyword (case-insensitive) is a valid page name that, however,
                // matches no pages. The reason is that "auto" is the default value of the "page" CSS property. In other words,
                // "auto" is the default page name in CSS.
                if (StringUtil.AsciiLowerCase(pageName) == "auto")
                {
                    // Mark the page name as "recognized but not supported".
                    pageName = "";
                }

                token = tokenSource.Consume();
            }

            // There may be more than one pseudo-class. Parse them all.
            while (token.Type == CssTokenType.Colon)
            {
                // Currently, we don't support @page pseudo-classes. That's why we mark the page name
                // as "recognized but not supported".
                pageName = "";

                // Parse the name of the pseudo-class.
                token = tokenSource.Consume();
                if (token.Type != CssTokenType.Ident)
                {
                    // Parsing error. Unexpected token.
                    return null;
                }

                // Check if the pseudo-class is recognized.
                string pseudoClassName = StringUtil.AsciiLowerCase(((CssTextToken)token).Text);
                bool pseudoClassIsRecognized =
                    (pseudoClassName == "first") ||
                    (pseudoClassName == "left") ||
                    (pseudoClassName == "right") ||
                    (pseudoClassName == "blank");
                if (!pseudoClassIsRecognized)
                {
                    // If a pseudo-class is not recognized, the whole page name list is considered invalid.
                    return null;
                }

                token = tokenSource.Consume();
            }

            tokenSource.Reconsume();
            return pageName;
        }

        private static CssMediaQueryList ConsumeMediaQueryList(CssTokenSource tokenSource)
        {
            CssToken token = tokenSource.ConsumeWhitespaceAndToken();
            if (token.IsEof())
            {
                // Empty media query list is valid and is equivalent to the "all" media type.
                return new CssMediaQueryList(true);
            }
            tokenSource.Reconsume();

            List<CssToken> mediaTypeTokens = new List<CssToken>();

            string mediaType = null;
            bool foundSupportedMediaType = false;
            while (true)
            {
                token = tokenSource.ConsumeWhitespaceAndToken();

                if (token.IsEof() || (token.Type == CssTokenType.Comma))
                {
                    if ((mediaType == "all") || (mediaType == "screen"))
                    {
                        foundSupportedMediaType = true;
                    }

                    if (token.IsEof())
                    {
                        // The list is well-formed and contains only supported media types.
                        return new CssMediaQueryList(foundSupportedMediaType);
                    }

                    // Comma separator. Start parsing a new list item.
                    mediaType = null;
                }
                else
                {
                    tokenSource.Reconsume();
                    mediaTypeTokens.Clear();
                    ConsumeComponent(tokenSource, mediaTypeTokens);

                    if ((mediaType == null) &&
                        (mediaTypeTokens.Count == 1) &&
                        (mediaTypeTokens[0].Type == CssTokenType.Ident))
                    {
                        // Media types are ASCII case-insensitive.
                        mediaType = StringUtil.AsciiLowerCase(((CssTextToken)token).Text);
                    }
                    else
                    {
                        // Unexpected token. The current list item contains anything other than one indent and optional
                        // whitespace. This media query is not supported.
                        mediaType = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Consumes a component.
        /// </summary>
        /// <remarks>
        /// A component is a token, a function with all its parameters, or a sequence of tokens in brackets.
        /// </remarks>
        private static void ConsumeComponent(CssTokenSource tokenSource, List<CssToken> result)
        {
            CssToken token = tokenSource.Consume();
            // The calling code must make sure there is a token to consume.
            Debug.Assert(!token.IsEof());

            // If the result is null, the calling code wants to skip the component and is not interested in its value.
            if (result != null)
            {
                result.Add(token);
            }

            CssTokenType stopTokenType = GetComponentStopTokenType(token.Type);
            if (stopTokenType != CssTokenType.Eof)
            {
                ConsumeComponentSequence(tokenSource, stopTokenType, result);
                if (tokenSource.HasMoreTokens())
                {
                    CssToken endingToken = tokenSource.Consume();
                    if (result != null)
                    {
                        result.Add(endingToken);
                    }
                }
            }
        }

        /// <summary>
        /// Consumes components until the stop token is encountered.
        /// </summary>
        /// <remarks>
        /// A component is a token, a function with all its parameters, or a sequence of tokens in brackets
        /// </remarks>
        private static void ConsumeComponentSequence(
            CssTokenSource tokenSource,
            CssTokenType stopTokenType,
            List<CssToken> result)
        {
            // Since components may be nested, we maintain a stack of stop tokens for each nesting level.
            // MEMORY. Nested components are not common so we allocate the stack lazily.
            CssTokenTypeStack nestedStopTokens = null;

            while (true)
            {
                CssToken token = tokenSource.Consume();
                if (token.IsEof())
                {
                    // Reached the EOF. Stop reading.
                    break;
                }

                if ((nestedStopTokens != null) && !nestedStopTokens.IsEmpty)
                {
                    // Finished reading a nested component value. Forget its stop token.
                    if (token.Type == nestedStopTokens.Peek())
                    {
                        nestedStopTokens.Pop();
                    }
                }
                else if (token.Type == stopTokenType)
                {
                    // Encountered the stop token of the whole sequence. Stop reading and leave the stop token uncounsumed.
                    tokenSource.Reconsume();
                    break;
                }

                // If the result is null, the calling code wants to skip the component and is not interested in its value.
                if (result != null)
                {
                    result.Add(token);
                }

                CssTokenType nestedStopTokenType = GetComponentStopTokenType(token.Type);
                if (nestedStopTokenType != CssTokenType.Eof)
                {
                    // Encountered a nested component value. Allocate the nested stop tokens stack if needed and remember
                    // the stop token.
                    if (nestedStopTokens == null)
                    {
                        nestedStopTokens = new CssTokenTypeStack();
                    }
                    nestedStopTokens.Push(nestedStopTokenType);
                }
            }
        }

        private static CssTokenType GetComponentStopTokenType(CssTokenType startTokenType)
        {
            switch (startTokenType)
            {
                case CssTokenType.BlockBracketLeft:
                    return CssTokenType.BlockBracketRight;
                case CssTokenType.SquareBracketLeft:
                    return CssTokenType.SquareBracketRight;
                case CssTokenType.RoundBracketLeft:
                case CssTokenType.Function:
                    return CssTokenType.RoundBracketRight;
                default:
                    return CssTokenType.Eof;
            }
        }

        private static void ConsumeBlock(
            CssTokenSource tokenSource,
            List<CssToken> result)
        {
            ConsumeComponentSequence(tokenSource, CssTokenType.BlockBracketRight, result);
            tokenSource.Consume(); // '}' or EOF
        }

        private static CssMediaRule BuildMediaRule(
            CssMediaQueryList mediaQueryList,
            List<CssRule> nestedRules)
        {
            // Inside @media rules, we allow only style, @page, and other @media rules. Remove rules of other types.
            int filteredOutRuleCount = 0;
            foreach (CssRule innerRule in nestedRules)
            {
                if (!IsAllowedInsideMediaRule(innerRule.Type))
                {
                    ++filteredOutRuleCount;
                }
            }

            // Place filtered rules into an array.
            CssRule[] nestedRulesArray = new CssRule[nestedRules.Count - filteredOutRuleCount];
            int ruleIndex = 0;
            foreach (CssRule innerRule in nestedRules)
            {
                if (IsAllowedInsideMediaRule(innerRule.Type))
                {
                    nestedRulesArray[ruleIndex] = innerRule;
                    ++ruleIndex;
                }
            }

            return new CssMediaRule(mediaQueryList, nestedRulesArray);
        }

        private static bool IsAllowedInsideMediaRule(CssRuleType ruleType)
        {
            return (ruleType == CssRuleType.Style) ||
                (ruleType == CssRuleType.Media) ||
                (ruleType == CssRuleType.Page);
        }

        private CssStyleRule BuildStyleRule(
            List<CssToken> selectorTokens,
            List<CssToken> declarationTokens)
        {
            CssSelector[] selectors = ParseSelectors(selectorTokens);
            if (selectors.Length == 0)
            {
                return null;
            }
            CssDeclarationCollection declarations = ConsumeDeclarationList(new CssListTokenSource(declarationTokens), false);
            return new CssStyleRule(selectors, declarations);
        }

        private CssSelector[] ParseSelectors(List<CssToken> tokens)
        {
            List<CssSelector> selectors = new List<CssSelector>();
            List<CssToken> selectorTokens = new List<CssToken>();
            // Here we make one step past the end of token lists. This allows to make the loop simpler, because the last token
            // is processed inside the loop.
            for (int i = 0; i <= tokens.Count; i++)
            {
                // If the current token is a selector separator (comma or EOF), parse selector tokens that we've collected.
                if ((i >= tokens.Count) || (tokens[i].Type == CssTokenType.Comma))
                {
                    // Remove trailing whitespace.
                    if ((selectorTokens.Count > 0) && selectorTokens[selectorTokens.Count - 1].IsWhitespace())
                    {
                        selectorTokens.RemoveAt(selectorTokens.Count - 1);
                    }

                    CssSelector selector = CssSelectorParser.Parse(selectorTokens, mDeclaredNamespaces);
                    if (selector != null)
                    {
                        selectors.Add(selector);
                    }
                    else
                    {
                        // Parsing error. If any selector cannot be parsed, the whole selector list is considered malformed,
                        // and the whole rule must be ignored.
                        return new CssSelector[0];
                    }

                    selectorTokens.Clear();
                }
                // Collect tokens of the current selector. Remove leading whitespace.
                else if ((selectorTokens.Count > 0) || !tokens[i].IsWhitespace())
                {
                    selectorTokens.Add(tokens[i]);
                }
            }
            return selectors.ToArray();
        }

        private CssDeclarationCollection ParseDeclaration(
            string propertyName,
            List<CssToken> value,
            bool isImportant)
        {
            // CSS property names contain only ASCII characters and are case-insensitive.
            CssPropertyDef propertyDef = CssPropertyDefFactory.GetPropertyDef(StringUtil.AsciiLowerCase(propertyName));

            CssValueList cssValues = ParseSpaceSeparatedValueList(new CssListTokenSource(value), mStylesheetBaseUri);
            return (cssValues.Count != 0)
                ? propertyDef.CreateDeclarations(cssValues, isImportant, mIsInQuirksMode)
                : null;
        }

        private static CssValueList ParseSpaceSeparatedValueList(
            CssTokenSource tokenSource,
            string stylesheetBaseUri)
        {
            List<CssValue> result = new List<CssValue>();

            tokenSource.ConsumeOptionalWhitespace();

            while (tokenSource.HasMoreTokens())
            {
                CssValue value = ParseValue(tokenSource, stylesheetBaseUri);
                if (value == null)
                {
                    // Parsing error.
                    return new CssValueList();
                }
                result.Add(value);

                tokenSource.ConsumeOptionalWhitespace();
            }

            return new CssValueList(result.ToArray());
        }

        private static CssValue ParseValue(
            CssTokenSource tokenSource,
            string stylesheetBaseUri)
        {
            CssToken token = tokenSource.Consume();

            // Is used as a value in the "font" shorthand property.
            if (token.IsDelim('/'))
            {
                return CssValue.Solidus;
            }

            switch (token.Type)
            {
                case CssTokenType.String:
                {
                    return new CssStringValue(((CssTextToken)token).Text);
                }
                case CssTokenType.Comma:
                {
                    return CssValue.Comma;
                }
                case CssTokenType.Number:
                {
                    return new CssNumberValue(((CssNumberToken)token).Value);
                }
                case CssTokenType.Percentage:
                {
                    return new CssPercentageValue(((CssNumberToken)token).Value);
                }
                case CssTokenType.Dimension:
                {
                    CssDimensionToken dimensionToken = (CssDimensionToken)token;

                    if (StringUtil.AsciiLowerCase(dimensionToken.Unit) == "deg")
                    {
                        return new CssDegreeValue(dimensionToken.Value);
                    }

                    CssUnit unit = ParseUnit(dimensionToken.Unit);
                    return (unit != CssUnit.None)
                        ? new CssLengthValue(dimensionToken.Value, unit)
                        : null;
                }
                case CssTokenType.Function:
                {
                    string name = ((CssTextToken)token).Text;
                    // CSS function names are ASCII case-insensitive.
                    name = StringUtil.AsciiLowerCase(name);

                    List<CssValue> parameters = new List<CssValue>();
                    while (true)
                    {
                        CssToken parameterToken = tokenSource.ConsumeWhitespaceAndToken();
                        if (parameterToken.IsEof())
                        {
                            return null;
                        }
                        if (parameterToken.Type == CssTokenType.RoundBracketRight)
                        {
                            return new CssFunctionValue(name, new CssValueList(parameters.ToArray()));
                        }
                        tokenSource.Reconsume();
                        // Note that the parameter might be a function too.
                        CssValue parameter = ParseValue(tokenSource, stylesheetBaseUri);
                        if (parameter == null)
                        {
                            return null;
                        }
                        parameters.Add(parameter);
                    }
                }
                case CssTokenType.Ident:
                {
                    return new CssIdentifierValue(((CssTextToken)token).Text);
                }
                case CssTokenType.Hash:
                {
                    return new CssHashValue(((CssTextToken)token).Text);
                }
                case CssTokenType.Url:
                {
                    string uri = ResolveRelativeUri(stylesheetBaseUri, ((CssTextToken)token).Text);
                    return new CssUriValue(uri);
                }
                default:
                {
                    // Parsing error. Unexpected token.
                    return null;
                }
            }
        }

        private static string ResolveRelativeUri(string stylesheetBaseUri, string uri)
        {
            if ((stylesheetBaseUri != null) && !UriUtil.IsAbsoluteHref(uri))
            {
                uri = UriUtil.ConstructAbsoluteUri(stylesheetBaseUri, uri);
            }
            return uri;
        }

        private static CssUnit ParseUnit(string value)
        {
            switch (StringUtil.AsciiLowerCase(value))
            {
                case "in":
                    return CssUnit.In;
                case "cm":
                    return CssUnit.Cm;
                case "mm":
                    return CssUnit.Mm;
                case "pt":
                    return CssUnit.Pt;
                case "pc":
                    return CssUnit.Pc;
                case "px":
                    return CssUnit.Px;
                case "em":
                    return CssUnit.Em;
                case "ex":
                    return CssUnit.Ex;
                case "rem":
                    return CssUnit.Rem;
                default:
                    return CssUnit.None;
            }
        }

        /// <summary>
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </summary>
        private readonly bool mIsInQuirksMode;

        /// <summary>
        /// Base URI (directory) of the stylesheet that is being parsed.
        /// </summary>
        /// <remarks>
        /// This base URI is prepended to all parsed relative URI values in order to resolve them relative to this stylesheet's
        /// location. This makes it easier to later combine and resolve URIs in CSS declarations loaded from different
        /// stylesheets.
        /// </remarks>
        private readonly string mStylesheetBaseUri;

        /// <summary>
        /// Stack of CSS rule lists processed by the parser. Allows to parse nested rule lists.
        /// </summary>
        private readonly CssRuleListStack mRuleLists = new CssRuleListStack();

        /// <summary>
        /// CSS namespaces declared in the stylesheet that is being parsed.
        /// </summary>
        private readonly CssNamespacePrefixes mDeclaredNamespaces = new CssNamespacePrefixes();

        /// <summary>
        /// When the stylesheet which is currently being parsed is loaded together with other stylesheets, this class helps
        /// to ensure that in parsed rules different namespaces will have different prefixes.
        /// </summary>
        private readonly CssNamespacePrefixResolver mMultipleStylesheetNamespacePrefixResover;

        /// <summary>
        /// Indicates whether a @namespace rule is allowed at the current position in CSS.
        /// </summary>
        /// <remarks>
        /// @namespace rules must be ignored if anything has already been processed except @charset or @import rules.
        /// </remarks>
        private bool mCanProcessMoreNamespaceRules = true;

        /// <summary>
        /// Indicates whether a @import rule is allowed at the current position in CSS.
        /// </summary>
        /// <remarks>
        /// @import rules must be ignored if anything has already been processed except @charset rules.
        /// </remarks>
        private bool mCanProcessMoreImportRules = true;
    }
}
