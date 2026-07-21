// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2015 by Victor Chebotok

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents generated content of a pseudo-element. Corresponds to the 'content' CSS property.
    /// </summary>
    /// <remarks>
    /// Generated content consists of typed parts - strings and functions - that are concatenated on rendering.
    /// For details, see http://www.w3.org/TR/CSS21/generate.html#propdef-content.
    /// </remarks>
    internal class PseudoElementContent
    {
        internal PseudoElementContent(PseudoElementContentPart[] parts)
        {
            Debug.Assert(parts != null);
            mParts = parts;
        }

        /// <summary>
        /// Parses values of the 'content' CSS property.
        /// </summary>
        /// <returns>
        /// Parsed content or <c>null</c> if the value is malformed or unsupported.
        /// </returns>
        internal static PseudoElementContent Parse(CssPropertyValue contentPropertyValue)
        {
            List<PseudoElementContentPart> parts = new List<PseudoElementContentPart>();
            bool contentIsMalformed = false;
            for (int i = 0; i < contentPropertyValue.Count; i++)
            {
                CssValue contentPart = contentPropertyValue[i];
                switch (contentPart.ValueType)
                {
                    case CssValueType.String:
                    {
                        string text = ((CssStringValue)contentPart).Value;
                        parts.Add(new PseudoElementContentText(text));
                        break;
                    }
                    case CssValueType.Function:
                    {
                        CssFunctionValue functionValue = (CssFunctionValue)contentPart;
                        PseudoElementContentPart parsedPart;
                        switch (functionValue.Name)
                        {
                            case "attr":
                                parsedPart = PseudoElementContentAttr.Parse(functionValue);
                                break;
                            case "counter":
                                parsedPart = PseudoElementContentCounter.Parse(functionValue);
                                break;
                            case "counters":
                                parsedPart = PseudoElementContentCounters.Parse(functionValue);
                                break;
                            default:
                                // Other functions are not recognized at the moment.
                                parsedPart = null;
                                break;
                        }
                        if (parsedPart != null)
                        {
                            parts.Add(parsedPart);
                        }
                        else
                        {
                            contentIsMalformed = true;
                        }
                        break;
                    }
                    case CssValueType.Uri:
                    {
                        // At the moment, references to external resources are not supported and they are replaced with
                        // empty strings.
                        parts.Add(new PseudoElementContentText(string.Empty));
                        break;
                    }
                    case CssValueType.Identifier:
                    {
                        bool isQuote = contentPart.Equals(CssValue.OpenQuote) ||
                            contentPart.Equals(CssValue.CloseQuote) ||
                            contentPart.Equals(CssValue.NoOpenQuote) ||
                            contentPart.Equals(CssValue.NoCloseQuote);
                        if (isQuote)
                        {
                            // At the moment, quotes are not supported and they are replaced with empty strings.
                            parts.Add(new PseudoElementContentText(string.Empty));
                        }
                        else
                        {
                            contentIsMalformed = true;
                        }
                        break;
                    }
                    default:
                    {
                        // Other value types are not recognized at the moment.
                        contentIsMalformed = true;
                        break;
                    }
                }

                // According to the specification, we should ignore a 'content' property if we cannot parse it,
                // and, as a result, we should hide the pseudo-element.
                if (contentIsMalformed)
                {
                    parts.Clear();
                    break;
                }
            }

            if (contentIsMalformed || (parts.Count == 0))
            {
                return null;
            }

            return new PseudoElementContent(parts.ToArray());
        }

        /// <summary>
        /// Returns names of all counters used in 'content' CSS property values.
        /// </summary>
        internal string[] GetCounterNames()
        {
            List<string> counterNames = new List<string>();
            foreach (PseudoElementContentPart part in mParts)
            {
                PseudoElementContentCounterFunction partAsCounterFunction = part as PseudoElementContentCounterFunction;
                if (partAsCounterFunction != null)
                {
                    counterNames.Add(partAsCounterFunction.CounterName);
                }
            }
            return counterNames.ToArray();
        }

        internal void Accept(IPseudoElementContentPartVisitor visitor)
        {
            foreach (PseudoElementContentPart part in mParts)
            {
                part.Accept(visitor);
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            bool isFirstPart = true;
            foreach (PseudoElementContentPart part in mParts)
            {
                if (!isFirstPart)
                {
                    text.Append(' ');
                }
                text.Append(part);
                isFirstPart = false;
            }
            return text.ToString();
        }
#endif

        private readonly PseudoElementContentPart[] mParts;
    }
}
