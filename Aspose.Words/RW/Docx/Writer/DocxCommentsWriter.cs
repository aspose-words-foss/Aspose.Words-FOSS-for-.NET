// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/08/2007 by Vladimir Averkin


using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Instance class for writing comments into the "Comments" package part.
    /// </summary>
    internal class DocxCommentsWriter
    {
        internal DocxCommentsWriter(DocxDocumentWriterBase writer)
        {
            mWriter = writer;

            if (writer.Compliance != OoxmlComplianceCore.Ecma376)
            {
                mBuilderEx = mWriter.CreateChildPartAndBuilder("commentsExtended.xml", DocxContentType.CommentsExtended, writer.RelTypes.CommentsEx);
                mBuilderEx.StartDocumentWithStandardNamespaces("w15:commentsEx");

                if (mWriter.MsWordExtensionsVersion >= MsWordVersionCore.Word2016)
                {
                    mBuilderIds = mWriter.CreateChildPartAndBuilder("commentsIds.xml",
                        DocxContentType.CommentsIds, writer.RelTypes.CommentsIds);
                    mBuilderIds.StartDocumentWithStandardNamespaces("w16cid:commentsIds");
                }

                if (mWriter.MsWordExtensionsVersion >= MsWordVersionCore.Word2019)
                {
                    mBuilderExtensible = mWriter.CreateChildPartAndBuilder("commentsExtensible.xml",
                        DocxContentType.CommentsExtensible, writer.RelTypes.CommentsExtensible);
                    mBuilderExtensible.StartDocumentWithStandardNamespaces("w16cex:commentsExtensible");
                }
            }

            mBuilder = mWriter.CreateChildPartAndBuilder("comments.xml", DocxContentType.Comments, writer.RelTypes.Comments);
            mBuilder.StartDocumentWithStandardNamespaces("w:comments");
        }

        /// <summary>
        /// Writes a w:comment element start into the "Comments" package part.
        /// </summary>
        internal void WriteStart(Comment comment, int annotationId)
        {
            int paraId = GetOrGenerateParaId(comment);

            WriteCommentEx(comment, paraId);
            WriteCommentId(comment, paraId);
            WriteCommentExtensible(comment);

            mBuilder.StartElement("w:comment");
            mBuilder.WriteAttribute("w:id", annotationId);
            mBuilder.WriteAttribute("w:author", comment.Author);

            if (comment.LocalDateTime != DateTime.MinValue)
            {
                // On saving a document, MS Word writes comment local time as is, without conversion to UTC. Do the same.
                mBuilder.WriteAttribute("w:date", comment.LocalDateTime, DateTimeKind.Unspecified);
            }

            mBuilder.WriteAttribute("w:initials", comment.Initial);

            // All the following story nodes will be written to "Comments" part.
            mWriter.PushBuilder(mBuilder);
        }

        /// <summary>
        /// Writes a w:comment element end into the "Comments" package part.
        /// </summary>
        internal void WriteEnd()
        {
            mBuilder.EndElement(); //w:comment

            // Restore current writer's builder to "Main Document" part.
            mWriter.PopBuilder();
        }

        internal void Close()
        {
            mBuilder.EndDocument();

            if (mBuilderEx != null)
                mBuilderEx.EndDocument();

            if (mBuilderIds != null)
                mBuilderIds.EndDocument();

            if (mBuilderExtensible != null)
                mBuilderExtensible.EndDocument();
        }

        private void WriteCommentEx(Comment comment, int paraId)
        {
            if (mBuilderEx == null)
                return;

            mBuilderEx.StartElement("w15:commentEx");

            // Note here is w15 prefix.
            mBuilderEx.WriteAttribute("w15:paraId", NrxXmlUtil.IntToHex(paraId));

            if ((comment.ParentId != Comment.NoParent) && mCommentIdToParaIdMap.ContainsKey(comment.ParentId))
            {
                mBuilderEx.WriteAttribute("w15:paraIdParent",
                    NrxXmlUtil.IntToHex(mCommentIdToParaIdMap[comment.ParentId]));
            }

            mBuilderEx.WriteAttribute("w15:done", comment.Done);

            mBuilderEx.EndElement(); //w15:commentEx
        }

        /// <summary>
        /// Gets ID of the last paragraph of the comment. If the paragraph has no ID, generates it.
        /// </summary>
        private int GetOrGenerateParaId(Comment comment)
        {
            int paraId = comment.LastParagraph.ParaId;

            if (paraId == 0)
                paraId = mWriter.GenerateParaId(comment.LastParagraph.ParentStory.StoryType);

            mWriter.CommentParagraphToParaId.Add(comment.LastParagraph, paraId);
            mCommentIdToParaIdMap.Add(comment.Id, paraId);

            return paraId;
        }

        /// <summary>
        /// Writes a commentId element.
        /// </summary>
        private void WriteCommentId(Comment comment, int paraId)
        {
            if (mBuilderIds == null)
                return;

            mBuilderIds.StartElement("w16cid:commentId");

            mBuilderIds.WriteAttribute("w16cid:paraId", NrxXmlUtil.IntToHex(paraId));

            int durableId = comment.DurableId;
            if (durableId == 0)
                comment.DurableId = GenerateDurableId();

            mBuilderIds.WriteAttribute("w16cid:durableId", NrxXmlUtil.IntToHex(comment.DurableId));

            mBuilderIds.EndElement(); //w16cid:commentId
        }

        /// <summary>
        /// Generates an unique comment durable ID.
        /// </summary>
        private int GenerateDurableId()
        {
            while (true)
            {
                int durableId = RandomUtil.NewGuid().GetHashCode();
                // Let's use positive values.
                durableId &= 0x7fffffff;

                if (durableId == 0)
                    continue;

                if (mWriter.SaveInfo.UsedCommentDurableIds.Contains(durableId))
                    continue;

                mWriter.SaveInfo.UsedCommentDurableIds.Add(durableId);

                return durableId;
            }
        }

        /// <summary>
        /// Writes a commentExtensible element.
        /// </summary>
        private void WriteCommentExtensible(Comment comment)
        {
            if ((mBuilderExtensible == null) || 
                ((comment.DateTimeUtc == DateTime.MinValue) && !comment.IsIntelligentPlaceholder && !comment.HasReactions))
                return;

            mBuilderExtensible.StartElement("w16cex:commentExtensible");

            Debug.Assert(comment.DurableId != 0); // Generated in WriteCommentId
            mBuilderExtensible.WriteAttribute("w16cex:durableId", NrxXmlUtil.IntToHex(comment.DurableId));

            if (comment.DateTimeUtc != DateTime.MinValue)
                mBuilderExtensible.WriteAttribute("w16cex:dateUtc", comment.DateTimeUtc);

            // It seems MS Word does not write 'intelligentPlaceholder' if it is 'false'.
            if (comment.IsIntelligentPlaceholder)
                mBuilderExtensible.WriteAttribute("w16cex:intelligentPlaceholder", comment.DateTimeUtc);

            if (comment.HasReactions)
                WriteCommentReactionExtension(comment.Reactions);

            mBuilderExtensible.EndElement(); //w16cex:commentExtensible
        }

        /// <summary>
        /// Writes an extension containing a 'reactions' element of the CT_CommentReaction complex type (§2.1.1.1 of
        /// [MS-OREACTXML]).
        /// </summary>
        private void WriteCommentReactionExtension(CommentReactionCollection reactions)
        {
            mBuilderExtensible.StartElement("w16cex:extLst");
            mBuilderExtensible.StartElement("w16:ext");

            mBuilderExtensible.WriteAttributeString("w16:uri", DocxCommentsExtensibleReader.ReactionsExtentionUri);

            mBuilderExtensible.StartElement("cr:reactions");
            DocxNamespaces namespaces =
                new DocxNamespaces(mBuilderExtensible.OoxmlCompliance == OoxmlComplianceCore.IsoStrict);
            mBuilderExtensible.WriteAttributeString("xmlns:cr", namespaces.CommentReaction);

            foreach (CommentReaction reaction in reactions)
                WriteCommentReaction(reaction);

            mBuilderExtensible.EndElement(); //cr:reactions
            mBuilderExtensible.EndElement(); //w16:ext
            mBuilderExtensible.EndElement(); //w16cex:extLst
        }

        /// <summary>
        /// Writes a 'reaction' element of the CT_CommentReaction complex type (§2.1.3.1 of [MS-OREACTXML]).
        /// </summary>
        private void WriteCommentReaction(CommentReaction reaction)
        {
            mBuilderExtensible.StartElement("cr:reaction");
            mBuilderExtensible.WriteAttribute("reactionType", reaction.ReactionType);

            foreach (CommentReactionInfo reactionInfo in reaction.ReactionInfos)
                WriteCommentReactionInfo(reactionInfo);

            mBuilderExtensible.EndElement(); //cr:reaction
        }

        /// <summary>
        /// Writes a 'reactionInfo' element of the CT_CommentReactionInfo complex type (§2.1.3.2 of [MS-OREACTXML]).
        /// </summary>
        private void WriteCommentReactionInfo(CommentReactionInfo reactionInfo)
        {
            mBuilderExtensible.StartElement("cr:reactionInfo");

            if (reactionInfo.DateTimeUtc != DateTime.MinValue)
                mBuilderExtensible.WriteAttribute("dateUtc", reactionInfo.DateTimeUtc);

            if (StringUtil.HasChars(reactionInfo.UserId))
            {
                mBuilderExtensible.StartElement("cr:user");

                mBuilderExtensible.WriteAttribute("userId", reactionInfo.UserId);
                mBuilderExtensible.WriteAttribute("userProvider", reactionInfo.UserProvider);
                mBuilderExtensible.WriteAttribute("userName", reactionInfo.UserName);

                mBuilderExtensible.EndElement(); //cr:user
            }

            mBuilderExtensible.EndElement(); //cr:reactionInfo
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxDocumentWriterBase mWriter;
        private readonly DocxBuilder mBuilder;
        private readonly DocxBuilder mBuilderEx;
        private readonly DocxBuilder mBuilderIds;
        private readonly DocxBuilder mBuilderExtensible;

        private readonly IntToIntDictionary mCommentIdToParaIdMap = new IntToIntDictionary();
    }
}
