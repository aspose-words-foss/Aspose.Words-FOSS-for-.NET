// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/02/2025 by Victor Chebotok

using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS selector as seen by MS Word. Allows to parse selectors into parts and the check the parts
    /// against specified patterns.
    /// </summary>
    internal class MsoHtmlSelector
    {
        internal MsoHtmlSelector(
            string parent,
            string tag,
            string id,
            string className,
            string pseudo)
        {
            Parent = parent;
            Tag = tag;
            Id = id;
            ClassName = className;
            Pseudo = pseudo;
        }

        /// <summary>
        /// Parses a CSS selector into parts as MS Word does.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that MS Word supports only a subset of CSS selectors supported by Aspose.Words.
        /// </para>
        /// <para>
        /// <paramref name="selectorCss"/> must be a valid CSS selector text and it must not contain whitespace (except for
        /// one space character per descendant combinator).
        /// </para>
        /// </remarks>
        /// <param name="selectorCss">Selector's CSS text.</param>
        /// <returns>A parsed selector. If CSS cannot be parsed by MS Word, an empty selector is returned.</returns>
        internal static MsoHtmlSelector Parse(string selectorCss)
        {
            int stop = selectorCss.Length;

            string parent = string.Empty;

            // Extract the last part (innermost selector) of a descendant selector. For example, the selector "div p span" will
            // be split into two parts: "div p" and "span". The innermost part is parsed further.
            int lastSpaceIndex = selectorCss.LastIndexOf(' ');
            if (lastSpaceIndex > 0)
            {
                if (selectorCss.IndexOfAny(gCombinators, 0, lastSpaceIndex) >= 0)
                {
                    return new MsoHtmlSelector(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                }

                parent = selectorCss.Substring(0, lastSpaceIndex);
                selectorCss = selectorCss.Substring(lastSpaceIndex + 1);
                stop = selectorCss.Length;
            }

            // From the MS Word's perspective, a CSS selector looks like this:
            // tag#id.class:pseudo
            // Any of the parts can be missing. For example, "tag", "tag#id", "#id.class", ":pseudo", "tag:pseudo" selectors
            // are all recognized.
            // CSS is eagerly parsed from right to left, one part at a time.

            // Extract the pseudo part of the selector (either pseudo-class or pseudo-element).
            // For example, "hover" from "tag#id.class:hover".
            string pseudo = ParseLastPart(selectorCss, stop, ':');
            if (pseudo.Length > 0)
            {
                stop -= pseudo.Length + 1;
            }

            // Extract the class part of the selector. For example, "class" from "tag#id.class".
            // Note that the pseudo part has been extracted already.
            string className = ParseLastPart(selectorCss, stop, '.');
            if (className.Length > 0)
            {
                stop -= className.Length + 1;
            }

            // Extract the id part of the selector. For example, "id" from "tag#id".
            string id = ParseLastPart(selectorCss, stop, '#');
            if (id.Length > 0)
            {
                stop -= id.Length + 1;
            }

            // The remaining part (if any) is the tag.
            string tag = string.Empty;
            if (stop >= 0)
            {
                tag = selectorCss.Substring(0, stop);
            }

            // MS Word converts class names to lower case.
            className = className.ToLowerInvariant();

            return new MsoHtmlSelector(parent, tag, id, className, pseudo);
        }

        /// <summary>
        /// Returns a value indicating whether this selector is supported by MS Word in HTML documents.
        /// </summary>
        internal bool IsSupportedByMsWord()
        {
            // Empty selectors (or selectors that cannot be parsed by MS Word) are not supported.
            if (IsEmpty())
            {
                return false;
            }

            // MS Word supports only a limited list of element names in selectors.
            if (StringUtil.HasChars(Tag) && !MsoHtmlUtil.IsElementSupportedByMsWord(Tag))
            {
                return false;
            }

            // MS Word doesn't support multi-ID selectors like "#id1#id2".
            if (StringUtil.HasChars(Id) && (Id.IndexOf('#') >= 0))
            {
                return false;
            }

            // MS Word doesn't support multi-class selectors like ".class1.class2". It also doesn't support mixed class and ID
            // selectors like ".class#id".
            if (StringUtil.HasChars(ClassName) &&
                ((ClassName.IndexOf('.') >= 0) || (ClassName.IndexOf('#') >= 0)))
            {
                return false;
            }

            if (StringUtil.HasChars(Pseudo))
            {
                // MS Word supports a limited list of pseudo-elements and pseudo-classes selectors.
                if (ArrayUtil.BinarySearch(gPseudoSupportedByMsWord, Pseudo) < 0)
                {
                    return false;
                }

                // Bare ":active" selectors are not supported. There must be a tag, an id, or a class specified: "em:active",
                // "#id:active", or ".class:active".
                if ((Pseudo == "active") &&
                    (StringUtil.HasChars(Tag) || StringUtil.HasChars(Id) || StringUtil.HasChars(ClassName)))
                {
                    return false;
                }

                // For "link" and "visited" pseudo-classes, only the following two cases are supported:
                // - bare pseudo-class, like ":link";
                // - an anchor element with a pseudo-class, like "a:link".
                if (((Pseudo == "link") || (Pseudo == "visited")) && StringUtil.HasChars(Tag) && (Tag != "a"))
                {
                    return false;
                }
            }

            // MS Word doesn't support selectors that have both ID and class specified. For example, "#id.class".
            if (StringUtil.HasChars(Id) && StringUtil.HasChars(ClassName))
            {
                return false;
            }

            return true;
        }

        internal bool IsEmpty()
        {
            return HasParts(false, false, false, false);
        }

        internal bool IsTagOnly(string tag)
        {
            return HasParts(true, false, false, false) && (Tag == tag);
        }

        internal bool IsTagAndClassAndAnyPseudo(
            string tag,
            string className)
        {
            return HasParts(true, false, true, true) && (Tag == tag) && (ClassName == className);
        }

        internal bool IsClassOnly(string className)
        {
            return HasParts(false, false, true, false) && (ClassName == className);
        }

        internal bool IsClassAndAnyPseudo(string className)
        {
            return HasParts(false, false, true, true) && (ClassName == className);
        }

        internal bool IsPseudoOnly(string pseudo)
        {
            return HasParts(false, false, false, true) && (Pseudo == pseudo);
        }

        internal bool IsTagAndClass(
            string tag,
            string className)
        {
            return HasParts(true, false, true, false) &&
                (Tag == tag) &&
                (ClassName == className);
        }

        internal bool IsTagAndAnyClass(string tag)
        {
            return HasParts(true, false, true, false) && (Tag == tag);
        }

        internal bool IsTagAndPseudo(
            string tag,
            string pseudo)
        {
            return HasParts(true, false, false, true) && (Tag == tag) && (Pseudo == pseudo);
        }

        internal bool IsTagAndAnything(
            string tag,
            bool allowClass)
        {
            if (!allowClass && HasPart(ClassName, true))
            {
                return false;
            }
            return (Tag == tag) &&
                (HasPart(Id, true) || HasPart(ClassName, true) || HasPart(Pseudo, true));
        }

        internal string Parent { get; }

        internal string Tag { get; }

        internal string Id { get; }

        internal string ClassName { get; }

        internal string Pseudo { get; }

        /// <summary>
        /// Eagerly parses the rightmost part of the selector.
        /// </summary>
        /// <remarks>
        /// The parsed part extends from the leftmost occurence of the specified <paramref name="starter"/> character
        /// up to the specified <paramref name="stop"/> position.
        /// If no <paramref name="starter"/> character is found, an empty string is returned.
        /// </remarks>
        private static string ParseLastPart(
            string selectorCss,
            int stop,
            char starter)
        {
            int position = selectorCss.IndexOf(starter);
            if ((position >= 0) && (position < stop))
            {
                return selectorCss.Substring(position + 1, stop - position - 1);
            }
            return string.Empty;
        }

        private bool HasParts(
            bool hasTag,
            bool hasId,
            bool hasClass,
            bool hasPseudo)
        {
            return HasPart(Tag, hasTag) &&
                HasPart(Id, hasId) &&
                HasPart(ClassName, hasClass) &&
                HasPart(Pseudo, hasPseudo);
        }

        private static bool HasPart(
            string partValue,
            bool isExpected)
        {
            return StringUtil.HasChars(partValue) == isExpected;
        }

        /// <summary>
        /// List of pseudo-element and pseudo-class names supported by MS Word.
        /// </summary>
        /// <remarks>
        /// The list is sorted for binary search to work.
        /// </remarks>
        private static readonly string[] gPseudoSupportedByMsWord = new string[]
        {
            "active", "first-letter", "first-line", "focus", "hover", "link", "visited"
        };

        /// <summary>
        /// Selector combinators that are supported by Aspose.Words but not supported by MS Word.
        /// </summary>
        private static readonly char[] gCombinators = new char[] { '>', '+', '~' };
    }
}
