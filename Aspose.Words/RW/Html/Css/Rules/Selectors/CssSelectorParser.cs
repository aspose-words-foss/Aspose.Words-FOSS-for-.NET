// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Converts a list of tokens that represents a CSS selector into a new selector instance.
    /// </summary>
    internal class CssSelectorParser
    {
        private CssSelectorParser(
            IList<CssToken> tokens,
            CssNamespacePrefixes declaredCssNamespaces)
        {
            Debug.Assert(declaredCssNamespaces != null);

            mTokens = tokens;
            mDeclaredCssNamespaces = declaredCssNamespaces;
        }

        /// <summary>
        /// Parses the tokens into a CSS selector.
        /// </summary>
        /// <param name="tokens">A list of tokens that represents a CSS selector.</param>
        /// <param name="declaredCssNamespaces">CSS namespace declarations met so far.</param>
        /// <returns>
        /// The CSS selector, if the tokens were parsed successfully.
        /// Otherwise, <c>null</c> is returned.
        /// </returns>
        internal static CssSelector Parse(
            IList<CssToken> tokens,
            CssNamespacePrefixes declaredCssNamespaces)
        {
            CssSelectorParser parser = new CssSelectorParser(tokens, declaredCssNamespaces);
            return parser.ParseSelector();
        }

        /// <summary>
        /// Parses the sequence "CombinableSelector[Combinator CombinableSelector][Combinator CombinableSelector][...]".
        /// </summary>
        /// <returns>
        /// The parsed selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSelector ParseSelector()
        {
            mTokenIndex = 0;
            if ((mTokens == null) || (mTokenIndex >= mTokens.Count))
            {
                // Parsing error. The list of tokens is empty.
                return null;
            }

            // Pre-fetch tokens.
            mCurrentToken = (mTokenIndex < mTokens.Count)
                ? mTokens[mTokenIndex]
                : CssToken.Eof;
            int nextTokenIndex = mTokenIndex + 1;
            mNextToken = (nextTokenIndex < mTokens.Count)
                ? mTokens[nextTokenIndex]
                : CssToken.Eof;

            CssCombinableSelector head = ParseCombinableSelector();
            if (head == null)
            {
                // Error. Cannot parse the head selector.
                return null;
            }

            // A selector can contain zero or more combinators.
            List<CssCombinator> combinators = ParseCombinators();
            if (combinators == null)
            {
                // Error while parsing combinators.
                return null;
            }

            // If a selector contains one or more combinators, it is a complex selector.
            if (combinators.Count > 0)
            {
                // Only the last selector in a combination is allowed to select a pseudo-element.
                if (head.SelectsPseudoElement())
                {
                    // Error. The head selector is invalid.
                    return null;
                }

                for (int i = 0; i < combinators.Count - 1; i++)
                {
                    CssCombinator combinator = combinators[i];
                    if (combinator.Selector.SelectsPseudoElement())
                    {
                        // Error. One of combinators is invalid.
                        return null;
                    }
                }

                return new CssComplexSelector(head, combinators);
            }
            // Otherwise, it is a simple selector.
            return head;
        }

        /// <summary>
        /// Tries to parse a combinator and a selector: "[Combinator CombinableSelector]".
        /// </summary>
        /// <returns>
        /// A list of parsed combinators (instances of <see cref="CssCombinator"/>).
        /// Items of the list are never <c>null</c>.
        /// The list is empty if there are no combinators.
        /// If a parsing error occurs, <c>null</c> is returned.
        /// </returns>
        private List<CssCombinator> ParseCombinators()
        {
            List<CssCombinator> combinators = new List<CssCombinator>();
            while (!mCurrentToken.IsEof())
            {
                if (mCurrentToken.IsWhitespace() &&
                    (mNextToken.IsDelim('+') || mNextToken.IsDelim('>') || mNextToken.IsDelim('~')))
                {
                    SkipWhitespace();
                }

                CssCombinator combinator;
                CssToken currentToken = mCurrentToken;
                if (currentToken.IsDelim('+'))
                {
                    combinator = ParseAdjacentSiblingCombinator();
                }
                else if (currentToken.IsDelim('>'))
                {
                    combinator = ParseChildCombinator();
                }
                else if (currentToken.IsDelim('~'))
                {
                    combinator = ParseGeneralSiblingCombinator();
                }
                else if (currentToken.IsWhitespace())
                {
                    combinator = ParseDescendantCombinator();
                }
                else
                {
                    // Parsing error. Unexpected token.
                    return null;
                }

                if (combinator == null)
                {
                    // Error. Cannot parse the combinator.
                    return null;
                }
                combinators.Add(combinator);
            }

            return combinators;
        }

        /// <summary>
        /// Parses the right-hand side of a general sibling combinator (the sequence "~ Selector").
        /// </summary>
        /// <returns>The parsed selector or <c>null</c> in case of an error.</returns>
        private CssGeneralSiblingCombinator ParseGeneralSiblingCombinator()
        {
            // Read a combinator token.
            CssToken combinatorToken = ReadTokenAndAdvance(CssTokenType.Delim);
            if ((combinatorToken == null) || (((CssDelimToken)combinatorToken).Value != '~'))
            {
                // Error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            // Read the right-hand selector.
            CssCombinableSelector right = ParseCombinableSelector();
            if (right == null)
            {
                // Error. Cannot parse the right-hand selector.
                return null;
            }

            return new CssGeneralSiblingCombinator(right);
        }

        /// <summary>
        /// Parses the right-hand side of a child element combinator (the sequence "&gt; Selector").
        /// </summary>
        /// <returns>The parsed selector or <c>null</c> in case of an error.</returns>
        private CssChildCombinator ParseChildCombinator()
        {
            // Read a combinator token.
            CssToken combinatorToken = ReadTokenAndAdvance(CssTokenType.Delim);
            if ((combinatorToken == null) || (((CssDelimToken)combinatorToken).Value != '>'))
            {
                // Error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            // Read the right-hand selector.
            CssCombinableSelector right = ParseCombinableSelector();
            if (right == null)
            {
                // Error. Cannot parse the right-hand selector.
                return null;
            }

            return new CssChildCombinator(right);
        }

        /// <summary>
        /// Parses the right-hand side of an adjacent sibling combinator (the sequence "+ Selector").
        /// </summary>
        /// <returns>The parsed selector or <c>null</c> in case of an error.</returns>
        private CssAdjacentSiblingCombinator ParseAdjacentSiblingCombinator()
        {
            // Read a combinator token.
            CssToken combinatorToken = ReadTokenAndAdvance(CssTokenType.Delim);
            if ((combinatorToken == null) || (((CssDelimToken)combinatorToken).Value != '+'))
            {
                // Error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            // Read the right-hand selector.
            CssCombinableSelector right = ParseCombinableSelector();
            if (right == null)
            {
                // Error. Cannot parse the right-hand selector.
                return null;
            }

            return new CssAdjacentSiblingCombinator(right);
        }

        /// <summary>
        /// Parses the right-hand side of a descendant combinator (the sequence "'space' Selector").
        /// </summary>
        /// <returns>The parsed selector or <c>null</c> in case of an error.</returns>
        private CssDescendantCombinator ParseDescendantCombinator()
        {
            // Read whitespace combinator.
            if (ReadTokenAndAdvance(CssTokenType.Whitespace) == null)
            {
                // Error. Unexpected token.
                return null;
            }

            // Read the right-hand selector.
            CssCombinableSelector right = ParseCombinableSelector();
            if (right == null)
            {
                // Error. Cannot parse the right-hand selector.
                return null;
            }

            return new CssDescendantCombinator(right);
        }

        /// <summary>
        /// Parses a combinable selector, which is either a simple selector, or a sequence of simple selectors
        /// (a compound selector).
        /// </summary>
        /// <returns>
        /// The parsed selector or <c>null</c> in case of an error.
        /// </returns>
        /// <remarks>
        /// For example, "div", "div[align=left]", and "div.class[align=left]:not(#id)" are combinable selectors.
        /// </remarks>
        private CssCombinableSelector ParseCombinableSelector()
        {
            CssSimpleSelector head = ParseSimpleSelector();
            if (head == null)
            {
                // Error. Cannot parse the head selector.
                return null;
            }

            List<CssSimpleSelector> compoundParts = ParseCompoundSelectorParts();
            if (compoundParts == null)
            {
                // An error has occurred while parsing compound selector parts.
                return null;
            }

            // It might be just a simple selector, without compound parts.
            if (compoundParts.Count == 0)
            {
                return head;
            }

            // It is a compound selector consisting of more than one part. Let's reconstruct it.
            compoundParts.Insert(0, head);
            CssCombinableSelector compoundSelector = compoundParts[compoundParts.Count - 1];
            for (int i = compoundParts.Count - 2; i >= 0; i--)
            {
                CssSimpleSelector part = compoundParts[i];
                if (part is CssPseudoElementSelector)
                {
                    // No more than one pseudo-element selector is allowed per compound selector.
                    // The pseudo-element selector must be the last item of a compound selector.
                    return null;
                }
                compoundSelector = new CssCompoundSelector(part, compoundSelector);
            }
            return compoundSelector;
        }

        /// <summary>
        /// Parses a sequence of simple selectors (a compound selector), which can be empty.
        /// </summary>
        /// <returns>
        /// A list of parsed simple selectors (instances of <see cref="CssSimpleSelector"/>) that constitute
        /// a compound selector.
        /// Items of the list are never <c>null</c>.
        /// The list is empty if there are no compound parts and the current selector is simple.
        /// In case of a parsing error <c>null</c> is returned.
        /// </returns>
        private List<CssSimpleSelector> ParseCompoundSelectorParts()
        {
            // Collect compound selector parts.
            List<CssSimpleSelector> selectorParts = new List<CssSimpleSelector>();
            while (!mCurrentToken.IsDelim('+') &&
                !mCurrentToken.IsDelim('>') &&
                !mCurrentToken.IsDelim('~') &&
                !mCurrentToken.IsWhitespace() &&
                !mCurrentToken.IsEof())
            {
                if (CurrentTokenStartsAClass() ||
                    (mCurrentToken.Type == CssTokenType.Hash) ||
                    (mCurrentToken.Type == CssTokenType.SquareBracketLeft) ||
                    (mCurrentToken.Type == CssTokenType.Colon))
                {
                    CssSimpleSelector part = ParseCompoundSelectorPart();
                    if (part == null)
                    {
                        // Error. Cannot parse a part of a compound selector.
                        return null;
                    }
                    selectorParts.Add(part);
                }
                else
                {

                    // Parsing error. Unexpected token.
                    return null;
                }
            }
            return selectorParts;
        }

        /// <summary>
        /// Parses second and later parts of a sequence of simple selectors.
        /// This can be any simple selector except the universal selector and the type selector.
        /// </summary>
        /// <returns>
        /// The parsed simple selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParseCompoundSelectorPart()
        {
            if (CurrentTokenStartsAClass())
            {
                return ParseClassSelector();
            }

            switch (mCurrentToken.Type)
            {
                case CssTokenType.Hash:
                    return ParseIdSelector();
                case CssTokenType.SquareBracketLeft:
                    return ParseAttributeSelector();
                case CssTokenType.Colon:
                    return ParsePseudoSelector();
                default:
                    // Parsing error. Unexpected token.
                    return null;
            }
        }

        /// <summary>
        /// Parses a simple selector.
        /// </summary>
        /// <returns>
        /// The parsed simple selector or <c>null</c> in case of an error.
        /// </returns>
        /// <remarks>
        /// The definition of the simple selector can be found here: http://www.w3.org/TR/css3-selectors/#selector-syntax
        /// </remarks>
        private CssSimpleSelector ParseSimpleSelector()
        {
            if ((mCurrentToken.Type == CssTokenType.Ident) ||
                mCurrentToken.IsDelim('*') ||
                mCurrentToken.IsDelim('|'))
            {
                return ParseTypeOrUniversalSelector();
            }
            else
            {
                return ParseCompoundSelectorPart();
            }
        }

        /// <summary>
        /// Parses a :not() pseudo-class selector.
        /// </summary>
        /// <returns>
        /// The parsed selector or <c>null</c> in case of an error.
        /// </returns>
        private CssNotSelector ParseNotSelector()
        {
            if (ReadTokenAndAdvance(CssTokenType.Function) == null)
            {
                // Error. Unexpected token.
                return null;
            }
            SkipWhitespace();
            CssSimpleSelector argument = ParseNotSelectorArgument();
            if (argument == null)
            {
                // Error. Cannot parse the argument.
                return null;
            }
            if (argument is CssPseudoElementSelector)
            {
                // A pseudo-element cannot be an argument of the :not pseudo-class.
                return null;
            }
            if (argument is CssNotSelector)
            {
                // :not pseudo-classes cannot be nested.
                return null;
            }
            SkipWhitespace();
            if (ReadTokenAndAdvance(CssTokenType.RoundBracketRight) == null)
            {
                // Error. Unexpected token.
                return null;
            }
            return new CssNotSelector(argument);
        }

        /// <summary>
        /// Parses an argument of a :not() pseudo-class selector. The argument must be a simple selector except
        /// the :not() pseudo-class itself.
        /// </summary>
        /// <returns>
        /// The parsed selector, or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParseNotSelectorArgument()
        {
            if (CurrentTokenStartsAClass())
            {
                return ParseClassSelector();
            }

            if (mCurrentToken.IsDelim('*') || mCurrentToken.IsDelim('|'))
            {
                return ParseTypeOrUniversalSelector();
            }

            switch (mCurrentToken.Type)
            {
                case CssTokenType.Ident:
                    return ParseTypeOrUniversalSelector();
                case CssTokenType.Hash:
                    return ParseIdSelector();
                case CssTokenType.SquareBracketLeft:
                    return ParseAttributeSelector();
                case CssTokenType.Colon:
                    return ParsePseudoSelector();
                default:
                {
                    // Parsing error. Unexpected token.
                    return null;
                }
            }
        }

        /// <summary>
        /// Parses either a pseudo-class or a pseudo-element selector.
        /// </summary>
        /// <returns>
        /// The parsed selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParsePseudoSelector()
        {
            if (ReadTokenAndAdvance(CssTokenType.Colon) == null)
            {
                // Error. Unexpected token.
                return null;
            }

            switch (mCurrentToken.Type)
            {
                case CssTokenType.Colon:
                {
                    Advance();
                    return ParsePseudoElement();
                }
                case CssTokenType.Ident:
                {
                    switch (StringUtil.AsciiLowerCase(((CssTextToken)mCurrentToken).Text))
                    {
                        case "before":
                        case "after":
                        case "first-line":
                        case "first-letter":
                            // For historical reasons, these pseudo-elements can be written either with one or with two
                            // leading colons.
                            return ParsePseudoElement();
                        default:
                            return ParsePseudoClassWithoutArguments();
                    }
                }
                case CssTokenType.Function:
                {
                    if (StringUtil.AsciiLowerCase(((CssTextToken)mCurrentToken).Text) == "not")
                    {
                        return ParseNotSelector();
                    }
                    else
                    {
                        return ParsePseudoClassWithArguments();
                    }
                }
                default:
                {
                    // Parsing error. Unexpected token.
                    return null;
                }
            }
        }

        /// <summary>
        /// Parses a pseudo-class selector that has an argument.
        /// </summary>
        /// <returns>
        /// The parsed pseudo-class selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParsePseudoClassWithArguments()
        {
            CssTextToken functionToken = (CssTextToken)ReadTokenAndAdvance(CssTokenType.Function);
            if (functionToken == null)
            {
                // Error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            // Read tokens of the pseudo-class argument.
            List<CssToken> argumentTokens = new List<CssToken>();
            bool whitespaceIsExpected = false;
            while ((mCurrentToken.Type != CssTokenType.RoundBracketRight) && !mCurrentToken.IsEof())
            {
                if (mCurrentToken.IsWhitespace())
                {
                    if (whitespaceIsExpected)
                    {
                        // Argument tokens can be delimeted by whitespace. The CSS parser combines adjacent whitespace
                        // characters into one whitespace token, so adjacent whitespace tokens are not expected.
                        argumentTokens.Add(mCurrentToken);
                        Advance();
                        whitespaceIsExpected = false;
                    }
                    else
                    {
                        // Parsing error. Unexpected token.
                        return null;
                    }
                }
                else
                {
                    argumentTokens.Add(mCurrentToken);
                    Advance();
                    whitespaceIsExpected = true;
                }
            }
            if (ReadTokenAndAdvance(CssTokenType.RoundBracketRight) == null)
            {
                // Error. Unexpected token.
                return null;
            }

            return CreatePseudoClassWithArguments(functionToken.Text, argumentTokens);
        }

        /// <summary>
        /// The pseudo-class selector factory. Creates a pseudo-class selector by its name and argument tokens.
        /// </summary>
        /// <returns>
        /// The created pseudo-class selector or <c>null</c> in case of an error.
        /// </returns>
        private static CssSimpleSelector CreatePseudoClassWithArguments(string name, IList<CssToken> argumentTokens)
        {
            switch (StringUtil.AsciiLowerCase(name))
            {
                case "lang":
                {
                    string language = CssLangArgumentParser.Parse(argumentTokens);
                    return (language != null)
                        ? new CssLangSelector(language)
                        : null;
                }
                case "nth-child":
                {
                    CssIndexArgument argument = CssIndexArgumentParser.Parse(argumentTokens);
                    return (argument != null)
                        ? new CssNthChildSelector(argument)
                        : null;
                }
                case "nth-last-child":
                {
                    CssIndexArgument argument = CssIndexArgumentParser.Parse(argumentTokens);
                    return (argument != null)
                        ? new CssNthLastChildSelector(argument)
                        : null;
                }
                case "nth-of-type":
                {
                    CssIndexArgument argument = CssIndexArgumentParser.Parse(argumentTokens);
                    return (argument != null)
                        ? new CssNthOfTypeSelector(argument)
                        : null;
                }
                case "nth-last-of-type":
                {
                    CssIndexArgument argument = CssIndexArgumentParser.Parse(argumentTokens);
                    return (argument != null)
                        ? new CssNthLastOfTypeSelector(argument)
                        : null;
                }
                default:
                {
                    // If a pseudo-class is unsupported, the selector must be considered invalid.
                    return null;
                }
            }
        }

        /// <summary>
        /// Parses a pseudo-class selector that does not have an argument.
        /// </summary>
        /// <returns>
        /// The parsed pseudo-class selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParsePseudoClassWithoutArguments()
        {
            CssTextToken nameToken = (CssTextToken)ReadTokenAndAdvance(CssTokenType.Ident);
            if (nameToken == null)
            {
                // Error. Unexpected token.
                return null;
            }
            string name = StringUtil.AsciiLowerCase(nameToken.Text);
            switch (name)
            {
                case "root":
                    return new CssRootSelector();
                case "empty":
                    return new CssEmptySelector();
                case "first-child":
                    // :first-child is same as :nth-child(1)
                    return new CssNthChildSelector(new CssIndexArgument(0, 1));
                case "last-child":
                    // :last-child is same as :nth-last-child(1)
                    return new CssNthLastChildSelector(new CssIndexArgument(0, 1));
                case "first-of-type":
                    // :first-of-type is same as :nth-of-type(1)
                    return new CssNthOfTypeSelector(new CssIndexArgument(0, 1));
                case "last-of-type":
                    // :last-of-type is same as :nth-last-of-type(1)
                    return new CssNthLastOfTypeSelector(new CssIndexArgument(0, 1));
                case "only-child":
                    return new CssOnlyChildSelector();
                case "only-of-type":
                    return new CssOnlyOfTypeSelector();
                case "link":
                    return new CssLinkSelector();
                case "visited":
                    return new CssVisitedSelector();
                case "active":
                    return new CssActiveSelector();
                case "hover":
                    return new CssHoverSelector();
                default:
                    // If a pseudo-class is unsupported, the selector must be considered invalid.
                    return null;
            }
        }

        /// <summary>
        /// Parses a pseudo-element selector.
        /// </summary>
        /// <returns>
        /// The parsed pseudo-element selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParsePseudoElement()
        {
            CssTextToken nameToken = (CssTextToken)ReadTokenAndAdvance(CssTokenType.Ident);
            if (nameToken == null)
            {
                // Error. Unexpected token.
                return null;
            }
            string name = nameToken.Text;
            HtmlElementPart part;
            switch (StringUtil.AsciiLowerCase(name))
            {
                case "before":
                    part = HtmlElementPart.Before;
                    break;
                case "after":
                    part = HtmlElementPart.After;
                    break;
                case "first-letter":
                    part = HtmlElementPart.FirstLetter;
                    break;
                case "first-line":
                    part = HtmlElementPart.FirstLine;
                    break;
                default:
                    // If a pseudo-element is unsupported, the selector must be considered invalid.
                    return null;
            }
            // WORDSJAVA-2911 use lower case for the name.
            return new CssPseudoElementSelector(part, StringUtil.AsciiLowerCase(name));
        }

        /// <summary>
        /// Parses an attribute selector that has the form "'['name[filter]']'" (the filter expression is optional;
        /// it determines the type of the attribute selector).
        /// </summary>
        /// <returns>
        /// The parsed attribute selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParseAttributeSelector()
        {
            // Read a left square bracket character.
            if (ReadTokenAndAdvance(CssTokenType.SquareBracketLeft) == null)
            {
                // Error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            // Read a namespace.
            // By default, attributes have no namespace. A default namespace, when declared in CSS, applies only
            // to type selectors.
            CssNamespace parsedNamespace = ParseNamespace(CssNamespace.Empty);
            if (parsedNamespace == null)
            {
                // Error. Cannot parse the namespace.
                return null;
            }

            // Read an attribute name.
            CssTextToken attributeNameToken = (CssTextToken)ReadTokenAndAdvance(CssTokenType.Ident);
            if (attributeNameToken == null)
            {
                // Error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            // Attribute names are case-insensitive in HTML. The HTML parser converts them to lowercase,
            // and the CSS parser should do the same.
            string attributeName = StringUtil.AsciiLowerCase(attributeNameToken.Text);

            CssSimpleSelector result = ParseAttributeFilter(parsedNamespace, attributeName);

            // Read a right square bracket character.
            if (ReadTokenAndAdvance(CssTokenType.SquareBracketRight) == null)
            {
                // Error. Unexpected token.
                return null;
            }

            return result;
        }

        /// <summary>
        /// Parses a filter expression of an attribute selector and creates an attribute selector corresponding to the parsed
        /// filter expression.
        /// </summary>
        /// <returns>
        /// The parsed attribute selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParseAttributeFilter(CssNamespace attributeNamespace, string attributeLocalName)
        {
            // Remember the equality operator char to choose the right selector type later.
            char operatorChar;

            if (mCurrentToken.Type == CssTokenType.Delim)
            {
                operatorChar = ((CssDelimToken)mCurrentToken).Value;
                if ((operatorChar == '^') ||
                    (operatorChar == '$') ||
                    (operatorChar == '*') ||
                    (operatorChar == '~') ||
                    (operatorChar == '|'))
                {
                    Advance(); // Prefix of complex match symbols like "^=" or "$=".
                    if (!mCurrentToken.IsDelim('='))
                    {
                        // Parsing error. Unexpected token.
                        return null;
                    }
                }
                Advance(); // '='
            }
            else if (mCurrentToken.Type == CssTokenType.SquareBracketRight)
            {
                // No filter, just an attribute name.
                return new CssAttributeNameSelector(attributeNamespace, attributeLocalName);
            }
            else
            {
                // Parsing error. Unexpected token.
                return null;
            }

            SkipWhitespace();

            string value = ParseAttributeFilterValue();
            if (value == null)
            {
                // Parsing error. Unexpected token type.
                return null;
            }

            SkipWhitespace();

            // Create a selector of required type.
            switch (operatorChar)
            {
                case '^':
                    return new CssAttributePrefixSelector(attributeNamespace, attributeLocalName, value);
                case '$':
                    return new CssAttributeSuffixSelector(attributeNamespace, attributeLocalName, value);
                case '*':
                    return new CssAttributeSubstringSelector(attributeNamespace, attributeLocalName, value);
                case '=':
                    return new CssAttributeValueSelector(attributeNamespace, attributeLocalName, value);
                case '~':
                    return new CssAttributeWordSelector(attributeNamespace, attributeLocalName, value);
                case '|':
                    return new CssAttributeDashPrefixSelector(attributeNamespace, attributeLocalName, value);
                default:
                    // Parsing error. Unexpected token.
                    return null;
            }
        }

        /// <summary>
        /// Parses a value of an attribute filter expression. The value is optionally enclosed in single or double quotes.
        /// </summary>
        /// <returns>
        /// The parsed filter value or <c>null</c> in case of an error.
        /// </returns>
        private string ParseAttributeFilterValue()
        {
            CssToken currentToken = mCurrentToken;
            switch (currentToken.Type)
            {
                case CssTokenType.Ident:
                case CssTokenType.String:
                    Advance();
                    return ((CssTextToken)currentToken).Text;
                default:
                    // Parsing error. Unexpected token.
                    return null;
            }
        }

        /// <summary>
        /// Parses a class selector.
        /// </summary>
        /// <returns>
        /// The parsed class selector or <c>null</c> in case of an error.
        /// </returns>
        private CssClassSelector ParseClassSelector()
        {
            Advance(); // Dot character.
            CssTextToken classNameToken = (CssTextToken)ReadTokenAndAdvance(CssTokenType.Ident);
            if (classNameToken == null)
            {
                // Error. Unexpected token.
                return null;
            }
            return new CssClassSelector(classNameToken.Text);
        }

        /// <summary>
        /// Parses an ID selector.
        /// </summary>
        /// <returns>
        /// The parsed selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParseIdSelector()
        {
            // Read the identifier.
            CssHashToken idToken = (CssHashToken)ReadTokenAndAdvance(CssTokenType.Hash);
            if ((idToken == null) || !idToken.IsIdentifier)
            {
                // Error. Unexpected token or the ID selector doesn't specify a valid identifier.
                return null;
            }
            return new CssIdSelector(idToken.Text);
        }

        /// <summary>
        /// Parses either a type selector or a universal selector.
        /// </summary>
        /// <returns>
        /// The parsed selector or <c>null</c> in case of an error.
        /// </returns>
        private CssSimpleSelector ParseTypeOrUniversalSelector()
        {
            CssNamespace parsedNamespace = ParseNamespace(mDeclaredCssNamespaces.DefaultNamespaceForElements);
            if (parsedNamespace == null)
            {
                // Error. Cannot parse the namespace.
                return null;
            }

            if (mCurrentToken.IsDelim('*'))
            {
                Advance();
                return new CssUniversalSelector(parsedNamespace);
            }

            if (mCurrentToken.Type == CssTokenType.Ident)
            {
                CssTextToken elementNameToken = (CssTextToken)mCurrentToken;
                Advance();

                // Element names are case-insensitive in HTML. The HTML parser converts them to lowercase.
                // The CSS parser should do the same.
                string elementName = StringUtil.AsciiLowerCase(elementNameToken.Text);

                return new CssTypeSelector(parsedNamespace, elementName);
            }
            else
            {
                // Parsing error. Unexpected token.
                return null;
            }
        }

        /// <summary>
        /// Parses sequences "tag", "|tag", and "namespace|tag". Extracts the namespace part of the sequences.
        /// </summary>
        /// <param name="defaultNamespace">The namespace that is returned if the sequence contains no namespace.</param>
        /// <returns>
        /// The extracted namespace part of the sequence. If the sequence contains no namespace, the specified default namespace
        /// is returned. In case of a parsing error, <c>null</c> is returned.
        /// </returns>
        private CssNamespace ParseNamespace(CssNamespace defaultNamespace)
        {
            if (mCurrentToken.IsDelim('*') || (mCurrentToken.Type == CssTokenType.Ident))
            {
                // Choose between two possible token sequences: "namespace|tag" and just "tag".
                // Have to look ahead one token to disambiguate the input.
                // We need to disambiguate between the scenarios where a dash character is a namespace delimeter
                // like in "[ns|tag='abc']" and where it is a part of the "dash-match" operator ("|="), like in "[tag|='abc']".
                bool nextTokenIsNamespaceDelimeter = mNextToken.IsDelim('|') && !TokenAfterNextIsDelim('=');
                if (nextTokenIsNamespaceDelimeter)
                {
                    // namespace|tag
                    CssNamespace result;
                    if (mCurrentToken.Type == CssTokenType.Ident)
                    {
                        string prefix = ((CssTextToken)mCurrentToken).Text;
                        CssNamespace declaredNamespace = mDeclaredCssNamespaces.GetNamespace(prefix);
                        if (declaredNamespace == null)
                        {
                            // Parsing error. The referenced namespace is not declared.
                            return null;
                        }
                        else
                        {
                            result = declaredNamespace;
                        }
                    }
                    else
                    {
                        // The current token is an asterisk.
                        result = CssNamespace.Any;
                    }
                    Advance(); // namespace token
                    Advance(); // '|'
                    return result;
                }
                else
                {
                    // Just tag.
                    return defaultNamespace;
                }
            }
            else if (mCurrentToken.IsDelim('|'))
            {
                Advance(); // '|'
                return CssNamespace.Empty;
            }
            else
            {
                // Parsing error. Unexpected token.
                return null;
            }
        }

        /// <summary>
        /// Advances the parser by one token.
        /// </summary>
        private void Advance()
        {
            if (mTokenIndex < mTokens.Count)
            {
                ++mTokenIndex;
                mCurrentToken = mNextToken;
                int nextTokenIndex = mTokenIndex + 1;
                mNextToken = (nextTokenIndex < mTokens.Count)
                    ? mTokens[nextTokenIndex]
                    : CssToken.Eof;
            }
            else
            {
                Debug.Fail("Attempt to read past the end of a token list.");
            }
        }

        /// <summary>
        /// Verifies that the current token has the specified type, returns the current token, and advances the parser
        /// by one token.
        /// </summary>
        /// <returns>
        /// The token of the specified type or <c>null</c> if the type of the current token is not equal to the specified type.
        /// </returns>
        private CssToken ReadTokenAndAdvance(CssTokenType tokenType)
        {
            CssToken result = mCurrentToken;
            if (result.Type != tokenType)
            {
                return null;
            }
            Advance();
            return result;
        }

        /// <summary>
        /// Advances the parser while the current token is the whitespace token.
        /// </summary>
        private void SkipWhitespace()
        {
            if (mCurrentToken.IsWhitespace())
            {
                Advance();
            }
        }

        /// <summary>
        /// Checks if the current token and the next token form a valid ".class" selector.
        /// </summary>
        private bool CurrentTokenStartsAClass()
        {
            // '.' delimeter followed by an identifier: ".class"
            return mCurrentToken.IsDelim('.') && (mNextToken.Type == CssTokenType.Ident);
        }

        /// <summary>
        /// Checks if the token after the next token is the specified delimeter.
        /// </summary>
        /// <remarks>
        /// We need two token look ahead to disambiguate between "ns|tag=..." and "tag|=...", where "|=" is a match operator.
        /// </remarks>
        private bool TokenAfterNextIsDelim(char delimChar)
        {
            int nextTokenIndex = mTokenIndex + 2;
            if (nextTokenIndex >= mTokens.Count)
            {
                return false;
            }
            CssToken nextToken = mTokens[nextTokenIndex];
            return nextToken.IsDelim(delimChar);
        }

        /// <summary>
        /// List of tokens representing the selector.
        /// </summary>
        private readonly IList<CssToken> mTokens;

        /// <summary>
        /// The index of the token being parsed currently.
        /// </summary>
        private int mTokenIndex;

        /// <summary>
        /// The token currently being parsed.
        /// </summary>
        private CssToken mCurrentToken;

        /// <summary>
        /// The next token to parse.
        /// </summary>
        private CssToken mNextToken;

        /// <summary>
        /// CSS namespace declarations known at the moment.
        /// </summary>
        private readonly CssNamespacePrefixes mDeclaredCssNamespaces;
    }
}
