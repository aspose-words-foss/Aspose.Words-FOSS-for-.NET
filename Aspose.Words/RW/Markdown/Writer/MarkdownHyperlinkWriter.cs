// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/04/2020 by Ilya Navrotskiy

using System;
using Aspose.Words.Fields;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Hyperlinks into markdown.
    /// </summary>
    internal class MarkdownHyperlinkWriter : MarkdownFieldWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownHyperlinkWriter" /> class.
        /// </summary>
        internal MarkdownHyperlinkWriter(FieldStart fieldStart, MarkdownLinkDefinitionWriter linkDefinitionWriter,
            MarkdownWriter writer)
            : base(fieldStart, writer)
        {
            mLinkDefinitionWriter = linkDefinitionWriter;
            mLinkExportMode = writer.SaveOptions.LinkExportMode;
            mFieldType = fieldStart.FieldType;
        }

        internal override void OnFieldSeparator(FieldSeparator fieldSeparator)
        {
            string text = FieldResult.ToString();
            if (!StringUtil.HasChars(text))
                return;

            // NOTE: FieldCodeHyperlink.Parse() cannot understand double quotes and backslashes
            // in the screen tip, if they are not escaped. Backslashes are thrown out.
            // The screen tip is cut at the first unescaped double quote.
            mFieldCodeHyperlink = FieldCodeHyperlink.Parse(text);

            // WORDSNET-25816 The trick with replacing FieldCodeHyperlink SubAddress by FieldRef Address
            // allows us to implement FieldRef support without introducing a new separate field writer.
            // WORDSNET-28104 Added the same trick for FieldPageRef.
            if ((mFieldType == FieldType.FieldRef) || (mFieldType == FieldType.FieldPageRef))
            {
                mFieldCodeHyperlink.SubAddress = mFieldCodeHyperlink.Address;
                mFieldCodeHyperlink.Address = string.Empty;
            }

            mLinkDefinitionWriter.OnHyperlink(mFieldCodeHyperlink);

            base.OnFieldSeparator(fieldSeparator);
        }

        /// <summary>
        /// Gets a string that is ready for writing into markdown.
        /// </summary>
        protected override string Text
        {
            get
            {
                // WORDSNET-26087 There is only a hidden text in FieldCode. We need a resilience for this case.
                if (mFieldCodeHyperlink == null)
                    return "";

                if (IsValidAutolink)
                    return string.Format("{0}{1}{2}",
                        AutolinkInlineBlock.OpeningDelimiter, FieldResult, AutolinkInlineBlock.ClosingDelimiter);

                string displayText = MarkdownUtil.EscapeSquareBrackets(FieldResult.ToString());
                string referenceString = MarkdownLinkDefinitionWriter.GetReferenceString(mFieldCodeHyperlink);
                string target = MarkdownUtil.EscapeMarkupSymbols(mFieldCodeHyperlink.HRef.ToString(false));
                string title = (mFieldCodeHyperlink.ScreenTip.Length > 0)
                    ? string.Format(" \"{0}\"", MarkdownUtil.EscapeDoubleQuotes(mFieldCodeHyperlink.ScreenTip.Replace(ControlChar.LineBreakChar, MarkdownUtil.HardLineBreakSlashChar)))
                    : string.Empty;

                if (StringUtil.HasChars(target) && target.StartsWith("#", StringComparison.InvariantCulture))
                    // WORDSNET-24954 ToLowerInvariant() is a workaround for html import of mixed case links.
                    // See TestExportMixedCaseLinks() for more details.
                    target = target.ToLowerInvariant();

                if (!mLinkDefinitionWriter.HasFullReferenceLinkDefinition(referenceString, mFieldCodeHyperlink.HRef.ToString(false),
                    mFieldCodeHyperlink.ScreenTip))
                {
                    string newReference = string.Format("{0}{1}{2}", LinkTextBlock.OpeningDelimiter,
                        displayText, LinkTextBlock.ClosingDelimiter);
                    mLinkDefinitionWriter.SetReference(newReference, mFieldCodeHyperlink.HRef.ToString(false),
                        mFieldCodeHyperlink.ScreenTip);
                }

                // Exported as Inline if the corresponding option is set or if the reference link could not have been resolved.
                bool isExportAsInline = (mLinkExportMode == MarkdownLinkExportMode.Inline) ||
                                        (!mLinkDefinitionWriter.HasCollapsedLinkDefinition(mFieldCodeHyperlink.HRef.ToString(false),
                                            mFieldCodeHyperlink.ScreenTip));
                // Exported as a shortcut if referenceLabel is empty or matches the display result text of the current field.
                bool isExportAsShortcut = (referenceString == string.Empty) ||
                                          (referenceString.Substring(1, referenceString.Length - 2) == displayText);
                return isExportAsInline
                    ? string.Format("{0}{1}{2}{3}{4}{5}{6}",
                        LinkTextBlock.OpeningDelimiter, displayText, LinkTextBlock.ClosingDelimiter,
                        LinkDestinationBlock.OpeningDelimiter, target, title, LinkDestinationBlock.ClosingDelimiter)
                    : isExportAsShortcut
                        ? string.Format("{0}{1}{2}", LinkTextBlock.OpeningDelimiter, displayText, LinkTextBlock.ClosingDelimiter)
                        : string.Format("{0}{1}{2}{3}", LinkTextBlock.OpeningDelimiter, displayText,
                            LinkTextBlock.ClosingDelimiter, referenceString);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either this hyperlink is a valid Autolink.
        /// </summary>
        private bool IsValidAutolink
        {
            get
            {
                Debug.Assert(mFieldCodeHyperlink != null);

                string displayText = FieldResult.ToString();
                string target = mFieldCodeHyperlink.HRef.ToString(false);

                // To be a valid Autolink, the target must be the same as display text.
                if (string.Equals(target, displayText, StringComparison.Ordinal) ||
                    (
                        // The MailTo prefix can be omitted at the beginning of the target (see TestAutolinkP()).
                        target.StartsWith(MailToPrefix, StringComparison.InvariantCultureIgnoreCase) &&
                        string.Equals(target.Substring(MailToPrefix.Length), displayText, StringComparison.Ordinal)
                    ))
                {
                    return AutolinkInlineBlock.IsValid(target, 0, target.Length - 1);
                }

                return false;
            }
        }

        /// <summary>
        /// FieldCode hyperlink.
        /// </summary>
        private FieldCodeHyperlink mFieldCodeHyperlink;

        private readonly FieldType mFieldType;

        private readonly MarkdownLinkDefinitionWriter mLinkDefinitionWriter;

        private readonly MarkdownLinkExportMode mLinkExportMode;

        private const string MailToPrefix = "mailto:";
    }
}
