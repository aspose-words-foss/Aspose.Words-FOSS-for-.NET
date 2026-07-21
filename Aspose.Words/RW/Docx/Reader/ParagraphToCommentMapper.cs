// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2021 by Mikhail Nepreteamov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Helper class for mapping paragraphs to comments.
    /// </summary>
    internal class ParagraphToCommentMapper
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ParagraphToCommentMapper()
        {
            DurableIdsUsedInComments = new HashSetGeneric<int>();
            ParaIdsUsedInComments = new HashSetGeneric<int>();
            ParaIdToCommentEx = new IntToObjDictionary<CommentEx>();
            ParaIdToCommentExtensible = new Dictionary<int, CommentExtensible>();
            ParaIdToParagraphMap = new IntToObjDictionary<Paragraph>();
        }

        /// <summary>
        /// Applies the stored CommentEx data to the comments.
        /// </summary>
        internal void ApplyCommentExData()
        {
            IntToObjDictionary<CommentEx>.Enumerator enumerator = ParaIdToCommentEx.GetEnumerator();

            while (enumerator.MoveNext())
            {
                int paraId = enumerator.CurrentKey;
                CommentEx commentEx = enumerator.CurrentValue;

                Paragraph para = ParaIdToParagraphMap[paraId];
                if (para == null)
                    continue;

                Comment comment = (Comment)para.GetAncestor(NodeType.Comment);

                // Update Done mark.
                comment.Done = commentEx.Done;

                Paragraph parentPara = ParaIdToParagraphMap[commentEx.ParentParaId];
                if (parentPara == null)
                    continue;

                Comment parentComment = (Comment)parentPara.GetAncestor(NodeType.Comment);

                // Update reference to parent comment.
                comment.ParentId = parentComment.Id;
            }
        }

        /// <summary>
        /// Applies the stored CommentExtensible data to the comments.
        /// </summary>
        internal void ApplyCommentExtensibleData()
        {
            foreach (KeyValuePair<int, CommentExtensible> item in ParaIdToCommentExtensible)
            {
                int paraId = item.Key;
                CommentExtensible extensible = item.Value;

                Paragraph para = ParaIdToParagraphMap[paraId];
                if (para == null)
                    continue;

                Comment comment = (Comment)para.GetAncestor(NodeType.Comment);

                comment.DurableId = extensible.DurableId;
                comment.DateTimeUtc = extensible.UtcDateTime;
                comment.Reactions = extensible.Reactions;
                comment.IsIntelligentPlaceholder = extensible.IsIntelligentPlaceholder;
            }
        }

        /// <summary>
        /// Finds a <see cref="CommentExtensible"/> object stored on reading commentsIds document part.
        /// </summary>
        internal CommentExtensible FindCommentExtensible(int durableId)
        {
            foreach (CommentExtensible extensible in ParaIdToCommentExtensible.Values)
            {
                if (extensible.DurableId == durableId)
                    return extensible;
            }
            return null;
        }

        /// <summary>
        /// Maps the specified ParaId and DurableId.
        /// </summary>
        internal void MapParaIdAndDurableId(int paraId, int durableId)
        {
            int uniqueDurableId = durableId;

            if (DurableIdsUsedInComments.Contains(uniqueDurableId))
                uniqueDurableId = ParaIdToCommentExtensible.ContainsKey(paraId) ? uniqueDurableId : GenerateAndStoreDurableId();
            else
                DurableIdsUsedInComments.Add(uniqueDurableId);

            ParaIdToCommentExtensible[paraId] = new CommentExtensible(paraId, uniqueDurableId);
        }

        /// <summary>
        /// Maps the specified ParaId and paragraph.
        /// </summary>
        internal void MapParaIdAndParagraph(int paraId, Paragraph para)
        {
            if ((paraId != int.MinValue) &&
                (
                    ParaIdToCommentEx.ContainsKey(paraId) ||
                    ParaIdToCommentExtensible.ContainsKey(paraId)
                ))
            {
                ParaIdToParagraphMap.Add(paraId, para);
            }
        }

        /// <summary>
        /// Maps the specified ParaId and ParaId of the parent paragraph.
        /// </summary>
        internal void MapParaIdAndParentParaId(int paraId, int paraIdParent, bool done)
        {
            // WORDSNET-24514 Use last "CommentEx" occurrence from the sequence.
            ParaIdToCommentEx[paraId] = new CommentEx(paraIdParent, done);
        }

        /// <summary>
        /// Maps the specified ParaId and resolves duplicate ParaIds if needed.
        /// </summary>
        internal int MapParaIdAndResolveDuplicateIfNeeded(int paraId)
        {
            if (!ParaIdsUsedInComments.Contains(paraId))
            {
                ParaIdsUsedInComments.Add(paraId);
                return paraId;
            }

            int uniqueParaId = GenerateAndStoreParaId();
            if (ParaIdToCommentEx.ContainsKey(paraId))
            {
                ParaIdToCommentEx.Add(uniqueParaId, new CommentEx(0, false));
                int newDurableId = GenerateAndStoreDurableId();
                ParaIdToCommentExtensible.Add(uniqueParaId, new CommentExtensible(uniqueParaId, newDurableId));
            }
            return uniqueParaId;
        }

        /// <summary>
        /// Generates a unique ParaId and stores it.
        /// </summary>
        private int GenerateAndStoreParaId()
        {
            return GenerateAndStoreIdToStorage(ParaIdsUsedInComments, "ParaId");
        }

        /// <summary>
        /// Generates a unique DurableId and stores it.
        /// </summary>
        private int GenerateAndStoreDurableId()
        {
            return GenerateAndStoreIdToStorage(DurableIdsUsedInComments, "DurableId");
        }

        /// <summary>
        /// Generates a unique identifier and stores it in the specified storage.
        /// </summary>
        private static int GenerateAndStoreIdToStorage(HashSetGeneric<int> storage, string initializer)
        {
            const int mask = 0x7FFFFFFF;

            while (true)
            {
                int id = (RandomUtil.NewGuid(initializer).GetHashCode() & mask);
                if ((id == 0) || storage.Contains(id))
                    continue;

                storage.Add(id);
                return id;
            }
        }

        /// <summary>
        /// ParaIds used in comments.
        /// </summary>
        private HashSetGeneric<int> ParaIdsUsedInComments { get; }

        /// <summary>
        /// DurableIds used in comments.
        /// </summary>
        private HashSetGeneric<int> DurableIdsUsedInComments { get; }

        /// <summary>
        /// Maps ParaId to CommentEx object which holds information about parent paragraph and done mark.
        /// </summary>
        private IntToObjDictionary<CommentEx> ParaIdToCommentEx { get; }

        /// <summary>
        /// Maps ParaId to <see cref="CommentExtensible"/> object which stores additional comment information.
        /// </summary>
        private Dictionary<int, CommentExtensible> ParaIdToCommentExtensible { get; }

        /// <summary>
        /// Maps ParaId to Paragraph node.
        /// </summary>
        /// <remarks>
        /// Currently collected only Ids referred from CommentEx part.
        /// </remarks>
        private IntToObjDictionary<Paragraph> ParaIdToParagraphMap { get; }
    }
}
