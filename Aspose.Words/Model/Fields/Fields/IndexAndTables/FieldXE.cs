// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the XE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Defines the text and page number for an index entry, which is used by an INDEX field.
    /// </remarks>
    public class FieldXE : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Returns a value indicating whether this instance of XE field should be processed while an INDEX field result
        /// building.
        /// </summary>
        /// <param name="fieldCode">Cached XE field properties. If not specified then direct values are used.</param>
        internal bool ShouldBeProcessed(FieldCodeXE fieldCode)
        {
            return IsValid() && HasValidText(fieldCode);
        }

        /// <summary>
        /// Returns a value indicating whether this instance of XE field is placed in a valid place of the document.
        /// </summary>
        private bool IsValid()
        {
            Node node = Start;
            while (true) // return inside.
            {
                node = node.GetStoryAncestor(NodeType.Any);
                if (node == null)
                    return false;

                switch (node.NodeType)
                {
                    case NodeType.Shape:
                    case NodeType.GroupShape:
                        // Index entries in shapes are accessed if they are inside bodies.
                        // So get the next one story ancestor.
                        break;
                    case NodeType.Document:
                        // This is the replacement of NodeType.Body. See NodeUtil.IsStoryNodeType() for details.
                        // Index entries inside bodies are accessed.
                        return true;
                    default:
                        // Index entries inside headers, footers, comments or footnotes are not accessed.
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether this instance of XE field has a valid text, i.e. the text that can be
        /// included to an INDEX field result.
        /// </summary>
        /// <param name="fieldCode">Cached XE field properties. If not specified then direct values are used.</param>
        private static bool HasValidText(FieldCodeXE fieldCode)
        {
            // Entries with an empty or whitespace text should not be processed.
            string text = fieldCode.Text;
            if (text == null)
                return false;

            int index = StringUtil.IndexOfNonWhitespace(text);
            if (index == -1)
                return false;

            // Entries with empty subentries of the min level should not be processed.
            return text[index] != IndexEntry.SubentrySeparator;
        }

        /// <summary>
        /// A shortcut method.
        /// </summary>
        private static NodeType GetStoryAncestorType(Node node)
        {
            return node.GetStoryAncestor(NodeType.Any).NodeType;
        }

        /// <summary>
        /// Formats the specified page number according to a page number style specified for the field start's
        /// parent section.
        /// </summary>
        internal string FormatPageNumber(int pageNumber)
        {
            Section formatSection = (Section)Start.GetAncestor(NodeType.Section);
            return NumberConverter.NumberToString(pageNumber, formatSection.PageSetup.PageNumberStyle, false);
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsBoldSwitch:
                case IsItalicSwitch:
                    return FieldSwitchType.Flag;
                case EntryTypeSwitch:
                case PageRangeBookmarkNameSwitch:
                case PageNumberReplacementSwitch:
                case YomiSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the text of the entry.
        /// </summary>
        public string Text
        {
            get { return FieldCodeCache.GetArgumentAsString(TextArgumentIndex); }
            set { FieldCodeCache.SetArgument(TextArgumentIndex, value); }
        }

        /// <summary>
        /// Gets a node range of the text argument.
        /// </summary>
        internal NodeRange TextRange
        {
            get { return FieldCodeCache.GetArgumentRange(TextArgumentIndex); }
        }

        /// <summary>
        /// Gets or sets whether to apply bold formatting to the entry's page number.
        /// </summary>
        public bool IsBold
        {
            get { return FieldCodeCache.HasSwitch(IsBoldSwitch); }
            set { FieldCodeCache.SetSwitch(IsBoldSwitch, value); }
        }

        /// <summary>
        /// Gets or sets an index entry type.
        /// </summary>
        public string EntryType
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryTypeSwitch); }
            set { FieldCodeCache.SetSwitch(EntryTypeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to apply italic formatting to the entry's page number.
        /// </summary>
        public bool IsItalic
        {
            get { return FieldCodeCache.HasSwitch(IsItalicSwitch); }
            set { FieldCodeCache.SetSwitch(IsItalicSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark that marks a range of pages that is inserted as the entry's page number.
        /// </summary>
        public string PageRangeBookmarkName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageRangeBookmarkNameSwitch); }
            set { FieldCodeCache.SetSwitch(PageRangeBookmarkNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets text used in place of a page number.
        /// </summary>
        public string PageNumberReplacement
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageNumberReplacementSwitch); }
            set { FieldCodeCache.SetSwitch(PageNumberReplacementSwitch, value); }
        }

        /// <summary>
        /// Gets a node range of the text argument to be used in place of a page number.
        /// </summary>
        internal NodeRange PageNumberReplacementRange
        {
            get { return FieldCodeCache.GetSwitchArgumentRange(PageNumberReplacementSwitch); }
        }

        /// <summary>
        /// Gets or sets the yomi (first phonetic character for sorting indexes) for the index entry
        /// </summary>
        public string Yomi
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(YomiSwitch); }
            set { FieldCodeCache.SetSwitch(YomiSwitch, value); }
        }

        internal const int TextArgumentIndex = 0;

        private const string IsBoldSwitch = "\\b";
        private const string EntryTypeSwitch = "\\f";
        private const string IsItalicSwitch = "\\i";
        private const string PageRangeBookmarkNameSwitch = "\\r";
        private const string PageNumberReplacementSwitch = "\\t";
        private const string YomiSwitch = "\\y";
    }
}
