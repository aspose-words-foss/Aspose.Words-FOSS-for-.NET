// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Notes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Text of a Word document is stored in stories. <see cref="StoryType"/> identifies a story.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum StoryType
    {
        /// <summary>
        /// Default value. There is no such story in the document.
        /// </summary>
        None     = 0,
        /// <summary>
        /// Contains the main text of the document, represented by <see cref="Aspose.Words.Body"/>.
        /// </summary>
        MainText = 1,
        /// <summary>
        /// Contains footnote text, represented by <see cref="Aspose.Words.Notes.Footnote"/>.
        /// </summary>
        Footnotes = 2,
        /// <summary>
        /// Contains endnotes text, represented by <see cref="Aspose.Words.Notes.Footnote"/>.
        /// </summary>
        Endnotes = 3,
        /// <summary>
        /// Contains document comments (annotations), represented by <see cref="Aspose.Words.Comment"/>.
        /// </summary>
        Comments = 4,
        /// <summary>
        /// Contains shape or textbox text, represented by <see cref="Aspose.Words.Drawing.Shape"/>.
        /// </summary>
        Textbox = 5,
        /// <summary>
        /// Contains text of the even pages header, represented by <see cref="Aspose.Words.HeaderFooter"/>.
        /// </summary>
        EvenPagesHeader = 6,
        /// <summary>
        /// Contains text of the primary header. When header is different for odd and even pages,
        /// contains text of the odd pages header. Represented by <see cref="Aspose.Words.HeaderFooter"/>.
        /// </summary>
        PrimaryHeader = 7,
        /// <summary>
        /// Contains text of the even pages footer, represented by <see cref="Aspose.Words.HeaderFooter"/>.
        /// </summary>
        EvenPagesFooter = 8,
        /// <summary>
        /// Contains text of the primary footer. When footer is different for odd and even pages,
        /// contains text of the odd pages footer. Represented by <see cref="Aspose.Words.HeaderFooter"/>.
        /// </summary>
        PrimaryFooter = 9,
        /// <summary>
        /// Contains text of the first page header, represented by <see cref="Aspose.Words.HeaderFooter"/>.
        /// </summary>
        FirstPageHeader = 10,
        /// <summary>
        /// Contains text of the first page footer, represented by <see cref="Aspose.Words.HeaderFooter"/>.
        /// </summary>
        FirstPageFooter = 11,
        /// <summary>
        /// Contains the text of the footnote separator. <dev>Represented by <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.</dev>
        /// </summary>
        FootnoteSeparator = 12,
        /// <summary>
        /// Contains the text of the footnote continuation separator. <dev>Represented by <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.</dev>
        /// </summary>
        FootnoteContinuationSeparator = 13,
        /// <summary>
        /// Contains the text of the footnote continuation notice separator. <dev>Represented by <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.</dev>
        /// </summary>
        FootnoteContinuationNotice = 14,
        /// <summary>
        /// Contains the text of the endnote separator. <dev>Represented by <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.</dev>
        /// </summary>
        EndnoteSeparator = 15,
        /// <summary>
        /// Contains the text of the endnote continuation separator. <dev>Represented by <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.</dev>
        /// </summary>
        EndnoteContinuationSeparator = 16,
        /// <summary>
        /// Contains the text of the endnote continuation notice separator. <dev>Represented by <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.</dev>
        /// </summary>
        EndnoteContinuationNotice = 17
    }

    internal class StoryTypeHelper
    {
        /// <summary>
        /// Returns uppercased name of given StoryType enum. Should be used instead of enum.ToString()
        /// See https://auckland.dynabic.com/wiki/display/org/Do+not+use+Enum.ToString%28%29+and+Enum.Parse.
        /// </summary>
        public static string ToUpperString(StoryType enumValue)
        {
            switch (enumValue)
            {
                case StoryType.None:
                    return "NONE";
                case StoryType.MainText:
                    return "MAINTEXT";
                case StoryType.Footnotes:
                    return "FOOTNOTES";
                case StoryType.Endnotes:
                    return "ENDNOTES";
                case StoryType.Comments:
                    return "COMMENTS";
                case StoryType.Textbox:
                    return "TEXTBOX";
                case StoryType.EvenPagesHeader:
                    return "EVENPAGESHEADER";
                case StoryType.PrimaryHeader:
                    return "PRIMARYHEADER";
                case StoryType.EvenPagesFooter:
                    return "EVENPAGESFOOTER";
                case StoryType.PrimaryFooter:
                    return "PRIMARYFOOTER";
                case StoryType.FirstPageHeader:
                    return "FIRSTPAGEHEADER";
                case StoryType.FirstPageFooter:
                    return "FIRSTPAGEFOOTER";
                case StoryType.FootnoteSeparator:
                    return "FOOTNOTESEPARATOR";
                case StoryType.FootnoteContinuationSeparator:
                    return "FOOTNOTECONTINUATIONSEPARATOR";
                case StoryType.FootnoteContinuationNotice:
                    return "FOOTNOTECONTINUATIONNOTICE";
                case StoryType.EndnoteSeparator:
                    return "ENDNOTESEPARATOR";
                case StoryType.EndnoteContinuationSeparator:
                    return "ENDNOTECONTINUATIONSEPARATOR";
                case StoryType.EndnoteContinuationNotice:
                    return "ENDNOTECONTINUATIONNOTICE";
                default:
                    throw new ArgumentOutOfRangeException("Unknown StoryType value, please update StoryTypeEnum.ToUpperString().");
            }
        }

        public static bool IsFootnoteEndnoteCommentStory(StoryType storyType)
        {
            switch (storyType)
            {
                case StoryType.Footnotes: case StoryType.Endnotes: case StoryType.Comments:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsHeaderFooterStory(StoryType storyType)
        {
            return IsHeader(storyType) || IsFooter(storyType);
        }

        public static bool IsHeader(StoryType storyType)
        {
            return (
                (storyType == StoryType.EvenPagesHeader) ||
                (storyType == StoryType.FirstPageHeader) ||
                (storyType == StoryType.PrimaryHeader));
        }

        public static bool IsFooter(StoryType storyType)
        {
            return (
                (storyType == StoryType.EvenPagesFooter) ||
                (storyType == StoryType.FirstPageFooter) ||
                (storyType == StoryType.PrimaryFooter));
        }



        public static StoryType FootnoteSeparatorTypeToStoryType(FootnoteSeparatorType type)
        {
            Debug.Assert(type >= FootnoteSeparatorType.FootnoteSeparator
                && type <= FootnoteSeparatorType.EndnoteContinuationNotice);

            // These are binary compatible
            return StoryType.FootnoteSeparator + (int)type;
        }
    }

    /// <summary>
    /// The StoryType stack.
    /// </summary>
    internal class StoryTypeStack
    {
        /// <summary>
        /// Pushes the specified StoryType.
        /// </summary>
        internal void Push(StoryType storyType)
        {
            mStack.Push(storyType);
            HandleInHeaderOrInFooterState(storyType, true);
        }

        /// <summary>
        /// Pops the specified StoryType.
        /// </summary>
        internal void Pop(StoryType storyType)
        {
            Debug.Assert(Current == storyType);
            mStack.Pop();
            HandleInHeaderOrInFooterState(storyType, false);
        }

        private void HandleInHeaderOrInFooterState(StoryType storyType, bool isPush)
        {
            if (((isPush) ? !IsInHeader : IsInHeader) && StoryTypeHelper.IsHeader(storyType))
                IsInHeader = isPush;
            else if (((isPush) ? !IsInFooter : IsInFooter) && StoryTypeHelper.IsFooter(storyType))
                IsInFooter = isPush;
        }

        /// <summary>
        /// Gets the current StoryType.
        /// </summary>
        internal StoryType Current { get { return mStack.Peek(); } }

        /// <summary>
        /// Returns true if the current StoryType is included in the header or footer.
        /// </summary>
        internal bool IsInHeaderOrFooter { get { return (IsInHeader || IsInFooter); } }

        /// <summary>
        /// Returns true if the current StoryType is included in the header.
        /// </summary>
        internal bool IsInHeader { get; private set; }

        /// <summary>
        /// Returns true if the current StoryType is included in the footer.
        /// </summary>
        internal bool IsInFooter { get; private set; }

        private readonly Stack<StoryType> mStack = new Stack<StoryType>();
    }
}
