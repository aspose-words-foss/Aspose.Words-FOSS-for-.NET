// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/12/2009 by Dmitry Vorobyev

using System.Text;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Encapsulates some common operations used to build a field code token.
    /// </summary>
    internal class FieldCodeTokenBuilder
    {
        internal FieldCodeTokenBuilder()
        {
            StartPosition = DocumentPosition.Void;
            EndPosition = DocumentPosition.Void;
        }

        /// <summary>
        /// Appends a current character provided by the specified tokenizer to the token text.
        /// </summary>
        /// <param name="tokenizer"></param>
        internal void AppendToken(IFieldCodeTokenizer tokenizer)
        {
            if (!HasContent)
                StartPosition = tokenizer.GetCurrentPosition();

            if (tokenizer.CurrentToken == FieldCodeToken.TextChar)
            {
                Builder.Append(tokenizer.CurrentChar);

                RichBuilder.Append(tokenizer.CurrentChar, ResolveRunFormatting(tokenizer.CurrentNode));
            }
        }

        private static RunPr ResolveRunFormatting(Node node)
        {
            Run run = node as Run;
            return run != null
                ? run.RunPr
                : new RunPr();
        }

        internal void VisitFieldStart(IFieldCodeTokenizer tokenizer)
        {
            Debug.Assert(tokenizer.CurrentToken == FieldCodeToken.ChildFieldStart);
            if (StartPosition.IsVoid)
                mCompleteFieldStartPosition = DocumentPosition.CreatePositionBefore(tokenizer.CurrentNode);
        }

        /// <summary>
        /// Sets the end position to the current tokenizer position.
        /// </summary>
        /// <param name="tokenizer"></param>
        internal void FinalizeToken(IFieldCodeTokenizer tokenizer)
        {
            if (!StartPosition.IsVoid)
                EndPosition = tokenizer.GetCurrentPosition();

            if (!mCompleteFieldStartPosition.IsVoid && tokenizer.CurrentToken == FieldCodeToken.ChildFieldEnd)
                mCompleteFieldEndPosition = DocumentPosition.CreatePositionAfter(tokenizer.CurrentNode);
        }

        internal void AppendChar(char c, RunPr runPr)
        {
            Builder.Append(c);

            if (runPr == null)
                runPr = new RunPr();

            RichBuilder.Append(c, runPr);
        }

        internal void Clear()
        {
            if (mBuilder != null)
            {
                mBuilder.Length = 0;
                mRichBuilder.Clear();
            }

            StartPosition = DocumentPosition.Void;
            EndPosition = DocumentPosition.Void;

            mCompleteFieldStartPosition = DocumentPosition.Void;
            mCompleteFieldEndPosition = DocumentPosition.Void;
        }

        /// <summary>
        /// Appends the content of the specified builder to this builder and clears the source builder.
        /// Also updates the current end point.
        /// </summary>
        /// <param name="builder"></param>
        internal void MergeWith(FieldCodeTokenBuilder builder)
        {
            if (!builder.HasContent)
                return;

            if (!HasContent)
                StartPosition = builder.StartPosition;

            EndPosition = builder.EndPosition;

            Builder.Append(builder.Text);
            for (int i = 0; i < builder.RichText.Length; i++)
                RichBuilder.AppendInternal(builder.RichText.GetInternal(i));

            builder.Clear();
        }

        /// <summary>
        /// Gets the current token's node range.
        /// </summary>
        /// <returns></returns>
        internal NodeRange GetNodeRange()
        {
            return new NodeRange(StartPosition, EndPosition);
        }

        internal NodeRange GetCompleteFieldNodeRange()
        {
            return new NodeRange(mCompleteFieldStartPosition, mCompleteFieldEndPosition);
        }

        /// <summary>
        /// Gets the token's text.
        /// </summary>
        internal string Text
        {
            get { return (mBuilder != null) ? mBuilder.ToString() : string.Empty; }
        }

        /// <summary>
        /// Gets the token's rich text.
        /// </summary>
        internal RichString RichText
        {
            get { return (mRichBuilder != null) ? mRichBuilder.ToRichString() : RichString.Empty; }
        }

        /// <summary>
        /// Gets the length of the token's text.
        /// </summary>
        internal int Length
        {
            get { return (mBuilder != null) ? mBuilder.Length : 0; }
        }

        /// <summary>
        /// True if the builder contains some contents (represented by a text or node range).
        /// </summary>
        internal bool HasContent
        {
            get
            {
                if (Length > 0)
                    return true;

                return (!StartPosition.IsVoid || !EndPosition.IsVoid);
            }
        }

        /// <summary>
        /// Gets the start position of the current token.
        /// </summary>
        internal DocumentPosition StartPosition { get; private set; }

        /// <summary>
        /// Gets the end position of the current token.
        /// </summary>
        internal DocumentPosition EndPosition { get; private set; }

        private StringBuilder Builder
        {
            get { return mBuilder ?? (mBuilder = new StringBuilder()); }
        }

        private RichStringBuilder RichBuilder
        {
            get { return mRichBuilder ?? (mRichBuilder = new RichStringBuilder()); }
        }

        private StringBuilder mBuilder;
        private RichStringBuilder mRichBuilder;

        private DocumentPosition mCompleteFieldStartPosition = DocumentPosition.Void;
        private DocumentPosition mCompleteFieldEndPosition = DocumentPosition.Void;
    }
}
